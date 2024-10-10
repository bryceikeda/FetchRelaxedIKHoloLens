# Docker Setup
## Building Docker
```
make build_ros_arcade
```

## Running Docker
```
make run_ros_arcade
```

## Reopening container
Check the container ID first four digits
```
docker container ls -a
```

Then start the container
```
make start_ros_arcade <First four digits of Container ID>
```

# Running relaxed_ik
In one window run relaxed_ik
```
roslaunch relaxed_ik_ros1 demo.launch
```
 
To connect to Unity, in another window run
```
hostname -I 
```

Take the ip address, in my case it was 172.17.0.2, and input that as the tcp_ip

```
roslaunch ros_tcp_endpoint endpoint.launch tcp_ip:=172.17.0.2 tcp_ip:=10000
```
