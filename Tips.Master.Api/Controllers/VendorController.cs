using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using AutoMapper;
using Contracts;
using Microsoft.EntityFrameworkCore;
using Entities.Migrations;
using System.Net;
using Newtonsoft.Json;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.AspNetCore.StaticFiles;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IConfiguration _config;
        private IFileUploadRepository _fileUploadRepository;

        public VendorController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper, IConfiguration config, IFileUploadRepository fileUploadRepository)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _config = config;
            _fileUploadRepository = fileUploadRepository;
        }

        // GET: api/<VendorController>
        [HttpGet]
        
            public async Task<IActionResult> GetAllVendorMasters([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
            { 
                ServiceResponse<IEnumerable<VendorMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<VendorMasterDto>>();

            try
            {
                 var getAllVendorMastersList = await _repository.VendorRepository.GetAllVendorMasters(pagingParameter, searchParams);
                var metadata = new
                {
                    getAllVendorMastersList.TotalCount,
                    getAllVendorMastersList.PageSize,
                    getAllVendorMastersList.CurrentPage,
                    getAllVendorMastersList.HasNext,
                    getAllVendorMastersList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all VendorMasters");
                var result = _mapper.Map<IEnumerable<VendorMasterDto>>(getAllVendorMastersList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all VendorMasters Successfully";
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

        // GET api/<VendorController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorMasterById(int id)
        {
            ServiceResponse<VendorMasterDto> serviceResponse = new ServiceResponse<VendorMasterDto>();

            try
            {
                var getvendorMasterById = await _repository.VendorRepository.GetVendorMasterById(id);

                if (getvendorMasterById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"VendorMaster with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Vendor with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned VendorMaster with id: {id}");
                    var result = _mapper.Map<VendorMasterDto>(getvendorMasterById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned VendorMasterById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetVendorMasterById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<VendorController>
        [HttpPost]
        public async Task<IActionResult> CreateVendorMaster([FromBody] VendorMasterPostDto vendorMasterPost)
        {
            ServiceResponse<VendorMasterDto> serviceResponse = new ServiceResponse<VendorMasterDto>();

            try
            {
                string serverKey = GetServerKey();

                if (vendorMasterPost is null)
                {
                    _logger.LogError("VendorMasters object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "VendorMasters object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
               if (!ModelState.IsValid)
               {
                    _logger.LogError("Invalid VendorMasters object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid VendorMasters object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                
                 var address = _mapper.Map<IEnumerable<VendorAddress>>(vendorMasterPost.Addresses);
                var contact = _mapper.Map<IEnumerable<VendorContacts>>(vendorMasterPost.Contacts);
                var banking = _mapper.Map<IEnumerable<VendorBanking>>(vendorMasterPost.VendorBankings);
                var related = _mapper.Map<IEnumerable<VendorRelatedVendor>>(vendorMasterPost.RelatedVendors);
                var headcount = _mapper.Map<IEnumerable<VendorHeadCounting>>(vendorMasterPost.HeadCountings);
                var vendorMaster = _mapper.Map<VendorMaster>(vendorMasterPost);

                if (serverKey == "avision")
                {
                    var vendorNo = vendorMasterPost.VendorId;
                    vendorMaster.VendorId = vendorNo;
                }
                else
                {
                    var vendorNumber = await _repository.VendorRepository.GenerateVendorId();
                    vendorMaster.VendorId = vendorNumber;
                }

                vendorMaster.Addresses = address.ToList();
                vendorMaster.Contacts = contact.ToList();
                vendorMaster.VendorBankings = banking.ToList();
                vendorMaster.RelatedVendors = related.ToList();
                vendorMaster.HeadCountings = headcount.ToList();

                await _repository.VendorRepository.CreateVendorMaster(vendorMaster); 

                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "VendorMaster Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetVendorById",serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateVendorMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateVendorMasterFileUpload([FromBody] List<FileUploadPostDto> fileUploadPostDtos)
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                if (fileUploadPostDtos is null)
                {
                    _logger.LogError("VendorMasterFile object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "VendorMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid VendorMasterFile object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                // var fileUploadDtoList = new List<FileUpload>();

                ////multiple file upload

                List<string>? id_s = new List<string>();
                var FileUploadDetails = fileUploadPostDtos;
                foreach (var FileUploadDetail in FileUploadDetails)
                {
                    Guid guids = Guid.NewGuid();
                    byte[] fileContent = Convert.FromBase64String(FileUploadDetail.FileByte);
                    //var itemNumber = fileUploadPostDtos.ItemNumber;
                    string fileName = guids.ToString() + "_" + FileUploadDetail.FileName + "." + FileUploadDetail.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();

                    //Guid guids = Guid.NewGuid();
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "FileUpload", fileName);
                    using (MemoryStream ms = new MemoryStream(fileContent))
                    {
                        ms.Position = 0;
                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            ms.WriteTo(fileStream);
                        }
                        var uploadedFile = new FileUpload
                        {
                            FileName = fileName,
                            FileExtension = FileExt,
                            FilePath = filePath,
                            ParentId = "Vendor Master",
                            DocumentFrom = "VendorMaster File Document",
                            FileByte = FileUploadDetail.FileByte
                        };
                        _repository.FileUploadRepository.CreateFileUploadDocument(uploadedFile);
                        _repository.SaveAsync();
                        id_s.Add(uploadedFile.Id.ToString());

                    }
                }
                serviceResponse.Data = id_s;
                serviceResponse.Message = " VendorMasterFile Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside VendorMasterFile action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private string GetServerKey()
        {
            var serverName = Environment.MachineName;
            var serverConfiguration = _config.GetSection("ServerConfiguration");

            if (serverConfiguration.GetValue<bool?>("Server1:EnableKeus") == true)
            {
                return "keus";
            }
            else if (serverConfiguration.GetValue<bool?>("Server1:EnableAvision") == true)
            {
                return "avision";

            }
            else
            {
                return "trasccon";
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDownloadUrlDetailsforVendorFiles(string fileids)
        {
            ServiceResponse<List<FileUploadDto>> serviceResponse = new ServiceResponse<List<FileUploadDto>>();
            try
            {
                string serverKey = GetServerKey();
                var vendorFiles = await _fileUploadRepository.GetDownloadUrlDetails(fileids);
                if (vendorFiles == null)
                {
                    _logger.LogError($"DownloadDetail with id: {fileids}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DownloadDetail with id: {fileids}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid VendorMaster UploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid VendorMaster UploadDocument sent from client.");
                    return BadRequest(serviceResponse);
                }
                List<FileUploadDto> fileUploads = new List<FileUploadDto>();
                if (vendorFiles != null)
                {
                    foreach (var fileUploadDetails in vendorFiles)
                    {
                        FileUploadDto fileUploadDto = _mapper.Map<FileUploadDto>(fileUploadDetails);
                        if (serverKey == "avision")
                        {
                            var baseUrl = $"{_config["ItemMasterBaseUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/apigateway/tips/VendorMaster/DownloadFile?Filename={fileUploadDto.FileName}";
                        }
                        else
                        {
                            var baseUrl = $"{_config["ItemMasterBaseUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/api/VendorMaster/DownloadFile?Filename={fileUploadDto.FileName}";
                        }

                        //fileUploadDto.FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "FileUpload", fileUploadDto.FileName);
                        fileUploads.Add(fileUploadDto);
                    }
                }
                _logger.LogInfo($"Returned DownloadDetail with id: {fileids}");
                //var result = _mapper.Map<IEnumerable<GetDownloadUrlDtos>>(getDownloadDetailByPoNumber);
                serviceResponse.Data = fileUploads;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside VendorMasterFiles action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadFile(string Filename)
        {
            ServiceResponse<FileContentResult> serviceResponse = new ServiceResponse<FileContentResult>();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "FileUpload", Filename);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var ContentType))
            {
                ContentType = "application/octet-stream";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(bytes, ContentType, Path.GetFileName(filePath));
        }

        // PUT api/<VendorController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendorMaster(int id, [FromBody] VendorMasterUpdateDto vendorMasterUpdateDto)
        {
            ServiceResponse<VendorMasterUpdateDto> serviceResponse = new ServiceResponse<VendorMasterUpdateDto>();

            try
            {
                if (vendorMasterUpdateDto is null)
                {
                    _logger.LogError("Update VendorMasters object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update VendorMasters object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update VendorMasters object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update VendorMasters object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updateVendorMaster = await _repository.VendorRepository.GetVendorMasterById(id);
                if (updateVendorMaster is null)
                {
                    _logger.LogError($"Update VendorMasters with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update VendorMasters with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

              

                var address = _mapper.Map<IEnumerable<VendorAddress>>(vendorMasterUpdateDto.Addresses);
                 
                var contact = _mapper.Map<IEnumerable<VendorContacts>>(vendorMasterUpdateDto.Contacts);

                var banking = _mapper.Map<IEnumerable<VendorBanking>>(vendorMasterUpdateDto.VendorBankings);
                var related = _mapper.Map<IEnumerable<VendorRelatedVendor>>(vendorMasterUpdateDto.RelatedVendors);

                var headcount = _mapper.Map<IEnumerable<VendorHeadCounting>>(vendorMasterUpdateDto.HeadCountings);

                var vendorMaster = _mapper.Map(vendorMasterUpdateDto, updateVendorMaster);


                vendorMaster.Addresses = address.ToList();
                vendorMaster.Contacts = contact.ToList();
                vendorMaster.VendorBankings = banking.ToList();
                vendorMaster.RelatedVendors = related.ToList();
                vendorMaster.HeadCountings = headcount.ToList();

                string result = await _repository.VendorRepository.UpdateVendorMaster(vendorMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "VendorMaster Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateVendorMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<VendorController>/5
        [HttpDelete("{id}")]
     public async Task<IActionResult> DeleteVendorMaster(int id)
        {
            ServiceResponse<VendorMasterDto> serviceResponse = new ServiceResponse<VendorMasterDto>();

            try
            {
                var deleteVendorMaster = await _repository.VendorRepository.GetVendorMasterById(id);
                if (deleteVendorMaster == null)
                {
                    _logger.LogError($"Delete VendorMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete VendorMaster with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.VendorRepository.DeleteVendorMaster(deleteVendorMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "VendorMaster Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteVendorMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveVendorNameList()
        {
            ServiceResponse<IEnumerable<VendorIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<VendorIdNameListDto>>();
            try
            {
                var getAllActiveVendorNameList = await _repository.VendorRepository.GetAllActiveVendorMasterNameList();
                var result = _mapper.Map<IEnumerable<VendorIdNameListDto>>(getAllActiveVendorNameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All ActiveVendorNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveVendorNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVendorNameList()
        {
            ServiceResponse<IEnumerable<VendorIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<VendorIdNameListDto>>();
            try
            {
                var getAllVendorNameList = await _repository.VendorRepository.GetAllVendorMasterNameList();
                var result = _mapper.Map<IEnumerable<VendorIdNameListDto>>(getAllVendorNameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All VendorNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllVendorNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateVendorMaster(int id)
        {
            ServiceResponse<VendorMasterDto> serviceResponse = new ServiceResponse<VendorMasterDto>();
            try
            {
                var vendorMasters = await _repository.VendorRepository.GetVendorMasterById(id);
                if (vendorMasters is null)
                {
                    _logger.LogError($"vendorMasters with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "vendorMasters object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                vendorMasters.IsActive = true;
                string result = await _repository.VendorRepository.UpdateVendorMaster(vendorMasters);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Activate Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateVendorMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateVendorMaster(int id)
        {
            ServiceResponse<VendorMasterDto> serviceResponse = new ServiceResponse<VendorMasterDto>();
            try
            {
                var vendorMasters = await _repository.VendorRepository.GetVendorMasterById(id);
                if (vendorMasters is null)
                {
                    _logger.LogError($"vendorMasters with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "vendorMasters object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                vendorMasters.IsActive = false;
                string result = await _repository.VendorRepository.UpdateVendorMaster(vendorMasters);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Deactivate Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateVendorMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
