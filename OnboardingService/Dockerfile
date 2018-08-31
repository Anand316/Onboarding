FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 49701
EXPOSE 44371

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY OnboardingService.csproj OnboardingService/
RUN dotnet restore OnboardingService/OnboardingService.csproj
COPY . .
WORKDIR /src
RUN dotnet build OnboardingService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish OnboardingService.csproj -c Release -o /app

#RUN chmod +x ./entrypoint.sh
#CMD /bin/bash ./entrypoint.sh
ENTRYPOINT ["entrypoint.sh"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OnboardingService.dll"]