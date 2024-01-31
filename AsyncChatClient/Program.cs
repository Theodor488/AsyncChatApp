using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Data;
using AsyncChatClient;
using AsyncChatServer;

IPAddress ipAddress = IPAddress.Loopback;
IPEndPoint ipEndPoint = new(ipAddress, 8080);
string eom = "<|EOM|>";
ConnectionStateStatus connectionState = new ConnectionStateStatus();
string clientId = "";

using Socket client = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);

// Initialize Connection. (Send and Receive Ack message)
await client.ConnectAsync(ipEndPoint);
await MessageHandler.SendMessage(client, $"Has entered the chat.{eom}");

string receivedMessage = await MessageHandler.ReceiveMessage(client);

// if receivedMessage is ID then assign client's ID
if (receivedMessage.Contains("ID:"))
{
    clientId = receivedMessage.Substring(3);
}

Console.WriteLine($"Client{clientId}");
Console.WriteLine("Chat Application. Type \"/exit\" to exit.\n");

while (true)
{
    // Start Receiving messages continuously
    var receiveTask = MessageHandler.ContinuousReceive(client);

    // Handle user input concurrently
    var userInputTask = MessageHandler.HandleUserInput(eom, client, connectionState);

    // Wait for either receiveTask or userInputTask to complete
    await Task.WhenAny(receiveTask, userInputTask);

    if (connectionState.EndConnection == true) break;
}

client.Shutdown(SocketShutdown.Both);