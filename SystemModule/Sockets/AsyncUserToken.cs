using System;
using System.Net;
using System.Net.Sockets;

namespace SystemModule.Sockets
{
    
    
    
    public class AsyncUserToken : EventArgs
    {
        private Socket m_socket;//Socket
        private int m_connectionId;//内部连接ID
        private IPEndPoint m_endPoint;//终结点
        private byte[] m_receiveBuffer;//缓冲区
        private int m_count;
        private int m_offset;//偏移量
        private int m_bytesReceived;//已经接收到的字节数
        private SocketAsyncEventArgs m_readEventArgs;// SocketAsyncEventArgs读对象
        private object m_operation;

        public AsyncUserToken()
            : this(null)
        {

        }

        
        
        
        public object Operation
        {
            set { m_operation = value; }
            get { return m_operation; }
        }

        
        
        
        public byte[] ReceiveBuffer => m_receiveBuffer;

        
        
        
        public int Offset => m_offset;

        
        
        
        public int BytesReceived => m_bytesReceived;

        
        
        
        public SocketAsyncEventArgs ReadEventArgs
        {
            set { m_readEventArgs = value; }
            get { return m_readEventArgs; }
        }

        
        
        
        
        public AsyncUserToken(Socket socket)
        {
            m_readEventArgs = new SocketAsyncEventArgs();
            m_readEventArgs.UserToken = this;
            if (null != socket)
            {
                m_socket = socket;
                this.m_endPoint = (IPEndPoint)socket.RemoteEndPoint;
            }
        }

        
        
        
        public Socket Socket
        {
            get { return m_socket; }
            set
            {
                if (value != null)
                {
                    m_socket = value;
                    m_endPoint = (IPEndPoint)m_socket.RemoteEndPoint;
                }
            }
        }

        public int SocHandle => (int)Socket.Handle;

        
        
        
        public int ConnectionId//内部连接ID
        {
            get { return this.m_connectionId; }
            set { this.m_connectionId = value; }
        }

        
        
        
        public IPEndPoint EndPoint => this.m_endPoint; 

        
        
        
        public string RemoteIPaddr => EndPoint?.Address.ToString();

        
        
        
        public int RemotePort
        {
            get
            {
                if (EndPoint == null)
                {
                    return 0;
                }
                return EndPoint.Port;
            }
        }

        
        
        
        
        public void SetBytesReceived(int bytesReceived)
        {
            m_bytesReceived = bytesReceived;
        }

        
        
        
        
        
        
        public void SetBuffer(byte[] buffer, int offset, int count)
        {
            m_receiveBuffer = buffer;
            m_offset = offset;
            m_count = count;
            m_bytesReceived = 0;
        }
    }
}