<h1 align="center">DD_Bot</h1>

<p align="center">A Discord-Bot to start and stop Docker Containers, using the Docker Socket</p>

<p align="center">
<img alt="Docker Image Size (tag)" src="https://img.shields.io/docker/image-size/assaro/ddbot/latest?style=for-the-badge">
<img alt="GitHub commit activity" src="https://img.shields.io/github/commit-activity/m/assaro/DD_Bot?color=brightgreen&style=for-the-badge">
<img alt="Docker Pulls" src="https://img.shields.io/docker/pulls/assaro/ddbot?style=for-the-badge"></p>

`"Conveniently, the program itself can be used as a Docker Container"` - ***Gadget Gabe***

## Screenshots

![Show Status of Containers](pics/Listcommand.png)
![Structured Settings File](pics/Settings.png)
![Send Command to Server](pics/Dockercommand.png)
![Bot's reply to command](pics/Dockerstart.png)

## Features

- Remotely start and stop Docker Containers using Discord Slash Commands
- Enable Friends to start specified Containers, e.g. Gameservers
    - Save Energy when noone is playing
- DD_Bot is designed to work on the same machine in its own Container
- Easy configuration through a single json file
- Built using [Discord.NET](https://github.com/discord-net/Discord.Net) and [Docker.DotNet](https://github.com/dotnet/Docker.DotNet)

## Requirements

- Docker
- a correctly configured bot from [Discord Developer Portal](https://discord.com/developers/), instructions can be found [here](/sites/discordbot.md)
- Internet connection

## [Installation](/sites/installation.md)

## [Settings](/sites/settings.md)

## [FAQ/Troubleshooting](/sites/faq.md)

## To-Do List

- [x] Initial release
- [x] Rewrite for docker sockets
- [x] Auto-updates for the settings.json Files
- [ ] Commands to grant and revoke privileges to users and groups
- [ ] Fully customizable messages for Discord
- [ ] More statistics
- [ ] \(Maybe) Auto-Shutdown for certain containers
- [ ] \(Maybe) more command options
- [ ] \(Maybe) implement RCON to control gameservers


### If you like my work, feel free to buy me a coffee
<p>
<br><a href="https://www.buymeacoffee.com/assaro"> <img align="left" src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" height="50" width="210" alt="assaro" /></a></p>
