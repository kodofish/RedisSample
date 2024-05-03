using StackExchange.Redis;

namespace SimpleChat;

class Program
{
    private static readonly ConnectionMultiplexer Multiplexer = ConnectionMultiplexer.Connect("localhost");
    private static readonly Queue<string> Messages = new();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter your name：");
        var userName = Console.ReadLine() ?? "Anonymous";

        var subscriber = Multiplexer.GetSubscriber();
        
        // 訂閱 chat 頻道
        await subscriber.SubscribeAsync("chat", (channel, message) =>
        {
            Messages.Enqueue($"{message}");
            DisplayMessages(Messages);
        });

        // 發佈訊息
        await subscriber.PublishAsync("chat", $"The user `{userName}` join to chat room.");

        while (true)
        {
            // 讀取使用者輸入的訊息並發佈到 chat 頻道
            var userMessage = Console.ReadLine();
            await subscriber.PublishAsync("chat", $"{userName}: {userMessage}");
        }
    }

    private static void DisplayMessages(Queue<string> messages)
    {
        Console.Clear();
        if (messages.Count > 20)
            messages.Dequeue();
        
        foreach (var message in messages.ToArray())
        {
            Console.WriteLine(message);
        }
    }
}