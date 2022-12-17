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
builder.Services.AddHostedService<MessageBrokerWorker>();

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


app.Run();
