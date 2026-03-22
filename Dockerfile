# Imagen de construcción SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo del proyecto y restaurar paquetes NuGet
COPY ["UpscaleWeb.csproj", "."]
RUN dotnet restore "./UpscaleWeb.csproj"

# Copiar el resto de los archivos y compilar
COPY . .
RUN dotnet build "UpscaleWeb.csproj" -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "UpscaleWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final de ejecución ASP.NET Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Exponer el puerto por defecto de .NET 8 (8080)
EXPOSE 8080
ENTRYPOINT ["dotnet", "UpscaleWeb.dll"]
