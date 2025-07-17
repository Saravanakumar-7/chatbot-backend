using System.Dynamic;
using System.Net;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Azure;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Repository;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class OpenGrinController : ControllerBase
    {
        private readonly IOpenGrinRepository _openGrinRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        private readonly String _createdBy;
        private readonly String _unitname;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OpenGrinController(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, IOpenGrinRepository openGrinRepository, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _openGrinRepository = openGrinRepository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOpenGrinDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<OpenGrinDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinDto>>();

            try
            {
                var openGrinDetails = await _openGrinRepository.GetAllOpenGrinDetails(pagingParameter, searchParams);
                var metadata = new
                {
                    openGrinDetails.TotalCount,
                    openGrinDetails.PageSize,
                    openGrinDetails.CurrentPage,
                    openGrinDetails.HasNext,
                    openGrinDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all OpenGrin");
                var result = _mapper.Map<IEnumerable<OpenGrinDto>>(openGrinDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenGrin";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllOpenGrinDetails API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllOpenGrinDetails API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOpenGrinSPReport([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<OpenGrin_SPReport>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrin_SPReport>>();
            try
            {
                var products = await _openGrinRepository.GetOpenGrinSPReport(pagingParameter);

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
                    serviceResponse.Message = $"Grin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Grin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Grin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOpenGrinSPReport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOpenGrinSPReport API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOpenGrinSPReportForTrans([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<OpenGrinSpReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinSpReportForTrans>>();
            try
            {
                var products = await _openGrinRepository.GetOpenGrinSPReportForTrans(pagingParameter);

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
                    serviceResponse.Message = $"Grin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Grin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Grin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOpenGrinSPReportForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOpenGrinSPReportForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        [HttpPost]
        public async Task<IActionResult> GetOpenGrinSPReportWithParam([FromBody] OpenGrinReportWithParamDto openGrinReportWithParamDto)
        {
            ServiceResponse<IEnumerable<OpenGrin_SPReport>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrin_SPReport>>();
            try
            {
                var products = await _openGrinRepository.GetOpenGrinSPReportWithParam(openGrinReportWithParamDto.OpenGrinNumber, openGrinReportWithParamDto.SenderName,
                                                                                                                                                    openGrinReportWithParamDto.ReceiptRefNo);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Grin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Grin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Grin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOpenGrinSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOpenGrinSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportGetOpenGrinSPReportWithParamOrDateToExcel([FromBody] OpenGrinReportWithParamWithDateDto openGrinReportWithParamWithDateDto)
        {

            ServiceResponse<OpenGrin_SPReport> serviceResponse = new ServiceResponse<OpenGrin_SPReport>();
            try
            {

                if (openGrinReportWithParamWithDateDto is null)
                {
                    _logger.LogError("openGrinReportWithParamWithDateDto object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "openGrinReportWithParamWithDateDto object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid openGrinReportWithParamWithDateDto object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                bool hasParams = !string.IsNullOrEmpty(openGrinReportWithParamWithDateDto.OpenGrinNumber)
                               || !string.IsNullOrEmpty(openGrinReportWithParamWithDateDto.SenderName)
                               || !string.IsNullOrEmpty(openGrinReportWithParamWithDateDto.ReceiptRefNo);

                bool hasDate = openGrinReportWithParamWithDateDto.FromDate != null || openGrinReportWithParamWithDateDto.ToDate != null;

                if (hasParams && hasDate)
                {
                    _logger.LogError("Input object must contain either filter parameters or date range, not both.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or a date range, not both.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!hasParams && (openGrinReportWithParamWithDateDto.FromDate == null || openGrinReportWithParamWithDateDto.ToDate == null))
                {
                    _logger.LogError("Input object must contain either at least one filter parameter or both from and to dates.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or both from and to dates.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);

                }

                var openGrinList = Enumerable.Empty<OpenGrin_SPReport>();

                if (hasParams && !hasDate)
                {

                    openGrinList = await _openGrinRepository.GetOpenGrinSPReportWithParam(
                        openGrinReportWithParamWithDateDto.OpenGrinNumber,
                        openGrinReportWithParamWithDateDto.SenderName, openGrinReportWithParamWithDateDto.ReceiptRefNo);
                }


                if (!hasParams && (openGrinReportWithParamWithDateDto.FromDate != null && openGrinReportWithParamWithDateDto.ToDate != null))
                {

                    openGrinList = await _openGrinRepository.GetOpenGrinSPReportWithDate(
                        openGrinReportWithParamWithDateDto.FromDate,
                        openGrinReportWithParamWithDateDto.ToDate);
                }



                // Create Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("OpenGrinReport");

                // Header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("KPN");
                headerRow.CreateCell(1).SetCellValue("Description");
                headerRow.CreateCell(2).SetCellValue("Item Type");
                headerRow.CreateCell(3).SetCellValue("UOM");
                headerRow.CreateCell(4).SetCellValue("Open GRIN Number");
                headerRow.CreateCell(5).SetCellValue("Sender Name");
                headerRow.CreateCell(6).SetCellValue("Sender ID");
                headerRow.CreateCell(7).SetCellValue("Receipt Ref No");
                headerRow.CreateCell(8).SetCellValue("Reference SO Number");
                headerRow.CreateCell(9).SetCellValue("Qty");
                headerRow.CreateCell(10).SetCellValue("Warehouse");
                headerRow.CreateCell(11).SetCellValue("Location");
                headerRow.CreateCell(12).SetCellValue("Returnable");
                headerRow.CreateCell(13).SetCellValue("Remarks");
                headerRow.CreateCell(14).SetCellValue("Serial No");
                headerRow.CreateCell(15).SetCellValue("Returned By");
                headerRow.CreateCell(16).SetCellValue("Created By");
                headerRow.CreateCell(17).SetCellValue("Created On");
                headerRow.CreateCell(18).SetCellValue("Last Modified By");
                headerRow.CreateCell(19).SetCellValue("Last Modified On");

                // Populate data
                int rowIndex = 1;
                foreach (var item in openGrinList)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.KPN ?? "");
                    row.CreateCell(1).SetCellValue(item.Description ?? "");
                    row.CreateCell(2).SetCellValue(item.ItemType);
                    row.CreateCell(3).SetCellValue(item.UOM ?? "");
                    row.CreateCell(4).SetCellValue(item.OpenGrinNumber ?? "");
                    row.CreateCell(5).SetCellValue(item.SenderName ?? "");
                    row.CreateCell(6).SetCellValue(item.SenderId ?? "");
                    row.CreateCell(7).SetCellValue(item.ReceiptRefNo ?? "");
                    row.CreateCell(8).SetCellValue(item.ReferenceSONumber ?? "");
                    row.CreateCell(9).SetCellValue((double)(item.Qty ?? 0));
                    row.CreateCell(10).SetCellValue(item.Warehouse ?? "");
                    row.CreateCell(11).SetCellValue(item.Location ?? "");
                    row.CreateCell(12).SetCellValue(Convert.ToInt32(item.Returnable) == 1 ? "true" : "false");
                    row.CreateCell(13).SetCellValue(item.Remarks ?? "");
                    row.CreateCell(14).SetCellValue(item.SerialNo ?? "");
                    row.CreateCell(15).SetCellValue(item.ReturnedBy ?? "");
                    row.CreateCell(16).SetCellValue(item.CreatedBy ?? "");
                    row.CreateCell(17).SetCellValue(item.CreatedOn?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(18).SetCellValue(item.LastModifiedBy ?? "");
                    row.CreateCell(19).SetCellValue(item.LastModifiedOn?.ToString("MM/dd/yyyy") ?? "");
                }



                // Export Excel
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();
                    return File(excelBytes,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "OpenGrinSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }




        [HttpPost]
        public async Task<IActionResult> GetOpenGrinSPReportWithParamForTrans([FromBody] OpenGrinReportWithParamForTransDto openGrinReportWithParamDto)
        {
            ServiceResponse<IEnumerable<OpenGrinSpReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinSpReportForTrans>>();
            try
            {
                var products = await _openGrinRepository.GetOpenGrinSPReportWithParamForTrans(openGrinReportWithParamDto.ItemNumber, openGrinReportWithParamDto.OpenGrinNumber, openGrinReportWithParamDto.SenderName,
                                                                                          openGrinReportWithParamDto.ReceiptRefNo, openGrinReportWithParamDto.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Grin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Grin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OpenGrinSPReportWithParamForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOpenGrinSPReportWithParamForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOpenGrinSPReportWithParamForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }




        [HttpPost]
        public async Task<IActionResult> ExportGetOpenGrinSPReportWithParamOrDateToExcelForTras([FromBody] OpenGrinReportWithParamAndDateForTransDto openGrinReportWithParamAndDateForTransDto)
        {

            ServiceResponse<OpenGrinSpReportForTrans> serviceResponse = new ServiceResponse<OpenGrinSpReportForTrans>();
            try
            {

                if (openGrinReportWithParamAndDateForTransDto is null)
                {
                    _logger.LogError("openGrinReportWithParamAndDateForTransDto object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "openGrinReportWithParamAndDateForTransDto object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid openGrinReportWithParamAndDateForTransDto object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                bool hasParams = !string.IsNullOrEmpty(openGrinReportWithParamAndDateForTransDto.OpenGrinNumber)
                               || !string.IsNullOrEmpty(openGrinReportWithParamAndDateForTransDto.SenderName)
                               || !string.IsNullOrEmpty(openGrinReportWithParamAndDateForTransDto.ReceiptRefNo)
                               || !string.IsNullOrEmpty(openGrinReportWithParamAndDateForTransDto.ItemNumber)
                               || !string.IsNullOrEmpty(openGrinReportWithParamAndDateForTransDto.ProjectNumber);

                bool hasDate = openGrinReportWithParamAndDateForTransDto.FromDate != null || openGrinReportWithParamAndDateForTransDto.ToDate != null;

                if (hasParams && hasDate)
                {
                    _logger.LogError("Input object must contain either filter parameters or date range, not both.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or a date range, not both.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!hasParams && (openGrinReportWithParamAndDateForTransDto.FromDate == null || openGrinReportWithParamAndDateForTransDto.ToDate == null))
                {
                    _logger.LogError("Input object must contain either at least one filter parameter or both from and to dates.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or both from and to dates.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);

                }

                var openGrinList = Enumerable.Empty<OpenGrinSpReportForTrans>();

                if (hasParams && !hasDate)
                {

                    openGrinList = await _openGrinRepository.GetOpenGrinSPReportWithParamForTrans(openGrinReportWithParamAndDateForTransDto.ItemNumber,
                        openGrinReportWithParamAndDateForTransDto.OpenGrinNumber,
                        openGrinReportWithParamAndDateForTransDto.SenderName, openGrinReportWithParamAndDateForTransDto.ReceiptRefNo, openGrinReportWithParamAndDateForTransDto.ProjectNumber);
                }


                if (!hasParams && (openGrinReportWithParamAndDateForTransDto.FromDate != null && openGrinReportWithParamAndDateForTransDto.ToDate != null))
                {

                    openGrinList = await _openGrinRepository.GetOpenGrinSPReportWithDateForTrans(
                        openGrinReportWithParamAndDateForTransDto.FromDate,
                        openGrinReportWithParamAndDateForTransDto.ToDate);
                }



                // Create Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("OpenGrinTransReport");

                // Header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Open GRIN Number");
                headerRow.CreateCell(1).SetCellValue("Open GRIN Date");
                headerRow.CreateCell(2).SetCellValue("Sender Name");
                headerRow.CreateCell(3).SetCellValue("Sender ID");
                headerRow.CreateCell(4).SetCellValue("Receipt Ref No");
                headerRow.CreateCell(5).SetCellValue("Project Number");
                headerRow.CreateCell(6).SetCellValue("Item Number");
                headerRow.CreateCell(7).SetCellValue("MPN");
                headerRow.CreateCell(8).SetCellValue("Description");
                headerRow.CreateCell(9).SetCellValue("UOM");
                headerRow.CreateCell(10).SetCellValue("Qty");
                headerRow.CreateCell(11).SetCellValue("Lot Number");
                headerRow.CreateCell(12).SetCellValue("Item Type");
                headerRow.CreateCell(13).SetCellValue("Warehouse");
                headerRow.CreateCell(14).SetCellValue("Location");
                headerRow.CreateCell(15).SetCellValue("Serial No");
                headerRow.CreateCell(16).SetCellValue("Remarks");

                // Populate data
                int rowIndex = 1;
                foreach (var item in openGrinList)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.OpenGrinNumber ?? "");
                    row.CreateCell(1).SetCellValue(item.OpenGrinDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(2).SetCellValue(item.SenderName ?? "");
                    row.CreateCell(3).SetCellValue(item.SenderId ?? "");
                    row.CreateCell(4).SetCellValue(item.ReceiptRefNo ?? "");
                    row.CreateCell(5).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(6).SetCellValue(item.ItemNumber ?? "");
                    row.CreateCell(7).SetCellValue(item.MPN ?? "");
                    row.CreateCell(8).SetCellValue(item.Description ?? "");
                    row.CreateCell(9).SetCellValue(item.UOM ?? "");
                    row.CreateCell(10).SetCellValue((double)(item.Qty ?? 0));
                    row.CreateCell(11).SetCellValue(item.lotNumber ?? "");
                    row.CreateCell(12).SetCellValue(item.ItemType?.ToString() ?? "");
                    row.CreateCell(13).SetCellValue(item.Warehouse ?? "");
                    row.CreateCell(14).SetCellValue(item.Location ?? "");
                    row.CreateCell(15).SetCellValue(item.SerialNo ?? "");
                    row.CreateCell(16).SetCellValue(item.Remarks ?? "");
                }



                // Export Excel
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();
                    return File(excelBytes,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "OpenGrin.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }





        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetOpenGrinSPReportWithDateForTrans([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<OpenGrinSpReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinSpReportForTrans>>();
            try
            {
                var products = await _openGrinRepository.GetOpenGrinSPReportWithDateForTrans(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenGrin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenGrin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OpenGrinSPReportWithDateForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOpenGrinSPReportWithDateForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOpenGrinSPReportWithDateForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetOpenGrinSPReportWithParamForAvi([FromBody] OpenGrinReportWithParamForAviDto openGrinReportWithParamDto)
        {
            ServiceResponse<IEnumerable<OpenGrinSpReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinSpReportForTrans>>();
            try
            {
                var products = await _openGrinRepository.GetOpenGrinSPReportWithParamForAvi(openGrinReportWithParamDto.OpenGrinNumber, openGrinReportWithParamDto.SenderName,
                                                                                          openGrinReportWithParamDto.ReceiptRefNo, openGrinReportWithParamDto.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Grin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Grin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OpenGrinSPReportWithParamForAvi Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOpenGrinSPReportWithParamForAvi API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOpenGrinSPReportWithParamForAvi API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPost]
        public async Task<IActionResult> ExportGetOpenGrinSPReportWithParamOrDateToExcelForAvi([FromBody] OpenGrinReportWithParamAndDateForAviDto openGrinReportWithParamAndDateForAviDto)
        {

            ServiceResponse<OpenGrinSpReportForTrans> serviceResponse = new ServiceResponse<OpenGrinSpReportForTrans>();
            try
            {

                if (openGrinReportWithParamAndDateForAviDto is null)
                {
                    _logger.LogError("openGrinReportWithParamAndDateForAviDto object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "openGrinReportWithParamAndDateForAviDto object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid openGrinReportWithParamAndDateForAviDto object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                bool hasParams = !string.IsNullOrEmpty(openGrinReportWithParamAndDateForAviDto.OpenGrinNumber)
                               || !string.IsNullOrEmpty(openGrinReportWithParamAndDateForAviDto.SenderName)
                               || !string.IsNullOrEmpty(openGrinReportWithParamAndDateForAviDto.ReceiptRefNo)
                               || !string.IsNullOrEmpty(openGrinReportWithParamAndDateForAviDto.ProjectNumber);

                bool hasDate = openGrinReportWithParamAndDateForAviDto.FromDate != null || openGrinReportWithParamAndDateForAviDto.ToDate != null;

                if (hasParams && hasDate)
                {
                    _logger.LogError("Input object must contain either filter parameters or date range, not both.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or a date range, not both.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!hasParams && (openGrinReportWithParamAndDateForAviDto.FromDate == null || openGrinReportWithParamAndDateForAviDto.ToDate == null))
                {
                    _logger.LogError("Input object must contain either at least one filter parameter or both from and to dates.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or both from and to dates.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);

                }

                var openGrinList = Enumerable.Empty<OpenGrinSpReportForTrans>();

                if (hasParams && !hasDate)
                {

                    openGrinList = await _openGrinRepository.GetOpenGrinSPReportWithParamForAvi(openGrinReportWithParamAndDateForAviDto.OpenGrinNumber,
                        openGrinReportWithParamAndDateForAviDto.SenderName, openGrinReportWithParamAndDateForAviDto.ReceiptRefNo, openGrinReportWithParamAndDateForAviDto.ProjectNumber);
                }


                if (!hasParams && (openGrinReportWithParamAndDateForAviDto.FromDate != null && openGrinReportWithParamAndDateForAviDto.ToDate != null))
                {

                    openGrinList = await _openGrinRepository.GetOpenGrinSPReportWithDateForAvi(
                        openGrinReportWithParamAndDateForAviDto.FromDate,
                        openGrinReportWithParamAndDateForAviDto.ToDate);
                }



                // Create Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("OpenGrinTransReport");

                // Header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Open GRIN Number");
                headerRow.CreateCell(1).SetCellValue("Open GRIN Date");
                headerRow.CreateCell(2).SetCellValue("Sender Name");
                headerRow.CreateCell(3).SetCellValue("Sender ID");
                headerRow.CreateCell(4).SetCellValue("Receipt Ref No");
                headerRow.CreateCell(5).SetCellValue("Project Number");
                headerRow.CreateCell(6).SetCellValue("Item Number");
                headerRow.CreateCell(7).SetCellValue("MPN");
                headerRow.CreateCell(8).SetCellValue("Description");
                headerRow.CreateCell(9).SetCellValue("UOM");
                headerRow.CreateCell(10).SetCellValue("Qty");
                headerRow.CreateCell(11).SetCellValue("Lot Number");
                headerRow.CreateCell(12).SetCellValue("Item Type");
                headerRow.CreateCell(13).SetCellValue("Warehouse");
                headerRow.CreateCell(14).SetCellValue("Location");
                headerRow.CreateCell(15).SetCellValue("Serial No");
                headerRow.CreateCell(16).SetCellValue("Remarks");

                // Populate data
                int rowIndex = 1;
                foreach (var item in openGrinList)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.OpenGrinNumber ?? "");
                    row.CreateCell(1).SetCellValue(item.OpenGrinDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(2).SetCellValue(item.SenderName ?? "");
                    row.CreateCell(3).SetCellValue(item.SenderId ?? "");
                    row.CreateCell(4).SetCellValue(item.ReceiptRefNo ?? "");
                    row.CreateCell(5).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(6).SetCellValue(item.ItemNumber ?? "");
                    row.CreateCell(7).SetCellValue(item.MPN ?? "");
                    row.CreateCell(8).SetCellValue(item.Description ?? "");
                    row.CreateCell(9).SetCellValue(item.UOM ?? "");
                    row.CreateCell(10).SetCellValue((double)(item.Qty ?? 0));
                    row.CreateCell(11).SetCellValue(item.lotNumber ?? "");
                    row.CreateCell(12).SetCellValue(item.ItemType?.ToString() ?? "");
                    row.CreateCell(13).SetCellValue(item.Warehouse ?? "");
                    row.CreateCell(14).SetCellValue(item.Location ?? "");
                    row.CreateCell(15).SetCellValue(item.SerialNo ?? "");
                    row.CreateCell(16).SetCellValue(item.Remarks ?? "");
                }



                // Export Excel
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();
                    return File(excelBytes,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "OpenGrinSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }





        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetOpenGrinSPReportWithDateForAvi([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<OpenGrinSpReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinSpReportForTrans>>();
            try
            {
                var products = await _openGrinRepository.GetOpenGrinSPReportWithDateForAvi(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenGrin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenGrin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OpenGrinSPReportWithDateForAvi Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOpenGrinSPReportWithDateForAvi API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOpenGrinSPReportWithDateForAvi API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetOpenGrinSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<OpenGrin_SPReport>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrin_SPReport>>();
            try
            {
                var products = await _openGrinRepository.GetOpenGrinSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenGrin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenGrin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OpenGrin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOpenGrinSPReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOpenGrinSPReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchOpenGrinDate([FromQuery] SearchDateParames searchDateParam)
        {
            ServiceResponse<IEnumerable<OpenGrinDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinDto>>();
            try
            {
                var openGrinDetials = await _openGrinRepository.SearchOpenGrinDate(searchDateParam);
                var result = _mapper.Map<IEnumerable<OpenGrinDto>>(openGrinDetials);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenGrin";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in SearchOpenGrinDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SearchOpenGrinDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchOpenGrin([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<OpenGrinDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinDto>>();
            try
            {
                var openGrinDetails = await _openGrinRepository.SearchOpenGrin(searchParams);

                _logger.LogInfo("Returned all OpenGrin");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<OpenGrin, OpenGrinDto>().ReverseMap()
                    .ForMember(dest => dest.OpenGrinParts, opt => opt.MapFrom(src => src.OpenGrinParts));
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<OpenGrinDto>>(openGrinDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenGrin";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in SearchOpenGrin API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SearchOpenGrin API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAllOpenGrinWithItems([FromBody] OpenGrinSearchDto openGrinSearchDto)
        {
            ServiceResponse<IEnumerable<OpenGrinDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinDto>>();
            try
            {
                var openGrinDetails = await _openGrinRepository.GetAllOpenGrinWithItems(openGrinSearchDto);

                _logger.LogInfo("Returned all OpenGrin");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<OpenGrin, OpenGrinDto>().ReverseMap()
                    .ForMember(dest => dest.OpenGrinParts, opt => opt.MapFrom(src => src.OpenGrinParts));
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<OpenGrinDto>>(openGrinDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenGrinWithItems";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllOpenGrinWithItems API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllOpenGrinWithItems API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOpenGrinDetailsbyId(int id)
        {
            ServiceResponse<OpenGrinDto> serviceResponse = new ServiceResponse<OpenGrinDto>();

            try
            {
                var OpenGrinDetailsById = await _openGrinRepository.GetOpenGrinDetailsbyId(id);
                if (OpenGrinDetailsById == null)
                {
                    _logger.LogError($"OpenGrin details with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenGrin details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Binnings with id: {id}");

                    OpenGrinDto openGrinDto = _mapper.Map<OpenGrinDto>(OpenGrinDetailsById);
                    List<OpenGrinPartsDto> OpenGrinPartsList = new List<OpenGrinPartsDto>();

                    if (OpenGrinDetailsById.OpenGrinParts != null)
                    {
                        foreach (var openGrinDetails in OpenGrinDetailsById.OpenGrinParts)
                        {
                            OpenGrinPartsDto openGrinPartsDtos = _mapper.Map<OpenGrinPartsDto>(openGrinDetails);
                            openGrinPartsDtos.OpenGrinDetails = _mapper.Map<List<OpenGrinDetailsDto>>(openGrinDetails.OpenGrinDetails);
                            OpenGrinPartsList.Add(openGrinPartsDtos);
                        }
                    }

                    openGrinDto.OpenGrinParts = OpenGrinPartsList;
                    serviceResponse.Data = openGrinDto;
                    serviceResponse.Message = $"Returned OpenGrinbyId Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOpenGrinDetailsbyId API for the following id :{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOpenGrinDetailsbyId API for the following id :{id} \n {ex.Message}";
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
        public async Task<IActionResult> CreateOpenGrin([FromBody] OpenGrinPostDto openGrinPostDto)
        {
            ServiceResponse<OpenGrinPostDto> serviceResponse = new ServiceResponse<OpenGrinPostDto>();
            try
            {
                string serverKey = GetServerKey();

                if (openGrinPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OpenGrin object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("OpenGrin object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid OpenGrin object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid OpenGrin object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var openGrinDetails = _mapper.Map<OpenGrin>(openGrinPostDto);
                var openGrinPartsDto = openGrinPostDto.OpenGrinParts;
                var openGrinPartsDtoList = new List<OpenGrinParts>();
                HttpStatusCode createInv = HttpStatusCode.OK;
                HttpStatusCode createInvTrans = HttpStatusCode.OK;
                HttpStatusCode getItemmResp = HttpStatusCode.OK;

                string openGrinPartsId = "";
                if (openGrinPartsDto != null)
                {
                    for (int i = 0; i < openGrinPartsDto.Count; i++)
                    {
                        OpenGrinParts openGrinPartsDetails = _mapper.Map<OpenGrinParts>(openGrinPartsDto[i]);
                        openGrinPartsDetails.OpenGrinDetails = _mapper.Map<List<OpenGrinDetails>>(openGrinPartsDto[i].OpenGrinDetails);
                        openGrinPartsDtoList.Add(openGrinPartsDetails);
                      
                    }
                }

                openGrinDetails.OpenGrinParts = openGrinPartsDtoList;

                //generate 
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));

                if (serverKey == "avision")
                {
                    var openGrinNumber = await _openGrinRepository.GenerateOpenGrinNumberForAvision();
                    openGrinDetails.OpenGrinNumber = openGrinNumber;
                }
                else
                {
                    var dateFormat = days + months + years;
                    var openGrinNumber = await _openGrinRepository.GenerateOpenGrinNumber();
                    openGrinDetails.OpenGrinNumber = dateFormat + openGrinNumber;
                }

                await _openGrinRepository.CreateOpenGrin(openGrinDetails);
                _openGrinRepository.SaveAsync();

                //if (openGrinDetails.OpenGrinParts != null)
                //{
                //    foreach (var openGrinPart in openGrinDetails.OpenGrinParts)
                //    {
                //        var openGrinPartId = await _openGrinPartsRepository.GetOpenGrinPartsDetailsbyId(openGrinPart.Id);
                //        openGrinPartsId.LotNumber = openGrinDetails.OpenGrinNumber + openGrinPartId.Id;
                //        await _openGrinPartsRepository.UpdateOpenGrinParts(openGrinPartId);

                //    }
                //}


                //Create OpenGrin To Inventory And InventoryTransaction

                if (openGrinPartsDtoList != null)
                {
                    foreach (var openGrinParts in openGrinPartsDtoList)
                    {
                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = openGrinParts.ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request.Headers.Add("Authorization", token);

                        var itemMasterObjectResult = await client.SendAsync(request);
                        if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                            getItemmResp = itemMasterObjectResult.StatusCode;

                        var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                        var itemMasterObjectData = JsonConvert.DeserializeObject<OpenGrinInvDetails>(itemMasterObjectString);
                        var itemMasterObject = itemMasterObjectData.data;
                        if (itemMasterObject.itemmasterAlternate.Count() > 0)
                        {
                            foreach (var openGrinDetail in openGrinParts.OpenGrinDetails)
                            {
                                OGInventoryDtoPost inventory = new OGInventoryDtoPost();

                                inventory.PartNumber = openGrinParts.ItemNumber;
                                inventory.MftrPartNumber = itemMasterObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                                inventory.Description = openGrinParts.Description;
                                inventory.ProjectNumber = openGrinParts.ReferenceSONumber;
                                inventory.Balance_Quantity = openGrinDetail.Qty;
                                inventory.IsStockAvailable = true;
                                inventory.Max = itemMasterObject.max;
                                inventory.Min = itemMasterObject.min;
                                inventory.UOM = openGrinParts.UOM;
                                inventory.Warehouse = openGrinDetail.Warehouse;
                                inventory.Location = openGrinDetail.Location;
                                inventory.GrinNo = openGrinDetails.OpenGrinNumber;
                                inventory.GrinPartId = 0;
                                inventory.PartType = openGrinParts.ItemType; // we have to take parttype from grinparts model;
                                inventory.GrinMaterialType = "";
                                inventory.ReferenceID = Convert.ToString(openGrinParts.Id);
                                inventory.ReferenceIDFrom = "OpenGrin";
                                inventory.ShopOrderNo = "";
                                inventory.Unit = "";
                                inventory.LotNumber = openGrinDetails.OpenGrinNumber + openGrinParts.Id;


                                var json = JsonConvert.SerializeObject(inventory);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");
                                //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventoryFromGrin"), data);

                                var client1 = _clientFactory.CreateClient();
                                var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                                var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                "CreateInventoryFromGrin"))
                                {
                                    Content = data
                                };
                                request1.Headers.Add("Authorization", token1);

                                var response = await client1.SendAsync(request1);
                                if (response.StatusCode != HttpStatusCode.OK) createInv = response.StatusCode;


                                OGInventoryTranctionDto inventoryTranction = new OGInventoryTranctionDto();

                                inventoryTranction.PartNumber = openGrinParts.ItemNumber;
                                inventoryTranction.MftrPartNumber = itemMasterObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                                inventoryTranction.LotNumber = inventory.LotNumber;
                                inventoryTranction.Description = openGrinParts.Description;
                                inventoryTranction.ProjectNumber = openGrinParts.ReferenceSONumber;
                                inventoryTranction.Issued_Quantity = openGrinDetail.Qty;
                                inventoryTranction.IsStockAvailable = true;
                                inventoryTranction.UOM = openGrinParts.UOM;
                                inventoryTranction.Issued_By = _createdBy;
                                inventoryTranction.Issued_DateTime = DateTime.Now;
                                inventoryTranction.Warehouse = openGrinDetail.Warehouse;
                                inventoryTranction.From_Location = openGrinDetail.Location;
                                inventoryTranction.TO_Location = openGrinDetail.Location;
                                inventoryTranction.GrinNo = openGrinDetails.OpenGrinNumber;
                                inventoryTranction.GrinPartId = 0;
                                inventoryTranction.PartType = openGrinParts.ItemType; // we have to take parttype from grinparts model;
                                inventoryTranction.GrinMaterialType = "OpenGrin";
                                inventoryTranction.ReferenceID = Convert.ToString(openGrinParts.Id);
                                inventoryTranction.ReferenceIDFrom = "OpenGrin";
                                inventoryTranction.ShopOrderNo = "";
                                inventoryTranction.Remarks = "OpenGrin Done";


                                var jsons = JsonConvert.SerializeObject(inventoryTranction);
                                var datas = new StringContent(jsons, Encoding.UTF8, "application/json");
                                // var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), datas);

                                var client2 = _clientFactory.CreateClient();
                                var token2 = HttpContext.Request.Headers["Authorization"].ToString();

                                var request2 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = datas
                                };
                                request2.Headers.Add("Authorization", token2);

                                var responses = await client1.SendAsync(request2);
                                if (responses.StatusCode != HttpStatusCode.OK) createInvTrans = responses.StatusCode;

                            }
                        }
                        else
                        {
                            _logger.LogError($"Something went wrong inside Create OpenGrin action: ItemMasterAlternates is not available for the ItemNumber Other Service Calling");
                            serviceResponse.Data = null;
                            serviceResponse.Message = "Saving Failed";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                            return StatusCode(500, serviceResponse);
                        }
                    }
                }
               

                if ( createInv == HttpStatusCode.OK && createInvTrans == HttpStatusCode.OK && getItemmResp == HttpStatusCode.OK)
                {
                    _openGrinRepository.SaveAsync();
                }
                else
                {
                    _logger.LogError($"Something went wrong inside Create OpenGrin action: Create Inventory Other Service Calling");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Saving Failed";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
                

                serviceResponse.Data = null;
                serviceResponse.Message = "OpenGrin Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateOpenGrin API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateOpenGrin API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOpenGrin(int id,[FromBody] OpenGrinUpdateDto openGrinUpdateDto)
        {
            ServiceResponse<OpenGrinUpdateDto> serviceResponse = new ServiceResponse<OpenGrinUpdateDto>();
            try
            {
                if (openGrinUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OpenGrin object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError("OpenGrin object sent from client is null.");
                    return Ok(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid OpenGrin object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError("Invalid OpenGrin object sent from client.");
                    return Ok(serviceResponse);
                }

                var openGrinDetailById = await _openGrinRepository.GetOpenGrinDetailsbyId(id);
                if (openGrinDetailById is null)
                {
                    _logger.LogError($"Binning details with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Grin with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var openGrinDetails = _mapper.Map<OpenGrin>(openGrinUpdateDto);
                var openGrinPartsDto = openGrinUpdateDto.OpenGrinParts;
                var openGrinPartsDtoList = new List<OpenGrinParts>();

                if (openGrinPartsDto != null)
                {
                    for (int i = 0; i < openGrinPartsDto.Count; i++)
                    {
                        OpenGrinParts openGrinPartsDetails = _mapper.Map<OpenGrinParts>(openGrinPartsDto[i]);
                        openGrinPartsDetails.OpenGrinDetails = _mapper.Map<List<OpenGrinDetails>>(openGrinPartsDto[i].OpenGrinDetails);
                        openGrinPartsDtoList.Add(openGrinPartsDetails);
                    }
                }
                var updateOpenGrin = _mapper.Map(openGrinUpdateDto, openGrinDetailById);
                updateOpenGrin.OpenGrinParts = openGrinPartsDtoList;
                await _openGrinRepository.UpdateOpenGrin(updateOpenGrin);
                _openGrinRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " OpenGrin Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateOpenGrin API for the following id :{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateOpenGrin API for the following id :{id} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOpenGrin(int id)
        {
            ServiceResponse<OpenGrinDto> serviceResponse = new ServiceResponse<OpenGrinDto>();
            try
            {
                var OpenGrinDetailbyId = await _openGrinRepository.GetOpenGrinDetailsbyId(id);
                if (OpenGrinDetailbyId == null)
                {
                    _logger.LogError($"Delete OpenGrin with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete OpenGrin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _openGrinRepository.DeleteOpenGrin(OpenGrinDetailbyId);
                _logger.LogInfo(result);
                _openGrinRepository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " OpenGrin Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeleteOpenGrin API for the following id :{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteOpenGrin API for the following id :{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOpenGrinDataList()
        {
            ServiceResponse<IEnumerable<OpenGrinDataListDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinDataListDto>>();

            try
            {
                var openGrinNoDetials = await _openGrinRepository.GetAllOpenGrinDataList();
                _logger.LogInfo("Returned all OpenGrinNumberDetials");
                var result = _mapper.Map<IEnumerable<OpenGrinDataListDto>>(openGrinNoDetials);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenGrinNumberDetials Successfully ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllOpenGrinDataList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllOpenGrinDataList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllOpenGrinSORefDetails()
        {
            ServiceResponse<IEnumerable<OpenGrinPartSORefDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinPartSORefDto>>();

            try
            {
                var openGrinNoDetials = await _openGrinRepository.GetAllOpenGrinSORefDetails();
                _logger.LogInfo("Returned all OpenGrinNumberDetials");
                var result = _mapper.Map<IEnumerable<OpenGrinPartSORefDto>>(openGrinNoDetials);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenGrinSORefDetails Successfully ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllOpenGrinSORefDetails API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllOpenGrinSORefDetails API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
    }
}