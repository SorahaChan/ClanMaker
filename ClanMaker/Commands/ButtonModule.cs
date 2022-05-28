using Discord.Interactions;
using Discord.WebSocket;

namespace ClanMaker.Commands;

public class ButtonModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly YmlProvider _ymlProvider;

    public ButtonModule(YmlProvider ymlProvider)
    {
        _ymlProvider = ymlProvider;
    }

    [ComponentInteraction("join_to_queue")]
    public async Task JoinToQueueAsync()
    {
        _ymlProvider.BattleStatus.WhitelistUsers.Add(Context.User.Id);
        
        _ymlProvider.SaveBattle();

        await RespondAsync($"{Context.User}さんのクラン戦参加処理が完了しました.", ephemeral: true);
    }
}