using Discord;
using Discord.Commands;
using RyaBot.Handlers;
using RyaBot.Processes;
using RyaBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Models;

namespace RyaBot.Modules
{
  public class Voice : ModuleBase<SocketCommandContext>
  {
    private readonly Settings _settings;
    private readonly Media _media;
    private readonly Youtube _youtube;
    private readonly ulong _voiceChannel = 331745615321235460;

    public Voice(Settings settings, Media media, Youtube youtube)
    {
      _settings = settings ?? throw new ArgumentNullException(nameof(Settings));
      _media = media ?? throw new ArgumentNullException(nameof(Media));
      _youtube = youtube ?? throw new ArgumentNullException(nameof(Youtube));
    }
    
    [Command("Summon", RunMode = RunMode.Async)]
    public async Task JoinChannel()
    {
      if (_settings.voiceClient == null)
      {
        var channel = Context.Client.GetChannel(_voiceChannel) as IVoiceChannel;
        _settings.voiceClient = await channel.ConnectAsync();
      }
    }
    
    [Command("Unsummon", RunMode = RunMode.Async)]
    public async Task LeaveChannel()
    {
      if (_settings.voiceClient != null)
      {
        await _media.StopCurrentStreamAsync();
        await _settings.voiceClient.StopAsync();

        _settings.voiceClient = null;
        _settings.playList.Clear();
        _settings.currentSong = "";
      }
    }
    
    [Command("Play", RunMode = RunMode.Async)]
    public async Task Playmusic(String url)
    {
      if (_settings.voiceClient != null)
      {
        if (await _youtube.Download(url, _settings))
          await Context.Channel.SendMessageAsync(Context.Message.Author.Mention + $" song: {_settings.playList.Values.Last().Title} has been added to the queue.");
        else
          await Context.Channel.SendMessageAsync(Context.Message.Author.Mention + $" Error occured while downloading song, song is either too long or doesn't exist.");
      }
    }
    
    [Command("Stop", RunMode = RunMode.Async)]
    public async Task Stopmusic()
    {
      if (_settings.voiceClient != null)
      {
        await _media.StopCurrentStreamAsync();

        _settings.playList.Clear();
        _settings.currentSong = "";

        await Context.Channel.SendMessageAsync(Context.Message.Author.Mention + $" Stopped playing music and cleared the queue.");
      }
    }

    [Command("Skip", RunMode = RunMode.Async)]
    public async Task SkipCurrentSong()
    {
      if (_settings.voiceClient != null)
      {
        await _media.StopCurrentStreamAsync();
        await Context.Channel.SendMessageAsync(Context.Message.Author.Mention + $" Stopped playing {_settings.currentSong}");
      }
    }

    [Command("Queue", RunMode = RunMode.Async)]
    public async Task ShowQueue()
    {
      if (_settings.playList.Count > 0)
      {
        var songNr = 0;
        var songList = "**Current song:** " + _settings.currentSong + "\n \n";
        songList += "**Song Queue:** \n \n";

        foreach (VideoInfo video in _settings.playList.Values)
        {
          if (songNr == 25) break;
          songNr++;

          songList += "**" + songNr + ".** ";
          songList += video.Title + " ";
          songList += video.Duration.Minutes + ":" + video.Duration.Seconds + "\n";
        }

        var embed = new EmbedGen().Generate(songList);
        await Context.Channel.SendMessageAsync("", embed: embed);
      }
      else
        await Context.Channel.SendMessageAsync(Context.Message.Author.Mention + $" No songs in the queue.");
    }
  }
}
