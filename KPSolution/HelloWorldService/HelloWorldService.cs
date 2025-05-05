using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using HelloWorld;
using NexCore.DI;
using ScriptHotfix.GamePlay.Share.Manager.ExampleSystem;

namespace HelloWorldService;
[Service]
public class HelloWorldService(HelloWorldManager helloWorldManager,ILogger<HelloWorldService> logger):HelloWorld.HelloWorld.HelloWorldBase
{
    public override async Task<GetInitDataRet> GetInitData(Empty request, ServerCallContext context)
    {
        logger.LogInformation("GetInitData");
        var data = helloWorldManager.CreatInitData();
        helloWorldManager.Init(data);
        var ret = new GetInitDataRet();
        logger.LogInformation((ret.InitData == null).ToString());
        ret.InitData.AddRange(data);
        return ret;
    }

    public override async Task<GetRandomRet> GetRandom(Empty request, ServerCallContext context)
    {
        var next = helloWorldManager.GetNext();
        logger.LogInformation("GetRandom:{num}",next);
        return new GetRandomRet()
        {
            RandomVal = next
        };
    }
}