# Сборка. Файлы проектов копируются отдельно от исходников,
# чтобы восстановление пакетов переиспользовало слой при правках кода.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Domain/RoomRental.Domain/RoomRental.Domain.csproj Domain/RoomRental.Domain/
COPY Application/RoomRental.Application.Abstraction/RoomRental.Application.Abstraction.csproj Application/RoomRental.Application.Abstraction/
COPY Application/RoomRental.Application/RoomRental.Application.csproj Application/RoomRental.Application/
COPY Infrastructure/Dal/RoomRental.Dal.csproj Infrastructure/Dal/
COPY Infrastructure/RoomRental.Infrastructure/RoomRental.Infrastructure.csproj Infrastructure/RoomRental.Infrastructure/
COPY Infrastructure/API/RoomRental.Contracts/RoomRental.Contracts.csproj Infrastructure/API/RoomRental.Contracts/
COPY Infrastructure/API/RoomRental/RoomRental.csproj Infrastructure/API/RoomRental/
RUN dotnet restore Infrastructure/API/RoomRental/RoomRental.csproj

COPY Domain/ Domain/
COPY Application/ Application/
COPY Infrastructure/ Infrastructure/
RUN dotnet publish Infrastructure/API/RoomRental/RoomRental.csproj \
    --configuration Release --no-restore --output /app

# Запуск
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "RoomRental.dll"]
