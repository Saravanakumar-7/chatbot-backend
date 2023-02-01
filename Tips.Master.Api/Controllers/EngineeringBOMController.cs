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
using Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EngineeringBOMController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private IReleaseEnggBomRepository _releaseEnggBomRepository;
        private ILoggerManager _logger;
        private IReleaseProductBomRepository _releaseProductBomRepository;
        private IMapper _mapper;
        private IReleaseCostBomRepository _releaseCostBomRepository; 
 
        public EngineeringBOMController(IRepositoryWrapperForMaster repository,IReleaseProductBomRepository releaseProductBomRepository, IReleaseCostBomRepository releaseCostBomRepository, IReleaseEnggBomRepository releaseEnggBomRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _releaseCostBomRepository = releaseCostBomRepository;
             _releaseEnggBomRepository = releaseEnggBomRepository;
            _releaseProductBomRepository = releaseProductBomRepository;
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
                var bomDetail = await _repository.EnggBomRepository.GetEnggBomById(id);



                if (bomDetail == null)
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
                    EnggBomDto enggBomDto = _mapper.Map<EnggBomDto>(bomDetail);
                    List<EnggChildItemDto> childItemsDtos = new List<EnggChildItemDto>();
                    enggBomDto.BomNREConsumableDto = _mapper.Map<List<BomNREConsumableDto>>(bomDetail.NREConsumable);

                    if (bomDetail.EnggChildItems != null)
                    {
                        foreach (var itemDetails in bomDetail.EnggChildItems)
                        {
                            EnggChildItemDto enggChildItemDto = _mapper.Map<EnggChildItemDto>(itemDetails);
                            enggChildItemDto.EnggAlternatesDtos = _mapper.Map<List<EnggAlternatesDto>>(itemDetails.EnggAlternates);
                            childItemsDtos.Add(enggChildItemDto);
                        }
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



        [HttpGet("{fgPartNumber}")]
        public async Task<IActionResult> GetEnggBomByFgPartNumber(string fgPartNumber)
        {
            ServiceResponse<EnggBomDto> serviceResponse = new ServiceResponse<EnggBomDto>();

            try
            {
                var bom = await _repository.EnggBomRepository.GetEnggBomByFgPartNumber(fgPartNumber);
                if (bom == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Engineering Bom with fgPartNumber: {fgPartNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Engineering Bom with fgPartNumber: {fgPartNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Engineering Bom with fgPartNumber: {fgPartNumber}");
                    EnggBomDto enggBomDto = _mapper.Map<EnggBomDto>(bom);
                    List<EnggChildItemDto> childItemsDtos = new List<EnggChildItemDto>();
                    foreach (var itemDetails in bom.EnggChildItems)
                    {
                        EnggChildItemDto enggChildItemDto = _mapper.Map<EnggChildItemDto>(itemDetails);
                        enggChildItemDto.EnggAlternatesDtos = _mapper.Map<List<EnggAlternatesDto>>(itemDetails.EnggAlternates);
                        childItemsDtos.Add(enggChildItemDto);
                    }
                    enggBomDto.EnggChildItemDtos = childItemsDtos;
                    serviceResponse.Data = enggBomDto;
                    serviceResponse.Message = $"Returned Engineering Bom with fgPartNumber: {fgPartNumber}";
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

        ////get nre details passing item id
        //[HttpPost]
        //public async Task<IActionResult> GetAllNREConsumableList([FromBody] List<long> itemNumber)
        //{
        //    ServiceResponse<List<NREConsumable>> serviceResponse = new ServiceResponse<List<NREConsumable>>();
        //    List<NREConsumable> nreConsumables = null;
        //    try
        //    {
        //        if (itemNumber is null)
        //        {
        //            _logger.LogError("NREConsumable object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "NREConsumable object is null";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }

        //        nreConsumables = await _repository.EnggBomNREConsumableRepository.GetAllItemsProcessList(itemNumber);
        //        List<ItemMasterRouting> rfqCSDto = _mapper.Map<List<ItemMasterRouting>>(itemMasterRouting);
        //        rfqCSDto = itemMasterRouting;
        //        serviceResponse.Data = rfqCSDto;
        //        serviceResponse.Message = "List Of ItemNumber ";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside UpdateRfq action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}

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
                var enggnre = enggBomPostDto.BomNREConsumablePostDto;
                var nreList = new List<NREConsumable>();
                for (int i = 0; i < enggnre.Count; i++)
                {
                    NREConsumable enggChildItemDetails = _mapper.Map<NREConsumable>(enggnre[i]);
                    nreList.Add(enggChildItemDetails);

                }
                enggBomList.NREConsumable = nreList;

                var enggChildItemDto = enggBomPostDto.EnggChildItemPosts;

                var enggChildItemList = new List<EnggChildItem>();
                for (int i = 0; i < enggChildItemDto.Count; i++)
                {
                    EnggChildItem enggChildItemDetail = _mapper.Map<EnggChildItem>(enggChildItemDto[i]);
                    enggChildItemDetail.EnggAlternates = _mapper.Map<List<EnggAlternates>>(enggChildItemDto[i].EnggAlternatesPostDtos);
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
                    enggChildItemList.Add(enggChildItemDetail);

                }
                enggBomList.EnggChildItems = enggChildItemList;

                var data = _mapper.Map(enggBomDto, enggBomList);

                string result = await _repository.EnggBomRepository.UpdateEnggBom(data);


                
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
                var listOfReleaseEnggBom = await _releaseEnggBomRepository.GetAllReleaseEnggBom(pagingParameter);

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
                //var releaseEnggBomDtoDetails = await _repository.releaseEnggBomRepository.GetReleaseEnggBomById(id);
                var releaseEnggBomDtoDetails = await _releaseEnggBomRepository.GetReleaseEnggBomById(id);


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

                var release = _mapper.Map<ReleaseEnggBom>(releaseEnggBomDtoPost);
                //_repository.releaseEnggBomRepository.CreateReleaseEnggBom(release);
                _releaseEnggBomRepository.CreateReleaseEnggBom(release);
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
                var updateReleaseEnggBom = await _releaseEnggBomRepository.GetReleaseEnggBomById(id);
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
                string result = await _releaseEnggBomRepository.UpdateReleaseEnggBom(data);
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
                var deleteReleaseEnggBom = await _releaseEnggBomRepository.GetReleaseEnggBomById(id);
                if (deleteReleaseEnggBom == null)
                {
                    _logger.LogError($"deleteReleaseEnggBom  with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $" deleteReleaseEnggBom hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _releaseEnggBomRepository.DeleteReleaseEnggBom(deleteReleaseEnggBom);
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
                _releaseCostBomRepository.CreateReleaseCostBom(release);
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
                _releaseProductBomRepository.CreateReleaseProductBom(release);
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

        // GET: api/<EnggBomGroupController>
        [HttpGet]
        public async Task<IActionResult> GetAllEnggBomGroup([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<EnggBomGroupDto>> serviceResponse = new ServiceResponse<IEnumerable<EnggBomGroupDto>>();
            try
            {
                var listOfEnggBomGroup = await _repository.EnggBomGroupRepository.GetAllEnggBomGroup(pagingParameter);
                var metadata = new
                {
                    listOfEnggBomGroup.TotalCount,
                    listOfEnggBomGroup.PageSize,
                    listOfEnggBomGroup.CurrentPage,
                    listOfEnggBomGroup.HasNext,
                    listOfEnggBomGroup.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all EnggBomGroup");
                var enggbomGroupEntity = _mapper.Map<IEnumerable<EnggBomGroupDto>>(listOfEnggBomGroup);
                serviceResponse.Data = enggbomGroupEntity;
                serviceResponse.Message = "Returned all EnggBomGroup";
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

        // GET: api/<EnggBomGroupController>
        [HttpGet]
        public async Task<IActionResult> GetAllActiveEnggBomGroup()
        {
            ServiceResponse<IEnumerable<EnggBomGroupDto>> serviceResponse = new ServiceResponse<IEnumerable<EnggBomGroupDto>>();

            try
            {
                var enggBomGroupList = await _repository.EnggBomGroupRepository.GetAllActiveEnggBomGroup();
                _logger.LogInfo("Returned all ActiveEnggBomGroup");
                var enggbomGroupEntity = _mapper.Map<IEnumerable<EnggBomGroupDto>>(enggBomGroupList);
                serviceResponse.Data = enggbomGroupEntity;
                serviceResponse.Message = "Returned all  ActiveEnggBomGroup Successfully";
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

        // GET: api/<EnggBomGroupController>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnggBomGroupById(int id)
        {
            ServiceResponse<EnggBomGroupDto> serviceResponse = new ServiceResponse<EnggBomGroupDto>();

            try
            {
                var enggbomGroupList = await _repository.EnggBomGroupRepository.GetEnggBomGroupById(id);
                if (enggbomGroupList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"EnggBomGroup hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"EnggBomGroup with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var enggbomGroupEntity = _mapper.Map<EnggBomGroupDto>(enggbomGroupList);
                    serviceResponse.Data = enggbomGroupEntity;
                    serviceResponse.Message = "Returned EnggBomGroup Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEnggBomGroupById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST: api/<EnggBomGroupController>
        [HttpPost]
        public IActionResult CreateEnggBomGroup([FromBody] EnggBomGroupDtoPost enggbomGroupDtoPost)
        {
            ServiceResponse<EnggBomGroupDtoPost> serviceResponse = new ServiceResponse<EnggBomGroupDtoPost>();

            try
            {
                if (enggbomGroupDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "EnggBomGroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("EnggBomGroup object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid EnggBomGroup object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid EnggBomGroup object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var enggbomGroupEntity = _mapper.Map<EnggBomGroup>(enggbomGroupDtoPost);
                _repository.EnggBomGroupRepository.CreateEnggBomGroup(enggbomGroupEntity);
                _repository.SaveAsync();
                serviceResponse.Message = "EnggBomGroup Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside CreateEnggBomGroup action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT: api/<EnggBomGroupController>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnggBomGroup(int id, [FromBody] EnggBomGroupDtoUpdate enggbomGroupDtoUpdate)
        {
            ServiceResponse<EnggBomGroupDtoUpdate> serviceResponse = new ServiceResponse<EnggBomGroupDtoUpdate>();

            try
            {
                if (enggbomGroupDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update EnggBomGroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update EnggBomGroup object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update EnggBomGroup object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update EnggBomGroup object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var enggbomGroupEntity = await _repository.EnggBomGroupRepository.GetEnggBomGroupById(id);
                if (enggbomGroupEntity is null)
                {
                    _logger.LogError($"Update EnggBomGroup with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " UpdateEnggBomGroup hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(enggbomGroupDtoUpdate, enggbomGroupEntity);
                string result = await _repository.EnggBomGroupRepository.UpdateEnggBomGroup(enggbomGroupEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "EnggBomGroup Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateEnggBomGroup action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE: api/<EnggBomGroupController>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnggBomGroup(int id)
        {
            ServiceResponse<EnggBomGroupDto> serviceResponse = new ServiceResponse<EnggBomGroupDto>();

            try
            {
                var enggBomGroupList = await _repository.EnggBomGroupRepository.GetEnggBomGroupById(id);
                if (enggBomGroupList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete EnggBomGroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete EnggBomGroup with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.EnggBomGroupRepository.DeleteEnggBomGroup(enggBomGroupList);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "EnggBomGroup Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteEnggBomGroup action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // GET: api/<EnggCustomFieldController>
        [HttpGet]
        public async Task<IActionResult> GetAllEnggCustomField([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<EnggCustomFieldDto>> serviceResponse = new ServiceResponse<IEnumerable<EnggCustomFieldDto>>();
            try
            {
                var listOfEnggCustomField = await _repository.EnggCustomFieldRepository.GetAllEnggCustomFields(pagingParameter);
                var metadata = new
                {
                    listOfEnggCustomField.TotalCount,
                    listOfEnggCustomField.PageSize,
                    listOfEnggCustomField.CurrentPage,
                    listOfEnggCustomField.HasNext,
                    listOfEnggCustomField.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all EnggCustomField");
                var enggcustomFieldEntity = _mapper.Map<IEnumerable<EnggCustomFieldDto>>(listOfEnggCustomField);
                serviceResponse.Data = enggcustomFieldEntity;
                serviceResponse.Message = "Returned all EnggCustomField";
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

        // GET: api/<EnggCustomFieldController>
        [HttpGet]
        public async Task<IActionResult> GetAllActiveEnggCustomField()
        {
            ServiceResponse<IEnumerable<EnggCustomFieldDto>> serviceResponse = new ServiceResponse<IEnumerable<EnggCustomFieldDto>>();

            try
            {
                var enggCustomFieldEntityList = await _repository.EnggCustomFieldRepository.GetAllActiveEnggCustomFields();
                _logger.LogInfo("Returned all EnggActiveCustomField");
                var enggCustomFieldEntity = _mapper.Map<IEnumerable<EnggCustomFieldDto>>(enggCustomFieldEntityList);
                serviceResponse.Data = enggCustomFieldEntity;
                serviceResponse.Message = "Returned all  ActiveEnggCustomField Successfully";
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

        // GET: api/<EnggCustomFieldController>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnggCustomFieldById(int id)
        {
            ServiceResponse<EnggCustomFieldDto> serviceResponse = new ServiceResponse<EnggCustomFieldDto>();

            try
            {
                var enggcustomFieldList = await _repository.EnggCustomFieldRepository.GetEnggCustomFieldById(id);
                if (enggcustomFieldList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"EnggcustomField hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"EnggcustomField with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned EnggcustomField with id: {id}");
                    var enggcustomFieldEntity = _mapper.Map<EnggCustomFieldDto>(enggcustomFieldList);
                    serviceResponse.Data = enggcustomFieldEntity;
                    serviceResponse.Message = "Returned EnggcustomField Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEnggCustomFieldById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST: api/<EnggCustomFieldController>
        [HttpPost]
        public IActionResult CreateEnggCustomField([FromBody] EnggCustomFieldDtoPost enggcustomFieldDtoPost)
        {
            ServiceResponse<EnggCustomFieldDtoPost> serviceResponse = new ServiceResponse<EnggCustomFieldDtoPost>();

            try
            {
                if (enggcustomFieldDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "EnggCustomField object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("EnggCustomField object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid EnggCustomField object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid EnggcustomField object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var enggcustomFieldEntity = _mapper.Map<EnggCustomField>(enggcustomFieldDtoPost);
                _repository.EnggCustomFieldRepository.CreateEnggCustomField(enggcustomFieldEntity);
                _repository.SaveAsync();
                serviceResponse.Message = "EnggCustomField Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside CreateEnggCustomField action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT: api/<EnggCustomFieldController>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnggCustomField(int id, [FromBody] EnggCustomFieldDtoUpdate enggcustomFieldDtoUpdate)
        {
            ServiceResponse<EnggCustomFieldDtoUpdate> serviceResponse = new ServiceResponse<EnggCustomFieldDtoUpdate>();

            try
            {
                if (enggcustomFieldDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update EnggCustomField object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update EnggCustomField object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update EnggCustomField object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update EnggCustomField object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var enggcustomFieldEntity = await _repository.EnggCustomFieldRepository.GetEnggCustomFieldById(id);
                if (enggcustomFieldEntity is null)
                {
                    _logger.LogError($"Update EnggCustomField with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " UpdateEnggCustomField hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(enggcustomFieldDtoUpdate, enggcustomFieldEntity);
                string result = await _repository.EnggCustomFieldRepository.UpdateEnggCustomField(enggcustomFieldEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "EnggCustomField Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateEnggCustomField action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE: api/<EnggCustomFieldController>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnggCustomField(int id)
        {
            ServiceResponse<EnggCustomFieldDto> serviceResponse = new ServiceResponse<EnggCustomFieldDto>();

            try
            {
                var enggCustomFieldList = await _repository.EnggCustomFieldRepository.GetEnggCustomFieldById(id);
                if (enggCustomFieldList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete EnggCustomField object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete EnggCustomField with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.EnggCustomFieldRepository.DeleteEnggCustomField(enggCustomFieldList);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "EnggCustomField Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteEnggBomGroup action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
