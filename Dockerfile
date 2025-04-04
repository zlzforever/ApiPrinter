FROM mcr.microsoft.com/dotnet/sdk:9.0 AS api-builder
WORKDIR /app
COPY . .
RUN cd src/ApiPrinter && dotnet publish -c Release -p:PublishReadyToRun=true -o out

FROM debian:12-slim AS app
ENV LANG C.UTF-8
WORKDIR /app
COPY --from=api-builder /app/src/ApiPrinter/out .
ENTRYPOINT ["ApiPrinter"]
