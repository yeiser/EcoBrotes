#!/bin/bash
docker run -d \
  --name ecobrotess-api \
  --network ecobrotess-network \
  -p 8080:80 \
  -e "ConnectionStrings__db=Host=postgres-ecobrotess;Port=5432;Database=ecobrotess;Username=postgres;Password=postgres" \
  -e "UseInMemoryDatabase=false" \
  -e "ASPNETCORE_ENVIRONMENT=Development" \
  -e "ASPNETCORE_HTTP_PORTS=80" \
  ecobrotess-api
