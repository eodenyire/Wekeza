#!/usr/bin/env python3
import os
import subprocess
import sys
from pathlib import Path

SCRIPT_DIR = Path(__file__).resolve().parent
DB_CONTAINER = os.getenv("DB_CONTAINER", "wekeza-v1-postgres")
DB_USER = os.getenv("DB_USER", "wekeza_app")
DB_NAME = os.getenv("DB_NAME", "WekezaCoreDB")


def run(cmd: list[str], label: str) -> None:
    print(f"[run] {label}: {' '.join(cmd)}")
    result = subprocess.run(cmd, check=False, text=True)
    if result.returncode != 0:
        raise RuntimeError(f"Command failed ({label}) with exit code {result.returncode}")


def main() -> int:
    try:
        run([str(SCRIPT_DIR / "01_start_stack.sh")], "start stack")

        sql_file = SCRIPT_DIR / "sql" / "14_daily_realtime_seed.sql"
        run(["docker", "cp", str(sql_file), f"{DB_CONTAINER}:/tmp/14_daily_realtime_seed.sql"], "copy daily seed sql")

        run(
            [
                "docker", "exec", "-i", DB_CONTAINER,
                "psql", "-v", "ON_ERROR_STOP=1", "-U", DB_USER, "-d", DB_NAME,
                "-f", "/tmp/14_daily_realtime_seed.sql",
            ],
            "execute daily realtime seed",
        )

        counts_sql = SCRIPT_DIR / "sql" / "13_all_table_row_counts.sql"
        run(["docker", "cp", str(counts_sql), f"{DB_CONTAINER}:/tmp/13_all_table_row_counts.sql"], "copy count sql")
        run(
            [
                "docker", "exec", "-i", DB_CONTAINER,
                "psql", "-v", "ON_ERROR_STOP=1", "-U", DB_USER, "-d", DB_NAME,
                "-f", "/tmp/13_all_table_row_counts.sql",
            ],
            "post-seed all-table counts",
        )

        print("[ok] Daily realtime seeding completed")
        return 0
    except Exception as exc:
        print(f"[error] {exc}")
        return 1


if __name__ == "__main__":
    sys.exit(main())
