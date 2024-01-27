using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Data;
using AsyncChatClient;

IPAddress ipAddress = IPAddress.Loopback;
IPEndPoint ipEndPoint = new(ipAddress, 8080);
string eom = "<|EOM|>";
ConnectionStateStatus connectionState = new ConnectionStateStatus();

using Socket client = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);

await client.ConnectAsync(ipEndPoint);

Console.WriteLine("Chat Application. Type \"/exit\" to exit.");

// Send message. (Initialize Connection)
await SendMessage(client, $"Has entered the chat.{eom}");

// Receive ack / Message.
await ReceiveMessage(client);

// User send messages
while (true)
{
    var receiveTask = ReceiveMessage(client);
    var sendTask = HandleUserInput(eom, client, connectionState);

    await Task.WhenAny(receiveTask, sendTask);

    if (connectionState.EndConnection == true) break;
}

client.Shutdown(SocketShutdown.Both);

static async Task SendMessage(Socket client, string message)
{
    var messageBytes = Encoding.UTF8.GetBytes(message);
    _ = await client.SendAsync(messageBytes, SocketFlags.None);
    Console.WriteLine($"Socket client sent message: \"{message.Replace("<|EOM|>", "")}\"");
}

static async Task<string?> HandleUserInput(string eom, Socket client, ConnectionStateStatus connectionState)
{
    Console.WriteLine("Client: ");
    var userMessage = Console.ReadLine();
    userMessage += eom;
    await SendMessage(client, userMessage);

    if (userMessage.Contains("/exit"))
    {
        connectionState.EndConnection = true;
    }

    return userMessage;
}

static async Task ContinuousReceive(Socket client)
{
    try
    {
        while (true)
        {
            var response = await ReceiveMessage(client);
            Console.WriteLine(response);
        }
    }
    catch (Exception ex) 
    {
        Console.WriteLine("Error in receiving data: " + ex.ToString());
    }
}

static async Task<string> ReceiveMessage(Socket client)
{
    var buffer = new byte[1_024];
    var received = await client.ReceiveAsync(buffer, SocketFlags.None);
    var response = Encoding.UTF8.GetString(buffer, 0, received);
    return response;
}