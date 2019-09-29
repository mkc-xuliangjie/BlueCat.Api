FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["BlueCat.Api/BlueCat.Api.csproj", "BlueCat.Api/"]
RUN dotnet restore "BlueCat.Api/BlueCat.Api.csproj"
COPY . .
WORKDIR "/src/BlueCat.Api"
RUN dotnet build "BlueCat.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BlueCat.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BlueCat.Api.dll"]