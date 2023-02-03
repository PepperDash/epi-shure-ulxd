﻿using PepperDash.Essentials.Core;

namespace epi_mics_shure_ulxd
{
    public class ShureJoinMap : JoinMapBaseAdvanced
    {
        public ShureJoinMap(uint joinStart) : base(joinStart, typeof (ShureJoinMap))
        {
            
        }

        [JoinName("ReceiverIsOnline")]
        public JoinDataComplete ReceiverIsOnline =
            new JoinDataComplete(new JoinData { JoinNumber = 1, JoinSpan = 1 },
            new JoinMetadata
            {
                Description = "Receiver Device Online",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital
            });

        [JoinName("ChargerIsOnline")]
        public JoinDataComplete ChargerIsOnline =
            new JoinDataComplete(new JoinData { JoinNumber = 49, JoinSpan = 1 },
            new JoinMetadata
            {
                Description = "Charger Device Online",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital
            });

        [JoinName("Charger2IsPresent")]
        public JoinDataComplete Charger2IsPresent =
            new JoinDataComplete(new JoinData { JoinNumber = 48, JoinSpan = 1 },
            new JoinMetadata
            {
                Description = "Charger2 Device Present",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital
            });
        [JoinName("Charger2IsOnline")]
        public JoinDataComplete Charger2IsOnline =
            new JoinDataComplete(new JoinData { JoinNumber = 50, JoinSpan = 1 },
            new JoinMetadata
            {
                Description = "Charger2 Device Online",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital
            });

        [JoinName("NumberOfChargers")]
        public JoinDataComplete NumberOfChargers =
            new JoinDataComplete(new JoinData { JoinNumber = 50, JoinSpan = 1 },
            new JoinMetadata
            {
                Description = "Number Of Chargers",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Analog
            });



        [JoinName("Enabled")]
        public JoinDataComplete Enabled =
            new JoinDataComplete(new JoinData { JoinNumber = 2, JoinSpan = 1 },
            new JoinMetadata
            {
                Description = "Mic Enabled",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital
            });

        [JoinName("LowBatteryCaution")]
        public JoinDataComplete LowBatteryCaution =
            new JoinDataComplete(new JoinData { JoinNumber = 3, JoinSpan = 1 },
            new JoinMetadata
            {
                Description = "Mic Low Battery Caution",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital
            });

        [JoinName("LowBatteryWarning")]
        public JoinDataComplete LowBatteryWarning =
            new JoinDataComplete(new JoinData { JoinNumber = 4, JoinSpan = 1 },
            new JoinMetadata
            {
                Description = "Mic Low Battery Warning",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital
            });

        [JoinName("OnCharger")]
        public JoinDataComplete OnCharger =
            new JoinDataComplete(new JoinData { JoinNumber = 5, JoinSpan = 1 },
            new JoinMetadata
            {
                Description = "Mic On Charger",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital
            });

        [JoinName("OnChargerFbEnable")]
        public JoinDataComplete OnChargerFbEnable =
            new JoinDataComplete(new JoinData { JoinNumber = 6, JoinSpan = 1 },
            new JoinMetadata
            {
                Description = "Show Mic On Charger Fb",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital
            });

        [JoinName("LocalStatus")]
        public JoinDataComplete LocalStatus =
            new JoinDataComplete(new JoinData { JoinNumber = 2, JoinSpan = 1 },
            new JoinMetadata
            {
                Description = "Mic Status",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Analog
            });

        [JoinName("BatteryLevel")]
        public JoinDataComplete BatteryLevel =
            new JoinDataComplete(new JoinData { JoinNumber = 3, JoinSpan = 1 },
            new JoinMetadata
            {
                Description = "Mic Battery level",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Analog
            });

        [JoinName("BatteryStatus")]
        public JoinDataComplete BatteryStatus =
            new JoinDataComplete(new JoinData { JoinNumber = 4, JoinSpan = 1 },
            new JoinMetadata
            {
                Description = "Mic Battery level",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Analog
            });


        [JoinName("Name")]
        public JoinDataComplete Name =
            new JoinDataComplete(new JoinData { JoinNumber = 2, JoinSpan = 1 },
            new JoinMetadata
            {
                Description = "Mic Name",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial
            });

        [JoinName("ErrorString")]
        public JoinDataComplete ErrorString =
            new JoinDataComplete(new JoinData { JoinNumber = 1, JoinSpan = 1 },
            new JoinMetadata
            {
                Description = "Aggregate ErrorString",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial
            });




    }
}