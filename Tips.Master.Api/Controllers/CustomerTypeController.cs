using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerTypeController : ControllerBase
    {
        private readonly IRepositoryWrapperForMaster _repository;
        private readonly ILogger _logger;

        public CustomerTypeController(IRepositoryWrapperForMaster repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: api/<CustomerTypeController>
        [HttpGet]
        public IActionResult GetAllCustomerTypes()
        {
            try
            {
                var customerTypeList = _repository.CustomerTypeRepository.FindAll();
                _logger.LogInformation("Returned all customerTypes");
                return Ok(customerTypeList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        // GET api/<CustomerTypeController>/5
        [HttpGet("{id}")]
        public string GetCustomerTypeById(int id)
        {
            return "value";
        }

        // POST api/<CustomerTypeController>
        [HttpPost]
        public void CreateCustomerType([FromBody] string value)
        {
        }

        // PUT api/<CustomerTypeController>/5
        [HttpPut("{id}")]
        public void UpdateCustomerType(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CustomerTypeController>/5
        [HttpDelete("{id}")]
        public void DeleteCustomerType(int id)
        {
        }
    }
}
