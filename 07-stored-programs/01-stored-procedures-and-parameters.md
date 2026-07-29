# Stored Procedures & Parameters

```sql
DELIMITER //
CREATE PROCEDURE GetUser(IN userId INT)
BEGIN
  SELECT * FROM users WHERE id = userId;
END //
DELIMITER ;
CALL GetUser(1);
```
