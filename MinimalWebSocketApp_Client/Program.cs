using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);


// Add WebSocket support
builder.Services.AddWebSockets(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(120);


    
});

static void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        // Register Swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebSocket API", Version = "v1" });
        });
    }

var app = builder.Build();

app.UseWebSockets();

app.MapPost("/api/send", async (List<MyDataModel> messages) =>
{
    using (ClientWebSocket client = new ClientWebSocket())
    {
        try
        {
            // Connect to the WebSocket server
            await client.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
            Console.WriteLine("Connected to the server.");

            // Serialize the message to JSON
            var jsonMessage = JsonSerializer.Serialize(messages);
            var bytesToSend = Encoding.UTF8.GetBytes(jsonMessage);
            await client.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);

            // Close the connection
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            Console.WriteLine("Connection closed.");

            return Results.Ok("Message sent and connection closed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return Results.Problem("Internal server error: " + ex.Message);
        }
    }
});



public class MyDataModel
{
    public string Name { get; set; } = string.Empty;
}
