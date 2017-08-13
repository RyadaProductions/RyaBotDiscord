using Discord.Audio;
using System.Collections.Generic;

namespace RyaBot.Models
{
  public class Settings
  {
    public string CurrentSong = "";
    public List<Song> PlayList = new List<Song>();
    public IAudioClient VoiceClient = null;
  }
}
