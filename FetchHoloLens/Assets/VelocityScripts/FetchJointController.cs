using System;
using RosMessageTypes.Geometry;
using RosMessageTypes.FetchUnity;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

public class FetchJointController : MonoBehaviour
{
    const int k_NumRobotJoints = 8;

    public static readonly string[] LinkNames =
        {"base_link/torso_lift_link", "/shoulder_pan_link", "/shoulder_lift_link", "/upperarm_roll_link", "/elbow_flex_link", "/forearm_roll_link", "/wrist_flex_link",  "/wrist_roll_link"};

    // Variables required for ROS communication
    [SerializeField]
    string m_TopicName = "/fetch_joints";

    [SerializeField]
    GameObject m_Fetch;
    [SerializeField]
    GameObject m_Target;
    readonly Quaternion m_PickOrientation = Quaternion.Euler(90, 90, 0);

    // Robot Joints
    // Make this URDFJoint since the Fetch has continuous, revolute and prismatic joints
    UrdfJoint[] m_UrdfJoints;

    // Articulation Bodies
    ArticulationBody[] m_JointArticulationBodies;
    ArticulationBody m_LeftGripper;
    ArticulationBody m_RightGripper;

    // ROS Connector
    ROSConnection m_Ros;

    void Start()
    {
        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.RegisterPublisher<FetchMoveitJointsMsg>(m_TopicName);

        m_UrdfJoints = new UrdfJoint[k_NumRobotJoints];
        m_JointArticulationBodies = new ArticulationBody[k_NumRobotJoints];
        
        var linkName = string.Empty;
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            linkName += LinkNames[i];
            var link = m_Fetch.transform.Find(linkName);
            m_UrdfJoints[i] = link.GetComponent<UrdfJoint>();
            m_JointArticulationBodies[i] = link.GetComponent<ArticulationBody>();
        }

        // Find left and right fingers
        var rightGripper = linkName + "/gripper_link/r_gripper_finger_link";
        var leftGripper = linkName + "/gripper_link/l_gripper_finger_link";

        m_RightGripper = m_Fetch.transform.Find(rightGripper).GetComponent<ArticulationBody>();
        m_LeftGripper = m_Fetch.transform.Find(leftGripper).GetComponent<ArticulationBody>();
    }

    public void SetJointTarget(int jointIndex, float jointValue)
    {
        var xDrive = m_JointArticulationBodies[jointIndex].xDrive;
        xDrive.target = jointValue;
        m_JointArticulationBodies[jointIndex].xDrive = xDrive;
    }


    public double GetJointPosition(int jointIndex)
    {
        return m_JointArticulationBodies[jointIndex].jointPosition[0];
    }

    public double GetJointTarget(int jointIndex)
    {
        return m_JointArticulationBodies[jointIndex].xDrive.target;
    }

    public ArticulationDrive GetJointDrive(int jointIndex)
    {
        return m_JointArticulationBodies[jointIndex].xDrive;
    }

    public void SetJointDrive(int jointIndex, ArticulationDrive xDrive)
    {
        m_JointArticulationBodies[jointIndex].xDrive = xDrive;
    }


    /// <summary>
    ///     Close the gripper
    /// </summary>
    public void CloseGripper()
    {
        var leftDrive = m_LeftGripper.xDrive;
        var rightDrive = m_RightGripper.xDrive;

        leftDrive.target = -0.45f;
        rightDrive.target = 0.45f;
        
        m_LeftGripper.xDrive = leftDrive;
        m_RightGripper.xDrive = rightDrive;
    }

    /// <summary>
    ///     Open the gripper
    /// </summary>
    public void OpenGripper()
    {
        var leftDrive = m_LeftGripper.xDrive;
        var rightDrive = m_RightGripper.xDrive;

        leftDrive.target = 0.45f;
        rightDrive.target = -0.45f;

        m_LeftGripper.xDrive = leftDrive;
        m_RightGripper.xDrive = rightDrive;
    }


}