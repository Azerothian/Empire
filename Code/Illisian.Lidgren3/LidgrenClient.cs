using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;
using System.Threading;
using Illisian.Lidgren3.Messages;
using Illisian.UnityUtil.Logging;
using Illisian.UnityUtil;

namespace Illisian.Lidgren3
{
    public class LidgrenClient : LidgrenBase
    {
        public string HostName { get; set; }
        public IPAddress TargetIp { get; set; }
        public int TargetPort { get; set; }

        private NetClient _client = null;
        private Thread _connectWorker = null;

        private Queue<NetMessage> _pendingMessages = null;

        protected override void OnInitialise()
        {
            _pendingMessages = new Queue<NetMessage>();
            _client = new NetClient(_config);
            _client.Start();
        }
        public void Connect(string hostname, int port)
        {
            HostName = hostname;
            TargetPort = port;
            Connect();
        }

        public void Connect(IPAddress ip, int port)
        {
            TargetIp = ip;
            TargetPort = port;
            Connect();
        }
        public void Connect()
        {

            if (TargetIp == null)
            {
                var result = Dns.GetHostEntry(HostName);
                TargetIp = result.AddressList.FirstOrDefault();
            }
            if (TargetIp == null || TargetIp == IPAddress.None || TargetPort < 1 || TargetPort > 65535)
            {
                throw new Exception(String.Format("Trying to connect to nowhere Hostname: {0}, IPAddress: {1}", TargetIp, HostName));
            }
            if (_connectWorker == null)
            {
                _connectWorker = new Thread(() => ConnectMonitor());
                _connectWorker.Start();
            }
            else
            {
                Log.Debug("Connect is being called more then once.");
            }
        }

        private bool _connectInitiated = false;
        private bool _neverConnected = true;
        private int _connectionRetries = 10;
        private void ConnectMonitor()
        {
            Log.Debug("Client Connection Monitor thread has started.");
            do
            {
                if (_connectionRetries <= 0)
                {
                    this.Shutdown();
                    return;
                }
                
                if (!_connectInitiated)
                {
                    _client.Connect(new IPEndPoint(TargetIp, TargetPort));
                    _connectInitiated = true;
                    _connectionRetries--;
                }
                switch (_client.ConnectionStatus)
                {
                    case NetConnectionStatus.Disconnected:
                        if (!_neverConnected)
                            _connectInitiated = false;
                        break;
                    case NetConnectionStatus.Disconnecting:
                    case NetConnectionStatus.InitiatedConnect:
                    case NetConnectionStatus.RespondedAwaitingApproval:
                    case NetConnectionStatus.RespondedConnect:

                        break;
                    case NetConnectionStatus.Connected:
                        _connectionRetries = 10;
                        _neverConnected = false;
                        if (Monitor.TryEnter(_pendingMessages))
                        {
                            if (_pendingMessages.Count > 0)
                            {
                                Log.Debug("Client has a queued message, passing to the main message threads");
                                var message = _pendingMessages.Dequeue();
                                message.NetConnection = _client.ServerConnection;
                                QueueNetMessage(message);
                            }
                            Monitor.Exit(_pendingMessages);
                        }
                        break;
                }
                Thread.Sleep(10);
            } while (Running);
        }


        protected override void OnShutdown()
        {
            _connectWorker = null;
            _client.Shutdown("bye");
        }

        protected override NetPeer NetPeer
        {
            get { return _client; }
        }

        protected override string Name
        {
            get { return "LidgrenClient"; }
        }

        public void SendMessage(NetMessage msg)
        {


            if (_client != null && _client.ConnectionStatus != NetConnectionStatus.Connected)
            {
                Connect();
            }
            Monitor.Enter(_pendingMessages);
            _pendingMessages.Enqueue(msg);
            Monitor.Exit(_pendingMessages);
        }
    }
}
