using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketClient
{
    private readonly ClientWebSocket _clientWebSocket = new ClientWebSocket();

    public async Task ConnectAsync(string uri)
    {
        await _clientWebSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
        Console.WriteLine("Connected to WebSocket server");
    }

    public async Task SendMessageAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        await _clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        Console.WriteLine($"Sent: {message}");
    }

    public async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        while (_clientWebSocket.State == WebSocketState.Open)
        {
            result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received: {receivedMessage}");
        }
    }

    public async Task CloseAsync()
    {
        await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        Console.WriteLine("WebSocket closed");
    }
}
