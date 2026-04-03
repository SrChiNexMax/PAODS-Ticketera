FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/PdsTk.Api/PdsTk.Api.csproj", "src/PdsTk.Api/"]
COPY ["src/PdsTk.Aplicacion/PdsTk.Aplicacion.csproj", "src/PdsTk.Aplicacion/"]
COPY ["src/PdsTk.Dominio/PdsTk.Dominio.csproj", "src/PdsTk.Dominio/"]
COPY ["src/PdsTk.Infraestructura/PdsTk.Infraestructura.csproj", "src/PdsTk.Infraestructura/"]

RUN dotnet restore "src/PdsTk.Api/PdsTk.Api.csproj"

COPY . .
WORKDIR "/src/src/PdsTk.Api"
RUN dotnet publish "PdsTk.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "PdsTk.Api.dll"]