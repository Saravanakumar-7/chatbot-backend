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

                //var enggChildItemDto = EnggBomDto.EnggChildItemPosts;

                //var enggChildItemList = new List<EnggChildItem>();
                //for (int i = 0; i < enggChildItemDto.Count; i++)
                //{
                //    EnggChildItem enggChildItemDetail = _mapper.Map<EnggChildItem>(enggChildItemDto[i]);
                //    enggChildItemDetail.EnggAlternates = _mapper.Map<List<EnggAlternates>>(enggChildItemDto[i].EnggAlternatesPostDtos);
                //    enggChildItemDetail.NREConsumable = _mapper.Map<NREConsumable>(enggChildItemDto[i].BomNREConsumablePostDto);
                //    enggChildItemList.Add(enggChildItemDetail);

                //}
                //enggBomList.EnggChildItems = enggChildItemList;

                _logger.LogInfo("Returned all Vendors");
                var result = _mapper.Map<IEnumerable<EnggBomDto>>(listOfBoms);
                serviceResponse.Data = result;
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
                var vendorBoms = await _repository.EnggBomRepository.GetEnggBomById(id);

                if (vendorBoms == null)
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
                    var result = _mapper.Map<EnggBomDto>(vendorBoms);
                    serviceResponse.Data = result;
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
        public async Task<IActionResult> UpdateEnggBom(int id, [FromBody] EnggBomPostDto enggBomDto)
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

                var enggChildItemDto = enggBomDto.EnggChildItemPosts;

                var enggChildItemList = new List<EnggChildItem>();
                for (int i = 0; i < enggChildItemDto.Count; i++)
                {
                    EnggChildItem enggChildItemDetail = _mapper.Map<EnggChildItem>(enggChildItemDto[i]);
                    enggChildItemDetail.EnggAlternates = _mapper.Map<List<EnggAlternates>>(enggChildItemDto[i].EnggAlternatesPostDtos);
                    enggChildItemDetail.NREConsumable = _mapper.Map<NREConsumable>(enggChildItemDto[i].BomNREConsumablePostDto);
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
    }
}
