using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
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
        public IQCReturnToVendorController(ILoggerManager logger, IMapper mapper, IConfiguration config, IGrinRepository grinRepository, IHttpClientFactory clientFactory,
            IIQCConfirmationRepository iQCConfirmationRepository, IIQCReturnToVendorRepository repository)
        {
            _logger = logger;
            _mapper = mapper;
            _config = config;
            _grinrepository = grinRepository;
            _iQCConfirmationRepository = iQCConfirmationRepository;
            _clientFactory = clientFactory;
            _repository = repository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateIQCReturnToVendor([FromBody] IQCReturnToVendorPostDto iQCRejectRecoveryPostDto)
        {
            ServiceResponse<List<PurchaseOrderReturns>> serviceResponse = new ServiceResponse<List<PurchaseOrderReturns>>();
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

                //foreach (var item in inventoryObject) 
                //{
                //    var grinitem = Grindetails.GrinParts.Where(x => x.Id == item.GrinPartId).FirstOrDefault();
                //    var project = grinitem.ProjectNumbers.Where(x => x.ProjectNumber == item.ProjectNumber).FirstOrDefault();
                //    project.RejectReturnQty = item.Balance_Quantity;

                //}

                //List<PurchaseOrderReturns> purchaseOrderReturns = new List<PurchaseOrderReturns>();

                //foreach (var items in iQCRejectRecoveryPostDto.iQCReturnToVendorItemsPostDtos)
                //{
                //    var existingPO = purchaseOrderReturns.FirstOrDefault(q => q.PurchaseOrderNo == items.PONumber);

                //    if (existingPO != null)
                //    {
                //        if (existingPO.purchaseOrderItems == null)
                //        {
                //            existingPO.purchaseOrderItems = new List<PurchaseOrderItems>();
                //        }

                //        existingPO.purchaseOrderItems.Add(new PurchaseOrderItems
                //        {
                //            ItemNumber = items.ItemNumber,
                //            ReturnQty = items.ReturnQty
                //        });
                //    }
                //    else
                //    {
                //        var newPurchaseOrderReturns = new PurchaseOrderReturns
                //        {
                //            PurchaseOrderNo = items.PONumber,
                //            purchaseOrderItems = new List<PurchaseOrderItems>
                //             {
                //                new PurchaseOrderItems
                //                   {
                //                      ItemNumber = items.ItemNumber,
                //                      ReturnQty = items.ReturnQty
                //                   }
                //              }
                //        };

                //        purchaseOrderReturns.Add(newPurchaseOrderReturns);
                //    }
                //}
                //if (purchaseOrderReturns.Count < 1)
                //{
                //    _logger.LogError($"In CreateIQCReturnToVendor: no PurchaseOrders were not Found for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber}");
                //    serviceResponse.Data = null;
                //    serviceResponse.Message = $"In CreateIQCReturnToVendor: no PurchaseOrders were not Found for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber}";
                //    serviceResponse.Success = false;
                //    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                //    return NotFound(serviceResponse);
                //}
                //// _logger.LogError($"Some PurchaseOrders were not found");


                ////var PoList = iQCRejectRecoveryPostDto.iQCReturnToVendorItemsPostDtos.Select(x => x.PONumber).Distinct().ToList();
                //var POchanges = JsonConvert.SerializeObject(purchaseOrderReturns);
                //var data = new StringContent(POchanges, Encoding.UTF8, "application/json");
                //var request2 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["PurchaseService"],
                // $"PurchaseOrder/ReturnToVendorPOs"))
                //{
                //    Content = data
                //};
                //request2.Headers.Add("Authorization", token1);
                //var responce = await client1.SendAsync(request2);
                //if (responce.StatusCode != HttpStatusCode.OK)
                //{
                //    _logger.LogError($"In CreateIQCReturnToVendor: There was an Error in ReturnToVendorPOs API Call for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber}");
                //    serviceResponse.Data = null;
                //    serviceResponse.Message = $"In CreateIQCReturnToVendor: There was an Error in ReturnToVendorPOs API Call for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber}";
                //    serviceResponse.Success = false;
                //    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                //    return NotFound(serviceResponse);
                //}
                //var PODetailsObjectString = await responce.Content.ReadAsStringAsync();
                //var PODetailsObjectData = JsonConvert.DeserializeObject<PurchaseOrderDtoDetails>(PODetailsObjectString);
                //var PODetailsObject = PODetailsObjectData.data;

                //List<PurchaseOrderReturnItemsBackDto> purchaseOrderReturnItemsBackDtos = PODetailsObject.SelectMany(dto=>dto.purchaseOrderItems).GroupBy(item=>item.ItemNumber)
                //    .Select(group=>new PurchaseOrderReturnItemsBackDto {       
                //                ItemNumber = group.Key,
                //                ReturnQty = group.Sum(item => item.ReturnQty),
                //                purchaseOrderReturnProjectBackDtos = group
                //                .SelectMany(item => item.purchaseOrderReturnProjectBackDtos)
                //                .GroupBy(project => project.ProjectNo)
                //                .Select(projectGroup => new PurchaseOrderReturnProjectBackDto
                //                {
                //                    ProjectNo = projectGroup.Key,
                //                    ReturnQty = projectGroup.Sum(project => project.ReturnQty)
                //                })
                //                .ToList()
                //     })
                //    .ToList();



                ////  List<PurchaseOrderDetails> POsUpdated = new List<PurchaseOrderDetails>();
                //foreach (var item in purchaseOrderReturnItemsBackDtos)
                //{
                //    var invitem = inventoryObject.Where(x => x.PartNumber == item.ItemNumber).ToList();

                //    foreach (var project in item.purchaseOrderReturnProjectBackDtos)
                //    {
                //        var invitemProject = invitem.Where(y => y.ProjectNumber == project.ProjectNo).ToList();
                //        var removingQty = project.ReturnQty;
                //        foreach (var prjInv in invitemProject)
                //        {
                //            if (prjInv.Balance_Quantity > removingQty)
                //            {
                //                prjInv.Balance_Quantity -= removingQty;
                //                removingQty = 0;
                //            }
                //            else if (prjInv.Balance_Quantity <= removingQty)
                //            {
                //                removingQty -= prjInv.Balance_Quantity;
                //                prjInv.Balance_Quantity = 0;
                //                prjInv.IsStockAvailable = false;
                //            }
                //            var updateInv = _mapper.Map<InventoryUpdateDto>(prjInv);
                //            var json = JsonConvert.SerializeObject(updateInv);
                //            var data1 = new StringContent(json, Encoding.UTF8, "application/json");
                //            var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["WarehouseService"],
                //            "Inventory/UpdateInventory?id=", prjInv.Id))
                //            {
                //                Content = data1
                //            };
                //            request5.Headers.Add("Authorization", token1);

                //            var response = await client1.SendAsync(request5);
                //            if (response.StatusCode != HttpStatusCode.OK)
                //            {
                //                _logger.LogError($"In CreateIQCReturnToVendor: An Error Occured in UpdateInventory for InvID: {prjInv.Id} for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} and ItemNumber: {item.ItemNumber}, ProjectNo: {project.ProjectNo}");
                //                serviceResponse.Data = null;
                //                serviceResponse.Message = $"In CreateIQCReturnToVendor: An Error Occured in UpdateInventory for InvID: {prjInv.Id} for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} and ItemNumber: {item.ItemNumber}, ProjectNo: {project.ProjectNo}";
                //                serviceResponse.Success = false;
                //                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                //                return StatusCode(500, serviceResponse);
                //            }
                //            IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                //            iqcInventoryTranctionDtos.PartNumber = prjInv.PartNumber;
                //            iqcInventoryTranctionDtos.LotNumber = prjInv.LotNumber;
                //            iqcInventoryTranctionDtos.MftrPartNumber = prjInv.MftrPartNumber;
                //            iqcInventoryTranctionDtos.Description = prjInv.Description;
                //            iqcInventoryTranctionDtos.ProjectNumber = prjInv.ProjectNumber;
                //            iqcInventoryTranctionDtos.Issued_Quantity = prjInv.Balance_Quantity;
                //            iqcInventoryTranctionDtos.UOM = prjInv.UOM;
                //            iqcInventoryTranctionDtos.Warehouse = prjInv.Warehouse;
                //            iqcInventoryTranctionDtos.From_Location = prjInv.Location;
                //            iqcInventoryTranctionDtos.TO_Location = prjInv.Location;
                //            iqcInventoryTranctionDtos.GrinNo = prjInv.GrinNo; ;
                //            iqcInventoryTranctionDtos.GrinPartId = prjInv.GrinPartId;
                //            iqcInventoryTranctionDtos.PartType = prjInv.PartType;
                //            iqcInventoryTranctionDtos.ReferenceID = prjInv.GrinNo;
                //            iqcInventoryTranctionDtos.ReferenceIDFrom = "IQCRejectRecovery";
                //            iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
                //            iqcInventoryTranctionDtos.ShopOrderNo = "";
                //            string iqcInventoryTranctionDtoJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDtos);
                //            var contents1 = new StringContent(iqcInventoryTranctionDtoJsons, Encoding.UTF8, "application/json");
                //            var request8 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["WarehouseService"],
                //            "InventoryTranction/CreateInventoryTranction"))
                //            {
                //                Content = contents1
                //            };
                //            request8.Headers.Add("Authorization", token1);

                //            var inventoryTransResponses1 = await client1.SendAsync(request8);
                //            if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK)
                //            {
                //                _logger.LogError($"In CreateIQCReturnToVendor: An Error Occured in CreateInventoryTranction for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} and ItemNumber: {item.ItemNumber}, ProjectNo: {project.ProjectNo}");
                //                serviceResponse.Data = null;
                //                serviceResponse.Message = $"In CreateIQCReturnToVendor: An Error Occured in CreateInventoryTranction for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} and ItemNumber: {item.ItemNumber}, ProjectNo: {project.ProjectNo}";
                //                serviceResponse.Success = false;
                //                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                //                return StatusCode(500, serviceResponse);
                //            }
                //            if (removingQty == 0) break;
                //        }
                //    }
                //}
                //foreach (var po in PODetailsObject)
                //{
                //    foreach (var Item in po.purchaseOrderItems)
                //    {
                //        var GrinItem = Grindetails.GrinParts.Where(x => x.ItemNumber == Item.ItemNumber && x.PONumber == po.PurchaseOrderNo).FirstOrDefault();
                //        var IqcItem = Iqcdetails.IQCConfirmationItems.Where(x => x.GrinPartId == GrinItem.Id).FirstOrDefault();
                //        IqcItem.RejectReturnQty = Item.ReturnQty;
                //        GrinItem.RejectReturnQty = Item.ReturnQty;
                //        foreach (var prj in Item.purchaseOrderReturnProjectBackDtos)
                //        {
                //            var Project = GrinItem.ProjectNumbers.Where(x => x.ProjectNumber == prj.ProjectNo).FirstOrDefault();
                //            Project.RejectReturnQty = prj.ReturnQty;
                //        }
                //    }
                //}

                //var IQCReturnToVendor = _mapper.Map<IQCReturnToVendor>(iQCRejectRecoveryPostDto);
                //IQCReturnToVendor.iQCReturnToVendorItems = _mapper.Map<List<IQCReturnToVendorItems>>(iQCRejectRecoveryPostDto.iQCReturnToVendorItemsPostDtos);

                //await _repository.CreateIQCReturnToVendor(IQCReturnToVendor);
                //await _iQCConfirmationRepository.UpdateIqcDetails(Iqcdetails);
                //await _grinrepository.UpdateGrin_ForTally(Grindetails);

                //_grinrepository.SaveAsync();
                //_iQCConfirmationRepository.SaveAsync();
                //_repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = $"CreateIQCReturnToVendor was Successfull";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"In CreateIQCReturnToVendor Error occored for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber}: \n" +ex.Message + "\n" + ex.InnerException);
                serviceResponse.Data = null;
                serviceResponse.Message = $"In CreateIQCReturnToVendor Error occored for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
