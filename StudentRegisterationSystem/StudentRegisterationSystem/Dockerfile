
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and projects
# Copy project files
COPY ["StudentRegisterationSystem/StudentRegisterationSystem.Server/StudentRegisterationSystem.Server.csproj", "StudentRegisterationSystem.Server/"]
COPY ["BusinessLogic/BusinessLogic.csproj", "BusinessLogic/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["StudentRegisterationSystem/StudentRegisterationSystem.Client/StudentRegisterationSystem.Client.csproj", "StudentRegisterationSystem.Client/"]


# Restore dependencies
RUN dotnet restore "StudentRegisterationSystem.Server/StudentRegisterationSystem.Server.csproj"

# Copy all files and build the project
COPY . .
WORKDIR "/src/StudentRegisterationSystem.Server"
RUN dotnet publish -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published app from build stage
COPY --from=build /app/publish .

# Expose HTTP port
EXPOSE 80

# Environment variable for connection string (optional override)
ENV ASPNETCORE_URLS=http://+:80
ENV ConnectionStrings__DefaultConnection="Server=db;Database=UniversityDB;User Id=sa;Password=Strong!Pass123;TrustServerCertificate=True;"

# Start the application
ENTRYPOINT ["dotnet", "StudentRegisterationSystem.Server.dll"]
