# RyaBotDiscord

Music bot example that should help out people that are new to voice.
It can only use Youtube files and not read local files currently.
Currently only windows is supported

## Getting Started

* Clone the repo to your desktop
* Add the bot token to your environment variables (key is "token")
* Download the 3rd party files
* Build the project in Visual Studio
* Run the project and you should have a working music bot

### Prerequisites

* FFMpeg
```
You can download the FFMpeg.exe from https://www.ffmpeg.org/ And place the exe in the 3rd_party folder.
```

* Voice libraries
```
Windows 64-bit: https://dsharpplus.emzi0767.com/natives/vnext_natives_win32_x64.zip
Windows 32-bit: https://dsharpplus.emzi0767.com/natives/vnext_natives_win32_x86.zip
You need to rename libopus.dll to opus.dll before use, otherwise audio client will still complain about missing libraries.
```

* Youtube-dl
```
I use a special version of youtube-dl.exe which is also used by Nadeko bot, to find it look where nadeko gets it from :P
```

### Installing

A step by step series of examples that tell you have to get a development env running

Add an environment variable called "token" to your windows

```
key: token
value: [Insert Discord token here]
```

Get the 3rd party files

```
place the youtube-dl + ffmpeg executables in the 3rd_party folder
place libsodium.dll and opus.dll in the root of the project
Links are higher up in the readme
```

Change the channelid in Voice.cs to your own voice channel
```
change the value of _voiceChannel
```

Run the project from Visual Studio

```
If you see the bot come online you have set it up correctly
```

## Deployment

Do the same things to setup the dev environment but then make sure you use the live token

## Built With

* [Discord.NET](https://github.com/RogueException/Discord.Net) - The discord library used

## Contributing

Submit a pull request if you wanna contribute. All help is welcome

## Authors

* **Ryada** - *Initial work* - [RyadaProductions](https://github.com/RyadaProductions)
