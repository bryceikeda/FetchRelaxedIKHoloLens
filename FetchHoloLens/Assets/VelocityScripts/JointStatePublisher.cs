using System;
using RosMessageTypes.Geometry;
using RosMessageTypes.FetchUnity;
using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using Unity.Robotics.Core;
using UnityEngine;
using RosMessageTypes.Std;

public class JointStatePublisher : MonoBehaviour
{
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


    public static readonly string[] JointNames =
        {"l_wheel_joint",
         "r_wheel_joint",
         "torso_lift_joint",
         "bellows_joint",
         "head_pan_joint",
         "head_tilt_joint",
         "shoulder_pan_joint",
         "shoulder_lift_joint",
         "upperarm_roll_joint",
         "elbow_flex_joint",
         "forearm_roll_joint",
         "wrist_flex_joint",
         "wrist_roll_joint",
         "l_gripper_finger_joint",
         "r_gripper_finger_joint"};


    const int k_NumRobotJoints = 15;

    [SerializeField]
    double m_PublishRateHz = 20f;
    // Variables required for ROS communication
    [SerializeField]
    string m_TopicName = "/joint_states";
    [SerializeField]
    GameObject m_Fetch;

    // ROS Connector
    ROSConnection m_Ros;

    double m_LastPublishTimeSeconds;

    double PublishPeriodSeconds => 1.0f / m_PublishRateHz;

    bool ShouldPublishMessage => Clock.NowTimeInSeconds > m_LastPublishTimeSeconds + PublishPeriodSeconds;

    JointStateMsg jointStateMessage;

    ArticulationBody[] m_JointArticulationBodies; 

    /// <summary>
    ///     Find all robot joints in Awake() and add them to the jointArticulationBodies array.
    ///     Find left and right finger joints and assign them to their respective articulation body objects.
    /// </summary>
    void Start()
    {
        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.RegisterPublisher<JointStateMsg>(m_TopicName);

        m_JointArticulationBodies = new ArticulationBody[k_NumRobotJoints];

        // Get articulation body of each link 
        var linkName = string.Empty;
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            var link = m_Fetch.transform.Find(LinkNames[i]);
            m_JointArticulationBodies[i] = link.GetComponent<ArticulationBody>();
        }

        // Create new joint state message
        jointStateMessage = new JointStateMsg(new HeaderMsg(),
                                            JointNames,
                                            new double[k_NumRobotJoints],
                                            new double[k_NumRobotJoints],
                                            new double[k_NumRobotJoints]);
    }


    /// <summary>
    ///     Get the current values of the robot's joint state
    /// </summary>
    /// <returns>FetchMoveitJoints</returns>
    void UpdateJointStates()
    {
        jointStateMessage.header = new HeaderMsg(0, new TimeStamp(Clock.time), "");
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            jointStateMessage.position[i] = m_JointArticulationBodies[i].jointPosition[0];
            jointStateMessage.velocity[i] = m_JointArticulationBodies[i].jointVelocity[0];
            jointStateMessage.effort[i] = m_JointArticulationBodies[i].jointForce[0];
        }
    }

    public void PublishMessage()
    {
        UpdateJointStates();
        // publish message
        m_Ros.Publish(m_TopicName, jointStateMessage);
        m_LastPublishTimeSeconds = Clock.FrameStartTimeInSeconds;
    }

    void Update()
    {
        if (ShouldPublishMessage)
        {
            PublishMessage();
        }
    }
}
