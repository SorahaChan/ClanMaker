using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using ICommandResult = Discord.Commands.IResult;
using IInteractionResult = Discord.Interactions.IResult;

namespace ClanMaker.Services;

public sealed class CommandHandlingService
{
    private readonly DiscordSocketClient _client;
    private readonly YmlProvider _ymlProvider;
    private readonly CommandService _command;
    private readonly InteractionService _interaction;
    private readonly IServiceProvider _provider;

    public CommandHandlingService(DiscordSocketClient client, YmlProvider ymlProvider, CommandService command,
        InteractionService interaction, IServiceProvider provider)
    {
        _client = client;
        _ymlProvider = ymlProvider;
        _command = command;
        _interaction = interaction;
        _provider = provider;

        _client.MessageReceived += OnMessageAsync;
        
        _client.SlashCommandExecuted += OnInteractionAsync;
        
        _client.ButtonExecuted += OnButtonAsync;
        
        _command.CommandExecuted += AfterCommandExecutedAsync;
        
        _interaction.SlashCommandExecuted += AfterSlashCommandExecutedAsync;
    }

    public async Task InitializeAsync()
    {
        await _command.AddModulesAsync(Assembly.GetExecutingAssembly(), _provider);

        await _interaction.AddModulesAsync(Assembly.GetExecutingAssembly(), _provider);
    }
        
    private async Task OnMessageAsync(SocketMessage socketMessage)
    {
        if (socketMessage is SocketUserMessage message)
        {
            if (message.Author.IsBot || message.Author.Id == _client.CurrentUser.Id) return;

            int argPos = 0;

            if (message.HasStringPrefix(_ymlProvider.Configuration.Prefix, ref argPos))
            {
                if (message.Channel is IPrivateChannel or IGroupChannel) return;
                
                var context = new SocketCommandContext(_client, message);
                
                await _command.ExecuteAsync(context, argPos, _provider);
            }
        }
    }

    private async Task AfterCommandExecutedAsync(Optional<CommandInfo> info, ICommandContext context, ICommandResult result)
    {
        if (result.IsSuccess) return;

        var builder = new EmbedBuilder();

        builder.WithTitle($"Command {(result.Error.HasValue ? result.Error.Value : "Exception")}");
            
        builder.WithDescription(result.ErrorReason);

        await context.Message.ReplyAsync(embed: builder.Build(), allowedMentions: AllowedMentions.None);
    }
        
    private async Task OnButtonAsync(SocketMessageComponent component)
    {
        await OnInteractionAsync(component);
    }

    private async Task OnInteractionAsync(SocketInteraction interaction)
    {
        var ctx = new SocketInteractionContext(_client, interaction);
            
        await _interaction.ExecuteCommandAsync(ctx, _provider);
    }
    
    private async Task AfterSlashCommandExecutedAsync(SlashCommandInfo info, IInteractionContext context, IInteractionResult result)
    {
        if (result.IsSuccess) return;

        var builder = new EmbedBuilder();

        builder.WithTitle($"Slash Command {(result.Error.HasValue ? result.Error.Value : "Exception")}");

        builder.WithDescription($"{result.ErrorReason}");

        await context.Interaction.RespondAsync(embed: builder.Build(), ephemeral: true);
    }
}