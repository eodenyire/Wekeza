-- Exact row counts for all public tables

DROP TABLE IF EXISTS tmp_table_counts;
CREATE TEMP TABLE tmp_table_counts
(
  table_name text NOT NULL,
  row_count bigint NOT NULL
);

DO $$
DECLARE
  r record;
  v_count bigint;
BEGIN
  FOR r IN
    SELECT table_name
    FROM information_schema.tables
    WHERE table_schema = 'public'
    ORDER BY table_name
  LOOP
    EXECUTE format('SELECT COUNT(*) FROM %I.%I', 'public', r.table_name) INTO v_count;
    INSERT INTO tmp_table_counts(table_name, row_count) VALUES (r.table_name, v_count);
  END LOOP;
END $$;

SELECT *
FROM tmp_table_counts
ORDER BY table_name;
