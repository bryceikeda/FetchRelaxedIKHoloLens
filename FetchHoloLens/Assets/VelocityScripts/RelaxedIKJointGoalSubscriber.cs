using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.RelaxedIkRos1;
using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Std;


// Run this script to get a continous reading of the joint states
// from ROS, then the Fetch robot will follow the simulated or real robot
public class RelaxedIKJointGoalSubscriber : MonoBehaviour
{

    public static readonly string[] LinkNames =
        {
         "base_link/torso_lift_link",
        "base_link/torso_lift_link/shoulder_pan_link",
        "base_link/torso_lift_link/shoulder_pan_link/shoulder_lift_link",
        "base_link/torso_lift_link/shoulder_pan_link/shoulder_lift_link/upperarm_roll_link",
        "base_link/torso_lift_link/shoulder_pan_link/shoulder_lift_link/upperarm_roll_link/elbow_flex_link",
        "base_link/torso_lift_link/shoulder_pan_link/shoulder_lift_link/upperarm_roll_link/elbow_flex_link/forearm_roll_link",
        "base_link/torso_lift_link/shoulder_pan_link/shoulder_lift_link/upperarm_roll_link/elbow_flex_link/forearm_roll_link/wrist_flex_link",
        "base_link/torso_lift_link/shoulder_pan_link/shoulder_lift_link/upperarm_roll_link/elbow_flex_link/forearm_roll_link/wrist_flex_link/wrist_roll_link"
    };

    // Variables required for ROS communication
    [SerializeField]
    string m_TopicName = "/relaxed_ik/joint_angle_solutions";

    [SerializeField]
    GameObject m_Fetch;

    // Robot Joints
    // Make this URDFJoint since the Fetch has continuous, revolute and prismatic joints
    public ArticulationBody[] m_JointArticulationBodies;

    // ROS Connector
    ROSConnection m_Ros;

    public Float64MultiArrayMsg angleMsg; 

    // Start is called before the first frame update
    void Start()
    {
        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.Subscribe<JointAnglesMsg>(m_TopicName, SetJointAngles);

        // Hard code 8 for number of joints for now
        // Will need to change depending on planning group
        m_JointArticulationBodies = new ArticulationBody[8];

        for (var i = 0; i < 8; i++)
        {
            m_JointArticulationBodies[i] = m_Fetch.transform.Find(LinkNames[i]).GetComponent<ArticulationBody>();
        }
    }

    void SetJointAngles(JointAnglesMsg msg)
    {
        angleMsg = msg.angles; 

        for (int angle = 0; angle < msg.angles.data.Length; angle++)
        {
            if (m_JointArticulationBodies[angle].jointType == ArticulationJointType.PrismaticJoint)
            {
                var jointXDrive = m_JointArticulationBodies[angle].xDrive;
                jointXDrive.target = (float)msg.angles.data[angle];
                m_JointArticulationBodies[angle].xDrive = jointXDrive;
            }
            else
            {
                var jointXDrive = m_JointArticulationBodies[angle].xDrive;
                jointXDrive.target = (float)msg.angles.data[angle] * Mathf.Rad2Deg;
                m_JointArticulationBodies[angle].xDrive = jointXDrive;
            }
        }
    }
}
