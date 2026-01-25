using System;
using SystemModule;
using SystemModule.Packages;
using SystemModule.Sockets;

namespace SelGate.Services
{
    
    
    
    public class ClientThread
    {
        private IClientScoket ClientSocket;
        
        
        
        public int ClientId = 0;
        
        
        
        public int MaxSession = 2000;
        
        
        
        public TSessionInfo[] SessionArray;
        
        
        
        public bool boCheckServerFail = false;
        
        
        
        public int CheckServerFailCount = 0;
        public bool KeepAlive;
        public int KeepAliveTick;
        public SockThreadStutas SockThreadStutas;
        
        
        
        public bool boGateReady = false;
        
        
        
        private bool isConnected = false;
        
        
        
        private readonly SessionManager _sessionManager;

        private readonly LogQueue _logQueue;

        public ClientThread(int clientId, string serverAddr, int serverPort, SessionManager sessionManager,
            LogQueue logQueue)
        {
            ClientId = clientId;
            SessionArray = new TSessionInfo[MaxSession];
            _sessionManager = sessionManager;
            _logQueue = logQueue;
            ClientSocket = new IClientScoket();
            ClientSocket.OnConnected += ClientSocketConnect;
            ClientSocket.OnDisconnected += ClientSocketDisconnect;
            ClientSocket.ReceivedDatagram += ClientSocketRead;
            ClientSocket.OnError += ClientSocketError;
            ClientSocket.Host = serverAddr;
            ClientSocket.Port = serverPort;
            SockThreadStutas = SockThreadStutas.Connecting;
            KeepAliveTick = HUtil32.GetTickCount();
            KeepAlive = true;
        }

        public bool IsConnected => isConnected;

        public string GetSocketIp()
        {
            return $"{ClientSocket.Host}:{ClientSocket.Port}";
        }

        public void Start()
        {
            ClientSocket.Connect();
        }

        public void ReConnected()
        {
            if (isConnected == false)
            {
                ClientSocket.Connect();
            }
        }

        public void Stop()
        {
            for (int i = 0; i < SessionArray.Length; i++)
            {
                if (SessionArray[i] != null && SessionArray[i].Socket != null)
                {
                    SessionArray[i].Socket.Close();
                }
            }
            ClientSocket.Disconnect();
        }

        public TSessionInfo[] GetSession()
        {
            return SessionArray;
        }

        private void ClientSocketConnect(object sender, DSCClientConnectedEventArgs e)
        {
            boGateReady = true;
            GateShare.dwCheckServerTick = HUtil32.GetTickCount();
            RestSessionArray();
            GateShare.dwCheckServerTimeMax = 0;
            GateShare.dwCheckServerTimeMax = 0;
            GateShare.ServerGateList.Add(this);
            _logQueue.Enqueue($"数据库服务器[{e.RemoteAddress}:{e.RemotePort}]链接成功.", 1);
            _logQueue.EnqueueDebugging($"线程[{Guid.NewGuid():N}]连接 {e.RemoteAddress}:{e.RemotePort} 成功...");
            isConnected = true;
            SockThreadStutas = SockThreadStutas.Connected;
            KeepAliveTick = HUtil32.GetTickCount();
        }

        private void ClientSocketDisconnect(object sender, DSCClientConnectedEventArgs e)
        {
            for (var i = 0; i < MaxSession; i++)
            {
                var userSession = SessionArray[i];
                if (userSession == null)
                {
                    continue;
                }
                if (userSession.Socket != null && userSession.Socket == e.socket)
                {
                    userSession.Socket.Close();
                    userSession.Socket = null;
                }
            }
            RestSessionArray();
            boGateReady = false;
            GateShare.ServerGateList.Remove(this);
            _logQueue.Enqueue($"数据库服务器[{e.RemoteAddress}:{e.RemotePort}]断开链接.", 1);
            isConnected = false;
        }

        
        
        
        
        
        
