using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;


namespace Tips.Purchase.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PurchaseRequisitionController : ControllerBase
    {
        private IPurchaseRequisitionRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        
        public PurchaseRequisitionController(IPurchaseRequisitionRepository repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<PurchaseRequisitionController>
        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseRequistion([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionDto>>();
            try
            {
                var listOfPurchaseRequisition = await _repository.GetAllPurchaseRequisition(pagingParameter);
                var metadata = new
                {
                    listOfPurchaseRequisition.TotalCount,
                    listOfPurchaseRequisition.PageSize,
                    listOfPurchaseRequisition.CurrentPage,
                    listOfPurchaseRequisition.HasNext,
                    listOfPurchaseRequisition.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all PurchaseRequisitions");
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionDto>>(listOfPurchaseRequisition);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseRequisitions";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET api/<PurchaseRequisitionController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseRequisitionById(int id)
        {
            ServiceResponse<PurchaseRequisitionDto> serviceResponse = new ServiceResponse<PurchaseRequisitionDto>();
            try
            {
                var purchaseRequisitionDetails = await _repository.GetPurchaseRequisitionById(id);

                if (purchaseRequisitionDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseRequisitions  hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseRequisitions with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    PurchaseRequisitionDto PurchaseRequisitionDto = _mapper.Map<PurchaseRequisitionDto>(purchaseRequisitionDetails);
                    List<PrItemsDto> prItemsDtoList = new List<PrItemsDto>();
                    foreach (var itemDetails in purchaseRequisitionDetails.PrItemList)
                    {
                        PrItemsDto prItemsDtos = _mapper.Map<PrItemsDto>(itemDetails);
                        prItemsDtos.PrAddprojectsDtoList = _mapper.Map<List<PrAddProjectDto>>(itemDetails.PrAddprojects);
                        prItemsDtos.PrAddDeliverySchedulesDtoList = _mapper.Map<List<PrAddDeliveryScheduleDto>>(itemDetails.PrAddDeliverySchedules);
                        prItemsDtoList.Add(prItemsDtos);
                    }
                    PurchaseRequisitionDto.PrItemsDtoList = prItemsDtoList;

                    serviceResponse.Data = PurchaseRequisitionDto;
                    serviceResponse.Message = "Returned PurchaseRequisitions";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPurchaseRequisitionById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<PurchaseRequisitionController>
        [HttpPost]
        public async Task<IActionResult> CreatePurchaseRequisition([FromBody] PurchaseRequisitionDtoPost purchaseRequistionDtoPost)
        {
            ServiceResponse<PurchaseRequisitionDtoPost> serviceResponse = new ServiceResponse<PurchaseRequisitionDtoPost>();
            try
            {
                if (purchaseRequistionDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseRequisitions object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PurchaseRequisitions object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PurchaseRequisitions object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PurchaseRequisitions object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var purchaseRequisition = _mapper.Map<PurchaseRequisition>(purchaseRequistionDtoPost);
                var itemsDto = purchaseRequistionDtoPost.PrItemsDtoPostList;
                var itemsDtoList = new List<PrItem>();
                for (int i = 0; i < itemsDto.Count; i++)
                {
                    PrItem prItemsDetails = _mapper.Map<PrItem>(itemsDto[i]);
                    prItemsDetails.PrAddprojects = _mapper.Map<List<PrAddProject>>(itemsDto[i].PrAddprojectsDtoPostList);
                    prItemsDetails.PrAddDeliverySchedules = _mapper.Map<List<PrAddDeliverySchedule>>(itemsDto[i].PrAddDeliverySchedulesDtoPostList);
                    itemsDtoList.Add(prItemsDetails);
                }
                purchaseRequisition.PrItemList = itemsDtoList;

                await _repository.CreatePurchaseRequisition(purchaseRequisition);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseRequisitions Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<PurchaseRequistionController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchaseRequisition(int id, [FromBody] PurchaseRequisitionDtoUpdate purchaseRequisitionDtoUpdate)
        {
            ServiceResponse<PurchaseRequisitionDtoUpdate> serviceResponse = new ServiceResponse<PurchaseRequisitionDtoUpdate>();
            try
            {
                if (purchaseRequisitionDtoUpdate is null)
                {
                    _logger.LogError("Update PurchaseRequisitions object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update PurchaseRequisitions object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update PurchaseRequisitions object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update PurchaseRequisitions object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updatePurchaseRequistion = await _repository.GetPurchaseRequisitionById(id);
                if (updatePurchaseRequistion is null)
                {
                    _logger.LogError($"Update PurchaseRequisitions with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update PurchaseRequisitions hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var purchaseRequistionList = _mapper.Map<PurchaseRequisition>(updatePurchaseRequistion);
                var itemsDto = purchaseRequisitionDtoUpdate.PrItemsDtoUpdateList;
                var itemsList = new List<PrItem>();
                for (int i = 0; i < itemsDto.Count; i++)
                {
                    PrItem prItemsDetails = _mapper.Map<PrItem>(itemsDto[i]);
                    prItemsDetails.PrAddprojects = _mapper.Map<List<PrAddProject>>(itemsDto[i].PrAddprojectsDtoUpdateList);
                    prItemsDetails.PrAddDeliverySchedules = _mapper.Map<List<PrAddDeliverySchedule>>(itemsDto[i].PrAddDeliverySchedulesDtoUpdateList);
                    itemsList.Add(prItemsDetails);
                }
                purchaseRequistionList.PrItemList = itemsList;
                var data = _mapper.Map(purchaseRequisitionDtoUpdate, purchaseRequistionList);
                string result = await _repository.UpdatePurchaseRequisition(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseRequisitions Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdatePurchaseRequisition action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<PurchaseRequisitionController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseRequisition(int id)
        {
            ServiceResponse<PurchaseRequisitionDto> serviceResponse = new ServiceResponse<PurchaseRequisitionDto>();
            try
            {
                var deletePurchaseRequisition = await _repository.GetPurchaseRequisitionById(id);
                if (deletePurchaseRequisition == null)
                {
                    _logger.LogError($"Delete PurchaseRequisitions with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete PurchaseRequisitions hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeletePurchaseRequisition(deletePurchaseRequisition);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseRequisitions Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivePurchaseRequisitionNameList()
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>>();
            try
            {
                var listOfPurchaseRequisition = await _repository.GetAllActivePurchaseRequisitionNameList();
                //_logger.LogInfo("Returned all PurchaseRequisitions");
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionIdNameListDto>>(listOfPurchaseRequisition);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseRequisitions";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActivePurchaseRequisitionNameList action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPendingPurchaseRequisitionApprovalINameList()
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>>();
            try
            {
                var listOfPurchaseRequisition = await _repository.GetAllPendingPurchaseRequisitionApprovalINameList();
                //_logger.LogInfo("Returned all PurchaseRequisitions");
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionIdNameListDto>>(listOfPurchaseRequisition);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PendingApprovalIPurchaseRequisition";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPendingPurchaseRequisitionApprovalINameList action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPendingPurchaseRequisitionApprovalIINameList()
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>>();
            try
            {
                var listOfPurchaseRequisition = await _repository.GetAllPendingPurchaseRequisitionApprovalIINameList();
                //_logger.LogInfo("Returned all PurchaseRequisitions");
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionIdNameListDto>>(listOfPurchaseRequisition);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PendingApprovalIIPurchaseRequisition";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPendingPurchaseRequisitionApprovalIINameList action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{PRNumber}")]
        public async Task<IActionResult> ActivatePurchaseRequisitionApprovalI(string PRNumber)
        {
            ServiceResponse<PurchaseRequisitionDto> serviceResponse = new ServiceResponse<PurchaseRequisitionDto>();

            try
            {
                var purchaseRequisition = await _repository.GetPurchaseRequisitionByPRNumber(PRNumber);
                if (purchaseRequisition is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseRequisitionApprovalI object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseRequisitionApprovalI with string: {PRNumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                purchaseRequisition.PRApprovalI = true;
                purchaseRequisition.PRApprovedIBy = "Admin";
                purchaseRequisition.PRApprovedIDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseRequisition(purchaseRequisition);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivatePurchaseRequisitionApprovalI action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{PRNumber}")]
        public async Task<IActionResult> ActivatePurchaseRequisitionApprovalII(string PRNumber)
        {
            ServiceResponse<PurchaseRequisitionDto> serviceResponse = new ServiceResponse<PurchaseRequisitionDto>();

            try
            {
                var purchaseRequisition = await _repository.GetPurchaseRequisitionByPRNumber(PRNumber);
                if (purchaseRequisition is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseRequisitionApprovalII object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseRequisitionApprovalII with string: {PRNumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                purchaseRequisition.PRApprovalII = true;
                purchaseRequisition.PRApprovedIIBy = "Admin";
                purchaseRequisition.PRApprovedIIDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseRequisition(purchaseRequisition);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivatePurchaseRequisitionApprovalII action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}