using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


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

public class Startup
{
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await HandleWebSocketConnection(webSocket);
            }
            else
            {
                await next();
            }
        });
    }

    // private async Task HandleWebSocketConnection(WebSocket webSocket)
    // {
    //     var buffer = new byte[1024 * 4];
    //     WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    //     while (!result.CloseStatus.HasValue)
    //     {
    //         await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

    //         result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    //     string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
    //      Console.WriteLine(string.Format("Receive data: {0}", receivedMessage) + "\r\n");
            
           
            

    //     }

    //     await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    // }


    private async Task HandleWebSocketConnection(WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];
    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    while (!result.CloseStatus.HasValue)
    {
        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Console.WriteLine($"Received data: {receivedMessage}");

        // Check if the received message is empty
        if (string.IsNullOrWhiteSpace(receivedMessage))
        {
            Console.WriteLine("Received an empty message.");
        }
        else
        {
            // Decode JSON data
           var jsonData = JsonSerializer.Deserialize<List<MyDataModel>>(receivedMessage);
                if (jsonData != null)
                {
                    foreach (var item in jsonData)
                    {
                        Console.WriteLine($"Name: {item.data}");
                    }
                }
        }

        // Echo the received message back to the client
        await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
        
        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    }

    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
}

}

public class MyDataModel
{
  public  string data{ get; set; }
}