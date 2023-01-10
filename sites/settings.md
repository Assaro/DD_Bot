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
- DockerSettings
    - BotName: The Name you gave this Container. Prevents shutting down itself
    - Retries: The Amount of checks that should be performed before a command is considered failed
    - TimeBeforeRetry: The Time between two checks in seconds
    - ContainersPerMessage: Splits the list into multiple messages if more containers are found. Lower it to prevent Error 50035