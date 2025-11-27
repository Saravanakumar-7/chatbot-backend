using AutoMapper;
using Azure;
using Contracts;
using Entities;
using Entities.DTOs;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Text;
using MimeKit;
using Mysqlx.Crud;
using Newtonsoft.Json;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Repository;
using EmailTemplateDto = Tips.Grin.Api.Entities.DTOs.EmailTemplateDto;
using System.Security.Claims;
using Entities.Enums;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class IQCConfirmationController : ControllerBase
    {
        private IIQCConfirmationRepository _iQCConfirmationRepository;
        private IIQCConfirmationItemsRepository _iQCConfirmationItemsRepository;
        private IGrinPartsRepository _grinPartsRepository;
        private IGrinRepository _grinRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;

        public IQCConfirmationController(IGrinRepository grinRepository, IIQCConfirmationRepository iQCConfirmationRepository, IHttpClientFactory clientFactory,
            IIQCConfirmationItemsRepository iQCConfirmationItemsRepository, IGrinPartsRepository grinPartsRepository,
            ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _iQCConfirmationRepository = iQCConfirmationRepository;
            _iQCConfirmationItemsRepository = iQCConfirmationItemsRepository;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _grinPartsRepository = grinPartsRepository;
            _grinRepository = grinRepository;
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

            [HttpGet]
            public async Task<IActionResult> GetAllIqcDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
            {
                ServiceResponse<IEnumerable<IQCConfirmationDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationDto>>();

                try
                {
                    var getAllIQCDetails = await _iQCConfirmationRepository.GetAllIqcDetails(pagingParameter, searchParams);

                    var metadata = new
                    {
                        getAllIQCDetails.TotalCount,
                        getAllIQCDetails.PageSize,
                        getAllIQCDetails.CurrentPage,
                        getAllIQCDetails.HasNext,
                        getAllIQCDetails.HasPreviuos
                    };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all IQCConfirmation details()s");
                var result = _mapper.Map<IEnumerable<IQCConfirmationDto>>(getAllIQCDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all IQCConfirmation";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllIqcDetails API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllIqcDetails API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetIQCConfirmationSPReportWithParam([FromBody] IQCConfirmationReportWithParamDto iQCConfirmationReportWithParamDto)
        {
            ServiceResponse<IEnumerable<IQCConfirmation_SPReport>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmation_SPReport>>();
            try
            {
                var products = await _iQCConfirmationRepository.GetIQCConfirmationSPReportWithParam(iQCConfirmationReportWithParamDto.GrinNumber,
                                                                                                                    iQCConfirmationReportWithParamDto.ItemNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCConfirmation hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"IQCConfirmation hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned IQCConfirmation Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetIQCConfirmationSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetIQCConfirmationSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ExportIQCConfirmationSPReportWithParamOrDateToExcel([FromBody] IQCConfirmationReportWithParamAndDateDto iqcconfirmationReportWithParamAndDateDto)
        {

            ServiceResponse<IQCConfirmation_SPReport> serviceResponse = new ServiceResponse<IQCConfirmation_SPReport>();
            try
            {

                if (iqcconfirmationReportWithParamAndDateDto is null)
                {
                    _logger.LogError("iqcconfirmationReportWithParamAndDateDto object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "iqcconfirmationReportWithParamAndDateDto object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid iqcconfirmationReportWithParamAndDateDto object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                bool hasParams = !string.IsNullOrEmpty(iqcconfirmationReportWithParamAndDateDto.GrinNumber)
                               || !string.IsNullOrEmpty(iqcconfirmationReportWithParamAndDateDto.ItemNumber);

                bool hasDate = iqcconfirmationReportWithParamAndDateDto.FromDate != null || iqcconfirmationReportWithParamAndDateDto.ToDate != null;

                if (hasParams && hasDate)
                {
                    _logger.LogError("Input object must contain either filter parameters or date range, not both.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or a date range, not both.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!hasParams && (iqcconfirmationReportWithParamAndDateDto.FromDate == null || iqcconfirmationReportWithParamAndDateDto.ToDate == null))
                {
                    _logger.LogError("Input object must contain either at least one filter parameter or both from and to dates.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or both from and to dates.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);

                }

                var IQCConfirmationItemsList = Enumerable.Empty<IQCConfirmation_SPReport>();

                if (hasParams && !hasDate)
                {

                    IQCConfirmationItemsList = await _iQCConfirmationRepository.GetIQCConfirmationSPReportWithParam(
                        iqcconfirmationReportWithParamAndDateDto.GrinNumber,
                        iqcconfirmationReportWithParamAndDateDto.ItemNumber);
                }


                if (!hasParams && (iqcconfirmationReportWithParamAndDateDto.FromDate != null && iqcconfirmationReportWithParamAndDateDto.ToDate != null))
                {

                    IQCConfirmationItemsList = await _iQCConfirmationRepository.GetIQCConfirmationSPReportWithDate(
                        iqcconfirmationReportWithParamAndDateDto.FromDate,
                        iqcconfirmationReportWithParamAndDateDto.ToDate);
                }



                // Create Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("IQCConfirmation");

                // Header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("GRIN Number");
                headerRow.CreateCell(1).SetCellValue("Vendor Name");
                headerRow.CreateCell(2).SetCellValue("Invoice Number");
                headerRow.CreateCell(3).SetCellValue("Invoice Date");
                headerRow.CreateCell(4).SetCellValue("Invoice Value");
                headerRow.CreateCell(5).SetCellValue("PO Number");
                headerRow.CreateCell(6).SetCellValue("Item Description");
                headerRow.CreateCell(7).SetCellValue("Mftr Item Number");
                headerRow.CreateCell(8).SetCellValue("Manufacture Batch Number");
                headerRow.CreateCell(9).SetCellValue("Unit Price");
                headerRow.CreateCell(10).SetCellValue("Project Number");
                headerRow.CreateCell(11).SetCellValue("Project Qty");
                headerRow.CreateCell(12).SetCellValue("UOM");
                headerRow.CreateCell(13).SetCellValue("Expiry Date");
                headerRow.CreateCell(14).SetCellValue("Manufacture Date");
                headerRow.CreateCell(15).SetCellValue("Received Qty");
                headerRow.CreateCell(16).SetCellValue("Accepted Qty");
                headerRow.CreateCell(17).SetCellValue("Rejected Qty");
                headerRow.CreateCell(18).SetCellValue("Remarks");
                headerRow.CreateCell(19).SetCellValue("Total Invoice Value");
                headerRow.CreateCell(20).SetCellValue("AWB Number 1");
                headerRow.CreateCell(21).SetCellValue("AWB Date 1");

                // Populate data
                int rowIndex = 1;
                foreach (var item in IQCConfirmationItemsList)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.GrinNumber ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.InvoiceNumber ?? "");
                    row.CreateCell(3).SetCellValue(item.InvoiceDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(4).SetCellValue((double)(item.InvoiceValue ?? 0));
                    row.CreateCell(5).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(6).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(7).SetCellValue(item.MftrItemNumber ?? "");
                    row.CreateCell(8).SetCellValue(item.ManufactureBatchNumber ?? "");
                    row.CreateCell(9).SetCellValue((double)(item.UnitPrice ?? 0));
                    row.CreateCell(10).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(11).SetCellValue((double)(item.ProjectQty ?? 0));
                    row.CreateCell(12).SetCellValue(item.UOM ?? "");
                    row.CreateCell(13).SetCellValue(item.ExpiryDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(14).SetCellValue(item.ManufactureDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(15).SetCellValue((double)(item.ReceivedQty ?? 0));
                    row.CreateCell(16).SetCellValue((double)(item.AcceptedQty ?? 0));
                    row.CreateCell(17).SetCellValue((double)(item.RejectedQty ?? 0));
                    row.CreateCell(18).SetCellValue(item.Remarks ?? "");
                    row.CreateCell(19).SetCellValue((double)(item.TotalInvoiceValue ?? 0));
                    row.CreateCell(20).SetCellValue(item.AWBNumber1 ?? "");
                    row.CreateCell(21).SetCellValue(item.AWBDate1?.ToString("MM/dd/yyyy") ?? "");
                }



                // Export Excel
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();
                    return File(excelBytes,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "IQCConfirmation.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetIQCConfirmationSPReportWithParamForTrans([FromBody] IQCConfirmationReportWithParamForTransDto iQCConfirmationReportWithParamDto)
        {
            ServiceResponse<IEnumerable<IQCConfirmationSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationSPReportForTrans>>();
            try
            {
                var products = await _iQCConfirmationRepository.GetIQCConfirmationSPReportWithParamForTrans(iQCConfirmationReportWithParamDto.GrinNumber,
                                                                                                                    iQCConfirmationReportWithParamDto.ItemNumber,
                                                                                                                    iQCConfirmationReportWithParamDto.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCConfirmation hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"IQCConfirmation hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned IQCConfirmationSPReportWithParamForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetIQCConfirmationSPReportWithParamForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetIQCConfirmationSPReportWithParamForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportIQCConfirmationSPReportWithParamOrDateToExcelForTras([FromBody] IQCConfirmationReportWithParamAndDateForTransDto iqcconfirmationReportWithParamAndDateForTransDto)
        {

            ServiceResponse<IQCConfirmationSPReportForTrans> serviceResponse = new ServiceResponse<IQCConfirmationSPReportForTrans>();
            try
            {

                if (iqcconfirmationReportWithParamAndDateForTransDto is null)
                {
                    _logger.LogError("iqcconfirmationReportWithParamAndDateForTransDto object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "iqcconfirmationReportWithParamAndDateForTransDto object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid iqcconfirmationReportWithParamAndDateForTransDto object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                bool hasParams = !string.IsNullOrEmpty(iqcconfirmationReportWithParamAndDateForTransDto.GrinNumber)
                               || !string.IsNullOrEmpty(iqcconfirmationReportWithParamAndDateForTransDto.ItemNumber)
                               || !string.IsNullOrEmpty(iqcconfirmationReportWithParamAndDateForTransDto.ProjectNumber);

                bool hasDate = iqcconfirmationReportWithParamAndDateForTransDto.FromDate != null || iqcconfirmationReportWithParamAndDateForTransDto.ToDate != null;

                if (hasParams && hasDate)
                {
                    _logger.LogError("Input object must contain either filter parameters or date range, not both.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or a date range, not both.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!hasParams && (iqcconfirmationReportWithParamAndDateForTransDto.FromDate == null || iqcconfirmationReportWithParamAndDateForTransDto.ToDate == null))
                {
                    _logger.LogError("Input object must contain either at least one filter parameter or both from and to dates.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or both from and to dates.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);

                }

                var IQCConfirmationItemsList = Enumerable.Empty<IQCConfirmationSPReportForTrans>();

                if (hasParams && !hasDate)
                {

                    IQCConfirmationItemsList = await _iQCConfirmationRepository.GetIQCConfirmationSPReportWithParamForTrans(
                        iqcconfirmationReportWithParamAndDateForTransDto.GrinNumber,
                        iqcconfirmationReportWithParamAndDateForTransDto.ItemNumber, iqcconfirmationReportWithParamAndDateForTransDto.ProjectNumber);
                }


                if (!hasParams && (iqcconfirmationReportWithParamAndDateForTransDto.FromDate != null && iqcconfirmationReportWithParamAndDateForTransDto.ToDate != null))
                {

                    IQCConfirmationItemsList = await _iQCConfirmationRepository.GetIQCConfirmationSPReportWithDateForTrans(
                        iqcconfirmationReportWithParamAndDateForTransDto.FromDate,
                        iqcconfirmationReportWithParamAndDateForTransDto.ToDate);
                }



                // Create Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("IQCConfirmation");

                // Header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("GRIN Number");
                headerRow.CreateCell(1).SetCellValue("Vendor Name");
                headerRow.CreateCell(2).SetCellValue("Invoice Number");
                headerRow.CreateCell(3).SetCellValue("IQC Cleared Date");
                headerRow.CreateCell(4).SetCellValue("Invoice Date");
                headerRow.CreateCell(5).SetCellValue("Invoice Value");
                headerRow.CreateCell(6).SetCellValue("PO Number");
                headerRow.CreateCell(7).SetCellValue("Item Description");
                headerRow.CreateCell(8).SetCellValue("GRIN Part No");
                headerRow.CreateCell(9).SetCellValue("Mftr Item Number");
                headerRow.CreateCell(10).SetCellValue("Manufacture Batch Number");
                headerRow.CreateCell(11).SetCellValue("Lot Number");
                headerRow.CreateCell(12).SetCellValue("Unit Price");
                headerRow.CreateCell(13).SetCellValue("Project Number");
                headerRow.CreateCell(14).SetCellValue("Project Qty");
                headerRow.CreateCell(15).SetCellValue("UOM");
                headerRow.CreateCell(16).SetCellValue("Expiry Date");
                headerRow.CreateCell(17).SetCellValue("GRIN Qty");
                headerRow.CreateCell(18).SetCellValue("Manufacture Date");
                headerRow.CreateCell(19).SetCellValue("Accepted Qty");
                headerRow.CreateCell(20).SetCellValue("Rejected Qty");
                headerRow.CreateCell(21).SetCellValue("Status");
                headerRow.CreateCell(22).SetCellValue("Remarks");
                headerRow.CreateCell(23).SetCellValue("Total Invoice Value");
                headerRow.CreateCell(24).SetCellValue("AWB Number 1");
                headerRow.CreateCell(25).SetCellValue("AWB Date 1");
                headerRow.CreateCell(26).SetCellValue("Received Qty");

                // Populate data
                int rowIndex = 1;
                foreach (var item in IQCConfirmationItemsList)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.GrinNumber ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.InvoiceNumber ?? "");
                    row.CreateCell(3).SetCellValue(item.IqcClearedDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(4).SetCellValue(item.InvoiceDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(5).SetCellValue((double)(item.InvoiceValue ?? 0));
                    row.CreateCell(6).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(7).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(8).SetCellValue(item.GrinPartNo ?? "");
                    row.CreateCell(9).SetCellValue(item.MftrItemNumber ?? "");
                    row.CreateCell(10).SetCellValue(item.ManufactureBatchNumber ?? "");
                    row.CreateCell(11).SetCellValue(item.LotNumber ?? "");
                    row.CreateCell(12).SetCellValue((double)(item.UnitPrice ?? 0));
                    row.CreateCell(13).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(14).SetCellValue((double)(item.ProjectQty ?? 0));
                    row.CreateCell(15).SetCellValue(item.UOM ?? "");
                    row.CreateCell(16).SetCellValue(item.ExpiryDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(17).SetCellValue((double)(item.GrinQty ?? 0));
                    row.CreateCell(18).SetCellValue(item.ManufactureDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(19).SetCellValue((double)(item.AcceptedQty ?? 0));
                    row.CreateCell(20).SetCellValue((double)(item.RejectedQty ?? 0));
                    row.CreateCell(21).SetCellValue(item.Status?.ToString() ?? "");
                    row.CreateCell(22).SetCellValue(item.Remarks ?? "");
                    row.CreateCell(23).SetCellValue((double)(item.TotalInvoiceValue ?? 0));
                    row.CreateCell(24).SetCellValue(item.AWBNumber1 ?? "");
                    row.CreateCell(25).SetCellValue(item.AWBDate1?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(26).SetCellValue((double)(item.ReceivedQty ?? 0));
                }




                // Export Excel
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();
                    return File(excelBytes,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "IQC Confirmations.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetIQCPendingSPReportWithParamForTrans([FromBody] IQCPendingReportWithParamForTransDto iQCPendingReportWithParamForTransDto)
        {
            ServiceResponse<IEnumerable<IQCPendingReportWithParamForTrans>> serviceResponse = new ServiceResponse<IEnumerable<IQCPendingReportWithParamForTrans>>();
            try
            {
                var products = await _iQCConfirmationRepository.GetIQCPendingSPReportWithParamForTrans(iQCPendingReportWithParamForTransDto.GrinNumber,
                                                                                                                    iQCPendingReportWithParamForTransDto.VendorName,
                                                                                                                    iQCPendingReportWithParamForTransDto.PONumber,
                                                                                                                    iQCPendingReportWithParamForTransDto.ItemNumber,
                                                                                                                    iQCPendingReportWithParamForTransDto.MPN,
                                                                                                                    iQCPendingReportWithParamForTransDto.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCPending hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"IQCPending hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned IQCPendingSPReportWithParamForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetIQCPendingSPReportWithParamForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetIQCPendingSPReportWithParamForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetIQCConfirmationSPReportForTrans([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<IQCConfirmationSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationSPReportForTrans>>();
            try
            {
                var products = await _iQCConfirmationRepository.GetIQCConfirmationSPReportForTrans(pagingParameter);

                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.HasNext,
                    products.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all GetGrinSPReport");

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCConfirmation hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"IQCConfirmation hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned IQCConfirmationSPReportForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetIQCConfirmationSPReportForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetIQCConfirmationSPReportForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetIQCConfirmationSPReportWithParamForAvi([FromBody] IQCConfirmationReportWithParamForTransDto iQCConfirmationReportWithParamDto)
        {
            ServiceResponse<IEnumerable<IQCConfirmationSPReportForAvi>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationSPReportForAvi>>();
            try
            {
                var products = await _iQCConfirmationRepository.GetIQCConfirmationSPReportWithParamForAvi(iQCConfirmationReportWithParamDto.GrinNumber,
                                                                                                                    iQCConfirmationReportWithParamDto.ItemNumber,
                                                                                                                    iQCConfirmationReportWithParamDto.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCConfirmationForAvi hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"IQCConfirmationForAvi hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned IQCConfirmationSPReportWithParamForAvi Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetIQCConfirmationSPReportWithParamForAvi API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetIQCConfirmationSPReportWithParamForAvi API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }




        [HttpPost]
        public async Task<IActionResult> ExportIQCConfirmationSPReportWithParamOrDateToExcelForAvi([FromBody] IQCConfirmationReportWithParamAndDateForAviDto iqcconfirmationReportWithParamAndDateForAviDto)
        {

            ServiceResponse<IQCConfirmationSPReportForAvi> serviceResponse = new ServiceResponse<IQCConfirmationSPReportForAvi>();
            try
            {

                if (iqcconfirmationReportWithParamAndDateForAviDto is null)
                {
                    _logger.LogError("iqcconfirmationReportWithParamAndDateForAviDto object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "iqcconfirmationReportWithParamAndDateForAviDto object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid iqcconfirmationReportWithParamAndDateForAviDto object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                bool hasParams = !string.IsNullOrEmpty(iqcconfirmationReportWithParamAndDateForAviDto.GrinNumber)
                               || !string.IsNullOrEmpty(iqcconfirmationReportWithParamAndDateForAviDto.ItemNumber);

                bool hasDate = iqcconfirmationReportWithParamAndDateForAviDto.FromDate != null || iqcconfirmationReportWithParamAndDateForAviDto.ToDate != null;

                if (hasParams && hasDate)
                {
                    _logger.LogError("Input object must contain either filter parameters or date range, not both.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or a date range, not both.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!hasParams && (iqcconfirmationReportWithParamAndDateForAviDto.FromDate == null || iqcconfirmationReportWithParamAndDateForAviDto.ToDate == null))
                {
                    _logger.LogError("Input object must contain either at least one filter parameter or both from and to dates.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or both from and to dates.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);

                }

                var IQCConfirmationItemsList = Enumerable.Empty<IQCConfirmationSPReportForAvi>();

                if (hasParams && !hasDate)
                {

                    IQCConfirmationItemsList = await _iQCConfirmationRepository.GetIQCConfirmationSPReportWithParamForAvi(
                        iqcconfirmationReportWithParamAndDateForAviDto.GrinNumber,
                        iqcconfirmationReportWithParamAndDateForAviDto.ItemNumber, iqcconfirmationReportWithParamAndDateForAviDto.ProjectNumber);
                }


                if (!hasParams && (iqcconfirmationReportWithParamAndDateForAviDto.FromDate != null && iqcconfirmationReportWithParamAndDateForAviDto.ToDate != null))
                {

                    IQCConfirmationItemsList = await _iQCConfirmationRepository.GetIQCConfirmationSPReportWithDateForAvi(
                        iqcconfirmationReportWithParamAndDateForAviDto.FromDate,
                        iqcconfirmationReportWithParamAndDateForAviDto.ToDate);
                }



                // Create Excel workbook
                // Create Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("IQCConfirmationReport");

                // Header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("GRIN Number");
                headerRow.CreateCell(1).SetCellValue("GRIN Cleared Date");
                headerRow.CreateCell(2).SetCellValue("Vendor Name");
                headerRow.CreateCell(3).SetCellValue("Invoice Number");
                headerRow.CreateCell(4).SetCellValue("Invoice Date");
                headerRow.CreateCell(5).SetCellValue("Invoice Value");
                headerRow.CreateCell(6).SetCellValue("PO Number");
                headerRow.CreateCell(7).SetCellValue("Item Description");
                headerRow.CreateCell(8).SetCellValue("Mftr Item Number");
                headerRow.CreateCell(9).SetCellValue("Manufacture Batch Number");
                headerRow.CreateCell(10).SetCellValue("Lot Number");
                headerRow.CreateCell(11).SetCellValue("Unit Price");
                headerRow.CreateCell(12).SetCellValue("Project Number");
                headerRow.CreateCell(13).SetCellValue("Project Qty");
                headerRow.CreateCell(14).SetCellValue("UOM");
                headerRow.CreateCell(15).SetCellValue("Expiry Date");
                headerRow.CreateCell(16).SetCellValue("GRIN Qty");
                headerRow.CreateCell(17).SetCellValue("Manufacture Date");
                headerRow.CreateCell(18).SetCellValue("Received Qty");
                headerRow.CreateCell(19).SetCellValue("Accepted Qty");
                headerRow.CreateCell(20).SetCellValue("Rejected Qty");
                headerRow.CreateCell(21).SetCellValue("Remarks");
                headerRow.CreateCell(22).SetCellValue("IQC Cleared Date");
                headerRow.CreateCell(23).SetCellValue("Total Invoice Value");
                headerRow.CreateCell(24).SetCellValue("AWB Number 1");
                headerRow.CreateCell(25).SetCellValue("AWB Date 1");

                // Populate data
                int rowIndex = 1;
                foreach (var item in IQCConfirmationItemsList)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.GrinNumber ?? "");
                    row.CreateCell(1).SetCellValue(item.GrinClearedDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(2).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(3).SetCellValue(item.InvoiceNumber ?? "");
                    row.CreateCell(4).SetCellValue(item.InvoiceDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(5).SetCellValue((double)(item.InvoiceValue ?? 0));
                    row.CreateCell(6).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(7).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(8).SetCellValue(item.MftrItemNumber ?? "");
                    row.CreateCell(9).SetCellValue(item.ManufactureBatchNumber ?? "");
                    row.CreateCell(10).SetCellValue(item.LotNumber ?? "");
                    row.CreateCell(11).SetCellValue((double)(item.UnitPrice ?? 0));
                    row.CreateCell(12).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(13).SetCellValue((double)(item.ProjectQty ?? 0));
                    row.CreateCell(14).SetCellValue(item.UOM ?? "");
                    row.CreateCell(15).SetCellValue(item.ExpiryDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(16).SetCellValue((double)(item.GrinQty ?? 0));
                    row.CreateCell(17).SetCellValue(item.ManufactureDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(18).SetCellValue((double)(item.ReceivedQty ?? 0));
                    row.CreateCell(19).SetCellValue((double)(item.AcceptedQty ?? 0));
                    row.CreateCell(20).SetCellValue((double)(item.RejectedQty ?? 0));
                    row.CreateCell(21).SetCellValue(item.Remarks ?? "");
                    row.CreateCell(22).SetCellValue(item.IqcClearedDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(23).SetCellValue((double)(item.TotalInvoiceValue ?? 0));
                    row.CreateCell(24).SetCellValue(item.AWBNumber1 ?? "");
                    row.CreateCell(25).SetCellValue(item.AWBDate1?.ToString("MM/dd/yyyy") ?? "");
                }



                // Export Excel
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();
                    return File(excelBytes,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "IQCConfirmationReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }








        [HttpGet]
        public async Task<IActionResult> GetIQCConfirmationSPReport([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<IQCConfirmation_SPReport>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmation_SPReport>>();
            try
            {
                var products = await _iQCConfirmationRepository.GetIQCConfirmationSPReport(pagingParameter);

                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.HasNext,
                    products.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all GetGrinSPReport");

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCConfirmation hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"IQCConfirmation hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned IQCConfirmation Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetIQCConfirmationSPReport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetIQCConfirmationSPReport API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetIQCConfirmationSPReportWithDateForTrans([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<IQCConfirmationSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationSPReportForTrans>>();
            try
            {
                var products = await _iQCConfirmationRepository.GetIQCConfirmationSPReportWithDateForTrans(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCConfirmation hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"IQCConfirmation hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned IQCConfirmationSPReportWithDateForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetIQCConfirmationSPReportWithDateForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetIQCConfirmationSPReportWithDateForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetIQCConfirmationSPReportWithDateForAvi([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<IQCConfirmationSPReportForAvi>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationSPReportForAvi>>();
            try
            {
                var products = await _iQCConfirmationRepository.GetIQCConfirmationSPReportWithDateForAvi(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCConfirmation hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"IQCConfirmation hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned IQCConfirmationSPReportWithDateForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetIQCConfirmationSPReportWithDateForAvi API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetIQCConfirmationSPReportWithDateForAvi API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetIQCConfirmationSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<IQCConfirmation_SPReport>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmation_SPReport>>();
            try
            {
                var products = await _iQCConfirmationRepository.GetIQCConfirmationSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCConfirmation hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"IQCConfirmation hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned IQCConfirmation Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetIQCConfirmationSPReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetIQCConfirmationSPReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchIQCConfirmationDate([FromQuery] SearchDateParames searchDateParam)
        {
            ServiceResponse<IEnumerable<IQCConfirmationReportDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationReportDto>>();
            try
            {
                //var searchDateParamIQC = await _iQCConfirmationRepository.SearchIQCConfirmationDate(searchDateParam);

                //var result = _mapper.Map<IEnumerable<IQCConfirmationDto>>(searchDateParamIQC);

                var iQCList = await _iQCConfirmationRepository.SearchIQCConfirmationDate(searchDateParam);

                // Get all the unique GrinIds from the iQCList
                var grinIds = iQCList
                    .Select(iqc => iqc.GrinId)
                    .Distinct()
                    .ToList();

                // Get all the unique GrinPartIds from the iQCList
                var grinPartIds = iQCList
                    .SelectMany(iqc => iqc.IQCConfirmationItems.Select(item => item.GrinPartId))
                    .Distinct()
                    .ToList();

                // Fetch all the required Grin details in a single query and store them in a dictionary
                var grinDetails = await _grinRepository.GetGrinDetailsByGrinIds(grinIds);
                var grinDetailsLookup = grinDetails.ToDictionary(gp => gp.Id, gp => gp);

                // Fetch all the required GrinPart details in a single query and store them in a dictionary
                var grinPartDetails = await _grinPartsRepository.GetGrinPartsDetailsByGrinPartIds(grinPartIds);
                var grinPartDetailsLookup = grinPartDetails.ToDictionary(gp => gp.Id, gp => gp);

                // Use the grinPartDetailsLookup for quick lookups while mapping the data to DTO objects
                var iqcListDto = iQCList.Select(iqc => new IQCConfirmationReportDto
                {
                    // Map IQCConfirmation properties here (assuming properties with the same name exist in the DTO)
                    Id = iqc.Id,
                    GrinId = iqc.GrinId,
                    GrinNumber = iqc.GrinNumber,
                    Unit = iqc.Unit,
                    CreatedBy = iqc.CreatedBy,
                    CreatedOn = iqc.CreatedOn,
                    LastModifiedBy = iqc.LastModifiedBy,
                    LastModifiedOn = iqc.LastModifiedOn,
                    VendorName = grinDetailsLookup[iqc.GrinId].VendorName,
                    VendorId = grinDetailsLookup[iqc.GrinId].VendorId,
                    InvoiceNumber = grinDetailsLookup[iqc.GrinId].InvoiceNumber,
                    InvoiceValue = grinDetailsLookup[iqc.GrinId].InvoiceValue,
                    InvoiceDate = grinDetailsLookup[iqc.GrinId].InvoiceDate,
                    AWBNumber1 = grinDetailsLookup[iqc.GrinId].AWBNumber1,
                    AWBDate1 = grinDetailsLookup[iqc.GrinId].AWBDate1,
                    AWBNumber2 = grinDetailsLookup[iqc.GrinId].AWBNumber2,
                    AWBDate2 = grinDetailsLookup[iqc.GrinId].AWBDate2,
                    BENumber = grinDetailsLookup[iqc.GrinId].BENumber,
                    BEDate = grinDetailsLookup[iqc.GrinId].BEDate,
                    TotalInvoiceValue = grinDetailsLookup[iqc.GrinId].TotalInvoiceValue,

                    IQCConfirmationItems = iqc.IQCConfirmationItems.Select(item => new IQCConfirmationItemsReportDto
                    {
                        // Map IQCConfirmationItem properties here (assuming properties with the same name exist in the DTO)
                        Id = item.Id,
                        GrinPartId = item.GrinPartId,
                        ItemNumber = item.ItemNumber,
                        ReceivedQty = item.ReceivedQty,
                        GrinNumber = iqc.GrinNumber,
                        Remarks = item.Remarks,
                        MftrItemNumber = grinPartDetailsLookup[item.GrinPartId].MftrItemNumber,
                        PONumber = grinPartDetailsLookup[item.GrinPartId].PONumber,
                        ItemDescription = grinPartDetailsLookup[item.GrinPartId].ItemDescription,
                        ManufactureBatchNumber = grinPartDetailsLookup[item.GrinPartId].ManufactureBatchNumber,
                        UOM = grinPartDetailsLookup[item.GrinPartId].UOM,
                        ExpiryDate = grinPartDetailsLookup[item.GrinPartId].ExpiryDate,
                        ManufactureDate = grinPartDetailsLookup[item.GrinPartId].ManufactureDate,

                    }).ToList(),
                }).ToList();

                serviceResponse.Data = iqcListDto;
                serviceResponse.Message = "Returned all IQCConfirmations";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in SearchIQCConfirmationDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SearchIQCConfirmationDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchIQCConfirmation([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<IQCConfirmationReportDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationReportDto>>();
            try
            {
                var iQCList = await _iQCConfirmationRepository.SearchIQCConfirmation(searchParams);

                // Get all the unique GrinIds from the iQCList
                var grinIds = iQCList
                    .Select(iqc => iqc.GrinId)
                    .Distinct()
                    .ToList();

                // Get all the unique GrinPartIds from the iQCList
                var grinPartIds = iQCList
                    .SelectMany(iqc => iqc.IQCConfirmationItems.Select(item => item.GrinPartId))
                    .Distinct()
                    .ToList();

                // Fetch all the required Grin details in a single query and store them in a dictionary
                var grinDetails = await _grinRepository.GetGrinDetailsByGrinIds(grinIds);
                var grinDetailsLookup = grinDetails.ToDictionary(gp => gp.Id, gp => gp);

                // Fetch all the required GrinPart details in a single query and store them in a dictionary
                var grinPartDetails = await _grinPartsRepository.GetGrinPartsDetailsByGrinPartIds(grinPartIds);
                var grinPartDetailsLookup = grinPartDetails.ToDictionary(gp => gp.Id, gp => gp);

                // Use the grinPartDetailsLookup for quick lookups while mapping the data to DTO objects
                var iqcListDto = iQCList.Select(iqc => new IQCConfirmationReportDto
                {
                    // Map IQCConfirmation properties here (assuming properties with the same name exist in the DTO)
                    Id = iqc.Id,
                    GrinId = iqc.GrinId,
                    GrinNumber = iqc.GrinNumber,
                    Unit = iqc.Unit,
                    CreatedBy = iqc.CreatedBy,
                    CreatedOn = iqc.CreatedOn,
                    LastModifiedBy = iqc.LastModifiedBy,
                    LastModifiedOn = iqc.LastModifiedOn,
                    VendorName = grinDetailsLookup[iqc.GrinId].VendorName,
                    VendorId = grinDetailsLookup[iqc.GrinId].VendorId,
                    InvoiceNumber = grinDetailsLookup[iqc.GrinId].InvoiceNumber,
                    InvoiceValue = grinDetailsLookup[iqc.GrinId].InvoiceValue,
                    InvoiceDate = grinDetailsLookup[iqc.GrinId].InvoiceDate,
                    AWBNumber1 = grinDetailsLookup[iqc.GrinId].AWBNumber1,
                    AWBDate1 = grinDetailsLookup[iqc.GrinId].AWBDate1,
                    AWBNumber2 = grinDetailsLookup[iqc.GrinId].AWBNumber2,
                    AWBDate2 = grinDetailsLookup[iqc.GrinId].AWBDate2,
                    BENumber = grinDetailsLookup[iqc.GrinId].BENumber,
                    BEDate = grinDetailsLookup[iqc.GrinId].BEDate,
                    TotalInvoiceValue = grinDetailsLookup[iqc.GrinId].TotalInvoiceValue,

                    IQCConfirmationItems = iqc.IQCConfirmationItems.Select(item => new IQCConfirmationItemsReportDto
                    {
                        // Map IQCConfirmationItem properties here (assuming properties with the same name exist in the DTO)
                        Id = item.Id,
                        GrinPartId = item.GrinPartId,
                        ItemNumber = item.ItemNumber,
                        ReceivedQty = item.ReceivedQty,
                        GrinNumber = iqc.GrinNumber,
                        Remarks = item.Remarks,
                        MftrItemNumber = grinPartDetailsLookup[item.GrinPartId].MftrItemNumber,
                        PONumber = grinPartDetailsLookup[item.GrinPartId].PONumber,
                        ItemDescription = grinPartDetailsLookup[item.GrinPartId].ItemDescription,
                        ManufactureBatchNumber = grinPartDetailsLookup[item.GrinPartId].ManufactureBatchNumber,
                        UOM = grinPartDetailsLookup[item.GrinPartId].UOM,
                        ExpiryDate = grinPartDetailsLookup[item.GrinPartId].ExpiryDate,
                        ManufactureDate = grinPartDetailsLookup[item.GrinPartId].ManufactureDate,

                    }).ToList(),
                }).ToList();

                _logger.LogInfo("Returned all IQCConfirmation");

                serviceResponse.Data = iqcListDto;
                serviceResponse.Message = "Returned all IQCConfirmation";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in SearchIQCConfirmation API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SearchIQCConfirmation API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetAllIQCConfirmationWithItems([FromBody] IQCConfirmationSearchDto iQCConfirmationSearch)
        {
            ServiceResponse<IEnumerable<IQCConfirmationReportDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationReportDto>>();
            try
            {
                var iQCList = await _iQCConfirmationRepository.GetAllIQCConfirmationWithItems(iQCConfirmationSearch);

                // Get all the unique GrinIds from the iQCList
                var grinIds = iQCList
                    .Select(iqc => iqc.GrinId)
                    .Distinct()
                    .ToList();

                // Get all the unique GrinPartIds from the iQCList
                var grinPartIds = iQCList
                    .SelectMany(iqc => iqc.IQCConfirmationItems.Select(item => item.GrinPartId))
                    .Distinct()
                    .ToList();

                // Fetch all the required Grin details in a single query and store them in a dictionary
                var grinDetails = await _grinRepository.GetGrinDetailsByGrinIds(grinIds);
                var grinDetailsLookup = grinDetails.ToDictionary(gp => gp.Id, gp => gp);

                // Fetch all the required GrinPart details in a single query and store them in a dictionary
                var grinPartDetails = await _grinPartsRepository.GetGrinPartsDetailsByGrinPartIds(grinPartIds);
                var grinPartDetailsLookup = grinPartDetails.ToDictionary(gp => gp.Id, gp => gp);

                // Use the grinPartDetailsLookup for quick lookups while mapping the data to DTO objects
                var iqcListDto = iQCList.Select(iqc => new IQCConfirmationReportDto
                {
                    // Map IQCConfirmation properties here (assuming properties with the same name exist in the DTO)
                    Id = iqc.Id,
                    GrinId = iqc.GrinId,
                    GrinNumber = iqc.GrinNumber,
                    Unit = iqc.Unit,
                    CreatedBy = iqc.CreatedBy,
                    CreatedOn = iqc.CreatedOn,
                    LastModifiedBy = iqc.LastModifiedBy,
                    LastModifiedOn = iqc.LastModifiedOn,
                    VendorName = grinDetailsLookup[iqc.GrinId].VendorName,
                    VendorId = grinDetailsLookup[iqc.GrinId].VendorId,
                    InvoiceNumber = grinDetailsLookup[iqc.GrinId].InvoiceNumber,
                    InvoiceValue = grinDetailsLookup[iqc.GrinId].InvoiceValue,
                    InvoiceDate = grinDetailsLookup[iqc.GrinId].InvoiceDate,
                    AWBNumber1 = grinDetailsLookup[iqc.GrinId].AWBNumber1,
                    AWBDate1 = grinDetailsLookup[iqc.GrinId].AWBDate1,
                    AWBNumber2 = grinDetailsLookup[iqc.GrinId].AWBNumber2,
                    AWBDate2 = grinDetailsLookup[iqc.GrinId].AWBDate2,
                    BENumber = grinDetailsLookup[iqc.GrinId].BENumber,
                    BEDate = grinDetailsLookup[iqc.GrinId].BEDate,
                    TotalInvoiceValue = grinDetailsLookup[iqc.GrinId].TotalInvoiceValue,

                    IQCConfirmationItems = iqc.IQCConfirmationItems.Select(item => new IQCConfirmationItemsReportDto
                    {
                        // Map IQCConfirmationItem properties here (assuming properties with the same name exist in the DTO)
                        Id = item.Id,
                        GrinPartId = item.GrinPartId,
                        ItemNumber = item.ItemNumber,
                        ReceivedQty = item.ReceivedQty,
                        GrinNumber = iqc.GrinNumber,
                        Remarks = item.Remarks,
                        MftrItemNumber = grinPartDetailsLookup[item.GrinPartId].MftrItemNumber,
                        PONumber = grinPartDetailsLookup[item.GrinPartId].PONumber,
                        ItemDescription = grinPartDetailsLookup[item.GrinPartId].ItemDescription,
                        ManufactureBatchNumber = grinPartDetailsLookup[item.GrinPartId].ManufactureBatchNumber,
                        UOM = grinPartDetailsLookup[item.GrinPartId].UOM,
                        ExpiryDate = grinPartDetailsLookup[item.GrinPartId].ExpiryDate,
                        ManufactureDate = grinPartDetailsLookup[item.GrinPartId].ManufactureDate,

                    }).ToList(),
                }).ToList();

                serviceResponse.Data = iqcListDto;
                serviceResponse.Message = "Returned all IQCConfirmation";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllIQCConfirmationWithItems API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllIQCConfirmationWithItems API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{grinNumber}")]
        public async Task<IActionResult> GetIqcDetailsbyGrinNo(string grinNumber)
        {
            ServiceResponse<IEnumerable<IQCConfirmationDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationDto>>();

            try
            {
                var iQCDetailsByGrinNo = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);
                if (iQCDetailsByGrinNo == null)
                {
                    _logger.LogError($"IQCConfirmation Details with GrinNumber: {grinNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCConfirmation Details with GrinNumber hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return BadRequest(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned IQCConfirmation Details with id: {grinNumber}");
                    var result = _mapper.Map<IEnumerable<IQCConfirmationDto>>(iQCDetailsByGrinNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Successfully Returned IQCConfirmationbyGrinNo";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Created("IQCDetailsByGrinNo", serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetIqcDetailsbyGrinNo API for the following grinNumber:{grinNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetIqcDetailsbyGrinNo API :\n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetRejectIQCDetails([FromQuery] string GrinNo)
        {
            ServiceResponse<RejectIQC> serviceResponse = new ServiceResponse<RejectIQC>();
            try
            {
                var iQCDetailsByGrinNo = await _grinRepository.GetGrinByGrinNo(GrinNo);
                var iQCDetails = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(GrinNo);
                if (iQCDetailsByGrinNo == null)
                {
                    _logger.LogError($"No Data Found in GetRejectIQCDetails for:{GrinNo}");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"No Data Found in GetRejectIQCDetails for:{GrinNo}";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    var rejectIQCDetailsList = iQCDetailsByGrinNo.GrinParts.Where(x => x.RejectedQty > 0 && x.RejectReturnQty < x.RejectedQty).GroupBy(item => item.PONumber)
                   .Select(group => new RejectIQCDetails
                   {
                       POnumber = group.Key, // PONumber is the group key
                       Items = group.Select(item => new RejectIQCItemDetails
                       {
                           ItemNumber = item.ItemNumber,
                           ItemDescription = item.ItemDescription,
                           MftrItemNumber = item.MftrItemNumber,
                           GrinPartId = item.Id,
                           UOM = item.UOM,
                           RemainingRejectedQty = item.RejectedQty - item.RejectReturnQty
                       }).ToList()
                   })
                    .ToList();

                    RejectIQC rejectIQC = new RejectIQC
                    {
                        IQCNumber = iQCDetails.IQCNumber,
                        GrinNumber = iQCDetailsByGrinNo.GrinNumber,
                        GrinId = iQCDetailsByGrinNo.Id,
                        VendorId = iQCDetailsByGrinNo.VendorId,
                        VendorName = iQCDetailsByGrinNo.VendorName,
                        VendorNumber = iQCDetailsByGrinNo.VendorNumber,
                        RejectIQCDetails = rejectIQCDetailsList
                    };
                    serviceResponse.Data = rejectIQC;
                    serviceResponse.Message = "IQCConfirmationById Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRejectIQCDetails API for the following GrinNo:{GrinNo} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRejectIQCDetails API for the following GrinNo:{GrinNo} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIqc(int id, [FromBody] IQCConfirmationUpdateDto IQCConfirmationUpdateDto)
        {
            ServiceResponse<IQCConfirmationUpdateDto> serviceResponse = new ServiceResponse<IQCConfirmationUpdateDto>();

            try
            {
                if (IQCConfirmationUpdateDto is null)
                {
                    _logger.LogError("IQCConfirmation details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update IQCConfirmation details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid IQCConfirmation details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var iQCUpdate = await _iQCConfirmationRepository.GetIqcDetailsbyId(id);
                if (iQCUpdate is null)
                {
                    _logger.LogError($"IQCConfirmation details with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }

                //var iQCConfirmationUpdate = _mapper.Map(IQCConfirmationUpdateDto, iQCUpdate);

                //string result = await _iQCConfirmationRepository.UpdateIqc(iQCConfirmationUpdate);


                var iqcItems = _mapper.Map<IEnumerable<IQCConfirmationItems>>(IQCConfirmationUpdateDto.IQCConfirmationItemsUpdateDtos);

                var iqcItemsList = _mapper.Map<IQCConfirmation>(IQCConfirmationUpdateDto);

                var iQCConfirmationItemsDtos = IQCConfirmationUpdateDto.IQCConfirmationItemsUpdateDtos;

                var IqcItemsList = new List<IQCConfirmationItems>();
                for (int i = 0; i < iQCConfirmationItemsDtos.Count; i++)
                {
                    IQCConfirmationItems iQCConfirmationItems = _mapper.Map<IQCConfirmationItems>(iQCConfirmationItemsDtos[i]);
                    IqcItemsList.Add(iQCConfirmationItems);


                }



                var data = _mapper.Map(IQCConfirmationUpdateDto, iQCUpdate);


                data.IQCConfirmationItems = iqcItems.ToList();

                string result = await _iQCConfirmationRepository.UpdateIqc(data);
                _logger.LogInfo(result);
                _iQCConfirmationRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "IQCConfirmation Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateIqc API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateIqc API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
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

        //create Iqc


        [HttpPost]
        public async Task<IActionResult> CreateIqc([FromBody] IQCConfirmationPostDto iQCConfirmationPostDto)
        {
            ServiceResponse<IQCConfirmationDto> serviceResponse = new ServiceResponse<IQCConfirmationDto>();

            try
            {
                string serverKey = GetServerKey();

                if (iQCConfirmationPostDto == null)
                {
                    _logger.LogError("IQCConfirmation details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCConfirmation details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid IQCConfirmation details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var iQCCreate = _mapper.Map<IQCConfirmation>(iQCConfirmationPostDto);
                var grinNumber = iQCCreate.GrinNumber;
                var iQCDto = iQCConfirmationPostDto.IQCConfirmationItemsPostDtos;

                var iQcItemNo = iQCDto[0].ItemNumber;
                var iQCItemList = new List<IQCConfirmationItems>();

                HttpStatusCode getItemmResp = HttpStatusCode.OK;
                HttpStatusCode createInvfromGrin = HttpStatusCode.OK;
                HttpStatusCode getInvdetailsGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInv = HttpStatusCode.OK;
                HttpStatusCode createInvTransfromGrin = HttpStatusCode.OK;
                HttpStatusCode createInvTransfromGrin1 = HttpStatusCode.OK;

                var existingIqcConfirmation = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);

                if (existingIqcConfirmation != null)
                {

                    for (int i = 0; i < iQCDto.Count; i++)
                    {
                        IQCConfirmationItems iQCConfirmationItems = _mapper.Map<IQCConfirmationItems>(iQCDto[i]);
                        iQCConfirmationItems.IQCConfirmationId = existingIqcConfirmation.Id;
                        var grinPartId = iQCDto[i].GrinPartId;
                        var grinPartsDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartId);
                        if (iQCDto[i].GrinPartId != grinPartsDetails.Id)
                        {
                            _logger.LogError($"Invalid Grin Part Id {grinPartId}");
                            serviceResponse.Data = null;
                            serviceResponse.Message = $"Invalid Grin Part Id {grinPartId}";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            return BadRequest(serviceResponse);
                        }
                        if (grinPartsDetails.Qty <= (iQCDto[i].AcceptedQty + iQCDto[i].RejectedQty))
                        {
                            grinPartsDetails.AcceptedQty = iQCDto[i].AcceptedQty;
                            grinPartsDetails.RejectedQty = iQCDto[i].RejectedQty;
                        }
                        else
                        {
                            _logger.LogError("Grinpart Quantity should not be lesser than accepted + rejected Quantity .");
                            serviceResponse.Data = null;
                            serviceResponse.Message = "Grinpart Quantity should not be lesser than accepted + rejected Quantity ";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            return BadRequest(serviceResponse);
                        }

                        //Updating IQC Status in IqcItem Level

                        iQCConfirmationItems.IsIqcCompleted = true;
                        await _iQCConfirmationItemsRepository.CreateIqcItem(iQCConfirmationItems);
                        _iQCConfirmationItemsRepository.SaveAsync();

                        ////Inventory Update Code

                        //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterEnggAPI"],
                        //     "GetItemMasterByItemNumber?", "&ItemNumber=", HttpUtility.UrlEncode(iQcItemNo)));
                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = iQCDto[i].ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request.Headers.Add("Authorization", token);

                        var itemMasterObjectResult = await client.SendAsync(request);
                        if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                            getItemmResp = itemMasterObjectResult.StatusCode;

                        var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                        dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                        dynamic itemMasterObject = itemMasterObjectData.data;

                        decimal acceptedQty = iQCDto[i].AcceptedQty;
                        decimal rejectedQty = iQCDto[i].RejectedQty;
                        foreach (var projectNo in grinPartsDetails.ProjectNumbers)
                        {
                            var client1 = _clientFactory.CreateClient();
                            var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                            var itemNo = iQCDto[i].ItemNumber;
                            var encodedItemNo = Uri.EscapeDataString(itemNo);
                            var grinNo = iQCCreate.GrinNumber;
                            var encodedgrinNo = Uri.EscapeDataString(grinNo);
                            var grinPartsIds = projectNo.GrinPartsId;
                            var projectNos = projectNo.ProjectNumber;
                            var encodedprojectNos = Uri.EscapeDataString(projectNos);
                            var lotNumber = grinPartsDetails.LotNumber;
                            if (string.IsNullOrWhiteSpace(lotNumber))
                            {
                                var errorMessage = $"Lot Number not found for Grin Part Id: {grinPartId}";

                                _logger.LogError(errorMessage);

                                serviceResponse.Data = null;
                                serviceResponse.Message = errorMessage;
                                serviceResponse.Success = false;
                                serviceResponse.StatusCode = HttpStatusCode.BadRequest;

                                return BadRequest(serviceResponse);
                            }
                            var encodedlotNumber = Uri.EscapeDataString(lotNumber);
                           

                            var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                $"GetInventoryDetailsByProjectNoandLotNo?ProjectNumber={encodedprojectNos}&LotNumber={encodedlotNumber}"));
                            request1.Headers.Add("Authorization", token1);

                            //var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                            //   $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                            //request1.Headers.Add("Authorization", token1);


                            var inventoryObjectResult = await client1.SendAsync(request1);
                            if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvdetailsGrinId = inventoryObjectResult.StatusCode;

                            var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                            dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                            dynamic inventoryObject = inventoryObjectData.data;
                            if (inventoryObject != null)
                            {
                                decimal balanceQty = inventoryObject.balance_Quantity;
                                int flag1 = 0;
                                int flag2 = 0;
                                decimal bal = 0;
                                if (inventoryObject.balance_Quantity <= acceptedQty && inventoryObject.balance_Quantity != 0)
                                {
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceID = iQCCreate.IQCNumber;
                                    inventoryObject.referenceIDFrom = "IQC";
                                    //inventoryObject.balance_Quantity -= acceptedQty;
                                    acceptedQty -= balanceQty;
                                }
                                else if (inventoryObject.balance_Quantity > acceptedQty)
                                {
                                    if (acceptedQty == 0)
                                    {
                                        inventoryObject.balance_Quantity = acceptedQty;
                                        inventoryObject.warehouse = "IQC";
                                        inventoryObject.location = "IQC";
                                        inventoryObject.referenceID = iQCCreate.IQCNumber;
                                        inventoryObject.referenceIDFrom = "IQC";
                                        flag1 = 1;

                                    }
                                    else
                                    {
                                        bal = inventoryObject.balance_Quantity - acceptedQty;
                                        if (bal != 0)
                                        {
                                            flag2 = 1;
                                        }
                                        inventoryObject.balance_Quantity = acceptedQty;
                                        inventoryObject.warehouse = "IQC";
                                        inventoryObject.location = "IQC";
                                        inventoryObject.referenceID = iQCCreate.IQCNumber;
                                        inventoryObject.referenceIDFrom = "IQC";
                                        acceptedQty = 0;

                                    }
                                }
                                if (inventoryObject.balance_Quantity == 0) { inventoryObject.isStockAvailable = 0; }
                                var json = JsonConvert.SerializeObject(inventoryObject);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");
                                //var response = await _httpClient.PutAsync(string.Concat(_config["InventoryAPI"],
                                //    "UpdateInventory?id=", inventoryObject.id), data);
                                var client5 = _clientFactory.CreateClient();
                                var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryAPI"],
                                "UpdateInventory?id=", inventoryObject.id))
                                {
                                    Content = data
                                };
                                request5.Headers.Add("Authorization", token5);

                                var response = await client5.SendAsync(request5);

                                if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;

                                //InventoryTranction Update Code

                                IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                                iqcInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                                iqcInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                                iqcInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                                iqcInventoryTranctionDto.Description = inventoryObject.description;
                                iqcInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                                iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                                iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                                iqcInventoryTranctionDto.Warehouse = "IQC";
                                iqcInventoryTranctionDto.From_Location = "GRIN";
                                iqcInventoryTranctionDto.TO_Location = "IQC";
                                iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo;
                                iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                                iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                                iqcInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;
                                iqcInventoryTranctionDto.ReferenceIDFrom = "IQC";
                                iqcInventoryTranctionDto.GrinMaterialType = "GRIN";
                                iqcInventoryTranctionDto.shopOrderNo = "";
                                iqcInventoryTranctionDto.Remarks = "GRINIQC Done";
                                iqcInventoryTranctionDto.IsStockAvailable = inventoryObject.isStockAvailable;
                                iqcInventoryTranctionDto.TransactionType = InventoryType.Inward;

                                string rfqSourcingPPdetailsJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                                var contents = new StringContent(rfqSourcingPPdetailsJsons, Encoding.UTF8, "application/json");
                                var client7 = _clientFactory.CreateClient();
                                var token7 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request7 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = contents
                                };
                                request7.Headers.Add("Authorization", token7);

                                var inventoryTransResponses = await client7.SendAsync(request7);

                                if (inventoryTransResponses.StatusCode != HttpStatusCode.OK) createInvTransfromGrin1 = inventoryTransResponses.StatusCode;


                                IQCInventoryTranctionDto GRINInventoryTranctionDto = new IQCInventoryTranctionDto();
                                GRINInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                                GRINInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                                GRINInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                                GRINInventoryTranctionDto.Description = inventoryObject.description;
                                GRINInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                                GRINInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                                GRINInventoryTranctionDto.UOM = inventoryObject.uom;
                                GRINInventoryTranctionDto.Warehouse = "GRIN";
                                GRINInventoryTranctionDto.From_Location = "GRIN";
                                GRINInventoryTranctionDto.TO_Location = "IQC";
                                GRINInventoryTranctionDto.GrinNo = inventoryObject.grinNo;
                                GRINInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                                GRINInventoryTranctionDto.PartType = inventoryObject.partType;
                                GRINInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;
                                GRINInventoryTranctionDto.ReferenceIDFrom = "GRIN";
                                GRINInventoryTranctionDto.GrinMaterialType = "GRIN";
                                GRINInventoryTranctionDto.shopOrderNo = "";
                                GRINInventoryTranctionDto.Remarks = "GRINIQC Done";
                                GRINInventoryTranctionDto.IsStockAvailable = false;
                                GRINInventoryTranctionDto.TransactionType = InventoryType.Outward;

                                string rfqSourcingPPdetailsJsons_1 = JsonConvert.SerializeObject(GRINInventoryTranctionDto);
                                var contents_1 = new StringContent(rfqSourcingPPdetailsJsons_1, Encoding.UTF8, "application/json");
                                var client7_1 = _clientFactory.CreateClient();
                                var token7_1 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request7_1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = contents_1
                                };
                                request7_1.Headers.Add("Authorization", token7_1);

                                var inventoryTransResponses_1 = await client7_1.SendAsync(request7_1);
                                if (inventoryTransResponses_1.StatusCode != HttpStatusCode.OK) createInvTransfromGrin1 = inventoryTransResponses_1.StatusCode;

                                if (rejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
                                {
                                    IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                    grinInventoryDto.PartNumber = iQCConfirmationItems.ItemNumber;
                                    grinInventoryDto.LotNumber = grinPartsDetails.LotNumber;
                                    grinInventoryDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                    grinInventoryDto.Description = grinPartsDetails.ItemDescription;
                                    grinInventoryDto.ProjectNumber = projectNos;
                                    grinInventoryDto.Max = itemMasterObject.max;
                                    grinInventoryDto.Min = itemMasterObject.min;
                                    grinInventoryDto.UOM = grinPartsDetails.UOM;
                                    grinInventoryDto.Warehouse = "Reject";
                                    grinInventoryDto.Location = "Reject";
                                    grinInventoryDto.GrinNo = iQCCreate.GrinNumber;
                                    grinInventoryDto.GrinPartId = iQCConfirmationItems.GrinPartId;
                                    grinInventoryDto.PartType = itemMasterObject.itemType;  //We need to check this
                                    grinInventoryDto.ReferenceID = iQCCreate.IQCNumber; // Convert.ToString(iQCConfirmationItems.Id) //;
                                    grinInventoryDto.ReferenceIDFrom = "IQC";
                                    grinInventoryDto.GrinMaterialType = "GRIN";
                                    grinInventoryDto.ShopOrderNo = "";

                                    if (flag1 == 1)
                                    {
                                        grinInventoryDto.Balance_Quantity = balanceQty;
                                        rejectedQty -= balanceQty;
                                    }
                                    else if (flag2 == 1)
                                    {
                                        grinInventoryDto.Balance_Quantity = bal;
                                        rejectedQty -= bal;

                                    }

                                    //var httpClientHandler = new HttpClientHandler();
                                    //httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                                    //var httpClient = new HttpClient(httpClientHandler);
                                    string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(grinInventoryDto);
                                    //var rfqApiUrl = _config["InventoryAPI"];
                                    var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                                    //var rfqCustomerIdResponse = await _httpClient.PostAsync($"{rfqApiUrl}CreateInventoryFromGrin", content);

                                    var client6 = _clientFactory.CreateClient();
                                    var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                    var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                    "CreateInventoryFromGrin"))
                                    {
                                        Content = content
                                    };
                                    request6.Headers.Add("Authorization", token6);

                                    var rfqCustomerIdResponse = await client6.SendAsync(request6);
                                    if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInvfromGrin = rfqCustomerIdResponse.StatusCode;

                                    //InventoryTranction Update Code

                                    IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                    iqcInventoryTranctionDtos.PartNumber = grinInventoryDto.PartNumber;
                                    iqcInventoryTranctionDtos.LotNumber = grinInventoryDto.LotNumber;
                                    iqcInventoryTranctionDtos.MftrPartNumber = grinInventoryDto.MftrPartNumber;
                                    iqcInventoryTranctionDtos.Description = grinInventoryDto.Description;
                                    iqcInventoryTranctionDtos.ProjectNumber = grinInventoryDto.ProjectNumber;
                                    iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                    iqcInventoryTranctionDtos.IsStockAvailable = true;
                                    iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                    iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                    iqcInventoryTranctionDtos.From_Location = "GRIN";
                                    iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                    iqcInventoryTranctionDtos.GrinNo = grinInventoryDto.GrinNo;
                                    iqcInventoryTranctionDtos.GrinPartId = grinInventoryDto.GrinPartId;
                                    iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                    iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.GrinNo;
                                    iqcInventoryTranctionDtos.ReferenceIDFrom = "IQC";
                                    iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
                                    iqcInventoryTranctionDtos.shopOrderNo = "";
                                    iqcInventoryTranctionDtos.Remarks = "GRINIQC Done";
                                    iqcInventoryTranctionDtos.TransactionType = InventoryType.Inward;

                                    //var httpClientHandlers = new HttpClientHandler();
                                    //httpClientHandlers.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                                    //var httpClients = new HttpClient(httpClientHandlers);
                                    string iqcInventoryTranctionDtoJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDtos);
                                    //var rfqApiUrls = _config["InventoryTranctionAPI"];
                                    var contents1 = new StringContent(iqcInventoryTranctionDtoJsons, Encoding.UTF8, "application/json");
                                    //var inventoryTransResponses = await _httpClient.PostAsync($"{rfqApiUrls}CreateInventoryTranction", contents);
                                    var client8 = _clientFactory.CreateClient();
                                    var token8 = HttpContext.Request.Headers["Authorization"].ToString();
                                    var request8 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                    "CreateInventoryTranction"))
                                    {
                                        Content = contents1
                                    };
                                    request8.Headers.Add("Authorization", token8);

                                    var inventoryTransResponses1 = await client8.SendAsync(request8);

                                    if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK) createInvTransfromGrin = inventoryTransResponses1.StatusCode;
                                }



                            }

                        }
                        ////update accepted qty and rejected qty in grin model
                        //Updating IQC Status in GrinParts

                        var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinPartsQty(iQCConfirmationItems.GrinPartId, iQCConfirmationItems.AcceptedQty.ToString(), iQCConfirmationItems.RejectedQty.ToString());

                        var grinParts = _mapper.Map<GrinParts>(updatedGrinPartsQty);
                        grinParts.IsIqcCompleted = true;
                        //if (itemMasterObject.poMaterialType == "ServiceItem")
                        //{
                        //    grinParts.IsBinningCompleted = true;
                        //}
                        string result = await _grinPartsRepository.UpdateGrinQty(grinParts);
                        _grinPartsRepository.SaveAsync();

                        //Updating binning Status in GrinParts
                        if (iQCDto[i].AcceptedQty == 0)
                        {
                            var grinPartsData = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(iQCDto[i].GrinPartId);
                            grinPartsData.IsBinningCompleted = true;
                            if (grinPartsData.Remarks == null || string.IsNullOrWhiteSpace(grinPartsData.Remarks))
                            {
                                grinPartsData.Remarks = "Iqc Rejected for all";
                            }
                            else
                            {
                                grinPartsData.Remarks = grinPartsData.Remarks + "[Iqc Rejected for all]";
                            }
                            await _grinPartsRepository.UpdateGrinQty(grinPartsData);
                            _grinPartsRepository.SaveAsync();
                        }

                    }

                    //Updating IQC Status in Grin

                    var grinPartsIqcStatuscount = await _grinPartsRepository.GetGrinPartsIqcStatusCount(iQCCreate.GrinId);

                    if (grinPartsIqcStatuscount == 0)
                    {
                        var grinDetails = await _grinRepository.GetGrinByGrinNo(grinNumber);
                        grinDetails.IsIqcCompleted = true;
                        await _grinRepository.UpdateGrin(grinDetails);
                        _grinRepository.SaveAsync();

                    }

                    //Updating IQC Status in IQC

                    var grinIqcStatuscount = await _grinRepository.GetGrinIqcStatusCount(grinNumber);

                    if (grinIqcStatuscount > 0)
                    {
                        var iqcDetails = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);
                        iqcDetails.IsIqcCompleted = true;
                        await _iQCConfirmationRepository.UpdateIqc(iqcDetails);

                    }
                    if (getItemmResp == HttpStatusCode.OK && createInvfromGrin == HttpStatusCode.OK && createInvTransfromGrin == HttpStatusCode.OK && createInvTransfromGrin1 == HttpStatusCode.OK && getInvdetailsGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK)
                    {
                        _iQCConfirmationRepository.SaveAsync();
                        _grinRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create IQCConfirmation action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                    if (serverKey == "avision")
                    {
                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();
                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"], "GetEmailTemplatebyProcessType?ProcessType=CreateIQC"));
                        request.Headers.Add("Authorization", token);
                        var response = await client.SendAsync(request);
                        var EmailTempString = await response.Content.ReadAsStringAsync();
                        var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                        var Operations = "From,CreateIQC";
                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                        request1.Headers.Add("Authorization", token);
                        var response1 = await client.SendAsync(request1);
                        var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                        var emaildetails1 = JsonConvert.DeserializeObject<Tips.Grin.Api.Entities.Dto.EmailIDsDto>(EmailTempString1);
                        var httpclientHandler = new HttpClientHandler();
                        var httpClient = new HttpClient(httpclientHandler);
                        var mails = (emaildetails1.data.Where(x => x.operation == "CreateIQC").Select(x => x.emailIds).FirstOrDefault()).Split(',');
                        var email = new MimeMessage();
                        email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));

                        email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));

                        email.Subject = emaildetails.data.subject;
                        string body = emaildetails.data.template;
                        var grindetails = await _grinRepository.GetGrinByGrinNo(grinNumber);
                        body = body.Replace("{{IQC Numbers}}", existingIqcConfirmation.IQCNumber);
                        body = body.Replace("{{GRIN Numbers}}", grinNumber);
                        body = body.Replace("{{Vendor Id}}", grindetails.VendorId);
                        body = body.Replace("{{Vendor Name}}", grindetails.VendorName);
                        body = body.Replace("{{Created By}}", existingIqcConfirmation.CreatedBy);
                        body = body.Replace("{{Created Dated}}", existingIqcConfirmation.CreatedOn.ToString());
                        string? ProjectNos = null;
                        List<string>? tempProj = new List<string>();
                        List<string>? tempPRno = new List<string>();
                        string? PONos = null;
                        foreach (var item in grindetails.GrinParts)
                        {
                            if (item.ProjectNumbers.Count > 0)
                                foreach (var project in item.ProjectNumbers)
                                {
                                    if (ProjectNos.IsNullOrEmpty()) { ProjectNos = project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                    else if (!tempProj.Contains(project.ProjectNumber)) { ProjectNos = ProjectNos + ", " + project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                }

                            if (PONos.IsNullOrEmpty()) { PONos = item.PONumber; tempPRno.Add(item.PONumber); }
                            else if (!tempPRno.Contains(item.PONumber)) { PONos = PONos + ", " + item.PONumber; tempPRno.Add(item.PONumber); }
                        }
                        body = body.Replace("{{Project Ref No}}", ProjectNos);
                        body = body.Replace("{{PurchaseOrder Number}}", PONos);

                        email.Body = new TextPart(TextFormat.Html) { Text = body };

                        using var smtp = new MailKit.Net.Smtp.SmtpClient();
                        int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                        smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                        smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                        smtp.Send(email);
                        smtp.Disconnect(true);

                    }
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCConfirmation and IQCConfirmationItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    if (serverKey == "avision")
                    {
                        var grinNum = iQCCreate.GrinNumber;
                        if (grinNum != null)
                        {
                            var iqcNum = grinNum.Replace("GRN", "IQC");
                            iQCCreate.IQCNumber = iqcNum;
                        }

                    }

                    for (int i = 0; i < iQCDto.Count; i++)
                    {
                        IQCConfirmationItems iQCConfirmationItems = _mapper.Map<IQCConfirmationItems>(iQCDto[i]);
                        var grinPartId = iQCDto[i].GrinPartId;

                        var grinPartsDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartId);
                        if (grinPartsDetails == null)
                        {
                            _logger.LogError($"Invalid Grin Part Id {grinPartId}");
                            serviceResponse.Data = null;
                            serviceResponse.Message = $"Invalid Grin Part Id {grinPartId}";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            return BadRequest(serviceResponse);
                        }
                        if (grinPartsDetails.Qty <= (iQCConfirmationItems.AcceptedQty + iQCConfirmationItems.RejectedQty))
                        {
                            grinPartsDetails.AcceptedQty = iQCConfirmationItems.AcceptedQty;
                            grinPartsDetails.RejectedQty = iQCConfirmationItems.RejectedQty;

                        }
                        else
                        {
                            _logger.LogError("Grinpart Quantity should not be lesser than accepted + rejected Quantity .");
                            serviceResponse.Data = null;
                            serviceResponse.Message = "Grinpart Quantity should not be lesser than accepted + rejected Quantity ";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            return BadRequest(serviceResponse);
                        }

                        //Updating IQC Status in IqcItem Level

                        iQCConfirmationItems.IsIqcCompleted = true;
                        iQCItemList.Add(iQCConfirmationItems);

                        //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterEnggAPI"],
                        //      "GetItemMasterByItemNumber?", "&ItemNumber=", HttpUtility.UrlEncode(iQcItemNo)));
                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = iQCDto[i].ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request.Headers.Add("Authorization", token);

                        var itemMasterObjectResult = await client.SendAsync(request);
                        if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                            getItemmResp = itemMasterObjectResult.StatusCode;

                        var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                        dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                        dynamic itemMasterObject = itemMasterObjectData.data;

                        decimal acceptedQty = iQCDto[i].AcceptedQty;
                        decimal rejectedQty = iQCDto[i].RejectedQty;
                        foreach (var projectNo in grinPartsDetails.ProjectNumbers)
                        {

                            var client1 = _clientFactory.CreateClient();
                            var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                            var itemNo = iQCDto[i].ItemNumber;
                            var encodedItemNo = Uri.EscapeDataString(itemNo);
                            var grinNo = iQCCreate.GrinNumber;
                            var encodedgrinNo = Uri.EscapeDataString(grinNo);
                            var grinPartsIds = projectNo.GrinPartsId;
                            var projectNos = projectNo.ProjectNumber;
                            var encodedprojectNos = Uri.EscapeDataString(projectNos);
                            var lotNumber = grinPartsDetails.LotNumber;
                            if (string.IsNullOrWhiteSpace(lotNumber))
                            {
                                var errorMessage = $"Lot Number not found for Grin Part Id: {grinPartId}";

                                _logger.LogError(errorMessage);

                                serviceResponse.Data = null;
                                serviceResponse.Message = errorMessage;
                                serviceResponse.Success = false;
                                serviceResponse.StatusCode = HttpStatusCode.BadRequest;

                                return BadRequest(serviceResponse);
                            }
                            var encodedlotNumber = Uri.EscapeDataString(lotNumber);

                            var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                            $"GetInventoryDetailsByProjectNoandLotNo?ProjectNumber={encodedprojectNos}&LotNumber={encodedlotNumber}"));
                            request1.Headers.Add("Authorization", token1);


                        //    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                        //$"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                        //    request1.Headers.Add("Authorization", token1);




                            var inventoryObjectResult = await client1.SendAsync(request1);
                            if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvdetailsGrinId = inventoryObjectResult.StatusCode;

                            var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                            dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                            dynamic inventoryObject = inventoryObjectData.data;
                            if (inventoryObject != null)
                            {
                                decimal balanceQty = inventoryObject.balance_Quantity;
                                int flag1 = 0;
                                int flag2 = 0;
                                decimal bal = 0;
                                if (inventoryObject.balance_Quantity <= acceptedQty && inventoryObject.balance_Quantity != 0)
                                {
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceID = iQCCreate.GrinNumber;
                                    inventoryObject.referenceIDFrom = "IQC";
                                   // inventoryObject.balance_Quantity -= acceptedQty;
                                    acceptedQty -= balanceQty;
                                }
                                else if (inventoryObject.balance_Quantity > acceptedQty)
                                {
                                    if (acceptedQty == 0)
                                    {
                                        inventoryObject.balance_Quantity = acceptedQty;
                                        inventoryObject.warehouse = "IQC";
                                        inventoryObject.location = "IQC";
                                        inventoryObject.referenceID = iQCCreate.GrinNumber;
                                        inventoryObject.referenceIDFrom = "IQC";
                                        flag1 = 1;
                                    }
                                    else
                                    {
                                        bal = inventoryObject.balance_Quantity - acceptedQty;
                                        if (bal != 0)
                                        {
                                            flag2 = 1;
                                        }
                                        inventoryObject.balance_Quantity = acceptedQty;
                                        inventoryObject.warehouse = "IQC";
                                        inventoryObject.location = "IQC";
                                        inventoryObject.referenceID = iQCCreate.GrinNumber;
                                        inventoryObject.referenceIDFrom = "IQC";
                                        acceptedQty = 0;
                                    }
                                }
                                if (inventoryObject.balance_Quantity == 0) { inventoryObject.isStockAvailable = 0; }
                                var json = JsonConvert.SerializeObject(inventoryObject);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");
                                //var response = await _httpClient.PutAsync(string.Concat(_config["InventoryAPI"],
                                //    "UpdateInventory?id=", inventoryObject.id), data);
                                var client5 = _clientFactory.CreateClient();
                                var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryAPI"],
                                "UpdateInventory?id=", inventoryObject.id))
                                {
                                    Content = data
                                };
                                request5.Headers.Add("Authorization", token5);

                                var response = await client5.SendAsync(request5);
                                if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;

                                //InventoryTranction Update Code
                                IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                                iqcInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                                iqcInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                                iqcInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                                iqcInventoryTranctionDto.Description = inventoryObject.description;
                                iqcInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                                iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                                iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                                iqcInventoryTranctionDto.Warehouse = "IQC";
                                iqcInventoryTranctionDto.From_Location = "GRIN";
                                iqcInventoryTranctionDto.TO_Location = "IQC";
                                iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo;
                                iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                                iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                                iqcInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;
                                iqcInventoryTranctionDto.ReferenceIDFrom = "IQC";
                                iqcInventoryTranctionDto.GrinMaterialType = "GRIN";
                                iqcInventoryTranctionDto.shopOrderNo = "";
                                iqcInventoryTranctionDto.Remarks = "GRINIQC Done";
                                iqcInventoryTranctionDto.IsStockAvailable = inventoryObject.isStockAvailable;
                                iqcInventoryTranctionDto.TransactionType = InventoryType.Inward;

                                string rfqSourcingPPdetailsJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                                var contents = new StringContent(rfqSourcingPPdetailsJsons, Encoding.UTF8, "application/json");
                                var client7 = _clientFactory.CreateClient();
                                var token7 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request7 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = contents
                                };
                                request7.Headers.Add("Authorization", token7);

                                var inventoryTransResponses = await client7.SendAsync(request7);

                                if (inventoryTransResponses.StatusCode != HttpStatusCode.OK) createInvTransfromGrin1 = inventoryTransResponses.StatusCode;

                                IQCInventoryTranctionDto GRINInventoryTranctionDto = new IQCInventoryTranctionDto();
                                GRINInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                                GRINInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                                GRINInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                                GRINInventoryTranctionDto.Description = inventoryObject.description;
                                GRINInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                                GRINInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                                GRINInventoryTranctionDto.UOM = inventoryObject.uom;
                                GRINInventoryTranctionDto.Warehouse = "GRIN";
                                GRINInventoryTranctionDto.From_Location = "GRIN";
                                GRINInventoryTranctionDto.TO_Location = "IQC";
                                GRINInventoryTranctionDto.GrinNo = inventoryObject.grinNo;
                                GRINInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                                GRINInventoryTranctionDto.PartType = inventoryObject.partType;
                                GRINInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;
                                GRINInventoryTranctionDto.ReferenceIDFrom = "GRIN";
                                GRINInventoryTranctionDto.GrinMaterialType = "GRIN";
                                GRINInventoryTranctionDto.shopOrderNo = "";
                                GRINInventoryTranctionDto.Remarks = "GRINIQC Done";
                                GRINInventoryTranctionDto.IsStockAvailable = inventoryObject.isStockAvailable;
                                GRINInventoryTranctionDto.TransactionType = InventoryType.Outward;

                                string rfqSourcingPPdetailsJsons_1 = JsonConvert.SerializeObject(GRINInventoryTranctionDto);
                                var contents_1 = new StringContent(rfqSourcingPPdetailsJsons_1, Encoding.UTF8, "application/json");
                                var client7_1 = _clientFactory.CreateClient();
                                var token7_1 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request7_1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = contents_1
                                };
                                request7_1.Headers.Add("Authorization", token7_1);

                                var inventoryTransResponses_1 = await client7_1.SendAsync(request7_1);
                                if (inventoryTransResponses_1.StatusCode != HttpStatusCode.OK) createInvTransfromGrin1 = inventoryTransResponses_1.StatusCode;

                                if (rejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
                                {
                                    IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                    grinInventoryDto.PartNumber = iQCConfirmationItems.ItemNumber;
                                    grinInventoryDto.LotNumber = grinPartsDetails.LotNumber;
                                    grinInventoryDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                    grinInventoryDto.Description = grinPartsDetails.ItemDescription;
                                    grinInventoryDto.ProjectNumber = projectNos;
                                    //grinInventoryDto.Balance_Quantity = Convert.ToDecimal(iQCConfirmationItems.RejectedQty);
                                    grinInventoryDto.Max = itemMasterObject.max;
                                    grinInventoryDto.Min = itemMasterObject.min;
                                    grinInventoryDto.UOM = grinPartsDetails.UOM;
                                    grinInventoryDto.Warehouse = "Reject";
                                    grinInventoryDto.Location = "Reject";
                                    grinInventoryDto.GrinNo = iQCCreate.GrinNumber;
                                    grinInventoryDto.GrinPartId = iQCConfirmationItems.GrinPartId;
                                    grinInventoryDto.PartType = itemMasterObject.itemType;  //We need to check this
                                    grinInventoryDto.ReferenceID = iQCCreate.GrinNumber; // Convert.ToString(iQCConfirmationItems.Id) //;
                                    grinInventoryDto.ReferenceIDFrom = "IQC";
                                    grinInventoryDto.GrinMaterialType = "GRIN";
                                    grinInventoryDto.ShopOrderNo = "";

                                    if (flag1 == 1)
                                    {
                                        grinInventoryDto.Balance_Quantity = balanceQty;
                                        rejectedQty -= balanceQty;
                                    }
                                    else if (flag2 == 1)
                                    {
                                        grinInventoryDto.Balance_Quantity = bal;
                                        rejectedQty -= bal;
                                    }

                                    //var httpClientHandler = new HttpClientHandler();
                                    //httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                                    //var httpClient = new HttpClient(httpClientHandler);
                                    string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(grinInventoryDto);
                                    //var rfqApiUrl = _config["InventoryAPI"];
                                    var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                                    //var rfqCustomerIdResponse = await _httpClient.PostAsync($"{rfqApiUrl}CreateInventoryFromGrin", content);

                                    var client6 = _clientFactory.CreateClient();
                                    var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                    var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                    "CreateInventoryFromGrin"))
                                    {
                                        Content = content
                                    };
                                    request6.Headers.Add("Authorization", token6);

                                    var rfqCustomerIdResponse = await client6.SendAsync(request6);
                                    if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInvfromGrin = rfqCustomerIdResponse.StatusCode;

                                    //InventoryTranction Update Code
                                    IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                    iqcInventoryTranctionDtos.PartNumber = grinInventoryDto.PartNumber;
                                    iqcInventoryTranctionDtos.LotNumber = grinInventoryDto.LotNumber;
                                    iqcInventoryTranctionDtos.MftrPartNumber = grinInventoryDto.MftrPartNumber;
                                    iqcInventoryTranctionDtos.Description = grinInventoryDto.Description;
                                    iqcInventoryTranctionDtos.ProjectNumber = grinInventoryDto.ProjectNumber;
                                    iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                    iqcInventoryTranctionDtos.IsStockAvailable = true;
                                    iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                    iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                    iqcInventoryTranctionDtos.From_Location = "GRIN";
                                    iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                    iqcInventoryTranctionDtos.GrinNo = grinInventoryDto.GrinNo;
                                    iqcInventoryTranctionDtos.GrinPartId = grinInventoryDto.GrinPartId;
                                    iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                    iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.GrinNo;
                                    iqcInventoryTranctionDtos.ReferenceIDFrom = "IQC";
                                    iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
                                    iqcInventoryTranctionDtos.shopOrderNo = "";
                                    iqcInventoryTranctionDtos.Remarks = "GRINIQC Done";
                                    iqcInventoryTranctionDtos.TransactionType = InventoryType.Inward;

                                    string iqcInventoryTranctionDtoJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDtos);
                                    var contents1 = new StringContent(iqcInventoryTranctionDtoJsons, Encoding.UTF8, "application/json");
                                    var client8 = _clientFactory.CreateClient();
                                    var token8 = HttpContext.Request.Headers["Authorization"].ToString();
                                    var request8 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                    "CreateInventoryTranction"))
                                    {
                                        Content = contents1
                                    };
                                    request8.Headers.Add("Authorization", token8);

                                    var inventoryTransResponses1 = await client8.SendAsync(request8);
                                    if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK) createInvTransfromGrin = inventoryTransResponses1.StatusCode;
                                }

                            }
                        }
                        ////update accepted qty and rejected qty in grin model
                        //Updating IQC Status in GrinParts
                        var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinPartsQty(iQCConfirmationItems.GrinPartId, iQCConfirmationItems.AcceptedQty.ToString(), iQCConfirmationItems.RejectedQty.ToString());

                        var grinParts = _mapper.Map<GrinParts>(updatedGrinPartsQty);
                        //if (itemMasterObject.poMaterialType == "ServiceItem")
                        //{
                        //    grinParts.IsBinningCompleted = true;
                        //}
                        grinParts.IsIqcCompleted = true;
                        string result = await _grinPartsRepository.UpdateGrinQty(grinParts);

                        //Updating binning Status in GrinParts
                        if (iQCDto[i].AcceptedQty == 0)
                        {
                            var grinPartsData = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(iQCDto[i].GrinPartId);
                            grinPartsData.IsBinningCompleted = true;
                            if (grinPartsData.Remarks == null || string.IsNullOrWhiteSpace(grinPartsData.Remarks))
                            {
                                grinPartsData.Remarks = "Iqc Rejected for all";
                            }
                            else
                            {
                                grinPartsData.Remarks = grinPartsData.Remarks + "[Iqc Rejected for all]";
                            }
                            await _grinPartsRepository.UpdateGrinQty(grinPartsData);
                            _grinPartsRepository.SaveAsync();
                        }
                    }

                    //Updating IQC Status in IQC
                    iQCCreate.IQCConfirmationItems = iQCItemList;
                    iQCCreate.IsIqcCompleted = true;
                    await _iQCConfirmationRepository.CreateIqc(iQCCreate);



                    //Updating IQC Status in Grin
                    var grinDetails = await _grinRepository.GetGrinByGrinNo(grinNumber);
                    grinDetails.IsIqcCompleted = true;
                    await _grinRepository.UpdateGrin(grinDetails);

                    if (getItemmResp == HttpStatusCode.OK && createInvfromGrin == HttpStatusCode.OK && createInvTransfromGrin1 == HttpStatusCode.OK && createInvTransfromGrin == HttpStatusCode.OK && getInvdetailsGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK)
                    {
                        _iQCConfirmationRepository.SaveAsync();
                        _grinRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create IQCConfirmation action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }
                    if (serverKey == "avision")
                    {
                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();
                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"], "GetEmailTemplatebyProcessType?ProcessType=CreateIQC"));
                        request.Headers.Add("Authorization", token);
                        var response = await client.SendAsync(request);
                        var EmailTempString = await response.Content.ReadAsStringAsync();
                        var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                        var Operations = "From,CreateIQC";
                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                        request1.Headers.Add("Authorization", token);
                        var response1 = await client.SendAsync(request1);
                        var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                        var emaildetails1 = JsonConvert.DeserializeObject<Tips.Grin.Api.Entities.Dto.EmailIDsDto>(EmailTempString1);
                        var httpclientHandler = new HttpClientHandler();
                        var httpClient = new HttpClient(httpclientHandler);
                        var mails = (emaildetails1.data.Where(x => x.operation == "CreateIQC").Select(x => x.emailIds).FirstOrDefault()).Split(',');
                        var email = new MimeMessage();
                        email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));

                        email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));

                        email.Subject = emaildetails.data.subject;
                        string body = emaildetails.data.template;
                        var grindetails = await _grinRepository.GetGrinByGrinNo(grinNumber);
                        body = body.Replace("{{IQC Numbers}}", iQCCreate.IQCNumber);
                        body = body.Replace("{{GRIN Numbers}}", grinNumber);
                        body = body.Replace("{{Vendor Id}}", grindetails.VendorNumber);
                        body = body.Replace("{{Vendor Name}}", grindetails.VendorName);
                        body = body.Replace("{{Created By}}", iQCCreate.CreatedBy);
                        body = body.Replace("{{Created Dated}}", iQCCreate.CreatedOn.ToString());
                        string? ProjectNos = null;
                        List<string>? tempProj = new List<string>();
                        List<string>? tempPRno = new List<string>();
                        string? PONos = null;
                        foreach (var item in grindetails.GrinParts)
                        {
                            if (item.ProjectNumbers.Count > 0)
                                foreach (var project in item.ProjectNumbers)
                                {
                                    if (ProjectNos.IsNullOrEmpty()) { ProjectNos = project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                    else if (!tempProj.Contains(project.ProjectNumber)) { ProjectNos = ProjectNos + ", " + project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                }

                            if (PONos.IsNullOrEmpty()) { PONos = item.PONumber; tempPRno.Add(item.PONumber); }
                            else if (!tempPRno.Contains(item.PONumber)) { PONos = PONos + ", " + item.PONumber; tempPRno.Add(item.PONumber); }
                        }
                        body = body.Replace("{{Project Ref No}}", ProjectNos);
                        body = body.Replace("{{PurchaseOrder Number}}", PONos);

                        email.Body = new TextPart(TextFormat.Html) { Text = body };

                        using var smtp = new MailKit.Net.Smtp.SmtpClient();
                        int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                        smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                        smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                        smtp.Send(email);
                        smtp.Disconnect(true);

                    }
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCConfirmation Successfully Created";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);


                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateIqc API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateIqc API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);


            }
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateIqc([FromBody] IQCConfirmationPostDto iQCConfirmationPostDto)
        //{
        //    ServiceResponse<IQCConfirmationDto> serviceResponse = new ServiceResponse<IQCConfirmationDto>();

        //    try
        //    {
        //        if (iQCConfirmationPostDto == null)
        //        {
        //            _logger.LogError("IQCConfirmation details object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "IQCConfirmation details object is null";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest();
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid IQCConfirmation details object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid model object";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }

        //        var iQCCreate = _mapper.Map<IQCConfirmation>(iQCConfirmationPostDto);
        //        var iQCDto = iQCConfirmationPostDto.IQCConfirmationItemsPostDtos;
        //        var iQcItemNo = iQCDto[0].ItemNumber;
        //        var iQCItemList = new List<IQCConfirmationItems>();

        //        for(int  i=0; i< iQCDto.Count;i++)
        //        {
        //            IQCConfirmationItems iQCConfirmationItems = _mapper.Map<IQCConfirmationItems>(iQCDto[i]);
        //            var grinPartId = iQCDto[i].GrinPartId;

        //            var grinPartsDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartId);
        //            if (grinPartsDetails == null)
        //            {
        //                _logger.LogError($"Invalid Grin Part Id {grinPartId}");
        //                serviceResponse.Data = null;
        //                serviceResponse.Message = $"Invalid Grin Part Id {grinPartId}";
        //                serviceResponse.Success = false;
        //                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //                return BadRequest(serviceResponse);
        //            }
        //            if (grinPartsDetails.Qty <= (iQCConfirmationItems.AcceptedQty + iQCConfirmationItems.RejectedQty))
        //            {
        //                grinPartsDetails.AcceptedQty = iQCConfirmationItems.AcceptedQty;
        //                grinPartsDetails.RejectedQty = iQCConfirmationItems.RejectedQty;                      
        //                _grinPartsRepository.SaveAsync();
        //            }
        //            else
        //            {
        //                _logger.LogError("Grinpart Quantity should not be lesser than accepted + rejected Quantity .");
        //                serviceResponse.Data = null;
        //                serviceResponse.Message = "Grinpart Quantity should not be lesser than accepted + rejected Quantity ";
        //                serviceResponse.Success = false;
        //                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //                return BadRequest(serviceResponse);
        //            }
        //            iQCItemList.Add(iQCConfirmationItems);


        //            var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterEnggAPI"],
        //                  "GetItemMasterByItemNumber?", "&ItemNumber=", iQcItemNo));
        //            var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
        //            dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
        //            dynamic itemMasterObject = itemMasterObjectData.data;

        //            IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
        //            grinInventoryDto.PartNumber = iQCConfirmationItems.ItemNumber;
        //            grinInventoryDto.LotNumber = grinPartsDetails.LotNumber;
        //            grinInventoryDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
        //            grinInventoryDto.Description = grinPartsDetails.ItemDescription;
        //            grinInventoryDto.ProjectNumber = grinPartsDetails.ProjectNumbers[i].ProjectNumber;
        //            grinInventoryDto.Balance_Quantity = Convert.ToDecimal(iQCConfirmationItems.RejectedQty);
        //            grinInventoryDto.UOM = grinPartsDetails.UOM;
        //            grinInventoryDto.Warehouse = "Reject";
        //            grinInventoryDto.Location = "Reject";
        //            grinInventoryDto.GrinNo = iQCCreate.GrinNumber;
        //            grinInventoryDto.GrinPartId = iQCConfirmationItems.GrinPartId;
        //            grinInventoryDto.PartType = itemMasterObject.itemType;  //We need to check this
        //            grinInventoryDto.ReferenceID = "GRIN"; /*/ Convert.ToString(iQCConfirmationItems.Id) /;*/
        //            grinInventoryDto.ReferenceIDFrom = "GRIN";
        //            grinInventoryDto.GrinMaterialType = "GRIN";
        //            grinInventoryDto.ShopOrderNo = "";

        //            var jsons = JsonConvert.SerializeObject(grinInventoryDto);
        //            var datas = new StringContent(jsons, Encoding.UTF8, "application/json");

        //            //Inventory Update Code
        //            var tokenValue = _httpContextAccessor?.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
        //            if (!string.IsNullOrEmpty(tokenValue) && tokenValue.StartsWith("Bearer "))
        //            {
        //                var token = tokenValue.Substring(7);
        //                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //            }
        //            var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventoryFromGrin"), datas);
        //            decimal acceptedQty = iQCDto[i].AcceptedQty;
        //            foreach (var projectNo in grinPartsDetails.ProjectNumbers)
        //            {
        //                var grinNo = iQCCreate.GrinNumber;
        //                var grinPartsId = projectNo.GrinPartsId;
        //                var itemNo = iQCDto[i].ItemNumber;
        //                var projectNos = projectNo.ProjectNumber;
        //                var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
        //                    "GetInventoryDetailsByGrinNoandGrinId?", "GrinNo=", grinNo, "&GrinPartsId=",
        //                    grinPartsId, "&ItemNumber=", itemNo, "&ProjectNumber=", projectNos));
        //                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
        //                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
        //                dynamic inventoryObject = inventoryObjectData.data;
        //                if(inventoryObject !=null)
        //                {
        //                    decimal balanceQty = inventoryObject.balance_Quantity;

        //                    if (inventoryObject.balance_Quantity <= acceptedQty && inventoryObject.balance_Quantity != 0)
        //                    {
        //                        inventoryObject.warehouse = "IQC";
        //                        inventoryObject.location = "IQC";
        //                        inventoryObject.referenceIDFrom = "GRIN";
        //                        acceptedQty -= balanceQty;

        //                    }
        //                    else if (inventoryObject.balance_Quantity > acceptedQty)
        //                    {
        //                        if (acceptedQty == 0)
        //                        {
        //                            inventoryObject.balance_Quantity = acceptedQty;
        //                            inventoryObject.warehouse = "IQC";
        //                            inventoryObject.location = "IQC";
        //                            inventoryObject.referenceIDFrom = "GRIN";
        //                            inventoryObject.isStockAvailable = false;
        //                        }
        //                        else
        //                        {
        //                            inventoryObject.balance_Quantity = acceptedQty;
        //                            inventoryObject.warehouse = "IQC";
        //                            inventoryObject.location = "IQC";
        //                            inventoryObject.referenceIDFrom = "GRIN";
        //                            acceptedQty = 0;
        //                        }
        //                    }

        //                    var json = JsonConvert.SerializeObject(inventoryObject);
        //                    var data = new StringContent(json, Encoding.UTF8, "application/json");
        //                    var response = await _httpClient.PutAsync(string.Concat(_config["InventoryAPI"],
        //                        "UpdateInventory?id=", inventoryObject.id), data);
        //                }

        //            }

        //            ////update accepted qty and rejected qty in grin model

        //            var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinPartsQty(iQCConfirmationItems.GrinPartId, iQCConfirmationItems.AcceptedQty.ToString(), iQCConfirmationItems.RejectedQty.ToString());

        //            var iQCCreates = _mapper.Map<GrinParts>(updatedGrinPartsQty);

        //            string result = await _grinPartsRepository.UpdateGrinQty(iQCCreates);

        //            _grinPartsRepository.SaveAsync();
        //        }

        //        iQCCreate.IQCConfirmationItems = iQCItemList;
        //        iQCCreate.IsIqcCompleted = true;
        //        await _iQCConfirmationRepository.CreateIqc(iQCCreate);
        //        _iQCConfirmationRepository.SaveAsync();

        //        //Updating IQC Status in Grin
        //        var grinNumber = iQCCreate.GrinNumber;
        //        var grinDetails = await _grinRepository.GetGrinByGrinNo(grinNumber);
        //        grinDetails.IsIqcCompleted = true;
        //        await _grinRepository.UpdateGrin(grinDetails);
        //        _grinRepository.SaveAsync();

        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "IQCConfirmation Successfully Created";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Created("IQCConfirmationById", serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside Create IQCConfirmation action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);


        //    }
        //}

        //check and enble the below code


        //[HttpPost]
        //public async Task<IActionResult> SaveMultipleIqc([FromBody] List<IQCConfirmationPostDto> iQCConfirmationPostDtos)
        //{
        //    ServiceResponse<IQCConfirmationPostDto> serviceResponse = new ServiceResponse<IQCConfirmationPostDto>();

        //    try
        //    {
        //        if (iQCConfirmationPostDtos == null)
        //        {
        //            _logger.LogError("IQCConfirmation details object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "IQCConfirmation details object is null";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest();
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid IQCConfirmation details object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid model object";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }

        //        var iQCConfirmationList = _mapper.Map<List<IQCConfirmation>>(iQCConfirmationPostDtos);
        //        bool isAnyRecordCreated = false;
        //        foreach (var iQCDetails in iQCConfirmationList)
        //        {
        //            if (iQCDetails.AcceptedQty > 0 || iQCDetails.RejectedQty > 0)
        //            {
        //                await _iQCConfirmationRepository.Create(iQCDetails);
        //                isAnyRecordCreated = true;
        //            }
        //        }
        //        if (isAnyRecordCreated)
        //        {
        //            _iQCConfirmationRepository.SaveAsync();
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Successfully Created";
        //            serviceResponse.Success = true;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Created("IQCConfirmationById", serviceResponse);
        //        }
        //        else
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Nothing to save,Because accepted or rejected quantity is not greater than 0 in any rows !";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Created("IQCConfirmationById", serviceResponse);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside Create IQCConfirmation action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, "Internal server error");


        //    }
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIqcDetailsbyId(int id)
        {
            ServiceResponse<IQCConfirmationDto> serviceResponse = new ServiceResponse<IQCConfirmationDto>();

            try
            {
                var iQCDetailsbyId = await _iQCConfirmationRepository.GetIqcDetailsbyId(id);
                if (iQCDetailsbyId == null)
                {
                    _logger.LogError($"IQCConfirmation details with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCConfirmation details with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned IQCConfirmation details with id: {id}");
                    List<IQCConfirmationItemsDto> iQCConfirmationItemsList = new List<IQCConfirmationItemsDto>();
                    var iQcGrinNo = iQCDetailsbyId.GrinNumber;
                    var grinDetailsbyGrinNo = await _grinRepository.GetGrinByGrinNo(iQcGrinNo);

                    var iQCConformationDetailsDto = _mapper.Map<IQCConfirmationDto>(grinDetailsbyGrinNo);
                    iQCConformationDetailsDto.Id = id;
                    iQCConformationDetailsDto.GrinId = grinDetailsbyGrinNo.Id;
                    var grinParts = grinDetailsbyGrinNo.GrinParts.Where(x => x.RejectedQty != 0 || x.AcceptedQty != 0).ToList();
                    if (grinParts.Count() != 0)
                    {
                        foreach (var grinDetails in grinParts)
                        {
                            IQCConfirmationItemsDto iQCConfirmationItemsDtos = _mapper.Map<IQCConfirmationItemsDto>(grinDetails);
                            iQCConfirmationItemsDtos.ProjectNumbers = _mapper.Map<List<ProjectNumbersDto>>(grinDetails.ProjectNumbers);
                            iQCConfirmationItemsDtos.Id = iQCConfirmationItemsDtos.Id;
                            iQCConfirmationItemsDtos.ReceivedQty = grinDetails.Qty;
                            iQCConfirmationItemsDtos.GrinPartId = grinDetails.Id;
                            iQCConfirmationItemsDtos.ExpiryDate = grinDetails.ExpiryDate;
                            iQCConfirmationItemsList.Add(iQCConfirmationItemsDtos);
                        }
                    }
                    iQCConformationDetailsDto.IQCConfirmationItems = iQCConfirmationItemsList;
                    serviceResponse.Data = iQCConformationDetailsDto;
                    serviceResponse.Message = "IQCConfirmationById Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetIqcDetailsbyId API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetIqcDetailsbyId API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIQC(int id)
        {
            ServiceResponse<IQCConfirmationDto> serviceResponse = new ServiceResponse<IQCConfirmationDto>();

            try
            {
                var iQCDetailById = await _iQCConfirmationRepository.GetIqcDetailsbyId(id);
                if (iQCDetailById == null)
                {
                    _logger.LogError($"IQCDelete with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCDelete with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                //iQCDetailById.IsDeleted = true;
                string result = await _iQCConfirmationRepository.UpdateIqc(iQCDetailById);
                serviceResponse.Message = "IQCConfirmation Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeleteIQC API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteIQC API for the following id:{id}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllIQCConfirmationItems()
        {
            ServiceResponse<IEnumerable<IQCConfirmationItemsDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationItemsDto>>();

            try
            {
                var getAllIQCItemDetails = await _iQCConfirmationItemsRepository.GetAllIQCConfirmationItems();
                _logger.LogInfo("Returned all IQCConfirmationItems details()s");
                var result = _mapper.Map<IEnumerable<IQCConfirmationItemsDto>>(getAllIQCItemDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all IQCConfirmationItems";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllIQCConfirmationItems API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllIQCConfirmationItems API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveIQCConfirmationIdNameList()
        {
            ServiceResponse<IEnumerable<IQCConfirmationIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationIdNameListDto>>();
            try
            {
                var listOfActiveIQCConfirmationName = await _iQCConfirmationRepository.GetAllActiveIQCConfirmationNameList();

                var result = _mapper.Map<IEnumerable<IQCConfirmationIdNameListDto>>(listOfActiveIQCConfirmationName);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All ActiveIQCConfirmationIdNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveIQCConfirmationIdNameList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveIQCConfirmationIdNameList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateIQCConfirmationItems([FromBody] IQCConfirmationSaveDto iqcConfirmationSaveDto)
        {
            ServiceResponse<IQCConfirmationSaveDto> serviceResponse = new ServiceResponse<IQCConfirmationSaveDto>();

            try
            {
                string serverKey = GetServerKey();

                if (iqcConfirmationSaveDto is null)
                {
                    _logger.LogError("Create IQCConfirmation object sent from the client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Create IQCConfirmation object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Create IQCConfirmation object sent from the client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Create IQCConfirmation object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var iqcConfirmation = _mapper.Map<IQCConfirmation>(iqcConfirmationSaveDto);
                var iqcConfirmationItemsDto = iqcConfirmationSaveDto.IQCConfirmationItemsPostDtos;
                var iqcConfirmationItems = _mapper.Map<IQCConfirmationItems>(iqcConfirmationItemsDto);
                var grinNumber = iqcConfirmation.GrinNumber;
                HttpStatusCode getInvGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInv = HttpStatusCode.OK;
                HttpStatusCode getInvTrancGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInvTranc = HttpStatusCode.OK;
                HttpStatusCode createInv = HttpStatusCode.OK;
                HttpStatusCode createInvTrans1 = HttpStatusCode.OK;
                HttpStatusCode createInvTrans = HttpStatusCode.OK;
                HttpStatusCode getItemmResp = HttpStatusCode.OK;

                var existingIqcConfirmation = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);

                if (existingIqcConfirmation != null)
                {
                    iqcConfirmationItems.IQCConfirmationId = existingIqcConfirmation.Id;

                    var grinPartId = iqcConfirmationItemsDto.GrinPartId;
                    var grinPartsDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartId);
                    if (iqcConfirmationItemsDto.GrinPartId != grinPartsDetails.Id)
                    {
                        _logger.LogError($"Invalid Grin Part Id {grinPartId}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Invalid Grin Part Id {grinPartId}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }
                    if (grinPartsDetails.Qty <= (iqcConfirmationItems.AcceptedQty + iqcConfirmationItems.RejectedQty))
                    {
                        grinPartsDetails.AcceptedQty = iqcConfirmationItems.AcceptedQty;
                        grinPartsDetails.RejectedQty = iqcConfirmationItems.RejectedQty;
                        //_grinPartsRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError("Grinpart Quantity should not be lesser than accepted + rejected Quantity .");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Grinpart Quantity should not be lesser than accepted + rejected Quantity ";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    //Updating IQC Status in IqcItem

                    iqcConfirmationItems.IsIqcCompleted = true;
                    await _iQCConfirmationItemsRepository.CreateIqcItem(iqcConfirmationItems);


                    //Updating IQC Status in GrinParts

                    grinPartsDetails.IsIqcCompleted = true;
                    await _grinPartsRepository.UpdateGrinQty(grinPartsDetails);
                    _grinPartsRepository.SaveAsync();

                    //Updating binning Status in GrinParts
                    if (iqcConfirmationItems.AcceptedQty == 0)
                    {
                        var grinPartsData = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(iqcConfirmationItems.GrinPartId);
                        grinPartsData.IsBinningCompleted = true;
                        if (grinPartsData.Remarks == null || string.IsNullOrWhiteSpace(grinPartsData.Remarks))
                        {
                            grinPartsData.Remarks = "Iqc Rejected for all";
                        }
                        else
                        {
                            grinPartsData.Remarks = grinPartsData.Remarks + "[Iqc Rejected for all]";
                        }
                        await _grinPartsRepository.UpdateGrinQty(grinPartsData);
                        _grinPartsRepository.SaveAsync();
                    }

                    //Updating Binning Status in IqcItems
                    //if (iqcConfirmationItems.AcceptedQty == 0)
                    //{
                    //    var iqcItemData = await _iQCConfirmationItemsRepository.GetIQCConfirmationItemsDetailsbyGrinPartId(iqcConfirmationItems.GrinPartId);
                    //    iqcItemData.IsBinningCompleted = true;
                    //    if (iqcItemData.Remarks == null || string.IsNullOrWhiteSpace(iqcItemData.Remarks))
                    //    {
                    //        iqcItemData.Remarks = "Iqc Rejected for all";
                    //    }
                    //    else
                    //    {
                    //        iqcItemData.Remarks = iqcItemData.Remarks + "[Iqc Rejected for all]";
                    //    }
                    //    await _iQCConfirmationItemsRepository.UpdateIqcItems(iqcItemData);
                    //    _iQCConfirmationItemsRepository.SaveAsync();
                    //}

                    //Updating IQC Status in Grin

                    var grinPartsIqcStatuscount = await _grinPartsRepository.GetGrinPartsIqcStatusCount(grinPartsDetails.GrinsId);

                    if (grinPartsIqcStatuscount == 0)
                    {
                        var grinDetails = await _grinRepository.GetGrinByGrinNo(grinNumber);
                        grinDetails.IsIqcCompleted = true;
                        await _grinRepository.UpdateGrin(grinDetails);
                        _grinRepository.SaveAsync();

                    }

                    //Updating IQC Status in IQC

                    var grinIqcStatuscount = await _grinRepository.GetGrinIqcStatusCount(grinNumber);

                    if (grinIqcStatuscount > 0)
                    {
                        var iqcDetails = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);
                        iqcDetails.IsIqcCompleted = true;
                        await _iQCConfirmationRepository.UpdateIqc(iqcDetails);

                    }



                    //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterEnggAPI"],
                    //      "GetItemMasterByItemNumber?", "&ItemNumber=", iqcConfirmationItemsDto.ItemNumber));

                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = iqcConfirmationItemsDto.ItemNumber;
                    var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                        $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                    request.Headers.Add("Authorization", token);

                    var itemMasterObjectResult = await client.SendAsync(request);
                    if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                        getItemmResp = itemMasterObjectResult.StatusCode;

                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterObject = itemMasterObjectData.data;
                    //Inventory Update Code
                    decimal acceptedQty = iqcConfirmationItemsDto.AcceptedQty;
                    decimal rejectedQty = iqcConfirmationItemsDto.RejectedQty;
                    var grinPartsId = iqcConfirmationItemsDto.GrinPartId;
                    var grinPartsDetail = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartsId);
                    foreach (var projectNo in grinPartsDetail.ProjectNumbers)
                    {
                        //var grinNo = iqcConfirmation.GrinNumber;
                        //var grinPartsIds = projectNo.GrinPartsId;
                        //var itemNo = iqcConfirmationItemsDto.ItemNumber;
                        //var projectNos = projectNo.ProjectNumber;
                        //var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                        //    "GetInventoryDetailsByGrinNoandGrinId?", "GrinNo=", grinNo, "&GrinPartsId=",
                        //    grinPartsId, "&ItemNumber=", itemNo, "&ProjectNumber=", projectNos));

                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var itemNo = iqcConfirmationItemsDto.ItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(itemNo);
                        var grinNo = iqcConfirmation.GrinNumber;
                        var encodedgrinNo = Uri.EscapeDataString(grinNo);
                        var grinPartsIds = projectNo.GrinPartsId;
                        var projectNos = projectNo.ProjectNumber;
                        var encodedprojectNos = Uri.EscapeDataString(projectNos);
                        var lotNumber = grinPartsDetails.LotNumber;
                        if (string.IsNullOrWhiteSpace(lotNumber))
                        {
                            var errorMessage = $"Lot Number not found for Grin Part Id: {grinPartId}";

                            _logger.LogError(errorMessage);

                            serviceResponse.Data = null;
                            serviceResponse.Message = errorMessage;
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;

                            return BadRequest(serviceResponse);
                        }

                        var encodedlotNumber = Uri.EscapeDataString(lotNumber);

                        //var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                        //    $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                        //request1.Headers.Add("Authorization", token1);

                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                              $"GetInventoryDetailsByProjectNoandLotNo?ProjectNumber={encodedprojectNos}&LotNumber={encodedlotNumber}"));
                        request1.Headers.Add("Authorization", token1);

                        var inventoryObjectResult = await client1.SendAsync(request1);
                        if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvGrinId = inventoryObjectResult.StatusCode;
                        var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                        dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                        dynamic inventoryObject = inventoryObjectData.data;
                        if (inventoryObject != null)
                        {
                            decimal balanceQty = inventoryObject.balance_Quantity;
                            int flag1 = 0;
                            int flag2 = 0;
                            decimal bal = 0;
                            if (inventoryObject.balance_Quantity <= acceptedQty && inventoryObject.balance_Quantity != 0)
                            {
                                inventoryObject.warehouse = "IQC";
                                inventoryObject.location = "IQC";
                                inventoryObject.referenceID = iqcConfirmation.GrinNumber;
                                inventoryObject.referenceIDFrom = "IQC";
                               // inventoryObject.balance_Quantity -= acceptedQty;
                                acceptedQty -= balanceQty;

                            }
                            else if (inventoryObject.balance_Quantity > acceptedQty)
                            {
                                if (acceptedQty == 0)
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceID = iqcConfirmation.GrinNumber;
                                    inventoryObject.referenceIDFrom = "IQC";
                                    flag1 = 1;
                                }
                                else
                                {
                                    bal = inventoryObject.balance_Quantity - acceptedQty;
                                    if (bal != 0)
                                    {
                                        flag2 = 1;
                                    }
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceID = iqcConfirmation.GrinNumber;
                                    inventoryObject.referenceIDFrom = "IQC";
                                    acceptedQty = 0;
                                }
                            }

                            if (inventoryObject.balance_Quantity == 0) { inventoryObject.isStockAvailable = 0; }
                            var json = JsonConvert.SerializeObject(inventoryObject);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            //HttpResponseMessage response = await _httpClient.PutAsync(string.Concat(_config["InventoryAPI"],
                            //    "UpdateInventory?id=", inventoryObject.id), data);

                            var client5 = _clientFactory.CreateClient();
                            var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryAPI"],
                            "UpdateInventory?id=", inventoryObject.id))
                            {
                                Content = data
                            };
                            request5.Headers.Add("Authorization", token5);

                            var response = await client5.SendAsync(request5);
                            if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;

                            //InventoryTranction Update Code

                            IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                            iqcInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                            iqcInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                            iqcInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                            iqcInventoryTranctionDto.Description = inventoryObject.description;
                            iqcInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                            iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                            iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                            iqcInventoryTranctionDto.Warehouse = "IQC";
                            iqcInventoryTranctionDto.From_Location = "GRIN";
                            iqcInventoryTranctionDto.TO_Location = "IQC";
                            iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo;
                            iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                            iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                            iqcInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;
                            iqcInventoryTranctionDto.ReferenceIDFrom = "IQC";
                            iqcInventoryTranctionDto.GrinMaterialType = "GRIN";
                            iqcInventoryTranctionDto.shopOrderNo = "";
                            iqcInventoryTranctionDto.IsStockAvailable = inventoryObject.isStockAvailable;
                            iqcInventoryTranctionDto.TransactionType = InventoryType.Inward;
                            iqcInventoryTranctionDto.Remarks = "IQCItem Done";

                            string rfqSourcingPPdetailsJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                            var contents = new StringContent(rfqSourcingPPdetailsJsons, Encoding.UTF8, "application/json");
                            var client7 = _clientFactory.CreateClient();
                            var token7 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request7 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                            "CreateInventoryTranction"))
                            {
                                Content = contents
                            };
                            request7.Headers.Add("Authorization", token7);

                            var inventoryTransResponses = await client7.SendAsync(request7);

                            if (inventoryTransResponses.StatusCode != HttpStatusCode.OK) createInvTrans1 = inventoryTransResponses.StatusCode;

                            IQCInventoryTranctionDto GRINInventoryTranctionDto = new IQCInventoryTranctionDto();
                            GRINInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                            GRINInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                            GRINInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                            GRINInventoryTranctionDto.Description = inventoryObject.description;
                            GRINInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                            GRINInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                            GRINInventoryTranctionDto.UOM = inventoryObject.uom;
                            GRINInventoryTranctionDto.Warehouse = "GRIN";
                            GRINInventoryTranctionDto.From_Location = "GRIN";
                            GRINInventoryTranctionDto.TO_Location = "IQC";
                            GRINInventoryTranctionDto.GrinNo = inventoryObject.grinNo;
                            GRINInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                            GRINInventoryTranctionDto.PartType = inventoryObject.partType;
                            GRINInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;
                            GRINInventoryTranctionDto.ReferenceIDFrom = "GRIN";
                            GRINInventoryTranctionDto.GrinMaterialType = "GRIN";
                            GRINInventoryTranctionDto.shopOrderNo = "";
                            GRINInventoryTranctionDto.Remarks = "GRINIQC Done";
                            GRINInventoryTranctionDto.IsStockAvailable = false;
                            GRINInventoryTranctionDto.TransactionType = InventoryType.Outward;

                            string rfqSourcingPPdetailsJsons_1 = JsonConvert.SerializeObject(GRINInventoryTranctionDto);
                            var contents_1 = new StringContent(rfqSourcingPPdetailsJsons_1, Encoding.UTF8, "application/json");
                            var client7_1 = _clientFactory.CreateClient();
                            var token7_1 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request7_1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                            "CreateInventoryTranction"))
                            {
                                Content = contents_1
                            };
                            request7_1.Headers.Add("Authorization", token7_1);

                            var inventoryTransResponses_1 = await client7_1.SendAsync(request7_1);
                            if (inventoryTransResponses_1.StatusCode != HttpStatusCode.OK) createInvTrans1 = inventoryTransResponses_1.StatusCode;

                            if (rejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
                            {
                                IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                grinInventoryDto.PartNumber = iqcConfirmationItemsDto.ItemNumber;
                                grinInventoryDto.LotNumber = grinPartsDetails.LotNumber;
                                grinInventoryDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                grinInventoryDto.Description = grinPartsDetails.ItemDescription;
                                grinInventoryDto.ProjectNumber = projectNos;
                                //grinInventoryDto.Balance_Quantity = Convert.ToDecimal(iqcConfirmationItemsDto.RejectedQty);
                                grinInventoryDto.Max = itemMasterObject.max;
                                grinInventoryDto.Min = itemMasterObject.min;
                                grinInventoryDto.UOM = grinPartsDetails.UOM;
                                grinInventoryDto.Warehouse = "Reject";
                                grinInventoryDto.Location = "Reject";
                                grinInventoryDto.GrinNo = iqcConfirmation.GrinNumber;
                                grinInventoryDto.GrinPartId = iqcConfirmationItemsDto.GrinPartId;
                                grinInventoryDto.PartType = itemMasterObject.itemType;  //We need to check this
                                grinInventoryDto.ReferenceID = iqcConfirmation.GrinNumber; // Convert.ToString(iQCConfirmationItems.Id) //;
                                grinInventoryDto.ReferenceIDFrom = "IQC";
                                grinInventoryDto.GrinMaterialType = "GRIN";
                                grinInventoryDto.ShopOrderNo = "";
                                if (flag1 == 1)
                                {
                                    grinInventoryDto.Balance_Quantity = balanceQty;
                                    rejectedQty -= balanceQty;
                                }
                                else if (flag2 == 1)
                                {
                                    grinInventoryDto.Balance_Quantity = bal;
                                    rejectedQty -= bal;
                                }
                                //var httpClientHandler = new HttpClientHandler();
                                //httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                                //var httpClient = new HttpClient(httpClientHandler);
                                string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(grinInventoryDto);
                                //var rfqApiUrl = _config["InventoryAPI"];
                                var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                                // var rfqCustomerIdResponse = await _httpClient.PostAsync($"{rfqApiUrl}CreateInventoryFromGrin", content);

                                var client6 = _clientFactory.CreateClient();
                                var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                "CreateInventoryFromGrin"))
                                {
                                    Content = content
                                };
                                request6.Headers.Add("Authorization", token6);

                                var rfqCustomerIdResponse = await client6.SendAsync(request6);
                                if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInv = rfqCustomerIdResponse.StatusCode;

                                //InventoryTranction Update Code
                                IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                iqcInventoryTranctionDtos.PartNumber = grinInventoryDto.PartNumber;
                                iqcInventoryTranctionDtos.LotNumber = grinInventoryDto.LotNumber;
                                iqcInventoryTranctionDtos.MftrPartNumber = grinInventoryDto.MftrPartNumber;
                                iqcInventoryTranctionDtos.Description = grinInventoryDto.Description;
                                iqcInventoryTranctionDtos.ProjectNumber = grinInventoryDto.ProjectNumber;
                                iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                iqcInventoryTranctionDtos.IsStockAvailable = true;
                                iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                iqcInventoryTranctionDtos.From_Location = "GRIN";
                                iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.GrinNo = grinInventoryDto.GrinNo;
                                iqcInventoryTranctionDtos.GrinPartId = grinInventoryDto.GrinPartId;
                                iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.GrinNo;
                                iqcInventoryTranctionDtos.ReferenceIDFrom = "IQC";
                                iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
                                iqcInventoryTranctionDtos.shopOrderNo = "";
                                iqcInventoryTranctionDtos.TransactionType = InventoryType.Inward; 
                                iqcInventoryTranctionDtos.Remarks = "IQCItem Done";

                                string iqcInventoryTranctionDtoJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDtos);
                                var contents1 = new StringContent(iqcInventoryTranctionDtoJsons, Encoding.UTF8, "application/json");
                                var client8 = _clientFactory.CreateClient();
                                var token8 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request8 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = contents1
                                };
                                request8.Headers.Add("Authorization", token8);

                                var inventoryTransResponses1 = await client8.SendAsync(request8);

                                if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK) createInvTrans = inventoryTransResponses1.StatusCode;
                            }

                        }

                    }

                    ////update accepted qty and rejected qty in grin model

                    var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinPartsQty(iqcConfirmationItems.GrinPartId, iqcConfirmationItems.AcceptedQty.ToString(), iqcConfirmationItems.RejectedQty.ToString());

                    var grinParts = _mapper.Map<GrinParts>(updatedGrinPartsQty);
                    if (itemMasterObject.poMaterialType == "ServiceItem") grinParts.IsBinningCompleted = true;
                    string result = await _grinPartsRepository.UpdateGrinQty(grinParts);
                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && getInvTrancGrinId == HttpStatusCode.OK && updateInvTranc == HttpStatusCode.OK && createInv == HttpStatusCode.OK && createInvTrans1 == HttpStatusCode.OK && createInvTrans == HttpStatusCode.OK && getItemmResp == HttpStatusCode.OK)
                    {
                        _iQCConfirmationItemsRepository.SaveAsync();
                        _grinPartsRepository.SaveAsync();
                        _iQCConfirmationRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create IQCConfirmationWithitems action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCConfirmationItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    if (serverKey == "avision")
                    {
                        var grinNum = iqcConfirmation.GrinNumber;
                        if (grinNum != null)
                        {
                            var iqcNum = grinNum.Replace("GRN", "IQC");
                            iqcConfirmation.IQCNumber = iqcNum;
                        }

                    }

                    var grinPartId = iqcConfirmationItemsDto.GrinPartId;
                    var grinPartsDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartId);
                    if (iqcConfirmationItemsDto.GrinPartId != grinPartsDetails.Id)
                    {
                        _logger.LogError($"Invalid Grin Part Id {grinPartId}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Invalid Grin Part Id {grinPartId}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }
                    if (grinPartsDetails.Qty <= (iqcConfirmationItems.AcceptedQty + iqcConfirmationItems.RejectedQty))
                    {
                        grinPartsDetails.AcceptedQty = iqcConfirmationItems.AcceptedQty;
                        grinPartsDetails.RejectedQty = iqcConfirmationItems.RejectedQty;
                        _grinPartsRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError("Grinpart Quantity should not be lesser than accepted + rejected Quantity .");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Grinpart Quantity should not be lesser than accepted + rejected Quantity ";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    //Updating IQC Status in IqcItem

                    iqcConfirmationItems.IsIqcCompleted = true;
                    iqcConfirmation.IQCConfirmationItems = new List<IQCConfirmationItems> { iqcConfirmationItems };
                    await _iQCConfirmationRepository.CreateIqc(iqcConfirmation);

                    //Updating IQC Status in GrinParts

                    grinPartsDetails.IsIqcCompleted = true;
                    await _grinPartsRepository.UpdateGrinQty(grinPartsDetails);
                    _grinPartsRepository.SaveAsync();

                    //Updating binning Status in GrinParts
                    if (iqcConfirmationItems.AcceptedQty == 0)
                    {
                        var grinPartsData = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(iqcConfirmationItems.GrinPartId);
                        grinPartsData.IsBinningCompleted = true;
                        if (grinPartsData.Remarks == null || string.IsNullOrWhiteSpace(grinPartsData.Remarks))
                        {
                            grinPartsData.Remarks = "Iqc Rejected for all";
                        }
                        else
                        {
                            grinPartsData.Remarks = grinPartsData.Remarks + "[Iqc Rejected for all]";
                        }
                        await _grinPartsRepository.UpdateGrinQty(grinPartsData);
                        _grinPartsRepository.SaveAsync();
                    }

                    //Updating Binning Status in IqcItems
                    //if (iqcConfirmationItems.AcceptedQty == 0)
                    //{
                    //    var iqcItemData = await _iQCConfirmationItemsRepository.GetIQCConfirmationItemsDetailsbyGrinPartId(iqcConfirmationItems.GrinPartId);
                    //    iqcItemData.IsBinningCompleted = true;
                    //    if (iqcItemData.Remarks == null || string.IsNullOrWhiteSpace(iqcItemData.Remarks))
                    //    {
                    //        iqcItemData.Remarks = "Iqc Rejected for all";
                    //    }
                    //    else
                    //    {
                    //        iqcItemData.Remarks = iqcItemData.Remarks + "[Iqc Rejected for all]";
                    //    }
                    //    await _iQCConfirmationItemsRepository.UpdateIqcItems(iqcItemData);
                    //    _iQCConfirmationItemsRepository.SaveAsync();
                    //}

                    //Updating IQC Status in Grin

                    var grinPartsIqcStatuscount = await _grinPartsRepository.GetGrinPartsIqcStatusCount(grinPartsDetails.GrinsId);

                    if (grinPartsIqcStatuscount == 0)
                    {
                        var grinDetails = await _grinRepository.GetGrinByGrinNo(grinNumber);
                        grinDetails.IsIqcCompleted = true;
                        await _grinRepository.UpdateGrin(grinDetails);
                        _grinRepository.SaveAsync();

                    }

                    //Updating IQC Status in IQC

                    var grinIqcStatuscount = await _grinRepository.GetGrinIqcStatusCount(grinNumber);

                    if (grinIqcStatuscount > 0)
                    {
                        var iqcDetails = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);
                        iqcDetails.IsIqcCompleted = true;
                        await _iQCConfirmationRepository.UpdateIqc(iqcDetails);

                    }

                    //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterEnggAPI"],
                    //      "GetItemMasterByItemNumber?", "&ItemNumber=", iqcConfirmationItemsDto.ItemNumber));

                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = iqcConfirmationItemsDto.ItemNumber;
                    var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                        $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                    request.Headers.Add("Authorization", token);

                    var itemMasterObjectResult = await client.SendAsync(request);
                    if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                        getItemmResp = itemMasterObjectResult.StatusCode;

                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterObject = itemMasterObjectData.data;
                    //Inventory Update Code
                    decimal acceptedQty = iqcConfirmationItemsDto.AcceptedQty;
                    decimal rejectedQty = iqcConfirmationItemsDto.RejectedQty;
                    var grinPartsId = iqcConfirmationItemsDto.GrinPartId;
                    var grinPartsDetail = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartsId);
                    foreach (var projectNo in grinPartsDetail.ProjectNumbers)
                    {
                        //var grinNo = iqcConfirmation.GrinNumber;
                        //var grinPartsIds = projectNo.GrinPartsId;
                        //var itemNo = iqcConfirmationItemsDto.ItemNumber;
                        //var projectNos = projectNo.ProjectNumber;
                        //var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                        //    "GetInventoryDetailsByGrinNoandGrinId?", "GrinNo=", grinNo, "&GrinPartsId=",
                        //    grinPartsId, "&ItemNumber=", itemNo, "&ProjectNumber=", projectNos));

                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var itemNo = iqcConfirmationItemsDto.ItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(itemNo);
                        var grinNo = iqcConfirmation.GrinNumber;
                        var encodedgrinNo = Uri.EscapeDataString(grinNo);
                        var grinPartsIds = projectNo.GrinPartsId;
                        var projectNos = projectNo.ProjectNumber;
                        var encodedprojectNos = Uri.EscapeDataString(projectNos);
                        var lotNumber = grinPartsDetails.LotNumber;
                        if (string.IsNullOrWhiteSpace(lotNumber))
                        {
                            var errorMessage = $"Lot Number not found for Grin Part Id: {grinPartId}";

                            _logger.LogError(errorMessage);

                            serviceResponse.Data = null;
                            serviceResponse.Message = errorMessage;
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;

                            return BadRequest(serviceResponse);
                        }
                        var encodedlotNumber = Uri.EscapeDataString(lotNumber);

                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                            $"GetInventoryDetailsByProjectNoandLotNo?ProjectNumber={encodedprojectNos}&LotNumber={encodedlotNumber}"));
                        request1.Headers.Add("Authorization", token1);



                        //var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                        //    $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                        //request1.Headers.Add("Authorization", token1);

                        var inventoryObjectResult = await client1.SendAsync(request1);
                        if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvGrinId = inventoryObjectResult.StatusCode;
                        var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                        dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                        dynamic inventoryObject = inventoryObjectData.data;
                        if (inventoryObject != null)
                        {
                            decimal balanceQty = inventoryObject.balance_Quantity;
                            int flag1 = 0;
                            int flag2 = 0;
                            decimal bal = 0;
                            if (inventoryObject.balance_Quantity <= acceptedQty && inventoryObject.balance_Quantity != 0)
                            {
                                inventoryObject.warehouse = "IQC";
                                inventoryObject.location = "IQC";
                                inventoryObject.referenceID = iqcConfirmation.GrinNumber;
                                inventoryObject.referenceIDFrom = "IQC";
                               // inventoryObject.balance_Quantity = acceptedQty;
                                acceptedQty -= balanceQty;

                            }
                            else if (inventoryObject.balance_Quantity > acceptedQty)
                            {

                                if (acceptedQty == 0)
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceID = iqcConfirmation.GrinNumber;
                                    inventoryObject.referenceIDFrom = "IQC";
                                    flag1 = 1;
                                }
                                else
                                {
                                    bal = inventoryObject.balance_Quantity - acceptedQty;
                                    if (bal != 0)
                                    {
                                        flag2 = 1;
                                    }
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceID = iqcConfirmation.GrinNumber;
                                    inventoryObject.referenceIDFrom = "IQC";
                                    acceptedQty = 0;
                                }
                            }

                            if (inventoryObject.balance_Quantity == 0) { inventoryObject.isStockAvailable = 0; }
                            var json = JsonConvert.SerializeObject(inventoryObject);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            //var response = await _httpClient.PutAsync(string.Concat(_config["InventoryAPI"],
                            //    "UpdateInventory?id=", inventoryObject.id), data);

                            var client5 = _clientFactory.CreateClient();
                            var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryAPI"],
                            "UpdateInventory?id=", inventoryObject.id))
                            {
                                Content = data
                            };
                            request5.Headers.Add("Authorization", token5);

                            var response = await client5.SendAsync(request5);
                            if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;


                            //InventoryTranction Update Code

                            IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                            iqcInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                            iqcInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                            iqcInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                            iqcInventoryTranctionDto.Description = inventoryObject.description;
                            iqcInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                            iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                            iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                            iqcInventoryTranctionDto.Warehouse = "IQC";
                            iqcInventoryTranctionDto.From_Location = "GRIN";
                            iqcInventoryTranctionDto.TO_Location = "IQC";
                            iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo;
                            iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                            iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                            iqcInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;
                            iqcInventoryTranctionDto.ReferenceIDFrom = "IQC";
                            iqcInventoryTranctionDto.GrinMaterialType = "GRIN";
                            iqcInventoryTranctionDto.shopOrderNo = "";
                            iqcInventoryTranctionDto.IsStockAvailable = inventoryObject.isStockAvailable;
                            iqcInventoryTranctionDto.TransactionType = InventoryType.Inward;
                            iqcInventoryTranctionDto.Remarks = "IQCItem Done";

                            string rfqSourcingPPdetailsJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                            var contents = new StringContent(rfqSourcingPPdetailsJsons, Encoding.UTF8, "application/json");
                            var client7 = _clientFactory.CreateClient();
                            var token7 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request7 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                            "CreateInventoryTranction"))
                            {
                                Content = contents
                            };
                            request7.Headers.Add("Authorization", token7);

                            var inventoryTransResponses = await client7.SendAsync(request7);

                            if (inventoryTransResponses.StatusCode != HttpStatusCode.OK) createInvTrans1 = inventoryTransResponses.StatusCode;

                            IQCInventoryTranctionDto GRINInventoryTranctionDto = new IQCInventoryTranctionDto();
                            GRINInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                            GRINInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                            GRINInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                            GRINInventoryTranctionDto.Description = inventoryObject.description;
                            GRINInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                            GRINInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                            GRINInventoryTranctionDto.UOM = inventoryObject.uom;
                            GRINInventoryTranctionDto.Warehouse = "GRIN";
                            GRINInventoryTranctionDto.From_Location = "GRIN";
                            GRINInventoryTranctionDto.TO_Location = "IQC";
                            GRINInventoryTranctionDto.GrinNo = inventoryObject.grinNo;
                            GRINInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                            GRINInventoryTranctionDto.PartType = inventoryObject.partType;
                            GRINInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;
                            GRINInventoryTranctionDto.ReferenceIDFrom = "GRIN";
                            GRINInventoryTranctionDto.GrinMaterialType = "GRIN";
                            GRINInventoryTranctionDto.shopOrderNo = "";
                            GRINInventoryTranctionDto.Remarks = "GRINIQC Done";
                            GRINInventoryTranctionDto.IsStockAvailable = false;
                            GRINInventoryTranctionDto.TransactionType = InventoryType.Outward;

                            string rfqSourcingPPdetailsJsons_1 = JsonConvert.SerializeObject(GRINInventoryTranctionDto);
                            var contents_1 = new StringContent(rfqSourcingPPdetailsJsons_1, Encoding.UTF8, "application/json");
                            var client7_1 = _clientFactory.CreateClient();
                            var token7_1 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request7_1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                            "CreateInventoryTranction"))
                            {
                                Content = contents_1
                            };
                            request7_1.Headers.Add("Authorization", token7_1);

                            var inventoryTransResponses_1 = await client7_1.SendAsync(request7_1);
                            if (inventoryTransResponses_1.StatusCode != HttpStatusCode.OK) createInvTrans1 = inventoryTransResponses_1.StatusCode;

                            if (rejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
                            {
                                IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                grinInventoryDto.PartNumber = iqcConfirmationItemsDto.ItemNumber;
                                grinInventoryDto.LotNumber = grinPartsDetails.LotNumber;
                                grinInventoryDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                grinInventoryDto.Description = grinPartsDetails.ItemDescription;
                                grinInventoryDto.ProjectNumber = projectNos;
                                //grinInventoryDto.Balance_Quantity = Convert.ToDecimal(iqcConfirmationItemsDto.RejectedQty);
                                grinInventoryDto.Max = itemMasterObject.max;
                                grinInventoryDto.Min = itemMasterObject.min;
                                grinInventoryDto.UOM = grinPartsDetails.UOM;
                                grinInventoryDto.Warehouse = "Reject";
                                grinInventoryDto.Location = "Reject";
                                grinInventoryDto.GrinNo = iqcConfirmation.GrinNumber;
                                grinInventoryDto.GrinPartId = iqcConfirmationItemsDto.GrinPartId;
                                grinInventoryDto.PartType = itemMasterObject.itemType;  //We need to check this
                                grinInventoryDto.ReferenceID = iqcConfirmation.GrinNumber; // Convert.ToString(iQCConfirmationItems.Id) //;
                                grinInventoryDto.ReferenceIDFrom = "IQC";
                                grinInventoryDto.GrinMaterialType = "GRIN";
                                grinInventoryDto.ShopOrderNo = "";
                                if (flag1 == 1)
                                {
                                    grinInventoryDto.Balance_Quantity = balanceQty;
                                    rejectedQty -= balanceQty;
                                }
                                else if (flag2 == 1)
                                {
                                    grinInventoryDto.Balance_Quantity = bal;
                                    rejectedQty -= bal;
                                }
                                //var httpClientHandler = new HttpClientHandler();
                                //httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                                //var httpClient = new HttpClient(httpClientHandler);
                                string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(grinInventoryDto);
                                // var rfqApiUrl = _config["InventoryAPI"];
                                var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                                //var rfqCustomerIdResponse = await _httpClient.PostAsync($"{rfqApiUrl}CreateInventoryFromGrin", content);

                                var client6 = _clientFactory.CreateClient();
                                var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                "CreateInventoryFromGrin"))
                                {
                                    Content = content
                                };
                                request6.Headers.Add("Authorization", token6);

                                var rfqCustomerIdResponse = await client6.SendAsync(request6);
                                if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInv = rfqCustomerIdResponse.StatusCode;

                                //InventoryTranction Update Code

                                IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                iqcInventoryTranctionDtos.PartNumber = grinInventoryDto.PartNumber;
                                iqcInventoryTranctionDtos.LotNumber = grinInventoryDto.LotNumber;
                                iqcInventoryTranctionDtos.MftrPartNumber = grinInventoryDto.MftrPartNumber;
                                iqcInventoryTranctionDtos.Description = grinInventoryDto.Description;
                                iqcInventoryTranctionDtos.ProjectNumber = grinInventoryDto.ProjectNumber;
                                iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                iqcInventoryTranctionDtos.IsStockAvailable = true;
                                iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                iqcInventoryTranctionDtos.From_Location = "GRIN";
                                iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.GrinNo = grinInventoryDto.GrinNo;
                                iqcInventoryTranctionDtos.GrinPartId = grinInventoryDto.GrinPartId;
                                iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.GrinNo;
                                iqcInventoryTranctionDtos.ReferenceIDFrom = "IQC";
                                iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
                                iqcInventoryTranctionDtos.shopOrderNo = "";
                                iqcInventoryTranctionDtos.TransactionType = InventoryType.Inward;
                                iqcInventoryTranctionDtos.Remarks = "IQCItem Done";

                                string iqcInventoryTranctionDtoJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDtos);
                                var contents1 = new StringContent(iqcInventoryTranctionDtoJsons, Encoding.UTF8, "application/json");
                                var client8 = _clientFactory.CreateClient();
                                var token8 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request8 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = contents1
                                };
                                request8.Headers.Add("Authorization", token8);

                                var inventoryTransResponses1 = await client8.SendAsync(request8);
                                if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK) createInvTrans = inventoryTransResponses1.StatusCode;
                            }

                        }

                    }
                    ////update accepted qty and rejected qty in grin model

                    var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinPartsQty(iqcConfirmationItems.GrinPartId, iqcConfirmationItems.AcceptedQty.ToString(), iqcConfirmationItems.RejectedQty.ToString());

                    var grinParts = _mapper.Map<GrinParts>(updatedGrinPartsQty);

                    string result = await _grinPartsRepository.UpdateGrinQty(grinParts);

                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && getInvTrancGrinId == HttpStatusCode.OK && updateInvTranc == HttpStatusCode.OK && createInv == HttpStatusCode.OK && createInvTrans1 == HttpStatusCode.OK && createInvTrans == HttpStatusCode.OK && getItemmResp == HttpStatusCode.OK)
                    {
                        _grinPartsRepository.SaveAsync();
                        _iQCConfirmationRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create IQCConfirmationWithitems action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCConfirmation and IQCConfirmationItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateIQCConfirmationItems API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateIQCConfirmationItems API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}


