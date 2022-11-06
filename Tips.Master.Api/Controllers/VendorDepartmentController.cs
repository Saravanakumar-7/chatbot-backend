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
    public class VendorDepartmentController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public VendorDepartmentController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }
        // GET: api/<VendorDepartmentController>
        [HttpGet]
        public async Task<IActionResult> GetAllVendorDepartment()
        {
            try
            {
                var vendorDepartments = await _repository.VendorDepartmentRepository.GetAllActiveVendorDepartment();
                _logger.LogInfo("Returned all Vendor Department");
                var result = _mapper.Map<IEnumerable<VendorDepartment>>(vendorDepartments);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }


        // GET api/<VendorDepartmentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorDepartmentById(int id)
        {
            try
            {
                var vendorDepartment = await _repository.VendorDepartmentRepository.GetVendorDepartmentById(id);
                if (vendorDepartment == null)
                {
                    _logger.LogError($"Vendor Department with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<VendorDepartmentDto>(vendorDepartment);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetVendorDepartmentById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<VendorDepartmentController>
        [HttpPost]
        public IActionResult CreateVendorDepartment([FromBody] VendorDepartmentPostDto vendorDepartmentPostDto)
        {
            try
            {
                if (vendorDepartmentPostDto is null)
                {
                    _logger.LogError("VendorDepartment object sent from client is null.");
                    return BadRequest("VendorDepartment object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid VendorDepartment object sent from client.");
                    return BadRequest("Invalid model object");
                }
                //var deliverTermEntity = _mapper.Map<DeliveryTerm>(deliveryTerm);
                //var id = _repository.DeliveryTermRepo.CreateDeliveryTerm(deliverTermEntity);
                //_repository.SaveAsync();

                var vendorDepartment = _mapper.Map<VendorDepartment>(vendorDepartmentPostDto);
                _repository.VendorDepartmentRepository.CreateVendorDepartment(vendorDepartment);
                _repository.SaveAsync();


                return Created("GetVendorDepartmentById", "Successfully Created");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<VendorDepartmentController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendorDepartment(int id, [FromBody] VendorDepartmentUpdateDto vendorDepartmentUpdateDto)
        {
            try
            {
                if (vendorDepartmentUpdateDto is null)
                {
                    _logger.LogError("VendorDepartment object sent from client is null.");
                    return BadRequest("VendorDepartment object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid VendorDepartment object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var vendorDepartment = await _repository.VendorDepartmentRepository.GetVendorDepartmentById(id);
                if (vendorDepartment is null)
                {
                    _logger.LogError($"VendorDepartment with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(vendorDepartmentUpdateDto, vendorDepartment);
                string result = await _repository.VendorDepartmentRepository.UpdateVendorDepartment(vendorDepartment);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateVendorDepartment action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<VendorDepartmentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendorDepartment(int id)
        {
            try
            {
                var vendorDepartment = await _repository.VendorDepartmentRepository.GetVendorDepartmentById(id);
                if (vendorDepartment == null)
                {
                    _logger.LogError($"VendorDepartment with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.VendorDepartmentRepository.DeleteVendorDepartment(vendorDepartment);
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
        public async Task<IActionResult> ActivateVendorDepartment(int id)
        {
            try
            {
                var vendorDepartment = await _repository.VendorDepartmentRepository.GetVendorDepartmentById(id);
                if (vendorDepartment is null)
                {
                    _logger.LogError($"VendorDepartment with id: {id}, hasn't been found in db.");
                    return BadRequest("VendorDepartment object is null");
                }
                vendorDepartment.IsActive = true;
                string result = await _repository.VendorDepartmentRepository.UpdateVendorDepartment(vendorDepartment);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateVendorDepartment action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateVendorDepartment(int id)
        {
            try
            {
                var vendorDepartment = await _repository.VendorDepartmentRepository.GetVendorDepartmentById(id);
                if (vendorDepartment is null)
                {
                    _logger.LogError($"VendorDepartment with id: {id}, hasn't been found in db.");
                    return BadRequest("VendorDepartment object is null");
                }
                vendorDepartment.IsActive = false;
                string result = await _repository.VendorDepartmentRepository.UpdateVendorDepartment(vendorDepartment);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateVendorDepartment action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
