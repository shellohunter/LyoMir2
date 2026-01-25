using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using SystemModule;

namespace LoginGate
{
    public class ServerManager
    {
        private static readonly ServerManager instance = new ServerManager();

        public static ServerManager Instance
        {
            get { return instance; }
        }

        private readonly IList<ServerService> _serverServices;

        
        
        
        private Channel<TMessageData> _reviceMsgList = null;

        public ServerManager()
        {
            _reviceMsgList = Channel.CreateUnbounded<TMessageData>();
            _serverServices = new List<ServerService>();
        }

        public void AddServer(ServerService serverService)
        {
            _serverServices.Add(serverService);
        }

        public void RemoveServer(ServerService serverService)
        {
            _serverServices.Remove(serverService);
        }

        public void Start()
        {
            for (var i = 0; i < _serverServices.Count; i++)
            {
                if (_serverServices[i] == null)
                {
                    continue;
                }
                _serverServices[i].Start();
            }
        }

        public void Stop()
        {
            for (var i = 0; i < _serverServices.Count; i++)
            {
                if (_serverServices[i] == null)
                {
                    continue;
                }
                _serverServices[i].Stop();
            }
        }

        public void SendQueue(TMessageData messageData)
        {
            _reviceMsgList.Writer.TryWrite(messageData);
        }

        
        
        
        public async Task ProcessReviceMessage()
        {
            while (await _reviceMsgList.Reader.WaitToReadAsync())
            {
                if (_reviceMsgList.Reader.TryRead(out var message))
                {
                    var clientSession = SessionManager.Instance.GetSession(message.MessageId);
                    clientSession?.HandleUserPacket(message);
                }
            }
        }

        public IList<ServerService> GetServerList()
        {
            return _serverServices;
        }

        public ClientThread GetClientThread()
        {
            
            
            
            
            
            if (_serverServices.Any())
            {
                var random = RandomNumber.GetInstance().Random(_serverServices.Count);
                return _serverServices[random].ClientThread;
            }
            return null;
        }
    }
}