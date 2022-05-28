using System.Text.RegularExpressions;
using ClanMaker.Yml;
using Discord;
using Discord.WebSocket;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Svg;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace ClanMaker.Services;

public class UpdatePlotService
{
    private readonly DiscordSocketClient _client;
    private readonly YmlProvider _ymlProvider;

    private readonly System.Timers.Timer _timer;
    
    private const string MessageRegex =
        @"属性:(?<Element>\[.+]) ランク:(?<Rank>【.+】)\s+(?<Name>.+)が待ち構えている...！\s+Lv.(?<Level>([0-9]+,?)+)  HP: (?<Hp>([0-9]+,?)+) 素早さ: (?<Speed>([0-9]+,?)+)";

    public UpdatePlotService(DiscordSocketClient client, YmlProvider ymlProvider)
    {
        _client = client;
        _ymlProvider = ymlProvider;

        _timer = new System.Timers.Timer(300_000);
 
        _timer.Elapsed += (sender, e) =>
        {
            var _ = Task.Run(async () =>
            {
                await UpdateInquiryAsync();
            });
        };
 
        _timer.Start();
    }

    private async Task UpdateInquiryAsync()
    {
        if (!_ymlProvider.BattleStatus.IsStarted) return;
        
        var guild = _client.GetGuild(_ymlProvider.BattleStatus.GuildId);
        
        foreach (var clan in _ymlProvider.ClanRoot.Clans)
        {
            var channel = guild.GetTextChannel(clan.BattleChannelId!.Value);

            foreach (var message in await channel.GetMessagesAsync(30).FlattenAsync())
            {
                foreach (var embed in message.Embeds)
                {
                    var match = Regex.Match(embed.Description, MessageRegex);

                    if (match.Success)
                    {
                        var progress = new ClanProgress(DateTime.UtcNow, Convert.ToInt32(match.Groups["Level"].Value.Replace(",", "")));
                        
                        clan.Progresses.Add(progress);
                    }
                }
            }
        }

        _ymlProvider.SaveClan();
        
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} [Info] Plot: Save level inquiry.");
        
        var model = new PlotModel
        {
            DefaultFont = "「Yu Gothic UI",
            Title = "Plot",
            Background = OxyColors.White
        };
        
        model.Axes.Add(new DateTimeAxis
        {
            Position = AxisPosition.Bottom,
            // Minimum = DateTimeAxis.ToDouble(timeSorted.First().DateTime),
            // Maximum = DateTimeAxis.ToDouble(timeSorted.Last().DateTime)
        });
        
        model.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            // Minimum = levelSorted.First().ChannelLevel,
            // Maximum = levelSorted.Last().ChannelLevel
        });

        // 階層データ保存してから描画
        foreach (var clan in _ymlProvider.ClanRoot.Clans)
        {
            // var timeSorted = clan.Progresses.OrderBy(x => x.DateTime).ToList();
            // var levelSorted = clan.Progresses.OrderBy(x => x.ChannelLevel).ToList();

            var series = new LineSeries { Title = clan.ClanName, Color = OxyColor.FromUInt32(clan.Color) };
            foreach (var progress in clan.Progresses)
            {
                series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(progress.DateTime), progress.ChannelLevel));
            }
            
            model.Series.Add(series);
        }
        
        Directory.CreateDirectory(AppContext.BaseDirectory + "charts");

        var utcNow = DateTime.UtcNow;
        
        File.Move(AppContext.BaseDirectory + "charts/latest.svg", AppContext.BaseDirectory + $"charts/{utcNow}.svg");
            
        using (var stream = File.Create(AppContext.BaseDirectory + "charts/latest.svg"))
        {
            var exporter = new SvgExporter { Width = 600, Height = 400 };
            exporter.Export(model, stream);
        }
            
        var svgDocument = SvgDocument.Open(AppContext.BaseDirectory + "charts/latest.svg");
        
        var bitmap = svgDocument.Draw();
        
        bitmap.Save(AppContext.BaseDirectory + "charts/latest.png", ImageFormat.Png);
        
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} [Info] Plot: Save chart.");
    }
}