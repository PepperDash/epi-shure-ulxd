using System;
using Crestron.SimplSharp.Net.Https;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using Crestron.SimplSharp;

namespace epi_mics_shure_ulxd
{
    public class BasicHttps
    {
        private HttpsClient _client;
        public event EventHandler<GenericHttpClientEventArgs> ResponseReceived;
        private string _ipAddress;
        private int _port;
        private int _pollTime = 30000;
        private CTimer _pollTimer;
        private bool _debug;

        private string _userName;
        private string _password;
        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public int PollTime
        {
            get { return _pollTime; }
            set
            {
                _pollTime = value;
                if (_pollTimer != null)
                {
                    _pollTimer.Stop();
                    _pollTimer.Dispose();
                    _pollTimer = null;
                }

                _pollTimer = new CTimer(PollStatus, null, 0, _pollTime);
            }
        }

        public BasicHttps(string key, string name, string ipAddress, string user, string pass)
		{
            _client = new HttpsClient();
            _ipAddress = ipAddress;
            _userName = user;
            _password = pass;

            _client.IncludeHeaders = true;
            _client.KeepAlive = true;

            _client.HostVerification = false;
            _client.PeerVerification = false;


		}
    }
}