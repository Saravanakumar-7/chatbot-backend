using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    // Materialrequest Controller
    public class MaterialReturnNoteController : ControllerBase
    {
        private IMaterialReturnNoteRepository _materialReturnNoteRepository;
        private IMapper _mapper;
        private ILoggerManager _logger;


        public MaterialReturnNoteController(IMaterialReturnNoteRepository materialReturnNoteRepository, IMapper mapper, ILoggerManager logger)
        {
            _materialReturnNoteRepository = materialReturnNoteRepository;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMaterialReturnNote([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<MaterialReturnNoteDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialReturnNoteDto>>();

            try
            {
                var getAllMaterialReturnNote = await _materialReturnNoteRepository.GetAllMaterialReturnNote(pagingParameter);
                var metadata = new
                {
                    getAllMaterialReturnNote.TotalCount,
                    getAllMaterialReturnNote.PageSize,
                    getAllMaterialReturnNote.CurrentPage,
                    getAllMaterialReturnNote.HasNext,
                    getAllMaterialReturnNote.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogError("Returned all getAllMaterialReturnNote");
                var result = _mapper.Map<IEnumerable<MaterialReturnNoteDto>>(getAllMaterialReturnNote);
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
                    serviceResponse.Message = $"materialReturnNoteDetail with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"materialReturnNoteDetail with id: {id}, hasn't been found in db.");
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
                            materialReturnNoteItemDtos.Add(materialReturnNoteItemDto);
                        }
                    }

                    materialReturnNoteDto.MaterialReturnNoteItems = materialReturnNoteItemDtos;
                    serviceResponse.Data = materialReturnNoteDto;
                    serviceResponse.Message = $"Returned materialReturnNote with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside materialReturnNoteDetailbyId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateMaterialReturnNote([FromBody] MaterialReturnNoteDtoPost materialReturnNoteDtoPost)
        {
            ServiceResponse<MaterialReturnNoteDto> serviceResponse = new ServiceResponse<MaterialReturnNoteDto>();

            try
            {
                if (materialReturnNoteDtoPost is null)
                {
                    _logger.LogError("materialReturnNote object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "materialReturnNote object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid materialReturnNote object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid materialReturnNote object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var createMaterialReturnNote = _mapper.Map<MaterialReturnNote>(materialReturnNoteDtoPost);
                var materialReturnNoteItemDto = materialReturnNoteDtoPost.MaterialReturnNoteItems;

                var materialReturnNoteItemList = new List<MaterialReturnNoteItem>();

                if (materialReturnNoteItemDto != null)
                {

                    for (int i = 0; i < materialReturnNoteItemDto.Count; i++)
                    {
                        MaterialReturnNoteItem materialReturnNoteItem = _mapper.Map<MaterialReturnNoteItem>(materialReturnNoteItemDto[i]);
                        materialReturnNoteItemList.Add(materialReturnNoteItem);

                    }
                }

                createMaterialReturnNote.MaterialReturnNoteItems = materialReturnNoteItemList;

                await _materialReturnNoteRepository.CreateMaterialReturnNote(createMaterialReturnNote);

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
        public async Task<IActionResult> UpdateMaterialReturnNote(int id, [FromBody] MaterialReturnNoteDtoUpdate materialReturnNoteDtoUpdate)
        {
            ServiceResponse<MaterialReturnNoteDto> serviceResponse = new ServiceResponse<MaterialReturnNoteDto>();

            try
            {
                if (materialReturnNoteDtoUpdate is null)
                {
                    _logger.LogError("MaterialReturnNote object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update MaterialReturnNote object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid MaterialReturnNote object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update MaterialReturnNote object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var getMaterialReturnNoteById = await _materialReturnNoteRepository.GetMaterialReturnNoteById(id);

                if (getMaterialReturnNoteById is null)
                {
                    _logger.LogError($"GetMaterialReturnNote with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update GetMaterialReturnNote with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var updateMaterialReturnNote = _mapper.Map<MaterialReturnNote>(getMaterialReturnNoteById);

                var materialReturnNotesItemDto = materialReturnNoteDtoUpdate.MaterialReturnNoteItems;

                var materialReturnNoteItemList = new List<MaterialReturnNoteItem>();

                for (int i = 0; i < materialReturnNotesItemDto.Count; i++)
                {
                    MaterialReturnNoteItem materialReturnNoteItem = _mapper.Map<MaterialReturnNoteItem>(materialReturnNotesItemDto[i]);
                    materialReturnNoteItemList.Add(materialReturnNoteItem);

                }

                var updateMaterialReturnNoteItem = _mapper.Map(materialReturnNoteDtoUpdate, getMaterialReturnNoteById);

                updateMaterialReturnNoteItem.MaterialReturnNoteItems = materialReturnNoteItemList;
                string result = await _materialReturnNoteRepository.UpdateMaterialReturnNote(updateMaterialReturnNoteItem);

                _materialReturnNoteRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
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
        public async Task<IActionResult> DeleteMaterialReturnNote(int id)
        {
            ServiceResponse<MaterialReturnNoteDto> serviceResponse = new ServiceResponse<MaterialReturnNoteDto>();

            try
            {
                var getMaterialReturnNoteById = await _materialReturnNoteRepository.GetMaterialReturnNoteById(id);
                if (getMaterialReturnNoteById == null)
                {
                    _logger.LogError($"DeleteMaterialReturnNote with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DeleteMaterialReturnNote with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _materialReturnNoteRepository.DeleteMaterialReturnNote(getMaterialReturnNoteById);
                _logger.LogError(result);
                _materialReturnNoteRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
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



    }
}
