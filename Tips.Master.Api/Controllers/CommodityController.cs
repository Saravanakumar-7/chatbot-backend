using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommodityController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public CommodityController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<CommodityController>
        [HttpGet]
        public async Task<IActionResult> GetAllCommodity()
        {
            try
            {
                var CommodityList = await _repository.CommodityRepository.GetAllCommodity();
                _logger.LogInfo("Returned all Commodity");
                var result = _mapper.Map<IEnumerable<CommodityDto>>(CommodityList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<CommodityController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommodityById(int id)
        {
            try
            {
                var Commodity = await _repository.CommodityRepository.GetCommodityById(id);
                if (Commodity == null)
                {
                    _logger.LogError($"Commodity with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<CommodityDto>(Commodity);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCommodityById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<CommodityController>
        [HttpPost]
        public IActionResult CreateCommodity([FromBody] CommodityDtoPost commodityDtoPost)
        {
            try
            {
                if (commodityDtoPost is null)
                {
                    _logger.LogError("Commodity object sent from client is null.");
                    return BadRequest("Commodity object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Commodity object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var CommodityEntity = _mapper.Map<Commodity>(commodityDtoPost);
                _repository.CommodityRepository.CreateCommodity(CommodityEntity);
                _repository.SaveAsync();

                return Created("GetCommodityById", "Successfully Created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<CommodityController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCommodity(int id, [FromBody] CommodityDtoUpdate commodityDtoUpdate)
        {
            try
            {
                if (commodityDtoUpdate is null)
                {
                    _logger.LogError("Commodity object sent from client is null.");
                    return BadRequest("Commodity object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Commodity object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var CommodityEntity = await _repository.CommodityRepository.GetCommodityById(id);
                if (CommodityEntity is null)
                {
                    _logger.LogError($"Commodity with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(commodityDtoUpdate, CommodityEntity);
                string result = await _repository.CommodityRepository.UpdateCommodity(CommodityEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateCommodity action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<CommodityController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommodity(int id)
        {
            try
            {
                var Commodity = await _repository.CommodityRepository.GetCommodityById(id);
                if (Commodity == null)
                {
                    _logger.LogError($"Commodity with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.CommodityRepository.DeleteCommodity(Commodity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateCommodity(int id)
        {
            try
            {
                var Commodity = await _repository.CommodityRepository.GetCommodityById(id);
                if (Commodity is null)
                {
                    _logger.LogError($"Commodity with id: {id}, hasn't been found in db.");
                    return BadRequest("Commodity object is null");
                }
                Commodity.ActiveStatus = true;
                string result = await _repository.CommodityRepository.UpdateCommodity(Commodity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateCommodity action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCommodity(int id)
        {
            try
            {
                var Commodity = await _repository.CommodityRepository.GetCommodityById(id);
                if (Commodity is null)
                {
                    _logger.LogError($"Commodity with id: {id}, hasn't been found in db.");
                    return BadRequest("Commodity object is null");
                }
                Commodity.ActiveStatus = false;
                string result = await _repository.CommodityRepository.UpdateCommodity(Commodity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateCommodity action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
