using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using rabbitmq_dotnet.Consumers;
using rabbitmq_dotnet_infrastructure;

namespace tests.Fixtures
{
    public class RabbitMqFixture : IDisposable
    {
        private MessageBrokerConfiguration _config;
        private RabbitMqTestcontainer _rabbitMqTestcontainer;

        public RabbitMqFixture()
        {
            _config = new MessageBrokerConfiguration
            {
               ConnectionString = "amqp://guest:guest@host.docker.internal:5672/"
            };
            CreateContainer().Wait();
        }

        public async Task CreateContainer()
        {
            var rabbitMqTestcontainer = await InitContainerTest();
            _rabbitMqTestcontainer = rabbitMqTestcontainer;
            await rabbitMqTestcontainer.StartAsync();
        }

        public void CreateQueuesAndExchanges<TEvent>(string queueName)
        {
            using var conn = _config.GetConnection();
            using var channel = conn.CreateModel();

            var consumerManager = new ConsumerManager();
            consumerManager.RegisterQueueAndExchange<TEvent>(channel, queueName);
        }

        public async Task SubscribeAndGetAsync<TEvent>(string queueName, Action<TEvent?> onMessageReceived)
        {
            using var conn = _config.GetConnection();
            using var channel = conn.CreateModel();
            var consumerManager = new ConsumerManager();
            consumerManager.RegisterAndConsumeOneMessage<TEvent>(channel,
                (@event) =>
                {
                    CreateQueuesAndExchanges<TEvent>(queueName);
                    onMessageReceived.Invoke(@event);
                    channel.ExchangeDelete(typeof(TEvent).Name, false);
                    channel.QueueDelete(queueName, false, false);
                },
                queueName);
        }

        public async Task Publish<T>(T message, string queueName = null, string exchangeName = null)
        {
            if (queueName != null)
                CreateQueuesAndExchanges<T>(queueName);
            var publisher = new Publisher(_config);
            await publisher.Publish(message, exchangeName);
        }

        public async Task<RabbitMqTestcontainer> InitContainerTest()
        {
            var testcontainersBuilder = new TestcontainersBuilder<RabbitMqTestcontainer>()
                .WithImage("rabbitmq:3-management")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
                .WithPortBinding(5672);

            return testcontainersBuilder.Build();
        }
        public void Dispose()
        {
            _rabbitMqTestcontainer.StopAsync();
            _rabbitMqTestcontainer.DisposeAsync();
        }
    }
}
