# **How to Install DD_Bot**

1. Get a [Discord Bot](/sites/discordbot.md)
2. Install [Docker](https://www.docker.com/get-started/)
3. Pull DD_Bot Image using `docker pull assaro/ddbot:latest`
4. Start image using `docker run -d --name='DD_Bot'  -v 'path/to/settings':'/app/settings/':'rw' -v /var/run/docker.sock:/var/run/docker.sock 'assaro/ddbot:latest' `
    - For Unraid, please refer to [this Screenshot](/pics/Unraidsettings.PNG)
    - For Docker Compose- 
      1. Please create a directory such as `~/dd-bot` and `cd` to it. 
      2. Then do `wget https://github.com/Assaro/DD_Bot/raw/master/docker-compose.yml`
      3. Run `docker compose up`
      4. Push Ctrl + C and then go on to step 6 

5. The Container will generate a settings file in your set directory
6. Stop the Container, input your Discord Token + Discord Id and then start it again. [Click for info about settings](/sites/settings.md)
    - Everything but the Discord Token can be changed while the container is running
    - Everything but the Discord Token can now be changed with a command
7. You should be good to go, if not, see [discordbot](/sites/discordbot.md), [settings](/sites/settings.md) or [faq](/sites/faq.md)
8. Infrormation about commands can be found [here](/sites/commands.md)
9. If something is still not working, feel free to dm me or open an issue
10. For information about the commands, click [here](/sites/commands.md)
