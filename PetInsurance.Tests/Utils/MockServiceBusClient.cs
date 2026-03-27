using System.Text.Json;

public class MockServiceBusClient : IServiceBusClient
{
    private readonly Dictionary<string, Queue<string>> _queues = new();
    public List<string> SentMessages { get; } = new(); // 🔍 Для ассертів у тестах

    public Task<bool> SendMessageAsync<T>(string queueName, T message)
    {
        if (!_queues.ContainsKey(queueName))
            _queues[queueName] = new Queue<string>();
        
        var json = JsonSerializer.Serialize(message);
        _queues[queueName].Enqueue(json);
        SentMessages.Add(json); // 🔍 Записуємо для перевірки
        
        return Task.FromResult(true);
    }

    public Task<T?> ReceiveMessageAsync<T>(string queueName)
    {
        if (!_queues.ContainsKey(queueName) || !_queues[queueName].Any())
            return Task.FromResult(default(T));
        
        var json = _queues[queueName].Dequeue();
        var result = JsonSerializer.Deserialize<T>(json);
        return Task.FromResult(result);
    }
}