using Colosseum.Abstractions;
using Colosseum.Impl;
using StrategyLibrary.Impl;

namespace PlayersWebApp;

public class WebStarter
{
    private static IHostBuilder CreateBuilder(AbstractPlayer player, string url)
    {
        return Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup(w => new Startup(player));
            webBuilder.UseUrls(url);
        });
    }
    
    public static void Main(string[] args)
    {
        var app1 = CreateBuilder(new ElonMask(new PickLastCardStrategy()), "http://localhost:5031").Build();
        var app2 = CreateBuilder(new MarkZuckerberg(new PickLastCardStrategy()), "http://localhost:5032").Build();
        Task.WaitAll(app1.RunAsync(), app2.RunAsync());
    } 
}

