using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities.Enums;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]

    // Materialrequest Controller
    public class MaterialReturnNoteController : ControllerBase
    {
        private IMaterialReturnNoteRepository _materialReturnNoteRepository;
        private IMapper _mapper;
        private ILoggerManager _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;

        public MaterialReturnNoteController(IHttpClientFactory clientFactory,IConfiguration config, HttpClient httpClient, IMaterialReturnNoteRepository materialReturnNoteRepository, IMapper mapper, ILoggerManager logger)
        {
            _materialReturnNoteRepository = materialReturnNoteRepository;
            _mapper = mapper;
            _logger = logger;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMaterialReturnNotes([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<MaterialReturnNoteDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialReturnNoteDto>>();

            try
            {
                var materialReturnNoteDetails = await _materialReturnNoteRepository.GetAllMaterialReturnNotes(pagingParameter, searchParamess);
                var metadata = new
                {
                    materialReturnNoteDetails.TotalCount,
                    materialReturnNoteDetails.PageSize,
                    materialReturnNoteDetails.CurrentPage,
                    materialReturnNoteDetails.HasNext,
                    materialReturnNoteDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogError("Returned all getAllMaterialReturnNote");
                var result = _mapper.Map<IEnumerable<MaterialReturnNoteDto>>(materialReturnNoteDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialReturnNote Successfully";
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
        public async Task<IActionResult> GetAllMRNOpenStatus([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<MaterialReturnNoteDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialReturnNoteDto>>();

            try
            {
                var materialReturnNoteDetails = await _materialReturnNoteRepository.GetAllMRNStatusOpen(pagingParameter, searchParamess);

                var metadata = new
                {
                    materialReturnNoteDetails.TotalCount,
                    materialReturnNoteDetails.PageSize,
                    materialReturnNoteDetails.CurrentPage,
                    materialReturnNoteDetails.HasNext,
                    materialReturnNoteDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogError("Returned all getAllMaterialReturnNoteStatusOpen");
                var result = _mapper.Map<IEnumerable<MaterialReturnNoteDto>>(materialReturnNoteDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialReturnNoteStatusOpen Successfully";
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
        public async Task<IActionResult> GetAllMRNOpenwithPartialStatus([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParammes)
        {
            ServiceResponse<IEnumerable<MaterialReturnNoteDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialReturnNoteDto>>();

            try
            {
                var materialReturnNoteDetails = await _materialReturnNoteRepository.GetAllMRNOpenwithPartialStatus(pagingParameter, searchParammes);

                var metadata = new
                {
                    materialReturnNoteDetails.TotalCount,
                    materialReturnNoteDetails.PageSize,
                    materialReturnNoteDetails.CurrentPage,
                    materialReturnNoteDetails.HasNext,
                    materialReturnNoteDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogError("Returned all getAllmaterialReturnNoteStatusOpen");
                var result = _mapper.Map<IEnumerable<MaterialReturnNoteDto>>(materialReturnNoteDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialReturnNoteOpenOrPartiallyClosedStatus Successfully";
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
        public async Task<IActionResult> GetAllMRNCloseStatus()
        {
            ServiceResponse<IEnumerable<MaterialReturnNoteDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialReturnNoteDto>>();

            try
            {
                var materialReturnNoteDetails = await _materialReturnNoteRepository.GetAllMRNStatusClose();

                _logger.LogError("Returned all getAllMaterialReturnNoteStatusClose");
                var result = _mapper.Map<IEnumerable<MaterialReturnNoteDto>>(materialReturnNoteDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialReturnNoteStatusClose Successfully";
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
        public async Task<IActionResult> GetMaterialReturnNoteById(int id)
        {
            ServiceResponse<MaterialReturnNoteDto> serviceResponse = new ServiceResponse<MaterialReturnNoteDto>();

            try
            {
                var materialReturnNoteDetailbyId = await _materialReturnNoteRepository.GetMaterialReturnNoteById(id);

                if (materialReturnNoteDetailbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialReturnNoteDetail with id hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MaterialReturnNoteDetail with id hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogError($"Returned materialReturnNoteDetail with id: {id}");

                    MaterialReturnNoteDto materialReturnNoteDto = _mapper.Map<MaterialReturnNoteDto>(materialReturnNoteDetailbyId);

                    List<MaterialReturnNoteItemDto> materialReturnNoteItemDtos = new List<MaterialReturnNoteItemDto>();

                    if (materialReturnNoteDetailbyId.MaterialReturnNoteItems != null)
                    {

                        foreach (var materialReturnNoteitemDetails in materialReturnNoteDetailbyId.MaterialReturnNoteItems)
                        {
                            MaterialReturnNoteItemDto materialReturnNoteItemDto = _mapper.Map<MaterialReturnNoteItemDto>(materialReturnNoteitemDetails);
                            materialReturnNoteItemDto.MRNWarehouseList = _mapper.Map<List<MRNWarehouseDetailsDto>>(materialReturnNoteitemDetails.MRNWarehouseList);
                            materialReturnNoteItemDtos.Add(materialReturnNoteItemDto);
                        }
                    }

                    materialReturnNoteDto.MaterialReturnNoteItems = materialReturnNoteItemDtos;
                    serviceResponse.Data = materialReturnNoteDto;
                    serviceResponse.Message = $"Returned materialReturnNote with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside MaterialReturnNoteById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
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
        public async Task<IActionResult> CreateMaterialReturnNote([FromBody] MaterialReturnNotePostDto materialReturnNotePostDto)
        {
            ServiceResponse<MaterialReturnNotePostDto> serviceResponse = new ServiceResponse<MaterialReturnNotePostDto>();

            try
            {
                string serverKey = GetServerKey();
                if (materialReturnNotePostDto is null)
                {
                    _logger.LogError("MaterialReturnNote object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "MaterialReturnNote object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid MaterialReturnNote object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid MaterialReturnNote object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                //var mrnNumber = await _materialReturnNoteRepository.GenerateMRNNumber();
                //if (mrnNumber == null)
                //{
                //    _logger.LogError("Something went wrong inside Service Method GenerateMRNNumber");
                //    serviceResponse.Data = null;
                //    serviceResponse.Message = "Something went wrong. please try again";
                //    serviceResponse.Success = false;
                //    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                //    return BadRequest(serviceResponse);
                //}

                var materialReturnNote = _mapper.Map<MaterialReturnNote>(materialReturnNotePostDto);
                var materialReturnNoteItemDto = materialReturnNotePostDto.MaterialReturnNoteItems;
                var materialReturnNoteItemList = new List<MaterialReturnNoteItem>();

                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));



                //var newcount = await _materialReturnNoteRepository.GetMRNumberAutoIncrementCount(date);

                //if (newcount > 0)
                //{
                //    var number = newcount + 1;
                //    string e = String.Format("{0:D4}", number);
                //    materialReturnNote.MRNNumber = days + months + years + "MRN" + (e);
                //}
                //else
                //{
                //    var count = 1;
                //    var e = count.ToString("D4");
                //    materialReturnNote.MRNNumber = days + months + years + "MRN" + (e);
                //}

                if (serverKey == "trasccon")
                {
                    var dateFormat = days + months + years;
                    var mrnNumber = await _materialReturnNoteRepository.GenerateMRNNumber();
                    materialReturnNote.MRNNumber = dateFormat + mrnNumber;
                }
                else if (serverKey == "keus")
                {
                    var dateFormat = days + months + years;
                    var mrnNumber = await _materialReturnNoteRepository.GenerateMRNNumber();
                    materialReturnNote.MRNNumber = dateFormat + mrnNumber;
                }
                else
                {
                    var mrnNumber = await _materialReturnNoteRepository.GenerateMRNNumberForAvision();
                    materialReturnNote.MRNNumber = mrnNumber;
                }

                if (materialReturnNoteItemDto != null)
                {

                    for (int i = 0; i < materialReturnNoteItemDto.Count; i++)
                    {
                        MaterialReturnNoteItem materialReturnNoteItem = _mapper.Map<MaterialReturnNoteItem>(materialReturnNoteItemDto[i]);
                        materialReturnNoteItemList.Add(materialReturnNoteItem);

                    }
                }

                materialReturnNote.MaterialReturnNoteItems = materialReturnNoteItemList;

                await _materialReturnNoteRepository.CreateMaterialReturnNote(materialReturnNote);

                _materialReturnNoteRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialReturnNote Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetMaterialReturnNoteById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateMaterialReturnNote action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterialReturnNote(int id, [FromBody] MaterialReturnNoteUpdateDto materialReturnNoteUpdateDto)
        {
            ServiceResponse<MaterialReturnNoteUpdateDto> serviceResponse = new ServiceResponse<MaterialReturnNoteUpdateDto>();

            try
            {
                if (materialReturnNoteUpdateDto is null)
                {
                    _logger.LogError("Update MaterialReturnNote object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update MaterialReturnNote object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update MaterialReturnNote object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update MaterialReturnNote object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var materialReturnNoteDetailById = await _materialReturnNoteRepository.GetMaterialReturnNoteById(id);

                if (materialReturnNoteDetailById is null)
                {
                    _logger.LogError($"GetMaterialReturnNote with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update GetMaterialReturnNote with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var materialReturnNoteDetail = _mapper.Map<MaterialReturnNote>(materialReturnNoteDetailById);
                var materialReturnNotesItemDto = materialReturnNoteUpdateDto.MaterialReturnNoteItems;
                var materialReturnNoteItemList = new List<MaterialReturnNoteItem>();
                if (materialReturnNotesItemDto != null)
                {
                    for (int i = 0; i < materialReturnNotesItemDto.Count; i++)
                    {
                        MaterialReturnNoteItem materialReturnNoteItem = _mapper.Map<MaterialReturnNoteItem>(materialReturnNotesItemDto[i]);
                        materialReturnNoteItemList.Add(materialReturnNoteItem);

                    }
                }
                materialReturnNoteDetail.MaterialReturnNoteItems = materialReturnNoteItemList;
                var updateMaterialReturnNoteItem = _mapper.Map(materialReturnNoteUpdateDto, materialReturnNoteDetailById);
                string result = await _materialReturnNoteRepository.UpdateMaterialReturnNote(updateMaterialReturnNoteItem);
                _materialReturnNoteRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialReturnNote Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateMaterialReturnNote action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ReturnMaterialReturnNote(int id, [FromBody] MaterialReturnNoteUpdateDto materialReturnNoteUpdateDto)
        {
            ServiceResponse<MaterialReturnNoteUpdateDto> serviceResponse = new ServiceResponse<MaterialReturnNoteUpdateDto>();

            try
            {
                if (materialReturnNoteUpdateDto is null)
                {
                    _logger.LogError("Update MaterialReturnNote object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update MaterialReturnNote object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update MaterialReturnNote object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update MaterialReturnNote object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var materialReturnNoteDetail = await _materialReturnNoteRepository.GetMaterialReturnNoteById(id);

                if (materialReturnNoteDetail is null)
                {
                    _logger.LogError($"GetMaterialReturnNote with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update GetMaterialReturnNote with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                 
                var materialReturnNotesItemDto = materialReturnNoteUpdateDto.MaterialReturnNoteItems;
                var materialReturnNoteItemList = new List<MaterialReturnNoteItem>();
                HttpStatusCode updateMaterialReturnNoteResp = HttpStatusCode.OK;
                if (materialReturnNotesItemDto != null)
                {
                    for (int i = 0; i < materialReturnNotesItemDto.Count; i++)
                    {
                        MaterialReturnNoteItem materialReturnNoteItem = _mapper.Map<MaterialReturnNoteItem>(materialReturnNotesItemDto[i]);
                        materialReturnNoteItem.ProjectNumber = materialReturnNoteUpdateDto.ProjectNumber;
                        materialReturnNoteItem.ShopOrderNumber = materialReturnNoteUpdateDto.ShopOrderNumber;
                        materialReturnNoteItem.MRNWarehouseList = _mapper.Map<List<MRNWarehouseDetails>>(materialReturnNotesItemDto[i].MRNWarehouseList);

                        materialReturnNoteItemList.Add(materialReturnNoteItem);

                    }
                }

                var mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<MaterialReturnNoteItem, MRNUpdateInventoryBalanceQty>()
                        .ForMember(dest => dest.PartNumber, opt => opt.MapFrom(src => src.PartNumber))
                        .ForMember(dest => dest.ProjectNumber, opt => opt.MapFrom(src => src.ProjectNumber))
                        .ForMember(dest => dest.ShopOrderNumber, opt => opt.MapFrom(src => src.ShopOrderNumber))
                        .ForMember(dest => dest.MRNDetails, opt => opt.MapFrom(src => src.MRNWarehouseList.Select(detail => new MRNInventoryUpdateDto
                        {
                            Warehouse = detail.Warehouse,
                            Location = detail.Location,
                            Qty = detail.Qty,
                            LocationStock = detail.LocationStock,
                        }).ToList()));
                });  

                var mapper = mapperConfiguration.CreateMapper();
                var materialReturnNoteDetails = materialReturnNoteItemList.Select(item => mapper.Map<MRNUpdateInventoryBalanceQty>(item)).ToList();
                 var json = JsonConvert.SerializeObject(materialReturnNoteDetails);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "MaterialReturnNoteInventoryBalanceQty"), data);

                var client1 = _clientFactory.CreateClient();
                var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                "MaterialReturnNoteInventoryBalanceQty"))
                {
                    Content = data
                };
                request1.Headers.Add("Authorization", token1);

                var response = await client1.SendAsync(request1);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    updateMaterialReturnNoteResp = response.StatusCode;
                }

                materialReturnNoteDetail.MaterialReturnNoteItems = materialReturnNoteItemList;
                var updateMaterialReturnNoteItem = _mapper.Map(materialReturnNoteUpdateDto, materialReturnNoteDetail);
                int? totalitems = updateMaterialReturnNoteItem.MaterialReturnNoteItems.Count();
                if (totalitems > 0)
                {
                    if ((updateMaterialReturnNoteItem.MaterialReturnNoteItems.Where(x => x.MrnStatus == MaterialStatus.Open).Count()) == totalitems) updateMaterialReturnNoteItem.MrnStatus = MaterialStatus.Open;
                    else if ((updateMaterialReturnNoteItem.MaterialReturnNoteItems.Where(x => x.MrnStatus == MaterialStatus.Closed).Count()) == totalitems) updateMaterialReturnNoteItem.MrnStatus = MaterialStatus.Closed;
                    else if (((updateMaterialReturnNoteItem.MaterialReturnNoteItems.Where(x => x.MrnStatus == MaterialStatus.PartiallyClosed).Count()) > 0) || (updateMaterialReturnNoteItem.MaterialReturnNoteItems.Where(x => x.MrnStatus == MaterialStatus.Open).Count() > 0)) updateMaterialReturnNoteItem.MrnStatus = MaterialStatus.PartiallyClosed;

                }
                string result = await _materialReturnNoteRepository.UpdateMaterialReturnNote(updateMaterialReturnNoteItem);

                if (updateMaterialReturnNoteResp == HttpStatusCode.OK)
                {
                    _materialReturnNoteRepository.SaveAsync();
                }
                else
                {
                    _logger.LogError($"Something went wrong inside ReturnMaterialReturnNote action. Inventory update action MaterialReturnNoteInventoryBalanceQty failed! ");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Internal server error";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialReturnNote Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateMaterialReturnNote action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialReturnNote(int id)
        {
            ServiceResponse<MaterialReturnNoteDto> serviceResponse = new ServiceResponse<MaterialReturnNoteDto>();

            try
            {
                var materialReturnNoteDtailsById = await _materialReturnNoteRepository.GetMaterialReturnNoteById(id);
                if (materialReturnNoteDtailsById == null)
                {
                    _logger.LogError($"DeleteMaterialReturnNote with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DeleteMaterialReturnNote with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _materialReturnNoteRepository.DeleteMaterialReturnNote(materialReturnNoteDtailsById);
                _logger.LogError(result);
                _materialReturnNoteRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialReturnNote Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteMaterialReturnNote action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMaterialReturnNoteIdNameList()
        {
            ServiceResponse<IEnumerable<MaterialReturnNoteIdNameList>> serviceResponse = new ServiceResponse<IEnumerable<MaterialReturnNoteIdNameList>>();
            try
            {
                var listOfAllmaterialReturnNotesIdNames = await _materialReturnNoteRepository.GetAllMaterialReturnNoteIdNameList();
                var result = _mapper.Map<IEnumerable<MaterialReturnNoteIdNameList>>(listOfAllmaterialReturnNotesIdNames);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All listOfAllmaterialReturnNotesIdNames";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllMaterialReturnNoteIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> SearchMaterialReturnNote([FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<MaterialReturnNoteDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialReturnNoteDto>>();
            try
            {
                var materialReturnNotesDetails = await _materialReturnNoteRepository.SearchMaterialReturnNote(searchParams);

                _logger.LogInfo("Returned all materialReturnNotesDetails");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<MaterialReturnNoteDto, MaterialReturnNote>().ReverseMap()
                    .ForMember(dest => dest.MaterialReturnNoteItems, opt => opt.MapFrom(src => src.MaterialReturnNoteItems));
                });
                var mapper = config.CreateMapper();

                var result = mapper.Map<IEnumerable<MaterialReturnNoteDto>>(materialReturnNotesDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all materialReturnNotesDetails";
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
        public async Task<IActionResult> GetAllMaterialReturnNoteWithItems([FromBody] MaterialReturnNoteSearchDto materialReturnNote)
        {
            ServiceResponse<IEnumerable<MaterialReturnNoteDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialReturnNoteDto>>();
            try
            {
                var materialReturnNotesDetails = await _materialReturnNoteRepository.GetAllMaterialReturnNoteWithItems(materialReturnNote);
                _logger.LogInfo("Returned all materialRequestDetails");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<MaterialReturnNoteDto, MaterialReturnNote>().ReverseMap()
                    .ForMember(dest => dest.MaterialReturnNoteItems, opt => opt.MapFrom(src => src.MaterialReturnNoteItems));
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<MaterialReturnNoteDto>>(materialReturnNotesDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all materialReturnNotesDetails";
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
        public async Task<IActionResult> SearchMaterialReturnNoteDate([FromQuery] SearchDateparames searchDateParam)
        {
            ServiceResponse<IEnumerable<MaterialReturnNoteDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialReturnNoteDto>>();
            try
            {
                var materialReturnNotesDetails = await _materialReturnNoteRepository.SearchMaterialReturnNoteDate(searchDateParam);
                var result = _mapper.Map<IEnumerable<MaterialReturnNoteDto>>(materialReturnNotesDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all materialReturnNotesDetails";
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
