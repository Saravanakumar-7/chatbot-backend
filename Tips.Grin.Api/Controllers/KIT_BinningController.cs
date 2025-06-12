using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public class KIT_BinningController : ControllerBase
    {
        private IKIT_IQCRepository _kIT_IQCRepository;
        private IKIT_GRINRepository _kIT_GRINRepository;
        private IKIT_BinningRepository _kIT_BinningRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public KIT_BinningController(IKIT_BinningRepository kIT_BinningRepository, IKIT_GRINRepository kIT_GRINRepository, IKIT_IQCRepository kIT_IQCRepository, IHttpClientFactory clientFactory, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _kIT_BinningRepository = kIT_BinningRepository;
            _kIT_GRINRepository = kIT_GRINRepository;
            _kIT_IQCRepository = kIT_IQCRepository;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
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
        public async Task<IActionResult> CreateKIT_Binning([FromBody] KIT_BinningPostDto kIT_BinningPostDto)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                if (kIT_BinningPostDto == null)
                {
                    _logger.LogError("KIT_Binning details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "KIT_Binning details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid KIT_Binning details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                string serverKey = GetServerKey();
                var kIT_Binning = _mapper.Map<KIT_Binning>(kIT_BinningPostDto);
                var kIT_GRIN = await _kIT_GRINRepository.GetKIT_GrinByKIT_GrinNumber(kIT_Binning.KIT_GrinNumber);
                var kIT_IQC = await _kIT_IQCRepository.GetKIT_IQCbyKIT_GrinNumber(kIT_Binning.KIT_GrinNumber);
                var ExistingkIT_Binning = await _kIT_BinningRepository.GetKIT_BinningbyKIT_GrinNumber(kIT_Binning.KIT_GrinNumber);
                if (ExistingkIT_Binning == null)
                {
                    await _kIT_BinningRepository.CreateKIT_Binning(kIT_Binning);
                    foreach (var item in kIT_Binning.KIT_BinningItems)
                    {
                        var grinitem = kIT_GRIN.KIT_GRINParts.Where(a => a.Id == item.KIT_GrinPartId).First();
                        foreach (var prj in grinitem.KIT_GRIN_ProjectNumbers)
                        {
                            foreach (var binloc in item.KIT_BinningItemsLocation.Where(a => a.ProjectNumber == prj.ProjectNumber).ToList())
                            {
                                prj.BinnedQty += binloc.Qty;
                                grinitem.BinnedQty += prj.BinnedQty;
                                foreach (var compo in prj.KIT_GRIN_KITComponents)
                                {
                                    var client = _clientFactory.CreateClient();
                                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                                    var itemNo = compo.PartNumber;
                                    var encodedItemNo = Uri.EscapeDataString(itemNo);
                                    var grinNo = kIT_Binning.KIT_GrinNumber;
                                    var encodedgrinNo = Uri.EscapeDataString(grinNo);
                                    var grinPartsIds = item.KIT_GrinPartId;
                                    var projectNos = prj.ProjectNumber;
                                    var encodedprojectNos = Uri.EscapeDataString(projectNos);
                                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                    $"GetAllIQCInventoryDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                                    request1.Headers.Add("Authorization", token);
                                    var inventoryObjectResult = await client.SendAsync(request1);
                                    //  if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvdetailsGrinId = inventoryObjectResult.StatusCode;
                                    var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                    var inventoryObjectData = JsonConvert.DeserializeObject<InventoryGetAllDto>(inventoryObjectString);
                                    var inventoryObject = inventoryObjectData.data;

                                    var MoveQty = binloc.Qty * compo.KitComponentBOMQty;
                                    compo.BinnedQty += MoveQty;
                                    GrinInventoryDto grinInventoryDto = new GrinInventoryDto();
                                    grinInventoryDto.PartNumber = inventoryObject[0].PartNumber;
                                    grinInventoryDto.LotNumber = inventoryObject[0].LotNumber;
                                    grinInventoryDto.MftrPartNumber = inventoryObject[0].MftrPartNumber;
                                    grinInventoryDto.Description = inventoryObject[0].Description;
                                    grinInventoryDto.ProjectNumber = inventoryObject[0].ProjectNumber;
                                    grinInventoryDto.Max = inventoryObject[0].Max;
                                    grinInventoryDto.Min = inventoryObject[0].Min;
                                    grinInventoryDto.UOM = inventoryObject[0].UOM;
                                    grinInventoryDto.Warehouse = binloc.Warehouse;
                                    grinInventoryDto.Location = binloc.Location;
                                    grinInventoryDto.ReferenceIDFrom = "KIT_Binning";
                                    grinInventoryDto.Balance_Quantity = MoveQty;
                                    grinInventoryDto.GrinNo = inventoryObject[0].GrinNo;
                                    grinInventoryDto.GrinPartId = inventoryObject[0].GrinPartId;
                                    grinInventoryDto.PartType = inventoryObject[0].PartType;
                                    grinInventoryDto.ReferenceID = inventoryObject[0].ReferenceID;
                                    grinInventoryDto.ReferenceIDFrom = inventoryObject[0].ReferenceIDFrom;
                                    grinInventoryDto.GrinMaterialType = "";
                                    grinInventoryDto.ShopOrderNo = "";

                                    var jsona = JsonConvert.SerializeObject(grinInventoryDto);
                                    var dataa = new StringContent(jsona, Encoding.UTF8, "application/json");
                                    var request1a = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                    "CreateInventoryFromGrin"))
                                    {
                                        Content = dataa
                                    };
                                    request1a.Headers.Add("Authorization", token);

                                    var responsea = await client.SendAsync(request1a);
                                    if (responsea.StatusCode != HttpStatusCode.OK)
                                    {
                                        // createinvResp = responsea.StatusCode;
                                    }

                                    grinInventoryTrasactionPostDto grinInventoryTranctionDto = new grinInventoryTrasactionPostDto();
                                    grinInventoryTranctionDto.PartNumber = inventoryObject[0].PartNumber;
                                    grinInventoryTranctionDto.LotNumber = inventoryObject[0].LotNumber;
                                    grinInventoryTranctionDto.MftrPartNumber = inventoryObject[0].MftrPartNumber;
                                    grinInventoryTranctionDto.Description = inventoryObject[0].Description;
                                    grinInventoryTranctionDto.ProjectNumber = inventoryObject[0].ProjectNumber;
                                    grinInventoryTranctionDto.Issued_Quantity = inventoryObject[0].Balance_Quantity;
                                    grinInventoryTranctionDto.UOM = inventoryObject[0].UOM;
                                    grinInventoryTranctionDto.Warehouse = grinInventoryDto.Warehouse;
                                    grinInventoryTranctionDto.From_Location = "KIT_IQC";
                                    grinInventoryTranctionDto.TO_Location = grinInventoryDto.Location;
                                    grinInventoryTranctionDto.ReferenceIDFrom = "KIT_Binning";
                                    grinInventoryTranctionDto.GrinMaterialType = "KIT_Binning";
                                    grinInventoryTranctionDto.Remarks = "KIT_Binning";
                                    grinInventoryTranctionDto.GrinNo = inventoryObject[0].GrinNo;
                                    grinInventoryTranctionDto.GrinPartId = inventoryObject[0].GrinPartId;
                                    grinInventoryTranctionDto.PartType = inventoryObject[0].PartType;
                                    grinInventoryTranctionDto.ReferenceID = inventoryObject[0].GrinNo;
                                    grinInventoryTranctionDto.shopOrderNo = "";
                                    grinInventoryTranctionDto.IsStockAvailable = true;

                                    var json1q = JsonConvert.SerializeObject(grinInventoryTranctionDto);
                                    var data1q = new StringContent(json1q, Encoding.UTF8, "application/json");
                                    var request2q = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                    "CreateInventoryTranctionFromGrin"))
                                    {
                                        Content = data1q
                                    };
                                    request2q.Headers.Add("Authorization", token);

                                    var response1q = await client.SendAsync(request2q);
                                    if (response1q.StatusCode != HttpStatusCode.OK)
                                    {
                                        //createinvTrancResp = response1q.StatusCode;
                                    }
                                    foreach (var upInv in inventoryObject)
                                    {
                                        if (upInv.Balance_Quantity <= MoveQty)
                                        {
                                            MoveQty -= upInv.Balance_Quantity;
                                            upInv.Balance_Quantity = 0;
                                        }
                                        else
                                        {
                                            upInv.Balance_Quantity -= MoveQty;
                                            MoveQty = 0;
                                        }
                                        var json = JsonConvert.SerializeObject(inventoryObject);
                                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                                        var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryAPI"],
                                        "UpdateInventory?id=", upInv.Id))
                                        {
                                            Content = data
                                        };
                                        request5.Headers.Add("Authorization", token);
                                        var response = await client.SendAsync(request5);
                                        //if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;
                                        if (MoveQty == 0) break;
                                    }
                                }
                            }                            
                            if (prj.BinnedQty == prj.AcceptedQty) prj.IsKIT_BinningCompleted = true;
                            else prj.IsKIT_BinningCompleted = false;
                        }
                        if (grinitem.BinnedQty == grinitem.AcceptedQty) grinitem.IsKIT_BinningCompleted = true;
                        else grinitem.IsKIT_BinningCompleted = false;
                        var iqcitem = kIT_IQC.kIT_IQCItems.Where(a => a.KIT_GrinPartId == item.KIT_GrinPartId).First();
                        iqcitem.BinnedQty = grinitem.BinnedQty;
                        iqcitem.IsKIT_BinningCompleted = grinitem.IsKIT_BinningCompleted;
                    }
                    kIT_GRIN.IsKIT_BinningCompleted = kIT_GRIN.KIT_GRINParts.All(x => x.IsKIT_BinningCompleted);
                    kIT_IQC.IsKIT_BinningCompleted = kIT_IQC.kIT_IQCItems.All(x => x.IsKIT_BinningCompleted);
                    await _kIT_GRINRepository.UpdateKIT_GRINDetails(kIT_GRIN);
                    await _kIT_IQCRepository.UpdateKIT_IQC(kIT_IQC);
                    _kIT_GRINRepository.SaveAsync();
                    _kIT_IQCRepository.SaveAsync();
                }
                else
                {
                    ExistingkIT_Binning.KIT_BinningItems.Where(x => kIT_Binning.KIT_BinningItems.Select(s => s.KIT_GrinPartId).ToList().Contains(x.KIT_GrinPartId)).ToList().ForEach(a =>
                    {
                        var binitem = kIT_Binning.KIT_BinningItems.Where(x => x.KIT_GrinPartId == a.KIT_GrinPartId).First();
                        a.KIT_BinningItemsLocation.Where(b => binitem.KIT_BinningItemsLocation.Select(s => new { s.Warehouse, s.Location}).ToList().Contains(new { b.Warehouse, b.Location })).ToList().ForEach(b =>
                        {
                            b.Qty += binitem.KIT_BinningItemsLocation.Where(x => x.Warehouse == b.Warehouse && x.Location==b.Location).Select(s => s.Qty).First();
                        });
                        a.KIT_BinningItemsLocation.AddRange(binitem.KIT_BinningItemsLocation.Where(x=>x.Id==0));
                    });
                    ExistingkIT_Binning.KIT_BinningItems.AddRange(kIT_Binning.KIT_BinningItems.Where(x=>x.Id==0));
                    await _kIT_BinningRepository.UpdateKIT_Binning(ExistingkIT_Binning);
                    _kIT_BinningRepository.SaveAsync();

                    foreach (var item in kIT_Binning.KIT_BinningItems)
                    {
                        var grinitem = kIT_GRIN.KIT_GRINParts.Where(a => a.Id == item.KIT_GrinPartId).First();
                        foreach (var prj in grinitem.KIT_GRIN_ProjectNumbers)
                        {
                            foreach (var binloc in item.KIT_BinningItemsLocation.Where(a => a.ProjectNumber == prj.ProjectNumber).ToList())
                            {
                                prj.BinnedQty += binloc.Qty;
                                grinitem.BinnedQty += binloc.Qty;
                                foreach (var compo in prj.KIT_GRIN_KITComponents)
                                {
                                    var client = _clientFactory.CreateClient();
                                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                                    var itemNo = compo.PartNumber;
                                    var encodedItemNo = Uri.EscapeDataString(itemNo);
                                    var grinNo = kIT_Binning.KIT_GrinNumber;
                                    var encodedgrinNo = Uri.EscapeDataString(grinNo);
                                    var grinPartsIds = item.KIT_GrinPartId;
                                    var projectNos = prj.ProjectNumber;
                                    var encodedprojectNos = Uri.EscapeDataString(projectNos);
                                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                    $"GetAllIQCInventoryDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                                    request1.Headers.Add("Authorization", token);
                                    var inventoryObjectResult = await client.SendAsync(request1);
                                    //  if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvdetailsGrinId = inventoryObjectResult.StatusCode;
                                    var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                    var inventoryObjectData = JsonConvert.DeserializeObject<InventoryGetAllDto>(inventoryObjectString);
                                    var inventoryObject = inventoryObjectData.data;

                                    var MoveQty = binloc.Qty * compo.KitComponentBOMQty;
                                    compo.BinnedQty += MoveQty;
                                    GrinInventoryDto grinInventoryDto = new GrinInventoryDto();
                                    grinInventoryDto.PartNumber = inventoryObject[0].PartNumber;
                                    grinInventoryDto.LotNumber = inventoryObject[0].LotNumber;
                                    grinInventoryDto.MftrPartNumber = inventoryObject[0].MftrPartNumber;
                                    grinInventoryDto.Description = inventoryObject[0].Description;
                                    grinInventoryDto.ProjectNumber = inventoryObject[0].ProjectNumber;
                                    grinInventoryDto.Max = inventoryObject[0].Max;
                                    grinInventoryDto.Min = inventoryObject[0].Min;
                                    grinInventoryDto.UOM = inventoryObject[0].UOM;
                                    grinInventoryDto.Warehouse = binloc.Warehouse;
                                    grinInventoryDto.Location = binloc.Location;
                                    grinInventoryDto.ReferenceIDFrom = "KIT_Binning";
                                    grinInventoryDto.Balance_Quantity = MoveQty;
                                    grinInventoryDto.GrinNo = inventoryObject[0].GrinNo;
                                    grinInventoryDto.GrinPartId = inventoryObject[0].GrinPartId;
                                    grinInventoryDto.PartType = inventoryObject[0].PartType;
                                    grinInventoryDto.ReferenceID = inventoryObject[0].ReferenceID;
                                    grinInventoryDto.ReferenceIDFrom = inventoryObject[0].ReferenceIDFrom;
                                    grinInventoryDto.GrinMaterialType = "";
                                    grinInventoryDto.ShopOrderNo = "";

                                    var jsona = JsonConvert.SerializeObject(grinInventoryDto);
                                    var dataa = new StringContent(jsona, Encoding.UTF8, "application/json");
                                    var request1a = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                    "CreateInventoryFromGrin"))
                                    {
                                        Content = dataa
                                    };
                                    request1a.Headers.Add("Authorization", token);

                                    var responsea = await client.SendAsync(request1a);
                                    if (responsea.StatusCode != HttpStatusCode.OK)
                                    {
                                        // createinvResp = responsea.StatusCode;
                                    }

                                    grinInventoryTrasactionPostDto grinInventoryTranctionDto = new grinInventoryTrasactionPostDto();
                                    grinInventoryTranctionDto.PartNumber = inventoryObject[0].PartNumber;
                                    grinInventoryTranctionDto.LotNumber = inventoryObject[0].LotNumber;
                                    grinInventoryTranctionDto.MftrPartNumber = inventoryObject[0].MftrPartNumber;
                                    grinInventoryTranctionDto.Description = inventoryObject[0].Description;
                                    grinInventoryTranctionDto.ProjectNumber = inventoryObject[0].ProjectNumber;
                                    grinInventoryTranctionDto.Issued_Quantity = inventoryObject[0].Balance_Quantity;
                                    grinInventoryTranctionDto.UOM = inventoryObject[0].UOM;
                                    grinInventoryTranctionDto.Warehouse = grinInventoryDto.Warehouse;
                                    grinInventoryTranctionDto.From_Location = "KIT_IQC";
                                    grinInventoryTranctionDto.TO_Location = grinInventoryDto.Location;
                                    grinInventoryTranctionDto.ReferenceIDFrom = "KIT_Binning";
                                    grinInventoryTranctionDto.GrinMaterialType = "KIT_Binning";
                                    grinInventoryTranctionDto.Remarks = "KIT_Binning";
                                    grinInventoryTranctionDto.GrinNo = inventoryObject[0].GrinNo;
                                    grinInventoryTranctionDto.GrinPartId = inventoryObject[0].GrinPartId;
                                    grinInventoryTranctionDto.PartType = inventoryObject[0].PartType;
                                    grinInventoryTranctionDto.ReferenceID = inventoryObject[0].GrinNo;
                                    grinInventoryTranctionDto.shopOrderNo = "";
                                    grinInventoryTranctionDto.IsStockAvailable = true;

                                    var json1q = JsonConvert.SerializeObject(grinInventoryTranctionDto);
                                    var data1q = new StringContent(json1q, Encoding.UTF8, "application/json");
                                    var request2q = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                    "CreateInventoryTranctionFromGrin"))
                                    {
                                        Content = data1q
                                    };
                                    request2q.Headers.Add("Authorization", token);

                                    var response1q = await client.SendAsync(request2q);
                                    if (response1q.StatusCode != HttpStatusCode.OK)
                                    {
                                        //createinvTrancResp = response1q.StatusCode;
                                    }
                                    foreach (var upInv in inventoryObject)
                                    {
                                        if (upInv.Balance_Quantity <= MoveQty)
                                        {
                                            MoveQty -= upInv.Balance_Quantity;
                                            upInv.Balance_Quantity = 0;
                                        }
                                        else
                                        {
                                            upInv.Balance_Quantity -= MoveQty;
                                            MoveQty = 0;
                                        }
                                        var json = JsonConvert.SerializeObject(inventoryObject);
                                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                                        var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryAPI"],
                                        "UpdateInventory?id=", upInv.Id))
                                        {
                                            Content = data
                                        };
                                        request5.Headers.Add("Authorization", token);
                                        var response = await client.SendAsync(request5);
                                        //if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;
                                        if (MoveQty == 0) break;
                                    }
                                }
                            }
                           
                            if (prj.BinnedQty == prj.AcceptedQty) prj.IsKIT_BinningCompleted = true;
                            else prj.IsKIT_BinningCompleted = false;
                        }
                        if (grinitem.BinnedQty == grinitem.AcceptedQty) grinitem.IsKIT_BinningCompleted = true;
                        else grinitem.IsKIT_BinningCompleted = false;
                        var iqcitem = kIT_IQC.kIT_IQCItems.Where(a => a.KIT_GrinPartId == item.KIT_GrinPartId).First();
                        iqcitem.BinnedQty = grinitem.BinnedQty;
                        iqcitem.IsKIT_BinningCompleted = grinitem.IsKIT_BinningCompleted;
                    }
                    kIT_GRIN.IsKIT_BinningCompleted = kIT_GRIN.KIT_GRINParts.All(x => x.IsKIT_BinningCompleted);
                    kIT_IQC.IsKIT_BinningCompleted = kIT_IQC.kIT_IQCItems.All(x => x.IsKIT_BinningCompleted);
                    await _kIT_GRINRepository.UpdateKIT_GRINDetails(kIT_GRIN);
                    await _kIT_IQCRepository.UpdateKIT_IQC(kIT_IQC);
                    _kIT_GRINRepository.SaveAsync();
                    _kIT_IQCRepository.SaveAsync();
                }
                _logger.LogInfo($"KIT_Binning Created for the KIT_GRIN No.:{kIT_Binning.KIT_GrinNumber}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"KIT_Binning Created for the KIT_GRIN No.:{kIT_Binning.KIT_GrinNumber}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in CreateKIT_Binning:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in CreateKIT_Binning:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllKIT_Binning([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<KIT_BinningDto>> serviceResponse = new ServiceResponse<IEnumerable<KIT_BinningDto>>();
            try
            {
                var getAllKIT_BinningDetails = await _kIT_BinningRepository.GetAllKIT_Binning(pagingParameter, searchParams);
                var metadata = new
                {
                    getAllKIT_BinningDetails.TotalCount,
                    getAllKIT_BinningDetails.PageSize,
                    getAllKIT_BinningDetails.CurrentPage,
                    getAllKIT_BinningDetails.HasNext,
                    getAllKIT_BinningDetails.HasPreviuos
                };
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<KIT_BinningDto>>(getAllKIT_BinningDetails);
                _logger.LogInfo("Returned all KIT_Binning details");
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all KIT_Binning";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetAllKIT_Binning:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in GetAllKIT_Binning:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetKIT_BinningbyId([FromQuery] int Id)
        {
            ServiceResponse<KIT_BinningDto> serviceResponse = new ServiceResponse<KIT_BinningDto>();
            try
            {
                var getKIT_Binning = await _kIT_BinningRepository.GetKIT_BinningbyId(Id);
                var result = _mapper.Map<KIT_BinningDto>(getKIT_Binning);
                _logger.LogInfo($"Returned KIT_Binning with Id: {Id}");
                serviceResponse.Data = result;
                serviceResponse.Message = $"Returned KIT_Binning with Id: {Id}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetKIT_BinningbyId for Id: {Id}:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in GetKIT_BinningbyId for Id: {Id}:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
