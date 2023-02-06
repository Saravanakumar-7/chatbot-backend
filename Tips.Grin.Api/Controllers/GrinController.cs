using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Entities;  
using AutoMapper;
using Tips.Grin.Api.Contracts;
using Contracts;
using Microsoft.EntityFrameworkCore;
using System.Net; 
using Entities;
using Newtonsoft.Json;
using Entities.DTOs;
using Tips.Grin.Api.Repository;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text;
using System.Dynamic;
using Azure.Core;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GrinController : ControllerBase
    {
        private IGrinRepository _repository;
        private IGrinPartsRepository _grinPartsRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IDocumentUploadRepository _documentUploadRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public GrinController(IGrinRepository repository, IDocumentUploadRepository documentUploadRepository, IGrinPartsRepository grinPartsRepository,
            IWebHostEnvironment webHostEnvironment, ILoggerManager logger, IMapper mapper, HttpClient httpClient,IConfiguration config)
        {
            _repository = repository;
            _grinPartsRepository = grinPartsRepository;
            _logger = logger;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _documentUploadRepository = documentUploadRepository;
            _httpClient = httpClient;
            _config = config;
        }
        // GET: api/<GrinController>
        [HttpGet]

        public async Task<IActionResult> GetAllGrin([FromQuery] PagingParameter pagingParameter,[FromQuery] SearchParams searchParams)

        {
            ServiceResponse<IEnumerable<GrinDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinDto>>();

            try
            {
                var GetallGrins = await _repository.GetAllGrin(pagingParameter,searchParams);

                var metadata = new
                {
                    GetallGrins.TotalCount,
                    GetallGrins.PageSize,
                    GetallGrins.CurrentPage,
                    GetallGrins.HasNext,
                    GetallGrins.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all Grins");
                var result = _mapper.Map<IEnumerable<GrinDto>>(GetallGrins);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Grins Successfully";
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

        // GET api/<GrinController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGrinById(int id)
        {
            ServiceResponse<GrinDto> serviceResponse = new ServiceResponse<GrinDto>();

            try
            {
                var GrinDetailsbyId = await _repository.GetGrinById(id);

                if (GrinDetailsbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Grin with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Grin with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else

                {
                    _logger.LogInfo($"Returned GrinDetailsById with id: {id}");
                    GrinDto grinDto = _mapper.Map<GrinDto>(GrinDetailsbyId);

                    
                    List<GrinPartsDto> grinPartsDtos = new List<GrinPartsDto>();

                    foreach (var GrinpartsDetails in GrinDetailsbyId.GrinParts)
                    {
                        GrinPartsDto grinPartsDto = _mapper.Map<GrinPartsDto>(GrinpartsDetails);
                        grinPartsDto.ProjectNumbers = _mapper.Map<List<ProjectNumbersDto>>(GrinpartsDetails.ProjectNumbers);
                        grinPartsDtos.Add(grinPartsDto);
                    }

                    grinDto.GrinParts = grinPartsDtos;
                    serviceResponse.Data = grinDto;
                    serviceResponse.Message = $"Returned Grin with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetGrinById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


   
        [HttpPost]
        public async Task<IActionResult> CreateGrin([FromBody] GrinPostDto grinPostDto)
        {
            ServiceResponse<GrinDto> serviceResponse = new ServiceResponse<GrinDto>();

            try
            {
                if (grinPostDto is null)
                {
                    _logger.LogError("Grin object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Grin object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Grin object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Grin object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var grins = _mapper.Map<Grins>(grinPostDto);
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));

                var newcount = await _repository.GetGrinNumberAutoIncrementCount(date);

                if (newcount > 0)
                {
                    var number = newcount + 1;
                    string e = String.Format("{0:D4}", number);
                    grins.GrinNumber = days + months + years + "G" + (e);
                }
                else
                {
                    var count = 1;
                    var e = count.ToString("D4");
                    grins.GrinNumber = days + months + years + "G" + (e);
                }

                var grinPartsDto = grinPostDto.GrinParts;

                var grinPartsList = new List<GrinParts>();
                if (grinPartsDto != null)
                {
                    for (int i = 0; i < grinPartsDto.Count; i++)
                    {
                        GrinParts grinParts = _mapper.Map<GrinParts>(grinPartsDto[i]);
                        grinParts.ProjectNumbers = _mapper.Map<List<ProjectNumbers>>(grinPartsDto[i].ProjectNumbers);
                        grinPartsList.Add(grinParts);

         
                    }
                }

              
                
                grins.GrinParts = grinPartsList;

                await _repository.CreateGrin(grins);

                //// grin upload

                //var grinUploadDetails = grinPostDto.GrinDocuments;
                //foreach (var grinUploadDetail in grinUploadDetails)
                //{
                //    var fileContent = grinUploadDetail.FileByte;
                //    var grinNumber = grins.GrinNumber;
                //    string fileName = grinUploadDetail.FileName + "." + grinUploadDetail.FileExtension;
                //    string FileExt = Path.GetExtension(fileName).ToUpper();

                //    Guid guid = Guid.NewGuid();
                //    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "GrinDocument", guid.ToString() + "_" + fileName);
                //     using (MemoryStream ms = new MemoryStream(fileContent))
                //    {
                //        ms.Position = 0;
                //        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                //        {
                //            ms.WriteTo(fileStream);
                //        }
                //        var uploadedFile = new DocumentUpload
                //        {
                //            FileName = fileName,
                //            FileExtension = FileExt,
                //            FilePath = filePath,
                //            ParentId = grinNumber,
                //            DocumentFrom = "GrinDocument",
                //        };

                //        _documentUploadRepository.CreateUploadDocumentGrin(uploadedFile);
                //        _documentUploadRepository.SaveAsync();

                //    } 

                //}
                ////parts coc upload

                //var grinPartsDetails = grinPostDto.GrinParts;
                //foreach (var grinCoCUpload in grinPartsDetails)
                //{

                //    var cocUploadDocs = grinCoCUpload.COCUpload;

                //    foreach (var cocUpload in cocUploadDocs)
                //    {
                //        var fileContent = cocUpload.FileByte;
                //        var grinNumber = grins.GrinNumber;
                //        string fileName = cocUpload.FileName + "." + cocUpload.FileExtension;
                //        string FileExt = Path.GetExtension(fileName).ToUpper();

                //        Guid guid = Guid.NewGuid();
                //        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "GrinCoCUpload", guid.ToString() + "_" + fileName);
                //         using (MemoryStream ms = new MemoryStream(fileContent))
                //        {
                //            ms.Position = 0;
                //            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                //            {
                //                ms.WriteTo(fileStream);
                //            }
                //            var uploadedFile = new DocumentUpload
                //            {
                //                FileName = fileName,
                //                FileExtension = FileExt,
                //                FilePath = filePath,
                //                ParentId = grinNumber,
                //                DocumentFrom = "GrinCoCDocument",
                //            };

                //            _documentUploadRepository.CreateUploadDocumentGrin(uploadedFile);
                //            _documentUploadRepository.SaveAsync();

                //        }

                //    }

                //}

                //end cocupload
                //

                _repository.SaveAsync();

                foreach (var parts in grinPartsList)
                {
                    foreach (var project in parts.ProjectNumbers)
                    {
                        dynamic inventoryObject = new ExpandoObject();
                        inventoryObject.PartNumber = parts.ItemNumber;
                        inventoryObject.MftrPartNumber = parts.MftrItemNumber;
                        inventoryObject.Description = parts.ItemDescription;
                        inventoryObject.ProjectNumber = project.ProjectNumber;
                        inventoryObject.Balance_Quantity = project.ProjectQty;
                        inventoryObject.UOM = parts.UOM;
                        inventoryObject.IsStockAvailable = true;
                        inventoryObject.Warehouse = "GRIN";
                        inventoryObject.Location = "GRIN";
                        inventoryObject.GrinNo = parts.Grins.GrinNumber;
                        inventoryObject.GrinPartId = parts.Id;
                        inventoryObject.PartType = "PurchasePart";
                        inventoryObject.ReferenceID = Convert.ToString(parts.Id);
                        inventoryObject.ReferenceIDFrom = "GRIN";

                        var json = JsonConvert.SerializeObject(inventoryObject);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"] ,"CreateInventory"), data);
                    }
                    
                }

                serviceResponse.Data = null;
                serviceResponse.Message = "Grin Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetGrinById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrin(int id, [FromBody] GrinUpdateDto grinDto)
        {
            ServiceResponse<GrinDto> serviceResponse = new ServiceResponse<GrinDto>();

            try
            {
                if (grinDto is null)
                {
                    _logger.LogError("Update Grin object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Grin object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update Grin object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Grin object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updategrin = await _repository.GetGrinById(id);
                if (updategrin is null)
                {
                    _logger.LogError($"Update Grin with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Grin with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
               

                var grinparts = _mapper.Map<IEnumerable<GrinParts>>(grinDto.GrinParts);

                var grinList = _mapper.Map<Grins>(grinDto);

                var grinPartsDto = grinDto.GrinParts;

                var GrinpartsList = new List<GrinParts>();
                for (int i = 0; i < grinPartsDto.Count; i++)
                {
                    GrinParts grinPartsDetail = _mapper.Map<GrinParts>(grinPartsDto[i]);
                    grinPartsDetail.ProjectNumbers = _mapper.Map<List<ProjectNumbers>>(grinPartsDto[i].ProjectNumbers);

                    GrinpartsList.Add(grinPartsDetail);


                }



                var data = _mapper.Map(grinDto, updategrin);


                data.GrinParts = grinparts.ToList();

                string result = await _repository.UpdateGrin(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Grin Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<GrinController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrin(int id)
        {
            ServiceResponse<GrinDto> serviceResponse = new ServiceResponse<GrinDto>();

            try
            {
                var Deletegrin = await _repository.GetGrinById(id);
                if (Deletegrin == null)
                {
                    _logger.LogError($"Delete grin with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete grin with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteGrin(Deletegrin);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Grin Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllActiveGrinNoList()
        {
            ServiceResponse<IEnumerable<GrinNumberListDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinNumberListDto>>();
            try
            {
                var AllActiveGrinNo = await _repository.GetAllActiveGrinNoList();
                var result = _mapper.Map<IEnumerable<GrinNumberListDto>>(AllActiveGrinNo);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GrinNoList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveGrinNoList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]

        public async Task<IActionResult> GetAllGrinParts([FromQuery] PagingParameter pagingParameter,[FromQuery] SearchParams searchParams)

        {
            ServiceResponse<IEnumerable<GrinPartsDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinPartsDto>>();

            try
            {
                var GetallGrinsParts = await _grinPartsRepository.GetAllGrinParts(pagingParameter,searchParams);

                var metadata = new
                {
                    GetallGrinsParts.TotalCount,
                    GetallGrinsParts.PageSize,
                    GetallGrinsParts.CurrentPage,
                    GetallGrinsParts.HasNext,
                    GetallGrinsParts.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all GrinsParts");
                var result = _mapper.Map<IEnumerable<GrinPartsDto>>(GetallGrinsParts);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GrinsParts Successfully";
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

    }
}
