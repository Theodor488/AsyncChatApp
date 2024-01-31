using System.Net.Sockets;
using System.Net;
using System.Text;
using AsyncChatServer;

// Server Initialization
IPAddress ipAddress = IPAddress.Loopback;
IPEndPoint ipEndPoint = new(ipAddress, 8080);

// Listener Socket creation. Binds to ipEndPoint and listens for incoming connection requests
using Socket listener = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);

listener.Bind(ipEndPoint);
listener.Listen(100);

// Continuously listens for new client connections.
while (true)
{
    // Accept a new client connection
    Socket newClientSocket = await listener.AcceptAsync();
    ChatServerManager chatServerManager = new ChatServerManager();

    // Create a new task to handle the client
    Task.Run(() => chatServerManager.HandleClient(newClientSocket));
}