using System.Diagnostics;
using StackExchange.Redis;

namespace ConnectDemo;

class Program
{
    static async Task Main(string[] args)
    {
        var options = new ConfigurationOptions
        {
            EndPoints = { "localhost:6379" },
            Ssl = false,
            AbortOnConnectFail = false
        };

        var textWriter = new StringWriter();

        var connect = await ConnectionMultiplexer.ConnectAsync(options, textWriter);
        Console.WriteLine(textWriter);
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
        await textWriter.FlushAsync();
        connect.GetSubscriber().Subscribe("chat",
            (channel, message) => Console.WriteLine($"Received message: {message} on channel: {channel}"));


        while (true)
        {
            var sw = new Stopwatch();
            try
            {
                
                Console.WriteLine($"Is connected : {connect.IsConnected}");
                Console.WriteLine($"Is connecting: {connect.IsConnecting}");
                // subscriber.Publish("chat", "Hello, World!");
                sw.Start();
                var subscriber = connect.GetSubscriber();
                subscriber.Publish("chat", "Hello, World!");
                var db = connect.GetDatabase();
                var timespan = db.Ping();
                Console.WriteLine($"Message sent. Ping: {timespan.TotalMilliseconds}ms");
            }
            catch (Exception e)
            {
                sw.Stop();
                Console.WriteLine(e.GetType().FullName);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine($"Press Enter to continue...{sw.ElapsedMilliseconds} ms");
            }

            Console.ReadLine();
        }
    }
}