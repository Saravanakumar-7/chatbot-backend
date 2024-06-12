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
using Microsoft.AspNetCore.Authorization;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CustomerMasterController : ControllerBase
    {
            private IRepositoryWrapperForMaster _repository;
            private ILoggerManager _logger;
            private IMapper _mapper;
        private readonly IConfiguration _config;
        private IFileUploadRepository _fileUploadRepository;
        public CustomerMasterController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper, IConfiguration config,IFileUploadRepository fileUploadRepository)
            {
                _repository = repository;
                _logger = logger;
                _mapper = mapper;
            _config = config;
            _fileUploadRepository = fileUploadRepository;
        }
 
        [HttpGet]
        public async Task<IActionResult> GetAllCustomerMaster([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CustomerMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<CustomerMasterDto>>();
            try
            {
                var getAllCustomerMastersList = await _repository.CustomerMasterRepository.GetAllCustomerMasters(pagingParameter, searchParams);
                var metadata = new
                {
                    getAllCustomerMastersList.TotalCount,
                    getAllCustomerMastersList.PageSize,
                    getAllCustomerMastersList.CurrentPage,
                    getAllCustomerMastersList.HasNext,
                    getAllCustomerMastersList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var result = _mapper.Map<IEnumerable<CustomerMasterDto>>(getAllCustomerMastersList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all CustomerMasters Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllCustomerMasters action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerMasterById(int id)
        {
            ServiceResponse<CustomerMasterDto> serviceResponse = new ServiceResponse<CustomerMasterDto>();
            try
            {
                var getCustomerMasterById = await _repository.CustomerMasterRepository.GetCustomerMasterById(id);

                if (getCustomerMasterById == null)
                {
                    serviceResponse.Data= null;
                    serviceResponse.Message = $"CustomerMaster with id hasn't been found in db.";
                    serviceResponse.Success=false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CustomerMaster with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned CustomerMaster with id: {id}");
                    var result = _mapper.Map<CustomerMasterDto>(getCustomerMasterById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned CustomerMasterById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCustomerMasterById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCustomerMasterById action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerMasterByCustomerNo(string customerNumber)
        {
            ServiceResponse<CustomerMasterDto> serviceResponse = new ServiceResponse<CustomerMasterDto>();
            try
            {
                var customerMasterDetails = await _repository.CustomerMasterRepository.GetCustomerMasterByCustomerNo(customerNumber);

                if (customerMasterDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CustomerMaster with CustomerNumber hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CustomerMaster with CustomerNumber: {customerNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned CustomerMaster with CustomerNumber: {customerNumber}");
                    var result = _mapper.Map<CustomerMasterDto>(customerMasterDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned CustomerMasterByCustomerNo Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCustomerMasterByCustomerNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCustomerMasterByCustomerNo action";
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
        [HttpPost]
        public async Task<IActionResult> CreateCustomerMaster([FromBody] CustomerMasterDtoPost customerMasterDtoPost)
        {
            ServiceResponse<CustomerMasterDto> serviceResponse = new ServiceResponse<CustomerMasterDto>();
            //CustomerMaster customerDetail = null;
            string serverKey = GetServerKey();
            try
            {
                if (customerMasterDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CustomerMaster object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("CustomerMaster object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid CustomerMaster object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid CustomerMaster object sent from client.");
                    return BadRequest("Invalid model object");
                }

                
                var contacts = _mapper.Map<IEnumerable<CustomerContacts>>(customerMasterDtoPost.CustomerContacts);
                var shippingAddresses = _mapper.Map<IEnumerable<CustomerShippingAddresses>>(customerMasterDtoPost.CustomerShippingAddresses);
                var addresses = _mapper.Map<IEnumerable<CustomerAddresses>>(customerMasterDtoPost.CustomerAddress);
                var related= _mapper.Map<IEnumerable<CustomerRelatedCustomer>>(customerMasterDtoPost.RelatedCustomers);
                var banking = _mapper.Map<IEnumerable<CustomerBanking>>(customerMasterDtoPost.CustomerBankings);
                var headcount = _mapper.Map<IEnumerable<CustomerMasterHeadCounting>>(customerMasterDtoPost.CustomerMasterHeadCountings);

                var customerMaster = _mapper.Map<CustomerMaster>(customerMasterDtoPost);

                customerMaster.CustomerAddresses = addresses.ToList();
                customerMaster.CustomerContacts = contacts.ToList();
                customerMaster.CustomerShippingAddresses = shippingAddresses.ToList();
                customerMaster.RelatedCustomers= related.ToList();
                customerMaster.CustomerBanking = banking.ToList();
                customerMaster.CustomerMasterHeadCountings= headcount.ToList();
               
                if (serverKey == "trasccon")
                {
                    var customerNumber = await _repository.CustomerMasterRepository.GenerateCustomerNumberAvision();
                    customerMaster.CustomerNumber = customerNumber;
                }
                else if (serverKey == "avision")
                {
                    var customerNumber = await _repository.CustomerMasterRepository.GenerateCustomerNumberAvision();
                    customerMaster.CustomerNumber = customerNumber;
                }
                //if (serverKey != "keus")
                //{
                else 
                {
                    //var customerDetails = await _repository.CustomerMasterRepository.GetCSNumberAutoIncrementCount();
                    // var newcount = customerDetails?.Id;
                    // if (newcount > 0)
                    // {
                    //     var number = newcount + 1;
                    //     string e = String.Format("{0:D4}", number);
                    //     customerMaster.CustomerNumber = "CS" + (e);
                    // }
                    // else
                    // {
                    //     var count = 1;
                    //     var e = count.ToString("D4");
                    //     customerMaster.CustomerNumber = "CS" + (e);
                    // }
                    var customerex = await _repository.CustomerMasterRepository.GetCustomerbyCustomerNumber(customerMaster.CustomerNumber);

                    if (customerex==1)
                    {
                        serviceResponse.Data = null;
                        serviceResponse.Message = "CustomerNumber Already Exists";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotAcceptable;
                        return StatusCode(406, serviceResponse);
                    }                   

                }
                await _repository.CustomerMasterRepository.CreateCustomerMaster(customerMaster);


                _repository.SaveAsync();

                var customerMasterDetails = await _repository.CustomerMasterRepository.GetCSNumberAutoIncrementCount();
                var customerData = _mapper.Map<CustomerMasterDto>(customerMasterDetails);

                serviceResponse.Data = customerData;
                serviceResponse.Message = "CustomerMaster Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.Created;
                return Created("GetCustomerMaster", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateCustomerMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomerMasterOtherUploads([FromBody] CustomerOtherUploadsPostDto customerOtherUploadsPostDto)
        {
            ServiceResponse<CustomerOtherUploadsDto> serviceResponse = new ServiceResponse<CustomerOtherUploadsDto>();
            try
            {
                if (customerOtherUploadsPostDto is null)
                {
                    _logger.LogError("CustomerMasterOtherUploads object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CustomerMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid CustomerMasterOtherUploads object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var otherUploads = _mapper.Map<CustomerOtherUploads>(customerOtherUploadsPostDto);
                await _repository.CustomerMasterOtherUploads.CreateCustomerOtherUploads(otherUploads);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CustomerOtherUploads Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CustomerMasterOtherUploads action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCustomerMasterOtherUploads([FromBody] CustomerOtherUploadsUpdateDto customerOtherUploadsUpdateDto)
        {
            ServiceResponse<CompanyOtherUploadsDto> serviceResponse = new ServiceResponse<CompanyOtherUploadsDto>();
            try
            {
                if (customerOtherUploadsUpdateDto is null)
                {
                    _logger.LogError("CustomerMasterOtherUploads object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CustomerMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid CustomerMasterOtherUploads object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var otherUploads = await _repository.CustomerMasterOtherUploads.GetCustomerMasterOtherUploadsbyCustomerId(customerOtherUploadsUpdateDto.CustomerId);
                var customerOtherUploads = _mapper.Map(customerOtherUploadsUpdateDto, otherUploads);
                var result = await _repository.CustomerMasterOtherUploads.UpdateCustomerOtherUploads(customerOtherUploads);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CustomerMaster Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CustomerMasterOtherUploads action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomerMasterOtherUploadsbyCustomerId(int CustomerId)
        {
            ServiceResponse<CustomerOtherUploadsDto> serviceResponse = new ServiceResponse<CustomerOtherUploadsDto>();
            try
            {
                var otherUploads = await _repository.CustomerMasterOtherUploads.GetCustomerMasterOtherUploadsbyCustomerId(CustomerId);
                if (otherUploads == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CustomerMasterOtherUploads with Customerid hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CustomerMasterOtherUploads with id: {CustomerId}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned CustomerMasterOtherUploads with Companyid: {CustomerId}");
                    var result = _mapper.Map<CustomerOtherUploadsDto>(otherUploads);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned CustomerMasterOtherUploads Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

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
        [HttpPost]
        public async Task<IActionResult> CreateCustomerMasterFileUpload([FromBody] List<FileUploadPostDto> fileUploadPostDtos)
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                if (fileUploadPostDtos is null)
                {
                    _logger.LogError("CustomerMasterFile object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CustomerMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid CustomerMasterFile object sent from client.");
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
                            ParentId = "Customer Master",
                            DocumentFrom = "CustomerMaster File Document",
                            FileByte = FileUploadDetail.FileByte
                        };
                        _repository.FileUploadRepository.CreateFileUploadDocument(uploadedFile);
                        _repository.SaveAsync();
                        id_s.Add(uploadedFile.Id.ToString());

                    }
                }
                serviceResponse.Data = id_s;
                serviceResponse.Message = " CustomerMasterFile Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CustomerMasterFile action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDownloadUrlDetailsforCustomerFiles(string fileids)
        {
            ServiceResponse<List<FileUploadDto>> serviceResponse = new ServiceResponse<List<FileUploadDto>>();
            try
            {
                string serverKey = GetServerKey();
                var customerFiles = await _fileUploadRepository.GetDownloadUrlDetails(fileids);
                if (customerFiles == null)
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
                    serviceResponse.Message = "Invalid Customermaster UploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Customermaster UploadDocument sent from client.");
                    return BadRequest(serviceResponse);
                }
                List<FileUploadDto> fileUploads = new List<FileUploadDto>();
                if (customerFiles != null)
                {
                    foreach (var fileUploadDetails in customerFiles)
                    {
                        FileUploadDto fileUploadDto = _mapper.Map<FileUploadDto>(fileUploadDetails);
                        if (serverKey == "avision")
                        {
                            var baseUrl = $"{_config["ItemMasterBaseUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/apigateway/tips/CustomerMaster/DownloadFile?Filename={fileUploadDto.FileName}";
                        }
                        else
                        {
                            var baseUrl = $"{_config["ItemMasterBaseUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/api/CustomerMaster/DownloadFile?Filename={fileUploadDto.FileName}";
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
                _logger.LogError($"Something went wrong inside CustomermasterFiles action: {ex.Message}");
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

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateCustomerMaster(int id, [FromBody] CustomerMasterDtoUpdate customerMasterDtoUpdate)
        {
            ServiceResponse<CustomerMasterDtoUpdate> serviceResponse = new ServiceResponse<CustomerMasterDtoUpdate>();
            try
            {
                if (customerMasterDtoUpdate is null)
                {
                    _logger.LogError("Update CustomerMaster object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update CustomerMaster object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update CustomerMaster object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update CustomerMaster object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest("Invalid model object");
                }
                var updateCustomerMaster = await _repository.CustomerMasterRepository.GetCustomerMasterById(id);
                if (updateCustomerMaster is null)
                {
                    _logger.LogError($"Update CustomerMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update CustomerMaster with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

               
                var addresses = _mapper.Map<IEnumerable<CustomerAddresses>>(customerMasterDtoUpdate.CustomerAddress);
                var related = _mapper.Map<IEnumerable<CustomerRelatedCustomer>>(customerMasterDtoUpdate.RelatedCustomers);
                var contacts = _mapper.Map<IEnumerable<CustomerContacts>>(customerMasterDtoUpdate.CustomerContacts);
                var shippingAddresses = _mapper.Map<IEnumerable<CustomerShippingAddresses>>(customerMasterDtoUpdate.CustomerShippingAddresses);
                var banking = _mapper.Map<IEnumerable<CustomerBanking>>(customerMasterDtoUpdate.CustomerBankings);
                var HeadcountDetails = _mapper.Map<IEnumerable<CustomerMasterHeadCounting>>(customerMasterDtoUpdate.CustomerMasterHeadCountings);

                var customerMasters = _mapper.Map(customerMasterDtoUpdate, updateCustomerMaster);

                customerMasters.CustomerAddresses= addresses.ToList();
                customerMasters.CustomerContacts= contacts.ToList();
                customerMasters.RelatedCustomers = related.ToList();
                customerMasters.CustomerShippingAddresses= shippingAddresses.ToList();
                customerMasters.CustomerBanking= banking.ToList();
                customerMasters.CustomerMasterHeadCountings = HeadcountDetails.ToList();
                string result = await _repository.CustomerMasterRepository.UpdateCustomerMaster(customerMasters);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CustomerMaster Successfully Updated"; ;
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateCustomerMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

         [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerMaster(int id)
        {
            ServiceResponse<CustomerMasterDto> serviceResponse = new ServiceResponse<CustomerMasterDto>();
            try
            {
                var deleteCustomerMaster = await _repository.CustomerMasterRepository.GetCustomerMasterById(id);
                if (deleteCustomerMaster == null)
                {
                    _logger.LogError($"Delete CustomerMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete CustomerMaster with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.CustomerMasterRepository.DeleteCustomerMaster(deleteCustomerMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CustomerMaster Successfully Deleted"; 
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteCustomerMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveCustomerIdNameList()
        {
            ServiceResponse<IEnumerable<CustomerIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<CustomerIdNameListDto>>();
            try
            {
                var listOfActiveCustomerMaster = await _repository.CustomerMasterRepository.GetAllActiveCustomerMasterIdNameList();
                //_logger.LogInfo("Returned all CustomerMaster");
                var result = _mapper.Map<IEnumerable<CustomerIdNameListDto>>(listOfActiveCustomerMaster);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All ActiveCustomerIdNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveCustomerIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomerIdNameList()
        {
            ServiceResponse<IEnumerable<CustomerIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<CustomerIdNameListDto>>();
            try
            {
                var listOfCustomerMaster = await _repository.CustomerMasterRepository.GetAllCustomerMasterIdNameList();
                //_logger.LogInfo("Returned all CustomerMaster");
                var result = _mapper.Map<IEnumerable<CustomerIdNameListDto>>(listOfCustomerMaster);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All CustomerIdNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllCustomerIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateCustomerMaster(int id)
        {
            ServiceResponse<CustomerMasterDto> serviceResponse = new ServiceResponse<CustomerMasterDto>();

            try
            {
                var customerMaster = await _repository.CustomerMasterRepository.GetCustomerMasterById(id);
                if (customerMaster is null)
                {
                    _logger.LogError($"customerMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "customerMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                customerMaster.IsActive = true;
                string result = await _repository.CustomerMasterRepository.UpdateCustomerMaster(customerMaster);
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
                _logger.LogError($"Something went wrong inside ActivateCustomerMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCustomerMaster(int id)
        {
            ServiceResponse<CustomerMasterDto> serviceResponse = new ServiceResponse<CustomerMasterDto>();

            try
            {
                var customerMaster = await _repository.CustomerMasterRepository.GetCustomerMasterById(id);
                if (customerMaster is null)
                {
                    _logger.LogError($"customerMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "customerMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                customerMaster.IsActive = false;
                string result = await _repository.CustomerMasterRepository.UpdateCustomerMaster(customerMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "DeActivate Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateCustomerMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetCustomerLeadIdSPReportOnDailyBasis()
        {
            ServiceResponse<IEnumerable<CustomerMasterLeadIdSPReport>> serviceResponse = new ServiceResponse<IEnumerable<CustomerMasterLeadIdSPReport>>();
            try
            {
                var products = await _repository.CustomerMasterRepository.GetCustomerLeadIdDataOnDailyBasis();

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GetCustomerLeadIdDataOnDailyBasis hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"GetCustomerLeadIdDataOnDailyBasis hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned GetCustomerLeadIdDataOnDailyBasis ";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCustomerLeadIdSPReportOnDailyBasis action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
