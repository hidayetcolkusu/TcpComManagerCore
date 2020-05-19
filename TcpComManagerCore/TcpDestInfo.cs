using BaseComManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace TcpComManager
{
    public class TcpDestInfo : BaseDestInfo, IBaseDestInfo
    {
        public int Port         { get; set; }
        public string IpAddress { get; set; }
    }
}
