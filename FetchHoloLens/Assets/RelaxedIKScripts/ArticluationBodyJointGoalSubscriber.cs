using UnityEngine;
using RosMessageTypes.RelaxedIkRos1;
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
        m_Ros.Subscribe<JointAnglesMsg>(m_TopicName, SetJointAngles);
    }

    void SetJointAngles(JointAnglesMsg msg)
    {
        // Set the joint angles for each joint in the list
        for (int joint = 0; joint < msg.angles.data.Length; joint++)
        {
            // If the joint is prismatic, it will not be in degrees
            if (m_JointOrdering[joint].jointType == ArticulationJointType.PrismaticJoint)
            {
                var jointXDrive = m_JointOrdering[joint].xDrive;
                jointXDrive.target = (float)msg.angles.data[joint];
                m_JointOrdering[joint].xDrive = jointXDrive;
            }
            // Convert the joint from radians to degrees for Unity
            else
            {
                var jointXDrive = m_JointOrdering[joint].xDrive;
                jointXDrive.target = (float)msg.angles.data[joint] * Mathf.Rad2Deg;
                m_JointOrdering[joint].xDrive = jointXDrive;
            }
        }
    }
}
