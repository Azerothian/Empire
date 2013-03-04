using Empire.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Empire.Core.Messages
{
    [Serializable]
    public class PlayerDisconnect : INetMessage
    {
        public Guid id { get; set; }
    }
}
