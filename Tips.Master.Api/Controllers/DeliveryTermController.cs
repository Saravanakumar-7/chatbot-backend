using Contracts;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryTermController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        

        public DeliveryTermController(IRepositoryWrapperForMaster repository)
        {
            _repository = repository;
            
        }

        // GET: api/<DeliveryTermController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<DeliveryTermController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DeliveryTermController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<DeliveryTermController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DeliveryTermController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
