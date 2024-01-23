using System.Net.Sockets;
using System.Net;
using System.Text;

IPAddress ipAddress = IPAddress.Loopback;
IPEndPoint ipEndPoint = new(ipAddress, 8080);
string eom = "<|EOM|>";

using Socket client = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);

await client.ConnectAsync(ipEndPoint);

Console.WriteLine("Chat Application. Type \"/exit\" to exit.");

// Send message. (Initialize Connection)
await SendMessage(client, $"Initializing connection (client side).{eom}");

// Receive ack.
await ReceiveAck(client);

// User send messages
while (true)
{
    Console.WriteLine("Client: ");
    var userMessage = Console.ReadLine();
    userMessage += eom;
    await SendMessage(client, userMessage);
    await ReceiveAck(client);

    if (userMessage.Contains("exit"))
    {
        break;
    }
}

client.Shutdown(SocketShutdown.Both);

static async Task SendMessage(Socket client, string message)
{
    var messageBytes = Encoding.UTF8.GetBytes(message);
    _ = await client.SendAsync(messageBytes, SocketFlags.None);
    Console.WriteLine($"Socket client sent message: \"{message.Replace("<|EOM|>", "")}\"");
}

static async Task ReceiveAck(Socket client)
{
    var buffer = new byte[1_024];
    var received = await client.ReceiveAsync(buffer, SocketFlags.None);
    var response = Encoding.UTF8.GetString(buffer, 0, received);
}