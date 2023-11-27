using System.Dynamic;
using System.Net;
using System.Text;
using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Repository;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OpenGrinController : ControllerBase
    {
        private readonly IOpenGrinRepository _openGrinRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public OpenGrinController(IOpenGrinRepository openGrinRepository, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _openGrinRepository = openGrinRepository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOpenGrinDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<OpenGrinDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinDto>>();

            try
            {
                var openGrinDetails = await _openGrinRepository.GetAllOpenGrinDetails(pagingParameter, searchParams);
                var metadata = new
                {
                    openGrinDetails.TotalCount,
                    openGrinDetails.PageSize,
                    openGrinDetails.CurrentPage,
                    openGrinDetails.HasNext,
                    openGrinDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all OpenGrin");
                var result = _mapper.Map<IEnumerable<OpenGrinDto>>(openGrinDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenGrin";
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
        public async Task<IActionResult> SearchOpenGrinDate([FromQuery] SearchDateParames searchDateParam)
        {
            ServiceResponse<IEnumerable<OpenGrinDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinDto>>();
            try
            {
                var openGrinDetials = await _openGrinRepository.SearchOpenGrinDate(searchDateParam);
                var result = _mapper.Map<IEnumerable<OpenGrinDto>>(openGrinDetials);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenGrin";
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
        public async Task<IActionResult> SearchOpenGrin([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<OpenGrinDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinDto>>();
            try
            {
                var openGrinDetails = await _openGrinRepository.SearchOpenGrin(searchParams);

                _logger.LogInfo("Returned all OpenGrin");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<OpenGrin, OpenGrinDto>().ReverseMap()
                    .ForMember(dest => dest.OpenGrinParts, opt => opt.MapFrom(src => src.OpenGrinParts));
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<OpenGrinDto>>(openGrinDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenGrin";
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
        public async Task<IActionResult> GetAllOpenGrinWithItems([FromBody] OpenGrinSearchDto openGrinSearchDto)
        {
            ServiceResponse<IEnumerable<OpenGrinDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinDto>>();
            try
            {
                var openGrinDetails = await _openGrinRepository.GetAllOpenGrinWithItems(openGrinSearchDto);

                _logger.LogInfo("Returned all OpenGrin");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<OpenGrin, OpenGrinDto>().ReverseMap()
                    .ForMember(dest => dest.OpenGrinParts, opt => opt.MapFrom(src => src.OpenGrinParts));
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<OpenGrinDto>>(openGrinDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenGrinWithItems";
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOpenGrinDetailsbyId(int id)
        {
            ServiceResponse<OpenGrinDto> serviceResponse = new ServiceResponse<OpenGrinDto>();

            try
            {
                var OpenGrinDetailsById = await _openGrinRepository.GetOpenGrinDetailsbyId(id);
                if (OpenGrinDetailsById == null)
                {
                    _logger.LogError($"OpenGrin details with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenGrin details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Binnings with id: {id}");

                    OpenGrinDto openGrinDto = _mapper.Map<OpenGrinDto>(OpenGrinDetailsById);
                    List<OpenGrinPartsDto> OpenGrinPartsList = new List<OpenGrinPartsDto>();

                    if (OpenGrinDetailsById.OpenGrinParts != null)
                    {
                        foreach (var openGrinDetails in OpenGrinDetailsById.OpenGrinParts)
                        {
                            OpenGrinPartsDto openGrinPartsDtos = _mapper.Map<OpenGrinPartsDto>(openGrinDetails);
                            openGrinPartsDtos.OpenGrinDetails = _mapper.Map<List<OpenGrinDetailsDto>>(openGrinDetails.OpenGrinDetails);
                            OpenGrinPartsList.Add(openGrinPartsDtos);
                        }
                    }

                    openGrinDto.OpenGrinParts = OpenGrinPartsList;
                    serviceResponse.Data = openGrinDto;
                    serviceResponse.Message = $"Returned OpenGrinbyId Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside OpenGrinbyId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
            else if (serverConfiguration.GetValue<bool?>("Server1:EnableAvision") == true)
            {
                return "avision";

            }
            else
            {
                return "trasccon";
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOpenGrin([FromBody] OpenGrinPostDto openGrinPostDto)
        {
            ServiceResponse<OpenGrinPostDto> serviceResponse = new ServiceResponse<OpenGrinPostDto>();
            try
            {
                string serverKey = GetServerKey();

                if (openGrinPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OpenGrin object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("OpenGrin object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid OpenGrin object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid OpenGrin object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var openGrinDetails = _mapper.Map<OpenGrin>(openGrinPostDto);
                var openGrinPartsDto = openGrinPostDto.OpenGrinParts;
                var openGrinPartsDtoList = new List<OpenGrinParts>();

             
                string openGrinPartsId = "";
                if (openGrinPartsDto != null)
                {
                    for (int i = 0; i < openGrinPartsDto.Count; i++)
                    {
                        OpenGrinParts openGrinPartsDetails = _mapper.Map<OpenGrinParts>(openGrinPartsDto[i]);
                        openGrinPartsDetails.OpenGrinDetails = _mapper.Map<List<OpenGrinDetails>>(openGrinPartsDto[i].OpenGrinDetails);
                        openGrinPartsDtoList.Add(openGrinPartsDetails);
                      
                    }
                }

                openGrinDetails.OpenGrinParts = openGrinPartsDtoList;

                //generate 
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));

                if (serverKey == "avision")
                {
                    var openGrinNumber = await _openGrinRepository.GenerateOpenGrinNumberForAvision();
                    openGrinDetails.OpenGrinNumber = openGrinNumber;
                }
                else
                {
                    var dateFormat = days + months + years;
                    var openGrinNumber = await _openGrinRepository.GenerateOpenGrinNumber();
                    openGrinDetails.OpenGrinNumber = dateFormat + openGrinNumber;
                }

                await _openGrinRepository.CreateOpenGrin(openGrinDetails);
                    _openGrinRepository.SaveAsync();
                

                //Create OpenGrin To Inventory

                if (openGrinPartsDtoList != null)
                {
                    foreach (var openGrinParts in openGrinPartsDtoList)
                    {
                        foreach (var openGrinDetail in openGrinParts.OpenGrinDetails)
                        {
                            OGInventoryDtoPost inventory = new OGInventoryDtoPost();

                            inventory.PartNumber = openGrinParts.ItemNumber;
                            inventory.MftrPartNumber = openGrinParts.ItemNumber;
                            inventory.Description = openGrinParts.Description;
                            inventory.ProjectNumber = openGrinParts.ReferenceSONumber;
                            inventory.Balance_Quantity = openGrinParts.Qty;
                            inventory.IsStockAvailable = true;
                            inventory.UOM = openGrinParts.UOM;
                            inventory.Warehouse = openGrinDetail.Warehouse;
                            inventory.Location = openGrinDetail.Location;
                            inventory.GrinNo = openGrinDetails.OpenGrinNumber;
                            inventory.GrinPartId = 0;
                            inventory.PartType = openGrinParts.ItemType; // we have to take parttype from grinparts model;
                            inventory.GrinMaterialType = "";
                            inventory.ReferenceID = Convert.ToString(openGrinParts.Id);
                            inventory.ReferenceIDFrom = "OpenGrin";
                            inventory.ShopOrderNo = "";


                            var json = JsonConvert.SerializeObject(inventory);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data);
                        }
                    }
                }

                //Create OpenGrin To InventoryTranction

                //if (openGrinPartsDtoList != null)
                //{
                //    foreach (var openGrinParts in openGrinPartsDtoList)
                //    {
                //        foreach (var openGrinDetail in openGrinParts.OpenGrinDetails)
                //        {
                //            OGInventoryTranctionDto inventoryTranction = new OGInventoryTranctionDto();

                //            inventoryTranction.PartNumber = openGrinParts.ItemNumber;
                //            inventoryTranction.MftrPartNumber = openGrinParts.ItemNumber;
                //            inventoryTranction.Description = openGrinParts.Description;
                //            inventoryTranction.ProjectNumber = openGrinParts.ReferenceSONumber;
                //            inventoryTranction.Issued_Quantity = openGrinParts.Qty;
                //            inventoryTranction.IsStockAvailable = true;
                //            inventoryTranction.UOM = openGrinParts.UOM;
                //            inventoryTranction.Warehouse = openGrinDetail.Warehouse;
                //            inventoryTranction.From_Location = openGrinDetail.Location;
                //            inventoryTranction.TO_Location = openGrinDetail.Location;
                //            inventoryTranction.GrinNo = openGrinDetails.OpenGrinNumber;
                //            inventoryTranction.GrinPartId = 0;
                //            inventoryTranction.PartType = openGrinParts.ItemType; // we have to take parttype from grinparts model;
                //            inventoryTranction.GrinMaterialType = "";
                //            inventoryTranction.ReferenceID = Convert.ToString(openGrinParts.Id);
                //            inventoryTranction.ReferenceIDFrom = "OpenGrin";
                //            inventoryTranction.ShopOrderNo = "";


                //            var json = JsonConvert.SerializeObject(inventoryTranction);
                //            var data = new StringContent(json, Encoding.UTF8, "application/json");
                //            var response = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), data);
                //        }
                //    }
                //}

                serviceResponse.Data = null;
                serviceResponse.Message = "OpenGrin Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOpenGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOpenGrin(int id,[FromBody] OpenGrinUpdateDto openGrinUpdateDto)
        {
            ServiceResponse<OpenGrinUpdateDto> serviceResponse = new ServiceResponse<OpenGrinUpdateDto>();
            try
            {
                if (openGrinUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OpenGrin object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError("OpenGrin object sent from client is null.");
                    return Ok(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid OpenGrin object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError("Invalid OpenGrin object sent from client.");
                    return Ok(serviceResponse);
                }

                var openGrinDetailById = await _openGrinRepository.GetOpenGrinDetailsbyId(id);
                if (openGrinDetailById is null)
                {
                    _logger.LogError($"Binning details with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Grin with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var openGrinDetails = _mapper.Map<OpenGrin>(openGrinUpdateDto);
                var openGrinPartsDto = openGrinUpdateDto.OpenGrinParts;
                var openGrinPartsDtoList = new List<OpenGrinParts>();

                if (openGrinPartsDto != null)
                {
                    for (int i = 0; i < openGrinPartsDto.Count; i++)
                    {
                        OpenGrinParts openGrinPartsDetails = _mapper.Map<OpenGrinParts>(openGrinPartsDto[i]);
                        openGrinPartsDetails.OpenGrinDetails = _mapper.Map<List<OpenGrinDetails>>(openGrinPartsDto[i].OpenGrinDetails);
                        openGrinPartsDtoList.Add(openGrinPartsDetails);
                    }
                }
                var updateOpenGrin = _mapper.Map(openGrinUpdateDto, openGrinDetailById);
                updateOpenGrin.OpenGrinParts = openGrinPartsDtoList;
                await _openGrinRepository.UpdateOpenGrin(updateOpenGrin);
                _openGrinRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " OpenGrin Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateOpenGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOpenGrin(int id)
        {
            ServiceResponse<OpenGrinDto> serviceResponse = new ServiceResponse<OpenGrinDto>();
            try
            {
                var OpenGrinDetailbyId = await _openGrinRepository.GetOpenGrinDetailsbyId(id);
                if (OpenGrinDetailbyId == null)
                {
                    _logger.LogError($"Delete OpenGrin with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete OpenGrin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _openGrinRepository.DeleteOpenGrin(OpenGrinDetailbyId);
                _logger.LogInfo(result);
                _openGrinRepository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " OpenGrin Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOpenGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOpenGrinDataList()
        {
            ServiceResponse<IEnumerable<OpenGrinDataListDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinDataListDto>>();

            try
            {
                var openGrinNoDetials = await _openGrinRepository.GetAllOpenGrinDataList();
                _logger.LogInfo("Returned all OpenGrinNumberDetials");
                var result = _mapper.Map<IEnumerable<OpenGrinDataListDto>>(openGrinNoDetials);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenGrinNumberDetials Successfully ";
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
    }
}