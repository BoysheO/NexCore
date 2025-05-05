// See https://aka.ms/new-console-template for more information

using Grpc.Net.Client;
using SimpleGrpcServiceViaWebSocket;

Console.WriteLine("Hello, World!");

using var channel = GrpcChannel.ForAddress("http://localhost:5161"); 
var client = new Greeter.GreeterClient(channel);
await client.SayHelloAsync(new HelloRequest()
{
    Name = "XiaoMing"
});
