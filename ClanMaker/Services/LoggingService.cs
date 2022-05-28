using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace ClanMaker.Services;

public sealed class LoggingService
{
    public LoggingService(DiscordSocketClient client, CommandService command, InteractionService interaction)
    {
        client.Log += OnLogging;
        
        command.Log += OnLogging;
        
        interaction.Log += OnLogging;
    }

    private Task OnLogging(LogMessage log)
    {
        switch (log.Severity)
        {
            case LogSeverity.Info:
            {
                var console = $"{DateTime.Now:HH:mm:ss} [{log.Severity}] {log.Source}: {log.Message}";

                Console.WriteLine(console);
                    
                break;
            }
            
            // 権限エラーは無視
            case LogSeverity.Error when !log.Exception.Message.Contains("Missing Permissions"):
            {
                var console = $"{DateTime.Now:HH:mm:ss} [{log.Severity}] {log.Source}: {log.Exception}";

                Console.WriteLine(console);
                    
                break;
            }

            case LogSeverity.Critical:
            {
                var console = $"{DateTime.Now:HH:mm:ss} [{log.Severity}] {log.Source}: {log.Exception}";

                Console.WriteLine(console);
                    
                break;
            }
            
            // TypeReaderの読み込み警告は無視
            case LogSeverity.Warning when !log.Exception.Message.Contains("The default TypeReader"):
            {
                var console = $"{DateTime.Now:HH:mm:ss} [{log.Severity}] {log.Source}: {log.ToString()}";

                Console.WriteLine(console);
                    
                break;
            }

            case LogSeverity.Debug:
            {
                var console = $"{DateTime.Now:HH:mm:ss} [{log.Severity}] {log.Source}: {log.Message}";

                Console.WriteLine(console);
                
                break;
            }
                
            // default: break;
        }
            
        return Task.CompletedTask;
    }
}