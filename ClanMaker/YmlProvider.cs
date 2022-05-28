using System.Text;
using ClanMaker.Yml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;

namespace ClanMaker;

public class YmlProvider
{
    public YmlProvider()
    {
        Directory.CreateDirectory(AppContext.BaseDirectory + "yml");
        
        LoadConfiguration();
        
        LoadBattle();
        
        LoadClan();
    }

    internal Configuration Configuration { get; private set; }

    internal BattleStatus BattleStatus { get; private set; }

    internal ClanRoot ClanRoot { get; private set; }

    private void LoadConfiguration()
    {
        var stream = new StreamReader(AppContext.BaseDirectory + "yml/configuration.yml", Encoding.UTF8);
        
        try
        {
            this.Configuration = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build()
                .Deserialize<Configuration>(stream);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            stream.Close();
        }
    }

    private void LoadBattle()
    {
        if (!File.Exists(AppContext.BaseDirectory + "yml/battle.yml"))
        {
            using (StreamWriter w = File.AppendText(AppContext.BaseDirectory + "yml/battle.yml")) ;
            
            this.BattleStatus = new BattleStatus(false);
            
            SaveBattle();
            
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [Info] Stream: Create battle yaml.");

            return;
        }
        
        var stream = new StreamReader(AppContext.BaseDirectory + "yml/battle.yml", Encoding.UTF8);
        
        try
        {
            this.BattleStatus = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build()
                .Deserialize<BattleStatus>(stream);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            stream.Close();
        }
    }

    private void LoadClan()
    {
        if (!File.Exists(AppContext.BaseDirectory + "yml/clan.yml"))
        {
            StreamWriter w = File.AppendText(AppContext.BaseDirectory + "yml/clan.yml");
            
            w.Close();

            this.ClanRoot = new ClanRoot();
            
            SaveClan();
            
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [Info] Stream: Create clan yaml.");

            return;
        }
        
        var stream = new StreamReader(AppContext.BaseDirectory + "yml/clan.yml", Encoding.UTF8);
        
        try
        {
            this.ClanRoot = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build()
                .Deserialize<ClanRoot>(stream);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            stream.Close();
        }
    }
    
    public void SaveBattle()
    {
        var stream = new StreamWriter(AppContext.BaseDirectory + "yml/battle.yml", Encoding.UTF8, new FileStreamOptions
        {
            Mode = FileMode.OpenOrCreate,
            Access = FileAccess.Write,
        });
        
        try
        {
            new SerializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build()
                .Serialize(stream, this.BattleStatus);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            stream.Close();
        }
    }
    
    public void SaveClan()
    {
        var stream = new StreamWriter(AppContext.BaseDirectory + "yml/clan.yml", Encoding.UTF8, new FileStreamOptions
        {
            Mode = FileMode.OpenOrCreate,
            Access = FileAccess.Write,
        });

        try
        {
            new SerializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build()
                .Serialize(stream, this.ClanRoot);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            stream.Close();
        }
    }
}