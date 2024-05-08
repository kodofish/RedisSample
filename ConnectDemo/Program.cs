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
        
        await ConnectionMultiplexer.ConnectAsync(options, textWriter);
        Console.WriteLine(textWriter);
        Console.ReadLine();

    }
}