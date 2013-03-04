using System;
using Lidgren.Network;

namespace Illisian.Lidgren3
{
    public struct NetMessage
    {
        private object _object;
        private int _sequenceChannel;
        private NetConnection _netConnection;
        private bool _unconnectedMessage;
        private string _destinationIp;
        private int _destinationPort;

        public bool UnconnectedMessage
        {
            get { return _unconnectedMessage; }
            set { _unconnectedMessage = value; }
        }
        public string DestinationIp
        {
            get { return _destinationIp; }
            set { _destinationIp = value; }
        }
        public int DestinationPort
        {
            get { return _destinationPort; }
            set { _destinationPort = value; }
        }

        public int SequenceChannel
        {
            get { return _sequenceChannel; }
            set { _sequenceChannel = value; }
        }
        public Type Type
        {
            get
            {

                if (_object != null)
                    return _object.GetType();
                return null;
            }
        }

        public object Object
        {
            get
            {
                return _object;
            }
            set
            {
                _object = value;
            }
        }
        public T GetObject<T>() where T : class
        {
            return _object as T;
        }
        public NetConnection NetConnection
        {
            get { return _netConnection; }
            set { _netConnection = value; }
        }
    }
}
