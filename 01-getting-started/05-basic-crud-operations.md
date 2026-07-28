# Creating Databases & Tables

```sql
CREATE DATABASE myapp;
USE myapp;
CREATE TABLE users (
  id INT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(100) NOT NULL,
  email VARCHAR(255) UNIQUE
);
```
