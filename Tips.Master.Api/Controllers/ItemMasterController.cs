using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repository;
using static System.Net.Mime.MediaTypeNames;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ItemMasterController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IConfiguration _config;
        private IMapper _mapper;
        private IFileUploadRepository _fileUploadRepository;
        private IImageUploadRepository _imageUploadRepository;
        private IWebHostEnvironment _webHostEnvironment;


        public ItemMasterController(IRepositoryWrapperForMaster repository, IWebHostEnvironment webHostEnvironment, IConfiguration config, IImageUploadRepository imageUploadRepository, IFileUploadRepository fileUploadRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _config = config;
            _webHostEnvironment = webHostEnvironment;

            _fileUploadRepository = fileUploadRepository;
            _imageUploadRepository = imageUploadRepository;
            _mapper = mapper;
        }
        // GET: api/<ItemMasterController>
        [HttpGet]
        public async Task<IActionResult> GetAllItemMasters([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ItemMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterDto>>();

            try
            {
                var getAllItemMastersList = await _repository.ItemMasterRepository.GetAllItemMasters(pagingParameter, searchParams);
                _logger.LogInfo("Returned all ItemMasters");
                var metadata = new
                {
                    getAllItemMastersList.TotalCount,
                    getAllItemMastersList.PageSize,
                    getAllItemMastersList.CurrentPage,
                    getAllItemMastersList.HasNext,
                    getAllItemMastersList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var result = _mapper.Map<IEnumerable<ItemMasterDto>>(getAllItemMastersList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ItemMasters Successfully";
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
        //GET All FG items
        [HttpGet]
        public async Task<IActionResult> GetAllFGItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ItemMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterDto>>();

            try
            {
                var getAllFGItemsList = await _repository.ItemMasterRepository.GetAllFGItems();
                _logger.LogInfo("Returned all FGItemMasters");

                var result = _mapper.Map<IEnumerable<ItemMasterDto>>(getAllFGItemsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all FGItemMasters Successfully";
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
        //passing items number and get process records




        //GET All Sa items
        [HttpGet]
        public async Task<IActionResult> GetAllSAItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ItemMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterDto>>();

            try
            {
                var getAllSAItemsList = await _repository.ItemMasterRepository.GetAllSAItems();
                _logger.LogInfo("Returned all SAItemMasters");

                var result = _mapper.Map<IEnumerable<ItemMasterDto>>(getAllSAItemsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SAItemMasters Successfully";
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
        //GET All FG&SAItems
        [HttpGet]
        public async Task<IActionResult> GetAllFGSAItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ItemMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterDto>>();

            try
            {
                var getAllFGSAItemsList = await _repository.ItemMasterRepository.GetAllFgSaItems();
                _logger.LogInfo("Returned all FGSAItemMasters");

                var result = _mapper.Map<IEnumerable<ItemMasterDto>>(getAllFGSAItemsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all FGSAItemMasters Successfully";
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

        // get all fg,sa,fru item list

        [HttpGet]
        //public async Task<IActionResult> getAllBomItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        public async Task<IActionResult> getAllBomItems()
        {
            ServiceResponse<IEnumerable<ItemMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterDto>>();

            try
            {
                var getAllFGSAItemsList = await _repository.ItemMasterRepository.GetAllFgSaFruItems();
                _logger.LogInfo("Returned all FGSAFRUItemMasters");

                var result = _mapper.Map<IEnumerable<ItemMasterDto>>(getAllFGSAItemsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all FG,SA,FRU ItemMasters Successfully";
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
        public async Task<IActionResult> GetAllPurchasePartItemNoList()
        {
            ServiceResponse<IEnumerable<ItemNoListDtos>> serviceResponse = new ServiceResponse<IEnumerable<ItemNoListDtos>>();

            try
            {
                var purchasePartItemNo = await _repository.ItemMasterRepository.GetAllPurchasePartItemNoList();
                _logger.LogInfo("Returned all Item Number with PurchasePart");
                var result = _mapper.Map<IEnumerable<ItemNoListDtos>>(purchasePartItemNo);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ItemNumberList Successfully";
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
        //test file path 
        //[HttpGet("{filename}")]
        //public async Task<ActionResult> GetFilePath(string filename)
        //{
        //    string path = _webHostEnvironment.ContentRootPath + "\\Upload\\ImageUpload\\";
        //    var paths = Path.Combine(path, filename); // Use Path.Combine to combine paths

        //    if (System.IO.File.Exists(paths))
        //    {
        //        byte[] fileBytes = System.IO.File.ReadAllBytes(paths);
        //        return File(fileBytes, "image/jpeg");
        //    }

        //    return NotFound(); // Return a 404 response if the file doesn't exist
        //}
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
        [HttpGet]
        public async Task<ActionResult> DownloadImage(string Filename)
        {
            ServiceResponse<FileContentResult> serviceResponse = new ServiceResponse<FileContentResult>();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "ImageUpload", Filename);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var ContentType))
            {
                ContentType = "application/octet-stream";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(bytes, ContentType, Path.GetFileName(filePath));
        }
        [HttpGet("{filename}")]
        public async Task<ActionResult> GetFilePath(string filename)
        {
            var fileByte = await _imageUploadRepository.GetImageFileByte(filename);
            if (fileByte != null)
            {
                byte[] imageBytes = Convert.FromBase64String(fileByte);
                using (var ms = new MemoryStream(imageBytes))
                using (var image = System.Drawing.Image.FromStream(ms))
                {
                    var imageStream = new MemoryStream();
                    image.Save(imageStream, ImageFormat.Jpeg); // You can change the format as needed

                    return File(imageStream.ToArray(), "image/jpeg"); // Change the content type as needed
                }
            }
            return NotFound();
        }
        //test
        [HttpGet("{filename}")]
        public async Task<ActionResult> ViewImage1(string filename)
        {
            string baseUrl = $"{Request.Scheme}://{_config["ItemMasterBaseUrl"]}";
            string FilePath = "/home/nayagam/GetaPcs/Aviation/Master/Upload/ImageUpload";
            string ImagePath = Path.Combine(FilePath, filename);
            //string patcho = GetFilePath();
            if (System.IO.File.Exists(ImagePath))
            {
                return PhysicalFile(ImagePath, "image/jpeg");
            }

            var ImageUrl = baseUrl + "/Upload/ImageUpload/" + filename;
            return Content(ImageUrl);
        }

        [HttpGet]
        public FileContentResult showsdata()
        {
            // get strings from db
            string base64image = "/9j/2wCEAAkGBxITEhITEhISFRUVFRcVFRUVFRUVFRUVFRUWFxUVFRUYHSggGBolHRUVITEhJSkrLi4uFx8zODMtNygtLisBCgoKDg0OFRAQFSsZFR0tLS0tLSstLS0rLS0rLS0tLS0tKy0tLS0tNysrLTctNy0tKzc3LS0tLS0tNzc3NysrK//AABEIAOEA4QMBIgACEQEDEQH/xAAbAAABBQEBAA...WghBDURCECX/9k=";
            byte[] Picture = Convert.FromBase64String(base64image);
            return File(Picture, "image/png");
        }



        [HttpGet("{filename}")]
        public async Task<ActionResult> ViewImage(string filename)
        {
            try
            {
                //string path = _web
                //Environment.WebRootPath + "\\";
                var baseUrl = $"{Request.Scheme}://{_config["ItemMasterBaseUrl"]}";
                string imageUrl = $"{baseUrl}GetaPcs/Aviation/Master/Upload/ImageUpload/{Uri.EscapeUriString(filename)}";
                return Ok(imageUrl);
                //return imageUrl;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }



        //Get download Url 

        [HttpGet]
        public async Task<IActionResult> GetDownloadUrlDetailsforItemImage(int imageid)
        {
            ServiceResponse<GetDownloadUrlDtos> serviceResponse = new ServiceResponse<GetDownloadUrlDtos>();

            try
            {
                string serverKey = GetServerKey();
                var getDownloadDetailByPoNumber = await _repository.ItemMasterRepository.GetDownloadUrlDetails(imageid);

                if (getDownloadDetailByPoNumber == null)
                {
                    _logger.LogError($"DownloadDetail with id: {imageid}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DownloadDetail with id: {imageid}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Itemmaster UploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Itemmaster UploadDocument sent from client.");
                    return BadRequest(serviceResponse);
                }
                if (serverKey == "avision")
                {
                    var baseUrl = $"{_config["ItemMasterBaseUrl"]}";
                   // fileUploadDto.DownloadUrl = $"{baseUrl}/apigateway/tips/ItemMaster/DownloadFile?Filename={fileUploadDto.FileName}";
                    getDownloadDetailByPoNumber.DownloadUrl = $"{baseUrl}/apigateway/tips/ItemMaster/DownloadFile?Filename={getDownloadDetailByPoNumber.FileName}";
                }
                else
                {
                    var baseUrl = $"{_config["ItemMasterBaseUrl"]}";
                   // fileUploadDto.DownloadUrl = $"{baseUrl}/api/ItemMaster/DownloadFile?Filename={fileUploadDto.FileName}";
                    getDownloadDetailByPoNumber.DownloadUrl = $"{baseUrl}/api/ItemMaster/DownloadFile?Filename={getDownloadDetailByPoNumber.FileName}";
                }
                //var baseUrl = $"{Request.Scheme}://{_config["ItemMasterBaseUrl"]}";
                //    getDownloadDetailByPoNumber.DownloadUrl = $"{baseUrl}/api/ItemMaster/DownloadFile?Filename={getDownloadDetailByPoNumber.FileName}";
                
                _logger.LogInfo($"Returned DownloadDetail with id: {imageid}");
                var result = _mapper.Map<GetDownloadUrlDtos>(getDownloadDetailByPoNumber);
                serviceResponse.Data = result;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Itemmaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
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
        public async Task<IActionResult> GetDownloadUrlDetailsforItemFiles(string fileids)
        {
            ServiceResponse<List<FileUploadDto>> serviceResponse = new ServiceResponse<List<FileUploadDto>>();
            try
            {
                string serverKey = GetServerKey();
                var itemsFiles = await _fileUploadRepository.GetDownloadUrlDetails(fileids);
                if (itemsFiles == null)
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
                    serviceResponse.Message = "Invalid Itemmaster UploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Itemmaster UploadDocument sent from client.");
                    return BadRequest(serviceResponse);
                }
                List<FileUploadDto> fileUploads = new List<FileUploadDto>();
                if (itemsFiles != null)
                {
                    foreach (var fileUploadDetails in itemsFiles)
                    {
                        FileUploadDto fileUploadDto = _mapper.Map<FileUploadDto>(fileUploadDetails);
                        if (serverKey == "avision")
                        {
                            var baseUrl = $"{_config["ItemMasterBaseUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/apigateway/tips/ItemMaster/DownloadFile?Filename={fileUploadDto.FileName}";
                        }
                        else
                        {
                            var baseUrl = $"{_config["ItemMasterBaseUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/api/ItemMaster/DownloadFile?Filename={fileUploadDto.FileName}";
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
                _logger.LogError($"Something went wrong inside ItemmasterFiles action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //[HttpGet]
        //public async Task<ActionResult> DownloadFile(string Filename)
        //{
        //    ServiceResponse<FileContentResult> serviceResponse = new ServiceResponse<FileContentResult>();

        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "ImageUpload", Filename);
        //    var provider = new FileExtensionContentTypeProvider();
        //    if (!provider.TryGetContentType(filePath, out var ContentType))
        //    {
        //        ContentType = "application/octet-stream";
        //    }
        //    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

        //    return File(bytes, ContentType, Path.GetFileName(filePath));
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemMasterById(int id)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

            try
            {
                var getItemMaster = await _repository.ItemMasterRepository.GetItemMasterById(id);
                if (getItemMaster == null)
                {
                    _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ItemMaster with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned owner with id: {id}");

                    ItemMasterDto itemMasterDto = _mapper.Map<ItemMasterDto>(getItemMaster);
                    List<ItemmasterAlternateDto> itemmasterAlternateDtos = new List<ItemmasterAlternateDto>();

                    //List<FileUploadDto> fileUploads = new List<FileUploadDto>();
                    //List<ImageUploadDto> imageUploads = new List<ImageUploadDto>();

                    //if (itemMasterDto.ImageUpload.Count() != 0)
                    //{
                    //    foreach (var imageUploadDetails in itemMasterDto.ImageUpload)
                    //    {
                    //        ImageUploadDto imageUploadDto = _mapper.Map<ImageUploadDto>(imageUploadDetails);
                    //        var baseUrl = $"{Request.Scheme}://{_config["ItemMasterBaseUrl"]}";
                    //        imageUploadDto.FilePath = $"{baseUrl}/api/ItemMaster/DownloadImage?Filename={Uri.EscapeUriString(imageUploadDto.FileName)}";

                    //        //imageUploadDto.FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "ImageUpload", Uri.EscapeUriString(imageUploadDto.FileName));
                    //        imageUploads.Add(imageUploadDto);
                    //    }
                    //}


                    //if (itemMasterDto.FileUpload.Count() != 0)
                    //{
                    //    foreach (var fileUploadDetails in itemMasterDto.FileUpload)
                    //    {
                    //        FileUploadDto fileUploadDto = _mapper.Map<FileUploadDto>(fileUploadDetails);
                    //        var baseUrl = $"{Request.Scheme}://{_config["ItemMasterBaseUrl"]}";
                    //        fileUploadDto.FilePath = $"{baseUrl}/api/ItemMaster/DownloadFile?Filename={fileUploadDto.FileName}";

                    //        //fileUploadDto.FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "FileUpload", fileUploadDto.FileName);
                    //        fileUploads.Add(fileUploadDto);
                    //    }
                    //}
                    //itemMasterDto.FileUpload = fileUploads;
                    serviceResponse.Data = itemMasterDto;
                    serviceResponse.Message = "Returned ItemMasterById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);


                }
            }


            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetItemMasterById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //// GET api/<ItemMasterController>/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetItemMasterById(int id)
        //{
        //    ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

        //    try
        //    {
        //        var getItemMaster = await _repository.ItemMasterRepository.GetItemMasterById(id);
        //        if (getItemMaster == null)
        //        {
        //            _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"ItemMaster with id hasn't been found in db.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(serviceResponse);
        //        }
        //        else
        //        {
        //            List<FileUpload> fileUplaodDtoList = new List<FileUpload>();
        //            FileUpload ImageUplaodDtoList = new FileUpload();

        //            if (getItemMaster.FileUpload.Count() != 0)
        //            {
        //                foreach (var fileUploadDetails in getItemMaster.FileUpload)
        //                {
        //                    FileUpload fileUpload = _mapper.Map<FileUpload>(fileUploadDetails);
        //                    fileUplaodDtoList.Add(fileUpload);
        //                }
        //            }
        //            getItemMaster.FileUpload = fileUplaodDtoList;


        //            _logger.LogInfo($"Returned ItemMaster with id: {id}");
        //            var result = _mapper.Map<ItemMasterDto>(getItemMaster);
        //            serviceResponse.Data = result;
        //            serviceResponse.Message = "Returned ItemMasterById Successfully";
        //            serviceResponse.Success = true;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Ok(serviceResponse);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside GetItemMasterById action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //     }
        //}
        [HttpGet]
        public async Task<IActionResult> GetAllFgTgItemMasterItemNoList()
        {
            ServiceResponse<IEnumerable<ItemMasterIdNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterIdNoListDto>>();
            try
            {
                var fgTgItemMasterList = await _repository.ItemMasterRepository.GetAllFgTgItemMasterItemNoList();

                var result = _mapper.Map<IEnumerable<ItemMasterIdNoListDto>>(fgTgItemMasterList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all FG and TG ItemMasterItemNoList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllFgTgItemMasterItemNoList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSAPurchasePartItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ItemMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterDto>>();

            try
            {
                var sAPurchasePartItemsList = await _repository.ItemMasterRepository.GetAllSAPurchasePartItems();
                _logger.LogInfo("Returned all SA & PurchasePartItemsListItemMasters");

                var result = _mapper.Map<IEnumerable<ItemMasterDto>>(sAPurchasePartItemsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SA and PurchasePartItemsListItemMasters Successfully";
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
        public async Task<IActionResult> SearchItemMasterDate([FromQuery] SearchDateParamess searchDateParam)
        {
            ServiceResponse<IEnumerable<ItemMasterReportDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterReportDto>>();
            try
            {
                var itemMasterList = await _repository.ItemMasterRepository.SearchItemMasterDate(searchDateParam);

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<ItemMaster, ItemMasterReportDto>()
                        .ForMember(dest => dest.ItemmasterAlternate, opt => opt.MapFrom(src => src.ItemmasterAlternate
                        .Select(AlternateSource => new ItemmasterAlternateReportDto
                        {
                            Id = AlternateSource.Id,
                            ItemNumber = src.ItemNumber,
                            ManufacturerPartNo = AlternateSource.ManufacturerPartNo,
                            Manufacturer = AlternateSource.Manufacturer,
                            AlternateSource = AlternateSource.AlternateSource,
                            IsDefault = AlternateSource.IsDefault,

                        })
                            )
                        )
                        .ForMember(dest => dest.ItemMasterWarehouse, opt => opt.MapFrom(src => src.ItemMasterWarehouse
                        .Select(Warehouse => new ItemMasterWarehouseReportDto
                        {
                            Id = Warehouse.Id,
                            ItemNumber = src.ItemNumber,
                            WareHouse = Warehouse.WareHouse,
                            IsActive = Warehouse.IsActive,
                        })
                            )
                        )
                        .ForMember(dest => dest.ItemMasterApprovedVendor, opt => opt.MapFrom(src => src.ItemMasterApprovedVendor
                        .Select(ApprovedVendor => new ItemMasterApprovedVendorReportDto
                        {
                            Id = ApprovedVendor.Id,
                            ItemNumber = src.ItemNumber,
                            VendorCode = ApprovedVendor.VendorCode,
                            VendorName = ApprovedVendor.VendorName,
                            ShareOfBusiness = ApprovedVendor.ShareOfBusiness,
                        })
                            )
                        )
                        .ForMember(dest => dest.ItemMasterRouting, opt => opt.MapFrom(src => src.ItemMasterRouting
                        .Select(Routing => new ItemMasterRoutingReportDto
                        {
                            Id = Routing.Id,
                            ItemNumber = src.ItemNumber,
                            ProcessStep = Routing.ProcessStep,
                            Process = Routing.ProcessStep,
                            RoutingDescription = Routing.RoutingDescription,
                            MachineHours = Routing.MachineHours,
                            LaborHours = Routing.LaborHours,
                            IsRoutingActive = Routing.IsRoutingActive,
                        })
                            )
                        );
                });
                var mapper = config.CreateMapper();

                var result = mapper.Map<IEnumerable<ItemMasterReportDto>>(itemMasterList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ItemMasters";
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

        [HttpGet]
        public async Task<IActionResult> SearchItemMaster([FromQuery] SearchParames searchParames)
        {
            ServiceResponse<IEnumerable<ItemMasterReportDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterReportDto>>();
            try
            {
                var itemMasterList = await _repository.ItemMasterRepository.SearchItemMaster(searchParames);

                _logger.LogInfo("Returned all ItemMasters");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<ItemMasterDto, ItemMaster>().ReverseMap()
                //         .ForMember(dest => dest.ItemmasterAlternate, opt => opt.MapFrom(src => src.ItemmasterAlternate))
                //         .ForMember(dest => dest.ItemMasterApprovedVendor, opt => opt.MapFrom(src => src.ItemMasterApprovedVendor))
                //          .ForMember(dest => dest.ItemMasterWarehouse, opt => opt.MapFrom(src => src.ItemMasterWarehouse))
                //           //.ForMember(dest => dest.ItemMasterFileUpload, opt => opt.MapFrom(src => src.ItemMasterFileUpload))
                //           .ForMember(dest => dest.ItemMasterRouting, opt => opt.MapFrom(src => src.ItemMasterRouting));
                //});

                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<ItemMaster, ItemMasterReportDto>()
                        .ForMember(dest => dest.ItemmasterAlternate, opt => opt.MapFrom(src => src.ItemmasterAlternate
                        .Select(AlternateSource => new ItemmasterAlternateReportDto
                        {
                            Id = AlternateSource.Id,
                            ItemNumber = src.ItemNumber,
                            ManufacturerPartNo = AlternateSource.ManufacturerPartNo,
                            Manufacturer = AlternateSource.Manufacturer,
                            AlternateSource = AlternateSource.AlternateSource,
                            IsDefault = AlternateSource.IsDefault,

                        })
                            )
                        )
                        .ForMember(dest => dest.ItemMasterWarehouse, opt => opt.MapFrom(src => src.ItemMasterWarehouse
                        .Select(Warehouse => new ItemMasterWarehouseReportDto
                        {
                            Id = Warehouse.Id,
                            ItemNumber = src.ItemNumber,
                            WareHouse = Warehouse.WareHouse,
                            IsActive = Warehouse.IsActive,
                        })
                            )
                        )
                        .ForMember(dest => dest.ItemMasterApprovedVendor, opt => opt.MapFrom(src => src.ItemMasterApprovedVendor
                        .Select(ApprovedVendor => new ItemMasterApprovedVendorReportDto
                        {
                            Id = ApprovedVendor.Id,
                            ItemNumber = src.ItemNumber,
                            VendorCode = ApprovedVendor.VendorCode,
                            VendorName = ApprovedVendor.VendorName,
                            ShareOfBusiness = ApprovedVendor.ShareOfBusiness,
                        })
                            )
                        )
                        .ForMember(dest => dest.ItemMasterRouting, opt => opt.MapFrom(src => src.ItemMasterRouting
                        .Select(Routing => new ItemMasterRoutingReportDto
                        {
                            Id = Routing.Id,
                            ItemNumber = src.ItemNumber,
                            ProcessStep = Routing.ProcessStep,
                            Process = Routing.ProcessStep,
                            RoutingDescription = Routing.RoutingDescription,
                            MachineHours = Routing.MachineHours,
                            LaborHours = Routing.LaborHours,
                            IsRoutingActive = Routing.IsRoutingActive,
                        })
                            )
                        );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<ItemMasterReportDto>>(itemMasterList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ItemMasters";
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

        [HttpPost]
        public async Task<IActionResult> GetAllItemMasterWithItems([FromBody] ItemMasterSearchDto itemMasterSearch)
        {
            ServiceResponse<IEnumerable<ItemMasterReportDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterReportDto>>();
            try
            {
                var itemMasterList = await _repository.ItemMasterRepository.GetAllItemMasterWithItems(itemMasterSearch);

                _logger.LogInfo("Returned all ItemMasters");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<ItemMasterDto, ItemMaster>().ReverseMap()
                //        .ForMember(dest => dest.ItemmasterAlternate, opt => opt.MapFrom(src => src.ItemmasterAlternate))
                //         .ForMember(dest => dest.ItemMasterApprovedVendor, opt => opt.MapFrom(src => src.ItemMasterApprovedVendor))
                //          .ForMember(dest => dest.ItemMasterWarehouse, opt => opt.MapFrom(src => src.ItemMasterWarehouse))
                //           //.ForMember(dest => dest.ItemMasterFileUpload, opt => opt.MapFrom(src => src.ItemMasterFileUpload))
                //           .ForMember(dest => dest.ItemMasterRouting, opt => opt.MapFrom(src => src.ItemMasterRouting));
                //});

                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<ItemMaster, ItemMasterReportDto>()
                        .ForMember(dest => dest.ItemmasterAlternate, opt => opt.MapFrom(src => src.ItemmasterAlternate
                        .Select(AlternateSource => new ItemmasterAlternateReportDto
                        {
                            Id = AlternateSource.Id,
                            ItemNumber = src.ItemNumber,
                            ManufacturerPartNo = AlternateSource.ManufacturerPartNo,
                            Manufacturer = AlternateSource.Manufacturer,
                            AlternateSource = AlternateSource.AlternateSource,
                            IsDefault = AlternateSource.IsDefault,

                        })
                            )
                        )
                        .ForMember(dest => dest.ItemMasterWarehouse, opt => opt.MapFrom(src => src.ItemMasterWarehouse
                        .Select(Warehouse => new ItemMasterWarehouseReportDto
                        {
                            Id = Warehouse.Id,
                            ItemNumber = src.ItemNumber,
                            WareHouse = Warehouse.WareHouse,
                            IsActive = Warehouse.IsActive,
                        })
                            )
                        )
                        .ForMember(dest => dest.ItemMasterApprovedVendor, opt => opt.MapFrom(src => src.ItemMasterApprovedVendor
                        .Select(ApprovedVendor => new ItemMasterApprovedVendorReportDto
                        {
                            Id = ApprovedVendor.Id,
                            ItemNumber = src.ItemNumber,
                            VendorCode = ApprovedVendor.VendorCode,
                            VendorName = ApprovedVendor.VendorName,
                            ShareOfBusiness = ApprovedVendor.ShareOfBusiness,
                        })
                            )
                        )
                        .ForMember(dest => dest.ItemMasterRouting, opt => opt.MapFrom(src => src.ItemMasterRouting
                        .Select(Routing => new ItemMasterRoutingReportDto
                        {
                            Id = Routing.Id,
                            ItemNumber = src.ItemNumber,
                            ProcessStep = Routing.ProcessStep,
                            Process = Routing.ProcessStep,
                            RoutingDescription = Routing.RoutingDescription,
                            MachineHours = Routing.MachineHours,
                            LaborHours = Routing.LaborHours,
                            IsRoutingActive = Routing.IsRoutingActive,
                        })
                            )
                        );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<ItemMasterReportDto>>(itemMasterList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ItemMasters";
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
        [HttpPost]
        public async Task<IActionResult> CreateItemMasterImageUpload([FromBody] ImageUploadPostDto imageUploadPostDto)
        {
            ServiceResponse<int> serviceResponse = new ServiceResponse<int>();
            try
            {
                if (imageUploadPostDto is null)
                {
                    _logger.LogError("ItemMasterImage object sent from client is null.");
                    serviceResponse.Data = 0;
                    serviceResponse.Message = "ItemMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ItemMasterImage object sent from client.");
                    serviceResponse.Data = 0;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                byte[] imageContent = Convert.FromBase64String(imageUploadPostDto.FileByte);
                //var itemNumbers = imageUploadPostDto.ItemNumber;
                string imageName = imageUploadPostDto.FileName + "." + imageUploadPostDto.FileExtension;
                string imageExt = Path.GetExtension(imageName).ToUpper();
                if (imageExt == ".PNG" || imageExt == ".JPG" || imageExt == ".JPEG" || imageExt == ".GIF")
                {
                    //Guid guid = Guid.NewGuid();
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "ImageUpload", /*/ guid.ToString() + "_" +/*/ imageName);
                    int id;
                    using (MemoryStream ms = new MemoryStream(imageContent))
                    {
                        ms.Position = 0;
                        using (var fileStream = new FileStream(imagePath, FileMode.Create, FileAccess.Write))
                        {
                            ms.WriteTo(fileStream);
                        }
                        var uploadedFiles = new ImageUpload
                        {
                            FileName = imageName,
                            FileExtension = imageExt,
                            FilePath = $"{Request.Scheme}://{Request.Host}/api/ItemMaster/Upload/ImageUpload/{imageName}",
                            ParentId = "ItemMaster",
                            DocumentFrom = "ItemMaster Image Document",
                            FileByte = imageUploadPostDto.FileByte
                        };
                        _repository.ImageUploadRepository.ImageUploadDocument(uploadedFiles);
                        _repository.SaveAsync();
                        id = uploadedFiles.Id;

                    }
                    serviceResponse.Data = id;
                    serviceResponse.Message = "ItemMasterImage Successfully Created";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogError("Invalid Image Format ..Please Use this JPG,JPEG,PNG,GIF....");
                    serviceResponse.Data = 0;
                    serviceResponse.Message = "Invalid Image Format ..Please Use this JPG,JPEG,PNG,GIF....";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateItemMasterImage action: {ex.Message}");
                serviceResponse.Data = 0;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateItemMasterFileUpload([FromBody] List<FileUploadPostDto> fileUploadPostDtos)
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                if (fileUploadPostDtos is null)
                {
                    _logger.LogError("ItemMasterFile object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ItemMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ItemMasterFile object sent from client.");
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
                            ParentId = "Item Master",
                            DocumentFrom = "ItemMaster File Document",
                            FileByte = FileUploadDetail.FileByte
                        };
                        _repository.FileUploadRepository.CreateFileUploadDocument(uploadedFile);
                        _repository.SaveAsync();
                        id_s.Add(uploadedFile.Id.ToString());

                    }
                }
                serviceResponse.Data = id_s;
                serviceResponse.Message = " ItemMasterFile Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ItemMasterFile action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // POST api/<ItemMasterController>
        [HttpPost]
        public IActionResult CreateItemMaster([FromBody] ItemMasterDtoPost itemMasterDtoPost)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

            try
            {
                if (itemMasterDtoPost is null)
                {
                    _logger.LogError("ItemMaster object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ItemMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ItemMaster object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var itemMasterEntity = _mapper.Map<ItemMaster>(itemMasterDtoPost);
                var itemMasterAlternate = _mapper.Map<IEnumerable<ItemmasterAlternate>>(itemMasterDtoPost.ItemmasterAlternate);
                var itemMasterApprovedVendor = _mapper.Map<IEnumerable<ItemMasterApprovedVendor>>(itemMasterDtoPost.ItemMasterApprovedVendor);
                //var itemMasterFileUpload = _mapper.Map<IEnumerable<ItemMasterFileUpload>>(itemMasterDtoPost.ItemMasterFileUpload);
                var itemMasterRouting = _mapper.Map<IEnumerable<ItemMasterRouting>>(itemMasterDtoPost.ItemMasterRouting);
                var itemMasterWarehouse = _mapper.Map<IEnumerable<ItemMasterWarehouse>>(itemMasterDtoPost.ItemMasterWarehouse);



                //single file upload 
                //var imageUploadDtoList = new ImageUpload();


                //var ImageUploadDetail = itemMasterDtoPost.ImageUpload;

                ////foreach (var ImageUploadDetail in ImageUploadDetails)
                ////{

                //    byte[] imageContent = Convert.FromBase64String(ImageUploadDetail.FileByte);
                //    var itemNumbers = itemMasterDtoPost.ItemNumber;
                //    string imageName = ImageUploadDetail.FileName + "." + ImageUploadDetail.FileExtension;
                //    string imageExt = Path.GetExtension(imageName).ToUpper();
                //    if (imageExt == ".PNG" || imageExt == ".JPG" || imageExt == ".JPEG" || imageExt == ".GIF")
                //    {
                //        //Guid guid = Guid.NewGuid();
                //        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "ImageUpload", /*/ guid.ToString() + "_" +/*/ imageName);
                //        using (MemoryStream ms = new MemoryStream(imageContent))
                //        {
                //            ms.Position = 0;
                //            using (var fileStream = new FileStream(imagePath, FileMode.Create, FileAccess.Write))
                //            {
                //                ms.WriteTo(fileStream);
                //            }
                //            var uploadedFiles = new ImageUpload
                //            {
                //                FileName = imageName,
                //                FileExtension = imageExt,
                //                FilePath = $"{Request.Scheme}://{Request.Host}/api/ItemMaster/Upload/ImageUpload/{imageName}",
                //                ParentId = itemNumbers,
                //                DocumentFrom = "ItemMaster Image Document",
                //                FileByte = ImageUploadDetail.FileByte
                //            };
                //            _repository.ImageUploadRepository.ImageUploadDocument(uploadedFiles);
                //            _repository.SaveAsync();
                //            if (uploadedFiles != null)
                //            {
                //                ImageUpload itemmasterImageDetails = _mapper.Map<ImageUpload>(uploadedFiles);
                //                imageUploadDtoList=itemmasterImageDetails;
                //            }

                //        }
                //    }

                //    else
                //    {
                //        _logger.LogError("Invalid Image Format ..Please Use this JPG,JPEG,PNG,GIF....");
                //        serviceResponse.Data = null;
                //        serviceResponse.Message = "Invalid Image Format ..Please Use this JPG,JPEG,PNG,GIF....";
                //        serviceResponse.Success = false;
                //        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                //        return BadRequest(serviceResponse);
                //    }

                //}
                //var fileUploadDtoList = new List<FileUpload>();

                //////multiple file upload


                //var FileUploadDetails = itemMasterDtoPost.FileUpload;
                //foreach (var FileUploadDetail in FileUploadDetails)
                //{
                //    byte[] fileContent = Convert.FromBase64String(FileUploadDetail.FileByte);
                //    var itemNumber = itemMasterDtoPost.ItemNumber;
                //    string fileName = FileUploadDetail.FileName + "." + FileUploadDetail.FileExtension;
                //    string FileExt = Path.GetExtension(fileName).ToUpper();

                //    //Guid guids = Guid.NewGuid();
                //    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "FileUpload",/*guids.ToString() + "_" +*/ fileName);
                //    using (MemoryStream ms = new MemoryStream(fileContent))
                //    {
                //        ms.Position = 0;
                //        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                //        {
                //            ms.WriteTo(fileStream);
                //        }
                //        var uploadedFile = new FileUpload
                //        {
                //            FileName = fileName,
                //            FileExtension = FileExt,
                //            FilePath = filePath,
                //            ParentId = itemNumber,
                //            DocumentFrom = "ItemMaster File Document",
                //            FileByte = FileUploadDetail.FileByte
                //        };
                //        _repository.FileUploadRepository.CreateFileUploadDocument(uploadedFile);
                //        _repository.SaveAsync();
                //        if (uploadedFile != null)
                //        {
                //            FileUpload itemmasterFileDetails = _mapper.Map<FileUpload>(uploadedFile);
                //            fileUploadDtoList.Add(itemmasterFileDetails);
                //        }

                //    }

                //}

                //itemMasterEntity.FileUpload = fileUploadDtoList;
                //itemMasterEntity.ImageUpload = imageUploadDtoList;
                _repository.ItemMasterRepository.CreateItemMaster(itemMasterEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ItemMaster Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetItemMasterById", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateItemMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemMaster(int id, [FromBody] ItemMasterDtoUpdate itemMasterDtoUpdate)
        {
            ServiceResponse<ItemMasterDtoUpdate> serviceResponse = new ServiceResponse<ItemMasterDtoUpdate>();

            try
            {
                if (itemMasterDtoUpdate is null)
                {
                    _logger.LogError("ItemMaster object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ItemMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ItemMaster object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updateItemMasterEntity = await _repository.ItemMasterRepository.GetItemMasterById(id);
                if (updateItemMasterEntity is null)
                {
                    _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ItemMaster hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                //if (itemMasterDtoUpdate.ImageUpload == null && updateItemMasterEntity.ImageUpload == null) { }
                //else if (itemMasterDtoUpdate.ImageUpload != null && updateItemMasterEntity.ImageUpload != null) { }
                //else if (itemMasterDtoUpdate.ImageUpload == null && updateItemMasterEntity.ImageUpload != null)
                //{
                //    int imageId = updateItemMasterEntity.ImageUpload.Id;
                //    await _repository.ImageUploadRepository.DeleteImage(imageId);
                //}
                //else if (itemMasterDtoUpdate.ImageUpload != null && updateItemMasterEntity.ImageUpload != null)
                //{
                //    int imageId = updateItemMasterEntity.ImageUpload.Id;
                //    await _repository.ImageUploadRepository.DeleteImage(imageId);
                //    var imageUploadDtoList = new ImageUpload();
                //    var ImageUploadDetail = itemMasterDtoUpdate.ImageUpload;
                //    byte[] imageContent = Convert.FromBase64String(ImageUploadDetail.FileByte);
                //    var itemNumbers = itemMasterDtoUpdate.ItemNumber;
                //    string imageName = ImageUploadDetail.FileName + "." + ImageUploadDetail.FileExtension;
                //    string imageExt = Path.GetExtension(imageName).ToUpper();
                //    if (imageExt == ".PNG" || imageExt == ".JPG" || imageExt == ".JPEG" || imageExt == ".GIF")
                //    {
                //        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "ImageUpload", /*/ guid.ToString() + "_" +/*/ imageName);
                //        using (MemoryStream ms = new MemoryStream(imageContent))
                //        {
                //            ms.Position = 0;
                //            using (var fileStream = new FileStream(imagePath, FileMode.Create, FileAccess.Write))
                //            {
                //                ms.WriteTo(fileStream);
                //            }
                //            var uploadedFiles = new ImageUpload
                //            {
                //                FileName = imageName,
                //                FileExtension = imageExt,
                //                FilePath = $"{Request.Scheme}://{Request.Host}/api/ItemMaster/Upload/ImageUpload/{imageName}",
                //                ParentId = itemNumbers,
                //                DocumentFrom = "ItemMaster Image Document",
                //                FileByte = ImageUploadDetail.FileByte,
                //                ItemMasterId = id
                //            };
                //            await _repository.ImageUploadRepository.ImageUploadDocument(uploadedFiles);
                //            //_repository.SaveAsync();
                //            if (uploadedFiles != null)
                //            {
                //                ImageUpload itemmasterImageDetails = _mapper.Map<ImageUpload>(uploadedFiles);
                //                imageUploadDtoList = itemmasterImageDetails;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        _logger.LogError("Invalid Image Format ..Please Use this JPG,JPEG,PNG,GIF....");
                //        serviceResponse.Data = null;
                //        serviceResponse.Message = "Invalid Image Format ..Please Use this JPG,JPEG,PNG,GIF....";
                //        serviceResponse.Success = false;
                //        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                //        return BadRequest(serviceResponse);
                //    }
                //}
                //else if (itemMasterDtoUpdate.ImageUpload != null && updateItemMasterEntity.ImageUpload == null)
                //{
                //    var imageUploadDtoList = new ImageUpload();
                //    var ImageUploadDetail = itemMasterDtoUpdate.ImageUpload;
                //    byte[] imageContent = Convert.FromBase64String(ImageUploadDetail.FileByte);
                //    var itemNumbers = itemMasterDtoUpdate.ItemNumber;
                //    string imageName = ImageUploadDetail.FileName + "." + ImageUploadDetail.FileExtension;
                //    string imageExt = Path.GetExtension(imageName).ToUpper();
                //    if (imageExt == ".PNG" || imageExt == ".JPG" || imageExt == ".JPEG" || imageExt == ".GIF")
                //    {
                //        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "ImageUpload", /*/ guid.ToString() + "_" +/*/ imageName);
                //        using (MemoryStream ms = new MemoryStream(imageContent))
                //        {
                //            ms.Position = 0;
                //            using (var fileStream = new FileStream(imagePath, FileMode.Create, FileAccess.Write))
                //            {
                //                ms.WriteTo(fileStream);
                //            }
                //            var uploadedFiles = new ImageUpload
                //            {
                //                FileName = imageName,
                //                FileExtension = imageExt,
                //                FilePath = $"{Request.Scheme}://{Request.Host}/api/ItemMaster/Upload/ImageUpload/{imageName}",
                //                ParentId = itemNumbers,
                //                DocumentFrom = "ItemMaster Image Document",
                //                FileByte = ImageUploadDetail.FileByte,
                //                ItemMasterId = id
                //            };
                //            await _repository.ImageUploadRepository.ImageUploadDocument(uploadedFiles);
                //            //_repository.SaveAsync();
                //            if (uploadedFiles != null)
                //            {
                //                ImageUpload itemmasterImageDetails = _mapper.Map<ImageUpload>(uploadedFiles);
                //                imageUploadDtoList = itemmasterImageDetails;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        _logger.LogError("Invalid Image Format ..Please Use this JPG,JPEG,PNG,GIF....");
                //        serviceResponse.Data = null;
                //        serviceResponse.Message = "Invalid Image Format ..Please Use this JPG,JPEG,PNG,GIF....";
                //        serviceResponse.Success = false;
                //        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                //        return BadRequest(serviceResponse);
                //    }
                //}
                //if (itemMasterDtoUpdate.FileUpload != null)
                //{
                //    foreach (var filE in itemMasterDtoUpdate.FileUpload)
                //    {
                //        if (filE.Id == null)
                //        {
                //            byte[] fileContent = Convert.FromBase64String(filE.FileByte);
                //            var itemNumber = itemMasterDtoUpdate.ItemNumber;
                //            string fileName = filE.FileName + "." + filE.FileExtension;
                //            string FileExt = Path.GetExtension(fileName).ToUpper();

                //            //Guid guids = Guid.NewGuid();
                //            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "FileUpload",/*guids.ToString() + "_" +*/ fileName);
                //            using (MemoryStream ms = new MemoryStream(fileContent))
                //            {
                //                ms.Position = 0;
                //                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                //                {
                //                    ms.WriteTo(fileStream);
                //                }
                //                var uploadedFile = new FileUpload
                //                {
                //                    FileName = fileName,
                //                    FileExtension = FileExt,
                //                    FilePath = filePath,
                //                    ParentId = itemNumber,
                //                    DocumentFrom = "ItemMaster File Document",
                //                    FileByte = filE.FileByte
                //                    //ItemMasterId=id
                //                };
                //                await _repository.FileUploadRepository.CreateFileUploadDocument(uploadedFile);
                //                //_repository.SaveAsync();
                //            }
                //        }
                //    }
                //}
                var itemMasterAlternate = _mapper.Map<IEnumerable<ItemmasterAlternate>>(itemMasterDtoUpdate.ItemmasterAlternate);
                var itemMasterApprovedVendor = _mapper.Map<IEnumerable<ItemMasterApprovedVendor>>(itemMasterDtoUpdate.ItemMasterApprovedVendor);
                //var itemMasterFileUpload = _mapper.Map<IEnumerable<ItemMasterFileUpload>>(itemMasterDtoUpdate.ItemMasterFileUpload);
                var itemMasterRouting = _mapper.Map<IEnumerable<ItemMasterRouting>>(itemMasterDtoUpdate.ItemMasterRouting);
                var itemMasterWarehouse = _mapper.Map<IEnumerable<ItemMasterWarehouse>>(itemMasterDtoUpdate.ItemMasterWarehouse);
                updateItemMasterEntity.ItemmasterAlternate = null;
                updateItemMasterEntity.ItemMasterApprovedVendor = null;
                //itemMaster.ItemMasterFileUpload=itemMasterFileUpload.ToList();
                //updateItemMasterEntity.ImageUpload = null;
                //updateItemMasterEntity.FileUpload = null;
                updateItemMasterEntity.ItemMasterRouting = null;
                updateItemMasterEntity.ItemMasterWarehouse = null;
                var itemMaster = _mapper.Map(itemMasterDtoUpdate, updateItemMasterEntity);

                itemMaster.ItemmasterAlternate = itemMasterAlternate.ToList();
                itemMaster.ItemMasterApprovedVendor = itemMasterApprovedVendor.ToList();
                //itemMaster.ItemMasterFileUpload=itemMasterFileUpload.ToList();
                //itemMaster.ImageUpload = null;
                //itemMaster.FileUpload = null;
                itemMaster.ItemMasterRouting = itemMasterRouting.ToList();
                itemMaster.ItemMasterWarehouse = itemMasterWarehouse.ToList();

                string result = await _repository.ItemMasterRepository.UpdateItemMaster(itemMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ItemMaster Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateItemMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<ItemMasterController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemMaster(int id)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

            try
            {
                var deleteItemMaster = await _repository.ItemMasterRepository.GetItemMasterById(id);
                if (deleteItemMaster == null)
                {
                    _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete ItemMaster hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.ItemMasterRepository.DeleteItemMaster(deleteItemMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ItemMaster Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteItemMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateItemMaster(int id)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

            try
            {
                var activateItemMaster = await _repository.ItemMasterRepository.GetItemMasterById(id);
                if (activateItemMaster is null)
                {
                    _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ItemMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                activateItemMaster.IsActive = true;
                string result = await _repository.ItemMasterRepository.UpdateItemMaster(activateItemMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ItemMaster Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateItemMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateItemMaster(int id)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

            try
            {
                var deactivateItemMaster = await _repository.ItemMasterRepository.GetItemMasterById(id);
                if (deactivateItemMaster is null)
                {
                    _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ItemMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                deactivateItemMaster.IsActive = false;
                string result = await _repository.ItemMasterRepository.UpdateItemMaster(deactivateItemMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ItemMaster Deactivated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateItemMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllActiveItemMasterIdNoList()
        {
            ServiceResponse<IEnumerable<ItemMasterIdNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterIdNoListDto>>();
            try
            {
                var getAllActiveItemMasters = await _repository.ItemMasterRepository.GetAllActiveItemMasterIdNoList();
                //_logger.LogInfo("Returned all CustomerMaster");
                var result = _mapper.Map<IEnumerable<ItemMasterIdNoListDto>>(getAllActiveItemMasters);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active ItemMasterIdNoList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveItemMasterIdNoList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItemMasterIdNoList()
        {
            ServiceResponse<IEnumerable<ItemMasterIdNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterIdNoListDto>>();
            try
            {
                var itemMasterDetails = await _repository.ItemMasterRepository.GetAllItemMasterIdNoList();
                //_logger.LogInfo("Returned all CustomerMaster");
                var result = _mapper.Map<IEnumerable<ItemMasterIdNoListDto>>(itemMasterDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all  ItemMasterIdNoList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllItemMasterIdNoList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetItemMasterByItemNumber(string ItemNumber)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

            try
            {
                var getItemMasterByItemNumber = await _repository.ItemMasterRepository.GetItemMasterByItemNumber(ItemNumber);
                if (getItemMasterByItemNumber == null)
                {
                    _logger.LogError($"Itemmasters with id: {ItemNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Itemmasters with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo("ItemmasterControlle" + Convert.ToString(getItemMasterByItemNumber));
                    _logger.LogInfo($"Returned Itemmasters with id: {ItemNumber}");
                    var result = _mapper.Map<ItemMasterDto>(getItemMasterByItemNumber);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned All ItemMasterByItemNumber:";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetItemMasterByItemNumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Internal server error: {ex.Message}{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetItemMasterFileUploadListByItemNumber(string ItemNumber)
        {
            ServiceResponse<IEnumerable<FileUpload>> serviceResponse = new ServiceResponse<IEnumerable<FileUpload>>();

            try
            {
                var itemMasterFileUploadByItemNumber = await _repository.ItemMasterRepository.GetAllItemMasterFileUploadList(ItemNumber);
                if (itemMasterFileUploadByItemNumber == null)
                {
                    _logger.LogError($"Itemmasters fileUpload with itemNo: {ItemNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Itemmasters fileUpload with itemNo hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    //_logger.LogInfo("ItemmasterControlle" + Convert.ToString(itemMasterFileUploadByItemNumber));
                    _logger.LogInfo($"Returned Itemmasters fileUpload with itemNo: {ItemNumber}");
                    var result = _mapper.Map<IEnumerable<FileUpload>>(itemMasterFileUploadByItemNumber);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned All ItemMasterFileUploadList";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetItemMasterFileUploadListByItemNumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Internal server error: {ex.Message}{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //get parttype by passing itemnumber
        [HttpPost]
        public async Task<IActionResult> GetItemPartTypeByItemNumber(List<string> itemNumberList)
        {
            ServiceResponse<List<ItemWithPartTypeDto>> serviceResponse = new ServiceResponse<List<ItemWithPartTypeDto>>();

            try
            {
                var getItemMasterByItemNumber = await _repository.ItemMasterRepository.GetItemPartTypeByItemNo(itemNumberList);
                if (getItemMasterByItemNumber == null)
                {
                    _logger.LogError($"some Itemmasters Not found in action method GetItemPartTypeByItemNumber");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Itemmasters Not found in action method GetItemPartTypeByItemNumber.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = getItemMasterByItemNumber;
                    serviceResponse.Message = "Returned All Item PartType:";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetItemMasterByItemNumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Internal server error: {ex.Message}{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetItemMasterDetailsForMNRByItemNo(string ItemNumber)
        {
            ServiceResponse<IEnumerable<ItemMasterMtrPartNoDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterMtrPartNoDto>>();

            try
            {
                var itemMasterDetails = await _repository.ItemMasterRepository.GetItemMasterByPartNo(ItemNumber);
                if (itemMasterDetails == null)
                {
                    _logger.LogError($"Itemmasters with id: {ItemNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Itemmasters with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Itemmasters with id: {ItemNumber}");
                    var result = _mapper.Map<List<ItemMasterMtrPartNoDto>>(itemMasterDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned All ItemMasterByItemNumber";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetItemMasterDetailsForMNRByItemNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
