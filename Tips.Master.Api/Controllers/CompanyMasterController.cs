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
using Microsoft.AspNetCore.StaticFiles;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CompanyMasterController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IConfiguration _config;
        private IFileUploadRepository _fileUploadRepository;

        public CompanyMasterController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper, IConfiguration config, IFileUploadRepository fileUploadRepository)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _config = config;
            _fileUploadRepository = fileUploadRepository;
        }

        // GET: api/<CompanyMasterController>
        [HttpGet]
        public async Task<IActionResult> GetAllCompanyMaster([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CompanyMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<CompanyMasterDto>>();
            try
            {
                var getallCompanyMasters = await _repository.CompanyMasterRepository.GetAllCompanyMasters(pagingParameter, searchParams);
                var metadata = new
                {
                    getallCompanyMasters.TotalCount,
                    getallCompanyMasters.PageSize,
                    getallCompanyMasters.CurrentPage,
                    getallCompanyMasters.HasNext,
                    getallCompanyMasters.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all CompanyMasters");
                var result = _mapper.Map<IEnumerable<CompanyMasterDto>>(getallCompanyMasters);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all CompanyMasters Successfully";
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

        // GET api/<CompanyMasterController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyMasterById(int id)
        {
            ServiceResponse<CompanyMasterDto> serviceResponse = new ServiceResponse<CompanyMasterDto>();
            try
            {
                var getCompanyMasterbyId = await _repository.CompanyMasterRepository.GetCompanyMasterById(id);

                if (getCompanyMasterbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CompanyMaster with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CompanyMaster with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned CompanyMaster with id: {id}");
                    var result = _mapper.Map<CompanyMasterDto>(getCompanyMasterbyId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned CompanyMasterById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCompanyMasterById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<CompanyMasterController>
        [HttpPost]
        public async Task<IActionResult> CreateCompanyMaster([FromBody] CompanyMasterDtoPost companyMasterDtoPost)
        {
            ServiceResponse<CompanyMasterDtoPost> serviceResponse = new ServiceResponse<CompanyMasterDtoPost>();
            try
            {
                if (companyMasterDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CompanyMaster object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("CompanyMaster object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid CompanyMaster object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid CompanyMaster object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var CompanyMaster = _mapper.Map<CompanyMaster>(companyMasterDtoPost);
                var Contacts = _mapper.Map<IEnumerable<CompanyContacts>>(companyMasterDtoPost.CompanyContacts);
                var Bankings = _mapper.Map<IEnumerable<CompanyBanking>>(companyMasterDtoPost.CompanyBankings);
                var Addresses = _mapper.Map<IEnumerable<CompanyAddresses>>(companyMasterDtoPost.CompanyAddresses);
                var Approval = _mapper.Map<IEnumerable<CompanyApproval>>(companyMasterDtoPost.CompanyApprovals);
                var CompanymasterHeadCount = _mapper.Map<IEnumerable<CompanyMasterHeadCounting>>(companyMasterDtoPost.CompanyMasterHeadCountings);

                // Multi-file upload for each CompanyApproval
                //var companyfileuploadpostdto = companyMasterDtoPost.CompanyApprovals;
                //var CompId = CompanyMaster.CompanyId;
                //if (companyfileuploadpostdto != null)
                //{
                //    for (int i = 0; i < companyfileuploadpostdto.Count; i++)
                //    {
                //        if (companyfileuploadpostdto[i].Upload != null && companyfileuploadpostdto[i].Upload.Count > 0)
                //        {
                //            var companyFileUploadDtoList = new List<CompanyFileUpload>();
                //            CoCDocumentSave(companyfileuploadpostdto, CompanyMaster, CompId.ToString(), i, companyFileUploadDtoList);
                //        }
                //    }
                //}

                await _repository.CompanyMasterRepository.CreateCompanyMaster(CompanyMaster);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CompanyMaster Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.Created;
                return Created("GetCompanyMasterById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateCompanyMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //private void CoCDocumentSave(List<CompanyApprovalPostDto> companyApprovalPostDto, CompanyMaster companyMaster, string CompId, int i, List<CompanyFileUpload> companyFileUploadDtoList)
        //{
        //    var companyfiles = companyApprovalPostDto[i].Upload;
        //    foreach (var companyfile in companyfiles)
        //    {
        //        var fileContent = companyfile.FileByte;
        //        string fileName = companyfile.FileName + "." + companyfile.FileExtension;
        //        string FileExt = Path.GetExtension(fileName).ToUpper();
        //        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "CompanyFileUpload",/* guid.ToString() + "_" +*/ fileName);
        //        using (MemoryStream ms = new MemoryStream(fileContent))
        //        {
        //            ms.Position = 0;
        //            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //            {
        //                ms.WriteTo(fileStream);
        //            }
        //            var uploadedFile = new CompanyFileUpload
        //            {
        //                FileName = fileName,
        //                FileExtension = FileExt,
        //                FilePath = filePath,
        //                ParentId = CompId,
        //                DocumentFrom = "CompanyApprovalFiles",
        //            };
        //            _repository.CompanyFileUploadRepository.CreateCompanyFileUpload(uploadedFile);
        //            _repository.SaveAsync();
        //            if (uploadedFile != null)
        //            {
        //                CompanyFileUpload CompanyFileDetails = _mapper.Map<CompanyFileUpload>(uploadedFile);
        //                companyFileUploadDtoList.Add(CompanyFileDetails);
        //            }
        //        }
        //        companyMaster.CompanyApprovals[i].Upload = companyFileUploadDtoList;
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> CreateCompanyMasterFileUpload([FromBody] List<FileUploadPostDto> fileUploadPostDtos)
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                if (fileUploadPostDtos is null)
                {
                    _logger.LogError("CompanyMasterFile object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CompanyMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid CompanyMasterFile object sent from client.");
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
                            ParentId = "Company Master",
                            DocumentFrom = "CompanyMaster File Document",
                            FileByte = FileUploadDetail.FileByte
                        };
                        _repository.FileUploadRepository.CreateFileUploadDocument(uploadedFile);
                        _repository.SaveAsync();
                        id_s.Add(uploadedFile.Id.ToString());

                    }
                }
                serviceResponse.Data = id_s;
                serviceResponse.Message = " CompanyMasterFile Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CompanyMasterFile action: {ex.Message},{ex.InnerException}");
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
        public async Task<IActionResult> GetDownloadUrlDetailsforCompanyFiles(string fileids)
        {
            ServiceResponse<List<FileUploadDto>> serviceResponse = new ServiceResponse<List<FileUploadDto>>();
            try
            {
                string serverKey = GetServerKey();
                var comapanyFiles = await _fileUploadRepository.GetDownloadUrlDetails(fileids);
                if (comapanyFiles == null)
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
                    serviceResponse.Message = "Invalid Companymaster UploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Companymaster UploadDocument sent from client.");
                    return BadRequest(serviceResponse);
                }
                List<FileUploadDto> fileUploads = new List<FileUploadDto>();
                if (comapanyFiles != null)
                {
                    foreach (var fileUploadDetails in comapanyFiles)
                    {
                        FileUploadDto fileUploadDto = _mapper.Map<FileUploadDto>(fileUploadDetails);
                        if (serverKey == "avision")
                        {
                            var baseUrl = $"{_config["ItemMasterBaseUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/apigateway/tips/CompanyMaster/DownloadFile?Filename={fileUploadDto.FileName}";
                        }
                        else
                        {
                            var baseUrl = $"{_config["ItemMasterBaseUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/api/CompanyMaster/DownloadFile?Filename={fileUploadDto.FileName}";
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
                _logger.LogError($"Something went wrong inside CompanymasterFiles action: {ex.Message}");
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

        // PUT api/<CompanyMasterController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompanyMaster(int id, [FromBody] CompanyMasterDtoUpdate companyMasterDtoUpdate)
        {
            ServiceResponse<CompanyMasterDtoUpdate> serviceResponse = new ServiceResponse<CompanyMasterDtoUpdate>();
            try
            {
                if (companyMasterDtoUpdate is null)
                {
                    _logger.LogError("Update CompanyMaster object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update CompanyMaster object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update CompanyMaster object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update CompanyMaster object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updateCompanyMaster = await _repository.CompanyMasterRepository.GetCompanyMasterById(id);
                if (updateCompanyMaster is null)
                {
                    _logger.LogError($"Update CompanyMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update CompanyMaster with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound; 
                    return NotFound(serviceResponse);
                }


                var Addresses = _mapper.Map<IEnumerable<CompanyAddresses>>(companyMasterDtoUpdate.CompanyAddresses);
                var Contacts = _mapper.Map<IEnumerable<CompanyContacts>>(companyMasterDtoUpdate.CompanyContacts);
                var Bankings = _mapper.Map<IEnumerable<CompanyBanking>>(companyMasterDtoUpdate.CompanyBankings);
                var CompanymasterHeadCounting = _mapper.Map<IEnumerable<CompanyMasterHeadCounting>>(companyMasterDtoUpdate.CompanyMasterHeadCountings);
                var Approval = _mapper.Map<IEnumerable<CompanyApproval>>(companyMasterDtoUpdate.CompanyApprovals); 

                var companyMaster = _mapper.Map(companyMasterDtoUpdate, updateCompanyMaster);


                companyMaster.CompanyAddresses = Addresses.ToList();
                companyMaster.CompanyContacts = Contacts.ToList();
                companyMaster.CompanyBankings = Bankings.ToList();
                companyMaster.CompanyMasterHeadCountings = CompanymasterHeadCounting.ToList();
                companyMaster.CompanyApprovals = Approval.ToList();

                string result = await _repository.CompanyMasterRepository.UpdateCompanyMaster(companyMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CompanyMaster Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateCompanyMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<CompanyMasterController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyMaster(int id)
        {
            ServiceResponse<CompanyMasterDto> serviceResponse = new ServiceResponse<CompanyMasterDto>();
            try
            {
                var deleteCompanyMaster = await _repository.CompanyMasterRepository.GetCompanyMasterById(id);
                if (deleteCompanyMaster == null)
                {
                    _logger.LogError($"Delete Company with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Company with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.CompanyMasterRepository.DeleteCompanyMaster(deleteCompanyMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " CompanyMaster Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteCompanyMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveCompanyIdNameList()
        {
            ServiceResponse<IEnumerable<CompanyIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<CompanyIdNameListDto>>();
            try
            {
                var getAllActiveCompanymasterIdNameList = await _repository.CompanyMasterRepository.GetAllActiveCompanyMasterIdNameList();
                var result = _mapper.Map<IEnumerable<CompanyIdNameListDto>>(getAllActiveCompanymasterIdNameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All ActiveCompanyIdNameList Successfully";
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

        [HttpGet]
        public async Task<IActionResult> GetAllCompanyIdNameList()
        {
            ServiceResponse<IEnumerable<CompanyIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<CompanyIdNameListDto>>();
            try
            {
                var getAllCompanymasterIdNameList = await _repository.CompanyMasterRepository.GetAllCompanyMasterIdNameList();
                var result = _mapper.Map<IEnumerable<CompanyIdNameListDto>>(getAllCompanymasterIdNameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All CompanyIdNameList Successfully";
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

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateCompanyMaster(int id)
        {
            ServiceResponse<CompanyMasterDto> serviceResponse = new ServiceResponse<CompanyMasterDto>();
            try
            {
                var companyMaster = await _repository.CompanyMasterRepository.GetCompanyMasterById(id);
                if (companyMaster is null)
                {
                    _logger.LogError($"companyMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "companyMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                companyMaster.IsActive = true;
                string result = await _repository.CompanyMasterRepository.UpdateCompanyMaster(companyMaster);
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
                _logger.LogError($"Something went wrong inside ActivateCompanyMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCompanyMaster(int id)
        {
            ServiceResponse<CompanyMasterDto> serviceResponse = new ServiceResponse<CompanyMasterDto>();
            try
            {
                var companyMaster = await _repository.CompanyMasterRepository.GetCompanyMasterById(id);
                if (companyMaster is null)
                {
                    _logger.LogError($"companyMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "companyMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                companyMaster.IsActive = false;
                string result = await _repository.CompanyMasterRepository.UpdateCompanyMaster(companyMaster);
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
                _logger.LogError($"Something went wrong inside DeactivateCompanyMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
