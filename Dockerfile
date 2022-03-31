FROM mcr.microsoft.com/dotnet/aspnet:5.0

COPY publish/ /app/
# allow all users access to this so we can run container as non root.
RUN chmod -R 775 /app
RUN useradd -u 99 -g 100 dockeruser

USER dockeruser

WORKDIR /app/

ENTRYPOINT ["dotnet", "DD_Bot.Bot.dll"]
