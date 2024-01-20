using UnityEngine;
using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector;

// Run this script to get a continous reading of the joint angles from relaxed IK
public class ArticulationBodyJointGoalSubscriber : MonoBehaviour
{
    // ROS Connector
    ROSConnection m_Ros;
    string m_TopicName = "/relaxed_ik/joint_angle_solutions";

    // Based on step 3b https://github.com/uwgraphics/relaxed_ik/blob/dev/src/start_here.py
    [SerializeField]
    ArticulationBody[] m_JointOrdering;

    // Start is called before the first frame update
    void Start()
    {
        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.Subscribe<JointStateMsg>(m_TopicName, SetJointAngles);
    }

    void SetJointAngles(JointStateMsg msg)
    {
        // Set the joint angles for each joint in the list
        for (int joint = 0; joint < msg.position.Length; joint++)
        {
            // If the joint is prismatic, it will not be in degrees
            if (m_JointOrdering[joint].jointType == ArticulationJointType.PrismaticJoint)
            {
                var jointXDrive = m_JointOrdering[joint].xDrive;
                jointXDrive.target = (float)msg.position[joint];
                m_JointOrdering[joint].xDrive = jointXDrive;
            }
            // Convert the joint from radians to degrees for Unity
            else
            {
                var jointXDrive = m_JointOrdering[joint].xDrive;
                jointXDrive.target = (float)msg.position[joint] * Mathf.Rad2Deg;
                m_JointOrdering[joint].xDrive = jointXDrive;
            }
        }
    }
}