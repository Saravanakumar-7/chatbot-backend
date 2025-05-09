using AutoMapper;
using Azure;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
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
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IDocumentUploadRepository _documentUploadRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        public KIT_GRINController(IKIT_GRINRepository repository, IKIT_GRINPartsRepository kIT_GRINPartsRepository, IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor, IDocumentUploadRepository documentUploadRepository, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _repository = repository;
            _kIT_GRINPartsRepository = kIT_GRINPartsRepository;
            _logger = logger;
            _mapper = mapper;
            _documentUploadRepository = documentUploadRepository;
            _httpClient = httpClient;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
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

        //[HttpPost]
        //public async Task<IActionResult> CreateKITGRIN([FromBody] KIT_GRINPostDto kIT_GRINPostDto)
        //{
        //    ServiceResponse<string> serviceResponse = new ServiceResponse<string>();

        //    try
        //    {
        //        string serverKey = GetServerKey();

        //        if (kIT_GRINPostDto is null)
        //        {
        //            _logger.LogError("KIT_Grin object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "KIT_Grin object sent from client is null.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid KIT_Grin object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid KIT_Grin object sent from client.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }

        //        var grins = _mapper.Map<KIT_GRIN>(kIT_GRINPostDto);
        //        var date = DateTime.Now;
        //        var days = Convert.ToString(date.Day.ToString("D2"));
        //        var months = Convert.ToString(date.Month.ToString("D2"));
        //        var years = Convert.ToString(date.ToString("yy"));
        //        HttpStatusCode invStatusCode = HttpStatusCode.OK;
        //        if (serverKey == "avision")
        //        {
        //            var grinNum = await _repository.GenerateKIT_GrinNumberForAvision();
        //            grins.KIT_GrinNumber = grinNum;
        //        }
        //        else
        //        {
        //            var dateFormat = days + months + years;
        //            var grinNumber = await _repository.GenerateKIT_GrinNumber();
        //            var grinNo = dateFormat + grinNumber;
        //            grins.KIT_GrinNumber = grinNo;
        //        }
        //        foreach (var part in grins.KIT_GRINParts)
        //        {
        //            foreach (var prj in part.KIT_GRIN_ProjectNumbers)
        //            {
        //                var kit_comp = _mapper.Map<List<KIT_GRIN_KITComponentscalculationofAvgcost>>(prj.KIT_GRIN_KITComponents);
        //                List<KIT_GRIN_KITComponents> KITComponentsList = new List<KIT_GRIN_KITComponents>();

        //                decimal? othercosttotal = 1;
        //                if ((grins.Freight + grins.Insurance + grins.LoadingorUnLoading + grins.Transport) > 1) othercosttotal = grins.Freight + grins.Insurance + grins.LoadingorUnLoading + grins.Transport;
        //                decimal? conversionrate = 1;
        //                if (grins.CurrencyConversion > 1) conversionrate = grins.CurrencyConversion;
        //                foreach (var KC in kit_comp)
        //                {
        //                    decimal? EP = KC.KitComponentQty * KC.KitComponentUnitPrice;
        //                    //decimal? Itemwithtax = gPart.SGST + gPart.IGST + gPart.CGST + gPart.UTGST + gPart.Duties;
        //                    //*if (Itemwithtax == null || Itemwithtax == 0) gPart.EPwithTax = EP * conversionrate;
        //                    //else gPart.EPwithTax = (EP + (EP * (Itemwithtax / 100))) * conversionrate;
        //                    KC.EPwithTax = EP * conversionrate;
        //                    KC.EPforSingleQty = KC.EPwithTax / KC.KitComponentQty;
        //                }
        //                decimal? SumofEPwithtax = kit_comp.Sum(x => x.EPwithTax);
        //                foreach (var KC in kit_comp)
        //                {
        //                    decimal? distriduteOthercostforitem = (KC.EPwithTax / SumofEPwithtax) * othercosttotal;
        //                    decimal? distriduteOthercostforitemsSingleQty = distriduteOthercostforitem / KC.KitComponentQty;
        //                    KC.AverageCost = distriduteOthercostforitemsSingleQty + KC.EPforSingleQty;
        //                    if (KC.AverageCost == null) KC.AverageCost = 0;
        //                    KIT_GRIN_KITComponents kITComponents = _mapper.Map<KIT_GRIN_KITComponents>(KC);
        //                    KITComponentsList.Add(kITComponents);
        //                }
        //                prj.KIT_GRIN_KITComponents = KITComponentsList;
        //            }
        //        }
        //        grins.IsKIT_GrinCompleted = true;
        //        await _repository.CreateKIT_Grin(grins);
        //        _repository.SaveAsync();
        //        if (grins.KIT_GRINParts != null)
        //        {
        //            foreach (var grinPart in grins.KIT_GRINParts)
        //            {
        //                var grinPartsId = await _kIT_GRINPartsRepository.GetKIT_GRINPartsById(grinPart.Id);
        //                //grinPartsId.LotNumber = grins.KIT_GrinNumber + grinPartsId.Id;
        //                foreach (var prj in grinPartsId.KIT_GRIN_ProjectNumbers)
        //                {
        //                    foreach (var KC in prj.KIT_GRIN_KITComponents)
        //                    {
        //                    KC.LotNumber= grins.KIT_GrinNumber + grinPartsId.Id +"Compo"+KC.Id;
        //                    }
        //                }
        //                    await _kIT_GRINPartsRepository.UpdateKIT_GRINPartsQty(grinPartsId);
        //            }
        //        }
        //        HttpStatusCode createinvResp = HttpStatusCode.OK;
        //        HttpStatusCode createinvTrancResp = HttpStatusCode.OK;
        //        HttpStatusCode getItemmResp = HttpStatusCode.OK;
        //        HttpStatusCode getItemForIqcResp = HttpStatusCode.OK;
        //        HttpStatusCode UpdatePoStatus = HttpStatusCode.OK;
        //        HttpStatusCode UpdatePoQty = HttpStatusCode.OK;
        //        HttpStatusCode UpdatePoProjQty = HttpStatusCode.OK;

        //        foreach (var parts in grins.KIT_GRINParts)
        //        {
        //            foreach (var prj in parts.KIT_GRIN_ProjectNumbers)
        //            {
        //                var client = _clientFactory.CreateClient();
        //                var token = HttpContext.Request.Headers["Authorization"].ToString();
        //                var json = JsonConvert.SerializeObject(prj.KIT_GRIN_KITComponents.Select(x=>x.PartNumber).ToList());
        //                var data = new StringContent(json, Encoding.UTF8, "application/json");
        //                var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterEnggAPI"],
        //                    $"GetItemDetailsByItemNumberList"))
        //                {
        //                    Content = data
        //                };
        //                request.Headers.Add("Authorization", token);
        //                var itemMasterObjectResult = await client.SendAsync(request);
        //                if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK) getItemmResp = itemMasterObjectResult.StatusCode;
        //                var itemString1 = await itemMasterObjectResult.Content.ReadAsStringAsync();
        //                var ItemStringdetails1 = JsonConvert.DeserializeObject<ItemMasterDto>(itemString1);
        //                var itemMasterObject = ItemStringdetails1.data;
        //                foreach (var KC in prj.KIT_GRIN_KITComponents)
        //                {
        //                    GrinInventoryDto grinInventoryDto = new GrinInventoryDto();
        //                    grinInventoryDto.PartNumber = KC.PartNumber;
        //                    grinInventoryDto.LotNumber = KC.LotNumber;
        //                    grinInventoryDto.MftrPartNumber = KC.MftrItemNumbers;
        //                    grinInventoryDto.Description = KC.ItemDescription;
        //                    grinInventoryDto.ProjectNumber = prj.ProjectNumber;
        //                    grinInventoryDto.Balance_Quantity = Convert.ToDecimal(KC.ProjectQty);
        //                    grinInventoryDto.Max = itemMasterObject.Where(x=>x.ItemNumber== KC.PartNumber).Select(x=>x.Max).First();
        //                    grinInventoryDto.Min = itemMasterObject.Where(x => x.ItemNumber == KC.PartNumber).Select(x => x.Min).First();
        //                    grinInventoryDto.UOM = parts.UOM;
        //                    grinInventoryDto.Warehouse = "GRIN";
        //                    grinInventoryDto.Location = "GRIN";
        //                    grinInventoryDto.GrinNo = grins.GrinNumber;
        //                    grinInventoryDto.GrinPartId = parts.Id;
        //                    grinInventoryDto.PartType = parts.ItemType;
        //                    grinInventoryDto.ReferenceID = grins.GrinNumber;
        //                    grinInventoryDto.ReferenceIDFrom = "GRIN";
        //                    grinInventoryDto.GrinMaterialType = "";
        //                    grinInventoryDto.ShopOrderNo = "";


        //                    var json = JsonConvert.SerializeObject(grinInventoryDto);
        //                    var data = new StringContent(json, Encoding.UTF8, "application/json");
        //                    var client1 = _clientFactory.CreateClient();
        //                    var token1 = HttpContext.Request.Headers["Authorization"].ToString();
        //                    var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
        //                    "CreateInventoryFromGrin"))
        //                    {
        //                        Content = data
        //                    };
        //                    request1.Headers.Add("Authorization", token1);

        //                    var response = await client1.SendAsync(request1);
        //                    if (response.StatusCode != HttpStatusCode.OK)
        //                    {
        //                        createinvResp = response.StatusCode;
        //                    }
        //                }
        //            }
                    
                    
        //        }

        //        foreach (var parts in grinPartsList)
        //        {
        //            if (parts.ProjectNumbers != null)
        //            {
        //                foreach (var project in parts.ProjectNumbers)
        //                {
        //                    grinInventoryTrasactionPostDto grinInventoryTranctionDto = new grinInventoryTrasactionPostDto();
        //                    grinInventoryTranctionDto.PartNumber = parts.ItemNumber;
        //                    grinInventoryTranctionDto.LotNumber = parts.LotNumber;
        //                    grinInventoryTranctionDto.MftrPartNumber = parts.MftrItemNumber;
        //                    grinInventoryTranctionDto.Description = parts.ItemDescription;
        //                    grinInventoryTranctionDto.ProjectNumber = project.ProjectNumber;
        //                    grinInventoryTranctionDto.Issued_Quantity = Convert.ToDecimal(project.ProjectQty);
        //                    grinInventoryTranctionDto.Issued_DateTime = DateTime.Now;
        //                    grinInventoryTranctionDto.Issued_By = _createdBy;
        //                    grinInventoryTranctionDto.UOM = parts.UOM;
        //                    grinInventoryTranctionDto.Warehouse = "GRIN";
        //                    grinInventoryTranctionDto.From_Location = "GRIN";
        //                    grinInventoryTranctionDto.TO_Location = "GRIN";
        //                    grinInventoryTranctionDto.GrinNo = grins.GrinNumber;
        //                    grinInventoryTranctionDto.GrinPartId = parts.Id;
        //                    grinInventoryTranctionDto.PartType = parts.ItemType;
        //                    grinInventoryTranctionDto.ReferenceID = grins.GrinNumber;
        //                    grinInventoryTranctionDto.ReferenceIDFrom = "GRIN";
        //                    grinInventoryTranctionDto.GrinMaterialType = "GRIN";
        //                    grinInventoryTranctionDto.shopOrderNo = "";
        //                    grinInventoryTranctionDto.IsStockAvailable = true;
        //                    grinInventoryTranctionDto.Remarks = "GRIN";

        //                    var json = JsonConvert.SerializeObject(grinInventoryTranctionDto);
        //                    var data = new StringContent(json, Encoding.UTF8, "application/json");
        //                    var client1 = _clientFactory.CreateClient();
        //                    var token1 = HttpContext.Request.Headers["Authorization"].ToString();
        //                    var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
        //                    "CreateInventoryTranctionFromGrin"))
        //                    {
        //                        Content = data
        //                    };
        //                    request1.Headers.Add("Authorization", token1);

        //                    var response = await client1.SendAsync(request1);
        //                    if (response.StatusCode != HttpStatusCode.OK)
        //                    {
        //                        createinvTrancResp = response.StatusCode;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside CreateGrin action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
    }
}
