using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class FG_Weighted_AvgCostController : ControllerBase
    {
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IRepositoryWrapperForMaster _repository;
        public FG_Weighted_AvgCostController(ILoggerManager logger, IMapper mapper, IRepositoryWrapperForMaster repository)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }
        

    }
}
