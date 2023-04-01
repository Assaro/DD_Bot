<h1 align="center">FAQ</h1>
<p align="center">If you are here, I probably did something wrong</p>

## I updated the Bot and my commands are not working anymore
- The 1.0.0 update introduced a new permission system
  - Either revert back to version 0.1.0 or adapt to the new system
  - Info about the new settings has been added to the [settings readme](sites/../settings.md)

## I just installed your Bot but i do not have admin rights
-You need to add your Discord ID to the settings.json. For more info about settings click [here](sites/../settings.md)

## The Bot does not go online in Discord

- Token is not valid
    - Check if you entered your token correctly in the settings.json
    - Restart the Container for Changes to take effect
    - If that did not work, regenerate the Token on Discord's Developer Portal

## The Container crashes

- Check if the Container generated a settings.json
    - If it exists, check your settings
    - If it doesn't exist, make sure that you have a folder pointing to /app/settings
- Check if `/var/run/docker.sock` exists on your machine. Adjust path to `docker.sock`

## I get ``` Unhandled exception. Discord.Net.HttpException: The server responded with error 50035: Invalid Form Body ``` when using the /list command
- Too many characters in your list. Lower the `ContainersPerMessage` setting value until it works again

## How can i contact you?
- [Github Issue](https://github.com/Assaro/DD_Bot/issues/new)
- [Discord](https://discord.com/users/341195755677286401) (Assaro Delamar#5823)
