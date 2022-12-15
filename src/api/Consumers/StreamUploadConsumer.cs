using rabbitmq_dotnet.Configuration;
using rabbitmq_dotnet_infrastructure;

namespace rabbitmq_dotnet.Consumers
{
    public class StreamUploadConsumer: IConsumer<StreamUploadEvent>
    {
        public void Consume(StreamUploadEvent message)
        {
            Console.WriteLine(message.FileName);
        }
    }

    public class StreamUploadEvent
    {
        public string FileName { get; set; }
        public DateTime EventTime { get; set; }
    }
}
