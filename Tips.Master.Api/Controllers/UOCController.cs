using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]")]
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
                var UOCList = await _repository.UOCRepository.GetAllUOC();
                _logger.LogInfo("Returned all UOC");
                var result = _mapper.Map<IEnumerable<UOCDto>>(UOCList);
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
                var UOC = await _repository.UOCRepository.GetUOCById(id);
                if (UOC == null)
                {
                    _logger.LogError($"UOC with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<UOCDto>(UOC);
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
        public IActionResult CreateUOC([FromBody] UOCDtoPost UocDtoPost)
        {
            try
            {
                if (UocDtoPost is null)
                {
                    _logger.LogError("UOC object sent from client is null.");
                    return BadRequest("UOC object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid UOC object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var UOCEntity = _mapper.Map<UOC>(UocDtoPost);
                _repository.UOCRepository.CreateUOC(UOCEntity);
                _repository.SaveAsync();

                return Created("GetUOCById", "Successfully Created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<UOMController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUOC(int id, [FromBody] UOCDtoUpdate UocDtoUpdate)
        {
            try
            {
                if (UocDtoUpdate is null)
                {
                    _logger.LogError("UOC object sent from client is null.");
                    return BadRequest("UOC object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid UOC object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var UOCEntity = await _repository.UOCRepository.GetUOCById(id);
                if (UOCEntity is null)
                {
                    _logger.LogError($"UOC with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(UocDtoUpdate, UOCEntity);
                string result = await _repository.UOCRepository.UpdateUOC(UOCEntity);
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
                var UOC = await _repository.UOCRepository.GetUOCById(id);
                if (UOC == null)
                {
                    _logger.LogError($"UOC with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.UOCRepository.DeleteUOC(UOC);
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
                var UOC = await _repository.UOCRepository.GetUOCById(id);
                if (UOC is null)
                {
                    _logger.LogError($"UOC with id: {id}, hasn't been found in db.");
                    return BadRequest("UOC object is null");
                }
                UOC.ActiveStatus = true;
                string result = await _repository.UOCRepository.UpdateUOC(UOC);
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
                var UOC = await _repository.UOCRepository.GetUOCById(id);
                if (UOC is null)
                {
                    _logger.LogError($"UOC with id: {id}, hasn't been found in db.");
                    return BadRequest("UOC object is null");
                }
                UOC.ActiveStatus = false;
                string result = await _repository.UOCRepository.UpdateUOC(UOC);
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
