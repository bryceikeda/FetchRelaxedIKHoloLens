using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.RelaxedIkRos1;
using Unity.Robotics.Core;
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
    GameObject poseRefGameObject;

    // List of messages with EE goals for each EE
    PoseMsg[] poseMsgs;

    // World objct keeps track of where the scene origin is
    // Since the HoloLens main camera alway starts at (0,0,0)
    [SerializeField]
    GameObject base_link;

    [SerializeField]
    PointMsg eeInitialPositionROSCoord;
    [SerializeField]
    QuaternionMsg eeInitialOrientationROSCoord;

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
            GameObject poseRef = Instantiate(poseRefGameObject, base_link.transform) as GameObject;
            poseRef.transform.localPosition = eeInitialPositionROSCoord.From<FLU>();
            poseRef.transform.localRotation = eeInitialOrientationROSCoord.From<FLU>();

            // If the user lets go of the gripper, put it back at the current position of the EELink
            poseRef.GetComponent<ResetPosition>().EELink = EELink;
            manipulableGrippers.Add(poseRef);
        }
    }

    void PublishMessage()
    {
        var publishTime = Clock.time;

        for (int i = 0; i < manipulableGrippers.Count; i++)
        {
            var grip_pos = manipulableGrippers[i].transform.localPosition.To<FLU>();
            var grip_ori = manipulableGrippers[i].transform.localRotation.To<FLU>();
            Debug.Log(grip_pos);
            Debug.Log(grip_ori);
            poseMsgs[i] = new PoseMsg
            {
                // Transform from Unity coordinates to ROS coordinates
                position = new PointMsg(grip_pos.x, grip_pos.y, grip_pos.z),
                orientation = new QuaternionMsg(grip_ori.x, grip_ori.y, grip_ori.z, -grip_ori.w),
                //position = new PointMsg(grip_pos.x + .89347f, grip_pos.y - .3215f, grip_pos.z + .40858f),
                // orientation = new QuaternionMsg(grip_ori.x - 0.7071f, grip_ori.y, grip_ori.z, - grip_ori.w - 0.2928945f),
                // position = new PointMsg(.89347f, -.3215f, .40858f),
                // orientation = new QuaternionMsg(-0.7071f, 0.0f, 0.0f, 0.7071f),
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