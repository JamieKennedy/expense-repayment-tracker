FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ExpenseRepaymentTracker/ExpenseRepaymentTracker.csproj", "ExpenseRepaymentTracker/"]
RUN dotnet restore "ExpenseRepaymentTracker/ExpenseRepaymentTracker.csproj"
COPY . .
WORKDIR "/src/ExpenseRepaymentTracker"
RUN dotnet build "ExpenseRepaymentTracker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExpenseRepaymentTracker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpenseRepaymentTracker.dll"]
