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
            var fileName = "test1.json";
            _rabbitMqFixture.CreateQueuesAndExchanges();
            await _rabbitMqFixture.Publish(new StreamUploadEvent
            {
                FileName = fileName,
                EventTime = DateTime.UtcNow
            });

            await _rabbitMqFixture.SubscribeAndGetAsync<StreamUploadEvent>(onMessageReceived: (@event) =>
            {
                @event.FileName.Should().Be(fileName);
                @event.EventTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            });
        }
    }
}
