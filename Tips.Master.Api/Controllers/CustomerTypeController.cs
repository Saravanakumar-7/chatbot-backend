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

        public CustomerTypeController(IRepositoryWrapperForMaster repository)
        {
            _repository = repository;
        }

        // GET: api/<CustomerTypeController>
        [HttpGet]
        public IActionResult GetAllCustomerTypes()
        {
            var customerTypeList = _repository.CustomerTypeRepository.FindAll();
            return Ok(customerTypeList);
        }

        // GET api/<CustomerTypeController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CustomerTypeController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CustomerTypeController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CustomerTypeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
