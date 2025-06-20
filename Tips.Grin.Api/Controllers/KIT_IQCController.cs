using AutoMapper;
using Contracts;
using Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;
using Newtonsoft.Json;
using System.Collections;
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
    public class KIT_IQCController : ControllerBase
    {
        private IKIT_IQCRepository _kIT_IQCRepository;
        private IKIT_GRINRepository _kIT_GRINRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public KIT_IQCController(IKIT_GRINRepository kIT_GRINRepository, IKIT_IQCRepository kIT_IQCRepository, IHttpClientFactory clientFactory, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
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
        public async Task<IActionResult> CreateKIT_IQC([FromBody] KIT_IQCPostDto kIT_IQCPostDto)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                if (kIT_IQCPostDto == null)
                {
                    _logger.LogError("CreateKIT_IQC details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CreateKIT_IQC details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid CreateKIT_IQC details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid CreateKIT_IQC details object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                string serverKey = GetServerKey();
                var kIT_IQC = _mapper.Map<KIT_IQC>(kIT_IQCPostDto);
                var kIT_GRIN = await _kIT_GRINRepository.GetKIT_GrinByKIT_GrinNumber(kIT_IQC.KIT_GrinNumber);
                var ExistingKit_IQC = await _kIT_IQCRepository.GetKIT_IQCbyKIT_GrinNumber(kIT_IQC.KIT_GrinNumber);
                if (ExistingKit_IQC == null)
                {
                    if (serverKey == "avision")
                    {
                        kIT_IQC.KIT_IQCNumber = kIT_IQC.KIT_GrinNumber.Replace("KIT_GRN", "KIT_IQC");
                    }
                    kIT_IQC.kIT_IQCItems.ForEach(x =>
                    {
                        if (x.RejectedQty == kIT_GRIN.KIT_GRINParts.Where(q => q.Id == x.KIT_GrinPartId).Select(v => v.Qty).First()) x.IsKIT_BinningCompleted = true;
                        if (x.BinnedQty != x.AcceptedQty) x.IsKIT_BinningCompleted = false;
                    });
                    kIT_IQC.IsKIT_BinningCompleted = kIT_IQC.kIT_IQCItems.All(x => x.IsKIT_BinningCompleted);
                    await _kIT_IQCRepository.CreateKIT_IQC(kIT_IQC);
                    //_kIT_IQCRepository.SaveAsync();
                    kIT_GRIN.KIT_GRINParts.Where(w=> kIT_IQC.kIT_IQCItems.Select(s=>s.KIT_GrinPartId).Distinct().ToList().Contains(w.Id)).ToList().ForEach(x =>
                    {
                        x.AcceptedQty += kIT_IQC.kIT_IQCItems.Where(a => a.KIT_GrinPartId == x.Id).Select(q => q.AcceptedQty).First();
                        x.RejectedQty += kIT_IQC.kIT_IQCItems.Where(a => a.KIT_GrinPartId == x.Id).Select(q => q.RejectedQty).First();
                        if (x.AcceptedQty + x.RejectedQty == x.Qty) { x.IsKIT_IqcCompleted = true; if (x.RejectedQty == x.Qty) x.IsKIT_BinningCompleted = true; }
                        if (x.BinnedQty != x.AcceptedQty) x.IsKIT_BinningCompleted = false;
                    });
                    kIT_GRIN.IsKIT_IqcCompleted = kIT_GRIN.KIT_GRINParts.All(x => x.IsKIT_IqcCompleted);
                    kIT_GRIN.IsKIT_BinningCompleted = kIT_GRIN.KIT_GRINParts.All(x => x.IsKIT_BinningCompleted);
                    HttpStatusCode getInvdetailsGrinId = HttpStatusCode.OK;
                    HttpStatusCode createinvResp = HttpStatusCode.OK;
                    HttpStatusCode createinvTrancResp = HttpStatusCode.OK;
                    HttpStatusCode updateInv = HttpStatusCode.OK;
                    foreach (var iqcItems in kIT_IQC.kIT_IQCItems)
                    {
                        var Itemaccp = iqcItems.AcceptedQty;
                        var Itemrej = iqcItems.RejectedQty;
                        foreach (var prj in kIT_GRIN.KIT_GRINParts.Where(x => x.Id == iqcItems.KIT_GrinPartId).Select(x => x.KIT_GRIN_ProjectNumbers).First())
                        {
                            decimal prjaccp, prjrej;
                            var prjQty = prj.ProjectQty - (prj.AcceptedQty + prj.RejectReturnQty);
                            if (Itemaccp <= prjQty)
                            {
                                prjaccp = Itemaccp;
                                prj.AcceptedQty += Itemaccp;
                                prjQty -= Itemaccp;
                                Itemaccp = 0;
                            }
                            else
                            {
                                prjaccp = prjQty;
                                prj.AcceptedQty += prjQty;
                                Itemaccp -= prjQty;
                                prjQty = 0;
                            }
                            if (Itemrej <= prjQty)
                            {
                                prjrej = Itemrej;
                                prj.RejectedQty += Itemrej;
                                prjQty -= Itemrej;
                                Itemrej = 0;
                            }
                            else
                            {
                                prjrej = prjQty;
                                prj.RejectedQty += prjQty;
                                Itemrej -= prjQty;
                                prjQty = 0;
                            }
                            if (prj.AcceptedQty + prj.RejectedQty == prj.ProjectQty) { prj.IsKIT_IqcCompleted = true; if (prj.RejectedQty == prj.ProjectQty) prj.IsKIT_BinningCompleted = true; }
                            if (prj.BinnedQty != prj.AcceptedQty) prj.IsKIT_BinningCompleted = false;
                            foreach (var compo in prj.KIT_GRIN_KITComponents)
                            {
                                decimal accp, reje, Invreduce = 0;
                                accp = prjaccp;
                                reje = prjrej;
                                bool conti = true;
                                var client = _clientFactory.CreateClient();
                                var token = HttpContext.Request.Headers["Authorization"].ToString();
                                var itemNo = compo.PartNumber;
                                var encodedItemNo = Uri.EscapeDataString(itemNo);
                                var grinNo = kIT_GRIN.KIT_GrinNumber;
                                var encodedgrinNo = Uri.EscapeDataString(grinNo);
                                var grinPartsIds = iqcItems.KIT_GrinPartId;
                                var projectNos = prj.ProjectNumber;
                                var encodedprojectNos = Uri.EscapeDataString(projectNos);
                                var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                                request1.Headers.Add("Authorization", token);
                                var inventoryObjectResult = await client.SendAsync(request1);
                                if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvdetailsGrinId = inventoryObjectResult.StatusCode;
                                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                var inventoryObjectData = JsonConvert.DeserializeObject<InventoryGetDto>(inventoryObjectString);
                                var inventoryObject = inventoryObjectData.data;
                                while (conti)
                                {
                                    GrinInventoryDto grinInventoryDto = new GrinInventoryDto();
                                    grinInventoryDto.PartNumber = inventoryObject.PartNumber;
                                    grinInventoryDto.LotNumber = inventoryObject.LotNumber;
                                    grinInventoryDto.MftrPartNumber = inventoryObject.MftrPartNumber;
                                    grinInventoryDto.Description = inventoryObject.Description;
                                    grinInventoryDto.ProjectNumber = inventoryObject.ProjectNumber;
                                    grinInventoryDto.Max = inventoryObject.Max;
                                    grinInventoryDto.Min = inventoryObject.Min;
                                    grinInventoryDto.UOM = inventoryObject.UOM;
                                    if (accp > 0)
                                    {
                                        grinInventoryDto.Warehouse = "IQC";
                                        grinInventoryDto.Location = "IQC";
                                        grinInventoryDto.ReferenceIDFrom = "KIT_IQC";
                                        grinInventoryDto.Balance_Quantity = prjaccp * compo.KitComponentBOMQty;
                                        Invreduce += prjaccp * compo.KitComponentBOMQty;
                                        compo.AcceptedQty = prjaccp * compo.KitComponentBOMQty;
                                        accp = 0;
                                    }
                                    else
                                    {
                                        grinInventoryDto.Warehouse = "Reject";
                                        grinInventoryDto.Location = "Reject";
                                        grinInventoryDto.ReferenceIDFrom = "KIT_IQC";
                                        grinInventoryDto.Balance_Quantity = prjrej * compo.KitComponentBOMQty;
                                        Invreduce += prjrej * compo.KitComponentBOMQty;
                                        compo.RejectedQty = prjrej * compo.KitComponentBOMQty;
                                        reje = 0;
                                    }
                                    grinInventoryDto.GrinNo = inventoryObject.GrinNo;
                                    grinInventoryDto.GrinPartId = inventoryObject.GrinPartId;
                                    grinInventoryDto.PartType = inventoryObject.PartType;
                                    grinInventoryDto.ReferenceID = inventoryObject.ReferenceID;
                                    grinInventoryDto.ReferenceIDFrom = inventoryObject.ReferenceIDFrom;
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
                                        createinvResp = responsea.StatusCode;
                                    }

                                    grinInventoryTrasactionPostDto grinInventoryTranctionDto = new grinInventoryTrasactionPostDto();
                                    grinInventoryTranctionDto.PartNumber = inventoryObject.PartNumber;
                                    grinInventoryTranctionDto.LotNumber = inventoryObject.LotNumber;
                                    grinInventoryTranctionDto.MftrPartNumber = inventoryObject.MftrPartNumber;
                                    grinInventoryTranctionDto.Description = inventoryObject.Description;
                                    grinInventoryTranctionDto.ProjectNumber = inventoryObject.ProjectNumber;
                                    grinInventoryTranctionDto.Issued_Quantity = inventoryObject.Balance_Quantity;
                                    grinInventoryTranctionDto.UOM = inventoryObject.UOM;
                                    grinInventoryTranctionDto.Warehouse = grinInventoryDto.Warehouse;
                                    grinInventoryTranctionDto.From_Location = "KIT_GRIN";
                                    grinInventoryTranctionDto.TO_Location = grinInventoryDto.Location;
                                    grinInventoryTranctionDto.ReferenceIDFrom = "KIT_IQC";
                                    grinInventoryTranctionDto.GrinMaterialType = "KIT_IQC";
                                    grinInventoryTranctionDto.Remarks = "KIT_IQC";
                                    grinInventoryTranctionDto.GrinNo = inventoryObject.GrinNo;
                                    grinInventoryTranctionDto.GrinPartId = inventoryObject.GrinPartId;
                                    grinInventoryTranctionDto.PartType = inventoryObject.PartType;
                                    grinInventoryTranctionDto.ReferenceID = inventoryObject.GrinNo;
                                    grinInventoryTranctionDto.shopOrderNo = "";
                                    grinInventoryTranctionDto.IsStockAvailable = true;
                                    grinInventoryTranctionDto.TransactionType = InventoryType.Inward;

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
                                        createinvTrancResp = response1q.StatusCode;
                                    }

                                    grinInventoryTrasactionPostDto grinInventoryTranctionDto_1 = new grinInventoryTrasactionPostDto();
                                    grinInventoryTranctionDto_1.PartNumber = inventoryObject.PartNumber;
                                    grinInventoryTranctionDto_1.LotNumber = inventoryObject.LotNumber;
                                    grinInventoryTranctionDto_1.MftrPartNumber = inventoryObject.MftrPartNumber;
                                    grinInventoryTranctionDto_1.Description = inventoryObject.Description;
                                    grinInventoryTranctionDto_1.ProjectNumber = inventoryObject.ProjectNumber;
                                    grinInventoryTranctionDto_1.Issued_Quantity = inventoryObject.Balance_Quantity;
                                    grinInventoryTranctionDto_1.UOM = inventoryObject.UOM;
                                    grinInventoryTranctionDto_1.Warehouse = "GRIN";
                                    grinInventoryTranctionDto_1.From_Location = "GRIN";
                                    grinInventoryTranctionDto_1.TO_Location = grinInventoryDto.Location;
                                    grinInventoryTranctionDto_1.ReferenceIDFrom = "KIT_IQC";
                                    grinInventoryTranctionDto_1.GrinMaterialType = "";
                                    grinInventoryTranctionDto_1.Remarks = "KIT_IQC";
                                    grinInventoryTranctionDto_1.GrinNo = inventoryObject.GrinNo;
                                    grinInventoryTranctionDto_1.GrinPartId = inventoryObject.GrinPartId;
                                    grinInventoryTranctionDto_1.PartType = inventoryObject.PartType;
                                    grinInventoryTranctionDto_1.ReferenceID = inventoryObject.GrinNo;
                                    grinInventoryTranctionDto_1.shopOrderNo = "";
                                    grinInventoryTranctionDto_1.IsStockAvailable = false;
                                    grinInventoryTranctionDto_1.TransactionType = InventoryType.Outward;

                                    var json1q_1 = JsonConvert.SerializeObject(grinInventoryTranctionDto_1);
                                    var data1q_1 = new StringContent(json1q_1, Encoding.UTF8, "application/json");
                                    var request2q_1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                    "CreateInventoryTranctionFromGrin"))
                                    {
                                        Content = data1q_1
                                    };
                                    request2q_1.Headers.Add("Authorization", token);

                                    var response1q_1 = await client.SendAsync(request2q_1);
                                    if (response1q_1.StatusCode != HttpStatusCode.OK)
                                    {
                                        createinvTrancResp = response1q_1.StatusCode;
                                    }
                                    if (accp == 0 && reje == 0) conti = false;
                                }
                                inventoryObject.Balance_Quantity -= Invreduce;
                                var json = JsonConvert.SerializeObject(inventoryObject);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");
                                var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryAPI"],
                                "UpdateInventory?id=", inventoryObject.Id))
                                {
                                    Content = data
                                };
                                request5.Headers.Add("Authorization", token);
                                var response = await client.SendAsync(request5);
                                if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;
                            }
                            if (Itemaccp == 0 && Itemrej == 0) break;
                        }
                    }
                    await _kIT_GRINRepository.UpdateKIT_GRINDetails(kIT_GRIN);
                    _kIT_GRINRepository.SaveAsync();
                    if (getInvdetailsGrinId != HttpStatusCode.OK && updateInv != HttpStatusCode.OK && createinvTrancResp != HttpStatusCode.OK && createinvResp != HttpStatusCode.OK)
                    {
                        _logger.LogError($"Something went wrong inside CreateKIT_IQC action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }
                    _logger.LogInfo($"KIT_IQC Created for the KIT_GRIN No.:{kIT_IQC.KIT_GrinNumber}");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"KIT_IQC Created for the KIT_GRIN No.:{kIT_IQC.KIT_GrinNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    ExistingKit_IQC.kIT_IQCItems.Where(q => kIT_IQC.kIT_IQCItems.Select(a => a.KIT_GrinPartId).ToList().Contains(q.KIT_GrinPartId)).ToList().ForEach(x =>
                    {
                        x.AcceptedQty += kIT_IQC.kIT_IQCItems.Where(a => a.KIT_GrinPartId == x.KIT_GrinPartId).Select(q => q.AcceptedQty).First();
                        x.RejectedQty += kIT_IQC.kIT_IQCItems.Where(a => a.KIT_GrinPartId == x.KIT_GrinPartId).Select(q => q.RejectedQty).First();
                        if (x.RejectedQty == kIT_GRIN.KIT_GRINParts.Where(q => q.Id == x.KIT_GrinPartId).Select(v => v.Qty).First()) x.IsKIT_BinningCompleted = true;
                        if (x.BinnedQty != x.AcceptedQty) x.IsKIT_BinningCompleted = false;
                    });
                    ExistingKit_IQC.kIT_IQCItems.AddRange(kIT_IQC.kIT_IQCItems.Where(q => !ExistingKit_IQC.kIT_IQCItems.Select(a => a.KIT_GrinPartId).ToList().Contains(q.KIT_GrinPartId)));
                    ExistingKit_IQC.IsKIT_BinningCompleted = ExistingKit_IQC.kIT_IQCItems.All(x => x.IsKIT_BinningCompleted);
                    await _kIT_IQCRepository.UpdateKIT_IQC(ExistingKit_IQC);
                    _kIT_IQCRepository.SaveAsync();
                    kIT_GRIN.KIT_GRINParts.Where(w => kIT_IQC.kIT_IQCItems.Select(s => s.KIT_GrinPartId).Distinct().ToList().Contains(w.Id)).ToList().ForEach(x =>
                    {
                        x.AcceptedQty += kIT_IQC.kIT_IQCItems.Where(a => a.KIT_GrinPartId == x.Id).Select(q => q.AcceptedQty).First();
                        x.RejectedQty += kIT_IQC.kIT_IQCItems.Where(a => a.KIT_GrinPartId == x.Id).Select(q => q.RejectedQty).First();
                        if (x.AcceptedQty + x.RejectedQty == x.Qty) { x.IsKIT_IqcCompleted = true; if (x.RejectedQty == x.Qty) x.IsKIT_BinningCompleted = true; }
                        if (x.BinnedQty != x.AcceptedQty) x.IsKIT_BinningCompleted = false;
                    });
                    kIT_GRIN.IsKIT_IqcCompleted = kIT_GRIN.KIT_GRINParts.All(x => x.IsKIT_IqcCompleted);
                    kIT_GRIN.IsKIT_BinningCompleted = kIT_GRIN.KIT_GRINParts.All(x => x.IsKIT_BinningCompleted);
                    HttpStatusCode getInvdetailsGrinId = HttpStatusCode.OK;
                    HttpStatusCode createinvResp = HttpStatusCode.OK;
                    HttpStatusCode createinvTrancResp = HttpStatusCode.OK;
                    HttpStatusCode updateInv = HttpStatusCode.OK;
                    foreach (var iqcItems in kIT_IQC.kIT_IQCItems)
                    {
                        var Itemaccp = iqcItems.AcceptedQty;
                        var Itemrej = iqcItems.RejectedQty;
                        foreach (var prj in kIT_GRIN.KIT_GRINParts.Where(x => x.Id == iqcItems.KIT_GrinPartId).Select(x => x.KIT_GRIN_ProjectNumbers).First())
                        {
                            decimal prjaccp, prjrej;
                            var prjQty = prj.ProjectQty-(prj.AcceptedQty+prj.RejectReturnQty);
                            if (Itemaccp <= prjQty)
                            {
                                prjaccp = Itemaccp;
                                prj.AcceptedQty += Itemaccp;
                                prjQty -= Itemaccp;
                                Itemaccp = 0;
                            }
                            else
                            {
                                prjaccp = prjQty;
                                prj.AcceptedQty += prjQty;
                                Itemaccp -= prjQty;
                                prjQty = 0;
                            }
                            if (Itemrej <= prjQty)
                            {
                                prjrej = Itemrej;
                                prj.RejectedQty += Itemrej;
                                prjQty -= Itemrej;
                                Itemrej = 0;
                            }
                            else
                            {
                                prjrej = prjQty;
                                prj.RejectedQty += prjQty;
                                Itemrej -= prjQty;
                                prjQty = 0;
                            }
                            if (prj.AcceptedQty + prj.RejectedQty == prj.ProjectQty) { prj.IsKIT_IqcCompleted = true; if (prj.RejectedQty == prj.ProjectQty) prj.IsKIT_BinningCompleted = true; }
                            if (prj.BinnedQty != prj.AcceptedQty) prj.IsKIT_BinningCompleted = false;
                            foreach (var compo in prj.KIT_GRIN_KITComponents)
                            {
                                decimal accp, reje, Invreduce = 0;
                                accp = prjaccp;
                                reje = prjrej;
                                bool conti = true;
                                var client = _clientFactory.CreateClient();
                                var token = HttpContext.Request.Headers["Authorization"].ToString();
                                var itemNo = compo.PartNumber;
                                var encodedItemNo = Uri.EscapeDataString(itemNo);
                                var grinNo = kIT_GRIN.KIT_GrinNumber;
                                var encodedgrinNo = Uri.EscapeDataString(grinNo);
                                var grinPartsIds = iqcItems.KIT_GrinPartId;
                                var projectNos = prj.ProjectNumber;
                                var encodedprojectNos = Uri.EscapeDataString(projectNos);
                                var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                                request1.Headers.Add("Authorization", token);
                                var inventoryObjectResult = await client.SendAsync(request1);
                                if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvdetailsGrinId = inventoryObjectResult.StatusCode;
                                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                var inventoryObjectData = JsonConvert.DeserializeObject<InventoryGetDto>(inventoryObjectString);
                                var inventoryObject = inventoryObjectData.data;
                                while (conti)
                                {
                                    GrinInventoryDto grinInventoryDto = new GrinInventoryDto();
                                    grinInventoryDto.PartNumber = inventoryObject.PartNumber;
                                    grinInventoryDto.LotNumber = inventoryObject.LotNumber;
                                    grinInventoryDto.MftrPartNumber = inventoryObject.MftrPartNumber;
                                    grinInventoryDto.Description = inventoryObject.Description;
                                    grinInventoryDto.ProjectNumber = inventoryObject.ProjectNumber;
                                    grinInventoryDto.Max = inventoryObject.Max;
                                    grinInventoryDto.Min = inventoryObject.Min;
                                    grinInventoryDto.UOM = inventoryObject.UOM;
                                    if (accp > 0)
                                    {
                                        grinInventoryDto.Warehouse = "IQC";
                                        grinInventoryDto.Location = "IQC";
                                        grinInventoryDto.ReferenceIDFrom = "KIT_IQC";
                                        grinInventoryDto.Balance_Quantity = prjaccp * compo.KitComponentBOMQty;
                                        Invreduce += prjaccp * compo.KitComponentBOMQty;
                                        compo.AcceptedQty = prjaccp * compo.KitComponentBOMQty;
                                        accp = 0;
                                    }
                                    else
                                    {
                                        grinInventoryDto.Warehouse = "Reject";
                                        grinInventoryDto.Location = "Reject";
                                        grinInventoryDto.ReferenceIDFrom = "KIT_IQC";
                                        grinInventoryDto.Balance_Quantity = prjrej * compo.KitComponentBOMQty;
                                        Invreduce += prjrej * compo.KitComponentBOMQty;
                                        compo.RejectedQty = prjrej * compo.KitComponentBOMQty;
                                        reje = 0;
                                    }
                                    grinInventoryDto.GrinNo = inventoryObject.GrinNo;
                                    grinInventoryDto.GrinPartId = inventoryObject.GrinPartId;
                                    grinInventoryDto.PartType = inventoryObject.PartType;
                                    grinInventoryDto.ReferenceID = inventoryObject.ReferenceID;
                                    grinInventoryDto.ReferenceIDFrom = inventoryObject.ReferenceIDFrom;
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
                                        createinvResp = responsea.StatusCode;
                                    }

                                    grinInventoryTrasactionPostDto grinInventoryTranctionDto = new grinInventoryTrasactionPostDto();
                                    grinInventoryTranctionDto.PartNumber = inventoryObject.PartNumber;
                                    grinInventoryTranctionDto.LotNumber = inventoryObject.LotNumber;
                                    grinInventoryTranctionDto.MftrPartNumber = inventoryObject.MftrPartNumber;
                                    grinInventoryTranctionDto.Description = inventoryObject.Description;
                                    grinInventoryTranctionDto.ProjectNumber = inventoryObject.ProjectNumber;
                                    grinInventoryTranctionDto.Issued_Quantity = inventoryObject.Balance_Quantity;
                                    grinInventoryTranctionDto.UOM = inventoryObject.UOM;
                                    grinInventoryTranctionDto.Warehouse = grinInventoryDto.Warehouse;
                                    grinInventoryTranctionDto.From_Location = "KIT_GRIN";
                                    grinInventoryTranctionDto.TO_Location = grinInventoryDto.Location;
                                    grinInventoryTranctionDto.ReferenceIDFrom = "KIT_IQC";
                                    grinInventoryTranctionDto.GrinMaterialType = "KIT_IQC";
                                    grinInventoryTranctionDto.Remarks = "KIT_IQC";
                                    grinInventoryTranctionDto.GrinNo = inventoryObject.GrinNo;
                                    grinInventoryTranctionDto.GrinPartId = inventoryObject.GrinPartId;
                                    grinInventoryTranctionDto.PartType = inventoryObject.PartType;
                                    grinInventoryTranctionDto.ReferenceID = inventoryObject.GrinNo;
                                    grinInventoryTranctionDto.shopOrderNo = "";
                                    grinInventoryTranctionDto.IsStockAvailable = true;
                                    grinInventoryTranctionDto.TransactionType = InventoryType.Inward;

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
                                        createinvTrancResp = response1q.StatusCode;
                                    }

                                    grinInventoryTrasactionPostDto grinInventoryTranctionDto_1 = new grinInventoryTrasactionPostDto();
                                    grinInventoryTranctionDto_1.PartNumber = inventoryObject.PartNumber;
                                    grinInventoryTranctionDto_1.LotNumber = inventoryObject.LotNumber;
                                    grinInventoryTranctionDto_1.MftrPartNumber = inventoryObject.MftrPartNumber;
                                    grinInventoryTranctionDto_1.Description = inventoryObject.Description;
                                    grinInventoryTranctionDto_1.ProjectNumber = inventoryObject.ProjectNumber;
                                    grinInventoryTranctionDto_1.Issued_Quantity = inventoryObject.Balance_Quantity;
                                    grinInventoryTranctionDto_1.UOM = inventoryObject.UOM;
                                    grinInventoryTranctionDto_1.Warehouse = "KIT_GRIN";
                                    grinInventoryTranctionDto_1.From_Location = "KIT_GRIN";
                                    grinInventoryTranctionDto_1.TO_Location = grinInventoryDto.Location;
                                    grinInventoryTranctionDto_1.ReferenceIDFrom = "KIT_IQC";
                                    grinInventoryTranctionDto_1.GrinMaterialType = "KIT_IQC";
                                    grinInventoryTranctionDto_1.Remarks = "KIT_IQC";
                                    grinInventoryTranctionDto_1.GrinNo = inventoryObject.GrinNo;
                                    grinInventoryTranctionDto_1.GrinPartId = inventoryObject.GrinPartId;
                                    grinInventoryTranctionDto_1.PartType = inventoryObject.PartType;
                                    grinInventoryTranctionDto_1.ReferenceID = inventoryObject.GrinNo;
                                    grinInventoryTranctionDto_1.shopOrderNo = "";
                                    grinInventoryTranctionDto_1.IsStockAvailable = false;
                                    grinInventoryTranctionDto_1.TransactionType = InventoryType.Outward;

                                    var json1q_1 = JsonConvert.SerializeObject(grinInventoryTranctionDto_1);
                                    var data1q_1 = new StringContent(json1q_1, Encoding.UTF8, "application/json");
                                    var request2q_1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                    "CreateInventoryTranctionFromGrin"))
                                    {
                                        Content = data1q_1
                                    };
                                    request2q_1.Headers.Add("Authorization", token);

                                    var response1q_1 = await client.SendAsync(request2q_1);
                                    if (response1q_1.StatusCode != HttpStatusCode.OK)
                                    {
                                        createinvTrancResp = response1q_1.StatusCode;
                                    }
                                    if (accp == 0 && reje == 0) conti = false;
                                }
                                inventoryObject.Balance_Quantity -= Invreduce;
                                var json = JsonConvert.SerializeObject(inventoryObject);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");
                                var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryAPI"],
                                "UpdateInventory?id=", inventoryObject.Id))
                                {
                                    Content = data
                                };
                                request5.Headers.Add("Authorization", token);
                                var response = await client.SendAsync(request5);
                                if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;
                            }
                            if (Itemaccp == 0 && Itemrej == 0) break;
                        }
                    }
                    await _kIT_GRINRepository.UpdateKIT_GRINDetails(kIT_GRIN);
                    _kIT_GRINRepository.SaveAsync();
                    if (getInvdetailsGrinId != HttpStatusCode.OK && updateInv != HttpStatusCode.OK && createinvTrancResp != HttpStatusCode.OK && createinvResp != HttpStatusCode.OK)
                    {
                        _logger.LogError($"Something went wrong inside CreateKIT_IQC action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }
                    _logger.LogInfo($"KIT_IQC Created for the KIT_GRIN No.:{kIT_IQC.KIT_GrinNumber}");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"KIT_IQC Created for the KIT_GRIN No.:{kIT_IQC.KIT_GrinNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in CreateKIT_IQC:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in CreateKIT_IQC:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllKIT_IQC([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<KIT_IQCDto>> serviceResponse = new ServiceResponse<IEnumerable<KIT_IQCDto>>();
            try
            {
                var getAllIQCDetails = await _kIT_IQCRepository.GetAllKIT_IQC(pagingParameter, searchParams);
                var metadata = new
                {
                    getAllIQCDetails.TotalCount,
                    getAllIQCDetails.PageSize,
                    getAllIQCDetails.CurrentPage,
                    getAllIQCDetails.HasNext,
                    getAllIQCDetails.HasPreviuos
                };
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<KIT_IQCDto>>(getAllIQCDetails);
                _logger.LogInfo("Returned all KIT_IQC details");
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all KIT_IQC";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetAllKIT_IQC:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in GetAllKIT_IQC:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetKIT_IQCbyId([FromQuery] int Id)
        {
            ServiceResponse<KIT_IQCDto> serviceResponse = new ServiceResponse<KIT_IQCDto>();
            try
            {
                var getKIT_IQC = await _kIT_IQCRepository.GetKIT_IQCbyId(Id);
                var result = _mapper.Map<KIT_IQCDto>(getKIT_IQC);
                _logger.LogInfo($"Returned KIT_IQC with Id: {Id}");
                serviceResponse.Data = result;
                serviceResponse.Message = $"Returned KIT_IQC with Id: {Id}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetKIT_IQCbyId for Id: {Id}:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in GetKIT_IQCbyId for Id: {Id}:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
