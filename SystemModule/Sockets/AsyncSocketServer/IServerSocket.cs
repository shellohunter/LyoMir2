using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SystemModule.Sockets
{
    
    
    
    public class ISocketServer
    {
        private IdWorker _idWorker;
        
        
        
        private Object m_bufferLock = new Object();
        
        
        
        private Object m_readPoolLock = new Object();
        
        
        
        private Object m_writePoolLock = new Object();
        
        
        
        ConcurrentDictionary<long, AsyncUserToken> m_tokens;
        
        
        
        int m_numConnections;
        
        
        
        int m_BufferSize;
        
        
        
        BufferManager m_bufferManager;
        
        
        
        const int opsToPreAlloc = 2;
        
        
        
        Socket listenSocket;
        
        
        
        SocketAsyncEventArgsPool m_readPool;
        
        
        
        SocketAsyncEventArgsPool m_writePool;
        bool isActive = false;
        
        
        
        
        long m_totalBytesRead;
        
        
        
        long m_totalBytesWrite;
        
        
        
        long m_numConnectedSockets;
        
        
        
        Semaphore m_maxNumberAcceptedClients;

        
        
        
        public long NumConnectedSockets
        {
            get { return m_numConnectedSockets; }
        }
        
        
        
        public long TotalBytesRead
        {
            get { return m_totalBytesRead; }
        }
        
        
        
        public long TotalBytesWrite
        {
            get { return m_totalBytesWrite; }
        }
        
        
        
        public event EventHandler<AsyncUserToken> OnClientConnect;
        
        
        
        public event EventHandler<AsyncSocketErrorEventArgs> OnClientError;
        
        
        
        public event EventHandler<AsyncUserToken> OnClientRead;
        
        
        
        public event EventHandler<AsyncUserToken> OnDataSendCompleted;
        
        
        
        public event EventHandler<AsyncUserToken> OnClientDisconnect;

        
        
        
        
        
        public bool IsOnline(int connectionId)
        {
            if (!this.m_tokens.ContainsKey(connectionId))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public IList<AsyncUserToken> GetSockets()
        {
            return this.m_tokens.Values.ToList();
        }

        
        
        
        
        
        public ISocketServer(int numConnections, int BufferSize)//构造函数
        {
            
            m_totalBytesRead = 0;
            m_totalBytesWrite = 0;
            m_numConnectedSockets = 0;
            
            m_numConnections = numConnections;
            
            m_BufferSize = BufferSize;

            

            m_bufferManager = new BufferManager(BufferSize * numConnections * opsToPreAlloc, BufferSize);

            
            m_readPool = new SocketAsyncEventArgsPool(numConnections);
            m_writePool = new SocketAsyncEventArgsPool(numConnections);

            
            m_tokens = new ConcurrentDictionary<long, AsyncUserToken>();

            
            m_maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);

            _idWorker = new IdWorker(new Random().Next(10));
        }

        
        
        
        public void Init()
        {
            
            m_bufferManager.InitBuffer();

            
            SocketAsyncEventArgs readWriteEventArg;
            AsyncUserToken token;
            

            
            for (int i = 0; i < m_numConnections; i++)
            {
                
                
                
                
                token = new AsyncUserToken();
                
                
                
                token.ReadEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                

                
                
                m_bufferManager.SetBuffer(token.ReadEventArgs);
                
                
                token.SetBuffer(token.ReadEventArgs.Buffer, token.ReadEventArgs.Offset, token.ReadEventArgs.Count);
                
                
                m_readPool.Push(token.ReadEventArgs);
            }
            
            for (int i = 0; i < m_numConnections; i++)
            {
                
                readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                readWriteEventArg.UserToken = null;

                
                m_bufferManager.SetBuffer(readWriteEventArg);
                

                
                m_writePool.Push(readWriteEventArg);
            }
        }

        
        
        
        
        
        public void Start(string ip, int port)
        {
            if (ip == "*" || ip == "all")
            {
                Start(port);
            }
            else
            {
                Start(new IPEndPoint(IPAddress.Parse(ip), port));
                isActive = true;
            }
        }

        
        
        
        
        
        public void Start(int port)
        {
            Start(new IPEndPoint(IPAddress.Any, port));
            isActive = true;
        }

        public bool Active
        {
            get { return isActive; }
        }

        
        
        
        
        private void Start(IPEndPoint localEndPoint)// 启动
        {
            try
            {
                
                if (null != listenSocket)
                {
                    listenSocket.Close();
                }
                else
                {
                    listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    listenSocket.Bind(localEndPoint);
                }
                
                listenSocket.NoDelay = false;
                listenSocket.ReceiveBufferSize = 4096;
                listenSocket.SendBufferSize = 4096;
                listenSocket.ReceiveTimeout = 20000;
                listenSocket.SendTimeout = 10000;
                listenSocket.Listen(1000);
            }
            catch (ObjectDisposedException)
            {

            }
            catch (SocketException ex)
            {
                
                
                if (ex.ErrorCode == (int)SocketError.AddressNotAvailable)
                {
                    throw new AsyncSocketException(ex.Message, ex);
                }
                else if (ex.ErrorCode == 48)
                {
                    throw new AsyncSocketException("Socket端口被占用", AsyncSocketErrorCode.ServerStartFailure);
                }
                else
                {
                    throw new AsyncSocketException("服务器启动失败", AsyncSocketErrorCode.ServerStartFailure);
                }
            }
            catch (Exception exception_debug)
            {
                Debug.WriteLine("调试：" + exception_debug.Message);
                throw exception_debug;
            }
            

            StartAccept(null); 

            Debug.WriteLine("服务器启动成功....");
        }

        
        
        
        
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                
                acceptEventArg.AcceptSocket = null;
            }
            try
            {
                

                bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
                if (!willRaiseEvent)
                {
                    ProcessAccept(acceptEventArg);
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException socketException)
            {
                RaiseErrorEvent(null, new AsyncSocketException("服务器接受客户端请求发生一次异常", socketException));
                
                
            }
            catch (Exception exception_debug)
            {
                Debug.WriteLine("调试：" + exception_debug.Message);
                throw exception_debug;
            }
        }

        
        
        
        void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            AsyncUserToken token;
            
            
            SocketAsyncEventArgs readEventArg;
            
            lock (m_readPool)
            {
                readEventArg = m_readPool.Pop();
            }

            token = (AsyncUserToken)readEventArg.UserToken;
            
            token.Socket = e.AcceptSocket;
            
            token.ConnectionId = (int)_idWorker.nextId();//Guid.NewGuid().ToString("N");
            if ((token.ConnectionId <= 0 || token.ConnectionId > ushort.MaxValue) && token.Socket != null)
            {
                token.ConnectionId = (int)token.Socket.Handle;
            }
            if (token.ConnectionId > ushort.MaxValue)
            {
                Console.WriteLine("生成SocketId异常.");
                return;
            }
            if (!this.m_tokens.TryAdd(token.ConnectionId, token)) 
            {
                Console.WriteLine("Socket链接异常");
                return;
            }

            EventHandler<AsyncUserToken> handler = OnClientConnect;
            
            handler?.Invoke(this, token);// 抛出客户端连接事件

            try
            {
                
                bool willRaiseEvent = token.Socket.ReceiveAsync(readEventArg);
                if (!willRaiseEvent)
                {
                    ProcessReceive(readEventArg);
                }
            }
            catch (ObjectDisposedException)
            {
                RaiseDisconnectedEvent(token);
            }
            catch (SocketException socketException)
            {
                if (socketException.ErrorCode == (int)SocketError.ConnectionReset)// 10054一个建立的连接被远程主机强行关闭
                {
                    RaiseDisconnectedEvent(token);// 引发断开连接事件
                }
                else
                {
                    RaiseErrorEvent(token, new AsyncSocketException("在SocketAsyncEventArgs对象上执行异步接收数据操作时发生SocketException异常", socketException));
                }
            }
            catch (Exception exception_debug)
            {
                Debug.WriteLine("调试：" + exception_debug.Message);
            }
            finally
            {
                
                StartAccept(e);
            }
        }

        
        
        
        
        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("最后一次在Socket上的操作不是接收或者发送操作");
            }
        }

        
        
        
        
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                
                Interlocked.Add(ref m_totalBytesRead, e.BytesTransferred);
                Debug.WriteLine($"服务器读取字节总数:{BytesToReadableValue(m_totalBytesRead)}");
                
                
                token.SetBytesReceived(e.BytesTransferred);

                EventHandler<AsyncUserToken> handler = OnClientRead;
                
                handler?.Invoke(this, token);// 抛出接收到数据事件                                                   

                try
                {
                    
                    var willRaiseEvent = token.Socket.ReceiveAsync(e);
                    if (!willRaiseEvent)
                    {
                        ProcessReceive(e);
                    }
                }
                catch (ObjectDisposedException)
                {
                    RaiseDisconnectedEvent(token);
                }
                catch (SocketException socketException)
                {
                    if (socketException.ErrorCode == (int)SocketError.ConnectionReset)//10054一个建立的连接被远程主机强行关闭
                    {
                        RaiseDisconnectedEvent(token);//引发断开连接事件
                    }
                    else
                    {
                        RaiseErrorEvent(token, new AsyncSocketException("在SocketAsyncEventArgs对象上执行异步接收数据操作时发生SocketException异常", socketException));
                    }
                }
                catch (Exception exception_debug)
                {
                    Debug.WriteLine("调试：" + exception_debug.Message);
                    throw exception_debug;
                }
            }
            else
            {
                RaiseDisconnectedEvent(token);
            }
        }

        public void SendAsync(int connectionId, byte[] buffer)
        {
            AsyncUserToken token;
            
            
            
            
            
            if (!this.m_tokens.TryGetValue(connectionId, out token))
            {
                throw new AsyncSocketException($"客户端:{connectionId}已经关闭或者未连接", AsyncSocketErrorCode.ClientSocketNoExist);
                
            }
            SocketAsyncEventArgs writeEventArgs;
            lock (m_writePool)
            {
                writeEventArgs = m_writePool.Pop();// 分配一个写SocketAsyncEventArgs对象
            }
            writeEventArgs.UserToken = token;
            if (buffer.Length <= m_BufferSize)
            {
                Array.Copy(buffer, 0, writeEventArgs.Buffer, writeEventArgs.Offset, buffer.Length);
                writeEventArgs.SetBuffer(writeEventArgs.Buffer, writeEventArgs.Offset, buffer.Length);
            }
            else
            {
                lock (m_bufferLock)
                {
                    m_bufferManager.FreeBuffer(writeEventArgs);
                }
                writeEventArgs.SetBuffer(buffer, 0, buffer.Length);
            }

            
            try
            {
                
                bool willRaiseEvent = token.Socket.SendAsync(writeEventArgs);
                if (!willRaiseEvent)
                {
                    ProcessSend(writeEventArgs);
                }
            }
            catch (ObjectDisposedException)
            {
                RaiseDisconnectedEvent(token);
            }
            catch (SocketException socketException)
            {
                if (socketException.ErrorCode == (int)SocketError.ConnectionReset)//10054一个建立的?颖辉冻讨骰?啃泄乇蕴
                {
                    RaiseDisconnectedEvent(token);//引发断开连接事件
                }
                else
                {
                    RaiseErrorEvent(token, new AsyncSocketException("在SocketAsyncEventArgs对象上执行异步发送数据操作时发生SocketException异常", socketException)); ;
                }
            }
            catch (Exception exception_debug)
            {
                Debug.WriteLine("调试：" + exception_debug.Message);
                throw exception_debug;
            }
        }

        
        
        
        
        
        
        public void SendAsync(int connectionId, byte[] buffer, object operation)
        {
            AsyncUserToken token;
            
            
            
            
            
            if (!this.m_tokens.TryGetValue(connectionId, out token))
            {
                throw new AsyncSocketException($"客户端:{connectionId}已经关闭或者未连接", AsyncSocketErrorCode.ClientSocketNoExist);
                
            }
            SocketAsyncEventArgs writeEventArgs;
            lock (m_writePool)
            {
                writeEventArgs = m_writePool.Pop();// 分配一个写SocketAsyncEventArgs对象
            }
            writeEventArgs.UserToken = token;
            token.Operation = operation;// 设置操作标志
            if (buffer.Length <= m_BufferSize)
            {
                Array.Copy(buffer, 0, writeEventArgs.Buffer, writeEventArgs.Offset, buffer.Length);
            }
            else
            {
                lock (m_bufferLock)
                {
                    m_bufferManager.FreeBuffer(writeEventArgs);
                }
                writeEventArgs.SetBuffer(buffer, 0, buffer.Length);
            }

            
            try
            {
                
                bool willRaiseEvent = token.Socket.SendAsync(writeEventArgs);
                if (!willRaiseEvent)
                {
                    ProcessSend(writeEventArgs);
                }
            }
            catch (ObjectDisposedException)
            {
                RaiseDisconnectedEvent(token);
            }
            catch (SocketException socketException)
            {
                if (socketException.ErrorCode == (int)SocketError.ConnectionReset)//10054一个建立的连接被远程主机强行关闭
                {
                    RaiseDisconnectedEvent(token);//引发断开连接事件
                }
                else
                {
                    RaiseErrorEvent(token, new AsyncSocketException("在SocketAsyncEventArgs对象上执行异步发送数据操作时发生SocketException异常", socketException)); ;
                }
            }
            catch (Exception exception_debug)
            {
                Debug.WriteLine("调试：" + exception_debug.Message);
                throw exception_debug;
            }
        }

        
        
        
        
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            
            Interlocked.Add(ref m_totalBytesWrite, e.BytesTransferred);
            if (e.Count > m_BufferSize)
            {
                lock (m_bufferLock)
                {
                    m_bufferManager.SetBuffer(e);// 恢复默认大小缓冲区
                }
                
            }
            lock (m_writePool)
            {
                
                m_writePool.Push(e);
            }
            
            e.UserToken = null;

            if (e.SocketError == SocketError.Success)
            {
                Debug.WriteLine($"发送总字节数:{BytesToReadableValue(e.BytesTransferred)}");
                
                
                
                
                
                
                
                
                
                EventHandler<AsyncUserToken> handler = OnDataSendCompleted;
                
                if (handler != null)
                {
                    handler(this, token);//抛出客户端发送完成事件
                }

                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
            }
            else
            {
                
                
                
                
                
                
                
                

                RaiseDisconnectedEvent(token);//引发断开连接事件
            }
        }

        public void Disconnect(int connectionId)//断开连接(形参 连接ID)
        {
            AsyncUserToken token;
            if (!this.m_tokens.TryGetValue(connectionId, out token))
            {
                throw new AsyncSocketException($"客户端:{connectionId}已经关闭或者未连接", AsyncSocketErrorCode.ClientSocketNoExist);
                
            }
            RaiseDisconnectedEvent(token);//抛出断开连接事件            
        }

        private void RaiseDisconnectedEvent(AsyncUserToken token)//引发断开连接事件
        {
            if (null != token)
            {
                if (this.m_tokens.ContainsKey(token.ConnectionId))
                {
                    this.m_tokens.TryRemove(token.ConnectionId, out token);
                    CloseClientSocket(token);
                    EventHandler<AsyncUserToken> handler = OnClientDisconnect;
                    
                    if ((handler != null) && (null != token))
                    {
                        handler(this, token);//抛出连接断开事件
                    }
                }
            }
        }

        private void CloseClientSocket(AsyncUserToken token)
        {
            
            if (token == null)
            {
                return;
            }

            
            try
            {
                token.Socket.Shutdown(SocketShutdown.Both);
                token.Socket.Close();
            }
            
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException)
            {
                token.Socket.Close();
            }
            catch (Exception exception_debug)
            {
                token.Socket.Close();
                Debug.WriteLine("调试:" + exception_debug.Message);
                throw exception_debug;
            }
            finally
            {
                
                
                
                
                lock (m_readPool)
                {
                    
                    m_readPool.Push(token.ReadEventArgs);
                }
            }
        }

        private void RaiseErrorEvent(AsyncUserToken token, AsyncSocketException exception)
        {
            EventHandler<AsyncSocketErrorEventArgs> handler = OnClientError;
            
            if (handler != null)
            {
                if (null != token)
                {
                    handler(token, new AsyncSocketErrorEventArgs(exception));//抛出客户端错误事件
                }
                else
                {
                    handler(null, new AsyncSocketErrorEventArgs(exception));//抛出服务器错误事件
                }
            }
        }

        public void Shutdown()
        {
            if (null != this.listenSocket)
            {
                this.listenSocket.Close();//停止侦听
            }
            foreach (AsyncUserToken token in this.m_tokens.Values)
            {
                try
                {
                    CloseClientSocket(token);
                    EventHandler<AsyncUserToken> handler = OnClientDisconnect;
                    
                    if ((handler != null) && (null != token))
                    {
                        handler(this, token);//抛出连接断开事件
                    }
                }
                
                
                
                catch (Exception exception_debug)
                {
                    Debug.WriteLine("调试:" + exception_debug.Message);
                }
            }
            this.m_tokens.Clear();
            isActive = false;
        }

        
        
        
        
        private string BytesToReadableValue(long length)
        {
            int byteConversion = 1024;
            double bytes = Convert.ToDouble(length);
            
            if (bytes >= Math.Pow(byteConversion, 6)) 
            {
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 6), 2), " EB");
            }
            if (bytes >= Math.Pow(byteConversion, 5)) 
            {
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 5), 2), " PB");
            }
            if (bytes >= Math.Pow(byteConversion, 4)) 
            {
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 4), 2), " TB");
            }
            if (bytes >= Math.Pow(byteConversion, 3)) 
            {
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 3), 2), " GB");
            }
            if (bytes >= Math.Pow(byteConversion, 2)) 
            {
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 2), 2), " MB");
            }
            if (bytes >= byteConversion) 
            {
                return string.Concat(Math.Round(bytes / byteConversion, 2), " KB");
            }
            return string.Concat(bytes, " Bytes");// Bytes
        }
    }
}