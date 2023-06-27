using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities.Enums;
using Tips.Production.Api.Repository;
using static Org.BouncyCastle.Math.EC.ECCurve;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MaterialIssueController : ControllerBase
    {
        private IMaterialIssueHistoryRepository _materialIssueHistoryRepository;
        private IMaterialIssueRepository  _materialIssueRepository;
        private ILoggerManager _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private IMapper _mapper; 

        public MaterialIssueController(IMaterialIssueHistoryRepository materialIssueHistoryRepository,IMaterialIssueRepository materialIssueRepository, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _materialIssueHistoryRepository = materialIssueHistoryRepository;
            _materialIssueRepository = materialIssueRepository;
            _logger = logger;
            _httpClient = httpClient;
            _config = config;
            _mapper = mapper;
        }

        // GET: api/<MaterialIssueController>
        [HttpGet]
        public async Task<IActionResult> GetAllMaterialIssues([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<MaterialIssueDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueDto>>();
            try
            {
                var materialIssueDetails = await _materialIssueRepository.GetAllMaterialIssues(pagingParameter, searchParamess);
                var metadata = new
                {
                    materialIssueDetails.TotalCount,
                    materialIssueDetails.PageSize,
                    materialIssueDetails.CurrentPage,
                    materialIssueDetails.HasNext,
                    materialIssueDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all MaterialIssues");
                var result = _mapper.Map<IEnumerable<MaterialIssueDto>>(materialIssueDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialIssue";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        private string GetServerKey()
        {
            var serverName = Environment.MachineName;
            var serverConfiguration = _config.GetSection("ServerConfiguration");

            if (serverConfiguration.GetValue<bool?>("Server1:EnableKeus") == true)
            {
                return "keus";
            }
            else
            {
                return "trasccon";
            }
        }
        //private string GetServerKey()
        //{
        //    var serverName = Dns.GetHostName();

        //    if (serverName == "Server1")
        //    {
        //        return "keus";
        //    }
        //    else if (serverName == "Server2")
        //    {
        //        return "avision";
        //    }
        //    else
        //    {
        //        return "trasccon";
        //    }
        //}


        // GET api/<MaterialIssueController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterialIssueById(int id)
        {
            ServiceResponse<MaterialIssueDto> serviceResponse = new ServiceResponse<MaterialIssueDto>();

            try
            {
                string serverKey = GetServerKey();// Set the server key here dynamically based on your logic
                
                var materialIssueDetailById = await _materialIssueRepository.GetMaterialIssueById(id);


                if (materialIssueDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialIssue with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MaterialIssue with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var materialIssueDetails = _mapper.Map<MaterialIssueDto>(materialIssueDetailById);

                    if (serverKey == "keus")
                    { 
                            for (int i = 0; i < materialIssueDetails.materialIssueItems.Count(); i++)
                            {
                            var balanceQty = 0;
                            var partnumber = materialIssueDetailById.materialIssueItems[i].PartNumber;
                                var projectnumber = materialIssueDetailById.materialIssueItems[i].ProjectNumber;
                                var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                                  "GetInventoryDetailsByItemNo?", "itemNumber=", partnumber));
                            if (inventoryObjectResult != null && inventoryObjectResult.StatusCode == HttpStatusCode.OK)
                            {
                                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                                dynamic inventoryObject = inventoryObjectData.data;
                                balanceQty = inventoryObject.balance_Quantity;
                            }
                                materialIssueDetails.materialIssueItems[i].AvailableQty = balanceQty;
                            }                        

                    }
                    else
                    {
                        for (int i = 0; i < materialIssueDetails.materialIssueItems.Count(); i++)
                        {
                            var balanceQty = 0;
                            var partnumber = materialIssueDetailById.materialIssueItems[i].PartNumber;
                            var projectnumber = materialIssueDetailById.materialIssueItems[i].ProjectNumber;
                            var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                              "GetInventoryDetailsByItemAndProjectNo?", "itemNumber=", partnumber, "&projectNumber=", projectnumber));
                            if(inventoryObjectResult !=null && inventoryObjectResult.StatusCode == HttpStatusCode.OK)
                            {
                                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                                dynamic inventoryObject = inventoryObjectData.data;
                                balanceQty = inventoryObject.balance_Quantity;
                            } 
                            materialIssueDetails.materialIssueItems[i].AvailableQty = balanceQty;
                        }
                    }

                    serviceResponse.Data = materialIssueDetails;
                    serviceResponse.Message = "Returned MaterialIssue with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMaterialIssueById action: {ex.Message} {ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMaterialIssueByShopOrderNo(string shopOrderNumber)
        {
            ServiceResponse<MaterialIssueDto> serviceResponse = new ServiceResponse<MaterialIssueDto>();

            try
            {
                var materialIssueDetail = await _materialIssueRepository.GetMaterialIssueByShopOrderNo(shopOrderNumber);

                if (materialIssueDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialIssue with shopOrderNumber hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"MaterialIssue with shopOrderNumber: {shopOrderNumber}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else

                {
                    _logger.LogInfo($"Returned MaterialIssueDetails with shopOrderNumber: {shopOrderNumber}");

                    MaterialIssueDto materialIssueDto = _mapper.Map<MaterialIssueDto>(materialIssueDetail);

                    List<MaterialIssueItemDto> MaterialIssueItemList = new List<MaterialIssueItemDto>();

                    foreach (var materialIssueItemDetails in materialIssueDetail.materialIssueItems)
                    {
                        MaterialIssueItemDto MaterialIssueItemDto = _mapper.Map<MaterialIssueItemDto>(materialIssueItemDetails);
                        MaterialIssueItemList.Add(MaterialIssueItemDto);
                    }

                    materialIssueDto.materialIssueItems = MaterialIssueItemList;
                    serviceResponse.Data = materialIssueDto;
                    serviceResponse.Message = $"Returned MaterialIssueDetails";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMaterialIssueByShopOrderNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        // POST api/<MaterialIssueController>
        [HttpPost]
        public IActionResult CreateMaterialIssue([FromBody] MaterialIssuePostDto materialIssuePostDto)
        {
            ServiceResponse<MaterialIssueDto> serviceResponse = new ServiceResponse<MaterialIssueDto>();

            try
            {
                if (materialIssuePostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "MaterialIssue object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("MaterialIssue object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid MaterialIssue object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid MaterialIssue object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var  materialIssue = _mapper.Map<MaterialIssue>(materialIssuePostDto);

                _materialIssueRepository.CreateMaterialIssue(materialIssue);
                _materialIssueRepository.SaveAsync(); 
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialIssue Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;

                return Created("GetMaterialIssueById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateMaterialIssue action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<MaterialIssueController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterialIssue(int id, [FromBody] MaterialIssueUpdateDto materialIssueUpdateDto)
        {
            ServiceResponse<MaterialIssueUpdateDto> serviceResponse = new ServiceResponse<MaterialIssueUpdateDto>();

            try
            {
                if (materialIssueUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "MaterialIssue object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("MaterialIssue object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid MaterialIssue object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid MaterialIssue object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var materialIssueDetailsById = await _materialIssueRepository.GetMaterialIssueById(id);
                if (materialIssueDetailsById is null)
                {
                    _logger.LogError($"MaterialIssue with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update MaterialIssue with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var updateMaterialIssue = _mapper.Map(materialIssueUpdateDto, materialIssueDetailsById);
                //_mapper.Map(materialIssueUpdateDto, materialIssueDetailsById);

                

                List<MaterialIssueItem> materialIssueItems = new List<MaterialIssueItem>();

                foreach (var item in materialIssueUpdateDto.MaterialIssueItems)
                {
                    MaterialIssueItem materialIssueItem = _mapper.Map<MaterialIssueItem>(item);

                    materialIssueItem.IssuedQty += item.NewIssueQty;
                    materialIssueItems.Add(materialIssueItem);

                    //update inventory 

                    var partnumber = materialIssueItem.PartNumber;
                    var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                     "GetInventoryDetailsByItemNo?", "&itemNumber=", partnumber));
                    var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                    dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                    dynamic inventoryObject = inventoryObjectData.data;
                    inventoryObject.balance_Quantity -= item.NewIssueQty; 
                    if(inventoryObject.balance_Quantity == 0)
                    {
                        inventoryObject.isStockAvailable = false;
                    }
                    
                    var json = JsonConvert.SerializeObject(inventoryObject);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync(string.Concat(_config["InventoryAPI"],
                        "UpdateInventory?id=",Convert.ToInt32(inventoryObject.id)), data);

                    MaterialIssueHistory materialissueHistory = new MaterialIssueHistory();
                    materialissueHistory.Id = materialIssueItem.Id;
                    materialissueHistory.PartNumber = materialIssueItem.PartNumber;
                    materialissueHistory.ShopOrderNumber = updateMaterialIssue.ShopOrderNumber;
                    materialissueHistory.Description = materialIssueItem.Description;
                    materialissueHistory.ProjectNumber = materialIssueItem.ProjectNumber;
                    materialissueHistory.PartType = materialIssueItem.PartType;
                    materialissueHistory.UOM = materialIssueItem.UOM;
                    materialissueHistory.RequiredQty = materialIssueItem.RequiredQty;
                    materialissueHistory.IssuedQty = materialIssueItem.IssuedQty;
                    materialissueHistory.MaterialIssuedStatus = materialIssueItem.MaterialIssuedStatus;
                    materialissueHistory.Unit = materialIssueItem.Unit;

                    _materialIssueHistoryRepository.CreateMaterialIssueHistory(materialissueHistory);
                    _materialIssueHistoryRepository.SaveAsync();

                }
                updateMaterialIssue.materialIssueItems = materialIssueItems;
                string result = await _materialIssueRepository.UpdateMaterialIssue(updateMaterialIssue);

                _logger.LogInfo(result);
                _materialIssueRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialIssue Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside UpdateMaterialIssue action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchMaterialIssueDate([FromQuery] SearchDateparames searchDateParam)
        {
            ServiceResponse<IEnumerable<MaterialIssueDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueDto>>();
            try
            {
                var materialIssueDetails = await _materialIssueRepository.SearchMaterialIssueDate(searchDateParam);
                var result = _mapper.Map<IEnumerable<MaterialIssueDto>>(materialIssueDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all materialIssueDetails";
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
        public async Task<IActionResult> GetAllMaterialIssueWithItems([FromBody] MaterialIssueSearchDto materialIssueSearch)
        {
            ServiceResponse<IEnumerable<MaterialIssueDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueDto>>();
            try
            {
                var materialIssueDetails = await _materialIssueRepository.GetAllMaterialIssueWithItems(materialIssueSearch);



                _logger.LogInfo("Returned all materialIssueDetails");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<MaterialIssueDto, MaterialIssue>().ReverseMap()
                    .ForMember(dest => dest.materialIssueItems, opt => opt.MapFrom(src => src.materialIssueItems));
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<MaterialIssueDto>>(materialIssueDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialIssueDetails";
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
        public async Task<IActionResult> SearchMaterialIssue([FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<MaterialIssueDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueDto>>();
            try
            {
                var materialIssueDetails = await _materialIssueRepository.SearchMaterialIssue(searchParams);

                _logger.LogInfo("Returned all materialIssueDetails");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<MaterialIssueDto, MaterialIssue>().ReverseMap()
                    .ForMember(dest => dest.materialIssueItems, opt => opt.MapFrom(src => src.materialIssueItems));
                });
                var mapper = config.CreateMapper();

                var result = mapper.Map<IEnumerable<MaterialIssueDto>>(materialIssueDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialIssueDetails";
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

        // DELETE api/<MaterialIssueController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialIssue(int id)
        {
            ServiceResponse<MaterialIssueDto> serviceResponse = new ServiceResponse<MaterialIssueDto>();

            try
            {
                var materialIssueDetailById = await _materialIssueRepository.GetMaterialIssueById(id);
                if (materialIssueDetailById == null)
                {
                    _logger.LogError($"Delete MaterialIssue with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete MaterialIssue with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _materialIssueRepository.DeleteMaterialIssue(materialIssueDetailById);
                _logger.LogInfo(result);
                _materialIssueRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialIssue Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteMaterialIssue action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMaterialIssueIdNameList()
        {
            ServiceResponse<IEnumerable<MaterialIssueIdNameList>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueIdNameList>>();
            try
            {
                var listOfAllMaterialIssueIdNames = await _materialIssueRepository.GetAllMaterialIssueIdNameList();
                var result = _mapper.Map<IEnumerable<MaterialIssueIdNameList>>(listOfAllMaterialIssueIdNames);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All listOfAllMaterialIssueIdNames";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllMaterialIssueIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
