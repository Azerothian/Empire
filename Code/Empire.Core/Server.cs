using Empire.Core.Entities;
using Empire.Core.Messages;
using Illisian.Lidgren3;
using Illisian.UnityUtil.Logging;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Empire.Core
{
    public class Server
    {
        public Dictionary<Guid, Player> _players = new Dictionary<Guid, Player>();
        //public Dictionary<Guid, NetConnection> _connections = new Dictionary<Guid, NetConnection>();
        LidgrenServer _server;
        public Server()
        {
            _server = new LidgrenServer();

        }

        public void Initialise()
        {
            Log.Info("Empire Server 0.1alpha starting up...");

            //Initialise GameLogic

            //Init Network stack

           // _server.LocalIP = System.Net.IPAddress.Parse("192.168.1.8");
            _server.Port = 11337;
            _server.OnIncomingMessage += _server_OnIncomingMessage;
            _server.Initialise();

          
        }
        private void _server_OnIncomingMessage(NetMessage message)
        {
            
            if (message.Type == typeof(PlayerConnect))
            {
                Log.Info("Player Connected");
                var player = message.GetObject<Player>();

                foreach (var v in _players.Keys)
                {
                    var p = _players[v];
                    _server.SendMessage(new NetMessage() { Object = new PlayerConnect() { id = p.id, Name = p.Name, Position = p.Position, Rotation = p.Rotation, State = p.State } });
                }

                _players.Add(player.id, player);
                _server.SendMessageToAll(message, message.NetConnection);
            }
            else if (message.Type == typeof(PlayerMove))
            {
                var player = message.GetObject<Player>();
                if (_players.ContainsKey(player.id))
                {
                    _players[player.id] = player;
                    _server.SendMessageToAll(message, message.NetConnection);
                }

            }
            else if (message.Type == typeof(PlayerDisconnect))
            {
                Log.Info("Player Disconnected");
                var player = message.GetObject<Player>();
                if (_players.ContainsKey(player.id))
                {
                    _server.SendMessageToAll(message, message.NetConnection);
                    _players.Remove(player.id);
                }
            }
        }

        

        public void Shutdown()
        {
            _server.Shutdown();
        }
    }
}
