-- Top queries by execution time
SELECT DIGEST_TEXT, COUNT_STAR, SUM_TIMER_WAIT FROM performance_schema.events_statements_summary_by_digest ORDER BY SUM_TIMER_WAIT DESC LIMIT 10;

-- Unused indexes
SELECT * FROM sys.schema_unused_indexes;