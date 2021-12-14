# Run #  
```
docker-compose -f "docker-compose.yml" up -d --build  
```

------------

# Stats  

### WRITE ###  
```
                              Redis + rdb (fifo) | Redis + aof (fifo) | Redis + rdb (lifo) | Redis + aof (lifo) |         Beanstalkd |
Transactions:                          3341 hits |          3317 hits |          3402 hits |          3412 hits |          3296 hits |
Availability:                           100.00 % |           100.00 % |           100.00 % |           100.00 % |           100.00 % |
Elapsed time:                         60.00 secs |         59.30 secs |         59.74 secs |         59.99 secs |         59.55 secs |
Data transferred:                        0.00 MB |            0.00 MB |            0.00 MB |            0.00 MB |            0.00 MB |
Response time:                         0.02 secs |          0.02 secs |          0.02 secs |          0.02 secs |          0.02 secs |
Transaction rate:                55.69 trans/sec |    55.93 trans/sec |    56.94 trans/sec |    56.88 trans/sec |    55.35 trans/sec |
Throughput:                          0.00 MB/sec |        0.00 MB/sec |        0.00 MB/sec |        0.00 MB/sec |        0.00 MB/sec |
Concurrency:                                1.32 |               1.02 |               1.20 |               1.02 |               1.14 |
Successful transactions:                    3341 |               3317 |               3402 |               3412 |               3296 |
Failed transactions:                           0 |                  0 |                  0 |                  0 |                  0 |
Longest transaction:                        0.09 |               0.10 |               0.09 |               0.09 |               0.28 |
Shortest transaction:                       0.00 |               0.00 |               0.00 |               0.00 |               0.00 |
```
### READ ###  
```

                              Redis + rdb (fifo) | Redis + aof (fifo) | Redis + rdb (lifo) | Redis + aof (lifo) |         Beanstalkd |
numberOfReadMessages:                  3341 hits |          3317 hits |          3402 hits |          3412 hits |          3296 hits |
avgElapsedTimeFromWriteToRead:             44 ms |              30 ms |              38 ms |              30 ms |              28 ms |
maxElapsedTimeFromWriteToRead time:       194 ms |             202 ms |             258 ms |             289 ms |             295 ms |
minElapsedTimeFromWriteToRead:              0 ms |               0 ms |               0 ms |               0 ms |               0 ms |
```

------------

# Redis + rdb (fifo)  
```
siege -c30 -t60S http://127.0.0.1:8800/api/writetoqueue/push  
http://127.0.0.1:8800/api/writetoqueue/Stats - get read stats  
http://127.0.0.1:8800/api/writetoqueue/Stats?isReset=true - reset read stats  
```
# Redis + aof (fifo)  
```
siege -c30 -t60S http://127.0.0.1:8801/api/writetoqueue/push  
http://127.0.0.1:8801/api/writetoqueue/Stats - get read stats  
http://127.0.0.1:8801/api/writetoqueue/Stats?isReset=true - reset read stats  
```
# Redis + rdb (lifo)  
```
siege -c30 -t60S http://127.0.0.1:8803/api/writetoqueue/push  
http://127.0.0.1:8803/api/writetoqueue/Stats - get read stats  
http://127.0.0.1:8803/api/writetoqueue/Stats?isReset=true - reset read stats  
```
# Redis + aof (lifo)  
```
siege -c30 -t60S http://127.0.0.1:8804/api/writetoqueue/push  
http://127.0.0.1:8804/api/writetoqueue/Stats - get read stats  
http://127.0.0.1:8804/api/writetoqueue/Stats?isReset=true - reset read stats  
```
# Beanstalkd  
```
siege -c30 -t60S http://127.0.0.1:8802/api/writetoqueue/push  
http://127.0.0.1:8802/api/writetoqueue/Stats - get read stats  
http://127.0.0.1:8802/api/writetoqueue/Stats?isReset=true - reset read stats  
```
