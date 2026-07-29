# CTE & Recursive CTE

```sql
WITH active_users AS (SELECT * FROM users WHERE active = 1)
SELECT * FROM active_users;
-- Recursive example
WITH RECURSIVE cte AS (SELECT 1 AS n UNION ALL SELECT n+1 FROM cte WHERE n < 10)
SELECT * FROM cte;
```
