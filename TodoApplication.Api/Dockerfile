FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
#EXPOSE 80
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TodoApplication.Api/TodoApplication.Api.csproj", "TodoApplication.Api/"]
RUN dotnet restore "TodoApplication.Api/TodoApplication.Api.csproj"
COPY . .
WORKDIR "/src/TodoApplication.Api"
RUN dotnet build "TodoApplication.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoApplication.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoApplication.Api.dll"]
