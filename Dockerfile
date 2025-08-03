FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Task4.csproj ./
RUN dotnet restore "./Task4.csproj"

COPY . .

RUN dotnet build "Task4.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Task4.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Task4.dll"]
