using System;
using System.Collections;
using System.Linq;
using RosMessageTypes.Geometry;
using RosMessageTypes.Control;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;
using RosMessageTypes.MoveitMsgs;
using RosMessageTypes.Moveit;
using RosMessageTypes.FetchUnity;
using System.Collections.Generic;
using Unity.Robotics.Core;


public class PoseQuery : MonoBehaviour
{
    // Hardcoded variables
    const int k_NumRobotJoints = 7;
    const float k_JointAssignmentWait = 0.1f;
    const float k_PoseAssignmentWait = 0.5f;

    // Variables required for ROS communication
    [SerializeField]
    string m_RosServiceName = "pose_query";
    public string RosServiceName { get => m_RosServiceName; set => m_RosServiceName = value; }

    [SerializeField]
    GameObject m_Fetch;
    public GameObject Fetch { get => m_Fetch; set => m_Fetch = value; }
    [SerializeField]
    GameObject m_Target;
    public GameObject Target { get => m_Target; set => m_Target = value; }

    // Assures that the gripper is always positioned above the m_Target cube before grasping.
    readonly Quaternion m_PoseOrientation = Quaternion.Euler(90, 90, 0);
    readonly Vector3 m_PoseOffset = Vector3.up * 0.1f;

    FetchJointController fetchJointController;

    // ROS Connector
    ROSConnection m_Ros;

    public Material validTargetMat;
    public Material invalidTargetMat;

    /// <summary>
    ///     Find all robot joints in Awake() and add them to the jointArticulationBodies array.
    ///     Find left and right finger joints and assign them to their respective articulation body objects.
    /// </summary>
    void Start()
    {
        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.RegisterRosService<PoseQueryServiceRequest, PoseQueryServiceResponse>(m_RosServiceName);
        fetchJointController = GetComponent<FetchJointController>();
    }


    /// <summary>
    ///     Get the current values of the robot's joint angles.
    /// </summary>
    /// <returns>FetchMoveitJoints</returns>
    RobotStateMsg CurrentJointConfig()
    {
        var joints = new RobotStateMsg();

        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            joints.joint_state.name[i] = FetchJointController.LinkNames[i + 1];
            joints.joint_state.position[i] = fetchJointController.GetJointPosition(i);
        }

        return joints;
    }

    /// <summary>
    ///     Create a new PoseQueryServiceRequest with the current values of the robot's joint angles,
    ///     the target cube's current position and rotation, and the targetPlacement position and rotation.
    ///     Call the FetchMoverService using the ROSConnection and if a trajectory is successfully planned,
    ///     execute the trajectories in a coroutine.
    /// </summary>
    public void QueryMove()
    {
        var request = new PoseQueryServiceRequest();

        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            request.start_joints_torso.joints[i] = fetchJointController.GetJointTarget(i);
        }

        // Pick Pose
        request.end_pose = new PoseMsg
        {
            //position = (m_Target.transform.position + m_PoseOffset).To<FLU>(),
            position = m_Target.transform.position.To<FLU>(),
            // The hardcoded x/z angles assure that the gripper is always positioned above the target cube before grasping.
            orientation = Quaternion.Euler(90, m_Target.transform.eulerAngles.y, 0).To<FLU>()
        };

        m_Ros.SendServiceMessage<PoseQueryServiceResponse>(m_RosServiceName, request, TrajectoryResponse);
    }

    void TrajectoryResponse(PoseQueryServiceResponse response)
    {
        Vector3 pos = response.end_pose.position.From<FLU>();

        if (response.trajectories.Length > 0)
        {
            m_Target.GetComponent<MeshRenderer>().material = validTargetMat;
        }
        else
        {
            m_Target.GetComponent<MeshRenderer>().material = invalidTargetMat;
        }
    }

    /// <summary>
    ///     Execute the returned trajectories from the FetchMoverService.
    ///     The expectation is that the FetchMoverService will return four trajectory plans,
    ///     PreGrasp, Grasp, PickUp, and Place,
    ///     where each plan is an array of robot poses. A robot pose is the joint angle values
    ///     of the six robot joints.
    ///     Executing a single trajectory will iterate through every robot pose in the array while updating the
    ///     joint values on the robot.
    /// </summary>
    /// <param name="response"> FetchMoverServiceResponse received from fetch_moveit mover service running in ROS</param>
    /// <returns></returns>
    IEnumerator ExecuteTrajectories(FetchMoverServiceResponse response)
    {
        if (response.trajectories != null)
        {
            // For every trajectory plan returned
            for (var poseIndex = 0; poseIndex < response.trajectories.Length; poseIndex++)
            {
                // For every robot pose in trajectory plan
                foreach (var t in response.trajectories[poseIndex].joint_trajectory.points)
                {
                    var jointPositions = t.positions;
                    var result = jointPositions.Select(r => (float)r * Mathf.Rad2Deg).ToArray();

                    // Set the joint values for every joint
                    for (var joint = 0; joint < FetchJointController.LinkNames.Length; joint++)
                    {
                        fetchJointController.SetJointTarget(joint, result[joint]);
                    }

                    // Wait for robot to achieve pose for all joint assignments
                    yield return new WaitForSeconds(k_JointAssignmentWait);
                }

                // Close the gripper if completed executing the trajectory for the Grasp pose
                if (poseIndex == (int)Poses.Grasp)
                {
                    fetchJointController.CloseGripper();
                }

                // Wait for the robot to achieve the final pose from joint assignment
                yield return new WaitForSeconds(k_PoseAssignmentWait);
            }

            // All trajectories have been executed, open the gripper to place the target cube
            fetchJointController.OpenGripper();
        }
    }

    enum Poses
    {
        PreGrasp,
        Grasp,
        PickUp,
        Place
    }
}