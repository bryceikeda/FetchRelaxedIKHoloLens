using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

using RosMessageTypes.Geometry; 

// Run this script to get a continous reading of the joint states
// from ROS, then the Fetch robot will follow the simulated or real robot
public class JointStateSubscriber : MonoBehaviour
{
    const int k_NumRobotJoints = 15;

    public PointMsg pos;
    public QuaternionMsg quat;
    public GameObject newObject; 

    public static readonly string[] LinkNames =
        {"base_link/l_wheel_link",
        "base_link/r_wheel_link",
        "base_link/torso_lift_link",  
        "base_link/torso_lift_link/bellows_link",
        "base_link/torso_lift_link/head_pan_link",
        "base_link/torso_lift_link/head_pan_link/head_tilt_link",
        "base_link/torso_lift_link/shoulder_pan_link",
        "base_link/torso_lift_link/shoulder_pan_link/shoulder_lift_link", 
        "base_link/torso_lift_link/shoulder_pan_link/shoulder_lift_link/upperarm_roll_link",
        "base_link/torso_lift_link/shoulder_pan_link/shoulder_lift_link/upperarm_roll_link/elbow_flex_link",
        "base_link/torso_lift_link/shoulder_pan_link/shoulder_lift_link/upperarm_roll_link/elbow_flex_link/forearm_roll_link",
        "base_link/torso_lift_link/shoulder_pan_link/shoulder_lift_link/upperarm_roll_link/elbow_flex_link/forearm_roll_link/wrist_flex_link",
        "base_link/torso_lift_link/shoulder_pan_link/shoulder_lift_link/upperarm_roll_link/elbow_flex_link/forearm_roll_link/wrist_flex_link/wrist_roll_link",
        "base_link/torso_lift_link/shoulder_pan_link/shoulder_lift_link/upperarm_roll_link/elbow_flex_link/forearm_roll_link/wrist_flex_link/wrist_roll_link/gripper_link/l_gripper_finger_link",
        "base_link/torso_lift_link/shoulder_pan_link/shoulder_lift_link/upperarm_roll_link/elbow_flex_link/forearm_roll_link/wrist_flex_link/wrist_roll_link/gripper_link/r_gripper_finger_link"};

    // Variables required for ROS communication
    [SerializeField]
    string m_TopicName = "/joint_states";

    [SerializeField]
    GameObject m_Fetch;

    // Robot Joints
    // Make this URDFJoint since the Fetch has continuous, revolute and prismatic joints
    public ArticulationBody[] m_JointArticulationBodies;

    // ROS Connector
    ROSConnection m_Ros;

    // Start is called before the first frame update
    void Start()
    {
        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.Subscribe<JointStateMsg>(m_TopicName, SetJointStates);

        m_JointArticulationBodies = new ArticulationBody[k_NumRobotJoints];

        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            m_JointArticulationBodies[i] = m_Fetch.transform.Find(LinkNames[i]).GetComponent<ArticulationBody>();
        }

        pos = newObject.transform.position.To<FLU>();
        quat = Quaternion.Euler(newObject.transform.eulerAngles.x, newObject.transform.eulerAngles.y, newObject.transform.eulerAngles.z).To<FLU>();
    }

    void SetJointStates(JointStateMsg states)
    {
        if (states != null) {
            for (var joint = 0; joint < k_NumRobotJoints; joint++)
            {
                if (joint == 2)
                {
                    var joint1XDrive = m_JointArticulationBodies[joint].xDrive;
                    joint1XDrive.target = (float)states.position[joint];
                    //joint1XDrive.targetVelocity = (float)states.velocity[joint];
                    m_JointArticulationBodies[joint].xDrive = joint1XDrive;
                }
                else if (joint == 3)
                {
                    var joint1XDrive = m_JointArticulationBodies[joint].xDrive;
                    joint1XDrive.target = (float)states.position[joint];
                    //joint1XDrive.targetVelocity = (float)states.velocity[joint];
                    m_JointArticulationBodies[joint].xDrive = joint1XDrive;
                }
                else {
                    var joint1XDrive = m_JointArticulationBodies[joint].xDrive;
                    joint1XDrive.target = (float)states.position[joint] * Mathf.Rad2Deg;
                    //joint1XDrive.targetVelocity = (float)states.velocity[joint];
                    m_JointArticulationBodies[joint].xDrive = joint1XDrive;
                }


                // leftDrive.target = -0.45f;
                // rightDrive.target = 0.45f;

                // m_LeftGripper.xDrive = leftDrive;
                // m_RightGripper.xDrive = rightDrive;
            }
        }
    }
}
