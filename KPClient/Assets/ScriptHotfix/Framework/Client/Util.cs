using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace HotScripts.Hotfix.FrameworkSystems
{
    public static class Util
    {
        public static async UniTask<string> GetIpAsync()
        {
            UnityWebRequest request = UnityWebRequest.Get("https://ip.900cha.com/");
            var res = await request.SendWebRequest();
            var tex = res.downloadHandler.text;
            //从返回的网页中查找显示的ip
            var regex = Regex.Match(tex, @"(?<=我的IP:\s*)\d+.\d+.\d+.\d+(?=\s*\n)");
            if (regex.Success) return regex.Value;
            return "";
        }

        /// <summary>
        /// 判断app是否在大陆内
        /// *将自己的ip发往服务器，由服务器判断？
        /// </summary>
        public static async UniTask<bool> IsInMainLan(string ip)
        {
            UnityWebRequest request = UnityWebRequest.Get($"https://ipapi.co/{ip}/json");
            // * 返回结果应类似如下
            // * {
            // *     "ip": "127.0.0.1",
            // *     "error": true,
            // *     "reason": "Reserved IP Address",
            // *     "reserved": true,
            // *     "version": "IPv4"
            // * }
            // * or
            // * {
            // *     "ip": "103.43.162.151",
            // *     "network": "103.43.160.0/22",
            // *     "version": "IPv4",
            // *     "city": "Ha Kwai Chung",
            // *     "region": "Kwai Tsing",
            // *     "region_code": "NKT",
            // *     "country": "HK",
            // *     "country_name": "Hong Kong",
            // *     "country_code": "HK",
            // *     "country_code_iso3": "HKG",
            // *     "country_capital": "Hong Kong",
            // *     "country_tld": ".hk",
            // *     "continent_code": "AS",
            // *     "in_eu": false,
            // *     "postal": null,
            // *     "latitude": 22.3539,
            // *     "longitude": 114.1342,
            // *     "timezone": "Asia/Hong_Kong",
            // *     "utc_offset": "+0800",
            // *     "country_calling_code": "+852",
            // *     "currency_name": "Dollar",
            // *     "currency": "HKD",
            // *     "languages": "zh-HK,yue,zh,en",
            // *     "country_area": 1092,
            // *     "country_population": 7451000,
            // *     "asn": "AS133744",
            // *     "org": "Better Cloud Limited"
            // * }

            var info = await request.SendWebRequest();
            if (info.result != UnityWebRequest.Result.Success)
                Debug.LogWarning("获取IP信息失败");

            var t = info.downloadHandler.text;
            var doc = JsonSerializer.Deserialize<JsonDocument>(t) ?? throw new NullReferenceException();
            JsonProperty? country_code = null;
            foreach (var v in doc.RootElement.EnumerateObject())
            {
                if (v.NameEquals("country_code"))
                {
                    country_code = v;
                    break;
                }
            }

            if (country_code?.Value.GetString() != "CN")
            {
                return false;
            }

            return true;
        }
    }
}