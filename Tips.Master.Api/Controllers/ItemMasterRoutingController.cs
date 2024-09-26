using System.Collections.Generic;
using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repository;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ItemMasterRoutingController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public ItemMasterRoutingController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        //public async Task<IActionResult> GetAllItemsProcessLists([FromBody] List<string> itemMasterRoutingIdNoListDtos)
        //{
        //    ServiceResponse<List<ItemMasterRouting>> serviceResponse = new ServiceResponse<List<ItemMasterRouting>>();
        //    ItemMasterRouting itemMasterRouting = null;
        //    try
        //    {
        //        if (itemMasterRoutingIdNoListDtos is null)
        //        {
        //            _logger.LogError("ItemNumber object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Item Number object is null";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }


        //        List<ItemMasterRouting> rfqCustomerSupportLists = new List<ItemMasterRouting>();

        //        for (int i = 0; i < itemMasterRoutingIdNoListDtos.Count; i++)
        //        {
        //            itemMasterRouting = await _repository.ItemMasterRoutingRepository.GetAllItemsProcessList(itemMasterRoutingIdNoListDtos[i].id);
        //            rfqCustomerSupportLists.Add(itemMasterRouting);

        //        }
        //        List<ItemMasterRouting> rfqCSDto = _mapper.Map<List<ItemMasterRouting>>(rfqCustomerSupportLists);
        //        rfqCSDto = rfqCustomerSupportLists;

        //        serviceResponse.Data = rfqCustomerSupportLists;
        //        serviceResponse.Message = "List Of ItemNumber ";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside UpdateRfq action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}




        [HttpPost]
        public async Task<IActionResult> GetItemsRoutingDetailsForLpCosting([FromBody] List<string> itemNumber)
        {
            ServiceResponse<List<ItemMasterRoutingListDto>> serviceResponse = new ServiceResponse<List<ItemMasterRoutingListDto>>();
            List<ItemMasterRoutingListDto> itemMasterRouting = null;
            try
            {
                if (itemNumber is null)
                {
                    _logger.LogError("ItemNumber object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Item Number object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                 
                itemMasterRouting = await _repository.ItemMasterRoutingRepository.GetItemsRoutingDetailsForLpCosting(itemNumber);
                List<ItemMasterRoutingListDto> rfqCSDto = _mapper.Map<List<ItemMasterRoutingListDto>>(itemMasterRouting);
                //rfqCSDto = itemMasterRouting;
                serviceResponse.Data = rfqCSDto;
                serviceResponse.Message = "List Of ItemNumber ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateRfq action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


    }
}