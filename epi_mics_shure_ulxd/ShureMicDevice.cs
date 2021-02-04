﻿using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using System.Text.RegularExpressions;
using Crestron.SimplSharpPro.DeviceSupport;
using PepperDash.Essentials.Core.Bridges;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace epi_mics_shure_ulxd
{
    public class ShureUlxMicDevice : EssentialsBridgeableDevice, IDeviceInfoProvider
    {
        private readonly ShureUlxMicDeviceProperties _props;
        
        /// <summary>
        /// Collection of all Device Feedbacks
        /// </summary>
        public FeedbackCollection<Feedback> Feedbacks;

        public Dictionary<int, bool> MicEnable;
        public Dictionary<int, BoolFeedback> MicEnableFeedback;

        public Dictionary<int, IntFeedback> MicStatusFeedback;
        public Dictionary<int, int> MicStatus;

        public Dictionary<int, IntFeedback> MicBatteryLevelFeedback;
        public Dictionary<int, int> MicBatteryLevel;

        public Dictionary<int, BoolFeedback> MicLowBatteryCautionFeedback;
        public Dictionary<int, bool> MicLowBatteryCaution;

        public Dictionary<int, BoolFeedback> MicLowBatteryWarningFeedback;
        public Dictionary<int, bool> MicLowBatteryWarning;

        public Dictionary<int, IntFeedback> MicLowBatteryStatusFeedback;
        public Dictionary<int, int> MicLowBatteryStatus;

        public Dictionary<int, BoolFeedback> MicOnChargerFeedback;
        public Dictionary<int, bool> MicOnCharger;

        public Dictionary<int, StringFeedback> MicNamesFeedback;
        public Dictionary<int, string> MicNames;

        private Dictionary<int, MuteStatus> _micMuted;
        private Dictionary<int, ChargeStatus> _micCharging;

        private string _error;

        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                ErrorFeedback.FireUpdate();
            }
        }

        public bool IsOnline
        {
            get { return CommunicationCharger.IsConnected && CommunicationReceiver.IsConnected; }
        }

        public StringFeedback ErrorFeedback;

        public BoolFeedback OnlineFeedback;

        public IBasicCommunication CommunicationReceiver { get; private set; }
        public CommunicationGather PortGatherReceiver { get; private set; }
        public GenericCommunicationMonitor CommunicationMonitorReceiver { get; private set; }

        public IBasicCommunication CommunicationCharger { get; private set; }
        public CommunicationGather PortGatherCharger { get; private set; }
        public GenericCommunicationMonitor CommunicationMonitorCharger { get; private set; }

        private long _cautionThreshold;

        public string ReceiverFirmware { get; private set; }
        public string ChargerFirmware { get; private set; }

        public string ConcactFirmware
        {
            get { return ReceiverFirmware + " , " + ChargerFirmware; }
        }

        int CautionThreshold
        {
            get
            {
                return (int)((_cautionThreshold * 65535) / 100);
            }
            set
            {
                _cautionThreshold = value;
            }
        }

        private long _warningThreshold;

        int WarningThreshold
        {
            get
            {
                return (int)((_warningThreshold * 65535) / 100);
            }
            set
            {
                _warningThreshold = value;
            }
        }




        public ShureUlxMicDevice(string key, string name, IBasicCommunication commReceiver, IBasicCommunication commCharger,
            ShureUlxMicDeviceProperties props) :
                base(key, name)
        {

            DeviceInfo = new DeviceInfo();

            _props = props;

            CommunicationReceiver = commReceiver;
            CommunicationCharger = commCharger;

            CautionThreshold = _props.CautionThreshold;
            WarningThreshold = _props.WarningThreshold;

            var socketReceiver = commReceiver as ISocketStatus;
            var socketCharger = commCharger as ISocketStatus;

            OnlineFeedback = new BoolFeedback(() => IsOnline);
            //Debug.Console(2, this, "Reg1");
            var receiverIp = _props.Control.TcpSshProperties.Address;
            //Debug.Console(2, this, "Reg2");

            var chargerIp = _props.ControlChargerBase.TcpSshProperties.Address;
           // Debug.Console(2, this, "Reg3");


            var concactIp = String.Format("{0} , {1}", receiverIp, chargerIp);
            //Debug.Console(2, this, "Reg4");

            const string pattern = @"^[a-zA-Z'.\s]";
            //Debug.Console(2, this, "Reg5");

            var rgx = new Regex(pattern);

            if (rgx.IsMatch(concactIp.Split(',')[0]))
            {
                Debug.Console(2, this, "RegexMatch");

                DeviceInfo.HostName = concactIp;
            }
            else
            {
                Debug.Console(2, this, "RegexNoMatch");

                DeviceInfo.IpAddress = concactIp;
            }


            

            if (socketReceiver != null) socketReceiver.ConnectionChange += socketReceiver_ConnectionChange;
            

            if (socketCharger != null) socketCharger.ConnectionChange += socketCharger_ConnectionChange;
           

            PortGatherCharger = new CommunicationGather(CommunicationCharger, ">"); 
          
            PortGatherReceiver = new CommunicationGather(CommunicationReceiver, ">");
         


            PortGatherCharger.LineReceived += PortGatherCharger_LineReceived;
          

            PortGatherReceiver.LineReceived += PortGatherReceiver_LineReceived;
           


            CommunicationMonitorCharger = new GenericCommunicationMonitor(this, CommunicationCharger, 30000, 180000, 300000, DoChargerPoll);
           

            CommunicationMonitorReceiver = new GenericCommunicationMonitor(this, CommunicationReceiver, 30000, 180000, 300000, DoReceiverPoll);
          


            Init();


        }

        private void Init()
        {
            MicEnable = new Dictionary<int, bool>();
            MicEnableFeedback = new Dictionary<int, BoolFeedback>();

            MicStatus = new Dictionary<int, int>();
            MicStatusFeedback = new Dictionary<int, IntFeedback>();

            MicBatteryLevel = new Dictionary<int, int>();
            MicBatteryLevelFeedback = new Dictionary<int, IntFeedback>();

            MicLowBatteryCaution = new Dictionary<int, bool>();
            MicLowBatteryCautionFeedback = new Dictionary<int, BoolFeedback>();

            MicLowBatteryWarning = new Dictionary<int, bool>();
            MicLowBatteryWarningFeedback = new Dictionary<int, BoolFeedback>();

            MicLowBatteryStatus = new Dictionary<int, int>();
            MicLowBatteryStatusFeedback = new Dictionary<int, IntFeedback>();

            MicOnCharger = new Dictionary<int, bool>();
            MicOnChargerFeedback = new Dictionary<int, BoolFeedback>();

            MicNames = new Dictionary<int, string>();
            MicNamesFeedback = new Dictionary<int, StringFeedback>();

            ErrorFeedback = new StringFeedback(() => Error);

            _micMuted = new Dictionary<int, MuteStatus>();
            _micCharging = new Dictionary<int, ChargeStatus>();

            foreach (var item in _props.Mics)
            {
                var i = item;
                Debug.Console(2, this, "This Mic's name is {0}", i.Name);

                _micMuted.Add(i.Index,MuteStatus.OFF);
                _micCharging.Add(i.Index, ChargeStatus.NO);

                MicStatus.Add(i.Index, 0);
                MicStatusFeedback.Add(i.Index, new IntFeedback(() => MicStatus[i.Index]));

                MicBatteryLevel.Add(i.Index, 0);
                MicBatteryLevelFeedback.Add(i.Index, new IntFeedback(() => MicBatteryLevel[i.Index]));

                MicLowBatteryCaution.Add(i.Index, false);
                MicLowBatteryCautionFeedback.Add(i.Index, new BoolFeedback(() => MicLowBatteryCaution[i.Index]));

                MicLowBatteryWarning.Add(i.Index, false);
                MicLowBatteryWarningFeedback.Add(i.Index, new BoolFeedback(() => MicLowBatteryWarning[i.Index]));

                MicLowBatteryStatus.Add(i.Index, 0);
                MicLowBatteryStatusFeedback.Add(i.Index, new IntFeedback(() => MicLowBatteryStatus[i.Index]));

                MicOnCharger.Add(i.Index, false);
                MicOnChargerFeedback.Add(i.Index, new BoolFeedback(() => MicOnCharger[i.Index]));

                MicNames.Add(i.Index, i.Name);
                MicNamesFeedback.Add(i.Index, new StringFeedback(() => MicNames[i.Index]));

                MicEnable.Add(i.Index, i.Enabled);
                MicEnableFeedback.Add(i.Index, new BoolFeedback(() => MicEnable[i.Index]));

            }

            CommunicationCharger.Connect();
            CommunicationReceiver.Connect();
            CommunicationMonitorCharger.Start();
            CommunicationMonitorReceiver.Start();
        }




        void PortGatherReceiver_LineReceived(object sender, GenericCommMethodReceiveTextArgs e)
        {
            Debug.Console(2, this, "Receiver RX: '{0}'", e.Text);

            try
            {
                var data = e.Text;

                if (!data.Contains("REP") || data.Contains("ERR"))
                {
                    return;
                }

                var dataChunks = data.Split(' ');

                if (data.Contains("FW_VER"))
                {
                    var firmware = dataChunks[3];

                    ReceiverFirmware = firmware.TrimStart('{').TrimEnd('}');

                    Debug.Console(2, this, "Receiver Firmware: {0}", ReceiverFirmware);

                    return;
                }

                var attribute = dataChunks[3];
                var index = int.Parse(dataChunks[2]);

				if (attribute.Contains("AUDIO_MUTE"))
				{
					var status = (MuteStatus)Enum.Parse(typeof(MuteStatus), dataChunks[4], true);

					if (MicEnable[index])	//2021-01-22 ERD: This doesnt seem to ignore the Mics changed to 'Enable: False' in config which is the intent
					{
						_micMuted[index] = status;
						
						ChargeStatus tMicCharge;
						if (_micCharging.TryGetValue(index, out tMicCharge))
						{
							Debug.Console(1, this, "_micCharging[{0}]: {1}", index, tMicCharge);
						}
						else
						{
							tMicCharge = 0;
						}
						if (tMicCharge != ChargeStatus.NO)
						{
							return;
						}

						switch (status)
						{
							case (MuteStatus.OFF):
								MicStatus[index] = (int)(Tx_Status.ACTIVE);
								break;
							case (MuteStatus.ON):
								MicStatus[index] = (int)(Tx_Status.MUTE);
								break;
							default:
								break;
						}
						return;
					}
				}

                if (attribute.Contains("BATT_CHARGE"))
                {
                    var battCharge = ushort.Parse(dataChunks[4]);

					if (MicEnable[index])
					{
						MicBatteryLevel[index] = battCharge;

						if (battCharge == 255)
						{
							MicStatus[index] = (int)Tx_Status.UNKNOWN;
							MicStatusFeedback[index].FireUpdate();
						}

						MicBatteryLevelFeedback[index].FireUpdate();

						UpdateAlert(index);

						return;
					}
                }
                CheckStatusConditions();
            }
            catch (Exception ex)
            {
				Debug.Console(1, this, "PortGatherReceiver Exception: {0}", ex.Message);
                Debug.Console(2, this, "PortGatherReceiver Stack Trace: {0}", ex.StackTrace);
            }
        }

        void PortGatherCharger_LineReceived(object sender, GenericCommMethodReceiveTextArgs e)
        {
            try
            {
                var data = e.Text;

                if (!data.Contains("REP") || data.Contains("ERR"))
                {
                    return;
                }

                var dataChunks = data.Split(' ');

                if (data.Contains("FW_VER"))
                {
                    var firmware = dataChunks[3];

                    ReceiverFirmware = firmware.TrimStart('{').TrimEnd('}');

                    return;
                }

                var index = int.Parse(dataChunks[2]);
                var attribute = dataChunks[3];

                if (attribute.Contains("TX_AVAILABLE"))
                {
                    
                    var status = (ChargeStatus) Enum.Parse(typeof (ChargeStatus), dataChunks[4], true);
                    _micCharging[index] = status;

                    if (status == ChargeStatus.YES) MicStatus[index] = (int)Tx_Status.ON_CHARGER;
                    MicOnCharger[index] = MicStatus[index] == (int) Tx_Status.ON_CHARGER;
                    MicOnChargerFeedback[index].FireUpdate();

                    UpdateAlert(index);
                }

                CheckStatusConditions();
            }

            catch (Exception ex)
            {
                Debug.Console(1, this, "PortGatherCharger Exception: {0}", ex.Message);
                Debug.Console(2, this, "PortGatherCharger Stack Trace: {0}", ex.StackTrace);
            }
        }

        private void UpdateAlert(int data)
        {
            if (MicStatus[data] == (int)Tx_Status.ON_CHARGER)
            {
                MicLowBatteryCaution[data] = false;
                MicLowBatteryWarning[data] = false;
                MicLowBatteryStatus[data] = 0; 
            }

            else if (MicStatus[data] != (int)Tx_Status.UNKNOWN)
            {
                if (MicBatteryLevel[data] <= WarningThreshold)
                {
                    MicLowBatteryWarning[data] = true;
                    MicLowBatteryCaution[data] = false;
                    MicLowBatteryStatus[data] = 2;

                }
                else if (MicBatteryLevel[data] <= CautionThreshold)
                {
                    MicLowBatteryWarning[data] = false;
                    MicLowBatteryCaution[data] = true;
                    MicLowBatteryStatus[data] = 1;
                }
                else
                {
                    MicLowBatteryCaution[data] = false;
                    MicLowBatteryWarning[data] = false;
                    MicLowBatteryStatus[data] = 0;
                }
            }

            MicLowBatteryCautionFeedback[data].FireUpdate();
            MicLowBatteryWarningFeedback[data].FireUpdate();
            MicLowBatteryStatusFeedback[data].FireUpdate();
            MicNamesFeedback[data].FireUpdate();
            if (!String.IsNullOrEmpty(ReceiverFirmware) && !String.IsNullOrEmpty(ChargerFirmware))
            {
                DeviceInfo.FirmwareVersion = ConcactFirmware;

                UpdateDeviceInfo();
            }
        }


        private void CheckStatusConditions()
        {
            var errorCode = 0;
            var errorStatus = "";

            foreach (var mic in MicEnable)
            {
                var index = mic.Key;
                var caution = MicLowBatteryCaution[index];
                var warning = MicLowBatteryWarning[index];
                var charging = MicOnCharger[index];
                var micName = MicNames[index];
                var cautionThreshold = CautionThreshold;
                var warningThreshold = WarningThreshold;

                if (errorStatus.Length > 0)
                {
                    errorStatus += "| ";
                }

                if (caution && !warning)
                {
                    errorStatus += String.Format("{0} - {1} - Mic Level < {2}% and{3} Charging", Name, micName,
                        cautionThreshold,
                        charging ? "" : " not");
                    if (errorCode < 1)
                    {
                        errorCode = 1;
                    }
                }

                else if (warning && !caution)
                {
                    errorStatus += String.Format("{0} - {1} - Mic Level < {2}% and{3} Charging", Name, micName,
                        warningThreshold,
                        charging ? "" : " not");
                    if (errorCode < 2)
                    {
                        errorCode = 2;
                    }
                }
            }
        }

        internal void socketCharger_ConnectionChange(object sender, GenericSocketStatusChageEventArgs e)
        {
            OnlineFeedback.FireUpdate();
        }

        internal void socketReceiver_ConnectionChange(object sender, GenericSocketStatusChageEventArgs e)
        {
            OnlineFeedback.FireUpdate();
        }

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var myJoinMap = new ShureJoinMap(joinStart);

            if (bridge != null)
            {
                bridge.AddJoinMap(Key, myJoinMap);
            }

            Debug.Console(1, this, "Linking to Trilist '{0}'", trilist.ID.ToString("X"));

            OnlineFeedback.LinkInputSig(trilist.BooleanInput[myJoinMap.IsOnline.JoinNumber]);
            ErrorFeedback.LinkInputSig(trilist.StringInput[myJoinMap.ErrorString.JoinNumber]);

            foreach (var item in _props.Mics)
            {
                var i = item;
                var index = i.Index;

                var offset = (uint) ((index - 1)*4);

                Debug.Console(2, this, "Mic Channel {0} Connect", i.Index);

                MicEnableFeedback[index].LinkInputSig(trilist.BooleanInput[myJoinMap.Enabled.JoinNumber + offset]);
                MicNamesFeedback[index].LinkInputSig(trilist.StringInput[myJoinMap.Name.JoinNumber + offset]);
                MicBatteryLevelFeedback[index].LinkInputSig(trilist.UShortInput[myJoinMap.BatteryLevel.JoinNumber + offset]);
                MicStatusFeedback[index].LinkInputSig(trilist.UShortInput[myJoinMap.LocalStatus.JoinNumber + offset]);
                MicLowBatteryStatusFeedback[index].LinkInputSig(trilist.UShortInput[myJoinMap.BatteryStatus.JoinNumber + offset]);
                MicLowBatteryCautionFeedback[index].LinkInputSig(trilist.BooleanInput[myJoinMap.LowBatteryCaution.JoinNumber + offset]);
                MicOnChargerFeedback[index].LinkInputSig(trilist.BooleanInput[myJoinMap.OnCharger.JoinNumber + offset]);
                MicLowBatteryWarningFeedback[index].LinkInputSig(trilist.BooleanInput[myJoinMap.LowBatteryWarning.JoinNumber + offset]);
                trilist.SetBool(myJoinMap.OnChargerFbEnable.JoinNumber, i.OnChargerFbEnable);

            }

            trilist.OnlineStatusChange += (d, args) =>
            {
                if (!args.DeviceOnLine) return;
                foreach (var item in MicNamesFeedback)
                {
                    item.Value.FireUpdate();
                }

                foreach (var item in MicEnableFeedback)
                {
                    item.Value.FireUpdate();
                }

                foreach (var item in _props.Mics)
                {
                    var i = item;
                    trilist.SetBool(myJoinMap.OnChargerFbEnable.JoinNumber, i.OnChargerFbEnable);
                }
            };

        }

        private void DoChargerPoll()
        {
            CommunicationCharger.SendText("< GET FW_VER >");
			//CommunicationCharger.SendText("< GET MODEL >");

			for (var i = 0; i < MicStatus.Count; i++)
			{
				CommunicationCharger.SendText(String.Format("< GET {0} TX_AVAILABLE >", i + 1));
			}
        }

        private void DoReceiverPoll()
        {
            CommunicationReceiver.SendText("< GET FW_VER >");

            for (var i = 0; i < MicStatus.Count; i++)
            {
                CommunicationReceiver.SendText(String.Format("< GET {0} BATT_CHARGE >", i + 1));
                CommunicationReceiver.SendText(String.Format("< GET {0} AUDIO_MUTE >", i + 1));
            }
        }

        enum Tx_Status
        {
            ACTIVE = 1,
            MUTE = 2,
            STANDBY = 3,
            ON_CHARGER = 4,
            UNKNOWN = 5
        }

        enum MuteStatus
        {
            ON = 1,
            OFF = 2
        }

        enum ChargeStatus
        {
            YES = 1,
            NO = 2
        }

        public DeviceInfo DeviceInfo { get; private set; }
        public event DeviceInfoChangeHandler DeviceInfoChanged;
        public void UpdateDeviceInfo()
        {
            DeviceInfoChanged(this, new DeviceInfoEventArgs(DeviceInfo));
        }
    }
    public class DeviceInfoEventArgs : EventArgs
    {
        public DeviceInfo DeviceInfo { get; set; }

        public DeviceInfoEventArgs()
        {

        }

        public DeviceInfoEventArgs(DeviceInfo devInfo)
        {
            DeviceInfo = devInfo;
        }
    }

    public class DeviceInfo
    {
        public string HostName { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
        public string SerialNumber { get; set; }
        public string FirmwareVersion { get; set; }
    }

    public interface IDeviceInfoProvider
    {
        DeviceInfo DeviceInfo { get; }

        event DeviceInfoChangeHandler DeviceInfoChanged;

        void UpdateDeviceInfo();
    }

    public delegate void DeviceInfoChangeHandler(IKeyed device, DeviceInfoEventArgs args);


}