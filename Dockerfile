 
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

 
COPY ["CMS.API/CMS.API.csproj", "CMS.API/"]
COPY ["CMS.BLL/CMS.BLL.csproj", "CMS.BLL/"]
COPY ["CMS.DAL/CMS.DAL.csproj", "CMS.DAL/"]
RUN dotnet restore "CMS.API/CMS.API.csproj"

 
COPY . .
WORKDIR "/src/CMS.API"
RUN dotnet publish "CMS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

 
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --chown=app:app --from=build /app/publish .
RUN mkdir -p /app/Uploads/Customers && chown -R app:app /app/Uploads
USER app



ENTRYPOINT ["dotnet", "CMS.API.dll"]
