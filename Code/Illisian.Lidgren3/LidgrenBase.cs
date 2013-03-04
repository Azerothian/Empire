using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Threading;
using Illisian.Lidgren3.Messages;
using System.Net;
using Illisian.UnityUtil;
using Illisian.UnityUtil.Logging;

namespace Illisian.Lidgren3
{
    public abstract class LidgrenBase
    {
        private IPAddress _localIP = null;
        private IPAddress _externalIP = null;
        private int _port = 0;


        protected bool NatIntroduction = false;

        public bool EnableUPnP { get; set; }

        public IPAddress LocalIP { get { return _localIP; } set { _localIP = value; } }
        public IPAddress ExternalIP { get { return _externalIP; } set { _externalIP = value; } }
        public int Port { get { return _port; } set { _port = value; } }

        private Thread _receiveMessageWorker;
        private Thread _sendMessageWorker;

        protected bool Running;
        private Queue<NetMessage> _outgoingMessages;
        public event Response<NetMessage> OnIncomingMessage;

        protected NetPeerConfiguration _config;

        protected INetEncryption _encryption
        {
            get
            {
                return new NetXorEncryption("illisian"); //TODO: figure out a proper Public/Private Key system for each individual user.
            }
        }

        protected abstract void OnInitialise();
        protected abstract void OnShutdown();
        protected abstract NetPeer NetPeer { get; }
        protected abstract string Name { get; }

        public void Initialise()
        {
            _outgoingMessages = new Queue<NetMessage>();
            _config = new NetPeerConfiguration("Illisian");
            _config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            _config.EnableMessageType(NetIncomingMessageType.NatIntroductionSuccess);
            _config.Port = Port;
            _config.EnableUPnP = EnableUPnP;

            if (LocalIP == null)
            {
                Log.Info("LocalIP is not set, selecting first in the list");
                IPHostEntry hostentry = Dns.GetHostEntry(Dns.GetHostName());
                LocalIP = hostentry.AddressList.FirstOrDefault();
                if (LocalIP == null)
                {
                    throw new Exception("No Host Entries Found.. do you have a network stack on your device?");
                }
                Log.Info(string.Format("LocalIP is {0}", LocalIP));
            }
            
            OnInitialise();
            if (EnableUPnP)
            {
                if (NetPeer.UPnP != null)
                {


                    Log.Info(String.Format("Enabling UPnP for Port {0}", NetPeer.Port));

                    if (NetPeer.UPnP.ForwardPort(NetPeer.Port, Name))
                    {
                        Log.Info(string.Format("UPNP has been enabled"));
                    }
                    else
                    {
                        Log.Info(string.Format("UPNP is not enabled, something is wrong with your router?"));
                    }
                }
                else
                {
                    Log.Info(string.Format("Unable to initialise UPnP as the object is null... bbqWTF?"));
                }

            }
            Running = true;

            _receiveMessageWorker = new Thread(ReceiveMessageWorker);
            _receiveMessageWorker.Start();
            _sendMessageWorker = new Thread(SendMessageWorker);
            _sendMessageWorker.Start();
        }



        private void SendMessageWorker()
        {
            do
            {
                if (Monitor.TryEnter(_outgoingMessages))
                {
                    if (_outgoingMessages.Count > 0)
                    {
                        var message = _outgoingMessages.Dequeue();
                        SendNetMessage(message);
                    }
                    Monitor.Exit(_outgoingMessages);
                }


                Thread.Sleep(10);
            } while (Running);

        }


        private void ReceiveMessageWorker()
        {
            do
            {
                NetIncomingMessage im;
                while ((im = NetPeer.ReadMessage()) != null)
                {
                    ProcessNetIncomingMessage(im);
                }
                NetPeer.MessageReceivedEvent.WaitOne();
            } while (Running);

        }

