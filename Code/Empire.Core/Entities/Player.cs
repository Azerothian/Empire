using Empire.Core.Enums;
using Empire.Core.Interfaces;
using Empire.Core.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Empire.Core.Entities
{
    [Serializable]
    public class Player : INetMessage
    {
        public Guid id { get; set; }
        public string Name { get; set; }
        public Vec3 Position { get; set; }
        public Quat Rotation { get; set; }
        public PlayerState State { get; set; }

    }

}
