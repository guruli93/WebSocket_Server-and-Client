using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using Microsoft.OpenApi.Models;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        
      
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

   

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers(); 

          
            endpoints.MapPost("/api/send", async (List<MyDataModel> messages, CancellationToken cancellationToken) =>
            {
                var websocketUri = new Uri("ws://192.168.100.5:5000");
                using (ClientWebSocket client = new ClientWebSocket())
                {
                    try
                    {
                        await client.ConnectAsync(websocketUri, cancellationToken);
                        Console.WriteLine("Connected to the server.");

                        var jsonMessage = JsonSerializer.Serialize(messages);
                        var bytesToSend = Encoding.UTF8.GetBytes(jsonMessage);
                        await client.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, cancellationToken);

                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationToken);
                        Console.WriteLine("Connection closed.");

                        return Results.Ok("Message sent and connection closed.");
                    }
                    catch (WebSocketException wex)
                    {
                        Console.WriteLine("WebSocket error: " + wex.Message);
                        return Results.Problem("WebSocket error: " + wex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                        return Results.Problem("Internal server error: " + ex.Message);
                    }
                }
            });
        });
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

public class MyDataModel
{
    public string Name { get; set; } = string.Empty;
}
