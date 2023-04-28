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
    public class ItemMasterController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IFileUploadRepository _fileUploadRepository;
        private IImageUploadRepository _imageUploadRepository;
        public ItemMasterController(IRepositoryWrapperForMaster repository, IImageUploadRepository imageUploadRepository, IFileUploadRepository fileUploadRepository,ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
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

        // GET api/<ItemMasterController>/5
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
                    List<FileUpload> fileUplaodDtoList = new List<FileUpload>();
                    FileUpload ImageUplaodDtoList = new FileUpload();

                    if (getItemMaster.FileUpload.Count() != 0)
                    {
                        foreach (var fileUploadDetails in getItemMaster.FileUpload)
                        {
                            FileUpload fileUpload = _mapper.Map<FileUpload>(fileUploadDetails);
                            fileUplaodDtoList.Add(fileUpload);
                        }
                    }
                    getItemMaster.FileUpload = fileUplaodDtoList;
                     

                    _logger.LogInfo($"Returned ItemMaster with id: {id}");
                    var result = _mapper.Map<ItemMasterDto>(getItemMaster);
                    serviceResponse.Data = result;
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
            ServiceResponse<IEnumerable<ItemMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterDto>>();
            try
            {
                var itemMasterList = await _repository.ItemMasterRepository.SearchItemMasterDate(searchDateParam);

                var result = _mapper.Map<IEnumerable<ItemMasterDto>>(itemMasterList);
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
            ServiceResponse<IEnumerable<ItemMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterDto>>();
            try
            {
                var itemMasterList = await _repository.ItemMasterRepository.SearchItemMaster(searchParames);

                _logger.LogInfo("Returned all ItemMasters");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<ItemMasterDto, ItemMaster>().ReverseMap()
                         .ForMember(dest => dest.ItemmasterAlternate, opt => opt.MapFrom(src => src.ItemmasterAlternate))
                         .ForMember(dest => dest.ItemMasterApprovedVendor, opt => opt.MapFrom(src => src.ItemMasterApprovedVendor))
                          .ForMember(dest => dest.ItemMasterWarehouse, opt => opt.MapFrom(src => src.ItemMasterWarehouse))
                           .ForMember(dest => dest.ItemMasterFileUpload, opt => opt.MapFrom(src => src.ItemMasterFileUpload))
                           .ForMember(dest => dest.ItemMasterRouting, opt => opt.MapFrom(src => src.ItemMasterRouting));
                });

                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<ItemMasterDto>>(itemMasterList);
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
            ServiceResponse<IEnumerable<ItemMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterDto>>();
            try
            {
                var itemMasterList = await _repository.ItemMasterRepository.GetAllItemMasterWithItems(itemMasterSearch);

                _logger.LogInfo("Returned all ItemMasters");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<ItemMasterDto, ItemMaster>().ReverseMap()
                        .ForMember(dest => dest.ItemmasterAlternate, opt => opt.MapFrom(src => src.ItemmasterAlternate))
                         .ForMember(dest => dest.ItemMasterApprovedVendor, opt => opt.MapFrom(src => src.ItemMasterApprovedVendor))
                          .ForMember(dest => dest.ItemMasterWarehouse, opt => opt.MapFrom(src => src.ItemMasterWarehouse))
                           .ForMember(dest => dest.ItemMasterFileUpload, opt => opt.MapFrom(src => src.ItemMasterFileUpload))
                           .ForMember(dest => dest.ItemMasterRouting, opt => opt.MapFrom(src => src.ItemMasterRouting));
                });

                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<ItemMasterDto>>(itemMasterList);
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
                var itemMasterFileUpload = _mapper.Map<IEnumerable<ItemMasterFileUpload>>(itemMasterDtoPost.ItemMasterFileUpload);
                var itemMasterRouting=_mapper.Map<IEnumerable<ItemMasterRouting>>(itemMasterDtoPost.ItemMasterRouting);
                var itemMasterWarehouse = _mapper.Map<IEnumerable<ItemMasterWarehouse>>(itemMasterDtoPost.ItemMasterWarehouse);



                //single file upload 
                var imageUploadDtoList = new List<ImageUpload>();


                //var ImageUploadDetails = itemMasterDtoPost.ImageUpload;
                
                //foreach (var ImageUploadDetail in ImageUploadDetails)
                //{
                //    var imageContent = ImageUploadDetail.FileByte;
                //    var itemNumbers = itemMasterDtoPost.ItemNumber;
                //    string imageName = ImageUploadDetail.FileName + "." + ImageUploadDetail.FileExtension;
                //    string imageExt = Path.GetExtension(imageName).ToUpper();
                //    if (imageExt == ".PNG" || imageExt == ".JPG" || imageExt == ".JPEG" || imageExt == ".GIF")
                //    {
                //        //Guid guid = Guid.NewGuid();
                //        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "ImageUpload", /*guid.ToString() + "_" +*/ imageName);
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
                //                FilePath = imagePath,
                //                ParentId = itemNumbers,
                //                DocumentFrom = "ItemMaster Image Document"
                //            };
                //            _repository.ImageUploadRepository.ImageUploadDocument(uploadedFiles);
                //            _repository.SaveAsync();
                //            if (uploadedFiles != null)
                //            {
                //                ImageUpload itemmasterImageDetails = _mapper.Map<ImageUpload>(uploadedFiles);
                //                imageUploadDtoList.Add(itemmasterImageDetails);
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

                ////multiple file upload


                //var FileUploadDetails = itemMasterDtoPost.FileUpload;
                //foreach (var FileUploadDetail in FileUploadDetails)
                //{
                //    var fileContent = FileUploadDetail.FileByte;
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
        public async Task<IActionResult> UpdateItemMaster(int id, [FromBody] ItemMasterDto itemMasterDtoUpdate)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

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
                 var itemMasterAlternate = _mapper.Map<IEnumerable<ItemmasterAlternate>>(itemMasterDtoUpdate.ItemmasterAlternate);
                var itemMasterApprovedVendor = _mapper.Map<IEnumerable<ItemMasterApprovedVendor>>(itemMasterDtoUpdate.ItemMasterApprovedVendor);
                var itemMasterFileUpload = _mapper.Map<IEnumerable<ItemMasterFileUpload>>(itemMasterDtoUpdate.ItemMasterFileUpload);
                var itemMasterRouting = _mapper.Map<IEnumerable<ItemMasterRouting>>(itemMasterDtoUpdate.ItemMasterRouting);
                var itemMasterWarehouse = _mapper.Map<IEnumerable<ItemMasterWarehouse>>(itemMasterDtoUpdate.ItemMasterWarehouse);
                var itemMaster = _mapper.Map(itemMasterDtoUpdate, updateItemMasterEntity);

                itemMaster.ItemmasterAlternate = itemMasterAlternate.ToList();
                itemMaster.ItemMasterApprovedVendor = itemMasterApprovedVendor.ToList();
                itemMaster.ItemMasterFileUpload=itemMasterFileUpload.ToList();
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
        [HttpGet("ItemNumber")]
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
                    _logger.LogInfo($"Returned Itemmasters with id: {ItemNumber}");
                    var result = _mapper.Map<ItemMasterDto>(getItemMasterByItemNumber);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned All ItemMasterByItemNumber";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetItemMasterByItemNumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
