FROM mcr.microsoft.com/dotnet/aspnet:6.0

COPY publish/ /app/
# allow all users access to this so we can run container as non root.
RUN chmod -R 775 /app
RUN useradd -u 99 -g 100 dockeruser
RUN curl -fsSLO https://get.docker.com/builds/Linux/x86_64/docker-17.03.1-ce.tgz 233 &&
tar --strip-components=1 -xvzf docker-17.03.1-ce.tgz -C /usr/local/bin
RUN chown -R dockeruser:dockeruser /var/run/docker.sock
USER dockeruser

WORKDIR /app/

ENTRYPOINT ["dotnet", "DD_Bot.Bot.dll"]
