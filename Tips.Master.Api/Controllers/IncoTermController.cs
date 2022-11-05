using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IncoTermController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public IncoTermController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }
        // GET: api/<IncoTermController>
        [HttpGet]
        public async Task<IActionResult> GetAllIncoTerms()
        {
            try
            {
                var incoTerms = await _repository.IncoTermRepository.GetAllActiveIncoTerm();
                _logger.LogInfo("Returned all Inco Term");
                var result = _mapper.Map<IEnumerable<IncoTermDto>>(incoTerms);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<IncoTermController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncoTermById(int id)
        {
            try
            {
                var incoTerm = await _repository.IncoTermRepository.GetIncoTermById(id);
                if (incoTerm == null)
                {
                    _logger.LogError($"IncoTerm with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<IncoTermDto>(incoTerm);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetIncoTermsById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<IncoTermController>
        [HttpPost]
        public IActionResult CreateIncoTerm([FromBody] IncoTermPostDto incoTerm)
        {
            try
            {
                if (incoTerm is null)
                {
                    _logger.LogError("IncoTerm object sent from client is null.");
                    return BadRequest("IncoTerm object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid IncoTerm object sent from client.");
                    return BadRequest("Invalid model object");
                } 

                var incoTerms = _mapper.Map<IncoTerm>(incoTerm);
                _repository.IncoTermRepository.CreateIncoTerm(incoTerms);
                _repository.SaveAsync();


                return Created("GetIncoTermById", "Successfully Created");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<IncoTermController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIncoTerm(int id, [FromBody] IncoTermUpdateDto incoTerm)
        {
            try
            {
                if (incoTerm is null)
                {
                    _logger.LogError("IncoTerm object sent from client is null.");
                    return BadRequest("IncoTerm object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid IncoTerm object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var incoterms = await _repository.IncoTermRepository.GetIncoTermById(id);
                if (incoterms is null)
                {
                    _logger.LogError($"IncoTerm with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(incoTerm, incoterms);
                string result = await _repository.IncoTermRepository.UpdateIncoTerm(incoterms);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateIncoTerm action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<IncoTermController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncoTerm(int id)
        {
            try
            {
                var incoTerm = await _repository.IncoTermRepository.GetIncoTermById(id);
                if (incoTerm == null)
                {
                    _logger.LogError($"IncoTerm with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.IncoTermRepository.DeleteIncoTerm(incoTerm);
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
        //start

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateIncoTerm(int id)
        {
            try
            {
                var incoTerm = await _repository.IncoTermRepository.GetIncoTermById(id);
                if (incoTerm is null)
                {
                    _logger.LogError($"IncoTerm with id: {id}, hasn't been found in db.");
                    return BadRequest("IncoTerm object is null");
                }
                incoTerm.IsActive = true;
                string result = await _repository.IncoTermRepository.UpdateIncoTerm(incoTerm);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateIncoTerm action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateIncoTerm(int id)
        {
            try
            {
                var incoTerm = await _repository.IncoTermRepository.GetIncoTermById(id);
                if (incoTerm is null)
                {
                    _logger.LogError($"IncoTerm with id: {id}, hasn't been found in db.");
                    return BadRequest("IncoTerm object is null");
                }
                incoTerm.IsActive = false;
                string result = await _repository.IncoTermRepository.UpdateIncoTerm(incoTerm);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateIncoTerm action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }



    }
}
