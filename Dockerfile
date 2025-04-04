FROM mcr.microsoft.com/dotnet/sdk:9.0 AS api-builder
WORKDIR /app
COPY . .
RUN cd src/ApiPrinter && dotnet publish -c Release -p:PublishAot=true -o out

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0 AS app
ENV LANG C.UTF-8
WORKDIR /app
COPY --from=api-builder /app/src/ApiPrinter/out .
ENTRYPOINT ["ApiPrinter"]
