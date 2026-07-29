#!/bin/bash
BACKUP_DIR=/backups/mysql
DATE=$(date +%Y%m%d_%H%M%S)
mysqldump -u root -p"$MYSQL_ROOT_PASSWORD" --all-databases --single-transaction | gzip > "$BACKUP_DIR/full_$DATE.sql.gz"
find $BACKUP_DIR -name "*.gz" -mtime +7 -delete