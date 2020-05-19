using BaseComManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace TcpComManager
{
    public class TcpInitInfo : BaseInitInfo, IBaseInitInfo
    {
        public int Port         { get; set; }
        public string IpAddress { get; set; }
    }
}
