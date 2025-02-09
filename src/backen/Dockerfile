# Используем SDK образ для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Скопировать csproj файлы и восстановить зависимости
COPY ["MdAPI/Web/Web.csproj", "Web/"]
COPY ["MdAPI/MdPWASM/MdPWASM.csproj", "MdPWASM/"]
RUN dotnet restore "Web/Web.csproj"

# Скопировать исходный код
COPY . .

# 🛠 Сборка WASM в wwwroot
WORKDIR /src/MdAPI/MdPWASM
RUN dotnet publish -c Release -o /src/MdAPI/Web/wwwroot

# 🛠 Сборка API
WORKDIR /src/MdAPI/Web
RUN dotnet publish -c Release -o /app/publish

# Используем `aspnet` runtime образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Указываем, на каком порту будет работать сервер внутри контейнера
ENV ASPNETCORE_URLS=http://0.0.0.0:5199

EXPOSE 5199
ENTRYPOINT ["dotnet", "Web.dll"]