using AutoMapper;
using Azure;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System.Data;
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
    public class KIT_GRINController : ControllerBase
    {
        private IKIT_GRINRepository _repository;
        private IKIT_GRINPartsRepository _kIT_GRINPartsRepository;
        private IKIT_IQCRepository _KIT_IQCRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IDocumentUploadRepository _documentUploadRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly String _createdBy;
        private readonly String _unitname;
        public KIT_GRINController(IKIT_GRINRepository repository, IKIT_IQCRepository KIT_IQCRepository, IKIT_GRINPartsRepository kIT_GRINPartsRepository, IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor, IDocumentUploadRepository documentUploadRepository, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _repository = repository;
            _kIT_GRINPartsRepository = kIT_GRINPartsRepository;
            _KIT_IQCRepository = KIT_IQCRepository;
            _logger = logger;
            _mapper = mapper;
            _documentUploadRepository = documentUploadRepository;
            _httpClient = httpClient;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
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
        public async Task<IActionResult> CreateKITGRIN([FromBody] KIT_GRINPostDto kIT_GRINPostDto)
        {
            ServiceResponse<string?> serviceResponse = new ServiceResponse<string?>();

            try
            {
                string serverKey = GetServerKey();

                if (kIT_GRINPostDto is null)
                {
                    _logger.LogError("KIT_Grin object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "KIT_Grin object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid KIT_Grin object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid KIT_Grin object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var grins = _mapper.Map<KIT_GRIN>(kIT_GRINPostDto);
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));
                HttpStatusCode invStatusCode = HttpStatusCode.OK;
                if (serverKey == "avision")
                {
                    var grinNum = await _repository.GenerateKIT_GrinNumberForAvision();
                    grins.KIT_GrinNumber = grinNum;
                }
                else
                {
                    var dateFormat = days + months + years;
                    var grinNumber = await _repository.GenerateKIT_GrinNumber();
                    var grinNo = dateFormat + grinNumber;
                    grins.KIT_GrinNumber = grinNo;
                }
                decimal? othercosttotal = 1;
                if ((grins.Freight + grins.Insurance + grins.LoadingorUnLoading + grins.Transport) > 1) othercosttotal = grins.Freight + grins.Insurance + grins.LoadingorUnLoading + grins.Transport;
                decimal? conversionrate = 1;
                if (grins.CurrencyConversion > 1) conversionrate = grins.CurrencyConversion;
                foreach (var part in grins.KIT_GRINParts)
                {
                    foreach (var prj in part.KIT_GRIN_ProjectNumbers)
                    {
                        var kit_comp = _mapper.Map<List<KIT_GRIN_KITComponentscalculationofAvgcost>>(prj.KIT_GRIN_KITComponents);
                        List<KIT_GRIN_KITComponents> KITComponentsList = new List<KIT_GRIN_KITComponents>();
                        foreach (var KC in kit_comp)
                        {
                            decimal? EP = KC.KitComponentQty * KC.KitComponentUnitPrice;
                            //decimal? Itemwithtax = gPart.SGST + gPart.IGST + gPart.CGST + gPart.UTGST + gPart.Duties;
                            //*if (Itemwithtax == null || Itemwithtax == 0) gPart.EPwithTax = EP * conversionrate;
                            //else gPart.EPwithTax = (EP + (EP * (Itemwithtax / 100))) * conversionrate;
                            KC.EPwithTax = EP * conversionrate;
                            KC.EPforSingleQty = KC.EPwithTax / KC.KitComponentQty;
                        }
                        decimal? SumofEPwithtax = kit_comp.Sum(x => x.EPwithTax);
                        foreach (var KC in kit_comp)
                        {
                            decimal? distriduteOthercostforitem = (KC.EPwithTax / SumofEPwithtax) * othercosttotal;
                            decimal? distriduteOthercostforitemsSingleQty = distriduteOthercostforitem / KC.KitComponentQty;
                            KC.AverageCost = distriduteOthercostforitemsSingleQty + KC.EPforSingleQty;
                            if (KC.AverageCost == null) KC.AverageCost = 0;
                            KIT_GRIN_KITComponents kITComponents = _mapper.Map<KIT_GRIN_KITComponents>(KC);
                            KITComponentsList.Add(kITComponents);
                        }
                        prj.KIT_GRIN_KITComponents = KITComponentsList;
                    }
                }
                await _repository.CreateKIT_Grin(grins);
                _repository.SaveAsync();
                if (grins.KIT_GRINParts != null)
                {
                    foreach (var grinPart in grins.KIT_GRINParts)
                    {
                        var grinPartsId = await _kIT_GRINPartsRepository.GetKIT_GRINPartsById(grinPart.Id);
                        //grinPartsId.LotNumber = grins.KIT_GrinNumber + grinPartsId.Id;
                        foreach (var prj in grinPartsId.KIT_GRIN_ProjectNumbers)
                        {
                            foreach (var KC in prj.KIT_GRIN_KITComponents)
                            {
                                KC.LotNumber = grins.KIT_GrinNumber + grinPartsId.Id + "Compo" + KC.Id;
                            }
                        }
                        await _kIT_GRINPartsRepository.UpdateKIT_GRINPartsQty(grinPartsId);
                    }
                }
                _kIT_GRINPartsRepository.SaveAsync();
                HttpStatusCode createinvResp = HttpStatusCode.OK;
                HttpStatusCode createinvTrancResp = HttpStatusCode.OK;
                HttpStatusCode getItemmResp = HttpStatusCode.OK;
                HttpStatusCode getItemForIqcResp = HttpStatusCode.OK;
                // HttpStatusCode UpdatePoStatus = HttpStatusCode.OK;
                HttpStatusCode UpdatePoQty = HttpStatusCode.OK;
                // HttpStatusCode UpdatePoProjQty = HttpStatusCode.OK;
                var clientz = _clientFactory.CreateClient();
                var tokenz = HttpContext.Request.Headers["Authorization"].ToString();
                var jsonz = JsonConvert.SerializeObject(grins.KIT_GRINParts.Select(x => x.ItemNumber).Distinct().ToList());
                var dataz = new StringContent(jsonz, Encoding.UTF8, "application/json");
                var requestz = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterEnggAPI"],
                    $"GetItemDetailsByItemNumberList"))
                {
                    Content = dataz
                };
                requestz.Headers.Add("Authorization", tokenz);
                var itemMasterObjectResultz = await clientz.SendAsync(requestz);
                if (itemMasterObjectResultz.StatusCode != HttpStatusCode.OK) getItemForIqcResp = itemMasterObjectResultz.StatusCode;
                var itemString1z = await itemMasterObjectResultz.Content.ReadAsStringAsync();
                var ItemStringdetails1z = JsonConvert.DeserializeObject<ItemMasterDto>(itemString1z);
                var PartsObject = ItemStringdetails1z.data;
                if (PartsObject.Where(b => b.IsIQCRequired == false).Count() > 1)
                {
                    KIT_IQC kIT_IQC = new KIT_IQC()
                    {
                        KIT_GrinNumber = grins.KIT_GrinNumber,
                        KIT_GrinId = grins.Id,
                        VendorId = grins.VendorId,
                        VendorName = grins.VendorName,
                        VendorNumber = grins.VendorNumber,
                        IsKIT_BinningCompleted = false,
                        kIT_IQCItems = new List<KIT_IQCItems>()
                    };
                    grins.KIT_GRINParts.Where(a => PartsObject.Where(b => b.IsIQCRequired == false).Select(c => c.ItemNumber).ToList().Contains(a.ItemNumber)).ToList()
                    .ForEach(x =>
                    {
                        x.AcceptedQty = x.Qty;
                        x.RejectedQty = 0;
                        x.IsKIT_IqcCompleted = true;
                        kIT_IQC.kIT_IQCItems.Add(new KIT_IQCItems()
                        {
                            ItemNumber = x.ItemNumber,
                            KIT_GrinPartId = x.Id,
                            AcceptedQty = x.AcceptedQty,
                            RejectedQty = x.RejectedQty,
                            RejectReturnQty = x.RejectReturnQty,
                            IsKIT_BinningCompleted = false
                        });
                    });
                    if (grins.KIT_GRINParts.Count() == kIT_IQC.kIT_IQCItems.Count()) grins.IsKIT_IqcCompleted = true;
                    await _repository.UpdateKIT_GRINDetails(grins);
                    _repository.SaveAsync();
                    if (serverKey == "avision")
                    {
                        kIT_IQC.KIT_IQCNumber = kIT_IQC.KIT_GrinNumber.Replace("KGRN", "KIQC");
                    }
                    await _KIT_IQCRepository.CreateKIT_IQC(kIT_IQC);
                    _KIT_IQCRepository.SaveAsync();
                }
                foreach (var parts in grins.KIT_GRINParts)
                {
                    foreach (var prj in parts.KIT_GRIN_ProjectNumbers)
                    {
                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();
                        var json = JsonConvert.SerializeObject(prj.KIT_GRIN_KITComponents.Select(x => x.PartNumber).ToList());
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterEnggAPI"],
                            $"GetItemDetailsByItemNumberList"))
                        {
                            Content = data
                        };
                        request.Headers.Add("Authorization", token);
                        var itemMasterObjectResult = await client.SendAsync(request);
                        if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK) getItemmResp = itemMasterObjectResult.StatusCode;
                        var itemString1 = await itemMasterObjectResult.Content.ReadAsStringAsync();
                        var ItemStringdetails1 = JsonConvert.DeserializeObject<ItemMasterDto>(itemString1);
                        var itemMasterObject = ItemStringdetails1.data;
                        foreach (var KC in prj.KIT_GRIN_KITComponents)
                        {
                            GrinInventoryDto grinInventoryDto = new GrinInventoryDto();
                            grinInventoryDto.PartNumber = KC.PartNumber;
                            grinInventoryDto.LotNumber = KC.LotNumber;
                            grinInventoryDto.MftrPartNumber = KC.MftrItemNumbers;
                            grinInventoryDto.Description = KC.Description;
                            grinInventoryDto.ProjectNumber = prj.ProjectNumber;
                            grinInventoryDto.Balance_Quantity = KC.KitComponentQty;
                            grinInventoryDto.Max = itemMasterObject.Where(x => x.ItemNumber == KC.PartNumber).Select(x => x.Max).First();
                            grinInventoryDto.Min = itemMasterObject.Where(x => x.ItemNumber == KC.PartNumber).Select(x => x.Min).First();
                            grinInventoryDto.UOM = KC.UOM;
                            if (PartsObject.Where(x => x.IsIQCRequired == false).Select(x => x.ItemNumber).ToList().Contains(parts.ItemNumber))
                            {
                                grinInventoryDto.Warehouse = "IQC";
                                grinInventoryDto.Location = "IQC";
                                grinInventoryDto.ReferenceIDFrom = "KIT_IQC";
                            }
                            else
                            {
                                grinInventoryDto.Warehouse = "GRIN";
                                grinInventoryDto.Location = "GRIN";
                                grinInventoryDto.ReferenceIDFrom = "KIT_GRIN";
                            }
                            grinInventoryDto.GrinNo = grins.KIT_GrinNumber;
                            grinInventoryDto.GrinPartId = parts.Id;
                            grinInventoryDto.PartType = KC.PartType;
                            grinInventoryDto.ReferenceID = grins.KIT_GrinNumber;
                            grinInventoryDto.GrinMaterialType = "";
                            grinInventoryDto.ShopOrderNo = "";

                            var jsona = JsonConvert.SerializeObject(grinInventoryDto);
                            var dataa = new StringContent(jsona, Encoding.UTF8, "application/json");
                            var client1a = _clientFactory.CreateClient();
                            var token1a = HttpContext.Request.Headers["Authorization"].ToString();
                            var request1a = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                            "CreateInventoryFromGrin"))
                            {
                                Content = dataa
                            };
                            request1a.Headers.Add("Authorization", token1a);

                            var responsea = await client1a.SendAsync(request1a);
                            if (responsea.StatusCode != HttpStatusCode.OK)
                            {
                                createinvResp = responsea.StatusCode;
                            }

                            grinInventoryTrasactionPostDto grinInventoryTranctionDto = new grinInventoryTrasactionPostDto();
                            grinInventoryTranctionDto.PartNumber = KC.PartNumber;
                            grinInventoryTranctionDto.LotNumber = KC.LotNumber;
                            grinInventoryTranctionDto.MftrPartNumber = KC.MftrItemNumbers;
                            grinInventoryTranctionDto.Description = KC.Description;
                            grinInventoryTranctionDto.ProjectNumber = prj.ProjectNumber;
                            grinInventoryTranctionDto.Issued_Quantity = KC.KitComponentQty;
                            grinInventoryTranctionDto.Issued_DateTime = DateTime.Now;
                            grinInventoryTranctionDto.Issued_By = _createdBy;
                            grinInventoryTranctionDto.UOM = parts.UOM;
                            if (PartsObject.Where(x => x.IsIQCRequired == false).Select(x => x.ItemNumber).ToList().Contains(parts.ItemNumber))
                            {
                                grinInventoryTranctionDto.Warehouse = "IQC";
                                grinInventoryTranctionDto.From_Location = "IQC";
                                grinInventoryTranctionDto.TO_Location = "IQC";
                                grinInventoryTranctionDto.ReferenceIDFrom = "KIT_IQC";
                                grinInventoryTranctionDto.GrinMaterialType = "KIT_IQC";
                                grinInventoryTranctionDto.Remarks = "KIT_IQC";
                            }
                            else
                            {
                                grinInventoryTranctionDto.Warehouse = "GRIN";
                                grinInventoryTranctionDto.From_Location = "GRIN";
                                grinInventoryTranctionDto.TO_Location = "GRIN";
                                grinInventoryTranctionDto.ReferenceIDFrom = "KIT_GRIN";
                                grinInventoryTranctionDto.GrinMaterialType = "KIT_GRIN";
                                grinInventoryTranctionDto.Remarks = "KIT_GRIN";
                            }
                            grinInventoryTranctionDto.GrinNo = grins.KIT_GrinNumber;
                            grinInventoryTranctionDto.GrinPartId = parts.Id;
                            grinInventoryTranctionDto.PartType = KC.PartType;
                            grinInventoryTranctionDto.ReferenceID = grins.KIT_GrinNumber;
                            grinInventoryTranctionDto.shopOrderNo = "";
                            grinInventoryTranctionDto.IsStockAvailable = true;

                            var json1q = JsonConvert.SerializeObject(grinInventoryTranctionDto);
                            var data1q = new StringContent(json1q, Encoding.UTF8, "application/json");
                            var client2q = _clientFactory.CreateClient();
                            var token2q = HttpContext.Request.Headers["Authorization"].ToString();
                            var request2q = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                            "CreateInventoryTranctionFromGrin"))
                            {
                                Content = data1q
                            };
                            request2q.Headers.Add("Authorization", token2q);

                            var response1q = await client2q.SendAsync(request2q);
                            if (response1q.StatusCode != HttpStatusCode.OK)
                            {
                                createinvTrancResp = response1q.StatusCode;
                            }
                        }
                    }
                }

                List<KIT_GRIN_POUpdate> kIT_GRIN_POUpdates = new List<KIT_GRIN_POUpdate>();
                var PoNos = grins.KIT_GRINParts.Select(x => x.PONumber).Distinct().ToList();
                foreach (var po in PoNos)
                {
                    KIT_GRIN_POUpdate kIT_GRIN_POUpdate = new KIT_GRIN_POUpdate();
                    kIT_GRIN_POUpdate.PONumber = po;
                    kIT_GRIN_POUpdate.POItems = new List<KIT_GRIN_POItemsUpdate>();
                    foreach (var parts in grins.KIT_GRINParts.Where(x => x.PONumber == po).ToList())
                    {
                        KIT_GRIN_POItemsUpdate kIT_GRIN_POItemsUpdate = new KIT_GRIN_POItemsUpdate();
                        kIT_GRIN_POItemsUpdate.ItemNumber = parts.ItemNumber;
                        kIT_GRIN_POItemsUpdate.Qty = parts.Qty;
                        kIT_GRIN_POItemsUpdate.POProjects = new List<KIT_GRIN_POProjectUpdate>();
                        foreach (var prj in parts.KIT_GRIN_ProjectNumbers)
                        {
                            KIT_GRIN_POProjectUpdate kIT_GRIN_POProjectUpdate = new KIT_GRIN_POProjectUpdate()
                            {
                                ProjectNumber = prj.ProjectNumber,
                                ProjectQty = prj.ProjectQty
                            };
                            kIT_GRIN_POProjectUpdate.POComponents = new List<KIT_GRIN_POComponentsUpdate>();
                            foreach (var compo in prj.KIT_GRIN_KITComponents)
                            {
                                KIT_GRIN_POComponentsUpdate kIT_GRIN_POComponentsUpdate = new KIT_GRIN_POComponentsUpdate()
                                {
                                    PartNumber = compo.PartNumber,
                                    KitComponentQty = compo.KitComponentQty
                                };
                                kIT_GRIN_POProjectUpdate.POComponents.Add(kIT_GRIN_POComponentsUpdate);
                            }
                            kIT_GRIN_POItemsUpdate.POProjects.Add(kIT_GRIN_POProjectUpdate);
                        }
                        kIT_GRIN_POUpdate.POItems.Add(kIT_GRIN_POItemsUpdate);
                    }
                    kIT_GRIN_POUpdates.Add(kIT_GRIN_POUpdate);
                }

                var jsonsw = JsonConvert.SerializeObject(kIT_GRIN_POUpdates);
                var data1w = new StringContent(jsonsw, Encoding.UTF8, "application/json");
                var client2w = _clientFactory.CreateClient();
                var token2w = HttpContext.Request.Headers["Authorization"].ToString();
                var request2w = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["PurchaseAPI"],
                "UpdateKIT_PODetails"))
                {
                    Content = data1w
                };
                request2w.Headers.Add("Authorization", token2w);

                var responsesw = await client2w.SendAsync(request2w);
                if (responsesw.StatusCode != HttpStatusCode.OK)
                {
                    UpdatePoQty = responsesw.StatusCode;
                }

                if (getItemmResp != HttpStatusCode.OK && UpdatePoQty != HttpStatusCode.OK && createinvTrancResp != HttpStatusCode.OK && createinvResp != HttpStatusCode.OK)
                {
                    _logger.LogError($"Something went wrong inside Create CreateKITGRIN action: Other Service Calling");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Saving Failed";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }

                serviceResponse.Data = null;
                serviceResponse.Message = "KITGRIN Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in CreateKITGRIN action:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in CreateKITGRIN action:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllKIT_GrinNumberForKIT_IQC()
        {
            ServiceResponse<IEnumerable<KIT_GrinNoForKIT_IqcAndKIT_Binning>> serviceResponse = new ServiceResponse<IEnumerable<KIT_GrinNoForKIT_IqcAndKIT_Binning>>();
            try
            {
                var result = await _repository.GetAllKIT_GrinNumberForKIT_IQC();
                _logger.LogInfo("Returned all GetAllKIT_GrinNumberForKIT_IQC");
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GetAllKIT_GrinNumberForKIT_IQC";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllKIT_GrinNumberForKIT_IQC action:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllKIT_GrinNumberForKIT_IQC action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetKIT_GrinById(int id)
        {
            ServiceResponse<KIT_GRINDto> serviceResponse = new ServiceResponse<KIT_GRINDto>();
            try
            {
                var GrinDetailsbyId = await _repository.GetKIT_GrinById(id);
                if (GrinDetailsbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"KIT_Grin with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"KIT_Grin with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returning GetKIT_GrinById with id: {id}");
                    var clientz = _clientFactory.CreateClient();
                    var tokenz = HttpContext.Request.Headers["Authorization"].ToString();
                    var jsonz = JsonConvert.SerializeObject(GrinDetailsbyId.KIT_GRINParts.Select(x => x.ItemNumber).ToList());
                    var dataz = new StringContent(jsonz, Encoding.UTF8, "application/json");
                    var requestz = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterEnggAPI"],
                        $"GetItemDetailsByItemNumberList"))
                    {
                        Content = dataz
                    };
                    requestz.Headers.Add("Authorization", tokenz);
                    var itemMasterObjectResultz = await clientz.SendAsync(requestz);
                    if (itemMasterObjectResultz.StatusCode != HttpStatusCode.OK) throw new Exception("Data not Found in GetItemDetailsByItemNumberList");
                    var itemString1z = await itemMasterObjectResultz.Content.ReadAsStringAsync();
                    var ItemStringdetails1z = JsonConvert.DeserializeObject<ItemMasterDto>(itemString1z);
                    var PartsObject = ItemStringdetails1z.data;

                    var grin = _mapper.Map<KIT_GRINDto>(GrinDetailsbyId);
                    grin.KIT_GRINParts.ForEach(t =>
                    {
                        var match = PartsObject.FirstOrDefault(x => x.ItemNumber == t.ItemNumber);
                        t.DrawingNo = match?.DrawingNo;
                        t.DocRet = match?.DocRet;
                        t.RevNo = match?.RevNo;
                        t.IsCocRequired = match?.IsCocRequired ?? false;
                        t.IsRohsItem = match?.IsRohsItem ?? false;
                        t.IsShelfLife = match?.IsShelfLife ?? false;
                        t.IsReachItem = match?.IsReachItem ?? false;
                    });

                    serviceResponse.Data = grin;
                    serviceResponse.Message = $"Returned GetKIT_GrinById with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetKIT_GrinById action:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in GetKIT_GrinById action:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateKIT_GRINFileUpload([FromBody] List<DocumentUploadPostDto> fileUploadPostDtos)
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                if (fileUploadPostDtos is null)
                {
                    _logger.LogError("CreateKIT_GRINFileUpload object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CreateKIT_GRINFileUpload object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid CreateKIT_GRINFileUpload object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid CreateKIT_GRINFileUpload object sent from client.";
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
                            ParentId = "KIT_GRINFile",
                            DocumentFrom = "KIT_GRINFile Document",
                            FileByte = FileUploadDetail.FileByte
                        };
                        await _documentUploadRepository.CreateUploadDocumentGrin(uploadedFile);
                        _documentUploadRepository.SaveAsync();
                        id_s.Add(uploadedFile.Id.ToString());
                    }
                }
                serviceResponse.Data = id_s;
                serviceResponse.Message = "CreateKIT_GRINFileUpload Successfull";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateKIT_GRINFileUpload action: \n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateKIT_GRINFileUpload action: \n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDownloadUrlDetailsforKIT_GRINFiles(string fileids)
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
                    serviceResponse.Message = "Invalid KIT_GRIN UploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid KIT_GRIN UploadDocument sent from client.");
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
                            fileUploadDto.DownloadUrl = $"{baseUrl}/apigateway/tips/KIT_GRIN/DownloadFile?Filename={fileUploadDto.FileName}";
                        }
                        else
                        {
                            var baseUrl = $"{_config["GrinUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/api/KIT_GRIN/DownloadFile?Filename={fileUploadDto.FileName}";
                        }
                        fileUploads.Add(fileUploadDto);
                    }
                }
                _logger.LogInfo($"Returned GetDownloadUrlDetailsforKIT_GRINFiles for ids: {fileids} successfully");
                serviceResponse.Data = fileUploads;
                serviceResponse.Message = $"Returned GetDownloadUrlDetailsforKIT_GRINFiles for ids: {fileids} successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetDownloadUrlDetailsforKIT_GRINFiles:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in GetDownloadUrlDetailsforKIT_GRINFiles:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<ActionResult> DownloadFile(string Filename)
        {
            ServiceResponse<FileContentResult> serviceResponse = new ServiceResponse<FileContentResult>();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "GrinDocument", Filename);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var ContentType))
            {
                ContentType = "application/octet-stream";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(bytes, ContentType, Path.GetFileName(filePath));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllKIT_GRIN([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)

        {
            ServiceResponse<IEnumerable<KIT_GRINPostDto>> serviceResponse = new ServiceResponse<IEnumerable<KIT_GRINPostDto>>();
            try
            {
                var GetallGrins = await _repository.GetAllKIT_GRIN(pagingParameter, searchParams);
                if (GetallGrins == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"No KIT_GRIN data not found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"No KIT_GRIN data not found in db");
                    return NotFound(serviceResponse);
                }
                var metadata = new
                {
                    GetallGrins.TotalCount,
                    GetallGrins.PageSize,
                    GetallGrins.CurrentPage,
                    GetallGrins.HasNext,
                    GetallGrins.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all KIT_GRIN Successfully");
                var result = _mapper.Map<IEnumerable<KIT_GRINPostDto>>(GetallGrins);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all KIT_GRIN Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetAllKIT_GRIN:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in GetAllKIT_GRIN:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKIT_GRIN(int id, [FromBody] KIT_GRINUpdateDto kIT_GRINUpdateDto)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                if (kIT_GRINUpdateDto is null)
                {
                    _logger.LogError("UpdateKIT_GRIN object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "UpdateKIT_GRIN object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid UpdateKIT_GRIN object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid UpdateKIT_GRIN object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updategrin = await _repository.GetKIT_GrinById(id);
                if (updategrin is null)
                {
                    _logger.LogError($"UpdateKIT_GRIN with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"UpdateKIT_GRIN with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var data = _mapper.Map(kIT_GRINUpdateDto, updategrin);
                decimal? othercosttotal = 1;
                if ((data.Freight + data.Insurance + data.LoadingorUnLoading + data.Transport) > 1) othercosttotal = data.Freight + data.Insurance + data.LoadingorUnLoading + data.Transport;
                decimal? conversionrate = 1;
                if (data.CurrencyConversion > 1) conversionrate = data.CurrencyConversion;
                foreach (var part in data.KIT_GRINParts)
                {
                    foreach (var prj in part.KIT_GRIN_ProjectNumbers)
                    {
                        var kit_comp = _mapper.Map<List<KIT_GRIN_KITComponentscalculationofAvgcost>>(prj.KIT_GRIN_KITComponents);
                        List<KIT_GRIN_KITComponents> KITComponentsList = new List<KIT_GRIN_KITComponents>();
                        foreach (var KC in kit_comp)
                        {
                            decimal? EP = KC.KitComponentQty * KC.KitComponentUnitPrice;
                            //decimal? Itemwithtax = gPart.SGST + gPart.IGST + gPart.CGST + gPart.UTGST + gPart.Duties;
                            //*if (Itemwithtax == null || Itemwithtax == 0) gPart.EPwithTax = EP * conversionrate;
                            //else gPart.EPwithTax = (EP + (EP * (Itemwithtax / 100))) * conversionrate;
                            KC.EPwithTax = EP * conversionrate;
                            KC.EPforSingleQty = KC.EPwithTax / KC.KitComponentQty;
                        }
                        decimal? SumofEPwithtax = kit_comp.Sum(x => x.EPwithTax);
                        foreach (var KC in kit_comp)
                        {
                            decimal? distriduteOthercostforitem = (KC.EPwithTax / SumofEPwithtax) * othercosttotal;
                            decimal? distriduteOthercostforitemsSingleQty = distriduteOthercostforitem / KC.KitComponentQty;
                            KC.AverageCost = distriduteOthercostforitemsSingleQty + KC.EPforSingleQty;
                            if (KC.AverageCost == null) KC.AverageCost = 0;
                            KIT_GRIN_KITComponents kITComponents = _mapper.Map<KIT_GRIN_KITComponents>(KC);
                            KITComponentsList.Add(kITComponents);
                        }
                        prj.KIT_GRIN_KITComponents = KITComponentsList;
                    }
                }
                await _repository.UpdateKIT_GRIN(data);
                _repository.SaveAsync();
                _logger.LogInfo("UpdateKIT_GRIN Updated Successfully");
                serviceResponse.Data = null;
                serviceResponse.Message = "UpdateKIT_GRIN Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in UpdateKIT_GRIN:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in UpdateKIT_GRIN:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllKIT_GrinNumberForKIT_Binning()
        {
            ServiceResponse<IEnumerable<KIT_GrinNoForKIT_IqcAndKIT_Binning>> serviceResponse = new ServiceResponse<IEnumerable<KIT_GrinNoForKIT_IqcAndKIT_Binning>>();
            try
            {
                var grinNoForBinning = await _repository.GetAllKIT_GrinNumberForKIT_Binning();
                var result = _mapper.Map<IEnumerable<KIT_GrinNoForKIT_IqcAndKIT_Binning>>(grinNoForBinning);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GetAllKIT_GrinNumberForKIT_Binning";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetAllKIT_GrinNumberForKIT_Binning:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in GetAllKIT_GrinNumberForKIT_Binning:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetKIT_GRINDetailsForKIT_IQC([FromQuery] string KIT_GrinNumber)
        {
            ServiceResponse<KIT_GRINDto> serviceResponse = new ServiceResponse<KIT_GRINDto>();
            try
            {
                var GrinDetailsbyId = await _repository.GetKIT_GrinByKIT_GrinNumber(KIT_GrinNumber);
                if (GrinDetailsbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"KIT_Grin with KIT_GrinNumber:{KIT_GrinNumber} hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"KIT_Grin with KIT_GrinNumber:{KIT_GrinNumber} hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                GrinDetailsbyId.KIT_GRINParts.RemoveAll(x=>x.AcceptedQty+x.RejectedQty==x.Qty);
                GrinDetailsbyId.KIT_GRINParts.ForEach(a => a.KIT_GRIN_ProjectNumbers.RemoveAll(x => x.AcceptedQty + x.RejectedQty == x.ProjectQty));
                var clientz = _clientFactory.CreateClient();
                var tokenz = HttpContext.Request.Headers["Authorization"].ToString();
                var jsonz = JsonConvert.SerializeObject(GrinDetailsbyId.KIT_GRINParts.Select(x => x.ItemNumber).ToList());
                var dataz = new StringContent(jsonz, Encoding.UTF8, "application/json");
                var requestz = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterEnggAPI"],
                    $"GetItemDetailsByItemNumberList"))
                {
                    Content = dataz
                };
                requestz.Headers.Add("Authorization", tokenz);
                var itemMasterObjectResultz = await clientz.SendAsync(requestz);
                if (itemMasterObjectResultz.StatusCode != HttpStatusCode.OK) throw new Exception("Data not Found in GetItemDetailsByItemNumberList");
                var itemString1z = await itemMasterObjectResultz.Content.ReadAsStringAsync();
                var ItemStringdetails1z = JsonConvert.DeserializeObject<ItemMasterDto>(itemString1z);
                var PartsObject = ItemStringdetails1z.data;

                var grin = _mapper.Map<KIT_GRINDto>(GrinDetailsbyId);
                grin.KIT_GRINParts.ForEach(t =>
                {
                    var match = PartsObject.FirstOrDefault(x => x.ItemNumber == t.ItemNumber);
                    t.DrawingNo = match?.DrawingNo;
                    t.DocRet = match?.DocRet;
                    t.RevNo = match?.RevNo;
                    t.IsCocRequired = match?.IsCocRequired ?? false;
                    t.IsRohsItem = match?.IsRohsItem ?? false;
                    t.IsShelfLife = match?.IsShelfLife ?? false;
                    t.IsReachItem = match?.IsReachItem ?? false;
                });

                serviceResponse.Data = grin;
                serviceResponse.Message = $"Returned GetKIT_GRINDetailsForKIT_IQC for KIT_GrinNumber:{KIT_GrinNumber}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetKIT_GRINDetailsForKIT_IQC for KIT_GrinNumber:{KIT_GrinNumber}:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in GetKIT_GRINDetailsForKIT_IQC action for KIT_GrinNumber:{KIT_GrinNumber}:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetKIT_GRINDetailsForKIT_Binning([FromQuery] string KIT_GrinNumber)
        {
            ServiceResponse<KIT_GRINDto> serviceResponse = new ServiceResponse<KIT_GRINDto>();
            try
            {
                var GrinDetailsbyId = await _repository.GetKIT_GrinByKIT_GrinNumber(KIT_GrinNumber);
                if (GrinDetailsbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"KIT_Grin with KIT_GrinNumber:{KIT_GrinNumber} hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"KIT_Grin with KIT_GrinNumber:{KIT_GrinNumber} hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                GrinDetailsbyId.KIT_GRINParts.RemoveAll(x => x.AcceptedQty != x.BinnedQty);
                GrinDetailsbyId.KIT_GRINParts.ForEach(a => a.KIT_GRIN_ProjectNumbers.RemoveAll(x => x.AcceptedQty != x.BinnedQty));
                var clientz = _clientFactory.CreateClient();
                var tokenz = HttpContext.Request.Headers["Authorization"].ToString();
                var jsonz = JsonConvert.SerializeObject(GrinDetailsbyId.KIT_GRINParts.Select(x => x.ItemNumber).ToList());
                var dataz = new StringContent(jsonz, Encoding.UTF8, "application/json");
                var requestz = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterEnggAPI"],
                    $"GetItemDetailsByItemNumberList"))
                {
                    Content = dataz
                };
                requestz.Headers.Add("Authorization", tokenz);
                var itemMasterObjectResultz = await clientz.SendAsync(requestz);
                if (itemMasterObjectResultz.StatusCode != HttpStatusCode.OK) throw new Exception("Data not Found in GetItemDetailsByItemNumberList");
                var itemString1z = await itemMasterObjectResultz.Content.ReadAsStringAsync();
                var ItemStringdetails1z = JsonConvert.DeserializeObject<ItemMasterDto>(itemString1z);
                var PartsObject = ItemStringdetails1z.data;

                var grin = _mapper.Map<KIT_GRINDto>(GrinDetailsbyId);
                grin.KIT_GRINParts.ForEach(t =>
                {
                    var match = PartsObject.FirstOrDefault(x => x.ItemNumber == t.ItemNumber);
                    t.DrawingNo = match?.DrawingNo;
                    t.DocRet = match?.DocRet;
                    t.RevNo = match?.RevNo;
                    t.IsCocRequired = match?.IsCocRequired ?? false;
                    t.IsRohsItem = match?.IsRohsItem ?? false;
                    t.IsShelfLife = match?.IsShelfLife ?? false;
                    t.IsReachItem = match?.IsReachItem ?? false;
                });

                serviceResponse.Data = grin;
                serviceResponse.Message = $"Returned GetKIT_GRINDetailsForKIT_Binning for KIT_GrinNumber:{KIT_GrinNumber}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetKIT_GRINDetailsForKIT_Binning for KIT_GrinNumber:{KIT_GrinNumber}:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in GetKIT_GRINDetailsForKIT_Binning action for KIT_GrinNumber:{KIT_GrinNumber}:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
