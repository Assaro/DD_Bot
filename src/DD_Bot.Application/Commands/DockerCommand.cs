using System;
using Discord;
using Discord.WebSocket;
using DD_Bot.Application.Services;
using System.Linq;
using System.Threading;
using DD_Bot.Domain;

namespace DD_Bot.Application.Commands
{
    public class DockerCommand
    {
        private DiscordSocketClient _discord;

        public DockerCommand(DiscordSocketClient discord)
        {
            _discord=discord;
        }

        #region CreateCommand

        public static ApplicationCommandProperties Create() //Create-Methode mit 3 Auswahlmöglichkeiten für den Reiter Command
        {
            var builder = new SlashCommandBuilder()
            {
                Name = "docker",
                Description = "Issue a command to Docker"
            };

            builder.AddOption("dockername",
                ApplicationCommandOptionType.String,
                    "choose a container",
                    true);

            builder.AddOption("command",
                ApplicationCommandOptionType.String, 
                "choose a command", 
                true, 
                choices: new[]
                {
                    new ApplicationCommandOptionChoiceProperties()
                    {
                        Name = "start",
                        Value ="start",
                    },
                    new ApplicationCommandOptionChoiceProperties()
                    {
                        Name = "stop",
                        Value ="stop",
                    },
                    new ApplicationCommandOptionChoiceProperties()
                    {
                        Name = "restart",
                        Value ="restart",
                    },
                });

            return builder.Build();
        }

        #endregion

        #region ExecuteCommand

        public static async void Execute(SocketSlashCommand arg, DockerService dockerService, DiscordSettings settings)
        {
            await arg.RespondAsync("Contacting Docker Service...");
            await dockerService.DockerUpdate();
            
            var command = arg.Data.Options.FirstOrDefault(option => option.Name == "command")?.Value as string;

            var dockerName = arg.Data.Options.FirstOrDefault(option => option.Name == "dockername")?.Value as string;

            #region authCheck

            if (!settings.AdminIDs.Contains(arg.User.Id)) //Auth Checks
            {
                if (settings.UserWhitelist && !settings.UserIDs.Contains(arg.User.Id))
                {
                    await arg.ModifyOriginalResponseAsync(edit => edit.Content = "You are not allowed to control this docker");
                    return;
                }

                if (!settings.AllowedContainers.Contains(dockerName))
                {
                    await arg.ModifyOriginalResponseAsync(edit => edit.Content = "You are not allowed to control this docker");
                    return;
                }

                if (!settings.UsersCanStopContainers && (command == "stop"|| command == "restart"))
                {
                    await arg.ModifyOriginalResponseAsync(edit => edit.Content = "You are not allowed to stop or restart this docker");
                    return;
                }
            }

            #endregion

            if (string.IsNullOrEmpty(dockerName)) //Schaut ob ein Name für den Docker eingegeben wurde
            {
                await arg.ModifyOriginalResponseAsync(edit => edit.Content = "No name has been specified");
                return;
            }


            var docker = dockerService.DockerStatus.FirstOrDefault(docker => docker.Names[0] == dockerName);

            if (docker == null) //Schaut ob gesuchter Docker Existiert
            {
                await arg.ModifyOriginalResponseAsync(edit => edit.Content = "Docker doesnt exist");
                return;
            }

            var dockerId = docker.ID;

            switch (command)
            {
                case "start":
                    if (dockerService.RunningDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = string.Format(dockerName + " is already running"));
                        return;
                    }
                    break;
                case "stop":
                case "restart":
                    if (dockerService.StoppedDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = string.Format(dockerName + " is already stopped"));
                        return;
                    }
                    break;
            }

            switch (command)
            {
               case "start":
                   dockerService.DockerCommandStart(dockerId);
                    break;
               case "stop":
                   dockerService.DockerCommandStop(dockerId);
                    break;
               case "restart":
                   dockerService.DockerCommandRestart(dockerId);
                    break;
            }

            await arg.ModifyOriginalResponseAsync(edit =>
                edit.Content = arg.User.Mention + " Command has been sent. Awaiting response");
            
            Thread.Sleep(TimeSpan.FromSeconds(5));
            await dockerService.DockerUpdate();

            switch (command)
            {
                case "start":
                    if (dockerService.RunningDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention+ " " + dockerName + " has been started");
                        return;
                    }
                    else
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " " + dockerName + " could not be started");
                        return;
                    }
                case "stop":
                    if (dockerService.StoppedDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " " + dockerName + " has been stopped");
                        return;
                    }
                    else
                    {

                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " " + dockerName +  " could not be stopped");

                        return;
                    }
                case "restart":
                    if (dockerService.RunningDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " " + dockerName +  " has been restarted");
                        return;
                    }
                    else
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " " + dockerName +  " could not be restarted");
                        return;
                    }
            }
        }

        #endregion
    }
}