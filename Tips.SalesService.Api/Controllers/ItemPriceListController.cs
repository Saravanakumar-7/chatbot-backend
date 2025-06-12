using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class ItemPriceListController : ControllerBase
    {
        private IItemPriceListRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly IHttpClientFactory _clientFactory;
        public ItemPriceListController(IItemPriceListRepository repository, IHttpClientFactory clientFactory, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _clientFactory = clientFactory;
        }
        // GET: api/<ItemPriceListController>
        [HttpGet]
        public async Task<IActionResult> GetAllItemPriceList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<ItemPriceListDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemPriceListDto>>();
            try
            {
                var GetAllItemPriceList = await _repository.GetAllItemPriceList(pagingParameter, searchParammes);
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
                _logger.LogError($"Error Occured in GetAllItemPriceList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllItemPriceList API : \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetItemPriceListById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetItemPriceListById API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetItemPriceListByPriceListName(string priceListName, [FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<ItemPriceListDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemPriceListDto>>();

            try
            {
                var itemPriceListDetails = await _repository.GetItemPriceListByPriceListName(priceListName, pagingParameter, searchParammes);

                var metadata = new
                {
                    itemPriceListDetails.TotalCount,
                    itemPriceListDetails.PageSize,
                    itemPriceListDetails.CurrentPage,
                    itemPriceListDetails.HasNext,
                    itemPriceListDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ItemPriceList");
                if (itemPriceListDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ItemPriceList with priceListName hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ItemPriceList with priceListName: {priceListName}, hasn't been found.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned ItemPriceList with id: {priceListName}");
                    var result = _mapper.Map<IEnumerable<ItemPriceListDto>>(itemPriceListDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned ItemPriceList with priceListName successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetItemPriceListByPriceListName API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetItemPriceListByPriceListName API : \n {ex.Message}";
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
                if (getItemPriceList.Count()== 0)
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
                _logger.LogError($"Error Occured in GetItemPriceListByItemNo API for the following itemNo:{itemNo} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetItemPriceListByItemNo API for the following itemNo:{itemNo} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //test

        [HttpPost]
        public async Task<IActionResult> GetItemPricesByListOfItemNoAndPriceListNames(List<ItemNumberAndPriceNameListDto> itemNumberAndPriceNameListDtos)
        {
            ServiceResponse<IEnumerable<ItemPriceList>> serviceResponse = new ServiceResponse<IEnumerable<ItemPriceList>>();
            try
            {
                if (itemNumberAndPriceNameListDtos is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ItemPriceList sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ItemPriceList sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ItemPriceList.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid ItemPriceList.");
                    return BadRequest(serviceResponse);
                }

                var itemPriceLists = new List<ItemPriceList>();                
                
                    foreach (var item in itemNumberAndPriceNameListDtos)
                    { 
                        var itemPriceList = await _repository.GetItemPricesByListOfItemNoAndPriceListNames(item.ItemNumber, item.PriceListName);
                    if(itemPriceList != null)
                    {
                        itemPriceLists.Add(itemPriceList);
                    }
                }
                
                serviceResponse.Data = itemPriceLists;
                serviceResponse.Message = "Returned ItemPriceList Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetItemPricesByListOfItemNoAndPriceListNames API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetItemPricesByListOfItemNoAndPriceListNames API : \n {ex.Message}";
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
                serviceResponse.Message = $"Error Occured in CreateItemPriceList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in CreateItemPriceList API : \n {ex.Message} \n{ex.InnerException}");
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
                if (getItemPriceList.Count() == 0)
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
                _logger.LogError($"Error Occured in GetItemPriceListByItemNoAndPriceListName API for the following ItemNo : {ItemNo} and priceListName : {priceListName} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetItemPriceListByItemNoAndPriceListName API for the following ItemNo : {ItemNo} and priceListName : {priceListName} \n {ex.Message}";
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
                serviceResponse.Message = $"Error Occured in UpdateItemPriceList API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateItemPriceList API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
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
                serviceResponse.Message = $"Error Occured in DeleteItemPriceList API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteItemPriceList API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
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
                _logger.LogError($"Error Occured in GetAllItemPriceNameList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllItemPriceNameList API : \n {ex.Message}";
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
                serviceResponse.Data = GetAllItemNumber;
                serviceResponse.Message = "Returned all ItemNumber";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllItemNumberList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllItemNumberList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


    }
}
