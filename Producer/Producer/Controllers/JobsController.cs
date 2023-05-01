using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Producer.Dto;
using Producer.RabbitMQService;

namespace Producer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobSearchService _jobSearchService;

        public JobsController(IJobSearchService jobSearchService)
        {
            _jobSearchService = jobSearchService;
        }
        [HttpPost]
        public async Task<IActionResult> SearchJob([FromBody] JobQueryRequest request)
        {
            _jobSearchService.PublishMessage(request);
            return Ok();
        }
    }
}
