using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illisian.UnityUtil
{
    public abstract class ContextAbstract<T> where T : class, new()
    {
        protected static T Instance;
        public static T Context
        {
            get
            {
                if (Instance == null)
                {
                    Instance = new T();
                }
                return Instance;
            }
        }
    }
}
