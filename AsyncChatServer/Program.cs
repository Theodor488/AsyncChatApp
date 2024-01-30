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

// List of connected chat clients
List<ChatClient> clients = new List<ChatClient>();

// HandleClient handles individual client connections
async Task HandleClient(Socket newClientSocket)
{
    // When a new client connects, assign Id, add new client to the clients list
    string clientId = $"ID:{clients.Count}";
    string clientIdNumber = clientId.Substring(3);
    ChatClient client = new ChatClient(newClientSocket, clientIdNumber);
    clients.Add(client);

    // Send the client their ID
    var clientIdBytes = Encoding.UTF8.GetBytes(clientId);
    await client.ClientSocket.SendAsync(clientIdBytes, 0);

    clientId = $"client{clientIdNumber}";

    while (true)
    {
        // ReceiveMessage waits for messages from connected client
        string response = await ReceiveMessage(newClientSocket);
        string eom = "<|EOM|>";

        // If response not empty then broadcast message to all connected clients
        if (!string.IsNullOrEmpty(response) && response.Contains(eom) && !response.Contains("/exit"))
        {
            await BroadcastMessageToClients(clients, clientId, clientIdNumber, response, eom);
        }

        // client exit handling
        if (response.Contains("/exit"))
        {
            response = $"{clientId} has diconnected.{eom}";
            await BroadcastMessageToClients(clients, clientId, clientIdNumber, response, eom);
            break;
        }
    }
}

// Continuously listens for new client connections.
while (true)
{
    // Accept a new client connection
    Socket newClientSocket = await listener.AcceptAsync();

    // Create a new task to handle the client
    Task.Run(() => HandleClient(newClientSocket));
}

// Reads data from client socket into a buffer and returns msg as a string
static async Task<string> ReceiveMessage(Socket handler)
{
    var buffer = new byte[1_024];
    var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
    var response = Encoding.UTF8.GetString(buffer, 0, received);
    return response;
}

static async Task BroadcastMessageToClients(List<ChatClient> clients, string clientId, string clientIdNumber, string response, string eom)
{
    string clientMessage = $"{clientId}: \"{response.Replace(eom, "")}\"";
    Console.WriteLine(clientMessage);

    var clientMessageBytes = Encoding.UTF8.GetBytes(clientMessage);
    var clientPromptBytes = Encoding.UTF8.GetBytes($"{clientId}: ");

    foreach (ChatClient chatClient in clients)
    {
        // Ensure chatClient is not client that originally sent most recent msg to avoid echo
        if (chatClient.Id != clientIdNumber)
        {
            await chatClient.ClientSocket.SendAsync(clientMessageBytes, 0);
        }
    }
}