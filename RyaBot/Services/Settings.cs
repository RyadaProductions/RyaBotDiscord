using Discord.Audio;
using RyaBot.Models;
using System.Collections.Generic;

namespace RyaBot.Services
{
  public class Settings
  {
    public string currentSong = "";
    public List<Song> playList = new List<Song>();
    public IAudioClient voiceClient = null;
  }
}
