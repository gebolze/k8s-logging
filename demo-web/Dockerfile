FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["demo-web.csproj", "demo-web.csproj"]
COPY . .
RUN dotnet build "demo-web.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "demo-web.csproj" -c Release -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "demo-web.dll"]