using rabbitmq_dotnet.Consumers;
using rabbitmq_dotnet_infrastructure;

namespace rabbitmq_dotnet.Configuration
{
    public class MessageBrokerWorker : BackgroundService
    {
        private readonly MessageBrokerConfiguration _messageBrokerConfiguration;
        private readonly ConsumerManager _consumerManager;
        private readonly IServiceProvider _serviceProvider;
        private const int WORKER_INTERVAL = 200;
        public MessageBrokerWorker(MessageBrokerConfiguration messageBrokerConfiguration, ConsumerManager consumerManager, IServiceProvider serviceProvider)
        {
            _messageBrokerConfiguration = messageBrokerConfiguration;
            _consumerManager = consumerManager;
            _serviceProvider = serviceProvider;

            CreateQueuesAndExchanges();
        }

        private void CreateQueuesAndExchanges()
        {
            using var conn = _messageBrokerConfiguration.GetConnection();
            using var channel = conn.CreateModel();

            _consumerManager.RegisterQueueAndExchange<StreamUploadEvent>(channel, $"{nameof(StreamUploadConsumer)}Sample");
        }

        // TODO: Otimizar codigo nesse metodo
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var conn = _messageBrokerConfiguration.GetConnection();
            using var channel = conn.CreateModel();

            var streamUploadConsumer = new StreamUploadConsumer();

            _consumerManager.RegisterConsumer<StreamUploadEvent, StreamUploadConsumer>(channel, streamUploadConsumer, $"{nameof(StreamUploadConsumer)}Sample");


            var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(WORKER_INTERVAL));
            while (await timer.WaitForNextTickAsync(stoppingToken)) // && !stoppingToken.IsCancellationRequested
            {

            }
        }
    }
}
