using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public ProductTypeController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductTypes()
        {
            ServiceResponse<IEnumerable<ProductTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<ProductTypeDto>>();
            try
            {

                var getAllProductType = await _repository.ProductTypeRepository.GetAllProductType();
                _logger.LogInfo("Returned all productTypes");
                var result = _mapper.Map<IEnumerable<ProductTypeDto>>(getAllProductType);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all productType Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductTypeById(int id)
        {
            ServiceResponse<ProductTypeDto> serviceResponse = new ServiceResponse<ProductTypeDto>();

            try
            {
                var productTypebyId = await _repository.ProductTypeRepository.GetProductTypeById(id);
                if (productTypebyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"productType with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"productType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned productTypebyId with id: {id}");
                    var result = _mapper.Map<ProductTypeDto>(productTypebyId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned productType with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside productTypebyId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{typeSolution}")]
        public async Task<IActionResult> GetListofTypeSolutionByProductType(string typeSolution)
        {
            ServiceResponse<IEnumerable<GetListofTypeSolutionByProductTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<GetListofTypeSolutionByProductTypeDto>>();

            try
            {
                var typeSolutionDetails = await _repository.ProductTypeRepository.GetListOfTypeSolutionByProductType(typeSolution);
                if (typeSolutionDetails == null)
                {
                    _logger.LogError($"typeSolution with id: {typeSolution}, hasn't been found.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"typeSolution with id: {typeSolution}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned typeSolution with id: {typeSolution}");
                    var result = _mapper.Map<IEnumerable<GetListofTypeSolutionByProductTypeDto>>(typeSolutionDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetListofTypeSolutionByProductType action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public IActionResult CreateProductType([FromBody] ProductTypePostDto productTypePostDto)
        {
            ServiceResponse<ProductTypeDto> serviceResponse = new ServiceResponse<ProductTypeDto>();

            try
            {
                if (productTypePostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ProductType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Entered Invalid Value.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ProductType object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Entered Invalid Value.");

                    return BadRequest(serviceResponse);
                }
                var productTypeCreate = _mapper.Map<ProductType>(productTypePostDto);
                _repository.ProductTypeRepository.CreateProductType(productTypeCreate);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ProductType Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetProductTypeById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateProductType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductField(int id, [FromBody] ProductTypeUpdateDto productTypeUpdateDto)
        {
            ServiceResponse<ProductTypeDto> serviceResponse = new ServiceResponse<ProductTypeDto>();

            try
            {
                if (productTypeUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update ProductType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Value Cannot be Null , Pass Proper Value.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update ProductType object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update ProductType object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var productTypeDetail = await _repository.ProductTypeRepository.GetProductTypeById(id);
                if (productTypeDetail is null)
                {
                    _logger.LogError($"Value NotFound");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update ProductType with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(productTypeUpdateDto, productTypeDetail);
                string result = await _repository.ProductTypeRepository.UpdateProductType(productTypeDetail);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateProductType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductField(int id)
        {
            ServiceResponse<ProductTypeDto> serviceResponse = new ServiceResponse<ProductTypeDto>();

            try
            {
                var deleteProductType = await _repository.ProductTypeRepository.GetProductTypeById(id);
                if (deleteProductType == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete ProductType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Value NotFound , Please Enter Proper Value.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.ProductTypeRepository.DeleteProductType(deleteProductType);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteProductType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}