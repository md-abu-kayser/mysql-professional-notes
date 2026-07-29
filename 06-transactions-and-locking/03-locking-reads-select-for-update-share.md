# Locking Reads

- `SELECT ... FOR UPDATE` – exclusive lock on rows.
- `SELECT ... FOR SHARE` (or `LOCK IN SHARE MODE` in older versions) – shared lock.

> 📘 Next: [Deadlocks – Detection & Resolution](04-deadlocks-detection-and-resolution.md)
