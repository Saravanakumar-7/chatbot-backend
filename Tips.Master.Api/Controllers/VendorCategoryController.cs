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
    public class VendorCategoryController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public VendorCategoryController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }
        // GET: api/<VendorCategoryController>
        [HttpGet]
        public async Task<IActionResult> GetAllVendorCategory()
        {
            try
            {
                var vendorCategories = await _repository.VendorCategoryRepository.GetAllActiveVendorCategory();
                _logger.LogInfo("Returned all VendorCategory");
                var result = _mapper.Map<IEnumerable<VendorCategory>>(vendorCategories);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }


        // GET api/<VendorCategoryController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorCategoryById(int id)
        {
            try
            {
                var vendorCategory = await _repository.VendorCategoryRepository.GetVendorCategoryById(id);
                if (vendorCategory == null)
                {
                    _logger.LogError($"VendorCategory with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<VendorCategoryDto>(vendorCategory);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetVendorCategoryById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<VendorCategoryController>
        [HttpPost]
        public IActionResult CreateVendorCategory([FromBody] VendorCategoryPostDto vendorCategoryPostDto)
        {
            try
            {
                if (vendorCategoryPostDto is null)
                {
                    _logger.LogError("VendorCategory object sent from client is null.");
                    return BadRequest("VendorCategory object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid VendorCategory object sent from client.");
                    return BadRequest("Invalid model object");
                }
                //var deliverTermEntity = _mapper.Map<DeliveryTerm>(deliveryTerm);
                //var id = _repository.DeliveryTermRepo.CreateDeliveryTerm(deliverTermEntity);
                //_repository.SaveAsync();

                var vendorCategory = _mapper.Map<VendorCategory>(vendorCategoryPostDto);
                _repository.VendorCategoryRepository.CreateVendorCategory(vendorCategory);
                _repository.SaveAsync();


                return Created("GetVendorCategoryById", "Successfully Created");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<VendorCategoryController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendorCategory(int id, [FromBody] VendorCategoryUpdateDto vendorCategoryUpdateDto)
        {
            try
            {
                if (vendorCategoryUpdateDto is null)
                {
                    _logger.LogError("VendorCategory object sent from client is null.");
                    return BadRequest("VendorCategory object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid VendorCategory object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var vendorCategory = await _repository.VendorCategoryRepository.GetVendorCategoryById(id);
                if (vendorCategory is null)
                {
                    _logger.LogError($"VendorCategory with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(vendorCategoryUpdateDto, vendorCategory);
                string result = await _repository.VendorCategoryRepository.UpdateVendorCategory(vendorCategory);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateVendorCategory action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<VendorCategoryController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendorCategory(int id)
        {
            try
            {
                var vendorCategory = await _repository.VendorCategoryRepository.GetVendorCategoryById(id);
                if (vendorCategory == null)
                {
                    _logger.LogError($"VendorCategory with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.VendorCategoryRepository.DeleteVendorCategory(vendorCategory);
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
        public async Task<IActionResult> ActivateVendorCategory(int id)
        {
            try
            {
                var vendorCategory = await _repository.VendorCategoryRepository.GetVendorCategoryById(id);
                if (vendorCategory is null)
                {
                    _logger.LogError($"VendorCategory with id: {id}, hasn't been found in db.");
                    return BadRequest("VendorCategory object is null");
                }
                vendorCategory.IsActive = true;
                string result = await _repository.VendorCategoryRepository.UpdateVendorCategory(vendorCategory);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateVendorCategory action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateVendorCategory(int id)
        {
            try
            {
                var vendorCategory = await _repository.VendorCategoryRepository.GetVendorCategoryById(id);
                if (vendorCategory is null)
                {
                    _logger.LogError($"VendorCategory with id: {id}, hasn't been found in db.");
                    return BadRequest("VendorCategory object is null");
                }
                vendorCategory.IsActive = false;
                string result = await _repository.VendorCategoryRepository.UpdateVendorCategory(vendorCategory);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateVendorCategory action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
