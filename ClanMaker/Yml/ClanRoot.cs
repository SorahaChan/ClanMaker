namespace ClanMaker.Yml;

public class ClanRoot
{
    public List<Clan> Clans { get; set; } = new ();
}

public class Clan
{
    public Clan()
    {
    }
    
    public Clan(string clanName, string clanDescription, uint color, List<ulong> members)
    {
        ClanName = clanName;
        ClanDescription = clanDescription;
        Color = color;
        Members = members;
    }

    public string ClanName { get; set; }
    
    public string ClanDescription { get; set; }
    
    public uint Color { get; set; }
    
    public List<ulong> Members { get; set; }

    public ulong? RoleId { get; set; } = 0;

    public ulong? BattleChannelId { get; set; } = 0;

    public List<ClanProgress> Progresses { get; set; } = new ();
}

public class ClanProgress
{
    public ClanProgress(DateTime dateTime, int channelLevel)
    {
        DateTime = dateTime;
        ChannelLevel = channelLevel;
    }

    public DateTime DateTime { get; }
    
    public int ChannelLevel { get; }
}