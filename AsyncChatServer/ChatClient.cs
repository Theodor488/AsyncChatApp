using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncChatServer
{
    class ChatClient
    {
        public Socket ClientSocket { get; }
        public String Id { get; }

        public ChatClient(Socket socket, String id)
        {
            ClientSocket = socket;
            Id = id;
        }
    }
}
