using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;
using System.Net;
using Illisian.Lidgren3.Messages;
using Illisian.UnityUtil.Logging;

namespace Illisian.Lidgren3
{
    public class LidgrenServer : LidgrenBase
    {
        NetServer _server;

        string _name = "LidgrenServer";

        protected override string Name
        {
            get { return _name; }
           // set { _name = value; }
        }

        protected override NetPeer NetPeer
        {
            get { return _server; }
        }
        protected override void OnInitialise()
        {
            Log.Info(String.Format("Initialising NetServer on Port {0}..", _config.Port));
            OnIncomingMessage += LidgrenServer_OnIncomingMessage;
            _config.AcceptIncomingConnections = true;
            _server = new NetServer(_config);
            _server.Start();
        }

        void LidgrenServer_OnIncomingMessage(NetMessage message)
        {
            //if (message.Type == typeof(RequestNatIntroduction))
            //{
            //    var internalEndPoint = new IPEndPoint(IPAddress.Parse(ConfigConnectionElement.Ip), ConfigConnectionElement.Port);
            //    var externalEndPoint = new IPEndPoint(IPAddress.Parse(ConfigConnectionElement.ExternalIp), ConfigConnectionElement.Port);
            //    Log.Debug("Sending Nat Introduction Message");
            //    NetPeer.Introduce(internalEndPoint, externalEndPoint, message.NetConnection.RemoteEndpoint, message.NetConnection.RemoteEndpoint, "intro");
            //    return;
            //}
        }

        protected override void OnShutdown()
        {
            Log.Info(String.Format("Terminating NetServer on Port {0}..", _config.Port));
            _server.Shutdown("bye");
        }

        public void SendMessage(NetMessage msg)
        {
            QueueNetMessage(msg);
        }
        public void SendMessageToAll(NetMessage msg, params NetConnection[] excluding)
        {
            foreach (var c in NetPeer.Connections)
            {
                if (!excluding.Contains(c))
                {
                    msg.NetConnection = c;
                    QueueNetMessage(msg);
                }
            }
        }

    }
}
