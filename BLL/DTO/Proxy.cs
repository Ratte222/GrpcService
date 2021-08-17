using BLL.Helpers.ConvertDateTimeJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class Proxy
    {
        //"_id":"61166a84ca7e415905410cb6",
        [JsonProperty("_id")]
        public string Id {  get; set; }
        //"ip":"193.150.117.31",
        [JsonProperty("ip")]
        public string Ip { get; set; }
        //"anonymityLevel":"transparent",
        [JsonProperty("anonymityLevel")]
        public string AnonymityLevel {  get; set; }
        //"asn":"AS57449",
        [JsonProperty("asn")]
        public string Asn { get; set;  }
        //"city":"Moscow",
        [JsonProperty("city")]
        public string City {  get; set; }
        //"country":"RU",
        [JsonProperty("country")]
        public string Country {  get; set; }
        //"created_at":"2021-08-13T12:50:12.811Z",
        [JsonProperty("created_at")]
        [JsonConverter(typeof(ESDateTimeConverter))]
        public DateTime CreatedAt {  get; set; }
        //"google":false,
        [JsonProperty("google")]
        public bool IsGoogle { get; set; }
        //"isp":"LTD \"ARENTEL\"",
        [JsonProperty("isp")]
        public string Isp {  get; set; }
        //"lastChecked":1628859709,//unix time
        //[JsonProperty("lastChecked")]
        //[JsonConverter(typeof(JavaScriptDateTimeConverter))]
        //public DateTime LastChecked { get; set; }
        //"latency":211,
        [JsonProperty("latency")]
        public double Latency { get;set;  }
        //"org":"LTD \"ARENTEL\"",
        [JsonProperty("org")]
        public string Org { get; set; }
        //"port":"8000",
        [JsonProperty("port")]
        public int Prot {  get; set; }
        //"protocols":["http"],
        [JsonProperty("protocols")]
        public string[] Protocols {  get; set; }
        //"region":null,
        [JsonProperty("region")]
        public string Region {  get; set; }
        //"responseTime":33,
        [JsonProperty("responseTime")]
        public int ResponseTime { get; set; }
        //"speed":null,
        [JsonProperty("speed")]
        public string Speed {  get; set; }
        //"updated_at":"2021-08-13T13:01:49.100Z",
        [JsonProperty("updated_at")]
        [JsonConverter(typeof(ESDateTimeConverter))]
        public DateTime UpdateAt {  get; set; }
        //"workingPercent":null,
        [JsonProperty("workingPercent")]
        public string WorkingPercent {  get; set; }
        //"upTime":100,
        [JsonProperty("upTime")]
        public double UpTime {  get; set; }
        //"upTimeSuccessCount":1,
        [JsonProperty("upTimeSuccessCount")]
        public int UpTimeSuccessCount { get; set; }
        //"upTimeTryCount":1
        [JsonProperty("upTimeTryCount")]
        public int UpTimeTryCount { get; set; }
        [JsonProperty("isBlock")]
        public bool IsBlock { get; set; } = false;
    }
}
