using FluentAssertions;
using rabbitmq_dotnet.Consumers;
using tests.Fixtures;

namespace tests.DummyTests
{
    [Collection("RabbitmqCollection")]
    public class RabbitMqTest2
    {
        private readonly RabbitMqFixture _rabbitMqFixture;

        public RabbitMqTest2(RabbitMqFixture rabbitMqFixture)
        {
            _rabbitMqFixture = rabbitMqFixture;
        }

        [Fact]
        public async Task test2()
        {
            var fileName = "test2.json";
            await _rabbitMqFixture.Publish(new StreamUploadEvent
            {
                FileName = fileName,
                EventTime = DateTime.UtcNow
            }, queueName: $"{nameof(StreamUploadEvent)}Sample");

            await _rabbitMqFixture.SubscribeAndGetAsync<StreamUploadEvent>($"{nameof(StreamUploadEvent)}Sample", 
                onMessageReceived: (@event) =>
            {
                @event.FileName.Should().Be(fileName);
                @event.EventTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            });
        }
    }
}
