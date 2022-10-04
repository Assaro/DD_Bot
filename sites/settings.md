# Overview of all Settings

![Screenshot of settings.json](/pics/Settings.png)

- LanguageSettings
    - Language: currently not working, as there is no other language than english
- DiscordSettings
    - Token: Your Discord Bot-Token
    - AdminIDs: A list of UserIDs that are allowed to control all Dockers
        - [how to find your ID](https://support.discord.com/hc/en-us/articles/206346498-Where-can-I-find-my-User-Server-Message-ID-)
    - UserWhitelist: If enabled, only users specified in UserIDs have control over AllowedContainers. Defaults to true
    - UserIDs: List of Users allowed to control Containers specified in AllowedContainers. Only active if UserWhitelist is set to true
        - [how to find your ID](https://support.discord.com/hc/en-us/articles/206346498-Where-can-I-find-my-User-Server-Message-ID-)
    - UsersCanStopContainers: If set to true, every user can stop and restart AllowedContainers. Whitelist still applies
    - AllowedContainers: List of Containers that will be listed and controllable for normal Users. **Case-Sensitive!!** Whitelist still applies
- SSHSettings
    - SshUser: Name of user for logging in to the server. Currently has to be root
    - SshPassword: Users password
    - SshPort: Your Servers SSH Port. Defaults to 22
    - ServerIP: Your Servers IP-Address, either local or public. Domain also possible
    - BotName: The Name you gave this Container. Prevents shutting down itself