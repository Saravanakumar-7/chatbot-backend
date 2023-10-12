using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public async Task<IActionResult> GetAllCategory([FromQuery]SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CategoryDto>> serviceResponse = new ServiceResponse<IEnumerable<CategoryDto>>();
            try
            {
                var GetallCategoryList = await _repository.CategoryRepository.GetAllCategory(searchParams);

                _logger.LogInfo("Returned all Category");
                var result = _mapper.Map<IEnumerable<CategoryDto>>(GetallCategoryList);

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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveCategory([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CategoryDto>> serviceResponse = new ServiceResponse<IEnumerable<CategoryDto>>();
            try
            {
                var GetallCategoryList = await _repository.CategoryRepository.GetAllActiveCategory(searchParams);

                _logger.LogInfo("Returned all Active Category");
                var result = _mapper.Map<IEnumerable<CategoryDto>>(GetallCategoryList);

                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all Active Category";
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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
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
                var CategorybyId = await _repository.CategoryRepository.GetCategoryById(id);
                if (CategorybyId == null)
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
                    var result = _mapper.Map<CategoryDto>(CategorybyId);
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
                var categoryCreate = _mapper.Map<Category>(categoryDtoPost);
                _repository.CategoryRepository.CreateCategory(categoryCreate);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = "Category Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside categoryCreate action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
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
                var categoryUpdate = await _repository.CategoryRepository.GetCategoryById(id);
                if (categoryUpdate is null)
                {
                    _logger.LogError($"Category with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Category hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                _mapper.Map(categoryDtoUpdate, categoryUpdate);
                string result = await _repository.CategoryRepository.UpdateCategory(categoryUpdate);
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
                serviceResponse.Message = "Internal Server Error";
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
                var categoryDelete = await _repository.CategoryRepository.GetCategoryById(id);
                if (categoryDelete == null)
                {
                    _logger.LogError($"Category with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Category hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                string result = await _repository.CategoryRepository.DeleteCategory(categoryDelete);
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
                _logger.LogError($"Something went wrong inside categoryDelete action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
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
                var CategoryActivate = await _repository.CategoryRepository.GetCategoryById(id);
                if (CategoryActivate is null)
                {
                    _logger.LogError($"Category with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Category hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                CategoryActivate.ActiveStatus = true;
                string result = await _repository.CategoryRepository.UpdateCategory(CategoryActivate);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCategory(int id)
        {
            ServiceResponse<CategoryDto> serviceResponse = new ServiceResponse<CategoryDto>();
            try
            {
                var CategoryDeactivate = await _repository.CategoryRepository.GetCategoryById(id);
                if (CategoryDeactivate is null)
                {
                    _logger.LogError($"Category with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Category hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                CategoryDeactivate.ActiveStatus = false;
                string result = await _repository.CategoryRepository.UpdateCategory(CategoryDeactivate);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Returned DeactivateCategory";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateCategory action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
