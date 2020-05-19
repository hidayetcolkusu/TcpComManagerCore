using BaseComManager;
using PackageManager;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpComManager
{
    public class TcpListener : BaseListener<TcpDestInfo, TcpInitInfo>, IBaseListener<TcpDestInfo, TcpInitInfo>
    {
        #region Events

        public override event NetworkPackageReceived PackageReceived;
        public override event NetworkBytesReceived BytesPackageReceived;
        public override event NetworkTextReceived TextReceived;
        public override event SendPackage<TcpDestInfo> SendPackage;
        public override event SendMessage<TcpDestInfo> SendMessage;
        public override event SendByteArray<TcpDestInfo> SendByteArray;

        #endregion

        #region Properties

        public string IpAddresses { get; set; }
        public int Port { get; set; }

        private System.Net.Sockets.TcpListener _tcpListener;
        private System.Net.Sockets.TcpClient _tcpClient;

        private NetworkPackageGenerator _packageGenerator; 

        #endregion

        public TcpListener()
        {

        }

        public override void Initialize(NetworkPackageGenerator packageGenerator, TcpInitInfo initInfo) 
        {
            _tcpListener = new System.Net.Sockets.TcpListener(IPAddress.Parse(initInfo.IpAddress), initInfo.Port);
            _packageGenerator = packageGenerator;
            IpAddresses = initInfo.IpAddress;
            Port = initInfo.Port;
        }

        public override void Connect()
        {
            _tcpListener.Start();
            Task.Run(() => StartReceive());
        }

        public override void Disconnect()
        {
            _tcpListener.Stop();
        }

        public override bool Send(string message, TcpDestInfo param)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            return SendBytes(bytes, param.IpAddress, param.Port);
        }

        public override bool Send(byte[] bytes, TcpDestInfo param)
        {
            return SendBytes(bytes, param.IpAddress, param.Port);
        }

        public override bool Send(NetworkPackage package, TcpDestInfo param)
        {
            byte[] bytes = package.GenerateByteArray();
            return SendBytes(bytes, param.IpAddress, param.Port); 
        }

        public override bool SendFromApi(string message, TcpDestInfo param)
        {
            return this.SendMessage(message, param);
        }

        public override bool SendFromApi(byte[] bytes, TcpDestInfo param)
        {
            return this.SendByteArray(bytes, param);
        }

        public override bool SendFromApi(NetworkPackage package, TcpDestInfo param)
        {
            return this.SendPackage(package, param);
        }




        private void StartReceive()
        {
            while (true)
            {
                _tcpClient = _tcpListener.AcceptTcpClient();
                byte[] bytes = new byte[1024];
                NetworkStream stream = _tcpClient.GetStream();
                stream.Read(bytes, 0, bytes.Length);
                Receive(bytes);
            }
        }
         

        private void Receive(byte[] bytes)
        {
            ReceivePackage(bytes);
            ReceiveBytes(bytes);
            ReceiveText(bytes);
        }

        private void ReceivePackage(byte[] bytes)
        {
            try
            {
                //NetworkPackage networkPackage = _packageGenerator.Generate(bytes);
                //PackageReceived?.Invoke(networkPackage);
            }
            catch
            {

            }
        }

        private void ReceiveBytes(byte[] bytes)
        {
            try
            {
                BytesPackageReceived?.Invoke(bytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ReceiveText(byte[] bytes)
        {
            try
            {
                TextReceived?.Invoke(Encoding.ASCII.GetString(bytes));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool SendBytes(byte[] bytes, string ipAddress, int port)
        {
            TcpClient client = new TcpClient(ipAddress, port);
            NetworkStream stream = client.GetStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
            client.Close(); 
            return true;
        }

    }
}
