FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY *.sln .
COPY FinanceManagement.Api/*.csproj FinanceManagement.Api/
COPY FinanceManagement.Application/*.csproj FinanceManagement.Application/
COPY FinanceManagement.Domain/*.csproj FinanceManagement.Domain/
COPY FinanceManagement.Infrastructure/*.csproj FinanceManagement.Infrastructure/
RUN dotnet restore FinanceManagement.Api/FinanceManagement.Api.csproj

COPY . .
RUN dotnet publish FinanceManagement.Api/FinanceManagement.Api.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FinanceManagement.Api.dll"]
