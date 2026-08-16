#!/usr/bin/env bash
# ./dev.sh

set -e

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

cleanup() {
  echo
  echo "Stopping development services..."

  kill "$API_PID" 2>/dev/null || true
  kill "$FRONTEND_PID" 2>/dev/null || true

  wait "$API_PID" 2>/dev/null || true
  wait "$FRONTEND_PID" 2>/dev/null || true
}

trap cleanup EXIT INT TERM

cd "$ROOT_DIR"

echo "Starting PostgreSQL..."

docker compose \
  --env-file .env.local \
  -f compose.local.yaml \
  up -d postgres

echo "Waiting for PostgreSQL..."

until docker compose \
  --env-file .env.local \
  -f compose.local.yaml \
  exec -T postgres \
  pg_isready -U personal_site -d personal_site >/dev/null 2>&1
do
  sleep 1
done

echo "PostgreSQL is ready."

if [ ! -d "Frontend/node_modules" ]; then
  echo "Installing frontend dependencies..."
  npm --prefix Frontend ci
fi

export AzureStorage__AccountName="$(
  grep '^AZURE_STORAGE_ACCOUNT_NAME=' "$ROOT_DIR/.env.local" |
  cut -d= -f2-
)"

export AzureStorage__ContainerName="$(
  grep '^AZURE_STORAGE_CONTAINER_NAME=' "$ROOT_DIR/.env.local" |
  cut -d= -f2-
)"

echo "Starting API..."

dotnet watch \
  --project PersonalSite.Api/PersonalSite.Api.csproj \
  run &

API_PID=$!

echo "Starting frontend..."

npm --prefix Frontend run dev &

FRONTEND_PID=$!

echo
echo "Development environment started:"
echo "Frontend: http://localhost:5173"
echo "API:      http://localhost:5285"
echo
echo "Press Ctrl+C to stop the API and frontend."
echo "PostgreSQL will remain running for faster startup next time."

wait