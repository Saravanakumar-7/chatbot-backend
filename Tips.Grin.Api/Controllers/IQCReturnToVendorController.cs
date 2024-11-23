using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Text;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class IQCReturnToVendorController : ControllerBase
    {
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IGrinRepository _grinrepository;
        private IIQCConfirmationRepository _iQCConfirmationRepository;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        public IQCReturnToVendorController(ILoggerManager logger, IMapper mapper, IConfiguration config, IGrinRepository grinRepository, IHttpClientFactory clientFactory,
            IIQCConfirmationRepository iQCConfirmationRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _config = config;
            _grinrepository = grinRepository;
            _iQCConfirmationRepository = iQCConfirmationRepository;
            _clientFactory = clientFactory;
        }
        //[HttpPost]
        //public async Task<IActionResult> CreateIQCReturnToVendor([FromBody] IQCReturnToVendorPostDto iQCRejectRecoveryPostDto)
        //{
        //    ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
        //    try
        //    {
        //        if (iQCRejectRecoveryPostDto == null)
        //        {
        //            _logger.LogError("IQCRejectRecovery details object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "IQCConfirmation details object is null";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid IQCRejectRecovery details object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid model object";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        var Grindetails = await _grinrepository.GetGrinById(iQCRejectRecoveryPostDto.GrinId);
        //        if (Grindetails == null)
        //        {
        //            _logger.LogError($"GrinId: {iQCRejectRecoveryPostDto.GrinId} Does not Exist");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"GrinId: {iQCRejectRecoveryPostDto.GrinId} Does not Exist";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(serviceResponse);
        //        }
        //        var Iqcdetails = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(iQCRejectRecoveryPostDto.GrinNumber);
        //        if (Iqcdetails == null)
        //        {
        //            _logger.LogError($"IQC for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} Does not Exist");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"IQC for Grin Number: {iQCRejectRecoveryPostDto.GrinNumber} Does not Exist";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(serviceResponse);
        //        }



        //        var client1 = _clientFactory.CreateClient();
        //        var token1 = HttpContext.Request.Headers["Authorization"].ToString();
        //        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["WarehouseService"],
        //        $"Inventory/GetRejectInventorybyGrinNo?GrinNo={iQCRejectRecoveryPostDto.GrinNumber}"));
        //        request1.Headers.Add("Authorization", token1);
        //        var inventoryObjectResult = await client1.SendAsync(request1);
        //        if (inventoryObjectResult.StatusCode != HttpStatusCode.OK)
        //        {
        //            _logger.LogError($"No Rejects Found in Inventory for the Grin Number: {iQCRejectRecoveryPostDto.GrinNumber}");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"No Rejects Found in Inventory for the Grin Number: {iQCRejectRecoveryPostDto.GrinNumber}";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(serviceResponse);
        //        }
        //        var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
        //        var inventoryObjectData = JsonConvert.DeserializeObject<InventoryDtoDetails>(inventoryObjectString);
        //        var inventoryObject = inventoryObjectData.data;

        //        List<PurchaseOrderReturns> purchaseOrderReturns = new List<PurchaseOrderReturns>();

        //        foreach (var items in iQCRejectRecoveryPostDto.iQCReturnToVendorItemsPostDtos)
        //        {

        //            var exPO = purchaseOrderReturns.Where(q => q.PurchaseOrderNo == items.PONumber).First();
        //            if (exPO == null)
        //            {
        //                PurchaseOrderReturns purchaseOrderReturns1 = new PurchaseOrderReturns();
        //                purchaseOrderReturns1.PurchaseOrderNo = items.PONumber;
        //                PurchaseOrderItems purchaseOrderItems = new PurchaseOrderItems();
        //                purchaseOrderItems.ItemNumber = items.ItemNumber;
        //                purchaseOrderItems.ReturnQty = items.ReturnQty;
        //                List<PurchaseOrderProject> purchaseOrderProjects = new List<PurchaseOrderProject>();
        //                foreach (var pro in items.iQCReturnToVendorItemsProjectPosts)
        //                {
        //                    var exPro = purchaseOrderProjects.Where(w => w.ProjectNumber == pro.ProjectNumber).First();
        //                    if (exPro == null)
        //                    {
        //                        PurchaseOrderProject purchaseOrderProject = new PurchaseOrderProject();
        //                        purchaseOrderProject.ProjectNumber = pro.ProjectNumber;
        //                        purchaseOrderProject.ReturnQty = pro.ReturnQty;
        //                        purchaseOrderProjects.Add(purchaseOrderProject);
        //                    }
        //                    else
        //                    {
                                
        //                    }
        //                }
        //            }


        //        }
        //        //var PoList = iQCRejectRecoveryPostDto.iQCReturnToVendorItemsPostDtos.Select(x => x.PONumber).Distinct().ToList();
        //        //var serialPoList = JsonConvert.SerializeObject(PoList);
        //        //var data = new StringContent(serialPoList, Encoding.UTF8, "application/json");
        //        //var request2 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["PurchaseService"],
        //        // $"PurchaseOrder/GetLatestPurchaseOrdersByPONumbers"))
        //        //{
        //        //    Content= data
        //        //};
        //        //request2.Headers.Add("Authorization", token1);
        //        //var responce = await client1.SendAsync(request2);
        //        //if (responce.StatusCode != HttpStatusCode.OK)
        //        //{
        //        //    _logger.LogError($"Some PurchaseOrders were not found");
        //        //    serviceResponse.Data = null;
        //        //    serviceResponse.Message = $"Some PurchaseOrders were not found";
        //        //    serviceResponse.Success = false;
        //        //    serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //        //    return NotFound(serviceResponse);
        //        //}                
        //        //var PODetailsObjectString = await responce.Content.ReadAsStringAsync();
        //        //var PODetailsObjectData = JsonConvert.DeserializeObject<PurchaseOrderDtoDetails>(PODetailsObjectString);
        //        //var PODetailsObject = PODetailsObjectData.data;

        //        //List<PurchaseOrderDetails> POsUpdated = new List<PurchaseOrderDetails>();
        //        foreach (var item in iQCRejectRecoveryPostDto.iQCReturnToVendorItemsPostDtos)
        //        {
        //            var invitem = inventoryObject.Where(x => x.PartNumber == item.ItemNumber).ToList();

        //            //var Po= PODetailsObject.Where(x=>x.PONumber== item.PONumber).First();
        //            //var PoItem= Po.POItems.Where(x=>x.ItemNumber==item.ItemNumber).First();


        //            foreach (var project in item.iQCReturnToVendorItemsProjectPosts)
        //            {
        //                var invitemProject = invitem.Where(y => y.ProjectNumber == project.ProjectNumber).ToList();
        //                var removingQty = project.ReturnQty.Value;
        //                foreach (var prjInv in invitemProject)
        //                {
        //                    if (prjInv.Balance_Quantity > removingQty)
        //                    {
        //                        prjInv.Balance_Quantity -= removingQty;
        //                        removingQty = 0;
        //                    }
        //                    else if (prjInv.Balance_Quantity <= removingQty)
        //                    {
        //                        removingQty -= prjInv.Balance_Quantity;
        //                        prjInv.Balance_Quantity = 0;
        //                        prjInv.IsStockAvailable = false;
        //                    }
        //                    var updateInv = _mapper.Map<InventoryUpdateDto>(prjInv);
        //                    var json = JsonConvert.SerializeObject(updateInv);
        //                    var data = new StringContent(json, Encoding.UTF8, "application/json");
        //                    var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["WarehouseService"],
        //                    "Inventory/UpdateInventory?id=", prjInv.Id))
        //                    {
        //                        Content = data
        //                    };
        //                    request5.Headers.Add("Authorization", token1);

        //                    var response = await client1.SendAsync(request5);

        //                    IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
        //                    iqcInventoryTranctionDtos.PartNumber = prjInv.PartNumber;
        //                    iqcInventoryTranctionDtos.LotNumber = prjInv.LotNumber;
        //                    iqcInventoryTranctionDtos.MftrPartNumber = prjInv.MftrPartNumber;
        //                    iqcInventoryTranctionDtos.Description = prjInv.Description;
        //                    iqcInventoryTranctionDtos.ProjectNumber = prjInv.ProjectNumber;
        //                    iqcInventoryTranctionDtos.Issued_Quantity = prjInv.Balance_Quantity;
        //                    iqcInventoryTranctionDtos.UOM = prjInv.UOM;
        //                    iqcInventoryTranctionDtos.Warehouse = prjInv.Warehouse;
        //                    iqcInventoryTranctionDtos.From_Location = prjInv.Location;
        //                    iqcInventoryTranctionDtos.TO_Location = prjInv.Location;
        //                    iqcInventoryTranctionDtos.GrinNo = prjInv.GrinNo; ;
        //                    iqcInventoryTranctionDtos.GrinPartId = prjInv.GrinPartId;
        //                    iqcInventoryTranctionDtos.PartType = prjInv.PartType;
        //                    iqcInventoryTranctionDtos.ReferenceID = prjInv.GrinNo;
        //                    iqcInventoryTranctionDtos.ReferenceIDFrom = "IQCRejectRecovery";
        //                    iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
        //                    iqcInventoryTranctionDtos.ShopOrderNo = "";
        //                    string iqcInventoryTranctionDtoJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDtos);
        //                    var contents1 = new StringContent(iqcInventoryTranctionDtoJsons, Encoding.UTF8, "application/json");
        //                    var request8 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["WarehouseService"],
        //                    "InventoryTranction/CreateInventoryTranction"))
        //                    {
        //                        Content = contents1
        //                    };
        //                    request8.Headers.Add("Authorization", token1);

        //                    var inventoryTransResponses1 = await client1.SendAsync(request8);

        //                    if (removingQty == 0) break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message + "\n" + ex.InnerException);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong,try again";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
    }
}
