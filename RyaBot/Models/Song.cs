using System;

namespace RyaBot.Models
{
  public class Song
  {
    public string Path;
    public string Title;
    public TimeSpan Duration;

    
    public override int GetHashCode()
    {
      return Title.GetHashCode();
    }

    public override bool Equals(Object obj)
    {
      var otherSong = obj as Song;
      return otherSong != null && otherSong.Title == Title;
    }
  }
}
