using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public CategoryController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<CategoryController>
        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            ServiceResponse<IEnumerable<CategoryDto>> serviceResponse = new ServiceResponse<IEnumerable<CategoryDto>>();
            try
            {
                var categoryList = await _repository.CategoryRepository.GetAllCategory();
                _logger.LogInfo("Returned all Category");
                var result = _mapper.Map<IEnumerable<CategoryDto>>(categoryList);

                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all Category";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            ServiceResponse<CategoryDto> serviceResponse = new ServiceResponse<CategoryDto>();
            try
            {
                var category = await _repository.CategoryRepository.GetCategoryById(id);
                if (category == null)
                {
                    _logger.LogError($"Category with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Category hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<CategoryDto>(category);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Category Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCategoryById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<CategoryController>
        [HttpPost]
        public IActionResult CreateCategory([FromBody] CategoryDtoPost categoryDtoPost)
        {
            ServiceResponse<CategoryDtoPost> serviceResponse = new ServiceResponse<CategoryDtoPost>();
            try
            {
                if (categoryDtoPost is null)
                {
                    _logger.LogError("Category object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Category object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Category object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Category object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                var categoryEntity = _mapper.Map<Category>(categoryDtoPost);
                _repository.CategoryRepository.CreateCategory(categoryEntity);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = "Category Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);

            }
        }

        // PUT api/<CategoryController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDtoUpdate categoryDtoUpdate)
        {
            ServiceResponse<CategoryDtoUpdate> serviceResponse = new ServiceResponse<CategoryDtoUpdate>();
            try
            {
                if (categoryDtoUpdate is null)
                {
                    _logger.LogError("Category object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Category object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Category object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Category object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                var categoryEntity = await _repository.CategoryRepository.GetCategoryById(id);
                if (categoryEntity is null)
                {
                    _logger.LogError($"Category with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Category hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                _mapper.Map(categoryDtoUpdate, categoryEntity);
                string result = await _repository.CategoryRepository.UpdateCategory(categoryEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Category Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateCategory action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            ServiceResponse<CategoryDto> serviceResponse = new ServiceResponse<CategoryDto>();
            try
            {
                var category = await _repository.CategoryRepository.GetCategoryById(id);
                if (category == null)
                {
                    _logger.LogError($"Category with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Category hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                string result = await _repository.CategoryRepository.DeleteCategory(category);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Category Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateCategory(int id)
        {
            ServiceResponse<CategoryDto> serviceResponse = new ServiceResponse<CategoryDto>();
            try
            {
                var category = await _repository.CategoryRepository.GetCategoryById(id);
                if (category is null)
                {
                    _logger.LogError($"Category with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Category hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                category.ActiveStatus = true;
                string result = await _repository.CategoryRepository.UpdateCategory(category);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Returned ActivateCategory";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateCategory action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DectivateCategory(int id)
        {
            ServiceResponse<CategoryDto> serviceResponse = new ServiceResponse<CategoryDto>();
            try
            {
                var category = await _repository.CategoryRepository.GetCategoryById(id);
                if (category is null)
                {
                    _logger.LogError($"Category with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Category hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                category.ActiveStatus = false;
                string result = await _repository.CategoryRepository.UpdateCategory(category);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Returned DectivateCategory";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DectivateCategory action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
