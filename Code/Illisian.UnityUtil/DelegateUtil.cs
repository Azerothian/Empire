using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illisian.UnityUtil
{
    public delegate void Response();
    public delegate void Response<T>(T msg1);
    public delegate void Response<T, T1>(T msg1, T1 msg2);
    public delegate void Response<T, T1, T2>(T msg1, T1 msg2, T2 msg3);
    public delegate void Response<T, T1, T2, T3>(T msg1, T1 msg2, T2 msg3, T3 msg4);
    public delegate void ResponseParams<T>(T msg1, params object[] arr);
    public delegate bool QueryReponse<T>(T obj);
}
