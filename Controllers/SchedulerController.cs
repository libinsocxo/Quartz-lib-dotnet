using LinkedinSchedulers.Infrastructure;
using LinkedinSchedulers.Models;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using System.Text.Json;

namespace LinkedinSchedulers.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class SchedulerController: ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<SchedulerController> _logger;


        public SchedulerController(ISchedulerFactory schedulerFactory, ILogger<SchedulerController> logger)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        [HttpPost("schedule")]
        public async Task<IActionResult> ScheduleJob([FromBody] ScheduleRequest request)
        {

            //Console.WriteLine(request.DateTime.ToUniversalTime());
            //Console.WriteLine(DateTime.UtcNow);
            DateTime requestDateTimeUtc = request.DateTime.ToUniversalTime();

            if (requestDateTimeUtc <= DateTime.UtcNow)
            {
                return BadRequest("Scheduled time must be in the future.");
            }

            var scheduler = await _schedulerFactory.GetScheduler();

            var jsonData = JsonSerializer.Serialize(request);

        




            var jobDetail = JobBuilder.Create<HttpPostJob>()
                .WithIdentity(Guid.NewGuid().ToString())
                .UsingJobData("data", jsonData)
                .Build();

        

            var trigger = TriggerBuilder.Create()
                .WithIdentity(Guid.NewGuid().ToString())
                .StartAt(request.DateTime.ToUniversalTime())
                .Build();

            await scheduler.ScheduleJob(jobDetail, trigger);

            return Ok("Job scheduled.");
        }

    }

}
