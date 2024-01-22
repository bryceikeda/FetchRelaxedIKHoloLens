# FetchRelaxedIKHoloLens2
This is a Unity project I created to show how you can control a robot arm in Unity using inverse kinematics calculations from ROS. Specifically, I use relaxedIK to compute the joint positions given an end-effector position, and send that to an AR visualization on the HoloLens 2.

Note: If you compile relaxedIK using windows, it will output a dll that you can use in Unity rather than sending and receiving information using ROS. The performance is much better. However, I was unable compile the repository. It should work with most other robots that they already compiled/built it for in their [Unity repository](https://github.com/uwgraphics/relaxed_ik_unity.git). Setup will probably be slightly different.

![fetch_gif](https://user-images.githubusercontent.com/56240638/207370039-4400c132-fe11-4ada-9e62-8e1ce592814c.gif)

## Requirements
- Unity 2020.3.38f1
- [MRTK v2.8.3](https://github.com/microsoft/MixedRealityToolkit-Unity/releases) with OpenXR, Examples, Extensions, Foundation, Standard Assets, Test Utilities, and Tools
- [ROS TCP Connector v0.7.0](https://github.com/Unity-Technologies/ROS-TCP-Connector)
- [URDF Importer v0.5.2](https://github.com/Unity-Technologies/URDF-Importer)
- [ROS TCP Endpoint](https://github.com/Unity-Technologies/ROS-TCP-Endpoint)
- Holographic remoting on the HoloLens 2
- ROS Noetic
- [relaxedIK](https://github.com/uwgraphics/relaxed_ik_ros1)

# Setup
## RelaxedIK
Create a new ROS workspace folder.
```sh
mkdir relaxed_ik
cd relaxed_ik/
```
Clone the following repositories into a src directory
```sh
git clone --recuse-submodules https://github.com/uwgraphics/relaxed_ik_ros1.git src 
cd src
git clone https://github.com/Unity-Technologies/ROS-TCP-Endpoint.git
```

If in WSL2 install rust like so:
```
sudo apt-get update
sudo apt-get install curl
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
source $HOME/.cargo/env
echo 'source $HOME/.cargo/env' >> $HOME/.bashrc
```

If in Ubuntu install rust like so:
```
sudo apt-get update
sudo apt-get install curl
curl https://sh.rustup.rs -sSf | bash -s -- -y
source $HOME/.cargo/env
echo 'source $HOME/.cargo/env' >> $HOME/.bashrc
```

Build relaxed_ik_core:
```
cd relaxed_ik_ros1/relaxed_ik_core/
cargo build
```

Use fetch.yaml settings:
```
cp configs/example_settings/fetch.yaml configs/settings.yaml
```

Build the directory
```sh
cd ..
catkin build
```
Source the directory
```sh
source devel/setup.bash
```

## Unity
- Open up the Unity project
- Open up the FetchRelaxedIKScene
- Open up the FetchRelaxedIKScene
- Update your IP address in the Robotics tab
- Enable holographic remoting for play mode under the Mixed Reality tab, adding the IP address of your Hololens holographic play mode app

# Running the Application
## ROS
Launch the Relaxed IK solver by typing the following command:
```sh
roslaunch relaxed_ik_ros1 demo.launch
```
Launch the ros_tcp_endpoint by typing the following command adding in your own IP address:
```sh
roslaunch ros_tcp_endpoint endpoint.launch tcp_ip:=xxx.xxx.x.x tcp_port:=10000
```

Run lfd helper package if using it:
```
rosrun lfd_receiver record.py
```

## Unity
- Open the HandTrackingScene using holographic remoting
- Press play on the Unity application
- Look at the QR code. If the scene does not align correctly, press the Reset World Origin button on the menu.
- To send a service message to ROS to begin recording hit the Begin Recording button.
- Move your hand around and the end effector should follow it. 

## Places to edit
To edit the placement of the end effector on the hand oen the Prefabs/HandTrackedPoseReference game object. Then move or rotate the Gripper child object. If you would like to test this in the scene, press play in Unity and hold space to show the right hand if not using the Hololens. Then move the gripper that is attached to the HandTrackedPoseReference which is a child object of the torso_lift_link. 

To edit the world position of the fetch robot, move the MoveThisToAlignRobotWithRealRobot game object in the QR/QROrigin game object in the heirarchy. 