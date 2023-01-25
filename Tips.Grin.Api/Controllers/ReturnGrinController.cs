using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Entities;
using AutoMapper;
using Tips.Grin.Api.Contracts;
using Contracts;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Entities;
using Newtonsoft.Json;
using Entities.DTOs;
using Tips.Grin.Api.Repository;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text;
using System.Dynamic;
using Azure.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReturnGrinController : ControllerBase
    {
        private IReturnGrinRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public ReturnGrinController(IReturnGrinRepository repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }



        [HttpGet]

        public async Task<IActionResult> GetAllReturnGrin([FromQuery] PagingParameter pagingParameter)

        {
            ServiceResponse<IEnumerable<ReturnGrinDto>> serviceResponse = new ServiceResponse<IEnumerable<ReturnGrinDto>>();

            try
            {
                var GetallReturnGrins = await _repository.GetAllReturnGrin(pagingParameter);

                var metadata = new
                {
                    GetallReturnGrins.TotalCount,
                    GetallReturnGrins.PageSize,
                    GetallReturnGrins.CurrentPage,
                    GetallReturnGrins.HasNext,
                    GetallReturnGrins.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all ReturnGrins");
                var result = _mapper.Map<IEnumerable<ReturnGrinDto>>(GetallReturnGrins);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Grins Successfully";
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

         [HttpPost]
        public async Task<IActionResult> CreateReturnGrin([FromBody] ReturnGrinDtoPost returnGrinDtoPost)
        {
            ServiceResponse<ReturnGrinDto> serviceResponse = new ServiceResponse<ReturnGrinDto>();

            try
            {
                if (returnGrinDtoPost is null)
                {
                    _logger.LogError("returnGrin object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "returnGrin object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid returnGrin object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid returnGrin object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var returnGrins = _mapper.Map<ReturnGrin>(returnGrinDtoPost);

                var returnGrinDto = returnGrinDtoPost.ReturnGrinParts;

                var returnGrinPartsList = new List<ReturnGrinParts>();

                if (returnGrinDto != null)
                {
                    for (int i = 0; i < returnGrinDto.Count; i++)
                    {
                        ReturnGrinParts returnGrinParts = _mapper.Map<ReturnGrinParts>(returnGrinDto[i]);
                        returnGrinPartsList.Add(returnGrinParts);


                    }
                }


                returnGrins.ReturnGrinParts = returnGrinPartsList;

                await _repository.CreateReturnGrin(returnGrins);

                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = "ReturnGrin Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetReturnGrinById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateReturnGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

      
                [HttpDelete("{id}")]

                public async Task<IActionResult> DeleteReturnGrin(int id)
                {
                    ServiceResponse<ReturnGrinDto> serviceResponse = new ServiceResponse<ReturnGrinDto>();

                    try
                    {
                        var deleteReturnGrin = await _repository.GetReturnGrinDetailsbyId(id);
                        if (deleteReturnGrin == null)
                        {
                            _logger.LogError($"Delete Returngrin with id: {id}, hasn't been found in db.");
                            serviceResponse.Data = null;
                            serviceResponse.Message = $"Delete Returngrin with id hasn't been found in db.";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.NotFound;
                            return NotFound(serviceResponse);
                        }
                        string result = await _repository.DeleteReturnGrin(deleteReturnGrin);
                        _logger.LogInfo(result);
                        _repository.SaveAsync();
                        serviceResponse.Data = null;
                        serviceResponse.Message = "ReturnGrin Deleted Successfully";
                        serviceResponse.Success = true;
                        serviceResponse.StatusCode = HttpStatusCode.OK;
                        return Ok(serviceResponse);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Something went wrong inside DeleteReturnGrin action: {ex.Message}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Internal server error";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }
                }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReturnGrinDetailsbyId(int id)
        {
            ServiceResponse<ReturnGrinDto> serviceResponse = new ServiceResponse<ReturnGrinDto>();

            try
            {
                var returnGrinById = await _repository.GetReturnGrinDetailsbyId(id);
                if (returnGrinById == null)
                {
                    _logger.LogError($"Binning details with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Binning details with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Binnings with id: {id}");

                    ReturnGrinDto returnGrinDto = _mapper.Map<ReturnGrinDto>(returnGrinById);



                    List<ReturnGrinPartsDto> returnGrinPartsDtos = new List<ReturnGrinPartsDto>();

                    if (returnGrinById.ReturnGrinParts != null)
                    {
                        foreach (var returnGrinPartsDetails in returnGrinById.ReturnGrinParts)
                        {
                            ReturnGrinPartsDto returnGrinPartsDto = _mapper.Map<ReturnGrinPartsDto>(returnGrinPartsDetails);
                            returnGrinPartsDtos.Add(returnGrinPartsDto);
                        }
                    }

                    returnGrinDto.ReturnGrinParts = returnGrinPartsDtos;
                    serviceResponse.Data = returnGrinDto;
                    serviceResponse.Message = $"Returned BinningbyId with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside BinningById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{partNo}")]
        public async Task<IActionResult> ReturnGrinPartsByPartNumber(string partNo)
        {
            ServiceResponse<IEnumerable<ReturnGrinPartsListDto>> serviceResponse = new ServiceResponse<IEnumerable<ReturnGrinPartsListDto>>();
            try
            {
                var getReturnGrinByPartNo = await _repository.ReturnGrinPartsByPartNumber(partNo);

                var result = _mapper.Map<IEnumerable<ReturnGrinPartsListDto>>(getReturnGrinByPartNo);
                serviceResponse.Data = result;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside ReturnGrinPartsByPartNumber action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }


} 
