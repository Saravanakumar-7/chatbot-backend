using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class MaterialTransactionNoteController : ControllerBase
    {
        private IMaterialTransactionNoteRepository _materialTransactionNoteRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly IHttpClientFactory _clientFactory;
        public MaterialTransactionNoteController(IMaterialTransactionNoteRepository materialTransactionNoteRepository, IHttpClientFactory clientFactory, ILoggerManager logger, IMapper mapper)
        {
            _materialTransactionNoteRepository = materialTransactionNoteRepository;
            _logger = logger;
            _mapper = mapper;
            _clientFactory = clientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMaterialTransactionNote([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<MaterialTransactionNoteDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialTransactionNoteDto>>();

            try
            {
                var listOfmtn = await _materialTransactionNoteRepository.GetAllMaterialTransactionNote(pagingParameter, searchParammes);
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
                _logger.LogError($"Error Occured in GetAllMaterialTransactionNote API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllMaterialTransactionNote API : \n {ex.Message} ";
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
                var MTNDetails = await _materialTransactionNoteRepository.GetMaterialTransactionNoteById(id);

                if (MTNDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"materialtransaction with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"materialtransaction with id: {id}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogError($"Returned materialtransaction with id: {id}");
                    MaterialTransactionNoteDto materialTransactionNoteDto = _mapper.Map<MaterialTransactionNoteDto>(MTNDetails);//Main model mapping


                    List<MaterialTransactionNoteItemDto> materialTransactionNoteItemDtos = new List<MaterialTransactionNoteItemDto>();

                    foreach (var materialitemDetails in materialTransactionNoteDto.MaterialTransactionNoteItemDtos)
                    {
                        MaterialTransactionNoteItemDto materialTransactionNoteItemDto = _mapper.Map<MaterialTransactionNoteItemDto>(materialitemDetails);
                        materialTransactionNoteItemDtos.Add(materialTransactionNoteItemDto);
                    }

                    materialTransactionNoteDto.MaterialTransactionNoteItemDtos = materialTransactionNoteItemDtos;
                    serviceResponse.Data = materialTransactionNoteDto;
                    serviceResponse.Message = $"Returned materialtransactionnotes with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetMaterialTransactionNoteById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetMaterialTransactionNoteById API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateMaterialTransactionNote([FromBody] MaterialTransactionNotePostDto materialTransactionNotePost)
        {
            ServiceResponse<MaterialTransactionNoteDto> serviceResponse = new ServiceResponse<MaterialTransactionNoteDto>();

            try
            {
                if (materialTransactionNotePost is null)
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
                var mtn = _mapper.Map<MaterialTransactionNote>(materialTransactionNotePost);
                var mtndto = materialTransactionNotePost.MaterialTransactionNoteItemPostDtos;

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
                serviceResponse.Message = "MaterialTransactionNote Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetMaterialTransactionNoteById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateMaterialTransactionNote API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateMaterialTransactionNote API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterialTransactionNote(int id, [FromBody] MaterialTransactionNoteUpdateDto materialTransactionNoteUpdateDto)
        {
            ServiceResponse<MaterialTransactionNoteDto> serviceResponse = new ServiceResponse<MaterialTransactionNoteDto>();

            try
            {
                if (materialTransactionNoteUpdateDto is null)
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
                    serviceResponse.Message = $"Update MaterialTransactionNote with id: {id}, hasn't been found.";
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

                var updateMTN = _mapper.Map(materialTransactionNoteUpdateDto, MtnList);

                string result = await _materialTransactionNoteRepository.UpdateMaterialTransactionNote(updateMTN);
                _materialTransactionNoteRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialTransactionNote Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateMaterialTransactionNote API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateMaterialTransactionNote API for the following id:{id} \n {ex.Message}";
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
                serviceResponse.Message = "MaterialTransactionNote Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeleteMaterialTransactionNote API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteMaterialTransactionNote API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}