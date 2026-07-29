# Full‑Text Indexes & Search

```sql
ALTER TABLE articles ADD FULLTEXT(title, body);
SELECT * FROM articles WHERE MATCH(title, body) AGAINST('MySQL tutorial');
```
