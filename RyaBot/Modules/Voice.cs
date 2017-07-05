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
    private Settings settings;
    private Media media;
    private Youtube youtube;
    private ulong voiceChannel = 331745615321235460;

    public Voice(Settings settings, Media media, Youtube youtube)
    {
      this.settings = settings;
      this.media = media;
      this.youtube = youtube;
    }
    
    [Command("Summon", RunMode = RunMode.Async)]
    public async Task JoinChannel()
    {
      if (settings.voiceClient == null)
      {
        var channel = Context.Client.GetChannel(voiceChannel) as IVoiceChannel;
        settings.voiceClient = await channel.ConnectAsync();
      }
    }
    
    [Command("Unsummon", RunMode = RunMode.Async)]
    public async Task LeaveChannel()
    {
      if (settings.voiceClient != null)
      {
        await media.StopStreamAsync();
        await settings.voiceClient.StopAsync();

        settings.voiceClient = null;
        settings.playList.Clear();
        settings.currentSong = "";
      }
    }
    
    [Command("Play", RunMode = RunMode.Async)]
    public async Task Playmusic(String url)
    {
      if (settings.voiceClient != null)
      {
        if (await youtube.Download(url, settings))
          await Context.Channel.SendMessageAsync(Context.Message.Author.Mention + $" song: {settings.playList.Values.Last().Title} has been added to the queue.");
        else
          await Context.Channel.SendMessageAsync(Context.Message.Author.Mention + $"Error occured while downloading song, song is either too long or doesn't exist.");
      }
    }
    
    [Command("Stop", RunMode = RunMode.Async)]
    public async Task Stopmusic()
    {
      if (settings.voiceClient != null)
      {
        await media.StopStreamAsync();

        settings.playList.Clear();
        settings.currentSong = "";

        await Context.Channel.SendMessageAsync(Context.Message.Author.Mention + $"Stopped playing music and cleared the queue.");
      }
    }
    
    [Command("Queue", RunMode = RunMode.Async)]
    public async Task ShowQueue()
    {
      if (settings.playList.Count > 0)
      {
        int songNr = 0;
        string songList = "**Current song:** " + settings.currentSong + "\n \n";
        songList += "**Song Queue:** \n \n";

        foreach (VideoInfo video in settings.playList.Values)
        {
          if (songNr == 25) break;
          songNr++;

          songList += "**" + songNr + ".** ";
          songList += video.Title + " ";
          songList += video.Duration.Minutes + ":" + video.Duration.Seconds + "\n";
        }

        Embed embed = new EmbedGen().Generate(songList);
        await Context.Channel.SendMessageAsync("", embed: embed);
      }
      else
        await Context.Channel.SendMessageAsync(Context.Message.Author.Mention + $"No messages in the queue.");
    }
  }
}
