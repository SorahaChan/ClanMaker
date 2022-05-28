using System.ComponentModel.DataAnnotations;

namespace ClanMaker.Yml;

public class BattleStatus
{
    public BattleStatus()
    {
    }
    
    public BattleStatus(bool isStarted)
    {
        IsStarted = isStarted;
        GuildId = 0;
        ButtonChannelId = 0;
        ButtonMessageId = 0;
    }

    public bool IsStarted { get; set; }

    public ulong GuildId { get; set; }

    public ulong ButtonChannelId { get; set; }

    public ulong ButtonMessageId { get; set; }

    public List<ulong> WhitelistUsers { get; set; } = new ();
}