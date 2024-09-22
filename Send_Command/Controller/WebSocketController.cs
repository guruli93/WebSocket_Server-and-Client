using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    [HttpPost("send")]

public async Task<IActionResult> SendMessage([FromBody] List< MyDataModel> message)
{
    using (ClientWebSocket client = new ClientWebSocket())
    {
        try
        {
    //    List<MyDataModel> sendCommand = new List<MyDataModel>();
            // Connect to the WebSocket server
            await client.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
            Console.WriteLine("Connected to the server.");

            // Serialize the message to JSON
            var jsonMessage = JsonSerializer.Serialize(message);
            var bytesToSend = Encoding.UTF8.GetBytes(jsonMessage);
            await client.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);

            // Close the connection
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            Console.WriteLine("Connection closed.");

            return Ok("Message sent and connection closed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
}

// Define the model class
public class MyDataModel
{
    public string Name { get; set; }=string.Empty;
    
}

}
