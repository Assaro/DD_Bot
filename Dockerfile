FROM mcr.microsoft.com/dotnet/aspnet:6.0

COPY publish/ /app/
# allow all users access to this so we can run container as non root.
RUN chmod -R 775 /app
USER root

WORKDIR /app/

ENTRYPOINT ["dotnet", "DD_Bot.Bot.dll"]
