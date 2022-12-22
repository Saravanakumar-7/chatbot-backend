using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using System.Net;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MaterialTransactionNoteController : ControllerBase
    {
        private IMaterialTransactionNoteRepository _materialTransactionNoteRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public MaterialTransactionNoteController(IMaterialTransactionNoteRepository materialTransactionNoteRepository, ILoggerManager logger, IMapper mapper)
        {
            _materialTransactionNoteRepository = materialTransactionNoteRepository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMaterialTransactionNote([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<MaterialTransactionNoteDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialTransactionNoteDto>>();

            try
            {
                var listOfmtn = await _materialTransactionNoteRepository.GetAllMaterialTransactionNote(pagingParameter);
                var metadata = new
                {
                    listOfmtn.TotalCount,
                    listOfmtn.PageSize,
                    listOfmtn.CurrentPage,
                    listOfmtn.HasNext,
                    listOfmtn.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogError("Returned all listOfMaterialTransactionNote");
                var result = _mapper.Map<IEnumerable<MaterialTransactionNoteDto>>(listOfmtn);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialTransactionNote Successfully";
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
        public async Task<IActionResult> GetMaterialTransactionNoteById(int id)
        {
            ServiceResponse<MaterialTransactionNoteDto> serviceResponse = new ServiceResponse<MaterialTransactionNoteDto>();

            try
            {
                var mtnId = await _materialTransactionNoteRepository.GetMaterialTransactionNoteById(id);

                if (mtnId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"materialtransaction with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"materialtransaction with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogError($"Returned materialtransaction with id: {id}");
                    MaterialTransactionNoteDto materialTransactionNoteDto = _mapper.Map<MaterialTransactionNoteDto>(mtnId);//Main model mapping

                    //below mapping is child under child  

                    List<MaterialTransactionNoteItemDto> materialTransactionNoteItemDtos = new List<MaterialTransactionNoteItemDto>();

                    foreach (var materialitemDetails in materialTransactionNoteDto.MaterialTransactionNoteItems)
                    {
                        MaterialTransactionNoteItemDto materialTransactionNoteItemDto = _mapper.Map<MaterialTransactionNoteItemDto>(materialitemDetails);
                        materialTransactionNoteItemDtos.Add(materialTransactionNoteItemDto);
                    }

                    materialTransactionNoteDto.MaterialTransactionNoteItems = materialTransactionNoteItemDtos;
                    serviceResponse.Data = materialTransactionNoteDto;
                    serviceResponse.Message = $"Returned materialtransactionnotes with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetmaterialtransactionnotesById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateMaterialTransactionNote([FromBody] MaterialTransactionNoteDtoPost materialTransactionNoteDtoPost)
        {
            ServiceResponse<MaterialTransactionNoteDto> serviceResponse = new ServiceResponse<MaterialTransactionNoteDto>();

            try
            {
                if (materialTransactionNoteDtoPost is null)
                {
                    _logger.LogError("MaterialTransactionNote object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "MaterialTransactionNote object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid MaterialTransactionNote object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid MaterialTransactionNote object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var mtn = _mapper.Map<MaterialTransactionNote>(materialTransactionNoteDtoPost);
                var mtndto = materialTransactionNoteDtoPost.MaterialTransactionNoteItems;

                var MtnItemList = new List<MaterialTransactionNoteItem>();
                for (int i = 0; i < mtndto.Count; i++)
                {
                    MaterialTransactionNoteItem materialItemListDetail = _mapper.Map<MaterialTransactionNoteItem>(mtndto[i]);
                    MtnItemList.Add(materialItemListDetail);

                }
                mtn.MaterialTransactionNoteItems = MtnItemList;

                _materialTransactionNoteRepository.CreateMaterialTransactionNote(mtn);

                _materialTransactionNoteRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetMaterialTransactionNoteById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateMaterialTransactionNote action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterialTransactionNote(int id, [FromBody] MaterialTransactionNoteDtoUpdate materialTransactionNoteDtoUpdate)
        {
            ServiceResponse<MaterialTransactionNoteDto> serviceResponse = new ServiceResponse<MaterialTransactionNoteDto>();

            try
            {
                if (materialTransactionNoteDtoUpdate is null)
                {
                    _logger.LogError("MaterialTransactionNote object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update MaterialTransactionNote object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid MaterialTransactionNote object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update MaterialTransactionNoteDto object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var Mtn = await _materialTransactionNoteRepository.GetMaterialTransactionNoteById(id);
                if (Mtn is null)
                {
                    _logger.LogError($"MaterialTransactionNote with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update MaterialTransactionNote with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var MtnList = _mapper.Map<MaterialTransactionNote>(Mtn);

                var MtnItemDto = Mtn.MaterialTransactionNoteItems;

                var MattransList = new List<MaterialTransactionNoteItem>();
                for (int i = 0; i < MtnItemDto.Count; i++)
                {
                    MaterialTransactionNoteItem MtnItemDetail = _mapper.Map<MaterialTransactionNoteItem>(MtnItemDto[i]);
                    MattransList.Add(MtnItemDetail);

                }

                MtnList.MaterialTransactionNoteItems = MattransList;

                var data = _mapper.Map(materialTransactionNoteDtoUpdate, MtnList);

                string result = await _materialTransactionNoteRepository.UpdateMaterialTransactionNote(data);
                _materialTransactionNoteRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateMaterialTransactionNote action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialTransactionNote(int id)
        {
            ServiceResponse<MaterialTransactionNoteDto> serviceResponse = new ServiceResponse<MaterialTransactionNoteDto>();

            try
            {
                var DelMtn = await _materialTransactionNoteRepository.GetMaterialTransactionNoteById(id);
                if (DelMtn == null)
                {
                    _logger.LogError($"DeletematerialTransactionnote with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DeletematerialTransactionnote with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _materialTransactionNoteRepository.DeleteMaterialTransactionNote(DelMtn);
                _logger.LogError(result);
                _materialTransactionNoteRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteMaterialTransactionNote action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}