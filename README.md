# FetchRelaxedIKHoloLens2
This is a Unity project I created to show how you could control a robot arm in Unity using inverse kinematics in ROS. Specifically, I use [relaxedIK](https://github.com/uwgraphics/relaxed_ik_ros1) to compute the joint positions given an end-effector position. 

## Requirements
- Unity 2020.3.38f1
- [MRTK v2.7.3](https://github.com/microsoft/MixedRealityToolkit-Unity/releases) with OpenXR, Examples, Extensions, Foundation, Standard Assets, Test Utilities, and Tools
- [ROS TCP Connector v0.7.0](https://github.com/Unity-Technologies/ROS-TCP-Connector)
- [URDF Importer v0.5.2](https://github.com/Unity-Technologies/URDF-Importer)
- [ROS TCP Endpoint](https://github.com/Unity-Technologies/ROS-TCP-Endpoint)
- Holographic remoting on the HoloLens 2
- ROS Noetic
- [relaxedIK](https://github.com/uwgraphics/relaxed_ik_ros1)

# Setup
## RelaxedIK
In your ROS workspace create a new folder. 
```sh
mkdir relaxed_ik/src
```
Clone the following repositories into the src directory
```sh
cd relaxed_ik/src
git clone https://github.com/Unity-Technologies/ROS-TCP-Endpoint.git
git clone https://github.com/ros/kdl_parser.git
git clone --branch ros1 https://github.com/ZebraDevs/fetch_ros.git
git clone https://github.com/uwgraphics/relaxed_ik_ros1.git
```
Then make sure you have all the dependencies listed in [relaxedIK](https://github.com/uwgraphics/relaxed_ik_ros1)

Build the directory
```sh
cd ..
catkin build
```
Source the directory
```sh
source devel/setup.bash
```
- If you have problems with nlopt v0.5.4, make try updating cmake. 
- If you have problems with ncollide2d and ncollide3d, update the version in the Config.toml file to 0.25. 

Then, copy all of the files in the Config folder in this repository to the Config folder in the relaxed_ik_ros1/relaxed_ik_core/config folder. This is for step 5 in the "Getting Started" section in the relaxed_ik_ros1 directions. 

Finally, follow all of the build directions in [relaxedIK](https://github.com/uwgraphics/relaxed_ik_ros1). You will need to download and use rust. 
## Unity
- Open up the Unity project
- Open up the FetchRelaxedIKScene
- Open up the FetchRelaxedIKScene
- Update your IP address in the Robotics tab
- Enable holographic remoting for play mode under the Mixed Reality tab, adding the IP address of your Hololens holographic play mode app

# Running the Application
## ROS
View the robot arm in rviz by typing the following command:
```sh
roslaunch relaxed_ik_ros1 rviz_viewer.launch
```
Launch the Relaxed IK solver by typing the following command:
```sh
roslaunch relaxed_ik_ros1 relaxed_ik_rust.launch
```
Launch the ros_tcp_endpoint by typing the following command adding in your own IP address:
```sh
roslaunch ros_tcp_endpoint endpoint.launch tcp_ip:=127.0.0.1 
```
Set a ROS parameter to start the simulation:
```sh
rosparam set /simulation_time go
```
## Unity
- Press play on the Unity application

