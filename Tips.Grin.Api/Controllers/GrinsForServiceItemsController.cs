using AutoMapper;
using Contracts;
using Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Net;
using System.Security.Claims;
using System.Text;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Repository;

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class GrinsForServiceItemsController : ControllerBase
    {
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        private IGrinsForServiceItemsPartsRepository _grinPartsRepository;
        private IGrinsForServiceItemsRepository _repository;
        private IIQCForServiceItemsRepository _iQCForServiceItemsRepository;
        private IIQCForServiceItems_ItemsRepository _iQCForServiceItems_ItemsRepository;
        private IDocumentUploadRepository _documentUploadRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;

        public GrinsForServiceItemsController(IHttpContextAccessor httpContextAccessor, IMapper mapper, ILoggerManager logger, IGrinsForServiceItemsRepository repository, IDocumentUploadRepository documentUploadRepository, IIQCForServiceItems_ItemsRepository iQCForServiceItems_ItemsRepository, IIQCForServiceItemsRepository iQCForServiceItemsRepository, IHttpClientFactory clientFactory, IConfiguration config, IGrinsForServiceItemsPartsRepository grinPartsRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _grinPartsRepository = grinPartsRepository;
            _iQCForServiceItemsRepository = iQCForServiceItemsRepository;
            _iQCForServiceItems_ItemsRepository = iQCForServiceItems_ItemsRepository;
            _documentUploadRepository = documentUploadRepository;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        [HttpGet]
        public async Task<IActionResult> GetGrinForServiceItemsAndIqcsForServiceItemsByPurchaseOrder(string Ponumber)
        {
            ServiceResponse<GrinForServiceItemsandIqcForServiceItemsDetail> serviceResponse = new ServiceResponse<GrinForServiceItemsandIqcForServiceItemsDetail>();
            try
            {
                if (Ponumber is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Ponumber object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Ponumber object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Ponumber object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Ponumber object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var getGrinIds = await _grinPartsRepository.GetGrinForServiceItemsIdsByPonumber(Ponumber);

                if (getGrinIds.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"No Grins Create for Ponumber: {Ponumber}";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"No Grins Create for Ponumber: {Ponumber}");
                    return StatusCode(404, serviceResponse);
                }
                var Grindetails = await _repository.GetGrinDetailsofPOByGrinIds(getGrinIds, Ponumber);
                var Iqcs = new Dictionary<string, List<string>>();
                foreach (var grins in Grindetails)
                {
                    grins.GrinsForServiceItemsParts.ForEach(x => x.GrinsForServiceItems = null);
                    grins.GrinsForServiceItemsParts.ForEach(x => x.GrinsForServiceItemsProjectNumbers.ForEach(z => z.GrinsForServiceItemsParts = null));
                    grins.GrinsForServiceItemsOtherCharges.ForEach(x => x.GrinsForServiceItems = null);

                    var partNo = grins.GrinsForServiceItemsParts.Where(x => x.IsIqcForServiceItemsCompleted == true).Select(x => x.ItemNumber).ToList();
                    if (partNo.Count != 0)
                    {
                        Iqcs.Add(grins.GrinsForServiceItemsNumber, partNo);
                    }
                }
                List<IQCForServiceItems>? IqcDetails = null;
                if (Iqcs.Count() != 0)
                {
                    IqcDetails = await _iQCForServiceItemsRepository.GetIqcForServiceItemsDetailsByGrinForServiceItemsNoAndParts(Iqcs);
                    IqcDetails.ForEach(x => x.IQCForServiceItems_Items.ForEach(z => z.IQCForServiceItems = null));
                }
                _logger.LogInfo($"Returned all GetGrinForServiceItemsAndIqcsForServiceItemsByPurchaseOrder {Ponumber}");

                GrinForServiceItemsandIqcForServiceItemsDetail result = new GrinForServiceItemsandIqcForServiceItemsDetail()
                {
                    grinsForServiceItems = Grindetails.ToList(),
                    iqcsForServiceItems = IqcDetails
                };
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GrinForServiceItems And IqcsForServiceItems Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetGrinForServiceItemsAndIqcsForServiceItemsByPurchaseOrder API :\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinForServiceItemsAndIqcsForServiceItemsByPurchaseOrder API :\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGrinsForServiceItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)

        {
            ServiceResponse<IEnumerable<GrinsForServiceItemsDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinsForServiceItemsDto>>();

            try
            {
                var GrinsForServiceItemsForServiceItems = await _repository.GrinsForServiceItemsForServiceItems(pagingParameter, searchParams);

                if (GrinsForServiceItemsForServiceItems == null || GrinsForServiceItemsForServiceItems.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GrinsForServiceItems data not found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GrinsForServiceItems data not found in db");
                    return Ok(serviceResponse);
                }
                var metadata = new
                {
                    GrinsForServiceItemsForServiceItems.TotalCount,
                    GrinsForServiceItemsForServiceItems.PageSize,
                    GrinsForServiceItemsForServiceItems.CurrentPage,
                    GrinsForServiceItemsForServiceItems.HasNext,
                    GrinsForServiceItemsForServiceItems.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all GrinsForServiceItems");
                var result = _mapper.Map<IEnumerable<GrinsForServiceItemsDto>>(GrinsForServiceItemsForServiceItems);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GrinsForServiceItems Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllGrinsForServiceItems API :\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllGrinsForServiceItems API :\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetGrinsForServiceItemsSPReportWithParam([FromBody] GrinForServiceItemsReportWithParamDto grinForServiceItemsReportWithParamDto)
        {
            ServiceResponse<IEnumerable<GrinForServiceItemsSPReport>> serviceResponse = new ServiceResponse<IEnumerable<GrinForServiceItemsSPReport>>();
            try
            {
                var products = await _repository.GetGrinsForServiceItemsSPReportWithParam(grinForServiceItemsReportWithParamDto.GrinsForServiceItemsNumber, grinForServiceItemsReportWithParamDto.VendorName,
                                                                            grinForServiceItemsReportWithParamDto.PONumber, grinForServiceItemsReportWithParamDto.KPN,
                                                                            grinForServiceItemsReportWithParamDto.MPN, grinForServiceItemsReportWithParamDto.Warehouse,
                                                                            grinForServiceItemsReportWithParamDto.Location);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GrinsForServiceItems hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GrinsForServiceItems hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned GrinsForServiceItems Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetGrinsForServiceItemsSPReportWithParam API :\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinsForServiceItemsSPReportWithParam API :\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportServiceItemsSPGrinReportWithParamOrDateForToExcel([FromBody] GrinForServiceItemsReportWithParamAndDateDto grinForServiceItemsReportWithParamAndDateDto)
        {

            ServiceResponse<GrinForServiceItemsSPReport> serviceResponse = new ServiceResponse<GrinForServiceItemsSPReport>();
            try
            {

                if (grinForServiceItemsReportWithParamAndDateDto is null)
                {
                    _logger.LogError("GrinForServiceItemsReportWithParamAndDateDto object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "GrinForServiceItemsReportWithParamAndDateDto object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid GrinForServiceItemsReportWithParamAndDateDto object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                bool hasParams = !string.IsNullOrEmpty(grinForServiceItemsReportWithParamAndDateDto.GrinsForServiceItemsNumber)
                               || !string.IsNullOrEmpty(grinForServiceItemsReportWithParamAndDateDto.VendorName)
                               || !string.IsNullOrEmpty(grinForServiceItemsReportWithParamAndDateDto.PONumber)
                               || !string.IsNullOrEmpty(grinForServiceItemsReportWithParamAndDateDto.KPN)
                               || !string.IsNullOrEmpty(grinForServiceItemsReportWithParamAndDateDto.MPN)
                               || !string.IsNullOrEmpty(grinForServiceItemsReportWithParamAndDateDto.Warehouse)
                               || !string.IsNullOrEmpty(grinForServiceItemsReportWithParamAndDateDto.Location);

                bool hasDate = grinForServiceItemsReportWithParamAndDateDto.FromDate != null || grinForServiceItemsReportWithParamAndDateDto.ToDate != null;

                if (hasParams && hasDate)
                {
                    _logger.LogError("Input object must contain either filter parameters or date range, not both.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or a date range, not both.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!hasParams && (grinForServiceItemsReportWithParamAndDateDto.FromDate == null || grinForServiceItemsReportWithParamAndDateDto.ToDate == null))
                {
                    _logger.LogError("Input object must contain either at least one filter parameter or both from and to dates.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or both from and to dates.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);

                }

                var grinForServiceItemsList = Enumerable.Empty<GrinForServiceItemsSPReport>();

                if (hasParams && !hasDate)
                {

                    grinForServiceItemsList = await _repository.GetGrinsForServiceItemsSPReportWithParam(
                        grinForServiceItemsReportWithParamAndDateDto.GrinsForServiceItemsNumber,
                        grinForServiceItemsReportWithParamAndDateDto.VendorName,
                        grinForServiceItemsReportWithParamAndDateDto.PONumber,
                        grinForServiceItemsReportWithParamAndDateDto.KPN,
                        grinForServiceItemsReportWithParamAndDateDto.MPN,
                        grinForServiceItemsReportWithParamAndDateDto.Warehouse,
                        grinForServiceItemsReportWithParamAndDateDto.Location);
                }


                if (!hasParams && (grinForServiceItemsReportWithParamAndDateDto.FromDate != null && grinForServiceItemsReportWithParamAndDateDto.ToDate != null))
                {

                    grinForServiceItemsList = await _repository.GetGrinsForServiceItemsSPReportWithDate(
                        grinForServiceItemsReportWithParamAndDateDto.FromDate,
                        grinForServiceItemsReportWithParamAndDateDto.ToDate);
                }



                // Create Excel workbook
                // Create Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("GrinForServiceItems");

                // Header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("GRIN For Service Items Number");
                headerRow.CreateCell(1).SetCellValue("Vendor Name");
                headerRow.CreateCell(2).SetCellValue("Vendor ID");
                headerRow.CreateCell(3).SetCellValue("Vendor Address");
                headerRow.CreateCell(4).SetCellValue("Invoice Number");
                headerRow.CreateCell(5).SetCellValue("Invoice Date");
                headerRow.CreateCell(6).SetCellValue("PO Number");
                headerRow.CreateCell(7).SetCellValue("KPN");
                headerRow.CreateCell(8).SetCellValue("MPN");
                headerRow.CreateCell(9).SetCellValue("Item Description");
                headerRow.CreateCell(10).SetCellValue("Manufacture Batch Number");
                headerRow.CreateCell(11).SetCellValue("Unit Price");
                headerRow.CreateCell(12).SetCellValue("Qty");
                headerRow.CreateCell(13).SetCellValue("Accepted Qty");
                headerRow.CreateCell(14).SetCellValue("UOM");
                headerRow.CreateCell(15).SetCellValue("SGST");
                headerRow.CreateCell(16).SetCellValue("CGST");
                headerRow.CreateCell(17).SetCellValue("IGST");
                headerRow.CreateCell(18).SetCellValue("UTGST");
                headerRow.CreateCell(19).SetCellValue("Total Value");
                headerRow.CreateCell(20).SetCellValue("Warehouse");
                headerRow.CreateCell(21).SetCellValue("Location");
                headerRow.CreateCell(22).SetCellValue("Remarks");
                headerRow.CreateCell(23).SetCellValue("GRIN Date");
                headerRow.CreateCell(24).SetCellValue("Project Number");
                headerRow.CreateCell(25).SetCellValue("UOC");

                // Populate data
                int rowIndex = 1;
                foreach (var item in grinForServiceItemsList)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.GrinsForServiceItemsNumber ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(3).SetCellValue(item.VendorAddress ?? "");
                    row.CreateCell(4).SetCellValue(item.InvoiceNumber ?? "");
                    row.CreateCell(5).SetCellValue(item.InvoiceDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(6).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(7).SetCellValue(item.KPN ?? "");
                    row.CreateCell(8).SetCellValue(item.MPN ?? "");
                    row.CreateCell(9).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(10).SetCellValue(item.ManufactureBatchNumber ?? "");
                    row.CreateCell(11).SetCellValue((double)(item.UnitPrice ?? 0));
                    row.CreateCell(12).SetCellValue((double)(item.Qty ?? 0));
                    row.CreateCell(13).SetCellValue((double)(item.AcceptedQty ?? 0));
                    row.CreateCell(14).SetCellValue(item.UOM ?? "");
                    row.CreateCell(15).SetCellValue((double)(item.SGST ?? 0));
                    row.CreateCell(16).SetCellValue((double)(item.CGST ?? 0));
                    row.CreateCell(17).SetCellValue((double)(item.IGST ?? 0));
                    row.CreateCell(18).SetCellValue((double)(item.UTGST ?? 0));
                    row.CreateCell(19).SetCellValue((double)(item.totalvalue ?? 0));
                    row.CreateCell(20).SetCellValue(item.Warehouse ?? "");
                    row.CreateCell(21).SetCellValue(item.Location ?? "");
                    row.CreateCell(22).SetCellValue(item.Remarks ?? "");
                    row.CreateCell(23).SetCellValue(item.GrinDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(24).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(25).SetCellValue(item.UOC ?? "");
                }


                // Export Excel
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();
                    return File(excelBytes,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "GrinForServiceItems.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetGrinsForServiceItemsSPReportWithParamForTrans([FromBody] GrinForServiceItemsReportWithParamForTransDto grinForServiceItemsReportWithParamDto)
        {
            ServiceResponse<IEnumerable<GrinForServiceItemsSPReport>> serviceResponse = new ServiceResponse<IEnumerable<GrinForServiceItemsSPReport>>();
            try
            {
                var products = await _repository.GetGrinsForServiceItemsSPReportWithParamForTrans(grinForServiceItemsReportWithParamDto.GrinsForServiceItemsNumber, grinForServiceItemsReportWithParamDto.VendorName,
                                                                            grinForServiceItemsReportWithParamDto.PONumber, grinForServiceItemsReportWithParamDto.KPN,
                                                                            grinForServiceItemsReportWithParamDto.MPN, grinForServiceItemsReportWithParamDto.Warehouse,
                                                                            grinForServiceItemsReportWithParamDto.Location, grinForServiceItemsReportWithParamDto.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GrinsForServiceItems hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GrinsForServiceItems hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned GrinsForServiceItems Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetGrinsForServiceItemsSPReportWithParamForTrans API :\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinsForServiceItemsSPReportWithParamForTrans API :\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        [HttpPost]
        public async Task<IActionResult> ExportServiceItemsSPGrinReportWithParamOrDateForTransToExcel([FromBody] GrinForServiceItemsReportWithParamAndDateForTransDto grinForServiceItemsReportWithParamAndDateForTransDto)
        {

            ServiceResponse<GrinForServiceItemsSPReport> serviceResponse = new ServiceResponse<GrinForServiceItemsSPReport>();
            try
            {

                if (grinForServiceItemsReportWithParamAndDateForTransDto is null)
                {
                    _logger.LogError("grinForServiceItemsReportWithParamAndDateForTransDto object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "grinForServiceItemsReportWithParamAndDateForTransDto object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid grinForServiceItemsReportWithParamAndDateForTransDto object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                bool hasParams = !string.IsNullOrEmpty(grinForServiceItemsReportWithParamAndDateForTransDto.GrinsForServiceItemsNumber)
                               || !string.IsNullOrEmpty(grinForServiceItemsReportWithParamAndDateForTransDto.VendorName)
                               || !string.IsNullOrEmpty(grinForServiceItemsReportWithParamAndDateForTransDto.PONumber)
                               || !string.IsNullOrEmpty(grinForServiceItemsReportWithParamAndDateForTransDto.KPN)
                               || !string.IsNullOrEmpty(grinForServiceItemsReportWithParamAndDateForTransDto.MPN)
                               || !string.IsNullOrEmpty(grinForServiceItemsReportWithParamAndDateForTransDto.Warehouse)
                               || !string.IsNullOrEmpty(grinForServiceItemsReportWithParamAndDateForTransDto.Location)
                               || !string.IsNullOrEmpty(grinForServiceItemsReportWithParamAndDateForTransDto.ProjectNumber);

                bool hasDate = grinForServiceItemsReportWithParamAndDateForTransDto.FromDate != null || grinForServiceItemsReportWithParamAndDateForTransDto.ToDate != null;

                if (hasParams && hasDate)
                {
                    _logger.LogError("Input object must contain either filter parameters or date range, not both.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or a date range, not both.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!hasParams && (grinForServiceItemsReportWithParamAndDateForTransDto.FromDate == null || grinForServiceItemsReportWithParamAndDateForTransDto.ToDate == null))
                {
                    _logger.LogError("Input object must contain either at least one filter parameter or both from and to dates.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or both from and to dates.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);

                }

                var grinForServiceItemsList = Enumerable.Empty<GrinForServiceItemsSPReport>();

                if (hasParams && !hasDate)
                {

                    grinForServiceItemsList = await _repository.GetGrinsForServiceItemsSPReportWithParamForTrans(
                        grinForServiceItemsReportWithParamAndDateForTransDto.GrinsForServiceItemsNumber,
                        grinForServiceItemsReportWithParamAndDateForTransDto.VendorName,
                        grinForServiceItemsReportWithParamAndDateForTransDto.PONumber,
                        grinForServiceItemsReportWithParamAndDateForTransDto.KPN,
                        grinForServiceItemsReportWithParamAndDateForTransDto.MPN,
                        grinForServiceItemsReportWithParamAndDateForTransDto.Warehouse,
                        grinForServiceItemsReportWithParamAndDateForTransDto.Location,grinForServiceItemsReportWithParamAndDateForTransDto.ProjectNumber);
                }


                if (!hasParams && (grinForServiceItemsReportWithParamAndDateForTransDto.FromDate != null && grinForServiceItemsReportWithParamAndDateForTransDto.ToDate != null))
                {

                    grinForServiceItemsList = await _repository.GetGrinsForServiceItemsSPReportWithDate(
                        grinForServiceItemsReportWithParamAndDateForTransDto.FromDate,
                        grinForServiceItemsReportWithParamAndDateForTransDto.ToDate);
                }



                // Create Excel workbook
                // Create Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("GrinForServiceItems");

                // Header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("GRIN For Service Items Number");
                headerRow.CreateCell(1).SetCellValue("Vendor Name");
                headerRow.CreateCell(2).SetCellValue("Vendor ID");
                headerRow.CreateCell(3).SetCellValue("Vendor Address");
                headerRow.CreateCell(4).SetCellValue("Invoice Number");
                headerRow.CreateCell(5).SetCellValue("Invoice Date");
                headerRow.CreateCell(6).SetCellValue("PO Number");
                headerRow.CreateCell(7).SetCellValue("KPN");
                headerRow.CreateCell(8).SetCellValue("MPN");
                headerRow.CreateCell(9).SetCellValue("Item Description");
                headerRow.CreateCell(10).SetCellValue("Manufacture Batch Number");
                headerRow.CreateCell(11).SetCellValue("Unit Price");
                headerRow.CreateCell(12).SetCellValue("Qty");
                headerRow.CreateCell(13).SetCellValue("Accepted Qty");
                headerRow.CreateCell(14).SetCellValue("UOM");
                headerRow.CreateCell(15).SetCellValue("SGST");
                headerRow.CreateCell(16).SetCellValue("CGST");
                headerRow.CreateCell(17).SetCellValue("IGST");
                headerRow.CreateCell(18).SetCellValue("UTGST");
                headerRow.CreateCell(19).SetCellValue("Total Value");
                headerRow.CreateCell(20).SetCellValue("Warehouse");
                headerRow.CreateCell(21).SetCellValue("Location");
                headerRow.CreateCell(22).SetCellValue("Remarks");
                headerRow.CreateCell(23).SetCellValue("GRIN Date");
                headerRow.CreateCell(24).SetCellValue("Project Number");
                headerRow.CreateCell(25).SetCellValue("UOC");

                // Populate data
                int rowIndex = 1;
                foreach (var item in grinForServiceItemsList)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.GrinsForServiceItemsNumber ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(3).SetCellValue(item.VendorAddress ?? "");
                    row.CreateCell(4).SetCellValue(item.InvoiceNumber ?? "");
                    row.CreateCell(5).SetCellValue(item.InvoiceDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(6).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(7).SetCellValue(item.KPN ?? "");
                    row.CreateCell(8).SetCellValue(item.MPN ?? "");
                    row.CreateCell(9).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(10).SetCellValue(item.ManufactureBatchNumber ?? "");
                    row.CreateCell(11).SetCellValue((double)(item.UnitPrice ?? 0));
                    row.CreateCell(12).SetCellValue((double)(item.Qty ?? 0));
                    row.CreateCell(13).SetCellValue((double)(item.AcceptedQty ?? 0));
                    row.CreateCell(14).SetCellValue(item.UOM ?? "");
                    row.CreateCell(15).SetCellValue((double)(item.SGST ?? 0));
                    row.CreateCell(16).SetCellValue((double)(item.CGST ?? 0));
                    row.CreateCell(17).SetCellValue((double)(item.IGST ?? 0));
                    row.CreateCell(18).SetCellValue((double)(item.UTGST ?? 0));
                    row.CreateCell(19).SetCellValue((double)(item.totalvalue ?? 0));
                    row.CreateCell(20).SetCellValue(item.Warehouse ?? "");
                    row.CreateCell(21).SetCellValue(item.Location ?? "");
                    row.CreateCell(22).SetCellValue(item.Remarks ?? "");
                    row.CreateCell(23).SetCellValue(item.GrinDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(24).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(25).SetCellValue(item.UOC ?? "");
                }


                // Export Excel
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();
                    return File(excelBytes,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "Grin For ServiceItems.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetGrinsForServiceItemsSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<GrinForServiceItemsSPReport>> serviceResponse = new ServiceResponse<IEnumerable<GrinForServiceItemsSPReport>>();
            try
            {
                var products = await _repository.GetGrinsForServiceItemsSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GrinsForServiceItems hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GrinsForServiceItems hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned GrinsForServiceItems Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetGrinsForServiceItemsSPReportWithDate API :\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinsForServiceItemsSPReportWithDate API :\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetGrinsForServiceItemsById(int id)
        {
            ServiceResponse<GrinsForServiceItemsItemMasterEnggDto> serviceResponse = new ServiceResponse<GrinsForServiceItemsItemMasterEnggDto>();
            try
            {
                var GrinDetailsbyId = await _repository.GetGrinsForServiceItemsById(id);

                if (GrinDetailsbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GrinsForServiceItems with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GrinsForServiceItems with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned GrinsForServiceItemsDetailsById with id: {id}");
                    GrinsForServiceItemsItemMasterEnggDto grinItemMasterEnggDto = _mapper.Map<GrinsForServiceItemsItemMasterEnggDto>(GrinDetailsbyId);
                    List<GrinsForServiceItemsPartsItemMasterEnggDto> grinPartsItemMasterEnggList = new List<GrinsForServiceItemsPartsItemMasterEnggDto>();
                    foreach (var GrinpartsDetails in GrinDetailsbyId.GrinsForServiceItemsParts)
                    {
                        GrinsForServiceItemsPartsItemMasterEnggDto grinPartsItemMasterEnggDto = _mapper.Map<GrinsForServiceItemsPartsItemMasterEnggDto>(GrinpartsDetails);
                        grinPartsItemMasterEnggDto.GrinsForServiceItemsProjectNumbers = _mapper.Map<List<GrinsForServiceItemsProjectNumbersDto>>(GrinpartsDetails.GrinsForServiceItemsProjectNumbers);
                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = grinPartsItemMasterEnggDto.ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request.Headers.Add("Authorization", token);

                        var itemMasterDetails = await client.SendAsync(request);
                        var inventoryObjectString = await itemMasterDetails.Content.ReadAsStringAsync();
                        dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                        dynamic inventoryObject = inventoryObjectData.data;
                        grinPartsItemMasterEnggDto.DrawingNo = inventoryObject.drawingNo;
                        grinPartsItemMasterEnggDto.DocRet = inventoryObject.docRet;
                        grinPartsItemMasterEnggDto.RevNo = inventoryObject.revNo;
                        grinPartsItemMasterEnggDto.IsCocRequired = inventoryObject.isCocRequired;
                        grinPartsItemMasterEnggDto.IsRohsItem = inventoryObject.isRohsItem;
                        grinPartsItemMasterEnggDto.IsShelfLife = inventoryObject.isShelfLife;
                        grinPartsItemMasterEnggDto.IsReachItem = inventoryObject.isReachItem;

                        grinPartsItemMasterEnggList.Add(grinPartsItemMasterEnggDto);

                    }

                    grinItemMasterEnggDto.GrinsForServiceItemsParts = grinPartsItemMasterEnggList;
                    serviceResponse.Data = grinItemMasterEnggDto;
                    serviceResponse.Message = $"Returned GrinsForServiceItems with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetGrinsForServiceItemsById API for the following id: {id}\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinsForServiceItemsById API for the following id: {id}\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGrinsForServiceItems([FromBody] GrinsForServiceItemsPostDto grinsForServiceItemsPostDto)
        {
            ServiceResponse<GrinsForServiceItemsDto> serviceResponse = new ServiceResponse<GrinsForServiceItemsDto>();

            try
            {
                string serverKey = GetServerKey();

                if (grinsForServiceItemsPostDto is null)
                {
                    _logger.LogError("GrinsForServiceItems object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "GrinsForServiceItems object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid GrinsForServiceItems object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid GrinsForServiceItems object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var grinsForServiceItems = _mapper.Map<GrinsForServiceItems>(grinsForServiceItemsPostDto);

                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));
                HttpStatusCode invStatusCode = HttpStatusCode.OK;

                if (serverKey == "avision")
                {
                    var grinNum = await _repository.GenerateGrinsForServiceItemsNumberForAvision();
                    grinsForServiceItems.GrinsForServiceItemsNumber = grinNum;
                }
                else
                {
                    var dateFormat = days + months + years;
                    var grinNumber = await _repository.GenerateGrinsForServiceItemsNumber();
                    var grinNo = dateFormat + grinNumber;
                    grinsForServiceItems.GrinsForServiceItemsNumber = grinNo;
                }
                var grinPartsDto = grinsForServiceItemsPostDto.GrinsForServiceItemsParts;
                var grinCal = _mapper.Map<List<GrinsForServiceItemsPartscalculationofAvgcost>>(grinPartsDto);
                List<GrinsForServiceItemsParts> grinPartsList = new List<GrinsForServiceItemsParts>();

                var othercosttotal = grinsForServiceItems.Freight + grinsForServiceItems.Insurance + grinsForServiceItems.LoadingorUnLoading + grinsForServiceItems.Transport;
                decimal? conversionrate = 1;
                if (grinsForServiceItems.CurrencyConversion > 1) conversionrate = grinsForServiceItems.CurrencyConversion;
                foreach (var gPart in grinCal)
                {
                    decimal? EP = gPart.Qty * gPart.UnitPrice;
                    decimal? Itemwithtax = gPart.SGST + gPart.IGST + gPart.CGST + gPart.UTGST + gPart.Duties;
                    gPart.EPwithTax = (EP + (EP * (Itemwithtax / 100))) * conversionrate;
                    gPart.EPforSingleQty = gPart.EPwithTax / gPart.Qty;
                }
                decimal? SumofEPwithtax = grinCal.Sum(x => x.EPwithTax);
                foreach (var gPart in grinCal)
                {
                    decimal? distriduteOthercostforitem = (gPart.EPwithTax / SumofEPwithtax) * othercosttotal;
                    decimal? distriduteOthercostforitemsSingleQty = distriduteOthercostforitem / gPart.Qty;
                    gPart.AverageCost = distriduteOthercostforitemsSingleQty + gPart.EPforSingleQty;
                    GrinsForServiceItemsParts grinParts = _mapper.Map<GrinsForServiceItemsParts>(gPart);
                    grinPartsList.Add(grinParts);
                }

                grinsForServiceItems.GrinsForServiceItemsParts = grinPartsList;
                grinsForServiceItems.IsGrinsForServiceItemsCompleted = true;

                await _repository.CreateGrinsForServiceItems(grinsForServiceItems);
                _repository.SaveAsync();



                if (grinsForServiceItems.GrinsForServiceItemsParts != null)
                {
                    foreach (var grinPart in grinsForServiceItems.GrinsForServiceItemsParts)
                    {
                        var grinPartsId = await _grinPartsRepository.GetGrinsForServiceItemsPartsById(grinPart.Id);
                        grinPartsId.LotNumber = grinsForServiceItems.GrinsForServiceItemsNumber + grinPartsId.Id;
                        await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinPartsId);

                    }
                }
                // HttpStatusCode createinvforServiceItemsResp = HttpStatusCode.OK;
                HttpStatusCode createinvResp = HttpStatusCode.OK;
                HttpStatusCode createinvTrancResp = HttpStatusCode.OK;
                HttpStatusCode getItemmResp = HttpStatusCode.OK;
                HttpStatusCode getItemForIqcResp = HttpStatusCode.OK;
                HttpStatusCode UpdatePoStatus = HttpStatusCode.OK;
                HttpStatusCode UpdatePoQty = HttpStatusCode.OK;
                HttpStatusCode UpdatePoProjQty = HttpStatusCode.OK;

                foreach (var parts in grinPartsList)
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = parts.ItemNumber;
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

                    if (parts.GrinsForServiceItemsProjectNumbers != null)
                    {
                        foreach (var project in parts.GrinsForServiceItemsProjectNumbers)
                        {
                            GrinInventoryDto grinInventoryDto = new GrinInventoryDto();
                            grinInventoryDto.PartNumber = parts.ItemNumber;
                            grinInventoryDto.LotNumber = parts.LotNumber;
                            grinInventoryDto.MftrPartNumber = parts.MftrItemNumber;
                            grinInventoryDto.Description = parts.ItemDescription;
                            grinInventoryDto.ProjectNumber = project.ProjectNumber;
                            grinInventoryDto.Balance_Quantity = Convert.ToDecimal(project.ProjectQty);
                            grinInventoryDto.Max = itemMasterObject.max;
                            grinInventoryDto.Min = itemMasterObject.min;
                            grinInventoryDto.UOM = parts.UOM;
                            grinInventoryDto.Warehouse = "GRIN";
                            grinInventoryDto.Location = "GRIN";
                            grinInventoryDto.GrinNo = grinsForServiceItems.GrinsForServiceItemsNumber;
                            grinInventoryDto.GrinPartId = parts.Id;
                            grinInventoryDto.PartType = parts.ItemType;
                            grinInventoryDto.ReferenceID = grinsForServiceItems.GrinsForServiceItemsNumber;
                            grinInventoryDto.ReferenceIDFrom = "GrinsForServiceItems";
                            grinInventoryDto.GrinMaterialType = "";
                            grinInventoryDto.ShopOrderNo = "";

                            var json = JsonConvert.SerializeObject(grinInventoryDto);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            var client1 = _clientFactory.CreateClient();
                            var token1 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryForServiceItemsAPI"],
                            "CreateInventoryForServiceItemsFromGrin"))
                            {
                                Content = data
                            };
                            request1.Headers.Add("Authorization", token1);

                            var response = await client1.SendAsync(request1);
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                createinvResp = response.StatusCode;
                            }

                            GrinsForServiceItemsInventoryTransaction grinInventoryTranctionDto = new GrinsForServiceItemsInventoryTransaction();
                            grinInventoryTranctionDto.PartNumber = parts.ItemNumber;
                            grinInventoryTranctionDto.LotNumber = parts.LotNumber;
                            grinInventoryTranctionDto.MftrPartNumber = parts.MftrItemNumber;
                            grinInventoryTranctionDto.Description = parts.ItemDescription;
                            grinInventoryTranctionDto.ProjectNumber = project.ProjectNumber;
                            grinInventoryTranctionDto.Issued_Quantity = Convert.ToDecimal(project.ProjectQty);
                            grinInventoryTranctionDto.UOM = parts.UOM;
                            grinInventoryTranctionDto.Warehouse = "GRIN";
                            grinInventoryTranctionDto.From_Location = "GRIN";
                            grinInventoryTranctionDto.TO_Location = "GRIN";
                            grinInventoryTranctionDto.GrinsForServiceItemsNumber = grinsForServiceItems.GrinsForServiceItemsNumber;
                            grinInventoryTranctionDto.GrinsForServiceItemsPartsId = parts.Id;
                            grinInventoryTranctionDto.PartType = parts.ItemType;
                            grinInventoryTranctionDto.ReferenceID = grinsForServiceItems.GrinsForServiceItemsNumber;
                            grinInventoryTranctionDto.ReferenceIDFrom = "GrinsForServiceItems";
                            grinInventoryTranctionDto.GrinsForServiceItemsMaterialType = "";
                            grinInventoryTranctionDto.shopOrderNo = "";
                            grinInventoryTranctionDto.IsStockAvailable = true;
                            grinInventoryTranctionDto.Remarks = "GrinsForServiceItems Done";
                            grinInventoryTranctionDto.TransactionType = InventoryType.Inward;

                            var json_1 = JsonConvert.SerializeObject(grinInventoryTranctionDto);
                            var data_1 = new StringContent(json_1, Encoding.UTF8, "application/json");
                            var request1_1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["WarehouseService"],
                            "InventoryTranctionforServiceItems/CreateInventoryTranctionforServiceItems"))
                            {
                                Content = data_1
                            };
                            request1_1.Headers.Add("Authorization", token1);

                            var response_1 = await client1.SendAsync(request1_1);
                            if (response_1.StatusCode != HttpStatusCode.OK)
                            {
                                createinvTrancResp = response_1.StatusCode;
                            }
                        }
                    }
                }

                var grinPartsDetail = _mapper.Map<List<GrinsForServiceItemsUpdateQtyDetailsDto>>(grinPartsDto);
                var jsons = JsonConvert.SerializeObject(grinPartsDetail);
                var data1 = new StringContent(jsons, Encoding.UTF8, "application/json");
                var client2 = _clientFactory.CreateClient();
                var token2 = HttpContext.Request.Headers["Authorization"].ToString();
                var request2 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["PurchaseAPI"],
                "UpdateBalanceQtyDetails"))
                {
                    Content = data1
                };
                request2.Headers.Add("Authorization", token2);

                var responses = await client2.SendAsync(request2);
                if (responses.StatusCode != HttpStatusCode.OK)
                {
                    UpdatePoQty = responses.StatusCode;
                }
                if (grinPartsDto.Count() > 0)
                {
                    foreach (var grinparts in grinPartsDto)
                    {
                        List<GrinsForServiceItemsUpdateProjectBalQtyDetailsDto> projectNameDtos = new List<GrinsForServiceItemsUpdateProjectBalQtyDetailsDto>();
                        foreach (var projectNo in grinparts.GrinsForServiceItemsProjectNumbers)
                        {
                            var grinPartsProjectNoDtoDetail = _mapper.Map<GrinsForServiceItemsUpdateProjectBalQtyDetailsDto>(projectNo);
                            grinPartsProjectNoDtoDetail.ItemNumber = grinparts.ItemNumber;
                            grinPartsProjectNoDtoDetail.PoItemId = grinparts.PoItemId;
                            projectNameDtos.Add(grinPartsProjectNoDtoDetail);
                        }

                        var jsonss = JsonConvert.SerializeObject(projectNameDtos);
                        var data2 = new StringContent(jsonss, Encoding.UTF8, "application/json");
                        var client3 = _clientFactory.CreateClient();
                        var token3 = HttpContext.Request.Headers["Authorization"].ToString();
                        var request3 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["PurchaseAPI"],
                        "UpdatePoProjectNoBalanceQtyDetails"))
                        {
                            Content = data2
                        };
                        request3.Headers.Add("Authorization", token3);

                        var results = await client3.SendAsync(request3);
                        if (results.StatusCode != HttpStatusCode.OK)
                        {
                            UpdatePoProjQty = results.StatusCode;
                        }
                    }
                }

                var grinPartsDetails = _mapper.Map<List<GrinsForServiceItemsQtyPoStatusUpdateDto>>(grinPartsDto);
                var jsonCon = JsonConvert.SerializeObject(grinPartsDetails);
                var data3 = new StringContent(jsonCon, Encoding.UTF8, "application/json");
                var client4 = _clientFactory.CreateClient();
                var token4 = HttpContext.Request.Headers["Authorization"].ToString();
                var request4 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["PurchaseAPI"],
                "UpdatePoStatus"))
                {
                    Content = data3
                };
                request4.Headers.Add("Authorization", token4);

                var result = await client4.SendAsync(request4);
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    UpdatePoStatus = result.StatusCode;
                }
                // createinvforServiceItemsResp == HttpStatusCode.OK &&
                if (getItemmResp == HttpStatusCode.OK && UpdatePoStatus == HttpStatusCode.OK && UpdatePoQty == HttpStatusCode.OK
                    && UpdatePoProjQty == HttpStatusCode.OK && createinvTrancResp == HttpStatusCode.OK && createinvResp == HttpStatusCode.OK)
                {
                    _repository.SaveAsync();
                    _grinPartsRepository.SaveAsync();

                    if (grinsForServiceItems.GrinsForServiceItemsParts != null)
                    {
                        List<string> grinPartsItemNoListDtos = new List<string>();
                        foreach (var grinParts in grinsForServiceItems.GrinsForServiceItemsParts)
                        {
                            var grinPartsProjectNoDtoDetail = _mapper.Map<string>(grinParts.ItemNumber);
                            grinPartsItemNoListDtos.Add(grinPartsProjectNoDtoDetail);
                        }

                        var jsonss = JsonConvert.SerializeObject(grinPartsItemNoListDtos);
                        var data4 = new StringContent(jsonss, Encoding.UTF8, "application/json");
                        var client5 = _clientFactory.CreateClient();
                        var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                        var request5 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterEnggAPI"],
                        "GetAllClosedIqcItemMasterItemNoList"))
                        {
                            Content = data4
                        };
                        request5.Headers.Add("Authorization", token5);

                        var itemMasterObjectResult = await client5.SendAsync(request5);
                        if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                        {
                            getItemForIqcResp = itemMasterObjectResult.StatusCode;
                        }
                        if (itemMasterObjectResult.StatusCode == HttpStatusCode.OK)
                        {
                            var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                            dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                            dynamic itemMasterObject = itemMasterObjectData;

                            if (itemMasterObject != null && itemMasterObject.Count > 0)
                            {
                                for (int i = 0; i < grinsForServiceItems.GrinsForServiceItemsParts.Count; i++)
                                {
                                    var grinPartItemNo = grinsForServiceItems.GrinsForServiceItemsParts[i].ItemNumber;
                                    for (int j = 0; j < itemMasterObject.Count; j++)
                                    {
                                        if (itemMasterObject[j] == grinPartItemNo)
                                        {
                                            var iqcGrinDetails = _mapper.Map<GrinsForServiceItemsIQCForServiceItemsSaveDto>(grinsForServiceItems);
                                            iqcGrinDetails.GrinsForServiceItemsId = grinsForServiceItems.Id;
                                            iqcGrinDetails.GrinsForServiceItemsNumber = grinsForServiceItems.GrinsForServiceItemsNumber;
                                            iqcGrinDetails.VendorNumber = grinsForServiceItems.VendorNumber;
                                            iqcGrinDetails.VendorId = grinsForServiceItems.VendorId;
                                            iqcGrinDetails.VendorName = grinsForServiceItems.VendorName;

                                            GrinsForServiceItemsIQCForServiceItems_ItemsSaveDto grinIQCForServiceItemsItemsSaveDto = _mapper.Map<GrinsForServiceItemsIQCForServiceItems_ItemsSaveDto>(grinsForServiceItems.GrinsForServiceItemsParts[i]);
                                            grinIQCForServiceItemsItemsSaveDto.ItemNumber = grinsForServiceItems.GrinsForServiceItemsParts[i].ItemNumber;
                                            grinIQCForServiceItemsItemsSaveDto.GrinsForServiceItemsPartId = grinsForServiceItems.GrinsForServiceItemsParts[i].Id;
                                            grinIQCForServiceItemsItemsSaveDto.ReceivedQty = grinsForServiceItems.GrinsForServiceItemsParts[i].Qty;
                                            grinIQCForServiceItemsItemsSaveDto.AcceptedQty = grinsForServiceItems.GrinsForServiceItemsParts[i].Qty;
                                            grinIQCForServiceItemsItemsSaveDto.RejectedQty = grinsForServiceItems.GrinsForServiceItemsParts[i].RejectedQty;
                                            iqcGrinDetails.GrinsForServiceItemsIQCForServiceItems_ItemsSaveDto = grinIQCForServiceItemsItemsSaveDto;

                                            await CreateIQCGrinsForServiceItems_Items(iqcGrinDetails);

                                        }
                                    }
                                }
                            }

                        }

                    }

                }
                else
                {
                    _logger.LogError($"Something went wrong inside Create CreateGrinsForServiceItems action: Other Service Calling");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Saving Failed";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
                serviceResponse.Data = null;
                serviceResponse.Message = "GrinsForServiceItems Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetGrinById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateGrinsForServiceItems API :\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateGrinsForServiceItems API :\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        private async Task<IActionResult> CreateIQCGrinsForServiceItems_Items([FromBody] GrinsForServiceItemsIQCForServiceItemsSaveDto grinsForServiceItemsIQCForServiceItemsSaveDto)
        {
            ServiceResponse<GrinsForServiceItemsIQCForServiceItemsSaveDto> serviceResponse = new ServiceResponse<GrinsForServiceItemsIQCForServiceItemsSaveDto>();

            try
            {
                string serverKey = GetServerKey();

                if (grinsForServiceItemsIQCForServiceItemsSaveDto is null)
                {
                    _logger.LogError("Create IQCForServiceItems object sent from the client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Create IQCForServiceItems object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Create IQCForServiceItems object sent from the client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Create IQCForServiceItems object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var iqcForServiceItems = _mapper.Map<IQCForServiceItems>(grinsForServiceItemsIQCForServiceItemsSaveDto);
                var iqcConfirmationItemsDto = grinsForServiceItemsIQCForServiceItemsSaveDto.GrinsForServiceItemsIQCForServiceItems_ItemsSaveDto;
                var iqcforserviceitems_items = _mapper.Map<IQCForServiceItems_Items>(iqcConfirmationItemsDto);
                var grinsForServiceItemsNumber = iqcForServiceItems.GrinsForServiceItemsNumber;
                var GrinsForServiceItemsId = iqcForServiceItems.GrinsForServiceItemsId;
                var existingIqcConfirmation = await _iQCForServiceItemsRepository.GetIqcForServiceItemsDetailsbyGrinForServiceItemsNo(grinsForServiceItemsNumber);

                if (existingIqcConfirmation != null)
                {
                    iqcforserviceitems_items.IQCForServiceItemsId = existingIqcConfirmation.Id;

                    var grinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartId;
                    var grinPartsDetails = await _grinPartsRepository.GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(grinPartId);
                    if (iqcConfirmationItemsDto.GrinsForServiceItemsPartId != grinPartsDetails.Id)
                    {
                        _logger.LogError($"Invalid GrinsForServiceItemsPart Id {grinPartId}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Invalid GrinsForServiceItemsPart Id {grinPartId}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    iqcforserviceitems_items.IsIqcForServiceItemsCompleted = true;
                    await _iQCForServiceItems_ItemsRepository.CreateIqcForServiceItems_Items(iqcforserviceitems_items);
                    _iQCForServiceItems_ItemsRepository.SaveAsync();
                    grinPartsDetails.IsIqcForServiceItemsCompleted = true;

                    //await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinPartsDetails);
                    //_grinPartsRepository.SaveAsync();

                    //var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinsForServiceItemsPartsQty(iqcforserviceitems_items.GrinsForServiceItemsPartsId, iqcforserviceitems_items.AcceptedQty.ToString(), iqcforserviceitems_items.RejectedQty.ToString());
                    //var grinParts = _mapper.Map<GrinsForServiceItemsParts>(updatedGrinPartsQty);
                    grinPartsDetails.RejectedQty = iqcforserviceitems_items.RejectedQty;
                    grinPartsDetails.AcceptedQty = iqcforserviceitems_items.AcceptedQty;
                    grinPartsDetails.IsIqcForServiceItemsCompleted = true;
                    await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinPartsDetails);
                    _grinPartsRepository.SaveAsync();

                    var grinPartsCount = await _grinPartsRepository.GetGrinsForServiceItemsPartsCount(GrinsForServiceItemsId);
                    var iqcConfomationCount = await _iQCForServiceItems_ItemsRepository.GetIQCForServiceItems_ItemsCount(existingIqcConfirmation.Id);

                    if (grinPartsCount == iqcConfomationCount)
                    {
                        var iqcDetails = await _iQCForServiceItemsRepository.GetIqcForServiceItemsDetailsbyGrinForServiceItemsNo(grinsForServiceItemsNumber);
                        iqcDetails.IsIqcForServiceItemsCompleted = true;
                        _iQCForServiceItemsRepository.SaveAsync();

                        var grinDetails = await _repository.GetGrinForServiceItemsByGrinForServiceItemsNo(grinsForServiceItemsNumber);
                        grinDetails.IsIqcForServiceItemsCompleted = true;
                        _repository.SaveAsync();
                    }
                    HttpStatusCode getInvGrinsForServiceItemsId = HttpStatusCode.OK;
                    HttpStatusCode updateInv = HttpStatusCode.OK;
                    HttpStatusCode getInvTrancGrinsForServiceItemsId = HttpStatusCode.OK;
                    HttpStatusCode createInvfromGrin = HttpStatusCode.OK;
                    HttpStatusCode createInvTranc = HttpStatusCode.OK;
                    HttpStatusCode createInvTranc1 = HttpStatusCode.OK;

                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = iqcConfirmationItemsDto.ItemNumber;
                    var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                        $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                    request.Headers.Add("Authorization", token);

                    var itemMasterObjectResult = await client.SendAsync(request);
                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterObject = itemMasterObjectData.data;
                    //Inventory Update Code
                    decimal? acceptedQty = iqcConfirmationItemsDto.AcceptedQty;
                    decimal rejectedQty = iqcConfirmationItemsDto.RejectedQty;
                    var grinPartsId = iqcConfirmationItemsDto.GrinsForServiceItemsPartId;
                    var grinPartsDetail = await _grinPartsRepository.GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(grinPartsId);
                    foreach (var projectNo in grinPartsDetail.GrinsForServiceItemsProjectNumbers)
                    {
                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var itemNo = iqcConfirmationItemsDto.ItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(itemNo);
                        var grinNo = iqcForServiceItems.GrinsForServiceItemsNumber;
                        var encodedgrinNo = Uri.EscapeDataString(grinNo);
                        var grinPartsIds = projectNo.GrinsForServiceItemsPartsId;
                        var projectNos = projectNo.ProjectNumber;
                        var encodedprojectNos = Uri.EscapeDataString(projectNos);

                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryForServiceItemsAPI"],
                            $"GetInventoryForServiceDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                        request1.Headers.Add("Authorization", token1);

                        var inventoryObjectResult = await client1.SendAsync(request1);
                        if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvGrinsForServiceItemsId = inventoryObjectResult.StatusCode;
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
                                inventoryObject.referenceIDFrom = "IQCForServiceItems";
                                acceptedQty -= balanceQty;

                            }
                            else if (inventoryObject.balance_Quantity > acceptedQty)
                            {
                                if (acceptedQty == 0)
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceIDFrom = "IQCForServiceItems";
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
                                    inventoryObject.referenceIDFrom = "IQCForServiceItems";
                                    acceptedQty = 0;
                                }
                            }

                            if (inventoryObject.balance_Quantity == 0) { inventoryObject.isStockAvailable = 0; }
                            var json = JsonConvert.SerializeObject(inventoryObject);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            var client5 = _clientFactory.CreateClient();
                            var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryForServiceItemsAPI"],
                            "UpdateInventoryForServiceDetails?id=", inventoryObject.id))
                            {
                                Content = data
                            };
                            request5.Headers.Add("Authorization", token5);

                            var response = await client5.SendAsync(request5);
                            if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;

                            //InventoryTranction Update Code

                            GrinsForServiceItemsInventoryTransaction iqcInventoryTranctionDto = new GrinsForServiceItemsInventoryTransaction();
                            iqcInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                            iqcInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                            iqcInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                            iqcInventoryTranctionDto.Description = inventoryObject.description;
                            iqcInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                            iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                            iqcInventoryTranctionDto.IsStockAvailable = inventoryObject.isStockAvailable;
                            iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                            iqcInventoryTranctionDto.Warehouse = "IQC";
                            iqcInventoryTranctionDto.From_Location = "GRIN";
                            iqcInventoryTranctionDto.TO_Location = "IQC";
                            iqcInventoryTranctionDto.GrinsForServiceItemsNumber = inventoryObject.grinNo;
                            iqcInventoryTranctionDto.GrinsForServiceItemsPartsId = inventoryObject.grinPartId;
                            iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                            iqcInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;
                            iqcInventoryTranctionDto.ReferenceIDFrom = "IQCForServiceItems";
                            iqcInventoryTranctionDto.GrinsForServiceItemsMaterialType = "";
                            iqcInventoryTranctionDto.shopOrderNo = "";
                            iqcInventoryTranctionDto.Remarks = "IQCForServiceItems Done";
                            iqcInventoryTranctionDto.TransactionType = InventoryType.Inward;

                            string rfqSourcingPPdetailsJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                            var contents = new StringContent(rfqSourcingPPdetailsJsons, Encoding.UTF8, "application/json");
                            var client7 = _clientFactory.CreateClient();
                            var token7 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request7 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["WarehouseService"],
                            "InventoryTranctionforServiceItems/CreateInventoryTranctionforServiceItems"))
                            {
                                Content = contents
                            };
                            request7.Headers.Add("Authorization", token7);

                            var inventoryTransResponses = await client7.SendAsync(request7);
                            if (inventoryTransResponses.StatusCode != HttpStatusCode.OK) createInvTranc1 = inventoryTransResponses.StatusCode;

                            GrinsForServiceItemsInventoryTransaction iqcInventoryTranctionDto_1 = new GrinsForServiceItemsInventoryTransaction();
                            iqcInventoryTranctionDto_1.PartNumber = inventoryObject.partNumber;
                            iqcInventoryTranctionDto_1.LotNumber = inventoryObject.lotNumber;
                            iqcInventoryTranctionDto_1.MftrPartNumber = inventoryObject.mftrPartNumber;
                            iqcInventoryTranctionDto_1.Description = inventoryObject.description;
                            iqcInventoryTranctionDto_1.ProjectNumber = inventoryObject.projectNumber;
                            iqcInventoryTranctionDto_1.Issued_Quantity = inventoryObject.balance_Quantity;
                            iqcInventoryTranctionDto_1.IsStockAvailable = false;
                            iqcInventoryTranctionDto_1.UOM = inventoryObject.uom;
                            iqcInventoryTranctionDto_1.Warehouse = "GRIN";
                            iqcInventoryTranctionDto_1.From_Location = "GRIN";
                            iqcInventoryTranctionDto_1.TO_Location = "IQC";
                            iqcInventoryTranctionDto_1.GrinsForServiceItemsNumber = inventoryObject.grinNo;
                            iqcInventoryTranctionDto_1.GrinsForServiceItemsPartsId = inventoryObject.grinPartId;
                            iqcInventoryTranctionDto_1.PartType = inventoryObject.partType;
                            iqcInventoryTranctionDto_1.ReferenceID = inventoryObject.grinNo;
                            iqcInventoryTranctionDto_1.ReferenceIDFrom = "GrinsForServiceItems";
                            iqcInventoryTranctionDto_1.GrinsForServiceItemsMaterialType = "";
                            iqcInventoryTranctionDto_1.shopOrderNo = "";
                            iqcInventoryTranctionDto_1.Remarks = "IQCForServiceItems Done";
                            iqcInventoryTranctionDto_1.TransactionType = InventoryType.Outward;

                            string rfqSourcingPPdetailsJsons_1 = JsonConvert.SerializeObject(iqcInventoryTranctionDto_1);
                            var contents_1 = new StringContent(rfqSourcingPPdetailsJsons_1, Encoding.UTF8, "application/json");
                            var request7_1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["WarehouseService"],
                            "InventoryTranctionforServiceItems/CreateInventoryTranctionforServiceItems"))
                            {
                                Content = contents_1
                            };
                            request7_1.Headers.Add("Authorization", token7);

                            var inventoryTransResponses_1 = await client7.SendAsync(request7_1);
                            if (inventoryTransResponses_1.StatusCode != HttpStatusCode.OK) createInvTranc1 = inventoryTransResponses_1.StatusCode;

                            if (iqcConfirmationItemsDto.RejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
                            {
                                IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                grinInventoryDto.PartNumber = iqcConfirmationItemsDto.ItemNumber;
                                grinInventoryDto.LotNumber = grinPartsDetails.LotNumber;
                                grinInventoryDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                grinInventoryDto.Description = grinPartsDetails.ItemDescription;
                                grinInventoryDto.ProjectNumber = projectNos;
                                grinInventoryDto.Max = itemMasterObject.max;
                                grinInventoryDto.Min = itemMasterObject.min;
                                grinInventoryDto.UOM = grinPartsDetails.UOM;
                                grinInventoryDto.Warehouse = "Reject";
                                grinInventoryDto.Location = "Reject";
                                grinInventoryDto.GrinNo = iqcForServiceItems.GrinsForServiceItemsNumber;
                                grinInventoryDto.GrinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartId;
                                grinInventoryDto.PartType = itemMasterObject.itemType;
                                grinInventoryDto.ReferenceID = iqcForServiceItems.GrinsForServiceItemsNumber;
                                grinInventoryDto.ReferenceIDFrom = "GrinsForServiceItems";
                                grinInventoryDto.GrinMaterialType = "GrinsForServiceItems";
                                grinInventoryDto.ShopOrderNo = "";
                                if (flag1 == 1)
                                {
                                    grinInventoryDto.Balance_Quantity = rejectedQty;
                                }
                                else if (flag2 == 1)
                                {
                                    grinInventoryDto.Balance_Quantity = bal;
                                    rejectedQty -= bal;
                                }
                                string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(grinInventoryDto);
                                var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                                var client6 = _clientFactory.CreateClient();
                                var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryForServiceItemsAPI"],
                                "CreateInventoryForServiceItemsFromGrin"))
                                {
                                    Content = content
                                };
                                request6.Headers.Add("Authorization", token6);

                                var rfqCustomerIdResponse = await client6.SendAsync(request6);
                                if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInvfromGrin = rfqCustomerIdResponse.StatusCode;

                                //InventoryTranction Update Code

                                GrinsForServiceItemsInventoryTransaction iqcInventoryTranctionDtos = new GrinsForServiceItemsInventoryTransaction();
                                iqcInventoryTranctionDtos.PartNumber = grinInventoryDto.PartNumber;
                                iqcInventoryTranctionDtos.LotNumber = grinInventoryDto.LotNumber;
                                iqcInventoryTranctionDtos.MftrPartNumber = grinInventoryDto.MftrPartNumber;
                                iqcInventoryTranctionDtos.Description = grinInventoryDto.Description;
                                iqcInventoryTranctionDtos.ProjectNumber = grinInventoryDto.ProjectNumber;
                                iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                iqcInventoryTranctionDtos.IsStockAvailable = grinInventoryDto.IsStockAvailable;
                                iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                iqcInventoryTranctionDtos.From_Location = "GRIN";
                                iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.GrinsForServiceItemsNumber = grinInventoryDto.GrinNo;
                                iqcInventoryTranctionDtos.GrinsForServiceItemsPartsId = grinInventoryDto.GrinPartId;
                                iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.GrinNo;
                                iqcInventoryTranctionDtos.ReferenceIDFrom = "GrinsForServiceItems";
                                iqcInventoryTranctionDtos.GrinsForServiceItemsMaterialType = "";
                                iqcInventoryTranctionDtos.shopOrderNo = "";
                                iqcInventoryTranctionDtos.Remarks = "GrinsForServiceItemsIQC Done";
                                iqcInventoryTranctionDtos.TransactionType = InventoryType.Inward;

                                string iqcInventoryTranctionDtoJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDtos);
                                var contents1 = new StringContent(iqcInventoryTranctionDtoJsons, Encoding.UTF8, "application/json");
                                var client8 = _clientFactory.CreateClient();
                                var token8 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request8 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["WarehouseService"],
                            "InventoryTranctionforServiceItems/CreateInventoryTranctionforServiceItems"))
                                {
                                    Content = contents1
                                };
                                request8.Headers.Add("Authorization", token8);

                                var inventoryTransResponses1 = await client8.SendAsync(request8);

                                if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK) createInvTranc = inventoryTransResponses1.StatusCode;
                            }
                        }
                    }

                    //var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinsForServiceItemsPartsQty(iqcforserviceitems_items.GrinsForServiceItemsPartsId, iqcforserviceitems_items.AcceptedQty.ToString(), iqcforserviceitems_items.RejectedQty.ToString());

                    //var grinParts = _mapper.Map<GrinsForServiceItemsParts>(updatedGrinPartsQty);
                    //string result = await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinParts);
                    if (getInvGrinsForServiceItemsId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && getInvTrancGrinsForServiceItemsId == HttpStatusCode.OK && createInvfromGrin == HttpStatusCode.OK && createInvTranc == HttpStatusCode.OK && createInvTranc1 == HttpStatusCode.OK)
                    {
                        _grinPartsRepository.SaveAsync();
                        _iQCForServiceItemsRepository.SaveAsync();
                        _repository.SaveAsync();
                        _iQCForServiceItems_ItemsRepository.SaveAsync();
                    }
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCForServiceItemsItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    if (serverKey == "avision")
                    {
                        var grinNum = iqcForServiceItems.GrinsForServiceItemsNumber;
                        if (grinNum != null)
                        {
                            var iqcNum = grinNum.Replace("GSI", "IQCSI");
                            iqcForServiceItems.IQCForServiceItemsNumber = iqcNum;
                        }
                    }
                    var grinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartId;
                    var grinPartsDetails = await _grinPartsRepository.GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(grinPartId);
                    if (iqcConfirmationItemsDto.GrinsForServiceItemsPartId != grinPartsDetails.Id)
                    {
                        _logger.LogError($"Invalid GrinsForServiceItemsPart Id {grinPartId}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Invalid GrinsForServiceItemsPart Id {grinPartId}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    iqcforserviceitems_items.IsIqcForServiceItemsCompleted = true;
                    iqcforserviceitems_items.IQCForServiceItemsId = iqcForServiceItems.Id;
                    iqcForServiceItems.IQCForServiceItems_Items = new List<IQCForServiceItems_Items> { iqcforserviceitems_items };
                    await _iQCForServiceItemsRepository.CreateIQCForServiceItems(iqcForServiceItems);
                    _iQCForServiceItemsRepository.SaveAsync();

                    //Updating IQC Status in GrinParts

                    //grinPartsDetails.IsIqcForServiceItemsCompleted = true;
                    //await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinPartsDetails);
                    //_grinPartsRepository.SaveAsync();
                    //var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinsForServiceItemsPartsQty(iqcforserviceitems_items.GrinsForServiceItemsPartsId, iqcforserviceitems_items.AcceptedQty.ToString(), iqcforserviceitems_items.RejectedQty.ToString());
                    //var grinParts = _mapper.Map<GrinsForServiceItemsParts>(updatedGrinPartsQty);
                    grinPartsDetails.RejectedQty = iqcforserviceitems_items.RejectedQty;
                    grinPartsDetails.AcceptedQty = iqcforserviceitems_items.AcceptedQty;
                    grinPartsDetails.IsIqcForServiceItemsCompleted = true;
                    await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinPartsDetails);
                    _grinPartsRepository.SaveAsync();
                    //Updating IQC and Grin Main Level Status
                    var grinPartsCount = await _grinPartsRepository.GetGrinsForServiceItemsPartsCount(GrinsForServiceItemsId);
                    var iqcConfomationCount = await _iQCForServiceItems_ItemsRepository.GetIQCForServiceItems_ItemsCount(iqcForServiceItems.Id);

                    if (grinPartsCount == iqcConfomationCount)
                    {
                        var iqcDetails = await _iQCForServiceItemsRepository.GetIqcForServiceItemsDetailsbyGrinForServiceItemsNo(grinsForServiceItemsNumber);
                        iqcDetails.IsIqcForServiceItemsCompleted = true;
                        _iQCForServiceItemsRepository.SaveAsync();

                        var grinDetails = await _repository.GetGrinForServiceItemsByGrinForServiceItemsNo(grinsForServiceItemsNumber);
                        grinDetails.IsIqcForServiceItemsCompleted = true;
                        _repository.SaveAsync();
                    }

                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = iqcConfirmationItemsDto.ItemNumber;
                    var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                        $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                    request.Headers.Add("Authorization", token);

                    var itemMasterObjectResult = await client.SendAsync(request);
                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterObject = itemMasterObjectData.data;
                    //Inventory Update Code
                    decimal? acceptedQty = iqcConfirmationItemsDto.AcceptedQty;
                    decimal rejectedQty = iqcConfirmationItemsDto.RejectedQty;
                    var grinPartsId = iqcConfirmationItemsDto.GrinsForServiceItemsPartId;
                    var grinPartsDetail = await _grinPartsRepository.GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(grinPartsId);
                    foreach (var projectNo in grinPartsDetail.GrinsForServiceItemsProjectNumbers)
                    {
                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var itemNo = iqcConfirmationItemsDto.ItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(itemNo);
                        var grinNo = iqcForServiceItems.GrinsForServiceItemsNumber;
                        var encodedgrinNo = Uri.EscapeDataString(grinNo);
                        var grinPartsIds = projectNo.GrinsForServiceItemsPartsId;
                        var projectNos = projectNo.ProjectNumber;
                        var encodedprojectNos = Uri.EscapeDataString(projectNos);

                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryForServiceItemsAPI"],
                           $"GetInventoryForServiceDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                        request1.Headers.Add("Authorization", token1);

                        var inventoryObjectResult = await client1.SendAsync(request1);
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
                                inventoryObject.referenceIDFrom = "IQCForServiceItems";
                                acceptedQty -= balanceQty;

                            }
                            else if (inventoryObject.balance_Quantity > acceptedQty)
                            {
                                if (acceptedQty == 0)
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceIDFrom = "IQCForServiceItems";
                                    inventoryObject.isStockAvailable = false;
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
                                    inventoryObject.referenceIDFrom = "IQCForServiceItems";
                                    acceptedQty = 0;
                                }
                            }

                            var json = JsonConvert.SerializeObject(inventoryObject);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            var client5 = _clientFactory.CreateClient();
                            var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryForServiceItemsAPI"],
                            "UpdateInventoryForServiceDetails?id=", inventoryObject.id))
                            {
                                Content = data
                            };
                            request5.Headers.Add("Authorization", token5);

                            var response = await client5.SendAsync(request5);

                            //InventoryTranction Update Code

                            GrinsForServiceItemsInventoryTransaction iqcInventoryTranctionDto = new GrinsForServiceItemsInventoryTransaction();
                            iqcInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                            iqcInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                            iqcInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                            iqcInventoryTranctionDto.Description = inventoryObject.description;
                            iqcInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                            iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                            iqcInventoryTranctionDto.IsStockAvailable = inventoryObject.isStockAvailable;
                            iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                            iqcInventoryTranctionDto.Warehouse = "IQC";
                            iqcInventoryTranctionDto.From_Location = "GRIN";
                            iqcInventoryTranctionDto.TO_Location = "IQC";
                            iqcInventoryTranctionDto.GrinsForServiceItemsNumber = inventoryObject.grinNo;
                            iqcInventoryTranctionDto.GrinsForServiceItemsPartsId = inventoryObject.grinPartId;
                            iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                            iqcInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;/* Convert.ToString(grinPartsDetails.Id);*/
                            iqcInventoryTranctionDto.ReferenceIDFrom = "IQCForServiceItems";
                            iqcInventoryTranctionDto.GrinsForServiceItemsMaterialType = "";
                            iqcInventoryTranctionDto.shopOrderNo = "";
                            iqcInventoryTranctionDto.Remarks = "IQCForServiceItems Done";
                            iqcInventoryTranctionDto.TransactionType = InventoryType.Inward;

                            string rfqSourcingPPdetailsJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                            var contents = new StringContent(rfqSourcingPPdetailsJsons, Encoding.UTF8, "application/json");
                            var client7 = _clientFactory.CreateClient();
                            var token7 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request7 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["WarehouseService"],
                            "InventoryTranctionforServiceItems/CreateInventoryTranctionforServiceItems"))
                            {
                                Content = contents
                            };
                            request7.Headers.Add("Authorization", token7);

                            var inventoryTransResponses = await client7.SendAsync(request7);
                            //if (inventoryTransResponses.StatusCode != HttpStatusCode.OK) createInvTranc1 = inventoryTransResponses.StatusCode;

                            GrinsForServiceItemsInventoryTransaction iqcInventoryTranctionDto_1 = new GrinsForServiceItemsInventoryTransaction();
                            iqcInventoryTranctionDto_1.PartNumber = inventoryObject.partNumber;
                            iqcInventoryTranctionDto_1.LotNumber = inventoryObject.lotNumber;
                            iqcInventoryTranctionDto_1.MftrPartNumber = inventoryObject.mftrPartNumber;
                            iqcInventoryTranctionDto_1.Description = inventoryObject.description;
                            iqcInventoryTranctionDto_1.ProjectNumber = inventoryObject.projectNumber;
                            iqcInventoryTranctionDto_1.Issued_Quantity = inventoryObject.balance_Quantity;
                            iqcInventoryTranctionDto_1.IsStockAvailable = inventoryObject.isStockAvailable;
                            iqcInventoryTranctionDto_1.UOM = inventoryObject.uom;
                            iqcInventoryTranctionDto_1.Warehouse = "GRIN";
                            iqcInventoryTranctionDto_1.From_Location = "GRIN";
                            iqcInventoryTranctionDto_1.TO_Location = "IQC";
                            iqcInventoryTranctionDto_1.GrinsForServiceItemsNumber = inventoryObject.grinNo;
                            iqcInventoryTranctionDto_1.GrinsForServiceItemsPartsId = inventoryObject.grinPartId;
                            iqcInventoryTranctionDto_1.PartType = inventoryObject.partType;
                            iqcInventoryTranctionDto_1.ReferenceID = inventoryObject.grinNo;
                            iqcInventoryTranctionDto_1.ReferenceIDFrom = "GrinsForServiceItems";
                            iqcInventoryTranctionDto_1.GrinsForServiceItemsMaterialType = "";
                            iqcInventoryTranctionDto_1.shopOrderNo = "";
                            iqcInventoryTranctionDto_1.Remarks = "IQCForServiceItems Done";
                            iqcInventoryTranctionDto_1.TransactionType = InventoryType.Outward;

                            string rfqSourcingPPdetailsJsons_1 = JsonConvert.SerializeObject(iqcInventoryTranctionDto_1);
                            var contents_1 = new StringContent(rfqSourcingPPdetailsJsons_1, Encoding.UTF8, "application/json");
                            var request7_1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["WarehouseService"],
                            "InventoryTranctionforServiceItems/CreateInventoryTranctionforServiceItems"))
                            {
                                Content = contents_1
                            };
                            request7_1.Headers.Add("Authorization", token7);

                            var inventoryTransResponses_1 = await client7.SendAsync(request7_1);
                            // if (inventoryTransResponses_1.StatusCode != HttpStatusCode.OK) createInvTranc1 = inventoryTransResponses_1.StatusCode;

                            if (iqcConfirmationItemsDto.RejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
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
                                grinInventoryDto.GrinNo = iqcForServiceItems.GrinsForServiceItemsNumber;
                                grinInventoryDto.GrinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartId;
                                grinInventoryDto.PartType = itemMasterObject.itemType;
                                grinInventoryDto.ReferenceID = iqcForServiceItems.GrinsForServiceItemsNumber;
                                grinInventoryDto.ReferenceIDFrom = "IQCForServiceItems";
                                grinInventoryDto.GrinMaterialType = "";
                                grinInventoryDto.ShopOrderNo = "";

                                if (flag1 == 1)
                                {
                                    grinInventoryDto.Balance_Quantity = rejectedQty;
                                }
                                else if (flag2 == 1)
                                {
                                    grinInventoryDto.Balance_Quantity = bal;
                                    rejectedQty -= bal;
                                }

                                string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(grinInventoryDto);
                                var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                                var client6 = _clientFactory.CreateClient();
                                var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryForServiceItemsAPI"],
                                "CreateInventoryForServiceItemsFromGrin"))
                                {
                                    Content = content
                                };
                                request6.Headers.Add("Authorization", token6);

                                var responses = await client6.SendAsync(request6);

                                //InventoryTranction Update Code

                                GrinsForServiceItemsInventoryTransaction iqcInventoryTranctionDtos = new GrinsForServiceItemsInventoryTransaction();
                                iqcInventoryTranctionDtos.PartNumber = grinInventoryDto.PartNumber;
                                iqcInventoryTranctionDtos.LotNumber = grinInventoryDto.LotNumber;
                                iqcInventoryTranctionDtos.MftrPartNumber = grinInventoryDto.MftrPartNumber;
                                iqcInventoryTranctionDtos.Description = grinInventoryDto.Description;
                                iqcInventoryTranctionDtos.ProjectNumber = grinInventoryDto.ProjectNumber;
                                iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                iqcInventoryTranctionDtos.IsStockAvailable = grinInventoryDto.IsStockAvailable;
                                iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                iqcInventoryTranctionDtos.From_Location = "GRIN";
                                iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.GrinsForServiceItemsNumber = grinInventoryDto.GrinNo;
                                iqcInventoryTranctionDtos.GrinsForServiceItemsPartsId = grinInventoryDto.GrinPartId;
                                iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.GrinNo;
                                iqcInventoryTranctionDtos.ReferenceIDFrom = "IQCForServiceItems";
                                iqcInventoryTranctionDtos.GrinsForServiceItemsMaterialType = "";
                                iqcInventoryTranctionDtos.shopOrderNo = "";
                                iqcInventoryTranctionDtos.Remarks = "GrinsForServiceItemsIQC Done";
                                iqcInventoryTranctionDtos.TransactionType = InventoryType.Inward;

                                string iqcInventoryTranctionDtoJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDtos);
                                var contents1 = new StringContent(iqcInventoryTranctionDtoJsons, Encoding.UTF8, "application/json");
                                var client8 = _clientFactory.CreateClient();
                                var token8 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request8 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["WarehouseService"],
                            "InventoryTranctionforServiceItems/CreateInventoryTranctionforServiceItems"))
                                {
                                    Content = contents1
                                };
                                request8.Headers.Add("Authorization", token8);

                                var inventoryTransResponses1 = await client8.SendAsync(request8);

                                //if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK) createInvTranc = inventoryTransResponses1.StatusCode;

                            }
                        }
                    }

                    ////update accepted qty and rejected qty in grin model

                    //var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinsForServiceItemsPartsQty(iqcforserviceitems_items.GrinsForServiceItemsPartsId, iqcforserviceitems_items.AcceptedQty.ToString(), iqcforserviceitems_items.RejectedQty.ToString());

                    //var grinParts = _mapper.Map<GrinsForServiceItemsParts>(updatedGrinPartsQty);
                    //string result = await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinParts);

                    // _grinPartsRepository.SaveAsync();

                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCForServiceItems and IQCForServiceItems_Items Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }



            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateIQCForServiceItemsItems API :\n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateIQCForServiceItemsItems API :\n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateGrinForServiceItemsFileUpload([FromBody] List<DocumentUploadPostDto> fileUploadPostDtos)
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                if (fileUploadPostDtos is null)
                {
                    _logger.LogError("GrinForServiceItemsFile object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "GrinForServiceItemsFile object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid GrinForServiceItemsFile object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                List<string>? id_s = new List<string>();
                var FileUploadDetails = fileUploadPostDtos;
                foreach (var FileUploadDetail in FileUploadDetails)
                {
                    Guid guids = Guid.NewGuid();
                    byte[] fileContent = Convert.FromBase64String(FileUploadDetail.FileByte);
                    string fileName = guids.ToString() + "_" + FileUploadDetail.FileName + "." + FileUploadDetail.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "GrinDocument", fileName);
                    using (MemoryStream ms = new MemoryStream(fileContent))
                    {
                        ms.Position = 0;
                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            ms.WriteTo(fileStream);
                        }
                        var uploadedFile = new DocumentUpload
                        {
                            FileName = fileName,
                            FileExtension = FileExt,
                            FilePath = filePath,
                            ParentId = "GrinForServiceItemsFile",
                            DocumentFrom = "GrinForServiceItemsFile Document",
                            FileByte = FileUploadDetail.FileByte
                        };
                        await _documentUploadRepository.CreateUploadDocumentGrin(uploadedFile);
                        _documentUploadRepository.SaveAsync();
                        id_s.Add(uploadedFile.Id.ToString());

                    }
                }
                serviceResponse.Data = id_s;
                serviceResponse.Message = " GrinForServiceItemsFile Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateGrinForServiceItemsFileUpload API :\n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateGrinForServiceItemsFileUpload API :\n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDownloadUrlDetailsforGrinForServiceItemsFiles(string fileids)
        {
            ServiceResponse<List<DocumentUploadDto>> serviceResponse = new ServiceResponse<List<DocumentUploadDto>>();
            try
            {
                string serverKey = GetServerKey();
                var itemsFiles = await _documentUploadRepository.GetDownloadUrlDetails(fileids);
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
                    serviceResponse.Message = "Invalid GrinForServiceItems UploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid GrinForServiceItems UploadDocument sent from client.");
                    return BadRequest(serviceResponse);
                }
                List<DocumentUploadDto> fileUploads = new List<DocumentUploadDto>();
                if (itemsFiles != null)
                {
                    foreach (var fileUploadDetails in itemsFiles)
                    {
                        DocumentUploadDto fileUploadDto = _mapper.Map<DocumentUploadDto>(fileUploadDetails);
                        if (serverKey == "avision")
                        {
                            var baseUrl = $"{_config["GrinUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/apigateway/tips/Grin/DownloadFile?Filename={fileUploadDto.FileName}";
                        }
                        else
                        {
                            var baseUrl = $"{_config["GrinUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/api/Grin/DownloadFile?Filename={fileUploadDto.FileName}";
                        }
                        fileUploads.Add(fileUploadDto);
                    }
                }
                _logger.LogInfo($"Returned DownloadDetail with id: {fileids}");
                serviceResponse.Data = fileUploads;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetDownloadUrlDetailsforGrinForServiceItemsFiles API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetDownloadUrlDetailsforGrinForServiceItemsFiles API : \n {ex.Message}";
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrinsForServiceItems(int id, [FromBody] GrinsForServiceItemsUpdateDto grinDto)
        {
            ServiceResponse<GrinsForServiceItemsDto> serviceResponse = new ServiceResponse<GrinsForServiceItemsDto>();

            try
            {
                if (grinDto is null)
                {
                    _logger.LogError("Update GrinsForServiceItems object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update GrinsForServiceItems object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update GrinsForServiceItems object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update GrinsForServiceItems object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updategrin = await _repository.GetGrinsForServiceItemsById(id);
                if (updategrin is null)
                {
                    _logger.LogError($"Update GrinsForServiceItems with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update GrinsForServiceItems with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var grinparts = _mapper.Map<IEnumerable<GrinsForServiceItemsParts>>(updategrin.GrinsForServiceItemsParts);

                var grinList = _mapper.Map<GrinsForServiceItems>(grinDto);

                var grinPartsDto = updategrin.GrinsForServiceItemsParts;
                var grinCal = _mapper.Map<List<GrinsForServiceItemsPartscalculationofAvgcost>>(grinPartsDto);
                var GrinpartsList = new List<GrinsForServiceItemsParts>();

                var othercosttotal = grinList.Freight + grinList.Insurance + grinList.LoadingorUnLoading + grinList.Transport;
                decimal? conversionrate = 1;
                if (grinList.CurrencyConversion > 1) conversionrate = grinList.CurrencyConversion;
                foreach (var gPart in grinCal)
                {
                    decimal? EP = gPart.Qty * gPart.UnitPrice;
                    decimal? Itemwithtax = gPart.SGST + gPart.IGST + gPart.CGST + gPart.UTGST + gPart.Duties;
                    gPart.EPwithTax = (EP + (EP * (Itemwithtax / 100))) * conversionrate;
                    gPart.EPforSingleQty = gPart.EPwithTax / gPart.Qty;
                }
                decimal? SumofEPwithtax = grinCal.Sum(x => x.EPwithTax);
                foreach (var gPart in grinCal)
                {
                    decimal? distriduteOthercostforitem = (gPart.EPwithTax / SumofEPwithtax) * othercosttotal;
                    decimal? distriduteOthercostforitemsSingleQty = distriduteOthercostforitem / gPart.Qty;
                    gPart.AverageCost = distriduteOthercostforitemsSingleQty + gPart.EPforSingleQty;
                    GrinsForServiceItemsParts grinParts = _mapper.Map<GrinsForServiceItemsParts>(gPart);
                    grinParts.GrinsForServiceItemsProjectNumbers = _mapper.Map<List<GrinsForServiceItemsProjectNumbers>>(gPart.GrinsForServiceItemsProjectNumbers);
                    GrinpartsList.Add(grinParts);
                }
                var data = _mapper.Map(grinDto, updategrin);
                data.GrinsForServiceItemsParts = GrinpartsList;

                string result = await _repository.UpdateGrinsForServiceItems(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "GrinsForServiceItems Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetGrinAndIqcsByPurchaseOrder API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinAndIqcsByPurchaseOrder API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllGrinForServiceItemsNumberForIqcForServiceItems()
        {
            ServiceResponse<IEnumerable<GrinForServiceItemsNoForIqcForServiceItems>> serviceResponse = new ServiceResponse<IEnumerable<GrinForServiceItemsNoForIqcForServiceItems>>();
            try
            {
                var result = await _repository.GetAllGrinForServiceItemsNumberForIqcForServiceItems();
                //var result = _mapper.Map<IEnumerable<GrinForServiceItemsNoForIqcForServiceItems>>(grinNoForIqc);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GrinNumberForIqc";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllGrinForServiceItemsNumberForIqcForServiceItems API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllGrinForServiceItemsNumberForIqcForServiceItems API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
