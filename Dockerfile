FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
ENV PATH="${PATH}:/root/.dotnet/tools"
COPY ["ChatBackend.csproj", ".config", "ChatBackend/"]
WORKDIR "/src/ChatBackend"
RUN dotnet tool restore
RUN dotnet restore "ChatBackend.csproj"
COPY . .
RUN dotnet build "ChatBackend.csproj" -c Release

FROM build AS publish
RUN dotnet publish "ChatBackend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatBackend.dll"]
