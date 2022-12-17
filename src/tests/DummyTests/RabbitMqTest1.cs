using FluentAssertions;
using rabbitmq_dotnet.Consumers;
using tests.Fixtures;

namespace tests.DummyTests
{
    [Collection("RabbitmqCollection")]
    public class RabbitMqTest1
    {
        private readonly RabbitMqFixture _rabbitMqFixture;

        public RabbitMqTest1(RabbitMqFixture rabbitMqFixture)
        {
            _rabbitMqFixture = rabbitMqFixture;
        }

        [Fact]
        public async Task test1()
        {
            var queueName = $"{nameof(StreamUploadEvent)}Sample";
            var fileName = "test1.json";
            await _rabbitMqFixture.Publish(new StreamUploadEvent
            {
                FileName = fileName,
                EventTime = DateTime.UtcNow
            }, queueName);

            await _rabbitMqFixture.SubscribeAndGetAsync<StreamUploadEvent>(
                queueName,
                onMessageReceived: (@event) =>
                {
                    @event.FileName.Should().Be(fileName);
                    @event.EventTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
                });
        }
    }
}
