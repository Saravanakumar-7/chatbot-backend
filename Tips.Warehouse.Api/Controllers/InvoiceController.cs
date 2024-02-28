using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Contracts;
using Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Repository;


namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class InvoiceController : ControllerBase
    {
        private IInvoiceRepository _invoiceRepository;
        private IBTODeliveryOrderItemsRepository _bTODeliveryOrderItemsRepository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public InvoiceController(IInvoiceRepository invoiceRepository, IInventoryTranctionRepository inventoryTranctionRepository, HttpClient httpClient, IConfiguration config, IBTODeliveryOrderItemsRepository bTODeliveryOrderItemsRepository, ILoggerManager logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _inventoryTranctionRepository = inventoryTranctionRepository;
            _config = config;
            _bTODeliveryOrderItemsRepository = bTODeliveryOrderItemsRepository;
            _httpContextAccessor = httpContextAccessor;

            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvoice([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<InvoiceDto>> serviceResponse = new ServiceResponse<IEnumerable<InvoiceDto>>();

            try
            {
                var getAllInvoicesList = await _invoiceRepository.GetAllInvoices(pagingParameter, searchParams);
                var metadata = new
                {
                    getAllInvoicesList.TotalCount,
                    getAllInvoicesList.PageSize,
                    getAllInvoicesList.CurrentPage,
                    getAllInvoicesList.HasNext,
                    getAllInvoicesList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<InvoiceDto>>(getAllInvoicesList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Invoices";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }

        }
        [HttpGet]
        public async Task<IActionResult> SearchInvoiceDate([FromQuery] SearchsDateParms searchDateParam)
        {
            ServiceResponse<IEnumerable<InvoiceReportDto>> serviceResponse = new ServiceResponse<IEnumerable<InvoiceReportDto>>();
            try
            {
                var invoicesDate = await _invoiceRepository.SearchInvoiceDate(searchDateParam);

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<Invoice, InvoiceReportDto>()
                       .ForMember(dest => dest.invoiceChildItems, opt => opt.MapFrom(src => src.invoiceChildItems
                       .Select(invoiceChildItem => new InvoiceChildItemReportDto
                       {
                           Id = invoiceChildItem.Id,
                           InvoiceNumber = src.InvoiceNumber,
                           DONumber = invoiceChildItem.DONumber,
                           FGItemNumber = invoiceChildItem.FGItemNumber,
                           Description = invoiceChildItem.Description,
                           InvoicedQty = invoiceChildItem.InvoicedQty,
                           PartType = invoiceChildItem.PartType,
                           UnitPrice = invoiceChildItem.UnitPrice,
                           UOC = invoiceChildItem.UOC,
                           UOM = invoiceChildItem.UOM,
                           TotalValue = invoiceChildItem.TotalValue,
                           SerialNumber = invoiceChildItem.SerialNumber,
                           SalesOrderID = invoiceChildItem.SalesOrderID,
                           SGST = invoiceChildItem.SGST,
                           IGST = invoiceChildItem.IGST,
                           CGST = invoiceChildItem.CGST,
                           UTGST = invoiceChildItem.UTGST,
                           TotalValueWithTax = invoiceChildItem.TotalValueWithTax,
                           Discount = invoiceChildItem.Discount,
                           DiscountType = invoiceChildItem.DiscountType,
                           BtoDeliveryOrderPartsId = invoiceChildItem.BtoDeliveryOrderPartsId,
                           InvoiceId = invoiceChildItem.InvoiceId,
                       })
                           )
                       )
                       .ForMember(dest => dest.InvoiceAdditionalCharges, opt => opt.MapFrom(src => src.InvoiceAdditionalCharges
                       .Select(invoiceAdditionalCharges => new InvoiceAdditionalChargesReportDto
                       {
                           Id = invoiceAdditionalCharges.Id,
                           InvoiceNumber = src.InvoiceNumber,
                           SalesOrderId = invoiceAdditionalCharges.SalesOrderId,
                           DONumber = invoiceAdditionalCharges.DONumber,
                           AdditionalChargesLabelName = invoiceAdditionalCharges.AdditionalChargesLabelName,
                           AddtionalChargesValueType = invoiceAdditionalCharges.AddtionalChargesValueType,
                           AddtionalChargesValueAmount = invoiceAdditionalCharges.AddtionalChargesValueAmount,
                           TotalValue = invoiceAdditionalCharges.TotalValue,
                           InvoicedValue = invoiceAdditionalCharges.InvoicedValue,
                           IGST = invoiceAdditionalCharges.IGST,
                           CGST = invoiceAdditionalCharges.CGST,
                           UTGST = invoiceAdditionalCharges.UTGST,
                           SGST = invoiceAdditionalCharges.SGST,
                           SalesAdditionalChargeId = invoiceAdditionalCharges.SalesAdditionalChargeId,

                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<InvoiceReportDto>>(invoicesDate);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Invoice";
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

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> InvoiceSPReportWithParameter([FromBody] InvoiceSPReportWithParamDTO invoiceSPReport)
        {
            ServiceResponse<IEnumerable<InvoiceSPReport>> serviceResponse = new ServiceResponse<IEnumerable<InvoiceSPReport>>();
            try
            {
                var products = await _invoiceRepository.InvoiceSPReportWithParameter(invoiceSPReport.InvoiceNumber, invoiceSPReport.DONumber, invoiceSPReport.CustomerId, invoiceSPReport.CustomerName, invoiceSPReport.CustomerAliasName, invoiceSPReport.SalesOrderNumber, invoiceSPReport.Location, invoiceSPReport.Warehouse, invoiceSPReport.KPN, invoiceSPReport.MPN, invoiceSPReport.IssuedTo);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Invoice hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Invoice hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Invoice Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside Invoice action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchInvoice([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<InvoiceReportDto>> serviceResponse = new ServiceResponse<IEnumerable<InvoiceReportDto>>();
            try
            {
                var invoicesList = await _invoiceRepository.SearchInvoice(searchParams);

                _logger.LogInfo("Returned all Invoice");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<InvoiceDto, Invoice>().ReverseMap()
                //    .ForMember(dest => dest.invoiceChildItems, opt => opt.MapFrom(src => src.invoiceChildItems));
                //});
                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<Invoice, InvoiceReportDto>()
                       .ForMember(dest => dest.invoiceChildItems, opt => opt.MapFrom(src => src.invoiceChildItems
                       .Select(invoiceChildItem => new InvoiceChildItemReportDto
                       {
                           Id = invoiceChildItem.Id,
                           InvoiceNumber = src.InvoiceNumber,
                           DONumber = invoiceChildItem.DONumber,
                           FGItemNumber = invoiceChildItem.FGItemNumber,
                           Description = invoiceChildItem.Description,
                           InvoicedQty = invoiceChildItem.InvoicedQty,
                           PartType = invoiceChildItem.PartType,
                           UnitPrice = invoiceChildItem.UnitPrice,
                           UOC = invoiceChildItem.UOC,
                           UOM = invoiceChildItem.UOM,
                           TotalValue = invoiceChildItem.TotalValue,
                           SerialNumber = invoiceChildItem.SerialNumber,
                           SalesOrderID = invoiceChildItem.SalesOrderID,
                           SGST = invoiceChildItem.SGST,
                           IGST = invoiceChildItem.IGST,
                           CGST = invoiceChildItem.CGST,
                           UTGST = invoiceChildItem.UTGST,
                           TotalValueWithTax = invoiceChildItem.TotalValueWithTax,
                           Discount = invoiceChildItem.Discount,
                           DiscountType = invoiceChildItem.DiscountType,
                           BtoDeliveryOrderPartsId = invoiceChildItem.BtoDeliveryOrderPartsId,
                           InvoiceId = invoiceChildItem.InvoiceId,
                       })
                           )
                       )
                       .ForMember(dest => dest.InvoiceAdditionalCharges, opt => opt.MapFrom(src => src.InvoiceAdditionalCharges
                       .Select(invoiceAdditionalCharges => new InvoiceAdditionalChargesReportDto
                       {
                           Id = invoiceAdditionalCharges.Id,
                           InvoiceNumber = src.InvoiceNumber,
                           SalesOrderId = invoiceAdditionalCharges.SalesOrderId,
                           DONumber = invoiceAdditionalCharges.DONumber,
                           AdditionalChargesLabelName = invoiceAdditionalCharges.AdditionalChargesLabelName,
                           AddtionalChargesValueType = invoiceAdditionalCharges.AddtionalChargesValueType,
                           AddtionalChargesValueAmount = invoiceAdditionalCharges.AddtionalChargesValueAmount,
                           TotalValue = invoiceAdditionalCharges.TotalValue,
                           InvoicedValue = invoiceAdditionalCharges.InvoicedValue,
                           IGST = invoiceAdditionalCharges.IGST,
                           CGST = invoiceAdditionalCharges.CGST,
                           UTGST = invoiceAdditionalCharges.UTGST,
                           SGST = invoiceAdditionalCharges.SGST,
                           SalesAdditionalChargeId = invoiceAdditionalCharges.SalesAdditionalChargeId,

                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<InvoiceReportDto>>(invoicesList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Invoices";
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
        public async Task<IActionResult> GetAllInvoiceWithItems([FromBody] InvoiceSearchDto invoiceSearchDto)
        {
            ServiceResponse<IEnumerable<InvoiceReportDto>> serviceResponse = new ServiceResponse<IEnumerable<InvoiceReportDto>>();
            try
            {
                var invoiceItems = await _invoiceRepository.GetAllInvoiceWithItems(invoiceSearchDto);

                _logger.LogInfo("Returned all Invoices");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<InvoiceDto, Invoice>().ReverseMap()
                //    .ForMember(dest => dest.invoiceChildItems, opt => opt.MapFrom(src => src.invoiceChildItems));
                //});
                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<Invoice, InvoiceReportDto>()
                       .ForMember(dest => dest.invoiceChildItems, opt => opt.MapFrom(src => src.invoiceChildItems
                       .Select(invoiceChildItem => new InvoiceChildItemReportDto
                       {
                           Id = invoiceChildItem.Id,
                           InvoiceNumber = src.InvoiceNumber,
                           DONumber = invoiceChildItem.DONumber,
                           FGItemNumber = invoiceChildItem.FGItemNumber,
                           Description = invoiceChildItem.Description,
                           InvoicedQty = invoiceChildItem.InvoicedQty,
                           PartType = invoiceChildItem.PartType,
                           UnitPrice = invoiceChildItem.UnitPrice,
                           UOC = invoiceChildItem.UOC,
                           UOM = invoiceChildItem.UOM,
                           TotalValue = invoiceChildItem.TotalValue,
                           SerialNumber = invoiceChildItem.SerialNumber,
                           SalesOrderID = invoiceChildItem.SalesOrderID,
                           SGST = invoiceChildItem.SGST,
                           IGST = invoiceChildItem.IGST,
                           CGST = invoiceChildItem.CGST,
                           UTGST = invoiceChildItem.UTGST,
                           TotalValueWithTax = invoiceChildItem.TotalValueWithTax,
                           Discount = invoiceChildItem.Discount,
                           DiscountType = invoiceChildItem.DiscountType,
                           BtoDeliveryOrderPartsId = invoiceChildItem.BtoDeliveryOrderPartsId,
                           InvoiceId = invoiceChildItem.InvoiceId,
                       })
                           )
                       )
                        .ForMember(dest => dest.InvoiceAdditionalCharges, opt => opt.MapFrom(src => src.InvoiceAdditionalCharges
                       .Select(invoiceAdditionalCharges => new InvoiceAdditionalChargesReportDto
                       {
                           Id = invoiceAdditionalCharges.Id,
                           InvoiceNumber = src.InvoiceNumber,
                           SalesOrderId = invoiceAdditionalCharges.SalesOrderId,
                           DONumber = invoiceAdditionalCharges.DONumber,
                           AdditionalChargesLabelName = invoiceAdditionalCharges.AdditionalChargesLabelName,
                           AddtionalChargesValueType = invoiceAdditionalCharges.AddtionalChargesValueType,
                           AddtionalChargesValueAmount = invoiceAdditionalCharges.AddtionalChargesValueAmount,
                           TotalValue = invoiceAdditionalCharges.TotalValue,
                           InvoicedValue = invoiceAdditionalCharges.InvoicedValue,
                           IGST = invoiceAdditionalCharges.IGST,
                           CGST = invoiceAdditionalCharges.CGST,
                           UTGST = invoiceAdditionalCharges.UTGST,
                           SGST = invoiceAdditionalCharges.SGST,
                           SalesAdditionalChargeId = invoiceAdditionalCharges.SalesAdditionalChargeId,

                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<InvoiceReportDto>>(invoiceItems);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Invoice";
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            ServiceResponse<InvoiceDto> serviceResponse = new ServiceResponse<InvoiceDto>();

            try
            {
                var getInvoiceDetailById = await _invoiceRepository.GetInvoiceById(id);
                if (getInvoiceDetailById == null)
                {
                    _logger.LogError($"Invoice with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Invoice with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned Invoice with id: {id}");
                    var result = _mapper.Map<InvoiceDto>(getInvoiceDetailById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned InvoiceById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInvoicestById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> InvoiceSPReportDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<InvoiceSPReport>> serviceResponse = new ServiceResponse<IEnumerable<InvoiceSPReport>>();
            try
            {
                var products = await _invoiceRepository.InvoiceSPReportDate(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Invoice hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Invoice hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Invoice Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside Invoice action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> InvoiceSPReport([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<InvoiceSPReport>> serviceResponse = new ServiceResponse<IEnumerable<InvoiceSPReport>>();
            try
            {
                var products = await _invoiceRepository.InvoiceSPReport(pagingParameter);
                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.HasNext,
                    products.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all InvoiceSPReport");
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Invoice hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Invoice hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Invoice Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside Invoice action";
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
        public async Task<IActionResult> CreateInvoice([FromBody] InvoicePostDto invoicePostDto)
        {
            ServiceResponse<InvoicePostDto> serviceResponse = new ServiceResponse<InvoicePostDto>();

            try
            {
                string serverKey = GetServerKey();
                if (invoicePostDto == null)
                {
                    _logger.LogError("Invoice object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invoice object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Invoice object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                //var invoiceChild = _mapper.Map<IEnumerable<InvoiceChildItem>>(invoicePostDto.invoiceChildItems);
                //var invoice = _mapper.Map<Invoice>(invoicePostDto);
                //invoice.invoiceChildItems = invoiceChild.ToList();

                var invoice = _mapper.Map<Invoice>(invoicePostDto);
                var invoiceitemsDto = invoicePostDto.InvoiceChildItems;

                var invoiceChildItemsEntityList = new List<InvoiceChildItem>();
                var InvoiceAdditionalChargesList = _mapper.Map<IEnumerable<InvoiceAdditionalCharges>>(invoicePostDto.InvoiceAdditionalCharges);


                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));



                //var newcount = await _invoiceRepository.GetInvoiceNumberAutoIncrementCount(date);

                //if (newcount > 0)
                //{
                //    var number = newcount + 1;
                //    string e = String.Format("{0:D4}", number);
                //    invoice.InvoiceNumber = days + months + years + "IN" + (e);
                //}
                //else
                //{
                //    var count = 1;
                //    var e = count.ToString("D4");
                //    invoice.InvoiceNumber = days + months + years + "IN" + (e);
                //}

                if (serverKey == "trasccon")
                {
                    var dateFormat = days + months + years;
                    var invoiceNumber = await _invoiceRepository.GenerateInvoiceNumber();
                    invoice.InvoiceNumber = dateFormat + invoiceNumber;
                }
                else if (serverKey == "keus")
                {
                    var dateFormat = days + months + years;
                    var invoiceNumber = await _invoiceRepository.GenerateInvoiceNumber();
                    invoice.InvoiceNumber = dateFormat + invoiceNumber;
                }
                else if (serverKey == "avision")
                {
                    var invoiceNumber = await _invoiceRepository.GenerateInvoiceNumberAvision();
                    invoice.InvoiceNumber = invoiceNumber;
                }
                else
                {
                    var invoiceNumber = await _invoiceRepository.GenerateInvoiceNumber();
                    invoice.InvoiceNumber = invoiceNumber;
                }

                if (invoiceitemsDto != null)
                {
                    for (int i = 0; i < invoiceitemsDto.Count; i++)
                    {
                        InvoiceChildItem invoiceChildItem = _mapper.Map<InvoiceChildItem>(invoiceitemsDto[i]);
                        invoiceChildItemsEntityList.Add(invoiceChildItem);

                        var invoiceQty = invoiceChildItem.InvoicedQty;
                        var doNumber = invoiceitemsDto[i].DONumber;

                        //DO Balance qty and Invoiced qty update method
                        invoiceQty = await DoItemBalanceQtyUpdateBasedOnInvoiceQty(invoiceChildItem, invoiceQty, doNumber);

                        //Add inventory Transaction Table
                        await InventoryTransactionSaveOnInvoiceCreate(invoice, invoiceChildItemsEntityList, i, invoiceChildItem);

                    }
                }

                invoice.invoiceChildItems = invoiceChildItemsEntityList;
                invoice.InvoiceAdditionalCharges = InvoiceAdditionalChargesList.ToList();

                await _invoiceRepository.CreateInvoice(invoice);
                _invoiceRepository.SaveAsync();


                //Sales order additional charge update method
                await SoAdditonalChargeUpdateOnInvoiceCreate(InvoiceAdditionalChargesList);

                serviceResponse.Data = null;
                serviceResponse.Message = "Invoice Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetInvoiceById", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateInvoice action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task SoAdditonalChargeUpdateOnInvoiceCreate(IEnumerable<InvoiceAdditionalCharges> InvoiceAdditionalChargesList)
        {
            List<SalesOrderAdditionalChargesUpdate> salesOrderAdditionalChargesUpdates = new List<SalesOrderAdditionalChargesUpdate>();

            foreach (var additionalChargeItem in InvoiceAdditionalChargesList)
            {
                SalesOrderAdditionalChargesUpdate additionalCharges = new SalesOrderAdditionalChargesUpdate
                {
                    SalesOrderId = Convert.ToInt32(additionalChargeItem.SalesOrderId),
                    InvoicedValue = Convert.ToDecimal(additionalChargeItem.InvoicedValue),
                    SalesAdditionalChargeId = additionalChargeItem.SalesAdditionalChargeId
                };
                salesOrderAdditionalChargesUpdates.Add(additionalCharges);
            }




            var soAdditionalChargeJson = JsonConvert.SerializeObject(salesOrderAdditionalChargesUpdates);
            var data = new StringContent(soAdditionalChargeJson, Encoding.UTF8, "application/json");
            // Include the token in the Authorization header
            var tokenValues = _httpContextAccessor?.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(tokenValues) && tokenValues.StartsWith("Bearer "))
            {
                var token = tokenValues.Substring(7);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "AdditionalChargeUpdateFromInvoice"), data);
        }

        private async Task InventoryTransactionSaveOnInvoiceCreate(Invoice invoice, List<InvoiceChildItem> invoiceChildItemsEntityList, int i, InvoiceChildItem invoiceChildItem)
        {
            InventoryTranction inventoryTranction = new InventoryTranction();
            inventoryTranction.PartNumber = invoiceChildItemsEntityList[i].FGItemNumber;
            inventoryTranction.MftrPartNumber = invoiceChildItemsEntityList[i].FGItemNumber;
            inventoryTranction.Description = "";
            inventoryTranction.Issued_Quantity = invoiceChildItem.InvoicedQty;
            inventoryTranction.UOM = invoiceChildItemsEntityList[i].UOM;
            inventoryTranction.Issued_DateTime = DateTime.Now;
            inventoryTranction.ReferenceID = invoice.InvoiceNumber;
            inventoryTranction.ReferenceIDFrom = "Invoice Delivery Order";
            inventoryTranction.Issued_By = _createdBy;
            inventoryTranction.CreatedBy = _createdBy;
            inventoryTranction.CreatedOn = DateTime.Now;
            inventoryTranction.From_Location = "BTO";
            inventoryTranction.TO_Location = "Invoice";
            inventoryTranction.Warehouse = "Invoice";
            inventoryTranction.Remarks = "Create - Invoice";
            inventoryTranction.PartType = PartType.FG;
            var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);


            await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransactions);
            _inventoryTranctionRepository.SaveAsync();
        }

        private async Task<decimal> DoItemBalanceQtyUpdateBasedOnInvoiceQty(InvoiceChildItem invoiceChildItem, decimal invoiceQty, string? doNumber)
        {
            var btoItemDetails = await _bTODeliveryOrderItemsRepository.GetOpenDoItemDetailsByItemNoAndDoNo(invoiceChildItem.FGItemNumber, doNumber);

            if (btoItemDetails != null)
            {
                foreach (var doItem in btoItemDetails)
                {
                    decimal doBalanceQty = Convert.ToDecimal(doItem.BalanceDoQty);
                    doItem.InvoicedQty += invoiceQty;

                    if (doBalanceQty >= invoiceQty)
                    {
                        doItem.BalanceDoQty -= invoiceQty;
                        invoiceQty = 0;
                    }
                    else
                    {
                        doItem.BalanceDoQty = 0;
                        invoiceQty -= doBalanceQty;
                    }

                    if (doItem.BalanceDoQty <= 0)
                    {
                        doItem.BalanceDoQty = 0;
                        doItem.DoStatus = Status.Closed;
                    }

                    await _bTODeliveryOrderItemsRepository.UpdateBtoDelieveryOrderItem(doItem);

                    if (invoiceQty == 0)
                    {
                        break;
                    }
                }
            }

            //var getAllInvoicesList = await _bTODeliveryOrderItemsRepository.UpdateBtoDelieveryOrderBalanceQty(invoiceChildItem.FGItemNumber, doNumber, invoiceChildItem.InvoicedQty);
            _bTODeliveryOrderItemsRepository.SaveAsync();
            return invoiceQty;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] InvoiceUpdateDto invoiceUpdateDto)
        {
            ServiceResponse<InvoiceUpdateDto> serviceResponse = new ServiceResponse<InvoiceUpdateDto>();

            try
            {
                if (invoiceUpdateDto is null)
                {
                    _logger.LogError("Invoice object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Invoice object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Invoice object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var getinvoiceDetailById = await _invoiceRepository.GetInvoiceById(id);
                if (getinvoiceDetailById is null)
                {
                    _logger.LogError($"Invoice with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }

                var updateInvoice = _mapper.Map(invoiceUpdateDto, getinvoiceDetailById);

                string result = await _invoiceRepository.UpdateInvoice(updateInvoice);
                _logger.LogInfo(result);
                _invoiceRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Invoice Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateInvoice action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            ServiceResponse<InvoiceDto> serviceResponse = new ServiceResponse<InvoiceDto>();

            try
            {
                var deleteInvoiceDetail = await _invoiceRepository.GetInvoiceById(id);
                if (deleteInvoiceDetail == null)
                {
                    _logger.LogError($"Delete Invoice with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Invoice with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _invoiceRepository.DeleteInvoice(deleteInvoiceDetail);
                _logger.LogInfo(result);
                _invoiceRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Invoice Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteInvoice action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllInvoiceIdNameList()
        {
            ServiceResponse<IEnumerable<InvoiceIdNameList>> serviceResponse = new ServiceResponse<IEnumerable<InvoiceIdNameList>>();
            try
            {
                var listOfInvoiceIdNames = await _invoiceRepository.GetAllInvoiceIdNameList();
                var result = _mapper.Map<IEnumerable<InvoiceIdNameList>>(listOfInvoiceIdNames);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All listOfInvoiceIdNames";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllInvoiceIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}

