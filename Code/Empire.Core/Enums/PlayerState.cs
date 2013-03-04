using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Empire.Core.Enums
{
    [Serializable]
    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Crouched,
        CrouchedWalking
    }
}
