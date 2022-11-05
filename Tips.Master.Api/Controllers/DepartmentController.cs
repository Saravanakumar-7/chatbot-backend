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
    public class DepartmentController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public DepartmentController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }

        // GET: api/<DepartmentController>
        [HttpGet]
        public async Task<IActionResult> GetAllDepartment()
        {
            try
            {
                var departments = await _repository.DepartmentRepository.GetAllActiveDepartment();
                _logger.LogInfo("Returned all Department");
                var result = _mapper.Map<IEnumerable<DepartmentDto>>(departments);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<DepartmentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            try
            {
                var department = await _repository.DepartmentRepository.GetDepartmentById(id);
                if (department == null)
                {
                    _logger.LogError($"Department with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<DepartmentDto>(department);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetDepartmentById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<DepartmentController>
        [HttpPost]
        public IActionResult CreateDepartment([FromBody] DepartmentPostDto department)
        {
            try
            {
                if (department is null)
                {
                    _logger.LogError("Department object sent from client is null.");
                    return BadRequest("Department object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Department object sent from client.");
                    return BadRequest("Invalid model object");
                } 
                var departments = _mapper.Map<Department>(department);
                _repository.DepartmentRepository.CreateDepartment(departments);
                _repository.SaveAsync();


                return Created("GetDepartmentById", "Successfully Created");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<DepartmentController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentUpdateDto department)
        {
            try
            {
                if (department is null)
                {
                    _logger.LogError("Department object sent from client is null.");
                    return BadRequest("Department object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Department object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var departments = await _repository.DepartmentRepository.GetDepartmentById(id);
                if (departments is null)
                {
                    _logger.LogError($"DeliveryTerm with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(department, departments);
                string result = await _repository.DepartmentRepository.UpdateDepartment(departments);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateDepartment action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<DepartmentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                var department = await _repository.DepartmentRepository.GetDepartmentById(id);
                if (department == null)
                {
                    _logger.LogError($"Department with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.DepartmentRepository.DeleteDepartment(department);
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
        public async Task<IActionResult> ActivateDepartment(int id)
        {
            try
            {
                var department = await _repository.DepartmentRepository.GetDepartmentById(id);
                if (department is null)
                {
                    _logger.LogError($"Department with id: {id}, hasn't been found in db.");
                    return BadRequest("Department object is null");
                }
                department.IsActive = true;
                string result = await _repository.DepartmentRepository.UpdateDepartment(department);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateDepartment action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateDepartment(int id)
        {
            try
            {
                var department = await _repository.DepartmentRepository.GetDepartmentById(id);
                if (department is null)
                {
                    _logger.LogError($"Department  with id: {id}, hasn't been found in db.");
                    return BadRequest("Department object is null");
                }
                department.IsActive = false;
                string result = await _repository.DepartmentRepository.UpdateDepartment(department);
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
