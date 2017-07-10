using Discord;
using Discord.Commands;
using RyaBot.Handlers;
using RyaBot.Models;
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

    private int voteCount = 0;

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
    public async Task PlayMusic(String url)
    {
      if (_settings.voiceClient != null)
      {
        if (await _youtube.Download(url, _settings))
          await Context.Channel.SendMessageAsync(Context.Message.Author.Mention + $" song: {_settings.playList.Last().Title} has been added to the queue.");
        else
          await Context.Channel.SendMessageAsync(Context.Message.Author.Mention + $" Error occured while downloading song, possible causes: {Environment.NewLine}- Song is too long.{Environment.NewLine}- Song does not exist.{Environment.NewLine}- Song is already in the queue.{Environment.NewLine}- No audio encoding received.");
      }
    }

    [Command("RemoveSong", RunMode = RunMode.Async)]
    public async Task RemoveMusic(int queueNr)
    {
      await Task.Run(() => {
        if (_settings.voiceClient != null && queueNr > 0 && queueNr <= _settings.playList.Count())
        {
          _settings.playList.RemoveAt(queueNr - 1);
        }
      });
    }

    [Command("Stop", RunMode = RunMode.Async)]
    public async Task StopMusic()
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
      if (_settings.voiceClient != null && _settings.currentSong != "")
      {
        voteCount++;
        int userCount = await (Context.Client.GetChannel(_voiceChannel) as IVoiceChannel).GetUsersAsync().Count();
        if (voteCount > (userCount / 2))
        {
          await _media.StopCurrentStreamAsync();
          await Context.Channel.SendMessageAsync(Context.Message.Author.Mention + $" Stopped playing {_settings.currentSong}");
        } else
        {
          await Context.Channel.SendMessageAsync($"{voteCount} people have voted to skip this song. {(userCount / 2) - voteCount} more votes required to skip this song.");
        }
      }
    }

    [Command("Queue", RunMode = RunMode.Async)]
    public async Task ShowQueue()
    {
      if (_settings.playList.Count() > 0)
      {
        var songNr = 0;
        var songList = "**Current song:** " + _settings.currentSong + "\n \n";
        songList += "**Song Queue:** \n \n";

        foreach (Song song in _settings.playList)
        {
          if (songNr == 25) break;
          songNr++;

          songList += "**" + songNr + ".** ";
          songList += song.Title + " ";
          songList += song.Duration.Minutes + ":" + song.Duration.Seconds + "\n";
        }

        var embed = new EmbedGen().Generate(songList);
        await Context.Channel.SendMessageAsync("", embed: embed);
      }
      else
        await Context.Channel.SendMessageAsync(Context.Message.Author.Mention + $" No songs in the queue.");
    }
  }
}
