using Empire.Core.Messages;
using Illisian.Lidgren3;
using Illisian.UnityUtil;
using System;
using System.Collections.Generic;

namespace Empire.Core
{
    public class Client
    {
        public event Response<PlayerConnect> OnPlayerConnect;
        public event Response<PlayerDisconnect> OnPlayerDisconnect;
        public event Response<PlayerMove> OnPlayerMove;

        Queue<NetMessage> _messageQueue = new Queue<NetMessage>();

        LidgrenClient _client;
        public Client()
        {
            _client = new LidgrenClient();
        }

        public void Initialise()
        {
            _client.HostName = "localhost";
            _client.TargetPort = 11337;
            _client.EnableUPnP = false;
            _client.Initialise();
            _client.Connect();
            _client.OnIncomingMessage += _client_OnIncomingMessage;
        }


        void _client_OnIncomingMessage(NetMessage message)
        {
           _messageQueue.Enqueue(message);
        }

        public bool IsMessageAvailable
        {
            get
            {
                return _messageQueue.Count > 0;
            }
        }

        public void ProcessMessage()
        {
            if (IsMessageAvailable)
            {
                var message = _messageQueue.Dequeue();

                if (OnPlayerConnect != null && message.Type == typeof(PlayerConnect))
                {
                    OnPlayerConnect(message.Object as PlayerConnect);
                }
                else if (OnPlayerDisconnect != null && message.Type == typeof(PlayerDisconnect))
                {
                    OnPlayerDisconnect(message.Object as PlayerDisconnect);
                }
                else if (OnPlayerMove != null && message.Type == typeof(PlayerMove))
                {
                    OnPlayerMove(message.Object as PlayerMove);
                }
            }
        }
        public void SendMessage(IMessage msg)
        {
            _client.SendMessage(new NetMessage() { Object = msg });
        }



        public void Shutdown()
        {
            _client.Shutdown();
        }
    }
}
