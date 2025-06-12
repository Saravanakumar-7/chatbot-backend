using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CityController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public CityController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllCities([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CityDto>> serviceResponse = new ServiceResponse<IEnumerable<CityDto>>();
            try
            {

                var cities = await _repository.CityRepository.GetAllCities(pagingParameter, searchParams);

                var metadata = new
                {
                    cities.TotalCount,
                    cities.PageSize,
                    cities.CurrentPage,
                    cities.HasNext,
                    cities.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all cities");
                var result = _mapper.Map<IEnumerable<CityDto>>(cities);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Cities  Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllCities API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllCities API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveCities([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CityDto>> serviceResponse = new ServiceResponse<IEnumerable<CityDto>>();

            try
            {
                var citiesList = await _repository.CityRepository.GetAllActiveCities(pagingParameter,searchParams);
                _logger.LogInfo("Returned all cities");
                var result = _mapper.Map<IEnumerable<CityDto>>(citiesList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active cities Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveCities API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveCities API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }
        // GET api/<DemoStatusController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityById(int id)
        {
            ServiceResponse<CityDto> serviceResponse = new ServiceResponse<CityDto>();

            try
            {
                var cityById = await _repository.CityRepository.GetCityById(id);
                if (cityById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"City with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"City with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned City with id: {id}");
                    var result = _mapper.Map<CityDto>(cityById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned City with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetCityById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetCityById API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateCity([FromBody] CityPostDto cityPostDto)
        {
            ServiceResponse<CityPostDto> serviceResponse = new ServiceResponse<CityPostDto>();

            try
            {
                if (cityPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "City object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("City object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid City object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid City object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var city = _mapper.Map<City>(cityPostDto);
                _repository.CityRepository.CreateCity(city);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "City Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetCityById", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateCity API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateCity API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] CityUpdateDto cityUpdateDto)
        {
            ServiceResponse<CityUpdateDto> serviceResponse = new ServiceResponse<CityUpdateDto>();

            try
            {
                if (cityUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update City object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update City object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update City object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update City object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var cityDetail = await _repository.CityRepository.GetCityById(id);
                if (cityDetail is null)
                {
                    _logger.LogError($"Update City with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update City with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(cityUpdateDto, cityDetail);
                string result = await _repository.CityRepository.UpdateCity(cityDetail);
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
                _logger.LogError($"Error Occured in UpdateCity API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateCity API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            ServiceResponse<CityDto> serviceResponse = new ServiceResponse<CityDto>();

            try
            {
                var cityDetail = await _repository.CityRepository.GetCityById(id);
                if (cityDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete BHK object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete BHK with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.CityRepository.DeleteCity(cityDetail);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeleteCity API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteCity API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateCity(int id)
        {
            ServiceResponse<CityDto> serviceResponse = new ServiceResponse<CityDto>();

            try
            {
                var CityDetail = await _repository.CityRepository.GetCityById(id);
                if (CityDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "City object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"City with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                CityDetail.IsActive = true;
                string result = await _repository.CityRepository.UpdateCity(CityDetail);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in ActivateCity API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in ActivateCity API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCity(int id)
        {
            ServiceResponse<CityDto> serviceResponse = new ServiceResponse<CityDto>();

            try
            {
                var cityDetail = await _repository.CityRepository.GetCityById(id);
                if (cityDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BHK object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"BHK with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                cityDetail.IsActive = false;
                string result = await _repository.CityRepository.UpdateCity(cityDetail);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deactivated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeactivateCity API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeactivateCity API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCountryNames()
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
                var countryNames = new SortedSet<string>();
                foreach (CultureInfo culture in cultures)
                {
                    RegionInfo region = new RegionInfo(culture.Name);
                    if (!countryNames.Contains(region.EnglishName))
                    {
                        countryNames.Add(region.EnglishName);
                    }
                }
                var result = countryNames.ToArray();
                _logger.LogInfo("Returned all Countries");
                serviceResponse.Data = result.ToList();
                serviceResponse.Message = "Returned all Countries  Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllCountryNames API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllCountryNames API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCountryCodes()
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                string[] countryPhoneCodes =
                {
                 "+1", "+1", "+7", "+20", "+27", "+30", "+31", "+32", "+33", "+34","+36", "+39", "+40", "+41", "+43", "+44", "+45", "+46", "+47", "+48",
                 "+49", "+51", "+52", "+53", "+54", "+55", "+56", "+57", "+58", "+60","+61", "+62", "+63", "+64", "+65", "+66", "+81", "+82", "+84", "+86",
                 "+90", "+91", "+92", "+93", "+94", "+95", "+98", "+211", "+212", "+213","+216", "+218", "+220", "+221", "+222", "+223", "+224", "+225", "+226",
                 "+227", "+228", "+229", "+230", "+231", "+232", "+233", "+234", "+235","+236", "+237", "+238", "+239", "+240", "+241", "+242", "+243", "+244",
                 "+245", "+246", "+248", "+249", "+250", "+251", "+252", "+253", "+254","+255", "+256", "+257", "+258", "+260", "+261", "+262", "+263", "+264",
                 "+265", "+266", "+267", "+268", "+269", "+290", "+291", "+297", "+298","+299", "+350", "+351", "+352", "+353", "+354", "+355", "+356", "+357",
                 "+358", "+359", "+370", "+371", "+372", "+373", "+374", "+375", "+376","+377", "+378", "+379", "+380", "+381", "+382", "+383", "+385", "+386",
                 "+387", "+389", "+420", "+421", "+423", "+500", "+501", "+502", "+503","+504", "+505", "+506", "+507", "+508", "+509", "+590", "+591", "+592",
                 "+593", "+594", "+595", "+596", "+597", "+598", "+599", "+670", "+672","+673", "+674", "+675", "+676", "+677", "+678", "+679", "+680", "+681",
                 "+682", "+683", "+684", "+685", "+686", "+687", "+688", "+689", "+690","+691", "+692", "+850", "+852", "+853", "+855", "+856", "+880", "+886",
                 "+960", "+961", "+962", "+963", "+964", "+965", "+966", "+967", "+968","+970", "+971", "+972", "+973", "+974", "+975", "+976", "+977", "+992",
                 "+993", "+994", "+995", "+996", "+998"
                };
                var result = countryPhoneCodes;
                _logger.LogInfo("Returned all CountriesCode");
                serviceResponse.Data = result.ToList();
                serviceResponse.Message = "Returned all CountriesCode  Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllCountryCodes API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllCountryCodes API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
