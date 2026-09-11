# Сборка. Файлы проектов копируются отдельно от исходников,
# чтобы восстановление пакетов переиспользовало слой при правках кода.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY src/Domain/RoomRental.Domain/RoomRental.Domain.csproj src/Domain/RoomRental.Domain/
COPY src/Application/RoomRental.Application.Abstraction/RoomRental.Application.Abstraction.csproj src/Application/RoomRental.Application.Abstraction/
COPY src/Application/RoomRental.Application/RoomRental.Application.csproj src/Application/RoomRental.Application/
COPY src/Infrastructure/Dal/RoomRental.Dal.csproj src/Infrastructure/Dal/
COPY src/Infrastructure/RoomRental.Infrastructure/RoomRental.Infrastructure.csproj src/Infrastructure/RoomRental.Infrastructure/
COPY src/Infrastructure/API/RoomRental.Contracts/RoomRental.Contracts.csproj src/Infrastructure/API/RoomRental.Contracts/
COPY src/Infrastructure/API/RoomRental/RoomRental.csproj src/Infrastructure/API/RoomRental/
RUN dotnet restore src/Infrastructure/API/RoomRental/RoomRental.csproj

COPY src/ src/
RUN dotnet publish src/Infrastructure/API/RoomRental/RoomRental.csproj \
    --configuration Release --no-restore --output /publish

# Запуск
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "RoomRental.dll"]
