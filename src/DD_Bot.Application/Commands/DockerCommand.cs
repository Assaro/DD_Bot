using Discord;
using Discord.WebSocket;
using DD_Bot.Application.Services;
using System.Linq;
using DD_Bot.Domain;

namespace DD_Bot.Application.Commands
{
    public class DockerCommand
    {
        private DiscordSocketClient Discord;

        public DockerCommand(DiscordSocketClient discord)
        {
            Discord=discord;
        }



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
                choices: new ApplicationCommandOptionChoiceProperties[]
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

        public static async void Execute(SocketSlashCommand arg, DockerService dockerService, DiscordSettings settings)
        {
            await arg.RespondAsync("Contacting Docker Service...");

            var dockerName = arg.Data.Options.FirstOrDefault(option => option.Name == "dockername")?.Value as string;

            if (!settings.AdminIDs.Contains(arg.User.Id)) //Überprüft Berechtigungen
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
            }

            #region authCheck
            if (string.IsNullOrEmpty(dockerName)) //Schaut ob ein Name für den Docker eingegeben wurde
            {
                await arg.ModifyOriginalResponseAsync(edit => edit.Content = "No name has been specified");
                return;
            }


            var docker = dockerService.DockerStatus.FirstOrDefault(docker => docker.Name == dockerName);

            if (docker == null) //Schaut ob gesuchter Docker Existiert
            {
                await arg.ModifyOriginalResponseAsync(edit => edit.Content = "Docker doesnt exist");
                return;
            }

            #endregion

            var command = arg.Data.Options.FirstOrDefault(option => option.Name == "command")?.Value as string;

            switch (command)
            {
                case "start":
                    if (dockerService.RunningDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = "Docker is already running");
                        return;
                    }
                    break;
                case "stop":
                case "restart":
                    if (dockerService.StoppedDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = "Docker ist already stopped");
                        return;
                    }
                    break;
            }

            await dockerService.DockerCommand(command + " ", dockerName);
            await dockerService.DockerUpdate();

            switch (command)
            {
                case "start":
                    if (dockerService.RunningDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " Docker has been started");
                        return;
                    }
                    else
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention +  " Docker could not be started");
                        return;
                    }
                case "stop":
                    if (dockerService.StoppedDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " Docker has been stopped");
                        return;
                    }
                    else
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " Docker could not be stopped nicht gestoppt werden");
                        return;
                    }

                case "restart":
                    if (dockerService.RunningDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " Docker has been restarted");
                        return;
                    }
                    else
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " Docker could not be restarted");
                        return;
                    }
            }
        }
    }
}