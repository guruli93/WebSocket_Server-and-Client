using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        using (ClientWebSocket client = new ClientWebSocket())
        {
            try
            {
                // Connect to the WebSocket server
                await client.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
                Console.WriteLine("Connected to the server.");

while (true){
                // Send a message to the server
                string message = "Hello, server9888888888888888888888888888!";
                var bytesToSend = Encoding.UTF8.GetBytes(message);
                await client.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);
                // Console.WriteLine("Message sent: " + message);
                Task.Delay(2000).Wait();
            }
                // Receive messages from the server
                var buffer = new byte[1024];
                WebSocketReceiveResult result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("Message received: " + receivedMessage);

                // Close the connection
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                Console.WriteLine("Connection closed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}
