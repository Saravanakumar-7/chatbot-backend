using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;


namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private IInventoryRepository _inventoryRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public InventoryController(IConfiguration config, HttpClient httpClient, IInventoryRepository inventoryRepository, ILoggerManager logger, IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
        }


        // GET: api/<InventoryController>
        [HttpGet]
        public async Task<IActionResult> GetAllInventory([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var getAllInventory = await _inventoryRepository.GetAllInventory(pagingParameter, searchParams);
                var metadata = new
                {
                    getAllInventory.TotalCount,
                    getAllInventory.PageSize,
                    getAllInventory.CurrentPage,
                    getAllInventory.HasNext,
                    getAllInventory.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all Inventory");
                var result = _mapper.Map<IEnumerable<InventoryDto>>(getAllInventory);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Inventory";
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

        [HttpGet]
        public async Task<IActionResult> GetItemNoByInventoryStock()
        {
            ServiceResponse<IEnumerable<InventoryItemNoStock>> serviceResponse = new ServiceResponse<IEnumerable<InventoryItemNoStock>>();
            try
            {
                var itemNoInventoryStock = await _inventoryRepository.GetItemNoByInventoryStock();
               
                _logger.LogInfo("Returned all Inventory");
                var result = _mapper.Map<IEnumerable<InventoryItemNoStock>>(itemNoInventoryStock);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned ItemNoByInventoryStock";
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

        //passing project 

        [HttpGet]
        public async Task<IActionResult> GetInventoryDetailsByGrinNoandGrinId(string GrinNo, int GrinPartsId, string ItemNumber, string ProjectNumber)

        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                var getInventoryDetailsByGrinNoandGrinId = await _inventoryRepository.GetInventoryDetailsByGrinNoandGrinId(GrinNo, GrinPartsId, ItemNumber, ProjectNumber);
                if (getInventoryDetailsByGrinNoandGrinId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory with id: {GrinNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with id: {GrinNo}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with id: {GrinNo}");
                    var result = _mapper.Map<InventoryDto>(getInventoryDetailsByGrinNoandGrinId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Inventory with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInventoryById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //passing itemnumber and location

        [HttpGet]
        public async Task<IActionResult> GetInventoryDetailsByItemnumberandLocation(string ItemNumber, string Location, string Warehouse,string projectNumber)

        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                var getInventoryDetailsByItemNoandLoc = await _inventoryRepository.GetInventoryDetailsByItemNumberandLocation(ItemNumber, Location, Warehouse, projectNumber);
                if (getInventoryDetailsByItemNoandLoc == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory with id: {ItemNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with id: {ItemNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with id: {ItemNumber}");
                    var result = _mapper.Map<InventoryDto>(getInventoryDetailsByItemNoandLoc);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Inventory with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInventoryById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }





        [HttpGet]
        public async Task<IActionResult> GetInventoryDetailsByGrinNo(string GrinNo, string ItemNumber, string ProjectNumber)

        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                var getInventoryDetailsByGrinNo = await _inventoryRepository.GetInventoryDetailsByGrinNo(GrinNo, ItemNumber, ProjectNumber);
                if (getInventoryDetailsByGrinNo == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory with id: {GrinNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with id: {GrinNo}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with id: {GrinNo}");
                    var result = _mapper.Map<InventoryDto>(getInventoryDetailsByGrinNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Inventory with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInventoryById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //passing itemnumber and projectnumber to get inventory details


        [HttpGet]
        public async Task<IActionResult> GetInventoryDetailsByItemAndProjectNo(string itemNumber, string projectNumber)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetInventoryDetailsByItemAndProjectNo(itemNumber, projectNumber);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory with itemNumber and ProjectNumber: {itemNumber} {projectNumber}, is invalid";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber and ProjectNumber: {itemNumber} {projectNumber}, is invalid");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber and ProjectNumber: {itemNumber} {projectNumber}");
                    var result = _mapper.Map<InventoryDto>(InventoryDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Inventory with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Invalid inventory{ex.Message},{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStockDetailsForAllLocationWarehouseByItemNo(string itemNumber)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();
            try
            {
                decimal InventoryDetails = await _inventoryRepository.GetStockDetailsForAllLocationWarehouseByItemNo(itemNumber);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber: {itemNumber}, is invalid");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber: {itemNumber}");
                    //var result = InventoryDetails;
                    //serviceResponse.Data = result;
                    //serviceResponse.Message = "Returned InventoryDetails with id Successfully";
                    //serviceResponse.Success = true;
                    //serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(InventoryDetails);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetInventoryDetailsByItemNo(string itemNumber)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetInventoryDetailsByItemNo(itemNumber);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber: {itemNumber}, is invalid");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber: {itemNumber}");
                    var result = _mapper.Map<InventoryDto>(InventoryDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned InventoryDetails with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetInventoryDetailsWithInventoryStock(string itemNumber,string warehouse,string location)
        {
            ServiceResponse<IEnumerable<InventoryDetailsLocationStock>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDetailsLocationStock>>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetInventoryDetailsWithInventoryStock(itemNumber, warehouse, location);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"Inventory with itemNumber: {itemNumber}, is invalid");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber: {itemNumber}");
                    var result = _mapper.Map<IEnumerable<InventoryDetailsLocationStock>>(InventoryDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned InventoryDetails With LocationStock Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //update material request issued item

        [HttpPost]
        public async Task<IActionResult> MaterialInventoryBalanceQty([FromBody] List<UpdateInventoryBalanceQty> updateInventoryBalanceQty)
        {             
            foreach (var materialIssueQty in updateInventoryBalanceQty)
            {
                foreach (var Location in materialIssueQty.MRNWarehouseList)
                {
                    IEnumerable<Inventory> inventories = await _inventoryRepository.GetInventoryDetailsByItemNoandLocationandwarehouse(materialIssueQty.PartNumber, Location.Location, Location.Warehouse);
                    var inventoryItem = inventories.FirstOrDefault();
                    inventoryItem.Balance_Quantity = inventoryItem.Balance_Quantity - Location.Qty;
                    await _inventoryRepository.UpdateInventory(inventoryItem);
                }
            }
            _inventoryRepository.SaveAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> MaterialReturnNoteInventoryBalanceQty([FromBody] List<MRNUpdateInventoryBalanceQty> updateInventoryBalanceQty)
        {
            foreach (var materialReturnQty in updateInventoryBalanceQty) 
            {
                foreach (var Location in materialReturnQty.MRNDetails)
                {
                    IEnumerable<Inventory> inventories = await _inventoryRepository.GetInventoryDetailsByItemNoandLocationandwarehouse(materialReturnQty.PartNumber, Location.Location, Location.Warehouse);
                    var inventoryItem = inventories.FirstOrDefault();
                    if (inventoryItem == null)
                    {
                        Inventory inventoryPost = new Inventory();
                        inventoryPost.PartNumber = materialReturnQty.PartNumber;
                        inventoryPost.MftrPartNumber = materialReturnQty.PartNumber;
                        inventoryPost.ProjectNumber = "Project";
                        inventoryPost.Description = "";
                        inventoryPost.Balance_Quantity = Location.Qty;
                        inventoryPost.UOM = "";
                        inventoryPost.GrinMaterialType = "";
                        inventoryPost.shopOrderNo = "";
                        inventoryPost.Unit = "";
                        inventoryPost.GrinNo = "";
                        inventoryPost.GrinPartId = 0;
                        inventoryPost.IsStockAvailable = true;
                        inventoryPost.Warehouse = Location.Warehouse;
                        inventoryPost.Location = Location.Location;
                        inventoryPost.PartType = "";
                        inventoryPost.ReferenceID = "0";
                        inventoryPost.ReferenceIDFrom = "MaterialReturnNote";
                        await _inventoryRepository.CreateInventory(inventoryPost);
                        _inventoryRepository.SaveAsync();
                    }
                    else
                    {
                        inventoryItem.Balance_Quantity = inventoryItem.Balance_Quantity + Location.Qty;
                        await _inventoryRepository.UpdateInventory(inventoryItem);
                    }
                }
            }
            _inventoryRepository.SaveAsync();
            return Ok();
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryById(int id)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                var getInventoryById = await _inventoryRepository.GetInventoryById(id);
                if (getInventoryById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with id: {id}");
                    var result = _mapper.Map<InventoryDto>(getInventoryById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Inventory with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInventoryById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchInventoryDate([FromQuery] SearchsDateParms searchDateParam)
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var inventoryDetails = await _inventoryRepository.SearchInventoryDate(searchDateParam);
                var result = _mapper.Map<IEnumerable<InventoryDto>>(inventoryDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all InventoryDetails By Date";
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
        public async Task<IActionResult> SearchInventory([FromQuery] SearchParames searchParames)
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var inventoryDetails = await _inventoryRepository.SearchInventory(searchParames);

                _logger.LogInfo("Returned all Inventory");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<Inventory, InventoryDto>().ReverseMap();
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<InventoryDto>>(inventoryDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all InventoryDetails";
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
        public async Task<IActionResult> SearchInventoryDetailsWithSumOfStock([FromQuery] InventoryItemNo inventoryItemNo)
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var inventoryDetails = await _inventoryRepository.SearchInventoryDetailsWithSumOfStock(inventoryItemNo);

                _logger.LogInfo("Returned all Inventory");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<Inventory, InventoryDto>().ReverseMap();
                //});
                //var mapper = config.CreateMapper();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Inventory, InventoryDto>()
                        .ForMember(dest => dest.Balance_Quantity, opt => opt.MapFrom(src => src.Balance_Quantity));
                });
                var mapper = config.CreateMapper();

                var result = mapper.Map<IEnumerable<InventoryDto>>(inventoryDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all InventoryDetails";
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
        public async Task<IActionResult> GetAllInventoryWithItems([FromBody] InventorySearchDto inventorySearch)
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var inventoryDetails = await _inventoryRepository.GetAllInventoryWithItems(inventorySearch);

                _logger.LogInfo("Returned all Inventory");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<Inventory, InventoryDto>().ReverseMap();
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<InventoryDto>>(inventoryDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all InventoryDetails";
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
        public async Task<IActionResult> GetInventoryDetailsWithSumOfStock([FromBody] InventoryBalQty inventoryBalQty)
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var inventoryDetails = await _inventoryRepository.GetInventoryDetailsWithSumOfStock(inventoryBalQty);

                _logger.LogInfo("Returned all Inventory");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<Inventory, InventoryDto>().ReverseMap();
                //});
                //var mapper = config.CreateMapper();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Inventory, InventoryDto>()
                        .ForMember(dest => dest.Balance_Quantity, opt => opt.MapFrom(src => src.Balance_Quantity));
                });
                var mapper = config.CreateMapper();

                var result = mapper.Map<IEnumerable<InventoryDto>>(inventoryDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all InventoryDetails";
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
        public async Task<IActionResult> GetInventoryDetailsWithSumOfBalQty([FromBody] InventoryDetailsBalQty inventoryDetailsBalQty)
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var inventoryDetails = await _inventoryRepository.GetInventoryDetailsWithSumOfBalQty(inventoryDetailsBalQty);

                _logger.LogInfo("Returned all Inventory");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<Inventory, InventoryDto>().ReverseMap();
                //});
                //var mapper = config.CreateMapper();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Inventory, InventoryDto>()
                        .ForMember(dest => dest.Balance_Quantity, opt => opt.MapFrom(src => src.Balance_Quantity));
                });
                var mapper = config.CreateMapper();

                var result = mapper.Map<IEnumerable<InventoryDto>>(inventoryDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all InventoryDetails";
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
        public async Task<IActionResult> CreateInventory([FromBody] InventoryDtoPost inventoryDtoPost)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                if (inventoryDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Inventory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Inventory object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Inventory object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Inventory object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var createInvetory = _mapper.Map<Inventory>(inventoryDtoPost);

                _inventoryRepository.CreateInventory(createInvetory);
                _inventoryRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Inventory Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;

                return Created("GetInventoryById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] InventoryDtoUpdate inventoryDtoUpdate)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                if (inventoryDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Inventory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Inventory object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Inventory object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Inventory object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var getInventoryById = await _inventoryRepository.GetInventoryById(id);
                 if (getInventoryById is null)
                {
                    _logger.LogError($"Inventory with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update Inventory with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var updateInventory = _mapper.Map(inventoryDtoUpdate, getInventoryById);
                 string result = await _inventoryRepository.UpdateInventory(updateInventory);
                _logger.LogInfo(result);
                _inventoryRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
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
                _logger.LogError($"Something went wrong inside UpdateCommodity action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                var getInventoryById = await _inventoryRepository.GetInventoryById(id);
                if (getInventoryById == null)
                {
                    _logger.LogError($"Delete Inventory with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Inventory with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _inventoryRepository.DeleteInventory(getInventoryById);
                _logger.LogInfo(result);
                _inventoryRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Inventory Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteInventory action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //pass data from MRN using _httpclient Production service to Warehouse Service

        [HttpPost]
        public async Task<IActionResult> UpdateInventoryForMRN([FromBody] InventoryUpdateDtoForMRN inventoryUpdateDtoForMRN)
        {
          
            var projectNumber = inventoryUpdateDtoForMRN.ProjectNumber;
            var unit = inventoryUpdateDtoForMRN.Unit;
            foreach (var item in inventoryUpdateDtoForMRN.MaterialReturnNoteItems)
            {
                foreach (var warehouse in item.MRNWarehouseList)
                {
                    Inventory inventoryDetails = await _inventoryRepository.GetInventoryDetailsByItemNoProjectNoUnitWarehouseAndLocation(item.PartNumber,projectNumber,unit,warehouse.Warehouse,warehouse.Location);
                    if (inventoryDetails != null)
                    {
                        inventoryDetails.Balance_Quantity += item.ReturnQty;
                        inventoryDetails.IsStockAvailable = true;
                        await _inventoryRepository.UpdateInventory(inventoryDetails);
                    }
                    else
                    {
                        var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterAPI"],
                                                                                "GetItemMasterDetailsForMNRByItemNo?", "&ItemNumber=",item.PartNumber));
                        var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                        dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                        dynamic inventoryObject = itemMasterObjectData.data;

                        Inventory inventory = new Inventory();
                        inventory.PartNumber = item.PartNumber;
                        inventory.ProjectNumber = projectNumber;
                        inventory.Unit = unit;
                        inventory.Warehouse = warehouse.Warehouse;
                        inventory.Location = warehouse.Location;
                        inventory.MftrPartNumber = inventoryObject.MftrPartNumber;
                        inventory.Description = inventoryObject.Description;
                        inventory.ReferenceIDFrom = inventoryUpdateDtoForMRN.MRNNumber;
                        inventory.ReferenceID = inventoryUpdateDtoForMRN.MRNNumber;
                        inventory.PartType = "";
                        inventory.UOM = inventoryObject.Uom;
                    }
                }
            }
            _inventoryRepository.SaveAsync();
            return Ok();
        }

    }
}
