using AsyncChatClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncChatClient
{
    public class MessageHandler
    {
        internal static async Task SendMessage(Socket client, string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            _ = await client.SendAsync(messageBytes, SocketFlags.None);
        }

        internal static async Task<string?> HandleUserInput(string eom, Socket client, ConnectionStateStatus connectionState)
        {
            var userMessage = Console.ReadLine();
            userMessage += eom;
            await SendMessage(client, userMessage);

            if (userMessage.Contains("/exit"))
            {
                connectionState.EndConnection = true;
            }

            return userMessage;
        }

        internal static async Task ContinuousReceive(Socket client)
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

        internal static async Task<string> ReceiveMessage(Socket client)
        {
            var buffer = new byte[1_024];
            var received = await client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);
            return response;
        }
    }
}