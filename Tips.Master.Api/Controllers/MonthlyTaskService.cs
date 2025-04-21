using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MonthlyTaskService : ControllerBase //BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private ILoggerManager _logger;
        public MonthlyTaskService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        var now = DateTime.Now;
        //        var nextRun = new DateTime(now.Year, now.Month, 1, 1, 0, 0, DateTimeKind.Local);

        //        if (now > nextRun)
        //        {
        //            nextRun = nextRun.AddMonths(1);
        //        }

        //        var delay = nextRun - now;
        //        await Task.Delay(delay, stoppingToken);

        //        if (!stoppingToken.IsCancellationRequested)
        //        {
        //            await PerformTaskAsync(stoppingToken);
        //        }
        //    }
        //}
        [HttpPost]
        public async Task<IActionResult> PerformTaskAsync()//CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var weightedAvgCostTask = scope.ServiceProvider.GetRequiredService<I_SA_Weighted_AvgCostTask>();
                    var FGweightedAvgCostTask = scope.ServiceProvider.GetRequiredService<I_FG_Weighted_AvgCostTask>();

                    await weightedAvgCostTask.Calculate_SA_Weighted_AvgCost();
                    await Task.Delay(100);
                    await FGweightedAvgCostTask.Calculate_FG_Weighted_AvgCost();
                    return StatusCode(200, "Successfull");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error Occured in MonthlyTaskService: " + ex.Message + $": \n {ex.InnerException}");
                    return StatusCode(500, $"Error Occured in MonthlyTaskService: " + ex.Message + $": \n {ex.InnerException}");
                }
            }
        }
    }
}