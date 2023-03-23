using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Contracts;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OpenDeliveryOrderController : ControllerBase
    {
        private IOpenDeliveryOrderRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public OpenDeliveryOrderController(IOpenDeliveryOrderRepository repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<OpenDeliveryOrderController>
        [HttpGet]
        public async Task<IActionResult> GetAllOpenDeliveryOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<OpenDeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenDeliveryOrderDto>>();

            try
            {
                var getAllOpenDeliveryOrderDetails = await _repository.GetAllOpenDeliveryOrders(pagingParameter, searchParams);

                var metadata = new
                {
                    getAllOpenDeliveryOrderDetails.TotalCount,
                    getAllOpenDeliveryOrderDetails.PageSize,
                    getAllOpenDeliveryOrderDetails.CurrentPage,
                    getAllOpenDeliveryOrderDetails.HasNext,
                    getAllOpenDeliveryOrderDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all OpenDeliveryOrders");
                var result = _mapper.Map<IEnumerable<OpenDeliveryOrderDto>>(getAllOpenDeliveryOrderDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenDeliveryOrders Successfully";
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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetOpenDeliveryOrderById(int id)
        {
            ServiceResponse<OpenDeliveryOrderDto> serviceResponse = new ServiceResponse<OpenDeliveryOrderDto>();

            try
            {
                var getOpenDeliveryOrderById = await _repository.GetOpenDeliveryOrderById(id);

                if (getOpenDeliveryOrderById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenDeliveryOrdersDetails with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenDeliveryOrdersDetails with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned OpenDeliveryOrdersDetails with id: {id}");
                    var result = _mapper.Map<OpenDeliveryOrderDto>(getOpenDeliveryOrderById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned OpenDeliveryOrderById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetOpenDeliveryOrdersById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOpenDeliveryOrder([FromBody] OpenDeliveryOrderDtoPost openDeliveryOrderDtoPost)
        {
            ServiceResponse<OpenDeliveryOrderDto> serviceResponse = new ServiceResponse<OpenDeliveryOrderDto>();

            try
            {
                if (openDeliveryOrderDtoPost is null)
                {
                    _logger.LogError("OpenDeliveryOrderDetails object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OpenDeliveryOrderDetails object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid OpenDeliveryOrderDetails object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid OpenDeliveryOrderDetails object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var openDeliveryOrderparts = _mapper.Map<IEnumerable<OpenDeliveryOrderParts>>(openDeliveryOrderDtoPost.OpenDeliveryOrderParts);

                var openDeliveryorder = _mapper.Map<OpenDeliveryOrder>(openDeliveryOrderDtoPost);

                openDeliveryorder.OpenDeliveryOrderParts = openDeliveryOrderparts.ToList();
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));

                var newcount = await _repository.GetODONumberAutoIncrementCount(date);

                if (newcount > 0)
                {
                    var number = newcount + 1;
                    string e = String.Format("{0:D4}", number);
                    openDeliveryorder.OpenDONumber = days + months + years + "ODO" + (e);
                }
                else
                {
                    var count = 1;
                    var e = count.ToString("D4");
                    openDeliveryorder.OpenDONumber = days + months + years + "ODO" + (e);
                }

                await _repository.CreateOpenDeliveryOrder(openDeliveryorder);

                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "OpenDeliveryorder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetOpenDeliveryOrderById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOpenDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOpenDeliveryOrder(int id, [FromBody] OpenDeliveryOrderDtoUpdate openDeliveryOrderDtoUpdate)
        {
            ServiceResponse<OpenDeliveryOrderDtoUpdate> serviceResponse = new ServiceResponse<OpenDeliveryOrderDtoUpdate>();

            try
            {
                if (openDeliveryOrderDtoUpdate is null)
                {
                    _logger.LogError("Update OpenDeliveryOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update OpenDeliveryOrder object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update OpenDeliveryOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update OpenDeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var getOpenDeliveryOrderbyId = await _repository.GetOpenDeliveryOrderById(id);
                if (getOpenDeliveryOrderbyId is null)
                {
                    _logger.LogError($"Update OpenDeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update OpenDeliveryOrder with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }



                var openDelivaryOrderparts = _mapper.Map<IEnumerable<OpenDeliveryOrderParts>>(openDeliveryOrderDtoUpdate.OpenDeliveryOrderParts);


                var updateOpenDelivaryOrder = _mapper.Map(openDeliveryOrderDtoUpdate, getOpenDeliveryOrderbyId);


                updateOpenDelivaryOrder.OpenDeliveryOrderParts = openDelivaryOrderparts.ToList();

                string result = await _repository.UpdateOpenDeliveryOrder(updateOpenDelivaryOrder);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "OpenDelivaryOrder Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateOpenDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOpenDeliveryOrder(int id)
        {
            ServiceResponse<OpenDeliveryOrderDto> serviceResponse = new ServiceResponse<OpenDeliveryOrderDto>();

            try
            {
                var deleteOpenDeliveryOrderbyId = await _repository.GetOpenDeliveryOrderById(id);
                if (deleteOpenDeliveryOrderbyId == null)
                {
                    _logger.LogError($"Delete OpenDeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete OpenDeliveryOrder with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteOpenDeliveryOrder(deleteOpenDeliveryOrderbyId);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "OpenDeliveryOrder Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOpenDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOpenDeliveryOrderIdNameList()
        {
            ServiceResponse<IEnumerable<BtoIDNameList>> serviceResponse = new ServiceResponse<IEnumerable<BtoIDNameList>>();
            try
            {
                var listOfAllOpenDeliveryOrderIdNames = await _repository.GetAllOpenDeliveryOrderIdNameList();
                var result = _mapper.Map<IEnumerable<BtoIDNameList>>(listOfAllOpenDeliveryOrderIdNames);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All listOfAllOpenDeliveryOrderIdNames";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllOpenDeliveryOrderIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
    }
}
