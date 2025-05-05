using Hotfix.LanMgr;
using Microsoft.Extensions.DependencyInjection;
using ScriptEngine.BuildIn.ShareCode.Manager.LanSystem.Abstractions;

namespace CommonCode.ProtobufGamePlayExtensions
{
    public static class LanExtensions
    {
        public static string ToLan(this int lanId)
        {
            if (lanId == 0) return "";
            var lanMgr = DIContext.ServiceProvider.GetRequiredService<ILanManager>();
            return lanMgr.GetText(lanId);
        }
    }
}