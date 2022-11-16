using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UOCController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public UOCController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<UOMController>
        [HttpGet]
        public async Task<IActionResult> GetAllUOC()
        {
            try
            {
                var uocList = await _repository.UOCRepository.GetAllUOC();
                _logger.LogInfo("Returned all UOC");
                var result = _mapper.Map<IEnumerable<UOCDto>>(uocList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }


        // GET api/<UOCController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUOCById(int id)
        {
            try
            {
                var uoc = await _repository.UOCRepository.GetUOCById(id);
                if (uoc == null)
                {
                    _logger.LogError($"UOC with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<UOCDto>(uoc);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetUOCById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<UOMController>
        [HttpPost]
        public IActionResult CreateUOC([FromBody] UOCDtoPost uocDtoPost)
        {
            try
            {
                if (uocDtoPost is null)
                {
                    _logger.LogError("UOC object sent from client is null.");
                    return BadRequest("UOC object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid UOC object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var uocEntity = _mapper.Map<UOC>(uocDtoPost);
                _repository.UOCRepository.CreateUOC(uocEntity);
                _repository.SaveAsync();

                return Created("GetUOCById", "Successfully Created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<UOCController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUOC(int id, [FromBody] UOCDtoUpdate uocDtoUpdate)
        {
            try
            {
                if (uocDtoUpdate is null)
                {
                    _logger.LogError("UOC object sent from client is null.");
                    return BadRequest("UOC object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid UOC object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var uocEntity = await _repository.UOCRepository.GetUOCById(id);
                if (uocEntity is null)
                {
                    _logger.LogError($"UOC with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(uocDtoUpdate, uocEntity);
                string result = await _repository.UOCRepository.UpdateUOC(uocEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateUOC action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<UOCController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUOC(int id)
        {
            try
            {
                var uoc = await _repository.UOCRepository.GetUOCById(id);
                if (uoc == null)
                {
                    _logger.LogError($"UOC with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.UOCRepository.DeleteUOC(uoc);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateUOC(int id)
        {
            try
            {
                var uoc = await _repository.UOCRepository.GetUOCById(id);
                if (uoc is null)
                {
                    _logger.LogError($"UOC with id: {id}, hasn't been found in db.");
                    return BadRequest("UOC object is null");
                }
                uoc.ActiveStatus = true;
                string result = await _repository.UOCRepository.UpdateUOC(uoc);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateUOC action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateUOC(int id)
        {
            try
            {
                var uoc = await _repository.UOCRepository.GetUOCById(id);
                if (uoc is null)
                {
                    _logger.LogError($"UOC with id: {id}, hasn't been found in db.");
                    return BadRequest("UOC object is null");
                }
                uoc.ActiveStatus = false;
                string result = await _repository.UOCRepository.UpdateUOC(uoc);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateUOC action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
