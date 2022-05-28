using ClanMaker.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using RunMode = Discord.Commands.RunMode;

namespace ClanMaker;

public class ClanMakerBot
{
    private readonly DiscordSocketClient _client;
    private readonly YmlProvider _ymlProvider;
    private const GatewayIntents Intents = GatewayIntents.GuildMembers | GatewayIntents.GuildMessages | GatewayIntents.Guilds;

    public ClanMakerBot()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            AlwaysDownloadUsers = true,
            GatewayIntents = Intents,
            MessageCacheSize = 1000
        });
        
        _ymlProvider = new YmlProvider();
    }

    public async Task LoginAsync()
    {
        ServiceProvider provider = new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_ymlProvider)
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
            }))
            .AddSingleton<InteractionService>()
            .AddSingleton<LoggingService>()
            .AddSingleton<CommandHandlingService>()
            .AddSingleton<UpdatePlotService>()
            .BuildServiceProvider();

        provider.GetRequiredService<LoggingService>();

        await provider.GetRequiredService<CommandHandlingService>().InitializeAsync();

        provider.GetRequiredService<UpdatePlotService>();

        await _client.LoginAsync(TokenType.Bot, _ymlProvider.Configuration.Token);

        await _client.StartAsync();
    }
}