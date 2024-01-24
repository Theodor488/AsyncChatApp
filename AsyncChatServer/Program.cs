using System.Net.Sockets;
using System.Net;
using System.Text;
using AsyncChatServer;

IPAddress ipAddress = IPAddress.Loopback;
IPEndPoint ipEndPoint = new(ipAddress, 8080);

using Socket listener = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);

listener.Bind(ipEndPoint);
listener.Listen(100);

// List of chat clients
List<ChatClient> clients = new List<ChatClient>();

async Task HandleClient(Socket newClientSocket)
{
    int lstLength = clients.Count;
    string clientId = $"client{lstLength}";

    ChatClient client = new ChatClient(newClientSocket, clientId);
    clients.Add(client);

    while (true)
    {
        // Receive message.
        string response = await ReceiveMessage(newClientSocket);
        var eom = "<|EOM|>";

        if (response.IndexOf(eom) > -1 /* is end of message */)
        {
            Console.WriteLine(
                $"{client.Id}: \"{response.Replace(eom, "")}\"");
            
            var ackMessage = "<|ACK|>";
            var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
            await newClientSocket.SendAsync(echoBytes, 0);
        }

        if (response.Contains("/exit"))
        {
            break;
        }
    }
}

while (true)
{
    // Accept a new client connection
    Socket newClientSocket = await listener.AcceptAsync();

    // Create a new task to handle the client
    Task.Run(() => HandleClient(newClientSocket));
}

static async Task<string> ReceiveMessage(Socket handler)
{
    var buffer = new byte[1_024];
    var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
    var response = Encoding.UTF8.GetString(buffer, 0, received);
    return response;
}