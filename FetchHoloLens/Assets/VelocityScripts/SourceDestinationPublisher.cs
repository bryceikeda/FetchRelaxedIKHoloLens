using System;
using RosMessageTypes.Geometry;
using RosMessageTypes.FetchUnity;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

public class SourceDestinationPublisher : MonoBehaviour
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
    [SerializeField]
    GameObject m_TargetPlacement;
    readonly Quaternion m_PickOrientation = Quaternion.Euler(90, 90, 0);

    // Robot Joints
    // Make this URDFJoint since the Fetch has continuous, revolute and prismatic joints
    UrdfJoint[] m_UrdfJoints;

    // Articulation Bodies
    public ArticulationBody[] m_JointArticulationBodies;
    public ArticulationBody m_LeftGripper;
    public ArticulationBody m_RightGripper;


    // ROS Connector
    ROSConnection m_Ros;

    void Start()
    {
        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.RegisterPublisher<FetchMoveitJointsMsg>(m_TopicName);

        m_UrdfJoints = new UrdfJoint[k_NumRobotJoints];

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

    public void Publish()
    {
        var sourceDestinationMessage = new FetchMoveitJointsMsg();

        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            sourceDestinationMessage.joints[i] = m_UrdfJoints[i].GetPosition();
        }

        // Pick Pose
        sourceDestinationMessage.pick_pose = new PoseMsg
        {
            position = m_Target.transform.position.To<FLU>(),
            orientation = Quaternion.Euler(90, m_Target.transform.eulerAngles.y, 0).To<FLU>()
        };

        // Place Pose
        sourceDestinationMessage.place_pose = new PoseMsg
        {
            position = m_TargetPlacement.transform.position.To<FLU>(),
            orientation = m_PickOrientation.To<FLU>()
        };

        // Finally send the message to server_endpoint.py running in ROS
        m_Ros.Publish(m_TopicName, sourceDestinationMessage);
    }
}