using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.RelaxedIkRos1;
using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Std;

using TMPro;

// Run this script to publish goals to relaxed IK algorithm
public class EEPoseGoalPublisher : MonoBehaviour
{
    public TextMeshPro timetxt; 

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

    // Spawn a copy of the gripper that is manipulable by MRTK input
    [SerializeField]
    GameObject manipulableGripperPrefab;
    List<GameObject> manipulableGrippers;

    // Spawn the starting pose of the robot for relaxedIK at the
    // position of the robots gripper. This will be (0, 0, 0)
    // which is tracked by the child of the poseRef prefab
    [SerializeField]
    GameObject poseRefPrefab; 
    List<Transform> poseGoals;

    // List of messages with EE goals for each EE
    PoseMsg[] poseMsgs;

    // World objct keeps track of where the scene origin is
    // Since the HoloLens main camera alway starts at (0,0,0)
    [SerializeField]
    GameObject world; 

    void Start()
    {
        m_ROS = ROSConnection.GetOrCreateInstance();
        m_ROS.RegisterPublisher<EEPoseGoalsMsg>("/relaxed_ik/ee_pose_goals");

        manipulableGrippers = new List<GameObject>();
        poseGoals = new List<Transform>();

        // Add PoseMsg for each EELink
        poseMsgs = new PoseMsg[EELinks.Count];


        foreach (Transform EELink in EELinks)
        {
            // Instantiate manipulable gripper at the position of the hand
            GameObject gripper = Instantiate(manipulableGripperPrefab, world.transform) as GameObject;
            gripper.transform.position = EELink.transform.position;
            gripper.transform.rotation = EELink.transform.rotation;

            // If the user lets go of the gripper, put it back at the current position of the EELink
            gripper.GetComponent<ResetPosition>().EELink = EELink;
            manipulableGrippers.Add(gripper);

            // Instantiate the pose reference at the starting position of the EElink which is (0, 0, 0) for relaxed IK
            GameObject poseRef = Instantiate(poseRefPrefab, world.transform) as GameObject;
            poseRef.transform.position = EELink.transform.position;
            poseRef.transform.rotation = EELink.transform.rotation;
            poseGoals.Add(poseRef.transform.Find("PoseGoal"));
        }
    }

    void PublishMessage()
    {
        var publishTime = Clock.time;

        for (int i = 0; i < manipulableGrippers.Count; i++)
        {
            // Transform the position of the manipulated gripper to the coordinate frame of the pose goal
            Vector3 transformedPosition = poseGoals[i].InverseTransformPoint(manipulableGrippers[i].transform.position);

            poseMsgs[i] = new PoseMsg
            {
                // Transform from Unity coordinates to ROS coordinates
                position = transformedPosition.To<FLU>(),
                orientation = (manipulableGrippers[i].transform.rotation*world.transform.rotation).To<FLU>()
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
        timetxt.text = Clock.FrameStartTimeInSeconds.ToString();
        if (ShouldPublishMessage)
        {
            PublishMessage();
        }
    }
}
