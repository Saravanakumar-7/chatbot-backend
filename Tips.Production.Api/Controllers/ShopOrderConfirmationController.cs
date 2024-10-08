using System.Net;
using System.Text;
using AutoMapper;
using Contracts;
using Entities;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ShopOrderConfirmationController : ControllerBase
    {
        private IShopOrderConfirmationRepository _shopOrderConfirmationRepository;
        private IShopOrderRepository _shopOrderRepo;
        private ILoggerManager _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private IMapper _mapper;
        private readonly IHttpClientFactory _clientFactory;

        public ShopOrderConfirmationController(IShopOrderConfirmationRepository shopOrderConfirmationRepository, IHttpClientFactory clientFactory,
            ILoggerManager logger, IMapper mapper, IShopOrderRepository shopOrderRepository, IConfiguration config, HttpClient httpClient)
        {
            _logger = logger;
            _shopOrderConfirmationRepository = shopOrderConfirmationRepository;
            _shopOrderRepo = shopOrderRepository;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShopOrderConfirmation([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<ShopOrderConfirmationDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderConfirmationDto>>();

            try
            {
                var shopOrderConfirmationDetails = await _shopOrderConfirmationRepository.GetAllShopOrderConfirmation(pagingParameter, searchParamess);

                var metadata = new
                {
                    shopOrderConfirmationDetails.TotalCount,
                    shopOrderConfirmationDetails.PageSize,
                    shopOrderConfirmationDetails.CurrentPage,
                    shopOrderConfirmationDetails.HasNext,
                    shopOrderConfirmationDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all ShopOrderConfirmationdetails");
                var result = _mapper.Map<IEnumerable<ShopOrderConfirmationDto>>(shopOrderConfirmationDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "ShopOrderConfirmation Successfully Returned";
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShopOrderConfirmationById(int id)
        {
            ServiceResponse<ShopOrderConfirmationDto> serviceResponse = new ServiceResponse<ShopOrderConfirmationDto>();

            try
            {
                var shopOrderConfirmationDetailById = await _shopOrderConfirmationRepository.GetShopOrderConfirmationById(id);
                if (shopOrderConfirmationDetailById == null)
                {
                    _logger.LogError($"ShopOrderConfirmationdetails with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrderConfirmationdetails with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrderConfirmationdetails with id: {id}");
                    var result = _mapper.Map<ShopOrderConfirmationDto>(shopOrderConfirmationDetailById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderConfirmationById Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetshopOrderConfirmationById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
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
        public async Task<IActionResult> CreateShopOrderConfirmation([FromBody] ShopOrderConfirmationPostDto shopOrderConfirmationPostDto)

        {
            ServiceResponse<ShopOrderConfirmationPostDto> serviceResponse = new ServiceResponse<ShopOrderConfirmationPostDto>();

            try
            {
                if (shopOrderConfirmationPostDto == null)
                {
                    _logger.LogError("ShopOrderConfirmationdetails object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ShopOrderConfirmationdetails object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ShopOrderConfirmationdetails object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ShopOrderConfirmationdetail object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var shopOrderConfirmation = _mapper.Map<ShopOrderConfirmation>(shopOrderConfirmationPostDto);
                var shopOrderNumber = shopOrderConfirmation.ShopOrderNumber;
                var shopOrderDetail = await _shopOrderRepo.GetShopOrderDetailsByShopOrderNo(shopOrderNumber);
                shopOrderDetail.WipQty = shopOrderDetail.WipQty + shopOrderConfirmation.WipConfirmedQty;
                if (shopOrderDetail.TotalSOReleaseQty == shopOrderDetail.WipQty) shopOrderDetail.ShopOrderConfirmationStatus = ShopOrderConformationStatus.FullyDone;
                if (shopOrderDetail.TotalSOReleaseQty > shopOrderDetail.WipQty) shopOrderDetail.ShopOrderConfirmationStatus = ShopOrderConformationStatus.PartiallyDone;
                await _shopOrderRepo.UpdateShopOrder(shopOrderDetail);
                await _shopOrderConfirmationRepository.CreateShopOrderConfirmation(shopOrderConfirmation);

                //shopOrderConfirmationPostDto.shopOrderItemConfirmations[0].WipConfirmedQty = shopOrderConfirmation.WipConfirmedQty;

                //update Inventory Code                          

                var json = JsonConvert.SerializeObject(shopOrderConfirmationPostDto.shopOrderItemConfirmations);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "UpdateInventoryOnShopOrderConfirmation"), data);

                var client1 = _clientFactory.CreateClient();
                var token1 = HttpContext.Request.Headers["Authorization"].ToString();
                var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                "UpdateInventoryOnShopOrderConfirmation"))
                {
                    Content = data
                };
                request1.Headers.Add("Authorization", token1);

                var response = await client1.SendAsync(request1);

                //Update InventoryTranction Code 

                //var jsons = JsonConvert.SerializeObject(shopOrderConfirmationPostDto.shopOrderItemConfirmations);
                //var datas = new StringContent(jsons, Encoding.UTF8, "application/json");
                //var result = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "UpdateInventoryTranctionOnShopOrderConfirmation"), datas);

                //var inventoryResponceString = await response.Content.ReadAsStringAsync();
                //dynamic inventoryResponceData = JsonConvert.DeserializeObject(inventoryResponceString);
                //dynamic inventoryObject = inventoryResponceData.data;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _shopOrderRepo.SaveAsync();
                    _shopOrderConfirmationRepository.SaveAsync();
                    string serverKey = GetServerKey();
                    if (serverKey == "avision")
                    {
                        _logger.LogInfo($"Avision ShopOrderConfirmation Email Creation Process");
                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();
                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"], "GetEmailTemplatebyProcessType?ProcessType=CreateShopOrderConfirmation"));
                        request.Headers.Add("Authorization", token);
                        var response1 = await client.SendAsync(request);
                        _logger.LogInfo($"GetEmailTemplatebyProcessType is doing");
                        if (response1.StatusCode != HttpStatusCode.OK)
                            _logger.LogError($"Something went wrong inside GetEmailTemplatebyProcessType During Email action");
                        var EmailTempString = await response1.Content.ReadAsStringAsync();
                        var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                        var Operations = "From,CreateShopOrderConfirmation";
                        var request2 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                        request2.Headers.Add("Authorization", token);
                        var response2 = await client.SendAsync(request2);
                        _logger.LogInfo($"GetEmailIdDetailsbyOperation is doing");
                        if (response2.StatusCode != HttpStatusCode.OK)
                            _logger.LogError($"Something went wrong inside GetEmailIdDetailsbyOperation During Email action");
                        var EmailTempString1 = await response2.Content.ReadAsStringAsync();
                        var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);
                        var httpclientHandler = new HttpClientHandler();
                        var httpClient = new HttpClient(httpclientHandler);
                        var mails = (emaildetails1.data.Where(x => x.operation == "CreateShopOrderConfirmation").Select(x => x.emailIds).FirstOrDefault()).Split(',');
                        var email = new MimeMessage();
                        email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));

                        email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));

                        email.Subject = emaildetails.data.subject;
                        string body = emaildetails.data.template;

                        body = body.Replace("{{ShopOrderNo}}", shopOrderDetail.ShopOrderNumber);
                        List<string>? SalesorderList = new List<string>();
                        string? salesorderNos = null;
                        foreach (var item in shopOrderDetail.ShopOrderItems)
                        {
                            if (salesorderNos == null)
                            {
                                salesorderNos = item.SalesOrderNumber;
                                SalesorderList.Add(item.SalesOrderNumber);
                            }
                            else
                            {
                                if (!SalesorderList.Contains(item.SalesOrderNumber))
                                {
                                    salesorderNos = salesorderNos + "," + item.SalesOrderNumber;
                                    SalesorderList.Add(item.SalesOrderNumber);
                                }
                            }
                        }

                        body = body.Replace("{{SalesOrderNo}}", salesorderNos);
                        body = body.Replace("{{ConfirmedBy}}", shopOrderConfirmation.CreatedBy);
                        body = body.Replace("{{ProjectNo}}", shopOrderDetail.ShopOrderItems[0].ProjectNumber);
                        body = body.Replace("{{Sl.No}}", "1");
                        body = body.Replace("{{ItemNumbers}}", shopOrderDetail.ItemNumber);
                        body = body.Replace("{{ItemDesc}}", shopOrderDetail.Description);
                        body = body.Replace("{{RevNo}}", shopOrderDetail.BomRevisionNo.ToString());
                        body = body.Replace("{{ProductQty}}", shopOrderDetail.WipQty.ToString());
                        body = body.Replace("{{WIPQty}}",(shopOrderDetail.TotalSOReleaseQty - shopOrderDetail.WipQty).ToString());
                        var ItemNumber = shopOrderDetail.ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);
                        var request3 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request3.Headers.Add("Authorization", token);
                        var itemMasterObjectResult = await client.SendAsync(request3);
                        if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                            _logger.LogError($"Something went wrong inside GetItemMasterByItemNumber During Email action");
                        var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                        dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                        dynamic itemMasterObject = itemMasterObjectData.data;
                        string uom = itemMasterObject.uom;
                        body = body.Replace("{{UOM}}", uom);

                        email.Body = new TextPart(TextFormat.Html) { Text = body };
                        _logger.LogInfo($"SmtpClient is doing");

                        using var smtp = new MailKit.Net.Smtp.SmtpClient();
                        int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                        smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                        smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                        smtp.Send(email);
                        smtp.Disconnect(true);

                    }

                }
                else
                {
                    _logger.LogError($"Something went wrong inside CreateShopOrderConfirmation inside http inventory action UpdateInventoryOnShopOrderConfirmation action");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Internal server error";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }


                serviceResponse.Data = null;
                serviceResponse.Message = "ShopOrderConfirmation Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetShopOrderItemById", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateShopOrderConfirmation action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShopOrderConfirmation(int id, [FromBody] ShopOrderConfirmationUpdateDto shopOrderConfirmationUpdateDto)
        {
            ServiceResponse<ShopOrderConfirmationUpdateDto> serviceResponse = new ServiceResponse<ShopOrderConfirmationUpdateDto>();

            try
            {
                if (shopOrderConfirmationUpdateDto is null)
                {
                    _logger.LogError("Update ShopOrderConfirmationdetails object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ShopOrderConfirmationdetails object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ShopOrderConfirmationdetails object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ShopOrderConfirmationdetail object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var shopOrderConfirmationDetailById = await _shopOrderConfirmationRepository.GetShopOrderConfirmationById(id);
                if (shopOrderConfirmationDetailById is null)
                {
                    _logger.LogError($"ShopOrderConfirmationdetails with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }

                var shopOrderConfirmation = _mapper.Map(shopOrderConfirmationUpdateDto, shopOrderConfirmationDetailById);

                string result = await _shopOrderConfirmationRepository.UpdateShopOrderConfirmation(shopOrderConfirmation);
                _logger.LogInfo(result);
                _shopOrderConfirmationRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ShopOrderConfirmation Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateShopOrderConfirmation action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShopOrderConfirmation(int id)
        {
            ServiceResponse<ShopOrderConfirmationDto> serviceResponse = new ServiceResponse<ShopOrderConfirmationDto>();

            try
            {
                var shopOrderConfirmationDetailById = await _shopOrderConfirmationRepository.GetShopOrderConfirmationById(id);
                if (shopOrderConfirmationDetailById == null)
                {
                    _logger.LogError($"ShopOrderConfirmation with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrderConfirmation with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                shopOrderConfirmationDetailById.IsDeleted = true;
                string result = await _shopOrderConfirmationRepository.UpdateShopOrderConfirmation(shopOrderConfirmationDetailById);
                _shopOrderConfirmationRepository.SaveAsync();
                serviceResponse.Message = "ShopOrderConfirmation Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteShopOrderConfirmation action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{shopOrderNo}")]
        public async Task<IActionResult> GetAllShopOrderConfirmationByShopOrderNo(string shopOrderNo)
        {
            ServiceResponse<IEnumerable<ShopOrderConfirmationDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderConfirmationDto>>();

            try
            {
                var shopOrderConfirmationByShopOrderNo = await _shopOrderConfirmationRepository.GetAllShopOrderConfirmationByShopOrderNo(shopOrderNo);
                if (shopOrderConfirmationByShopOrderNo == null)
                {
                    _logger.LogError($"ShopOrderConfirmationdetails with ShopOrderNo: {shopOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrderConfirmationdetails with ShopOrderNo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrderConfirmationdetails with ShopOrderNo: {shopOrderNo}");
                    var result = _mapper.Map<IEnumerable<ShopOrderConfirmationDto>>(shopOrderConfirmationByShopOrderNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned all ShopOrderConfirmationByShopOrderNo Successfully ";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllShopOrderConfirmationByShopOrderNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{shopOrderNo}")]
        public async Task<IActionResult> GetOpenDataForOqcByShopOrderNo(string shopOrderNo)
        {
            ServiceResponse<IEnumerable<ShopOrderConfirmationDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderConfirmationDto>>();

            try
            {
                var openDataForOqcByShopOrderNo = await _shopOrderConfirmationRepository.GetOpenDataForOqcByShopOrderNo(shopOrderNo);
                if (openDataForOqcByShopOrderNo == null)
                {
                    _logger.LogError($"Oqc with shopOrderNo: {shopOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Oqc with shopOrderNo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    // shopOrderConfirmationList.IsOQCDone = False;
                    _logger.LogInfo($"Returned Oqc with shopOrderNo: {shopOrderNo}");
                    var result = _mapper.Map<IEnumerable<ShopOrderConfirmationDto>>(openDataForOqcByShopOrderNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "OpenDataForOqcByShopOrderNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetOpenDataForOqcByShopOrderNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetShopOrderItemNoByFGItemType()
        {
            ServiceResponse<IEnumerable<ShopOrderItemNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderItemNoListDto>>();

            try
            {
                var shopOrderItemNoList = await _shopOrderConfirmationRepository.GetShopOrderItemNoByFGItemType();
                if (shopOrderItemNoList == null)
                {
                    _logger.LogError($"FGItemType hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"FGItemType hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned ShopOrderItemNo By FGItemType");
                    var result = _mapper.Map<IEnumerable<ShopOrderItemNoListDto>>(shopOrderItemNoList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderItemNoList By FGItemType  Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShopOrderItemNoByFGItemType action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetShopOrderItemNoBySAItemType()
        {
            ServiceResponse<IEnumerable<ShopOrderItemNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderItemNoListDto>>();

            try
            {
                var shopOrderItemNoList = await _shopOrderConfirmationRepository.GetShopOrderItemNoBySAItemType();
                if (shopOrderItemNoList == null)
                {
                    _logger.LogError($"SAItemType hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAItemType hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned ShopOrderItemNo By SAItemType");
                    var result = _mapper.Map<IEnumerable<ShopOrderItemNoListDto>>(shopOrderItemNoList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderItemNoList By SAItemType  Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShopOrderItemNoBySAItemType action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetShopOrderFGSAItemTypeDetailsByItemNo(string itemNumber)
        {
            ServiceResponse<IEnumerable<ShopOrderDetailsDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderDetailsDto>>();

            try
            {
                var shopOrderDetails = await _shopOrderConfirmationRepository.GetShopOrderDetailsByItemNo(itemNumber);
                if (shopOrderDetails == null)
                {
                    _logger.LogError($"ShopOrderItemNo hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrderItemNo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned ShopOrderDetails By ItemNo");
                    var result = _mapper.Map<IEnumerable<ShopOrderDetailsDto>>(shopOrderDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderDetails ByItemNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShopOrderFGSAItemTypeDetailsByItemNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetShopOrderConformationDetailsByItemNo(string itemNumber)
        {
            ServiceResponse<IEnumerable<ShopOrderDetailsDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderDetailsDto>>();

            try
            {
                var shopOrderDetails = await _shopOrderConfirmationRepository.GetShopOrderConformationDetailsByItemNo(itemNumber);
                if (shopOrderDetails == null)
                {
                    _logger.LogError($"ShopOrderItemNo hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrderItemNo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned ShopOrderDetails By ItemNo");
                    var result = _mapper.Map<IEnumerable<ShopOrderDetailsDto>>(shopOrderDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderDetails ByItemNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShopOrderConformationDetailsByItemNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }

}
