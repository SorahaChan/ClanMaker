using ClanMaker.Yml;
using Discord;
using Discord.Commands;

namespace ClanMaker.Commands;

public class ModModule : ModuleBase<SocketCommandContext>
{
    private readonly YmlProvider _ymlProvider;

    public ModModule(YmlProvider ymlProvider)
    {
        _ymlProvider = ymlProvider;
    }

    [Command("create")]
    public async Task CreateParticipationAsync()
    {
        if (!_ymlProvider.Configuration.ModUsers.Contains(Context.User.Id))
        {
            await Context.Message.ReplyAsync("権限不足です.", allowedMentions: AllowedMentions.None);

            return;
        }
        
        if (_ymlProvider.BattleStatus.IsStarted)
        {
            await Context.Message.ReplyAsync("現在クラン戦が開催中なのでこのコマンドは使えません.", allowedMentions: AllowedMentions.None);
            
            return;
        }
        
        var builder = new ComponentBuilder().WithButton("Join", "join_to_queue", ButtonStyle.Success);
        
        var message = await Context.Channel.SendMessageAsync("クラン戦参加ボタン", components: builder.Build());

        if (message != null)
        {
            _ymlProvider.BattleStatus.ButtonChannelId = message.Channel.Id;
            
            _ymlProvider.BattleStatus.ButtonMessageId = message.Id;
            
            _ymlProvider.SaveBattle();
        }
        else
        {
            await Context.Message.ReplyAsync("メッセージが正しく送信されませんでした.", allowedMentions: AllowedMentions.None);
        }
    }
    
    [Command("add")]
    public async Task AddClanAsync(string clanName)
    {
        if (!_ymlProvider.Configuration.ModUsers.Contains(Context.User.Id))
        {
            await Context.Message.ReplyAsync("権限不足です.", allowedMentions: AllowedMentions.None);

            return;
        }
        
        if (_ymlProvider.BattleStatus.IsStarted)
        {
            await Context.Message.ReplyAsync("現在クラン戦が開催中なのでこのコマンドは使えません.", allowedMentions: AllowedMentions.None);
            
            return;
        }

        if (_ymlProvider.ClanRoot.Clans.Any(x => x.ClanName == clanName))
        {
            await Context.Message.ReplyAsync("同名のクランが存在します.", allowedMentions: AllowedMentions.None);

            return;
        }
        
        _ymlProvider.ClanRoot.Clans.Add(new Clan(clanName, "sample description", 0, new List<ulong>()));
        
        _ymlProvider.SaveClan();
        
        await Context.Message.ReplyAsync("クランを作成しました.", allowedMentions: AllowedMentions.None);
    }
    
    [Alias("desc"), Command("description")]
    public async Task EditClanDescriptionAsync(string clanName, [Remainder] string description)
    {
        if (!_ymlProvider.Configuration.ModUsers.Contains(Context.User.Id))
        {
            await Context.Message.ReplyAsync("権限不足です.", allowedMentions: AllowedMentions.None);

            return;
        }
        
        if (_ymlProvider.BattleStatus.IsStarted)
        {
            await Context.Message.ReplyAsync("現在クラン戦が開催中なのでこのコマンドは使えません.", allowedMentions: AllowedMentions.None);
            
            return;
        }

        var clan = _ymlProvider.ClanRoot.Clans.FirstOrDefault(x => x.ClanName == clanName);
        
        if (clan is null)
        {
            await Context.Message.ReplyAsync("クランが存在しません.", allowedMentions: AllowedMentions.None);

            return;
        }

        clan.ClanDescription = description;
        
        _ymlProvider.SaveClan();
        
        await Context.Message.ReplyAsync("クランの詳細を変更しました.", allowedMentions: AllowedMentions.None);
    }
    
    [Command("color")]
    public async Task EditClanColorAsync(string clanName, uint color)
    {
        if (!_ymlProvider.Configuration.ModUsers.Contains(Context.User.Id))
        {
            await Context.Message.ReplyAsync("権限不足です.", allowedMentions: AllowedMentions.None);

            return;
        }
        
        if (_ymlProvider.BattleStatus.IsStarted)
        {
            await Context.Message.ReplyAsync("現在クラン戦が開催中なのでこのコマンドは使えません.", allowedMentions: AllowedMentions.None);
            
            return;
        }

        var clan = _ymlProvider.ClanRoot.Clans.FirstOrDefault(x => x.ClanName == clanName);
        
        if (clan is null)
        {
            await Context.Message.ReplyAsync("クランが存在しません.", allowedMentions: AllowedMentions.None);

            return;
        }

        clan.Color = color;
        
        _ymlProvider.SaveClan();
        
        await Context.Message.ReplyAsync("クランのカラーを変更しました.", allowedMentions: AllowedMentions.None);
    }
    
