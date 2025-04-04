FROM mcr.microsoft.com/dotnet/sdk:9.0 AS api-builder
WORKDIR /app
RUN apt-get update \
    && apt-get install clang zlib1g-dev -y
COPY . .
RUN cd src/ApiPrinter && dotnet publish -c Release -p:PublishAot=true -o /out && cd /out && ls -l

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0 AS app
ENV LANG C.UTF-8
WORKDIR /app
COPY --from=api-builder /out .
ENTRYPOINT ["/app/ApiPrinter"]
