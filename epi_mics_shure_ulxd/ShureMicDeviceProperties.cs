using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PepperDash.Essentials.Core;

namespace epi_mics_shure_ulxd
{
    public class ShureUlxMicDeviceProperties
    {

        [JsonProperty("control")]
        public EssentialsControlPropertiesConfig Control { get; set; }

        [JsonProperty("controlChargerBase")]
        public EssentialsControlPropertiesConfig ControlChargerBase { get; set; }

        [JsonProperty("controlChargerBase2")]
        public EssentialsControlPropertiesConfig ControlChargerBase2 { get; set; }


        [JsonProperty("mics")]
        public List<Mics> Mics { get; set; }

        [JsonProperty("cautionThreshold")]
        public int CautionThreshold { get; set; }

        [JsonProperty("warningThreshold")]
        public int WarningThreshold { get; set; }
    }

    public class Mics
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("onChargerFbEnable")]
        public Boolean OnChargerFbEnable { get; set; }
    }

}
