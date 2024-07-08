using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class GrinsForServiceItemsController : ControllerBase
    {
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IGrinsForServiceItemsRepository _repository;
        public GrinsForServiceItemsController(IGrinsForServiceItemsRepository repository)
        {
           _repository=repository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllGrinsForServiceItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)

        {
            ServiceResponse<IEnumerable<GrinsForServiceItemsDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinsForServiceItemsDto>>();

            try
            {
                var GrinsForServiceItemsForServiceItems = await _repository.GrinsForServiceItemsForServiceItems(pagingParameter, searchParams);

                if (GrinsForServiceItemsForServiceItems == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GrinsForServiceItems data not found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GrinsForServiceItems data not found in db");
                    return NotFound(serviceResponse);
                }
                var metadata = new
                {
                    GrinsForServiceItemsForServiceItems.TotalCount,
                    GrinsForServiceItemsForServiceItems.PageSize,
                    GrinsForServiceItemsForServiceItems.CurrentPage,
                    GrinsForServiceItemsForServiceItems.HasNext,
                    GrinsForServiceItemsForServiceItems.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all GrinsForServiceItems");
                var result = _mapper.Map<IEnumerable<GrinsForServiceItemsDto>>(GrinsForServiceItemsForServiceItems);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GrinsForServiceItems Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Internal server error {ex.Message}{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
