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
    public class PurchaseOrderController : ControllerBase
    {
        private IPurchaseOrderRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        
        public PurchaseOrderController(IPurchaseOrderRepository repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<PurchaseOrderController>
        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseOrder([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<PurchaseOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderDto>>();
            try
            {
                var AllPurchaseOrder = await _repository.GetAllPurchaseOrder(pagingParameter);
                var metadata = new
                {
                    AllPurchaseOrder.TotalCount,
                    AllPurchaseOrder.PageSize,
                    AllPurchaseOrder.CurrentPage,
                    AllPurchaseOrder.HasNext,
                    AllPurchaseOrder.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all PurchaseOrder");
                var result = _mapper.Map<IEnumerable<PurchaseOrderDto>>(AllPurchaseOrder);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseOrder";
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

        // GET api/<PurchaseOrderController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderById(int id)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();
            try
            {
                var purchaseOrderDetailsbyId = await _repository.GetPurchaseOrderById(id);

                if (purchaseOrderDetailsbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrder  hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    PurchaseOrderDto purchaseOrderDto = _mapper.Map<PurchaseOrderDto>(purchaseOrderDetailsbyId);
                    List<PoItemsDto> poItemsDtoList = new List<PoItemsDto>();
                    foreach (var itemDetails in purchaseOrderDetailsbyId.POItemList)
                    {
                        PoItemsDto poItemsDtos = _mapper.Map<PoItemsDto>(itemDetails);
                        poItemsDtos.POAddprojectsDtoList = _mapper.Map<List<PoAddProjectDto>>(itemDetails.POAddprojects);
                        poItemsDtos.POAddDeliverySchedulesDtoList = _mapper.Map<List<PoAddDeliveryScheduleDto>>(itemDetails.POAddDeliverySchedules);
                        poItemsDtoList.Add(poItemsDtos);
                    }

                    purchaseOrderDto.POItemsDtoList = poItemsDtoList;     
                    serviceResponse.Data = purchaseOrderDto;
                    serviceResponse.Message = "Returned PurchaseOrder";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPurchaseOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<PurchaseOrderController>
        [HttpPost]
        public async Task<IActionResult> CreatePurchaseOrder([FromBody] PurchaseOrderDtoPost purchaseOrderDtoPost)
        {
            ServiceResponse<PurchaseOrderDtoPost> serviceResponse = new ServiceResponse<PurchaseOrderDtoPost>();
            try
            {
                if (purchaseOrderDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PurchaseOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PurchaseOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PurchaseOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var purchaseOrder = _mapper.Map<PurchaseOrder>(purchaseOrderDtoPost);
                var itemsDto = purchaseOrderDtoPost.POItemsDtoPostList;
                var itemsDtoList = new List<PoItem>();
                for(int i=0;i< itemsDto.Count;i++)
                {
                    PoItem poItemsDetails=_mapper.Map<PoItem>(itemsDto[i]);
                    poItemsDetails.POAddprojects = _mapper.Map<List<PoAddProject>>(itemsDto[i].POAddprojectsDtoPostList);
                    poItemsDetails.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliverySchedule>>(itemsDto[i].POAddDeliverySchedulesDtoPostList);
                    
                    itemsDtoList.Add(poItemsDetails);
                }
                purchaseOrder.POItemList=itemsDtoList;

                await _repository.CreatePurchaseOrder(purchaseOrder);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseOrder Successfully Created";
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


        // PUT api/<PurchaseOrderController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchaseOrder(int id, [FromBody] PurchaseOrderDtoUpdate purchaseOrderDtoUpdate)
        {
            ServiceResponse<PurchaseOrderDtoUpdate> serviceResponse = new ServiceResponse<PurchaseOrderDtoUpdate>();
            try
            {
                if (purchaseOrderDtoUpdate is null)
                {
                    _logger.LogError("Update PurchaseOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update PurchaseOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update PurchaseOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update PurchaseOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updatePurchaseOrder = await _repository.GetPurchaseOrderById(id);
                if (updatePurchaseOrder is null)
                {
                    _logger.LogError($"Update PurchaseOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update PurchaseOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var purchaseOrderList = _mapper.Map<PurchaseOrder>(updatePurchaseOrder);
                var itemsDto = purchaseOrderDtoUpdate.POItemsDtoUpdateList;
                var itemsList = new List<PoItem>();
                for (int i = 0; i < itemsDto.Count; i++)
                {
                    PoItem poItemsDetails = _mapper.Map<PoItem>(itemsDto[i]);
                    poItemsDetails.POAddprojects = _mapper.Map<List<PoAddProject>>(itemsDto[i].POAddprojectsDtoUpdateList);
                    poItemsDetails.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliverySchedule>>(itemsDto[i].POAddDeliverySchedulesDtoUpdateList);  
                    itemsList.Add(poItemsDetails);
                }
                purchaseOrderList.POItemList = itemsList;
                var data = _mapper.Map(purchaseOrderDtoUpdate, purchaseOrderList);
                string result = await _repository.UpdatePurchaseOrder(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdatePurchaseOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<CompanyMasterController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrder(int id)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();
            try
            {
                var deletePurchaseOrder = await _repository.GetPurchaseOrderById(id);
                if (deletePurchaseOrder == null)
                {
                    _logger.LogError($"Delete PurchaseOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete PurchaseOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeletePurchaseOrder(deletePurchaseOrder);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseOrder Successfully Deleted";
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
        public async Task<IActionResult> GetAllActivePurchaseOrderNameList()
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var AllActivePurchaseOrder = await _repository.GetAllActivePurchaseOrderNameList();
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(AllActivePurchaseOrder);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActivePurchaseOrderNameList action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPendingPurchaseOrderApprovalINameList()
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var AllPendingPurchaseOrderApprovalIName = await _repository.GetAllPendingPurchaseOrderApprovalINameList();
             
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(AllPendingPurchaseOrderApprovalIName);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PendingApprovalIPurchaseOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPendingPurchaseOrderApprovalINameList action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPendingPurchaseOrderApprovalIINameList()
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var AllPendingPurchaseOrderApprovalIIName = await _repository.GetAllPendingPurchaseOrderApprovalIINameList();
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(AllPendingPurchaseOrderApprovalIIName);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PendingApprovalIIPurchaseOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPendingPurchaseOrderApprovalIINameList action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{PONumber}")]
        public async Task<IActionResult> ActivatePurchaseOrderApprovalI(string PONumber)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();

            try
            {
                var ActivatePOApprovalI = await _repository.GetPurchaseOrderByPONumber(PONumber);
                if (ActivatePOApprovalI is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrderApprovalI object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseOrderApprovalI with string: {PONumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                ActivatePOApprovalI.POApprovalI = true;
                ActivatePOApprovalI.POApprovedIBy = "Admin";
                ActivatePOApprovalI.POApprovedIDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseOrder(ActivatePOApprovalI);
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
                _logger.LogError($"Something went wrong inside ActivatePurchaseOrderApprovalI action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{PONumber}")]
        public async Task<IActionResult> ActivatePurchaseOrderApprovalII(string PONumber)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();

            try
            {
                var ActivatePOApprovalII = await _repository.GetPurchaseOrderByPONumber(PONumber);
                if (ActivatePOApprovalII is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrderApprovalII object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseOrderApprovalII with string: {PONumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                ActivatePOApprovalII.POApprovalII = true;
                ActivatePOApprovalII.POApprovedIIBy = "Admin";
                ActivatePOApprovalII.POApprovedIIDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseOrder(ActivatePOApprovalII);
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
                _logger.LogError($"Something went wrong inside ActivatePurchaseOrderApprovalII action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{VendorName}")]
        public async Task<IActionResult> GetAllPoNumberListByVendorName(string VendorName)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var AllPONumberbyVendorName = await _repository.GetAllPoNumberListByVendorName(VendorName);
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(AllPONumberbyVendorName);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PoNumberList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPoNumberListByVendorName action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{PoNumber}")]
        public async Task<IActionResult> GetAllPoItemNumberListByPoNumber(string PoNumber)
        {
            ServiceResponse<IEnumerable<PurchaseOrderItemNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderItemNoListDto>>();
            try
            {
                var AllPOItemNumberbyPONumber = await _repository.GetAllPoItemNumberListByPoNumber(PoNumber);
                var result = _mapper.Map<IEnumerable<PurchaseOrderItemNoListDto>>(AllPOItemNumberbyPONumber);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PoItemNumberList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPoItemNumberListByPoNumber action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


    }
}
