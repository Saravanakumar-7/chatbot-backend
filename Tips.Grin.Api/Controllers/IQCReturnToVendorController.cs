using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Repository;

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class IQCReturnToVendorController : ControllerBase
    {
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IGrinRepository _grinrepository;
        private IIQCConfirmationRepository _iQCConfirmationRepository;
        private IIQCReturnToVendorRepository _repository;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public IQCReturnToVendorController(IHttpContextAccessor httpContextAccessor, ILoggerManager logger, IMapper mapper, IConfiguration config, IGrinRepository grinRepository, IHttpClientFactory clientFactory,
            IIQCConfirmationRepository iQCConfirmationRepository, IIQCReturnToVendorRepository repository)
        {
            _logger = logger;
            _mapper = mapper;
            _config = config;
            _grinrepository = grinRepository;
            _iQCConfirmationRepository = iQCConfirmationRepository;
            _clientFactory = clientFactory;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        [HttpPost]
        public async Task<IActionResult> CreateIQCReturnToVendor([FromBody] IQCReturnToVendorPostDto iQCRejectRecoveryPostDto)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                if (iQCRejectRecoveryPostDto == null)
                {
                    _logger.LogError("IQCRejectRecovery details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCConfirmation details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid IQCRejectRecovery details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var Grindetails = await _grinrepository.GetGrinById(iQCRejectRecoveryPostDto.GrinId);
                if (Grindetails == null)
                {
                    _logger.LogError($"GrinId: {iQCRejectRecoveryPostDto.GrinId} Does not Exist");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GrinId: {iQCRejectRecoveryPostDto.GrinId} Does not Exist";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var Iqcdetails = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(iQCRejectRecoveryPostDto.GrinNumber);
                if (Iqcdetails == null)
                {
                    _logger.LogError($"In CreateIQCReturnToVendor: IQC for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} Does not Exist");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"In CreateIQCReturnToVendor: IQC for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} Does not Exist";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var client1 = _clientFactory.CreateClient();
                var token1 = HttpContext.Request.Headers["Authorization"].ToString();
                var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["WarehouseService"],
                $"Inventory/GetRejectInventorybyGrinNo?GrinNo={iQCRejectRecoveryPostDto.GrinNumber}"));
                request1.Headers.Add("Authorization", token1);
                var inventoryObjectResult = await client1.SendAsync(request1);
                if (inventoryObjectResult.StatusCode != HttpStatusCode.OK)
                {
                    _logger.LogError($"In CreateIQCReturnToVendor: No Rejects Found in Inventory for the Grin Number: {iQCRejectRecoveryPostDto.GrinNumber}");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"In CreateIQCReturnToVendor: No Rejects Found in Inventory for the Grin Number: {iQCRejectRecoveryPostDto.GrinNumber}";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                var inventoryObjectData = JsonConvert.DeserializeObject<InventoryDtoDetails>(inventoryObjectString);
                var inventoryObject = inventoryObjectData.data;

                var purchaseOrders = new List<PurchaseOrderReturnsBackDto>();
                foreach (var Item in iQCRejectRecoveryPostDto.iQCReturnToVendorItemsPostDtos)
                {
                    var returnqty = Item.ReturnQty;
                    foreach (var Inv in inventoryObject.Where(x => x.GrinPartId == Item.GrinPartId).ToList())
                    {
                        if (returnqty >= Inv.Balance_Quantity)
                        {
                            var grinitem = Grindetails.GrinParts.Where(x => x.Id == Inv.GrinPartId).FirstOrDefault();
                            var iqcitem = Iqcdetails.IQCConfirmationItems.Where(x => x.GrinPartId == Inv.GrinPartId).FirstOrDefault();
                            var project = grinitem.ProjectNumbers.Where(x => x.ProjectNumber == Inv.ProjectNumber).FirstOrDefault();
                            grinitem.RejectReturnQty += Inv.Balance_Quantity;
                            iqcitem.RejectReturnQty += Inv.Balance_Quantity;
                            project.RejectReturnQty = Inv.Balance_Quantity;

                            var existingPurchaseOrder = purchaseOrders.FirstOrDefault(po => po.PurchaseOrderNo == grinitem.PONumber);

                            if (existingPurchaseOrder == null)
                            {
                                existingPurchaseOrder = new PurchaseOrderReturnsBackDto
                                {
                                    PurchaseOrderNo = grinitem.PONumber,
                                    purchaseOrderItems = new List<PurchaseOrderReturnItemsBackDto>()
                                };
                                purchaseOrders.Add(existingPurchaseOrder);
                            }

                            var existingItem = existingPurchaseOrder.purchaseOrderItems
                                .FirstOrDefault(item => item.ItemNumber == grinitem.ItemNumber);

                            if (existingItem == null)
                            {
                                existingItem = new PurchaseOrderReturnItemsBackDto
                                {
                                    ItemNumber = grinitem.ItemNumber,
                                    ReturnQty = Inv.Balance_Quantity,
                                    purchaseOrderReturnProjectBackDtos = new List<PurchaseOrderReturnProjectBackDto>()
                                };
                                existingPurchaseOrder.purchaseOrderItems.Add(existingItem);
                            }
                            else
                            {
                                existingItem.ReturnQty += Inv.Balance_Quantity;
                            }

                            var existingProject = existingItem.purchaseOrderReturnProjectBackDtos
                                .FirstOrDefault(proj => proj.ProjectNo == project.ProjectNumber);

                            if (existingProject == null)
                            {
                                existingItem.purchaseOrderReturnProjectBackDtos.Add(new PurchaseOrderReturnProjectBackDto
                                {
                                    ProjectNo = project.ProjectNumber,
                                    ReturnQty = Inv.Balance_Quantity
                                });
                            }
                            else
                            {
                                existingProject.ReturnQty += Inv.Balance_Quantity;
                            }
                            returnqty -= Inv.Balance_Quantity;
                            Inv.Balance_Quantity = 0;
                            var updateInv = _mapper.Map<InventoryUpdateDto>(Inv);
                            var json = JsonConvert.SerializeObject(updateInv);
                            var data1 = new StringContent(json, Encoding.UTF8, "application/json");
                            var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["WarehouseService"],
                            "Inventory/UpdateInventory?id=", Inv.Id))
                            {
                                Content = data1
                            };
                            request5.Headers.Add("Authorization", token1);

                            var response = await client1.SendAsync(request5);
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                _logger.LogError($"In CreateIQCReturnToVendor: An Error Occured in UpdateInventory for InvID: {Inv.Id} for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} and ItemNumber: {Inv.PartNumber}, ProjectNo: {Inv.ProjectNumber}");
                                serviceResponse.Data = null;
                                serviceResponse.Message = $"In CreateIQCReturnToVendor: An Error Occured in UpdateInventory for InvID: {Inv.Id} for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} and ItemNumber: {Inv.PartNumber}, ProjectNo: {Inv.ProjectNumber}";
                                serviceResponse.Success = false;
                                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                                return StatusCode(500, serviceResponse);
                            }

                            IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                            iqcInventoryTranctionDtos.PartNumber = Inv.PartNumber;
                            iqcInventoryTranctionDtos.LotNumber = Inv.LotNumber;
                            iqcInventoryTranctionDtos.MftrPartNumber = Inv.MftrPartNumber;
                            iqcInventoryTranctionDtos.Description = Inv.Description;
                            iqcInventoryTranctionDtos.ProjectNumber = Inv.ProjectNumber;
                            iqcInventoryTranctionDtos.Issued_Quantity = Inv.Balance_Quantity;
                            iqcInventoryTranctionDtos.IsStockAvailable = Inv.IsStockAvailable;
                            iqcInventoryTranctionDtos.UOM = Inv.UOM;
                            iqcInventoryTranctionDtos.Warehouse = Inv.Warehouse;
                            iqcInventoryTranctionDtos.From_Location = Inv.Location;
                            iqcInventoryTranctionDtos.TO_Location = Inv.Location;
                            iqcInventoryTranctionDtos.GrinNo = Inv.GrinNo; ;
                            iqcInventoryTranctionDtos.GrinPartId = Inv.GrinPartId;
                            iqcInventoryTranctionDtos.PartType = Inv.PartType;
                            iqcInventoryTranctionDtos.ReferenceID = Inv.GrinNo;
                            iqcInventoryTranctionDtos.ReferenceIDFrom = "IQCRejectRecovery";
                            iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
                            iqcInventoryTranctionDtos.shopOrderNo = "";
                            iqcInventoryTranctionDtos.Remarks = "RTV Done";

                            string iqcInventoryTranctionDtoJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDtos);
                            var contents1 = new StringContent(iqcInventoryTranctionDtoJsons, Encoding.UTF8, "application/json");
                            var request8 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["WarehouseService"],
                            "InventoryTranction/CreateInventoryTranction"))
                            {
                                Content = contents1
                            };
                            request8.Headers.Add("Authorization", token1);

                            var inventoryTransResponses1 = await client1.SendAsync(request8);
                            if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK)
                            {
                                _logger.LogError($"In CreateIQCReturnToVendor: An Error Occured in CreateInventoryTranction for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} and ItemNumber: {Inv.PartNumber}, ProjectNo: {project.ProjectNumber}");
                                serviceResponse.Data = null;
                                serviceResponse.Message = $"In CreateIQCReturnToVendor: An Error Occured in CreateInventoryTranction for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} and ItemNumber: {Inv.PartNumber}, ProjectNo: {project.ProjectNumber}";
                                serviceResponse.Success = false;
                                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                                return StatusCode(500, serviceResponse);
                            }
                        }
                        else
                        {
                            var grinitem = Grindetails.GrinParts.Where(x => x.Id == Inv.GrinPartId).FirstOrDefault();
                            var iqcitem = Iqcdetails.IQCConfirmationItems.Where(x => x.GrinPartId == Inv.GrinPartId).FirstOrDefault();
                            var project = grinitem.ProjectNumbers.Where(x => x.ProjectNumber == Inv.ProjectNumber).FirstOrDefault();
                            grinitem.RejectReturnQty += returnqty;
                            iqcitem.RejectReturnQty += returnqty;
                            project.RejectReturnQty = returnqty;

                            var existingPurchaseOrder = purchaseOrders.FirstOrDefault(po => po.PurchaseOrderNo == grinitem.PONumber);

                            if (existingPurchaseOrder == null)
                            {
                                existingPurchaseOrder = new PurchaseOrderReturnsBackDto
                                {
                                    PurchaseOrderNo = grinitem.PONumber,
                                    purchaseOrderItems = new List<PurchaseOrderReturnItemsBackDto>()
                                };
                                purchaseOrders.Add(existingPurchaseOrder);
                            }

                            var existingItem = existingPurchaseOrder.purchaseOrderItems
                                .FirstOrDefault(item => item.ItemNumber == grinitem.ItemNumber);

                            if (existingItem == null)
                            {
                                existingItem = new PurchaseOrderReturnItemsBackDto
                                {
                                    ItemNumber = grinitem.ItemNumber,
                                    ReturnQty = returnqty,
                                    purchaseOrderReturnProjectBackDtos = new List<PurchaseOrderReturnProjectBackDto>()
                                };
                                existingPurchaseOrder.purchaseOrderItems.Add(existingItem);
                            }
                            else
                            {
                                existingItem.ReturnQty += returnqty;
                            }

                            var existingProject = existingItem.purchaseOrderReturnProjectBackDtos
                                .FirstOrDefault(proj => proj.ProjectNo == project.ProjectNumber);

                            if (existingProject == null)
                            {
                                existingItem.purchaseOrderReturnProjectBackDtos.Add(new PurchaseOrderReturnProjectBackDto
                                {
                                    ProjectNo = project.ProjectNumber,
                                    ReturnQty = returnqty
                                });
                            }
                            else
                            {
                                existingProject.ReturnQty += returnqty;
                            }
                            Inv.Balance_Quantity -= returnqty;
                            returnqty = 0;
                            var updateInv = _mapper.Map<InventoryUpdateDto>(Inv);
                            var json = JsonConvert.SerializeObject(updateInv);
                            var data1 = new StringContent(json, Encoding.UTF8, "application/json");
                            var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["WarehouseService"],
                            "Inventory/UpdateInventory?id=", Inv.Id))
                            {
                                Content = data1
                            };
                            request5.Headers.Add("Authorization", token1);

                            var response = await client1.SendAsync(request5);
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                _logger.LogError($"In CreateIQCReturnToVendor: An Error Occured in UpdateInventory for InvID: {Inv.Id} for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} and ItemNumber: {Inv.PartNumber}, ProjectNo: {Inv.ProjectNumber}");
                                serviceResponse.Data = null;
                                serviceResponse.Message = $"In CreateIQCReturnToVendor: An Error Occured in UpdateInventory for InvID: {Inv.Id} for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} and ItemNumber: {Inv.PartNumber}, ProjectNo: {Inv.ProjectNumber}";
                                serviceResponse.Success = false;
                                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                                return StatusCode(500, serviceResponse);
                            }
                            IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                            iqcInventoryTranctionDtos.PartNumber = Inv.PartNumber;
                            iqcInventoryTranctionDtos.LotNumber = Inv.LotNumber;
                            iqcInventoryTranctionDtos.MftrPartNumber = Inv.MftrPartNumber;
                            iqcInventoryTranctionDtos.Description = Inv.Description;
                            iqcInventoryTranctionDtos.ProjectNumber = Inv.ProjectNumber;
                            iqcInventoryTranctionDtos.Issued_Quantity = returnqty; 
                            iqcInventoryTranctionDtos.IsStockAvailable = Inv.IsStockAvailable;
                            iqcInventoryTranctionDtos.UOM = Inv.UOM;
                            iqcInventoryTranctionDtos.Warehouse = Inv.Warehouse;
                            iqcInventoryTranctionDtos.From_Location = Inv.Location;
                            iqcInventoryTranctionDtos.TO_Location = Inv.Location;
                            iqcInventoryTranctionDtos.GrinNo = Inv.GrinNo; ;
                            iqcInventoryTranctionDtos.GrinPartId = Inv.GrinPartId;
                            iqcInventoryTranctionDtos.PartType = Inv.PartType;
                            iqcInventoryTranctionDtos.ReferenceID = Inv.GrinNo;
                            iqcInventoryTranctionDtos.ReferenceIDFrom = "IQCRejectRecovery";
                            iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
                            iqcInventoryTranctionDtos.shopOrderNo = "";
                            iqcInventoryTranctionDtos.Remarks = "RTV Done";

                            string iqcInventoryTranctionDtoJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDtos);
                            var contents1 = new StringContent(iqcInventoryTranctionDtoJsons, Encoding.UTF8, "application/json");
                            var request8 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["WarehouseService"],
                            "InventoryTranction/CreateInventoryTranction"))
                            {
                                Content = contents1
                            };
                            request8.Headers.Add("Authorization", token1);

                            var inventoryTransResponses1 = await client1.SendAsync(request8);
                            if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK)
                            {
                                _logger.LogError($"In CreateIQCReturnToVendor: An Error Occured in CreateInventoryTranction for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} and ItemNumber: {Inv.PartNumber}, ProjectNo: {project.ProjectNumber}");
                                serviceResponse.Data = null;
                                serviceResponse.Message = $"In CreateIQCReturnToVendor: An Error Occured in CreateInventoryTranction for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} and ItemNumber: {Inv.PartNumber}, ProjectNo: {project.ProjectNumber}";
                                serviceResponse.Success = false;
                                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                                return StatusCode(500, serviceResponse);
                            }


                        }
                        if (returnqty == 0) break;
                    }

                }

                var POchanges = JsonConvert.SerializeObject(purchaseOrders);
                var data = new StringContent(POchanges, Encoding.UTF8, "application/json");
                var request2 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["PurchaseService"],
                 $"PurchaseOrder/ReturnToVendorPOs"))
                {
                    Content = data
                };
                request2.Headers.Add("Authorization", token1);
                var responce = await client1.SendAsync(request2);
                if (responce.StatusCode != HttpStatusCode.OK)
                {
                    _logger.LogError($"In CreateIQCReturnToVendor: There was an Error in ReturnToVendorPOs API Call for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber}");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"In CreateIQCReturnToVendor: There was an Error in ReturnToVendorPOs API Call for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber}";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var IQCReturnToVendor = _mapper.Map<IQCReturnToVendor>(iQCRejectRecoveryPostDto);
                IQCReturnToVendor.iQCReturnToVendorItems = _mapper.Map<List<IQCReturnToVendorItems>>(iQCRejectRecoveryPostDto.iQCReturnToVendorItemsPostDtos);

                await _repository.CreateIQCReturnToVendor(IQCReturnToVendor);
                await _iQCConfirmationRepository.UpdateIqcDetails(Iqcdetails);
                await _grinrepository.UpdateGrin_ForTally(Grindetails);

                _grinrepository.SaveAsync();
                _iQCConfirmationRepository.SaveAsync();
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = $"CreateIQCReturnToVendor was Successfull";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"In CreateIQCReturnToVendor Error occored for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber}: \n" + ex.Message + "\n" + ex.InnerException);
                serviceResponse.Data = null;
                serviceResponse.Message = $"In CreateIQCReturnToVendor Error occored for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllIQCReturnToVendor([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<IQCReturnToVendorDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCReturnToVendorDto>>();

            try
            {
                var GetallIQCReturnToVendor = await _repository.GetAllIQCReturnToVendor(pagingParameter, searchParams);

                if (GetallIQCReturnToVendor == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCReturnToVendor data not found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"IQCReturnToVendor data not found in db");
                    return NotFound(serviceResponse);
                }
                var metadata = new
                {
                    GetallIQCReturnToVendor.TotalCount,
                    GetallIQCReturnToVendor.PageSize,
                    GetallIQCReturnToVendor.CurrentPage,
                    GetallIQCReturnToVendor.HasNext,
                    GetallIQCReturnToVendor.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all IQCReturnToVendor");
                var result = _mapper.Map<IEnumerable<IQCReturnToVendorDto>>(GetallIQCReturnToVendor);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all IQCReturnToVendorDto Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetAllIQCReturnToVendor: {ex.Message} \n {ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Internal server error in GetAllIQCReturnToVendor";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIQCReturnToVendorById(int id)
        {
            ServiceResponse<IQCReturnToVendorDto> serviceResponse = new ServiceResponse<IQCReturnToVendorDto>();

            try
            {
                var IQCReturnToVendorDetailsbyId = await _repository.GetIQCReturnToVendorById(id);

                if (IQCReturnToVendorDetailsbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCReturnToVendor with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"IQCReturnToVendor with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned GetIQCReturnToVendorById with id: {id}");
                    IQCReturnToVendorDto iqcReturnToVendorDto = _mapper.Map<IQCReturnToVendorDto>(IQCReturnToVendorDetailsbyId);
                    serviceResponse.Data = iqcReturnToVendorDto;
                    serviceResponse.Message = $"Returned IQCReturnToVendor with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetIQCReturnToVendorById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
