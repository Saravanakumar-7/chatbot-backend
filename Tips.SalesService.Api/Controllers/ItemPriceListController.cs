using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using System.Net;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ItemPriceListController : ControllerBase
    {
        private IItemPriceListRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public ItemPriceListController(IItemPriceListRepository repository,  ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
          
        }
        // GET: api/<ItemPriceListController>
        [HttpGet]
        public async Task<IActionResult> GetAllItemPriceList([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ItemPriceListDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemPriceListDto>>();
            try
            {
                var GetAllItemPriceList = await _repository.GetAllItemPriceList(pagingParameter);
                var metadata = new
                {
                    GetAllItemPriceList.TotalCount,
                    GetAllItemPriceList.PageSize,
                    GetAllItemPriceList.CurrentPage,
                    GetAllItemPriceList.HasNext,
                    GetAllItemPriceList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ItemPriceList");
                var result = _mapper.Map<IEnumerable<ItemPriceListDto>>(GetAllItemPriceList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ItemPriceList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET api/<ItemPriceListController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemPriceListById(int id)
        {
            ServiceResponse<ItemPriceListDto> serviceResponse = new ServiceResponse<ItemPriceListDto>();

            try
            {
                var getItemPriceListById = await _repository.GetItemPriceListById(id);
                if (getItemPriceListById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ItemPriceList with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ItemPriceList with id: {id}, hasn't been found.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned ItemPriceList with id: {id}");
                    var result = _mapper.Map<ItemPriceListDto>(getItemPriceListById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned ItemPriceList with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetItemPriceListById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetItemPriceListByItemNo(string itemNo)
        {
            ServiceResponse<IEnumerable<ItemPriceListDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemPriceListDto>>();

            try
            {
                var getItemPriceList = await _repository.GetItemPriceListByItemNo(itemNo);
                if (getItemPriceList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ItemPriceList with id: {itemNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ItemPriceList with id: {itemNo}, hasn't been found.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned ItemPriceList with id: {itemNo}");
                    var result = _mapper.Map<IEnumerable<ItemPriceListDto>>(getItemPriceList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned ItemPriceList with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetItemPriceListByItemNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //test

        [HttpPost]
        public async Task<IActionResult> GetItemPriceListByListOfItemNoAndPriceListName(ItemNumberAndPriceNameListDto[][] itemNumberAndPriceNameListDtos)
        {
            ServiceResponse<IEnumerable<ItemPriceList>> serviceResponse = new ServiceResponse<IEnumerable<ItemPriceList>>();
            try
            {
                var test = new List<ItemPriceList>();

                foreach (var array in itemNumberAndPriceNameListDtos)
                {
                    foreach (var item in array)
                    {
                        var itemNumber = item.ItemNumber;
                        var priceListName = item.PriceListName;
                        var getItemPriceLists = await _repository.GetItemPriceListByListOfItemNoAndPriceListName(itemNumber, priceListName);
                        ItemPriceList bTODeliveryOrderItemsDetails = _mapper.Map<ItemPriceList>(item);
                        bTODeliveryOrderItemsDetails.Qty = getItemPriceLists.Qty;
                        bTODeliveryOrderItemsDetails.Description = getItemPriceLists.Description;
                        bTODeliveryOrderItemsDetails.UOC = getItemPriceLists.UOC;
                        bTODeliveryOrderItemsDetails.LeastCost = getItemPriceLists.LeastCost;
                        bTODeliveryOrderItemsDetails.Id = getItemPriceLists.Id;
                        bTODeliveryOrderItemsDetails.LeastCostPlus = getItemPriceLists.LeastCostPlus;
                        bTODeliveryOrderItemsDetails.LeastCostminus = getItemPriceLists.LeastCostminus;
                        bTODeliveryOrderItemsDetails.DiscountMinus = getItemPriceLists.DiscountMinus;
                        bTODeliveryOrderItemsDetails.DiscountPlus = getItemPriceLists.DiscountPlus;
                        bTODeliveryOrderItemsDetails.Markup = getItemPriceLists.Markup;
                        bTODeliveryOrderItemsDetails.PriceListName = getItemPriceLists.PriceListName;
                        bTODeliveryOrderItemsDetails.ValidThrough = getItemPriceLists.ValidThrough;
                        bTODeliveryOrderItemsDetails.IsDiscountApplicable = getItemPriceLists.IsDiscountApplicable;
                        bTODeliveryOrderItemsDetails.Unit = getItemPriceLists.Unit;
                        bTODeliveryOrderItemsDetails.CreatedOn = getItemPriceLists.CreatedOn;
                        bTODeliveryOrderItemsDetails.CreatedBy = getItemPriceLists.CreatedBy;
                        bTODeliveryOrderItemsDetails.LastModifiedBy = getItemPriceLists.LastModifiedBy;
                        bTODeliveryOrderItemsDetails.LastModifiedOn = getItemPriceLists.LastModifiedOn;
                        bTODeliveryOrderItemsDetails.ReleaseLpId = getItemPriceLists.ReleaseLpId;
                        bTODeliveryOrderItemsDetails.ReleaseLp = getItemPriceLists.ReleaseLp;
                        test.Add(bTODeliveryOrderItemsDetails);
                    }
                }
                var result = _mapper.Map<IEnumerable<ItemPriceList>>(test);

                serviceResponse.Data = result;
                serviceResponse.Message = "Returned ItemPriceList Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ItemPriceListdetail action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        } 

        // POST api/<ItemPriceListController>
        [HttpPost]
        public IActionResult CreateItemPriceList([FromBody] ItemPriceListPostDto itemPriceListPostDto)
        {
            ServiceResponse<ItemPriceListDto> serviceResponse = new ServiceResponse<ItemPriceListDto>();

            try
            {
                if (itemPriceListPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ItemPriceList object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ItemPriceList object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ItemPriceList object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid ItemPriceList object sent from client.");

                    return BadRequest(serviceResponse);
                }
                var itemPriceListCreate = _mapper.Map<ItemPriceList>(itemPriceListPostDto);
                _repository.CreateItemPriceList(itemPriceListCreate);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ItemPriceList Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetItemPriceListById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateItemPriceList action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetItemPriceListByItemNoAndPriceListName(string ItemNo, string priceListName)
        {
            ServiceResponse<IEnumerable<ItemPriceListDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemPriceListDto>>();

            try
            {
                var getItemPriceList = await _repository.GetItemPriceListByItemNoAndPriceListName(ItemNo, priceListName);
                if (getItemPriceList == null)
                {
                    _logger.LogError($"ItemPriceListdetail with id: {ItemNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ItemPriceListdetail with id: {ItemNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned ItemPriceListdetail with id: {ItemNo}");
                    var result = _mapper.Map<IEnumerable<ItemPriceListDto>>(getItemPriceList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned ItemPriceList Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ItemPriceListdetail action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }
        // PUT api/<ItemPriceListController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemPriceList(int id, [FromBody] ItemPriceListUpdateDto itemPriceListUpdateDto)
        {
            ServiceResponse<ItemPriceListDto> serviceResponse = new ServiceResponse<ItemPriceListDto>();

            try
            {
                if (itemPriceListUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update ItemPriceList object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update ItemPriceList object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update ItemPriceList object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update ItemPriceList object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var getItemPriceListDetails = await _repository.GetItemPriceListById(id);
                if (getItemPriceListDetails is null)
                {
                    _logger.LogError($"Update ItemPriceList with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update ItemPriceList with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(itemPriceListUpdateDto, getItemPriceListDetails);
                string result = await _repository.UpdateItemPriceList(getItemPriceListDetails);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ItemPriceList Updated Successfully";
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
                _logger.LogError($"Something went wrong inside UpdateItemPriceList action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<ItemPriceListController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemPriceList(int id)
        {
            ServiceResponse<ItemPriceListDto> serviceResponse = new ServiceResponse<ItemPriceListDto>();

            try
            {
                var deleteItemPriceList = await _repository.GetItemPriceListById(id);
                if (deleteItemPriceList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete ItemPriceList object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete ItemPriceList with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.DeleteItemPriceList(deleteItemPriceList);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "ItemPriceList Deleted Successfully";
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
                _logger.LogError($"Something went wrong inside DeleteItemPriceList action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        //getallpricelist from itemmasterpricelist model

        [HttpGet]
        public async Task<IActionResult> GetAllItemPriceNameList()
        {
            ServiceResponse<IEnumerable<ItemPriceListNameDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemPriceListNameDto>>();
            try
            {
                var GetAllItemPriceList = await _repository.GetAllItemPriceNameList();                

                _logger.LogInfo("Returned all ItemPriceListName");
                var result = _mapper.Map<IEnumerable<ItemPriceListNameDto>>(GetAllItemPriceList);

                 serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ItemPriceListName";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //get all pricelist item number list

        [HttpGet]
        public async Task<IActionResult> GetAllItemNumberList()
        {
            ServiceResponse<IEnumerable<ItemNumberListDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemNumberListDto>>();
            try
            {
                var GetAllItemNumber= await _repository.GetAllItemNumberList();

                _logger.LogInfo("Returned all GetAllItemNumber");
                var result = _mapper.Map<IEnumerable<ItemNumberListDto>>(GetAllItemNumber);

                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ItemNumber";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


    }
}
