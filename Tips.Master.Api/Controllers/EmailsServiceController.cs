using Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

using MimeKit.Text;
using System.Net.Mail;
using System.Net;
using Entities.DTOs;
using AutoMapper;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmailsServiceController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapperForMaster _repository;
        private IMapper _mapper;
        public EmailsServiceController(ILoggerManager logger, IRepositoryWrapperForMaster repository, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _repository=repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmailTemplatebyProcessType(string ProcessType)
        {
            ServiceResponse<EmailTemplateDto> serviceResponse = new ServiceResponse<EmailTemplateDto>();
            try
            {
                if (ProcessType is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ProcessType object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ProcessType object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ProcessType object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid ProcessType object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var emailTemplate = await _repository.EmailTemplateRepository.GetEmailTemplatebyProcessType(ProcessType);
                var result=_mapper.Map<EmailTemplateDto>(emailTemplate);
                serviceResponse.Data = result;
                serviceResponse.Message = $"Template Return for {ProcessType}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEmailTemplatebyProcessType  action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SendMailToCustomer()
        {
                ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                // var name = sendMailDto.CustomerName;

                var httpclientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(httpclientHandler);



                var mails = "it@avisionsystems.com";


                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("erp@avisionsystems.com"));
                // email.To.Add(MailboxAddress.Parse(sendMailDto.EmailID));
                email.To.Add(MailboxAddress.Parse(mails));
                email.Subject = "Testing Mail";
                email.Body = new TextPart(TextFormat.Html) { Text = "<h1>Testing mail</h1>" };

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.Connect("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("erp@avisionsystems.com", "R#9183753474150W");

                smtp.Send(email);
                smtp.Disconnect(true);
                // var dataFound = true;
                // Exit the loop since valid data is found




                return Ok();
            }

            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SendMailToCustomer action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //using (var message = new MailMessage())
        //{
        //    message.To.Add(new MailAddress("it@avisionsystems.com", "Avision"));
        //    message.From = new MailAddress("erp@avisionsystems.com", "Test ERP");
        //    message.Subject = "Testing Mail";
        //    message.Body = "Testing mail";
        //    message.IsBodyHtml = false; // change to true if body msg is in html

        //    using (var client = new System.Net.Mail.SmtpClient("smtp.office365.com"))
        //    {
        //        client.UseDefaultCredentials = false;
        //        client.Port = 587;
        //        client.Credentials = new NetworkCredential("erp@avisionsystems.com", "R#9183753474150W");
        //        client.EnableSsl = true;

        //        try
        //        {
        //            await client.SendMailAsync(message); // Email sent
        //            return Ok();
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError($"Something went wrong inside GetCustomerDetailsByCustomerName action: {ex.Message}");
        //                 serviceResponse.Data = null;
        //            serviceResponse.Message = "Internal server error";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //            return StatusCode(500, serviceResponse);
        //        }
        //    }
        //}
    }
    
}
