# Faza izgradnje - sadrzi kompletan razvojni alat (SDK), prevodi i objavljuje projekat.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Igraonica.Domain/Igraonica.Domain.csproj", "src/Igraonica.Domain/"]
COPY ["src/Igraonica.Application/Igraonica.Application.csproj", "src/Igraonica.Application/"]
COPY ["src/Igraonica.Infrastructure/Igraonica.Infrastructure.csproj", "src/Igraonica.Infrastructure/"]
COPY ["src/Igraonica.WebAPI/Igraonica.WebAPI.csproj", "src/Igraonica.WebAPI/"]
RUN dotnet restore "src/Igraonica.WebAPI/Igraonica.WebAPI.csproj"

COPY . .
RUN dotnet publish "src/Igraonica.WebAPI/Igraonica.WebAPI.csproj" -c Release -o /app/publish --no-restore

# Faza izvrsavanja - znatno manje okruzenje koje sadrzi samo izvrsno okruzenje.
# Nema prevodioca ni izvornog koda, cime je povrsina za napad smanjena.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Igraonica.WebAPI.dll"]
