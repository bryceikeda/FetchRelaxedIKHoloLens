using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.RelaxedIkRos1;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Std;

// Run this script to publish goals to relaxed IK algorithm
public class EEPoseGoalPublisher : MonoBehaviour
{
    // Publishing rate information 
    [SerializeField]
    double m_PublishRateHz = 100f;
    double m_LastPublishTimeSeconds;
    double PublishPeriodSeconds => 1.0f / m_PublishRateHz;
    bool ShouldPublishMessage => Clock.FrameStartTimeInSeconds - PublishPeriodSeconds > m_LastPublishTimeSeconds;

    ROSConnection m_ROS;

    // Some robots have one end effector like the Fetch, others have two like the Baxter
    [SerializeField]
    List<Transform> EELinks;

    List<GameObject> manipulableGrippers;

    // Spawn the starting pose of the robot for relaxedIK at the
    // position of the robots gripper. This will be (0, 0, 0)
    // which is tracked by the child of the poseRef prefab
    [SerializeField]
    GameObject eeTrackingPrefab;

    // List of messages with EE goals for each EE
    PoseMsg[] poseMsgs;

    // World objct keeps track of where the scene origin is
    // Since the HoloLens main camera alway starts at (0,0,0)
    [SerializeField]
    GameObject base_link;

    void Start()
    {
        m_ROS = ROSConnection.GetOrCreateInstance();
        m_ROS.RegisterPublisher<EEPoseGoalsMsg>("/relaxed_ik/ee_pose_goals");

        manipulableGrippers = new List<GameObject>();

        // Add PoseMsg for each EELink
        poseMsgs = new PoseMsg[EELinks.Count];


        foreach (Transform EELink in EELinks)
        {
            // Instantiate the pose reference at the starting position of the EElink which is (0, 0, 0) for relaxed IK
            GameObject poseRef = Instantiate(eeTrackingPrefab, base_link.transform);

            // If the user lets go of the gripper, put it back at the current position of the EELink
            poseRef.GetComponent<ResetPosition>().EELink = EELink;
            manipulableGrippers.Add(poseRef.transform.GetChild(0).gameObject);
        }
    }

    void PublishMessage()
    {
        var publishTime = Clock.time;

        for (int i = 0; i < manipulableGrippers.Count; i++)
        {

            var pos = base_link.transform.InverseTransformPoint(manipulableGrippers[i].transform.position);
            var rot = Quaternion.Inverse(base_link.transform.rotation) * manipulableGrippers[i].transform.rotation;

            var grip_pos = pos.To<FLU>();
            var grip_ori = rot.To<FLU>();

            poseMsgs[i] = new PoseMsg
            {
                // Transform from Unity coordinates to ROS coordinates
                position = new PointMsg(grip_pos.x, grip_pos.y, grip_pos.z),
                orientation = new QuaternionMsg(grip_ori.x, grip_ori.y, grip_ori.z, grip_ori.w),
            };
        }

        // Add header and pose msgs to new end effector pos goal message
        EEPoseGoalsMsg eePoseGoalMsg = new EEPoseGoalsMsg
        {
            header = new HeaderMsg(),
            ee_poses = poseMsgs
        };

        m_ROS.Publish("/relaxed_ik/ee_pose_goals", eePoseGoalMsg);
        m_LastPublishTimeSeconds = publishTime;
    }


    // Update is called once per frame
    void Update()
    {
        if (ShouldPublishMessage)
        {
            PublishMessage();
        }
    }
}