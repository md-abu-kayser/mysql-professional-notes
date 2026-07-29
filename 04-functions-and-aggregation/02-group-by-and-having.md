# GROUP BY & HAVING

```sql
SELECT country, COUNT(*) FROM users GROUP BY country HAVING COUNT(*) > 5;
```