        private void ClientSocketRead(object sender, DSCClientDataInEventArgs e)
        {
            if (e.BuffLen <= 0)
            {
                return;
            }
            var sData = HUtil32.GetString(e.Buff, 0, e.BuffLen);
            var sText = string.Empty;
            int sSessionId = -1;
            HUtil32.ArrestStringEx(sData, "%", "$", ref sText);
            HUtil32.GetValidStr3(sText, ref sSessionId, new[] { "/" });
            var userData = new TMessageData();
            userData.SessionId = sSessionId;
            userData.Body = e.Buff;
            _sessionManager.SendQueue.TryWrite(userData);
        }

        private void ClientSocketError(object sender, DSCClientErrorEventArgs e)
        {
            switch (e.ErrorCode)
            {
                case System.Net.Sockets.SocketError.ConnectionRefused:
                    _logQueue.Enqueue("数据库服务器[" + ClientSocket.Host + ":" + ClientSocket.Port + "]拒绝链接...", 1);
                    isConnected = false;
                    break;
                case System.Net.Sockets.SocketError.ConnectionReset:
                    _logQueue.Enqueue("数据库服务器[" + ClientSocket.Host + ":" + ClientSocket.Port + "]关闭连接...", 1);
                    isConnected = false;
                    break;
                case System.Net.Sockets.SocketError.TimedOut:
                    _logQueue.Enqueue("数据库服务器[" + ClientSocket.Host + ":" + ClientSocket.Port + "]链接超时...", 1);
                    isConnected = false;
                    break;
            }
        }

        public void RestSessionArray()
        {
            for (var i = 0; i < MaxSession; i++)
            {
                if (SessionArray[i] != null)
                {
                    SessionArray[i].Socket = null;
                    SessionArray[i].dwReceiveTick = HUtil32.GetTickCount();
                    SessionArray[i].SocketId = 0;
                    SessionArray[i].ClientIP = string.Empty;
                }
            }
        }

        public void SendServerMsg(ushort nIdent, int wSocketIndex, int nSocket, ushort nUserListIndex, int nLen,
            string Data)
        {
            if (!string.IsNullOrEmpty(Data))
            {
                var strBuff = HUtil32.GetBytes(Data);
                SendServerMsg(nIdent, wSocketIndex, nSocket, nUserListIndex, nLen, strBuff);
            }
            else
            {
                SendServerMsg(nIdent, wSocketIndex, nSocket, nUserListIndex, nLen, Array.Empty<byte>());
            }
        }

        private void SendServerMsg(ushort nIdent, int wSocketIndex, int nSocket, ushort nUserListIndex, int nLen,
            byte[] Data)
        {
            var GateMsg = new PacketHeader();
            GateMsg.PacketCode = Grobal2.RUNGATECODE;
            GateMsg.Socket = nSocket;
            GateMsg.SocketIdx = (ushort)wSocketIndex;
            GateMsg.Ident = nIdent;
            GateMsg.UserIndex = nUserListIndex;
            GateMsg.PackLength = nLen;
            var sendBuffer = GateMsg.GetBuffer();
            if (Data is { Length: > 0 })
            {
                var tempBuff = new byte[20 + Data.Length];
                Array.Copy(sendBuffer, 0, tempBuff, 0, sendBuffer.Length);
                Array.Copy(Data, 0, tempBuff, sendBuffer.Length, Data.Length);
                SendSocket(tempBuff);
            }
            else
            {
                SendSocket(sendBuffer);
            }
        }

        public void SendData(string sendText)
        {
            SendSocket(HUtil32.GetBytes(sendText));
        }

        private void SendSocket(byte[] sendBuffer)
        {
            if (ClientSocket.IsConnected)
            {
                ClientSocket.Send(sendBuffer);
            }
        }
    }

    public enum SockThreadStutas : byte
    {
        Connecting = 0,
        Connected = 1,
        TimeOut = 2
    }
}