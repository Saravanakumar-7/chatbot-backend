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
    public class DeliveryTermController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

 
        public DeliveryTermController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }

        // GET: api/<DeliveryTermController>
        [HttpGet]
        public async Task<IActionResult> GetAllDeliveryTerms()
        {
            try
            {
                var deliveryTermsList = await _repository.DeliveryTermRepo.GetAllActiveDeliveryTerms();
                _logger.LogInfo("Returned all DeliveryTerms");
                var result = _mapper.Map<IEnumerable<DeliveryTermGetDto>>(deliveryTermsList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<CustomerTypeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeliveryTermsById(int id)
        {
            try
            {
                var deliveryTerm = await _repository.DeliveryTermRepo.GetDeliveryTermById(id);
                if (deliveryTerm == null)
                {
                    _logger.LogError($"deliveryTerm with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<DeliveryTermGetDto>(deliveryTerm);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetDeliveryTermsById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<DeliveryTermController> 
        [HttpPost]
        public IActionResult CreateDeliveryTerm([FromBody] DeliveryTermPostDto deliveryTerm)
        {
            try
            {
                if (deliveryTerm is null)
                {
                    _logger.LogError("DeliverTerm object sent from client is null.");
                    return BadRequest("DeliverTerm object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid DeliverTerm object sent from client.");
                    return BadRequest("Invalid model object");
                }
                //var deliverTermEntity = _mapper.Map<DeliveryTerm>(deliveryTerm);
                //var id = _repository.DeliveryTermRepo.CreateDeliveryTerm(deliverTermEntity);
                //_repository.SaveAsync();

                var deliverTermEntity = _mapper.Map<DeliveryTerm>(deliveryTerm);
                _repository.DeliveryTermRepo.CreateDeliveryTerm(deliverTermEntity);
                _repository.SaveAsync();


                return Created("GetDeliveryTermById", "Successfully Created");
                 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<DeliveryTermController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeliveryTerm(int id, [FromBody] DeliveryTermUpdateDto deliveryTerm)
        {
            try
            {
                if (deliveryTerm is null)
                {
                    _logger.LogError("DeliveryTerm object sent from client is null.");
                    return BadRequest("DeliveryTerm object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid DeliveryTerm object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var deliveryTermEntity = await _repository.DeliveryTermRepo.GetDeliveryTermById(id);
                if (deliveryTermEntity is null)
                {
                    _logger.LogError($"DeliveryTerm with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(deliveryTerm, deliveryTermEntity);
                string result = await _repository.DeliveryTermRepo.UpdateDeliveryTerm(deliveryTermEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateDeliveryTerm action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<DeliveryTermController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeliveryTerm(int id)
        {
            try
            {
                var deliveryTerm = await _repository.DeliveryTermRepo.GetDeliveryTermById(id);
                if (deliveryTerm == null)
                {
                    _logger.LogError($"DeliveryTerm with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.DeliveryTermRepo.DeleteDeliveryTerm(deliveryTerm);
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
        public async Task<IActionResult> ActivateDeliveryTerm(int id)
        {
            try
            {
                var deliveryTerm = await _repository.DeliveryTermRepo.GetDeliveryTermById(id);
                if (deliveryTerm is null)
                {
                    _logger.LogError($"DeliveryTerm with id: {id}, hasn't been found in db.");
                    return BadRequest("DeliveryTerm object is null");
                }
                deliveryTerm.IsActive = true;
                string result = await _repository.DeliveryTermRepo.UpdateDeliveryTerm(deliveryTerm);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateDeliveryTerm action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateDeliveryTerm(int id)
        {
            try
            {
                var deliveryTerm = await _repository.DeliveryTermRepo.GetDeliveryTermById(id);
                if (deliveryTerm is null)
                {
                    _logger.LogError($"DeliveryTerm with id: {id}, hasn't been found in db.");
                    return BadRequest("DeliveryTerm object is null");
                }
                deliveryTerm.IsActive = false;
                string result = await _repository.DeliveryTermRepo.UpdateDeliveryTerm(deliveryTerm);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateDeliveryTerm action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
