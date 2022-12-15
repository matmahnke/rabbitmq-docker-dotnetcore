using Microsoft.AspNetCore.Mvc;
using rabbitmq_dotnet.Consumers;
using rabbitmq_dotnet_infrastructure;

namespace rabbitmq_dotnet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly IPublisher _publisher;

        public SampleController(IPublisher publisher)
        {
            _publisher = publisher;
        }

        [HttpGet]
        public void PublishMessage(string message)
        {
            _publisher.Publish(new StreamUploadEvent
            {
                EventTime = DateTime.Now,
                FileName = message
            });
        }
    }
}
