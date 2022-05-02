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
                    "Name des Containers",
                    true);

            builder.AddOption("command",
                ApplicationCommandOptionType.String, 
                "wähle den Befehl aus", 
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

            if (string.IsNullOrEmpty(dockerName)) //Schaut ob ein Name für den Docker eingegeben wurde
            {
                await arg.ModifyOriginalResponseAsync(edit => edit.Content = "Dockername darf nicht null sein");
                return;
            }

            if (!settings.AllowedContainers.Contains(dockerName) && !settings.AdminID.Contains(arg.User.Id)) //Überprüft Berechtigungen
            {
                await arg.ModifyOriginalResponseAsync(edit => edit.Content = "Du hast nicht die Berechtigung diesen Docker zu steuern");
                return;
            }

            var docker = dockerService.DockerStatus.FirstOrDefault(docker => docker.Name == dockerName);

            if (docker == null) //Schaut ob gesuchter Docker Existiert
            {
                await arg.ModifyOriginalResponseAsync(edit => edit.Content = "Docker existiert nicht");
                return;
            }
            var command = arg.Data.Options.FirstOrDefault(option => option.Name == "command")?.Value as string;




            

            switch (command)
            {
                case "start":
                    if (dockerService.RunningDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = "Docker ist bereits gestartet");
                        return;
                    }
                    break;
                case "stop":
                case "restart":
                    if (dockerService.StoppedDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = "Docker ist gestoppt");
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
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " Docker wurde erfolgreich gestartet");
                        return;
                    }
                    else
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention +  " Docker konnte nicht gestartet werden");
                        return;
                    }
                case "stop":
                    if (dockerService.StoppedDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " Docker wurde erfolgreich gestoppt");
                        return;
                    }
                    else
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " Docker konnte nicht gestoppt werden");
                        return;
                    }

                case "restart":
                    if (dockerService.RunningDockers.Contains(dockerName))
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " Docker wurde erfolgreich neugestartet");
                        return;
                    }
                    else
                    {
                        await arg.ModifyOriginalResponseAsync(edit => edit.Content = arg.User.Mention + " Docker konnte nicht neugestartet werden");
                        return;
                    }
            }
        }
    }
}
