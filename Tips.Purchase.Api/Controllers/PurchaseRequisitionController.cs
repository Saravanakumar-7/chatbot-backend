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

        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseRequistion([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionDto>>();
            try
            {
                var allPurchaseRequisitionDetails = await _repository.GetAllPurchaseRequisition(pagingParameter);
                var metadata = new
                {
                    allPurchaseRequisitionDetails.TotalCount,
                    allPurchaseRequisitionDetails.PageSize,
                    allPurchaseRequisitionDetails.CurrentPage,
                    allPurchaseRequisitionDetails.HasNext,
                    allPurchaseRequisitionDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all PurchaseRequisitions");
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionDto>>(allPurchaseRequisitionDetails);
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


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseRequisitionById(int id)
        {
            ServiceResponse<PurchaseRequisitionDto> serviceResponse = new ServiceResponse<PurchaseRequisitionDto>();
            try
            {
                var purchaseRequisitionDetailsbyId = await _repository.GetPurchaseRequisitionById(id);

                if (purchaseRequisitionDetailsbyId == null)
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
                    _logger.LogInfo($"Returned purchaseRequisition with id: {id}");

                    PurchaseRequisitionDto PurchaseRequisitionDto = _mapper.Map<PurchaseRequisitionDto>(purchaseRequisitionDetailsbyId);
                    List<PrItemsDto> prItemsDtoList = new List<PrItemsDto>();

                    if (purchaseRequisitionDetailsbyId.PrItemList != null)
                    {


                        foreach (var itemDetails in purchaseRequisitionDetailsbyId.PrItemList)
                        {
                            PrItemsDto prItemsDtos = _mapper.Map<PrItemsDto>(itemDetails);
                            prItemsDtos.PrAddprojectsDtoList = _mapper.Map<List<PrAddProjectDto>>(itemDetails.PrAddprojects);
                            prItemsDtos.PrAddDeliverySchedulesDtoList = _mapper.Map<List<PrAddDeliveryScheduleDto>>(itemDetails.PrAddDeliverySchedules);
                            prItemsDtoList.Add(prItemsDtos);
                        }
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

                var createPurchaseRequisition = _mapper.Map<PurchaseRequisition>(purchaseRequistionDtoPost);
                var prItemsDto = purchaseRequistionDtoPost.PrItemsDtoPostList;
                var pritemsDtoList = new List<PrItem>();

                if (prItemsDto != null)
                {
                    for (int i = 0; i < prItemsDto.Count; i++)
                    {
                        PrItem prItems = _mapper.Map<PrItem>(prItemsDto[i]);
                        prItems.PrAddprojects = _mapper.Map<List<PrAddProject>>(prItemsDto[i].PrAddprojectsDtoPostList);
                        prItems.PrAddDeliverySchedules = _mapper.Map<List<PrAddDeliverySchedule>>(prItemsDto[i].PrAddDeliverySchedulesDtoPostList);
                        pritemsDtoList.Add(prItems);
                    }
                }
                createPurchaseRequisition.PrItemList = pritemsDtoList;

                await _repository.CreatePurchaseRequisition(createPurchaseRequisition);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseRequisitions Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreatePurchaseRequisition action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


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
                var purchaseRequistionById = await _repository.GetPurchaseRequisitionById(id);
                if (purchaseRequistionById is null)
                {
                    _logger.LogError($"Update PurchaseRequisitions with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update PurchaseRequisitions hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var purchaseRequisition = _mapper.Map<PurchaseRequisition>(purchaseRequistionById);
                var pritemsDto = purchaseRequisitionDtoUpdate.PrItemsDtoUpdateList;
                var pritemsList = new List<PrItem>();

                if (pritemsDto != null)
                {
                    for (int i = 0; i < pritemsDto.Count; i++)
                    {
                        PrItem prItems = _mapper.Map<PrItem>(pritemsDto[i]);
                        prItems.PrAddprojects = _mapper.Map<List<PrAddProject>>(pritemsDto[i].PrAddprojectsDtoUpdateList);
                        prItems.PrAddDeliverySchedules = _mapper.Map<List<PrAddDeliverySchedule>>(pritemsDto[i].PrAddDeliverySchedulesDtoUpdateList);
                        pritemsList.Add(prItems);
                    }
                }

                purchaseRequisition.PrItemList = pritemsList;
                var updatePurchaseRequisition = _mapper.Map(purchaseRequisitionDtoUpdate, purchaseRequisition);
                string result = await _repository.UpdatePurchaseRequisition(updatePurchaseRequisition);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseRequisition(int id)
        {
            ServiceResponse<PurchaseRequisitionDto> serviceResponse = new ServiceResponse<PurchaseRequisitionDto>();
            try
            {
                var purchaseRequisitionDetailById = await _repository.GetPurchaseRequisitionById(id);
                if (purchaseRequisitionDetailById == null)
                {
                    _logger.LogError($"Delete PurchaseRequisitions with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete PurchaseRequisitions hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeletePurchaseRequisition(purchaseRequisitionDetailById);
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
                var allActivePRNameDetails = await _repository.GetAllActivePurchaseRequisitionNameList();
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionIdNameListDto>>(allActivePRNameDetails);
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
                var allPendingPRApprovalINameDetails = await _repository.GetAllPendingPurchaseRequisitionApprovalINameList();
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionIdNameListDto>>(allPendingPRApprovalINameDetails);
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
                var allPendingPRApprovalIINameDetails = await _repository.GetAllPendingPurchaseRequisitionApprovalIINameList();
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionIdNameListDto>>(allPendingPRApprovalIINameDetails);
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
                var prDetailByPRNumber = await _repository.GetPurchaseRequisitionByPRNumber(PRNumber);
                if (prDetailByPRNumber is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseRequisitionApprovalI object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseRequisitionApprovalI with string: {PRNumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                prDetailByPRNumber.PRApprovalI = true;
                prDetailByPRNumber.PRApprovedIBy = "Admin";
                prDetailByPRNumber.PRApprovedIDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseRequisition(prDetailByPRNumber);
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
                var prDetailByPRNumber = await _repository.GetPurchaseRequisitionByPRNumber(PRNumber);
                if (prDetailByPRNumber is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseRequisitionApprovalII object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseRequisitionApprovalII with string: {PRNumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                prDetailByPRNumber.PRApprovalII = true;
                prDetailByPRNumber.PRApprovedIIBy = "Admin";
                prDetailByPRNumber.PRApprovedIIDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseRequisition(prDetailByPRNumber);
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