
using ScriptHotfix.GamePlay.Share.Manager.ExampleSystem;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddSingleton<HelloWorldManager>();//这个项目仅演示如何使用前端定义的代码。实际生产中，所有Manager的生命周期应当与用户登陆/登出周期一致，而不是如示例代码这样作为单例注入
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<HelloWorldService.HelloWorldService>();
app.MapGrpcReflectionService();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();