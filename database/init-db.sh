#!/usr/bin/env bash
# 10-11-2026 Construido por: Ing GAAE - Initializes the database schema, indexes and seed data.
# Executes the schema, index and seed scripts against the SQL Server container.

set -euo pipefail

CONTAINER_NAME="${CONTAINER_NAME:-taskflow-sqlserver}"
SA_PASSWORD="${MSSQL_SA_PASSWORD:-TaskFlow@2026!}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Locate the sqlcmd binary available inside the SQL Server image.
SQLCMD="/opt/mssql-tools18/bin/sqlcmd"
if ! docker exec "${CONTAINER_NAME}" test -x "${SQLCMD}" >/dev/null 2>&1; then
    SQLCMD="/opt/mssql-tools/bin/sqlcmd"
fi

# 10-11-2026 Construido por: Ing GAAE - Waits until SQL Server accepts connections.
echo "Waiting for SQL Server to accept connections..."
for attempt in $(seq 1 60); do
    if docker exec "${CONTAINER_NAME}" \
        "${SQLCMD}" -S localhost -U sa -P "${SA_PASSWORD}" -C -b -Q "SELECT 1" >/dev/null 2>&1; then
        break
    fi

    if [ "${attempt}" -eq 60 ]; then
        echo "SQL Server did not become ready in time." >&2
        exit 1
    fi

    sleep 2
done

for script in 01_schema.sql 02_indexes.sql 05_seed.sql; do
    echo "Running ${script}..."
    docker exec -i "${CONTAINER_NAME}" \
        "${SQLCMD}" -S localhost -U sa -P "${SA_PASSWORD}" -C -b \
        -i "/scripts/${script}"
done

echo "Database initialization finished successfully."
