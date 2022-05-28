namespace ClanMaker;

public class Program
{
    public static async Task Main()
    {
        var bot = new ClanMakerBot();

        await bot.LoginAsync();

        await Task.Delay(-1);
    }
}