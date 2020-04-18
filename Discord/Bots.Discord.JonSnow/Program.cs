namespace Bots.Discord.JonSnow
{
    using System.Threading.Tasks;
    using Bots.Common.Clients;
    using Bots.Common.Services;
    using global::Discord;
    using global::Discord.WebSocket;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).AddEnvironmentVariables().Build();
            var httpService = new HttpService(new HttpClient(), configuration);
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IHttpService>(httpService)
                .AddSingleton<IJonSnowService>(new JonSnowService(configuration, httpService))
                .BuildServiceProvider();

            var discordClient = new DiscordSocketClient();
            await discordClient.LoginAsync(TokenType.Bot, configuration["DiscordAppToken"]);
            await discordClient.StartAsync();

            var eventsRepo = new EventsRepository(serviceProvider.GetService<IJonSnowService>());

            discordClient.MessageReceived += eventsRepo.MessageReceived;

            await Task.Delay(-1);
        }
    }
}