    [Command("remove")]
    public async Task RemoveClanAsync(string clanName)
    {
        if (!_ymlProvider.Configuration.ModUsers.Contains(Context.User.Id))
        {
            await Context.Message.ReplyAsync("権限不足です.", allowedMentions: AllowedMentions.None);

            return;
        }
        
        if (_ymlProvider.BattleStatus.IsStarted)
        {
            await Context.Message.ReplyAsync("現在クラン戦が開催中なのでこのコマンドは使えません.", allowedMentions: AllowedMentions.None);
            
            return;
        }

        var clan = _ymlProvider.ClanRoot.Clans.FirstOrDefault(x => x.ClanName == clanName);
        
        if (clan is null)
        {
            await Context.Message.ReplyAsync("クランが存在しません.", allowedMentions: AllowedMentions.None);

            return;
        }
        
        _ymlProvider.ClanRoot.Clans.Remove(clan);
        
        _ymlProvider.SaveClan();
        
        await Context.Message.ReplyAsync("クランを削除しました.", allowedMentions: AllowedMentions.None);
    }

    [Command("start")]
    public async Task StartClanBattleAsync()
    {
        if (!_ymlProvider.Configuration.ModUsers.Contains(Context.User.Id))
        {
            await Context.Message.ReplyAsync("権限不足です.", allowedMentions: AllowedMentions.None);

            return;
        }
        
        if (_ymlProvider.BattleStatus.IsStarted)
        {
            await Context.Message.ReplyAsync("現在クラン戦が開催中なのでこのコマンドは使えません.", allowedMentions: AllowedMentions.None);
            
            return;
        }

        if (_ymlProvider.BattleStatus.WhitelistUsers.Count == 0)
        {
            await Context.Message.ReplyAsync("参加予定メンバーが0人なのでこのコマンドは使えません.", allowedMentions: AllowedMentions.None);
            
            return;
        }
        
        if (_ymlProvider.ClanRoot.Clans.Count < 2)
        {
            await Context.Message.ReplyAsync("クランが2つ未満なのでこのコマンドは使えません.", allowedMentions: AllowedMentions.None);
            
            return;
        }

        var userGroups = RandomSplit(_ymlProvider.BattleStatus.WhitelistUsers, _ymlProvider.ClanRoot.Clans.Count).ToList();

        for (int i = 0; i < userGroups.Count; i++)
        {
            var group = userGroups[i];

            _ymlProvider.ClanRoot.Clans[i].Members = group.ToList();
        }

        var category = await Context.Guild.CreateCategoryChannelAsync("clan-battle");
        
        foreach (var clan in _ymlProvider.ClanRoot.Clans)
        {
            var role = await Context.Guild.CreateRoleAsync(clan.ClanName, color: new Color(clan.Color), isHoisted: true);

            var perm = new OverwritePermissions(sendMessages: PermValue.Allow, viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow);

            var everyone = new OverwritePermissions(sendMessages: PermValue.Deny, viewChannel: PermValue.Deny, readMessageHistory: PermValue.Deny);
            
            var channel = await Context.Guild.CreateTextChannelAsync($"{clan.ClanName}-battle", x =>
            {
                x.CategoryId = category.Id;
                x.PermissionOverwrites = new Overwrite[]
                {
                    new Overwrite(role.Id, PermissionTarget.Role, perm),
                    new Overwrite(Context.Guild.EveryoneRole.Id, PermissionTarget.Role, everyone)
                };
            });
            
            await Context.Guild.CreateTextChannelAsync($"{clan.ClanName}-chat", x =>
            {
                x.CategoryId = category.Id;
                x.PermissionOverwrites = new Overwrite[]
                {
                    new Overwrite(role.Id, PermissionTarget.Role, perm),
                    new Overwrite(Context.Guild.EveryoneRole.Id, PermissionTarget.Role, everyone)
                };
            });

            foreach (var member in clan.Members)
                await Context.Guild.GetUser(member).AddRoleAsync(role);

            clan.RoleId = role.Id;

            clan.BattleChannelId = channel.Id;
        }

        _ymlProvider.BattleStatus.GuildId = Context.Guild.Id;

        _ymlProvider.BattleStatus.IsStarted = true;

        _ymlProvider.BattleStatus.WhitelistUsers = new List<ulong>();
        
        _ymlProvider.SaveBattle();
        
        _ymlProvider.SaveClan();
        
        await Context.Message.ReplyAsync("クラン戦を開始しました.", allowedMentions: AllowedMentions.None);

        IEnumerable<IEnumerable<T>> RandomSplit<T>(IEnumerable<T> list, int parts)
        {
            int i = 0;

            return list.OrderBy(x => Guid.NewGuid()).GroupBy(item => i++ % parts).Select(part => part.AsEnumerable());
        }
    }

    [Command("end")]
    public async Task EndClanBattleAsync()
    {
        if (!_ymlProvider.Configuration.ModUsers.Contains(Context.User.Id))
        {
            await Context.Message.ReplyAsync("権限不足です.", allowedMentions: AllowedMentions.None);

            return;
        }
        
        if (!_ymlProvider.BattleStatus.IsStarted)
        {
            await Context.Message.ReplyAsync("現在クラン戦が開催中では無いのでこのコマンドは使えません.", allowedMentions: AllowedMentions.None);
            
            return;
        }
        
        foreach (var clan in _ymlProvider.ClanRoot.Clans)
        {
            var channel = Context.Guild.GetTextChannel(clan.BattleChannelId!.Value);

            await channel.RemovePermissionOverwriteAsync(Context.Guild.GetRole(clan.RoleId!.Value));
        }
        
        await Context.Message.ReplyAsync("クラン戦を終了しました.", allowedMentions: AllowedMentions.None);
    }
}