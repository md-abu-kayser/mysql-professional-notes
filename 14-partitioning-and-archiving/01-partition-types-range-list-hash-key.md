# Partition Types

```sql
CREATE TABLE orders ( ... ) PARTITION BY RANGE (YEAR(order_date)) (
  PARTITION p0 VALUES LESS THAN (2020),
  PARTITION p1 VALUES LESS THAN (2021),
  ...
);
```