        private void ProcessNetIncomingMessage(NetIncomingMessage im)
        {
            // handle incoming message
            switch (im.MessageType)
            {
                    
                case NetIncomingMessageType.DebugMessage:
                    Log.Debug(string.Format("Lidgren DebugMessage :: {0} ", im.ReadString()));
                    break;
                case NetIncomingMessageType.ErrorMessage:
                    Log.Critical(string.Format("Lidgren ErrorMessage :: {0} ", im.ReadString()));
                    break;
                case NetIncomingMessageType.WarningMessage:
                    Log.Warn(string.Format("Lidgren WarningMessage :: {0} ", im.ReadString()));
                    break;
                case NetIncomingMessageType.VerboseDebugMessage:
                    Log.Debug(im.ReadString());
                    break;
                case NetIncomingMessageType.StatusChanged:
                    NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                    string reason = im.ReadString();
                    Log.Info(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + " : " + reason);
                    break;

                case NetIncomingMessageType.NatIntroductionSuccess:
                    Log.Debug("NAT Introduction was a success");
                    NatIntroduction = true;
                    break;
                case NetIncomingMessageType.UnconnectedData:
                    ProcessNetIncomingMessageData(im, true);
                    break;
                case NetIncomingMessageType.Data:

                    ProcessNetIncomingMessageData(im, false);//TODO: Split this into two threads with a list inbetween and fire the events on the second thread, possibly using a pulse manager, so this is non blocking on recieve.

                    break;
                    
                default:
                    Log.Critical("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                    break;
            }
        }

        private void ProcessNetIncomingMessageData(NetIncomingMessage message, bool unconnected)
        {
            try
            {
                if (!unconnected)
                {
                    switch (message.SequenceChannel)
                    {
                        case 0: // Assume everything sent along channel 0 is encrypted
                            if (!message.Decrypt(_encryption))
                            {
                                Log.Critical("Unable to decrypt packet on Channel 0, please check the key that is being used.");
                            }
                            break;
                    }
                }

                //var data = Illisian.UnityUtil.Compression.ByteArrays.Decompress(message.ReadBytes(message.LengthBytes));
                var data = message.ReadBytes(message.LengthBytes);
                NetMessage _msg = new NetMessage();
                _msg.NetConnection = message.SenderConnection;
                _msg.Object = Illisian.UnityUtil.Serialise.Binary.ByteArrayToObject(data);
                _msg.SequenceChannel = message.SequenceChannel;
                _msg.UnconnectedMessage = unconnected;

                if (OnIncomingMessage != null)
                {
                    OnIncomingMessage(_msg);
                }
                else
                {
                    Log.Warn("Packets are being received but nothing is listening on the event hook");
                }
            }
            catch (Exception ex)
            {
                Log.Warn(String.Format("Unable to Process Incoming Message :: Error Message: {0}", ex.Message));
            }


        }
        public void Shutdown()
        {
            if (_config != null && _config.EnableUPnP)
            {
                Log.Info(String.Format("Disabling UPnP for Port {0}", NetPeer.Port));
                NetPeer.UPnP.DeleteForwardingRule(NetPeer.Port);
            }
            Log.Info(String.Format("Stopping NetMessage Thread on Port {0}..", NetPeer.Port));
            Running = false;
            Thread.Sleep(100);
            if (_receiveMessageWorker != null)
            {
                do
                {
                    if (_receiveMessageWorker.ThreadState == ThreadState.Running)
                    {
                        Log.Info(String.Format("Waiting for NetMessage Thread to terminate..", _config.Port));
                        Thread.Sleep(1000);
                    }

                } while (_receiveMessageWorker.ThreadState == ThreadState.Running);
            }
            OnShutdown();
        }

        public void QueueNetMessage(NetMessage msg)
        {
            if (msg.Object == null)
            {
                Log.Critical("Attempting to send a empty message", msg);
                return;
            }
            if (!msg.UnconnectedMessage && msg.NetConnection == null)
            {
                Log.Critical("Attempting to send a message to nowhere", msg);
                return;
            }

            Monitor.Enter(_outgoingMessages);
            _outgoingMessages.Enqueue(msg);
            Monitor.Exit(_outgoingMessages);
        }

        private void SendNetMessage(NetMessage msg)
        {
           // byte[] data = Illisian.UnityUtil.Compression.ByteArrays.Compress(Illisian.UnityUtil.Serialise.Binary.ObjectToByteArray(msg.Object));
            byte[] data = Illisian.UnityUtil.Serialise.Binary.ObjectToByteArray(msg.Object);

            NetOutgoingMessage sendMsg = NetPeer.CreateMessage();

            sendMsg.Write(data);
            if (!msg.UnconnectedMessage)
            {
                switch (msg.SequenceChannel)
                {
                    case 0: // Assume everything sent along channel 0 is encrypted
                        if (!sendMsg.Encrypt(_encryption))
                        {
                            Log.Critical("Unable to encrypt packet on Channel 0, please check the key that is being used.");
                        }
                        break;
                }
                NetPeer.SendMessage(sendMsg, msg.NetConnection, NetDeliveryMethod.ReliableOrdered, msg.SequenceChannel);
            }
            else
            {
                NetPeer.SendUnconnectedMessage(sendMsg, msg.DestinationIp, msg.DestinationPort);
            }
        }


    }
}
