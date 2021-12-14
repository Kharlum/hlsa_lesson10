# Run #  
```
docker-compose -f "docker-compose.yml" up -d --build  
```

# Redis + rdb (fifo)  

```
siege -c30 -t60S http://127.0.0.1:8800/api/writetoqueue/push  
```
### WRITE ###  
```
Transactions:                   3341 hits  
Availability:                 100.00 %  
Elapsed time:                  60.00 secs  
Data transferred:               0.00 MB  
Response time:                  0.02 secs  
Transaction rate:              55.69 trans/sec  
Throughput:                     0.00 MB/sec  
Concurrency:                    1.32  
Successful transactions:        3341  
Failed transactions:               0  
Longest transaction:            0.09  
Shortest transaction:           0.00  
```
### READ ###  
```
numberOfReadMessages:          3341 hits  
avgElapsedTimeFromWriteToRead: 44 ms  
maxElapsedTimeFromWriteToRead: 194 ms  
minElapsedTimeFromWriteToRead: 0 ms    
```
