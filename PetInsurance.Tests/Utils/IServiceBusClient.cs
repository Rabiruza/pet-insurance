
public interface IServiceBusClient
{
    Task<bool> SendMessageAsync<T>(string queueName, T message);
    Task<T?> ReceiveMessageAsync<T>(string queueName);
}