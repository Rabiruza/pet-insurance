using Azure.Messaging.ServiceBus;
using System.Text.Json;

public class AzureServiceBusClient : IServiceBusClient, IDisposable
{
    private readonly ServiceBusClient _client;
    private bool _disposed;

    public AzureServiceBusClient(string connectionString)
    {
        _client = new ServiceBusClient(connectionString);
    }

    public async Task<bool> SendMessageAsync<T>(string queueName, T message)
    {
        try
        {
            var sender = _client.CreateSender(queueName);
            var json = JsonSerializer.Serialize(message);
            var sbMessage = new ServiceBusMessage(json)
            {
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString()
            };
            
            await sender.SendMessageAsync(sbMessage);
            await sender.CloseAsync();
            return true;
        }
        catch
        {
            return false; // 🔍 У реальності краще логувати та перекидати виняток
        }
    }

    public async Task<T?> ReceiveMessageAsync<T>(string queueName)
    {
        var receiver = _client.CreateReceiver(queueName);
        var message = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));
        
        if (message == null) return default;
        
        var result = JsonSerializer.Deserialize<T>(message.Body.ToString());
        await receiver.CompleteMessageAsync(message);
        return result;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _client?.DisposeAsync().GetAwaiter().GetResult();
            _disposed = true;
        }
    }
}