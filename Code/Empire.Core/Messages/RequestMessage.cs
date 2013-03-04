using Empire.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Empire.Core.Messages
{
    public class RequestMessage
    {
        public Guid id { get; set; }
        public RequestMessageType Type { get; set; }
    }
}
