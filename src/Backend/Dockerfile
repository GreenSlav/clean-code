# Markdown/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Скопировать csproj и восстановить зависимости
COPY ["./Markdown/MdProcessorWebApi/MdProcessorWebApi.csproj", "./MdProcessorWebApi/"]
COPY ["./Markdown/MdPWASM/MdPWASM.csproj", "./MdPWASM/"]
RUN dotnet restore "./MdProcessorWebApi/MdProcessorWebApi.csproj"

# Скопировать исходный код и собрать WASM
COPY ./Markdown ./Markdown
WORKDIR /src/Markdown/MdPWASM
RUN dotnet publish -c Release -o /src/Markdown/MdProcessorWebApi/wwwroot

# Собрать основной API
WORKDIR /src/Markdown/MdProcessorWebApi
RUN dotnet publish -c Release -o /app/publish

# Используем runtime образ для минимизации размера контейнера
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5199
EXPOSE 5199
ENTRYPOINT ["dotnet", "MdProcessorWebApi.dll"]