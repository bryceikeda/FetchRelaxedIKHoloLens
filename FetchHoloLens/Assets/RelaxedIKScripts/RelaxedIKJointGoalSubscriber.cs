using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

// Run this script to get a continous reading of the joint angles from relaxed IK
public class RelaxedIKJointGoalSubscriber : MonoBehaviour
{
    // ROS Connector
    ROSConnection m_Ros;
    public string m_TopicName = "/relaxed_ik/joint_angle_solutions";

    // Based on step 3b https://github.com/uwgraphics/relaxed_ik/blob/dev/src/start_here.py
    [SerializeField]
    JointController[] m_OrderedJoints;

    // Start is called before the first frame update
    void Start()
    {
        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.Subscribe<JointStateMsg>(m_TopicName, SetJointAngles);
    }


    // Hardcoded for the Fetch robot
    void SetJointAngles(JointStateMsg msg)
    {
        m_OrderedJoints[0].SetJointPosition(-(float)msg.position[0]);
        m_OrderedJoints[2].SetJointPosition(-(float)msg.position[2]);
        m_OrderedJoints[4].SetJointPosition(-(float)msg.position[4]);
        m_OrderedJoints[6].SetJointPosition(-(float)msg.position[6]);
        m_OrderedJoints[1].SetJointPosition((float)msg.position[1]);
        m_OrderedJoints[3].SetJointPosition((float)msg.position[3]);
        m_OrderedJoints[5].SetJointPosition((float)msg.position[5]);
    }
}