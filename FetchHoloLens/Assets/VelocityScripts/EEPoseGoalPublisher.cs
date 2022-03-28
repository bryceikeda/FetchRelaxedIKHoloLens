using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.RelaxedIkRos1;
using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;

using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Std;
public class EEPoseGoalPublisher : MonoBehaviour
{
    [SerializeField]
    double m_PublishRateHz = 100f;
    double m_LastPublishTimeSeconds;
    double PublishPeriodSeconds => 1.0f / m_PublishRateHz;
    bool ShouldPublishMessage => Clock.FrameStartTimeInSeconds - PublishPeriodSeconds > m_LastPublishTimeSeconds;

    ROSConnection m_ROS;
    public List<Transform> EELinks;
    public GameObject manipulableGripperPrefab;

    public GameObject poseRefGripperPrefab; 


    private List<GameObject> manipulableGrippers;
    private List<Transform> poseGoals;
    public PoseMsg[] EEPoses;
    Vector3 pos = new Vector3(.315f, .68f, 0.74f);
    Quaternion rotation = Quaternion.Euler(0f, 23f, 13f);

    public GameObject test;

    public Vector3 transformedPosition; 

    //Vector3 pos = new Vector3(0f, 0f, 0f);
    //Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
    // Start is called before the first frame update
    void Start()
    {
        m_ROS = ROSConnection.GetOrCreateInstance();
        m_ROS.RegisterPublisher<EEPoseGoalsMsg>("/relaxed_ik/ee_pose_goals");

        // Instantiate array that holds gripper transforms
        manipulableGrippers = new List<GameObject>();
        poseGoals = new List<Transform>();

        // Add PoseMsg for each EELink
        EEPoses = new PoseMsg[EELinks.Count];


        foreach (Transform EELink in EELinks)
        {
            //GameObject gripper = manipulableGripperPrefab; //Instantiate(manipulableGripperPrefab, pos, rotation) as GameObject;
            GameObject gripper = Instantiate(manipulableGripperPrefab, EELink.position, EELink.rotation) as GameObject;
            gripper.GetComponent<ResetPosition>().EELink = EELink;
            manipulableGrippers.Add(gripper);

            GameObject poseRefGripper = Instantiate(poseRefGripperPrefab, EELink.position, EELink.rotation) as GameObject;
            poseGoals.Add(poseRefGripper.transform.Find("PoseGoal"));
        }
    }

    void PublishMessage()
    {
        var publishTime = Clock.time;
        for (int i = 0; i < manipulableGrippers.Count; i++)
        {
            // transform world space manipulable gripper into the local coordinate space as the reference gripper
            transformedPosition = poseGoals[i].InverseTransformPoint(manipulableGrippers[i].transform.position);

            Debug.Log(poseGoals[i].InverseTransformPoint(new Vector3(0f, 0f, 0f)));

            EEPoses[i] = new PoseMsg
            {
                //position = pos.To<FLU>(),
                //orientation = rotation.To<FLU>()
                position = transformedPosition.To<FLU>(),
                orientation = manipulableGrippers[i].transform.rotation.To<FLU>()
            };
        }

        EEPoseGoalsMsg eePoseGoalMsg = new EEPoseGoalsMsg
        {
            header = new HeaderMsg(),
            ee_poses = EEPoses 
        };

        m_LastPublishTimeSeconds = publishTime;
        m_ROS.Publish("/relaxed_ik/ee_pose_goals", eePoseGoalMsg);
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
