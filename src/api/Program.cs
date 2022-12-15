using rabbitmq_dotnet.Configuration;
using rabbitmq_dotnet.Consumers;
using rabbitmq_dotnet_infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<MessageBrokerConfiguration>();
builder.Services.AddSingleton<ConsumerManager>();
builder.Services.AddSingleton<IPublisher, Publisher>();
builder.Services.AddScoped<IConsumer<StreamUploadEvent>, StreamUploadConsumer>();
//builder.Services.AddScoped<IServiceProvider>(x => builder.Services.BuildServiceProvider());
builder.Services.AddHostedService<MessageBrokerWorker>();
//builder.Services.AddHostedService<Worker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();




//var factory = new ConnectionFactory()
//{
//    HostName = "localhost", // o endereço do servidor RabbitMQ
//    UserName = "guest", // o nome de usuário do servidor RabbitMQ
//    Password = "guest" // a senha do servidor RabbitMQ
//};



//using (var connection = factory.CreateConnection())
//{
//    using (var channel = connection.CreateModel())
//    {
//        channel.QueueDeclare(queue: "uploaded_file_queue",
//                             durable: true,
//                             exclusive: false,
//                             autoDelete: false,
//                             arguments: null);

//        var consumer = new EventingBasicConsumer(channel);
//        consumer.Received += (model, ea) =>
//        {
//            var body = ea.Body.ToArray();
//            var message = Encoding.UTF8.GetString(body);

//            new UploadFileConsumer().Consume(message);
//        };

//        channel.BasicConsume(queue: "uploaded_file_queue", // o nome da fila
//                     autoAck: true, // se as mensagens devem ser automaticamente confirmadas pelo consumidor
//                     consumer: consumer); // o consumidor que irá processar as mensagens

//    }
//}



app.Run();
