#!/usr/bin/env bash
set -u

BACKUP_DIR="${BACKUP_DIR:-/backups}"
BACKUP_LOG_FILE="${BACKUP_LOG_FILE:-${BACKUP_DIR}/logs/backup.log}"
BACKUP_FILE_PREFIX="${BACKUP_FILE_PREFIX:-orbital_academy}"

timestamp="$(date -u +"%Y%m%d_%H%M%S")"
backup_file_name="${BACKUP_FILE_PREFIX}_${timestamp}.dump"
backup_file_path="${BACKUP_DIR%/}/${backup_file_name}"
log_dir="$(dirname "${BACKUP_LOG_FILE}")"

log_event() {
    local status="$1"
    local file_name="$2"
    local exit_code="$3"
    local message="$4"
    local now

    now="$(date -u +"%Y-%m-%dT%H:%M:%SZ")"
    printf '%s | status=%s | file=%s | exit_code=%s | message=%s\n' \
        "${now}" "${status}" "${file_name}" "${exit_code}" "${message}" >> "${BACKUP_LOG_FILE}"
}

if ! mkdir -p "${BACKUP_DIR}" "${log_dir}"; then
    echo "Nao foi possivel criar ${BACKUP_DIR} ou ${log_dir}." >&2
    exit 10
fi

if ! command -v pg_dump >/dev/null 2>&1; then
    log_event "ERROR" "-" "127" "pg_dump nao encontrado no ambiente"
    exit 127
fi

missing_vars=()
for var_name in PGHOST PGPORT PGDATABASE PGUSER PGPASSWORD; do
    if [ -z "${!var_name:-}" ]; then
        missing_vars+=("${var_name}")
    fi
done

if [ "${#missing_vars[@]}" -gt 0 ]; then
    log_event "ERROR" "-" "2" "variaveis obrigatorias ausentes: ${missing_vars[*]}"
    exit 2
fi

pg_dump --format=custom --no-owner --no-privileges --file="${backup_file_path}"
exit_code="$?"

if [ "${exit_code}" -eq 0 ]; then
    chmod 600 "${backup_file_path}" 2>/dev/null || true
    log_event "SUCCESS" "${backup_file_name}" "0" "backup concluido"
    echo "Backup gerado: ${backup_file_path}"
    exit 0
fi

rm -f "${backup_file_path}" 2>/dev/null || true
log_event "ERROR" "${backup_file_name}" "${exit_code}" "falha ao executar pg_dump"
exit "${exit_code}"
