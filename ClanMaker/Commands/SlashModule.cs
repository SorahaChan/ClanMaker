using Discord.Interactions;

namespace ClanMaker.Commands;

public class SlashModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly YmlProvider _ymlProvider;

    public SlashModule(YmlProvider ymlProvider)
    {
        _ymlProvider = ymlProvider;
    }

    [SlashCommand("inq", "現在のクラン勢力図を表示します.")]
    public async Task DisplayInquiryAsync()
    {
        if (!_ymlProvider.BattleStatus.IsStarted)
        {
            await RespondAsync("現在クラン戦は開催されていません.", ephemeral: false);
            
            return;
        }

        if (File.Exists(AppContext.BaseDirectory + "charts/latest.png"))
        {
            await RespondWithFileAsync(AppContext.BaseDirectory + "charts/latest.png", "inquiry.png", "最新の勢力図", ephemeral: false);
        }
        else
        {
            await RespondAsync("情報が一度も更新されていません.しばらくお待ちください.", ephemeral: false);
        }
    }
}