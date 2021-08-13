using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

using PepperDash.Core;
using PepperDash.Core.JsonStandardObjects;
using PepperDash.Essentials.Core;

using Newtonsoft.Json;

namespace epi_mics_shure_ulxd
{
    public class DeviceFactory : EssentialsPluginDeviceFactory<ShureUlxMicDevice>
    {
        public DeviceFactory()
        {
            // Set the minimum Essentials Framework Version
            MinimumEssentialsFrameworkVersion = "1.6.7";

            // In the constructor we initialize the list with the typenames that will build an instance of this device
            TypeNames = new List<string>() { "shureulxd", "ulxd", "shure-ulx-d" };
        }

        // Builds and returns an instance of EssentialsPluginDeviceTemplate
        public override EssentialsDevice BuildDevice(PepperDash.Essentials.Core.Config.DeviceConfig dc)
        {
            Debug.Console(1, "Factory Attempting to create new device from type: {0}", dc.Type);
            //var propertiesConfig = JsonConvert.DeserializeObject<ShureUlxMicDeviceProperties>(dc.Properties.ToString());

            var propertiesConfig = dc.Properties.ToObject<ShureUlxMicDeviceProperties>();

            var c = propertiesConfig.ControlChargerBase.TcpSshProperties;

            var c2 = propertiesConfig.ControlChargerBase2 == null
                ? null
                : propertiesConfig.ControlChargerBase2.TcpSshProperties;

            var commReceiver = CommFactory.CreateCommForDevice(dc);

            var chargerKey = String.Format("{0}-{1}", dc.Key, "ChargerBase");

            var chargerKey2 = String.Format("{0}-{1}", dc.Key, "ChargerBase2");

            var chargerSocket = new GenericTcpIpClient(chargerKey + "-tcp", c.Address, c.Port, c.BufferSize)
            {
                AutoReconnect = c.AutoReconnect,
                AutoReconnectIntervalMs = c.AutoReconnect ? c.AutoReconnectIntervalMs : 0
            };
            
            DeviceManager.AddDevice(chargerSocket);


            var chargerSocket2 = c2 == null ? 
                null  
                : new GenericTcpIpClient(chargerKey2 + "-tcp", c2.Address, c2.Port, c2.BufferSize)
            {
                AutoReconnect = c2.AutoReconnect,
                AutoReconnectIntervalMs = c2.AutoReconnect ? c2.AutoReconnectIntervalMs : 5000
            };

            if (c2 != null)
            {
                Debug.Console(1, "Adding {0}!!", chargerKey2);
                DeviceManager.AddDevice(chargerSocket2);
            }


            return new ShureUlxMicDevice(dc.Key, dc.Name, commReceiver, chargerSocket, chargerSocket2, propertiesConfig);
        }

    }
}