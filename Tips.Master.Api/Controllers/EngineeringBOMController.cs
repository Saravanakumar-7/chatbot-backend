using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using AutoMapper;
using Contracts;
using Microsoft.EntityFrameworkCore;
using Entities.Migrations;
using System.Net;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EngineeringBOMController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public EngineeringBOMController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<EngineeringBOMController>
        [HttpGet]
        public async Task<IActionResult> GetAllEnggBOM([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<EnggBomDto>> serviceResponse = new ServiceResponse<IEnumerable<EnggBomDto>>();

            try
            {
                var listOfBoms = await _repository.EnggBomRepository.GetAllEnggBOM(pagingParameter);

                var metadata = new
                {
                    listOfBoms.TotalCount,
                    listOfBoms.PageSize,
                    listOfBoms.CurrentPage,
                    listOfBoms.HasNext,
                    listOfBoms.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                 


                _logger.LogInfo("Returned all Boms");
                var bomDtoDetails = _mapper.Map<IEnumerable<EnggBomDto>>(listOfBoms);

                //var bomDtoDetails = new List<EnggBomDto>();

                //foreach (var bom in listOfBoms)
                //{
                //    EnggBomDto enggBomDto = _mapper.Map<EnggBomDto>(bom);
                //    List<EnggChildItemDto> childItemsDtos = new List<EnggChildItemDto>();   
                //    foreach (var itemDetails in bom.EnggChildItems)
                //    {
                //        EnggChildItemDto enggChildItemDto = _mapper.Map<EnggChildItemDto>(itemDetails);
                //        enggChildItemDto.EnggAlternatesDtos = _mapper.Map<List<EnggAlternatesDto>>(itemDetails.EnggAlternates);
                //        enggChildItemDto.BomNREConsumableDto = _mapper.Map<BomNREConsumableDto>(itemDetails.NREConsumable);
                //        childItemsDtos.Add(enggChildItemDto);
                //    }
                //    enggBomDto.EnggChildItemDtos = childItemsDtos;
                //    bomDtoDetails.Add(enggBomDto);
                //}

                serviceResponse.Data = bomDtoDetails;
                serviceResponse.Message = "Returned all Engineering Boms Successfully";
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

        // GET api/<EngineeringBOMController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnggBomById(int id)
        {
            ServiceResponse<EnggBomDto> serviceResponse = new ServiceResponse<EnggBomDto>();

            try
            {
                var bom = await _repository.EnggBomRepository.GetEnggBomById(id);

               

                if (bom == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Engineering Bom with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Engineering Bom with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Engineering Bom with id: {id}");
                     EnggBomDto enggBomDto = _mapper.Map<EnggBomDto>(bom);
                    List<EnggChildItemDto> childItemsDtos = new List<EnggChildItemDto>();
                    foreach (var itemDetails in bom.EnggChildItems)
                    {
                        EnggChildItemDto enggChildItemDto = _mapper.Map<EnggChildItemDto>(itemDetails);
                        enggChildItemDto.EnggAlternatesDtos = _mapper.Map<List<EnggAlternatesDto>>(itemDetails.EnggAlternates);
                        enggChildItemDto.BomNREConsumableDto = _mapper.Map<BomNREConsumableDto>(itemDetails.NREConsumable);
                        childItemsDtos.Add(enggChildItemDto);
                    }
                    enggBomDto.EnggChildItemDtos = childItemsDtos;
                    serviceResponse.Data = enggBomDto;
                    serviceResponse.Message = $"Returned Engineering Bom with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
               
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEngineeringBomById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<EngineeringBOMController>
        [HttpPost]
        public async Task<IActionResult> CreateEnggBom([FromBody] EnggBomPostDto enggBomPostDto)
        {
            ServiceResponse<EnggBomDto> serviceResponse = new ServiceResponse<EnggBomDto>();

            try
            {
                if (enggBomPostDto is null)
                {
                    _logger.LogError("Engineering Bom object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Engineering Bom object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Engineering Bom object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Engineering Bom object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var enggBomList = _mapper.Map<EnggBom>(enggBomPostDto);


                var enggChildItemDto = enggBomPostDto.EnggChildItemPosts;

                var enggChildItemList = new List<EnggChildItem>();
                for (int i = 0; i < enggChildItemDto.Count; i++)
                {
                    EnggChildItem enggChildItemDetail = _mapper.Map<EnggChildItem>(enggChildItemDto[i]);
                    enggChildItemDetail.EnggAlternates = _mapper.Map<List<EnggAlternates>>(enggChildItemDto[i].EnggAlternatesPostDtos);
                    enggChildItemDetail.NREConsumable = _mapper.Map<NREConsumable>(enggChildItemDto[i].BomNREConsumablePostDto);
                    enggChildItemList.Add(enggChildItemDetail);

                }
                enggBomList.EnggChildItems = enggChildItemList;
                
                _repository.EnggBomRepository.CreateEnggBom(enggBomList);

                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetEnggBomById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateEnggBom action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<EngineeringBOMController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnggBom(int id, [FromBody] EnggBomUpdateDto enggBomDto)
        {
            ServiceResponse<EnggBomDto> serviceResponse = new ServiceResponse<EnggBomDto>();

            try
            {
                if (enggBomDto is null)
                {
                    _logger.LogError("Update EngineeringBom object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update EngineeringBom object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update EngineeringBom object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update EngineeringBom object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updateEnggBom = await _repository.EnggBomRepository.GetEnggBomById(id);
                if (updateEnggBom is null)
                {
                    _logger.LogError($"Update EngineeringBom with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update EngineeringBom with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var enggBomList = _mapper.Map<EnggBom>(updateEnggBom);

                var enggChildItemDto = enggBomDto.EnggChildItemUpdates;

                var enggChildItemList = new List<EnggChildItem>();
                for (int i = 0; i < enggChildItemDto.Count; i++)
                {
                    EnggChildItem enggChildItemDetail = _mapper.Map<EnggChildItem>(enggChildItemDto[i]);
                    enggChildItemDetail.EnggAlternates = _mapper.Map<List<EnggAlternates>>(enggChildItemDto[i].EnggAlternatesUpdateDtos);
                    enggChildItemDetail.NREConsumable = _mapper.Map<NREConsumable>(enggChildItemDto[i].BomNREConsumableUpdateDto);
                    enggChildItemList.Add(enggChildItemDetail);

                }
                enggBomList.EnggChildItems = enggChildItemList;

                 var data = _mapper.Map(enggBomDto, enggBomList);

                string result = await _repository.EnggBomRepository.UpdateEnggBom(data);


                //var address = _mapper.Map<IEnumerable<VendorAddress>>(vendorMasterUpdateDto.Addresses);

                //var contact = _mapper.Map<IEnumerable<VendorContacts>>(vendorMasterUpdateDto.Contacts);

                //var banking = _mapper.Map<IEnumerable<VendorBanking>>(vendorMasterUpdateDto.VendorBankings);

                //var Headcount = _mapper.Map<IEnumerable<HeadCounting>>(vendorMasterUpdateDto.HeadCountings);

                //var data = _mapper.Map(vendorMasterUpdateDto, updatevendor);


                //data.Addresses = address.ToList();
                //data.Contacts = contact.ToList();
                //data.VendorBankings = banking.ToList();
                //data.HeadCountings = Headcount.ToList();

                //string result = await _repository.VendorRepository.UpdateVendor(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateEngineering Bom action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<EngineeringBOMController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // GET: api/<ReleaseEnggBomController>
        [HttpGet]
        public async Task<IActionResult> GetAllReleaseEnggBom([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ReleaseEnggBomDto>> serviceResponse = new ServiceResponse<IEnumerable<ReleaseEnggBomDto>>();

            try
            {
                var listOfReleaseEnggBom = await _repository.releaseEnggBomRepository.GetAllReleaseEnggBom(pagingParameter);

                var metadata = new
                {
                    listOfReleaseEnggBom.TotalCount,
                    listOfReleaseEnggBom.PageSize,
                    listOfReleaseEnggBom.CurrentPage,
                    listOfReleaseEnggBom.HasNext,
                    listOfReleaseEnggBom.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ReleaseEnggBom");
                var releaseEnggBomDtoDetails = _mapper.Map<IEnumerable<ReleaseEnggBomDto>>(listOfReleaseEnggBom);
                serviceResponse.Data = releaseEnggBomDtoDetails;
                serviceResponse.Message = "Returned all ReleaseEnggBom  Successfully";
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

        // GET api/<ReleaseEnggBomController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReleaseEnggBomById(int id)
        {
            ServiceResponse<ReleaseEnggBomDto> serviceResponse = new ServiceResponse<ReleaseEnggBomDto>();

            try
            {
                var releaseEnggBomDtoDetails = await _repository.releaseEnggBomRepository.GetReleaseEnggBomById(id);

                if (releaseEnggBomDtoDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ReleaseEnggBomDetails hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ReleaseEnggBomDetails with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ReleaseEnggBomDetails with id: {id}");
                    var result = _mapper.Map<ReleaseEnggBomDto>(releaseEnggBomDtoDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned ReleaseEnggBomDetails with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetReleaseEnggBomById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<ReleaseEnggBomController>
        [HttpPost]
        public async Task<IActionResult> CreateReleaseEnggBom([FromBody] ReleaseEnggBomDtoPost releaseEnggBomDtoPost)
        {
            ServiceResponse<ReleaseEnggBomDtoPost> serviceResponse = new ServiceResponse<ReleaseEnggBomDtoPost>();

            try
            {
                if (releaseEnggBomDtoPost is null)
                {
                    _logger.LogError("ReleaseEnggBom object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ReleaseEnggBom object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ReleaseEnggBom object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ReleaseEnggBom object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var release = _mapper.Map <ReleaseEnggBom> (releaseEnggBomDtoPost);
                _repository.releaseEnggBomRepository.CreateReleaseEnggBom(release);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ReleaseEnggBom Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetReleaseEnggBomById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ReleaseEnggBom action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // PUT api/<ReleaseEnggBomController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReleaseEnggBom(int id, [FromBody] ReleaseEnggBomDtoUpdate releaseEnggBomDtoUpdate)
        {
            ServiceResponse<ReleaseEnggBomDtoUpdate> serviceResponse = new ServiceResponse<ReleaseEnggBomDtoUpdate>();

            try
            {
                if (releaseEnggBomDtoUpdate is null)
                {
                    _logger.LogError("Update ReleaseEnggBom object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ReleaseEnggBom object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update ReleaseEnggBom object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update ReleaseEnggBom object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updateReleaseEnggBom = await _repository.releaseEnggBomRepository.GetReleaseEnggBomById(id);
                if (updateReleaseEnggBom is null)
                {
                    _logger.LogError($" updateReleaseEnggBom with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $" updateReleaseEnggBom hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var data = _mapper.Map(releaseEnggBomDtoUpdate, updateReleaseEnggBom);
                string result = await _repository.releaseEnggBomRepository.UpdateReleaseEnggBom(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ReleaseEnggBom Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside updateReleaseEnggBom action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // DELETE api/<ReleaseEnggBomController>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReleaseEnggBom(int id)
        {
            ServiceResponse<ReleaseEnggBomDto> serviceResponse = new ServiceResponse<ReleaseEnggBomDto>();

            try
            {
                var deleteReleaseEnggBom = await _repository.releaseEnggBomRepository.GetReleaseEnggBomById(id);
                if (deleteReleaseEnggBom == null)
                {
                    _logger.LogError($"deleteReleaseEnggBom  with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $" deleteReleaseEnggBom hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.releaseEnggBomRepository.DeleteReleaseEnggBom(deleteReleaseEnggBom);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ReleaseEnggBom Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside deleteReleaseEnggBom action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<ReleaseCostBomController>
        [HttpPost]
        public async Task<IActionResult> CreateReleaseCostBom([FromBody] ReleaseCostBomDtoPost releaseCostBomDtoPost)
        {
            ServiceResponse<ReleaseCostBomDtoPost> serviceResponse = new ServiceResponse<ReleaseCostBomDtoPost>();

            try
            {
                if (releaseCostBomDtoPost is null)
                {
                    _logger.LogError("ReleaseCostBom object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ReleaseCostBom object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ReleaseCostBom object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ReleaseCostBom object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var release = _mapper.Map<ReleaseCostBom>(releaseCostBomDtoPost);
                _repository.releaseCostBomRepository.CreateReleaseCostBom(release);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ReleaseCostBom Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetReleaseCostBomById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ReleaseCostBom action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // POST api/<ReleaseProductBomController>
        [HttpPost]
        public async Task<IActionResult> CreateReleaseProductBom([FromBody] ReleaseProductBomDtoPost releaseProductBomDtoPost)
        {
            ServiceResponse<ReleaseProductBomDtoPost> serviceResponse = new ServiceResponse<ReleaseProductBomDtoPost>();

            try
            {
                if (releaseProductBomDtoPost is null)
                {
                    _logger.LogError("ReleaseProductBom object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ReleaseProductBom object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ReleaseProductBom object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ReleaseProductBom object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var release = _mapper.Map<ReleaseProductBom>(releaseProductBomDtoPost);
                _repository.releaseProductBomRepository.CreateReleaseProductBom(release);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ReleaseProductBom Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetReleaseProductBomById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ReleaseProductBom action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
