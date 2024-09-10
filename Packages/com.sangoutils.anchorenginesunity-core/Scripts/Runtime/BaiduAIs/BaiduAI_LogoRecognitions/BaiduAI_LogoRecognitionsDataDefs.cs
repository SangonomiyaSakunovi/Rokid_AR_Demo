using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

namespace SangoUtils.AnchorEngines_Unity.Core.BaiduAIs.LogoRecognitions
{
    internal class BaiduAI_LogoRecognitionsDataDefs
    {
        [SerializeField]
        public class RspData
        {
            [JsonPropertyName("result_num")]
            public int ResultNum { get; set; } = 0;
            [JsonPropertyName("result")]
            public IList<ResData>? Results { get; set; }
            [JsonPropertyName("error_code")]
            public int ErrorCode { get; set; } = 0;
            [JsonExtensionData]
            public Dictionary<string, JsonElement>? ExtensionData { get; set; }
        }

        [SerializeField]
        public class ResData
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("type")]
            public int Type { get; set; }
            [JsonPropertyName("probability")]
            public float Probability { get; set; }
            [JsonPropertyName("location")]
            public LocData Location { get; set; }
        }

        [SerializeField]
        public class LocData
        {
            [JsonPropertyName("top")]
            public int Top { get; set; }
            [JsonPropertyName("height")]
            public int Height { get; set; }
            [JsonPropertyName("left")]
            public int Left { get; set; }
            [JsonPropertyName("width")]
            public int Width { get; set; }
        }
    }
}