<h1 align="center">FAQ</h1>
<p align="center">If you are here, I probably did something wrong</p>

## The Bot does not go online in Discord

- Token is not valid
    - Check if you entered your token correctly in the settings.json
    - Restart the Container for Changes to take effect

## The Container crashes

- Check if the Container generated a settings.json
    - If it exists, check your settings
    - If it doesn't exist, make sure that you have a folder pointing to /app/settings
- Check if /var/run/docker.sock exists on your machine. Eventually adjust path to docker.sock if it has been relocated

## I updated to a newer image and now it crashes
- I probably added new settings.
    - Back up your settings.json and let the Container generate a new one
        - This will be changed at some point, sorry for the Inconvenience

## I get ``` Unhandled exception. Discord.Net.HttpException: The server responded with error 50035: Invalid Form Body ``` when using the /list command
- Too many characters in your list. Lower the ContainersPerList setting value until it works again

## How can i contact you
- [Github Issue](https://github.com/Assaro/DD_Bot/issues/new)
- [Discord](https://discord.com/users/341195755677286401) (Assaro Delamar#5823)
