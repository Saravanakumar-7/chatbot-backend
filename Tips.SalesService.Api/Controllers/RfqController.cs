using AutoMapper;
using Tips.SalesService.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Repository;
using Contracts;
using Entities.DTOs;
using Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RfqController : ControllerBase
    {
        private IRfqRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private TipsSalesServiceDbContext _repositoryContext;

        public RfqController(IRfqRepository repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }

         [HttpGet]
        public async Task<IActionResult> GetAllRfqs()
        {
            try
            {
                var listOfRfq = await _repository.GetAllRfq();
                _logger.LogInfo("Returned all rfq");
                var result = _mapper.Map<IEnumerable<Rfq>>(listOfRfq);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
             
        }

         [HttpGet("{id}")]
        public async Task<IActionResult> GetRfqById(int id)
        {
            try
            {
                var rfq = await _repository.GetRfqById(id);

                if (rfq == null)
                {
                    _logger.LogError($"Rfq with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<RfqDto>(rfq);

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetrfqById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
             
        }

         [HttpPost]
        public IActionResult CreateRfq([FromBody] RfqPostDto rfq)
        {
            try
            {
                if (rfq is null)
                {
                    _logger.LogError("Rfq object sent from client is null.");
                    return BadRequest("Rfq object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Rfq object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var rfqs = _mapper.Map<Rfq>(rfq);
                var customfield = _mapper.Map<IEnumerable<RfqCustomerSupport>>(rfq.rfqCustomerSupports);
                var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                _repository.CreateRfq(rfqs);
                _repositoryContext.SaveChanges();
                //_repository.SaveAsync();

                return Created("GetRfqById", "Successfully Created");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

         [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRfq(int id, [FromBody] RfqUpdateDto rfqUpdateDto)
        {
            try
            {
                if (rfqUpdateDto is null)
                {
                    _logger.LogError("Rfq object sent from client is null.");
                    return BadRequest("Rfq object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Rfq object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var rfq = await _repository.GetRfqById(id);
                if (rfq is null)
                {
                    _logger.LogError($"Rfq with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                //_mapper.Map(rfqUpdateDto, rfq);
                var data = _mapper.Map(rfqUpdateDto, rfq);
                var customfield = _mapper.Map<IEnumerable<RfqCustomerSupport>>(rfq.rfqCustomerSupports);
                var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                string result = await _repository.UpdateRfq(rfq);
                _logger.LogInfo(result);
                _repositoryContext.SaveChanges();

               // _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateRfq action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

         [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRfq(int id)
        {
            try
            {
                var rfq = await _repository.GetRfqById(id);
                if (rfq == null)
                {
                    _logger.LogError($"Rfq with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.DeleteRfq(rfq);
                _logger.LogInfo(result);
                _repositoryContext.SaveChanges();

                //_repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
