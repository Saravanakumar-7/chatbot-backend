using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
//using NuGet.Protocol.Core.Types;
using System.Net;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Repository;
using Microsoft.EntityFrameworkCore;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Net.Http;
using System.Data;
using System.Data.SqlClient;
using Google.Protobuf.WellKnownTypes;


namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConsumptionReportController : ControllerBase
    {
        private IInventoryRepository _inventoryRepository;
        private IMapper _mapper;
        private ILoggerManager _logger;
        private object _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public ConsumptionReportController(IInventoryRepository inventoryRepository, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        { 
            _mapper = mapper;
            _inventoryRepository = inventoryRepository;
            _logger = logger;
            _httpClient = httpClient;
            _config = config;
        }



    }
}
