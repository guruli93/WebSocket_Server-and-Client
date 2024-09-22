using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

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
 
            await client.ConnectAsync(new Uri("ws://127.0.0.1:5000"), CancellationToken.None);
            Console.WriteLine("Connected to the server.");

        
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

public class MyDataModel
{
    public string Name { get; set; }=string.Empty;
    
}

}
