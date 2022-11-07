using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using AutoMapper;
using Contracts;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public VendorController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<VendorController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<VendorController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<VendorController>
        [HttpPost]
        public async Task<IActionResult> CreateVendor([FromBody] VendorMasterPostDto vendorMasterPost)
        {
            try
            {
                if (vendorMasterPost is null)
                {
                    _logger.LogError("VendorDetails object sent from client is null.");
                    return BadRequest("VendorDetails object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid VendorDetails object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var vendorMaster = _mapper.Map<VendorMaster>(vendorMasterPost);
                await _repository.VendorRepository.CreateVendor(vendorMaster);
                _repository.SaveAsync();

                return Created("GetVendorCategoryById", "Successfully Created");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<VendorController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<VendorController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
