using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Channels;

namespace GameSvr
{
    public class SendQueue
    {
        private readonly Channel<byte[]> _sendQueue = null;
        private readonly Socket _sendSocket;
        private readonly CancellationTokenSource _cancellation;

        public SendQueue(Socket socket)
        {
            _sendQueue = Channel.CreateUnbounded<byte[]>();
            _cancellation = new CancellationTokenSource();
            _sendSocket = socket;
        }

        
        
        
        public int GetQueueCount => _sendQueue.Reader.Count;

        
        
        
        public void AddToQueue(byte[] buffer)
        {
            _sendQueue.Writer.TryWrite(buffer);
        }

        public void Stop()
        {
            _cancellation.Cancel();
        }

        
        
        
        
        public async Task ProcessSendQueue()
        {
            while (await _sendQueue.Reader.WaitToReadAsync(_cancellation.Token))
            {
                if (_sendQueue.Reader.TryRead(out var buffer))
                {
                    if (_sendSocket.Connected)
                    {
                        var sendLen = _sendSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
                        if (sendLen < buffer.Length)
                        {
                            Debug.WriteLine("发送封包出现异常。");
                        }
                    }
                }
            }
        }
    }
}
