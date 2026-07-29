# pt‑query‑digest Examples

```bash
# Analyze slow query log
pt-query-digest /var/log/mysql/slow.log > report.txt
# Analyze from tcpdump
tcpdump -i eth0 port 3306 -s 65535 -w mysql.pcap
pt-query-digest --type tcpdump mysql.pcap
```
