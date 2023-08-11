using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.Enums;
using Tips.Production.Api.Repository;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Entities.Migrations;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using Mysqlx.Crud;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MaterialRequestsController : ControllerBase
    {

        private IMaterialRequestsRepository _materialRequestRepository;
        private IMapper _mapper;
        private ILoggerManager _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;


        public MaterialRequestsController(IConfiguration config, HttpClient httpClient,IMaterialRequestsRepository materialRequestRepository, IMapper mapper, ILoggerManager logger)
        {
            _materialRequestRepository = materialRequestRepository;
            _mapper = mapper;
            _logger = logger;
            _httpClient = httpClient;
            _config = config;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMaterialRequest([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParammes)
        {
            ServiceResponse<IEnumerable<MaterialRequestsDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestsDto>>();

            try
            {
                var materialRequestDetails = await _materialRequestRepository.GetAllMaterialRequest(pagingParameter, searchParammes);
                var metadata = new
                {
                    materialRequestDetails.TotalCount,
                    materialRequestDetails.PageSize,
                    materialRequestDetails.CurrentPage,
                    materialRequestDetails.HasNext,
                    materialRequestDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogError("Returned all listOfmaterials");
                var result = _mapper.Map<IEnumerable<MaterialRequestsDto>>(materialRequestDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialRequest Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllMROpenStatus([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParammes)
        {
            ServiceResponse<IEnumerable<MaterialRequestsDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestsDto>>();

            try
            {
                var materialRequestDetails = await _materialRequestRepository.GetAllMRStatusOpen(pagingParameter, searchParammes);

                var metadata = new
                {
                    materialRequestDetails.TotalCount,
                    materialRequestDetails.PageSize,
                    materialRequestDetails.CurrentPage,
                    materialRequestDetails.HasNext,
                    materialRequestDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogError("Returned all getAllMaterialRequestStatusOpen");
                var result = _mapper.Map<IEnumerable<MaterialRequestsDto>>(materialRequestDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialRequestStatusOpen Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllMRCloseStatus()
        {
            ServiceResponse<IEnumerable<MaterialRequestsDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestsDto>>();

            try
            {
                var materialRequestDetails = await _materialRequestRepository.GetAllMRStatusClose();

                _logger.LogError("Returned all getAllMaterialRequestStatusClose");
                var result = _mapper.Map<IEnumerable<MaterialRequestsDto>>(materialRequestDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialRequestStatusClose Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterialRequestById(int id)
        {
            ServiceResponse<MaterialRequestsDto> serviceResponse = new ServiceResponse<MaterialRequestsDto>();

            try
            {
                var materialRequestDetails = await _materialRequestRepository.GetMaterialRequestById(id);

                if (materialRequestDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"materialrequest with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"materialrequest with id: {id}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogError($"Returned materialrequest with id: {id}");

                    MaterialRequestsDto materialRequestDto = _mapper.Map<MaterialRequestsDto>(materialRequestDetails);


                    List<MaterialRequestItemsDto> materialRequestItemDtos = new List<MaterialRequestItemsDto>();

                    if (materialRequestDetails.MaterialRequestItems != null)
                    {

                        foreach (var materialitemDetails in materialRequestDetails.MaterialRequestItems)
                        {
                            MaterialRequestItemsDto materialRequestItemDto = _mapper.Map<MaterialRequestItemsDto>(materialitemDetails);
                            materialRequestItemDto.MRStockDetails = _mapper.Map<List<MRStockDetailsDto>>(materialitemDetails.MRStockDetails);

                            materialRequestItemDtos.Add(materialRequestItemDto);
                        }
                    }

                    materialRequestDto.MaterialRequestItems = materialRequestItemDtos;
                    serviceResponse.Data = materialRequestDto;
                    serviceResponse.Message = $"Returned materialrequest with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetmaterialRequestById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetMaterialReqByShopOrderNumber(string ShopOrderNo)
        {
            ServiceResponse<MaterialRequestsDto> serviceResponse = new ServiceResponse<MaterialRequestsDto>();

            try
            {
                var getSObySONo = await _materialRequestRepository.GetMaterialReqByShopOrderNumber(ShopOrderNo);

                if (getSObySONo == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Get MaterialRequest Details By ShopOrderNo with id: {ShopOrderNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Get MaterialRequest Details By ShopOrderNo with shoporderNo: {ShopOrderNo}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Get MaterialRequest Details By ShopOrderNo with shoporderNo: {ShopOrderNo}");

                    MaterialRequestsDto materialRequestDto = _mapper.Map<MaterialRequestsDto>(getSObySONo);

                    List<MaterialRequestItemsDto> materialRequestItemDtos = new List<MaterialRequestItemsDto>();
                    foreach (var materialReqbyMRNo in getSObySONo.MaterialRequestItems)
                    {
                        MaterialRequestItemsDto materialRequestItemDto = _mapper.Map<MaterialRequestItemsDto>(materialReqbyMRNo);
                        materialRequestItemDto.MRStockDetails = _mapper.Map<List<MRStockDetailsDto>>(materialReqbyMRNo.MRStockDetails);

                        materialRequestItemDtos.Add(materialRequestItemDto);
                    }
                    materialRequestDto.MaterialRequestItems = materialRequestItemDtos;
                    serviceResponse.Data = materialRequestDto;
                    serviceResponse.Message = $"Returned Get MaterialRequest Details By ShopOrderNo with ShopOrderNo: {ShopOrderNo}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMRDetailsByShopOrderNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateMaterialRequest([FromBody] MaterialRequestsPostDto materialRequestPostDto)
        {
            ServiceResponse<MaterialRequestsDto> serviceResponse = new ServiceResponse<MaterialRequestsDto>();

            try
            {
                if (materialRequestPostDto is null)
                {
                    _logger.LogError("MaterialRequest object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "MaterialRequest object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid MaterialRequest object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid MaterialRequest object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var createMaterialReq = _mapper.Map<MaterialRequests>(materialRequestPostDto);
                var materialReqDto = materialRequestPostDto.MaterialRequestItems;

                var materialReqItemList = new List<MaterialRequestItems>();

                if (materialReqDto != null)
                {

                    for (int i = 0; i < materialReqDto.Count; i++)
                    {
                        MaterialRequestItems materialItemListDetail = _mapper.Map<MaterialRequestItems>(materialReqDto[i]);
                        materialItemListDetail.MRStockDetails = _mapper.Map<List<MRStockDetails>>(materialReqDto[i].MRStockDetails);
                        materialReqItemList.Add(materialItemListDetail);

                    }
                }

                createMaterialReq.MaterialRequestItems = materialReqItemList;
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));



                //var newcount = await _materialRequestRepository.GetMRNumberAutoIncrementCount(date);

                //if (newcount > 0)
                //{
                //    var number = newcount + 1;
                //    string e = String.Format("{0:D4}", number);
                //    createMaterialReq.MRNumber = days + months + years + "MR" + (e);
                //}
                //else
                //{
                //    var count = 1;
                //    var e = count.ToString("D4");
                //    createMaterialReq.MRNumber = days + months + years + "MR" + (e);
                //}

                var dateFormat = days + months + years;
                var mrNumber = await _materialRequestRepository.GenerateMRNumber();
                createMaterialReq.MRNumber = mrNumber;

                await _materialRequestRepository.CreateMaterialRequest(createMaterialReq);

                _materialRequestRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialRequest Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetMaterialRequestById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateMaterialRequest action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterialRequest(int id, [FromBody] MaterialRequestUpdateDto materialRequestUpdateDto)
        {
             ServiceResponse<MaterialRequestUpdateDto> serviceResponse = new ServiceResponse<MaterialRequestUpdateDto>();

            try
            {
                if (materialRequestUpdateDto is null)
                {
                    _logger.LogError("MaterialRequest object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update MaterialRequest object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid MaterialRequest object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update MaterialRequest object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var getMaterialRequest = await _materialRequestRepository.GetMaterialRequestById(id);
                if (getMaterialRequest is null)
                {
                    _logger.LogError($"materialReq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update materialReq with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var updateMaterialReqquest = _mapper.Map<MaterialRequests>(getMaterialRequest);

                var materialReqItemDto = materialRequestUpdateDto.MaterialRequestItems;

                var materialReqItemList = new List<MaterialRequestItems>();

                for (int i = 0; i < materialReqItemDto.Count; i++)
                {
                    MaterialRequestItems materialItemDetail = _mapper.Map<MaterialRequestItems>(materialReqItemDto[i]);
                    materialItemDetail.MRStockDetails = _mapper.Map<List<MRStockDetails>>(materialReqItemDto[i].MRStockDetails);

                    materialReqItemList.Add(materialItemDetail);

                }


                updateMaterialReqquest.MaterialRequestItems = materialReqItemList;
                var updateMaterialReq = _mapper.Map(materialRequestUpdateDto, getMaterialRequest);
               // updateMaterialReq.MaterialRequestItems = materialReqItemList;
                string result = await _materialRequestRepository.UpdateMaterialRequest(updateMaterialReq);
                _materialRequestRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "materialReq Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateMaterialRequest action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        


        [HttpPut("{id}")]
        public async Task<IActionResult> IssueMaterialRequest(int id, [FromBody] MaterialRequestUpdateDto materialRequestUpdateDto)
        {
            ServiceResponse<MaterialRequestUpdateDto> serviceResponse = new ServiceResponse<MaterialRequestUpdateDto>();


            try
            {
                if (materialRequestUpdateDto is null)
                {
                    _logger.LogError("MaterialRequest object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update MaterialRequest object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid MaterialRequest object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update MaterialRequest object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var getMaterialRequest = await _materialRequestRepository.GetMaterialRequestById(id);
                if (getMaterialRequest is null)
                {
                    _logger.LogError($"materialReq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update materialReq with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var updateMaterialReqquest = _mapper.Map<MaterialRequests>(getMaterialRequest);

                var materialReqItemDto = materialRequestUpdateDto.MaterialRequestItems;

                var materialReqItemList = new List<MaterialRequestItems>();
                var shopOrderNumber = materialRequestUpdateDto.ShopOrderNumber;
                var projectNo = materialRequestUpdateDto.ProjectNumber;

                List<InventoryDtoForMaterialRequest> inventoryDtos = new List<InventoryDtoForMaterialRequest>();

                
                for (int i = 0; i < materialReqItemDto.Count; i++)
                {
                    MaterialRequestItems materialItemDetail = _mapper.Map<MaterialRequestItems>(materialReqItemDto[i]);
                    var mrStockDetails = _mapper.Map<List<MRStockDetails>>(materialReqItemDto[i].MRStockDetails);
                    materialItemDetail.MRStockDetails = mrStockDetails;
                    materialItemDetail.IssuedQty = mrStockDetails.Select(x=> x.Qty).Sum();
                    materialReqItemList.Add(materialItemDetail);
                    //add material request data to somaterialissue tracker
                    InventoryDtoForMaterialRequest inventoryDtoForMaterialRequest = new InventoryDtoForMaterialRequest
                    {
                        PartNumber = materialItemDetail.PartNumber,
                        ProjectNumber = projectNo,
                        DataFrom = "MR",
                        IssueQty = materialItemDetail.IssuedQty,
                        ShopOrderNumber = shopOrderNumber
                    };
                    inventoryDtos.Add(inventoryDtoForMaterialRequest);

                }

                var jsons = JsonConvert.SerializeObject(inventoryDtos);
                var datas = new StringContent(jsons, Encoding.UTF8, "application/json");
                var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateMaterialRequestOnSOMaterialIssueTracker"), datas);

                //InventoryDtoForMaterialRequest inventoryDtoForMaterialRequest = new InventoryDtoForMaterialRequest();
                //inventoryDtoForMaterialRequest.PartNumber = materialItemDetail.PartNumber;
                //inventoryDtoForMaterialRequest.ProjectNumber = projectNo;
                //inventoryDtoForMaterialRequest.DataFrom = "MR";
                //inventoryDtoForMaterialRequest.IssueQty = materialItemDetail.IssuedQty;
                //inventoryDtoForMaterialRequest.ShopOrderNumber = shopOrderNumber;
                //var json1 = JsonConvert.SerializeObject(inventoryDtoForMaterialRequest);

                //var data1 = new StringContent(json1, Encoding.UTF8, "application/json");
                //var response1 = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "UpdateInventoryOnMaterialIssue"), data1);

                //var materialRequestDetails = _mapper.Map<List<UpdateInventoryBalanceQty>>(materialReqItemList);

                //var json = JsonConvert.SerializeObject(materialRequestDetails);
                //var data = new StringContent(json, Encoding.UTF8, "application/json");
                //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "MaterialInventoryBalanceQty"), data);

                var mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<MaterialRequestItems, UpdateInventoryBalanceQty>()
                        .ForMember(dest => dest.PartNumber, opt => opt.MapFrom(src => src.PartNumber))
                        .ForMember(dest => dest.MRNWarehouseList, opt => opt.MapFrom(src => src.MRStockDetails.Select(detail => new InventoryUpdateDtoForMRWarehouse
                        {
                            Warehouse = detail.Warehouse,
                            Location = detail.Location,
                            LocationStock = detail.LocationStock,
                            Qty = detail.Qty,
                        }).ToList()));
                });

                var mapper = mapperConfiguration.CreateMapper();
                var materialRequestDetails = materialReqItemList.Select(item => mapper.Map<UpdateInventoryBalanceQty>(item)).ToList();
                
                var json = JsonConvert.SerializeObject(materialRequestDetails);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "MaterialInventoryBalanceQty"), data);


                updateMaterialReqquest.MaterialRequestItems = materialReqItemList;
                var updateMaterialReq = _mapper.Map(materialRequestUpdateDto, getMaterialRequest);
                // updateMaterialReq.MaterialRequestItems = materialReqItemList;
                updateMaterialReqquest.MrStatus = MaterialStatus.close;
                string result = await _materialRequestRepository.UpdateMaterialRequest(updateMaterialReq);
                _materialRequestRepository.SaveAsync();




                //update balance qty and Return qty in Inventory table


                //JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                //{
                //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                //};
                //string json = JsonConvert.SerializeObject(updateMaterialReqquest);


                //var data = new StringContent(json, Encoding.UTF8, "application/json");
                //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "UpdateInventoryForMR"), data);

                serviceResponse.Data = null;
                serviceResponse.Message = "materialReq Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateMaterialRequest action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialRequest(int id)
        {
            ServiceResponse<MaterialRequestsDto> serviceResponse = new ServiceResponse<MaterialRequestsDto>();

            try
            {
                var MaterialReqDetails = await _materialRequestRepository.GetMaterialRequestById(id);
                if (MaterialReqDetails == null)
                {
                    _logger.LogError($"materialReq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete materialReq with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _materialRequestRepository.DeleteMaterialRequest(MaterialReqDetails);
                _logger.LogError(result);
                _materialRequestRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialReq Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteMaterialRequest action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOpenMRIdNoList()
        {
            ServiceResponse<IEnumerable<MaterialRequestIdNoDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestIdNoDto>>();
            try
            {
                var getAllMaterialReq = await _materialRequestRepository.GetAllOpenMRIdNoList();
                var result = _mapper.Map<IEnumerable<MaterialRequestIdNoDto>>(getAllMaterialReq);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All MaterialRequests";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{MRNumber}")]
        public async Task<IActionResult> GetMaterialReqByMRNumber(string MRNumber)
        {
            ServiceResponse<MaterialRequestsDto> serviceResponse = new ServiceResponse<MaterialRequestsDto>();

            try
            {
                var getMRbyMRNo = await _materialRequestRepository.GetMaterialReqByMRNumber(MRNumber);

                if (getMRbyMRNo == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GetMRNoDetailsById with id: {MRNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GetMRNoDetailsById with id: {MRNumber}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned GetMRNoDetailsById with id: {MRNumber}");

                    MaterialRequestsDto materialRequestDto = _mapper.Map<MaterialRequestsDto>(getMRbyMRNo);

                    List<MaterialRequestItemsDto> materialRequestItemDtos = new List<MaterialRequestItemsDto>();
                    foreach (var materialReqbyMRNo in getMRbyMRNo.MaterialRequestItems)
                    {
                        MaterialRequestItemsDto materialRequestItemDto = _mapper.Map<MaterialRequestItemsDto>(materialReqbyMRNo);
                        materialRequestItemDto.MRStockDetails = _mapper.Map<List<MRStockDetailsDto>>(materialReqbyMRNo.MRStockDetails);

                        materialRequestItemDtos.Add(materialRequestItemDto);
                    }
                    materialRequestDto.MaterialRequestItems = materialRequestItemDtos;
                    serviceResponse.Data = materialRequestDto;
                    serviceResponse.Message = $"Returned GetMRNoDetailsById with id: {MRNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMRNoDetailsById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchMaterialRequests([FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<MaterialRequestsReportDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestsReportDto>>();
            try
            {
                var MaterialRequestsDetails = await _materialRequestRepository.SearchMaterialRequests(searchParams);
                _logger.LogInfo("Returned all MaterialRequestsDetails");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<MaterialRequestsDto, MaterialRequests>().ReverseMap()
                //    .ForMember(dest => dest.MaterialRequestItems, opt => opt.MapFrom(src => src.MaterialRequestItems));
                //});
                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<MaterialRequests, MaterialRequestsReportDto>()
                       .ForMember(dest => dest.MaterialRequestItems, opt => opt.MapFrom(src => src.MaterialRequestItems
                       .Select(materialRequestItems => new MaterialRequestItemsReportDto
                       {
                           Id = materialRequestItems.Id,
                           MRNumber = src.MRNumber,
                           PartNumber = materialRequestItems.PartNumber,
                           PartDescription = materialRequestItems.PartDescription,
                           PartType = materialRequestItems.PartType,
                           Stock = materialRequestItems.Stock,
                           IssuedQty = materialRequestItems.IssuedQty,
                           IssueStatus = materialRequestItems.IssueStatus,
                           RequiredQty = materialRequestItems.RequiredQty,
                           MRStockDetails = materialRequestItems.MRStockDetails
                           .Select(mrStockDetails => new MRStockDetailsDto
                           {
                               Id = mrStockDetails.Id,
                               Warehouse = mrStockDetails.Warehouse,
                               Location = mrStockDetails.Location,
                               Qty = mrStockDetails.Qty,
                               LocationStock = mrStockDetails.LocationStock,
                           }).ToList()
                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<MaterialRequestsReportDto>>(MaterialRequestsDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialRequestsDetails";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAllMaterialRequestsWithItems([FromBody] MaterialRequestSearchDto materialRequestSearch)
        {
            ServiceResponse<IEnumerable<MaterialRequestsReportDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestsReportDto>>();
            try
            {
                var materialRequestDetails = await _materialRequestRepository.GetAllMaterialRequestsWithItems(materialRequestSearch);

                _logger.LogInfo("Returned all materialRequestDetails");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<MaterialRequestsDto, MaterialRequests>().ReverseMap()
                //    .ForMember(dest => dest.MaterialRequestItems, opt => opt.MapFrom(src => src.MaterialRequestItems));
                //});
                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<MaterialRequests, MaterialRequestsReportDto>()
                       .ForMember(dest => dest.MaterialRequestItems, opt => opt.MapFrom(src => src.MaterialRequestItems
                       .Select(materialRequestItems => new MaterialRequestItemsReportDto
                       {
                           Id = materialRequestItems.Id,
                           MRNumber = src.MRNumber,
                           PartNumber = materialRequestItems.PartNumber,
                           PartDescription = materialRequestItems.PartDescription,
                           PartType = materialRequestItems.PartType,
                           Stock = materialRequestItems.Stock,
                           IssuedQty = materialRequestItems.IssuedQty,
                           IssueStatus = materialRequestItems.IssueStatus,
                           RequiredQty = materialRequestItems.RequiredQty,
                           MRStockDetails = materialRequestItems.MRStockDetails
                           .Select(mrStockDetails => new MRStockDetailsDto
                           {
                               Id = mrStockDetails.Id,
                               Warehouse = mrStockDetails.Warehouse,
                               Location = mrStockDetails.Location,
                               Qty = mrStockDetails.Qty,
                               LocationStock = mrStockDetails.LocationStock,
                           }).ToList()
                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<MaterialRequestsReportDto>>(materialRequestDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialRequestsDetails";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> SearchMaterialRequestsDate([FromQuery] SearchDateparames searchDateParam)
        {
            ServiceResponse<IEnumerable<MaterialRequestsReportDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestsReportDto>>();
            try
            {
                var materialRequestDetails = await _materialRequestRepository.SearchMaterialRequestsDate(searchDateParam);

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<MaterialRequests, MaterialRequestsReportDto>()
                       .ForMember(dest => dest.MaterialRequestItems, opt => opt.MapFrom(src => src.MaterialRequestItems
                       .Select(materialRequestItems => new MaterialRequestItemsReportDto
                       {
                           Id = materialRequestItems.Id,
                           MRNumber = src.MRNumber,
                           PartNumber = materialRequestItems.PartNumber,
                           PartDescription = materialRequestItems.PartDescription,
                           PartType = materialRequestItems.PartType,
                           Stock = materialRequestItems.Stock,
                           IssuedQty = materialRequestItems.IssuedQty,
                           IssueStatus = materialRequestItems.IssueStatus,
                           RequiredQty = materialRequestItems.RequiredQty,
                           MRStockDetails = materialRequestItems.MRStockDetails
                           .Select(mrStockDetails => new MRStockDetailsDto
                           {
                               Id = mrStockDetails.Id,
                               Warehouse = mrStockDetails.Warehouse,
                               Location = mrStockDetails.Location,
                               Qty = mrStockDetails.Qty,
                               LocationStock = mrStockDetails.LocationStock,
                           }).ToList()
                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<MaterialRequestsReportDto>>(materialRequestDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all materialRequestDetails";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}