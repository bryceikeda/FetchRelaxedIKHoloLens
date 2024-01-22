using RosMessageTypes.Sensor;
using RosMessageTypes.LfdReceiver; 
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using RosMessageTypes.RelaxedIkRos1;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using static UnityEditor.PlayerSettings;
using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine.InputSystem;

public class DemonstrationPublisher : MonoBehaviour
{
    // ROS Connector
    ROSConnection m_Ros;
    [SerializeField]
    string m_SubTopicName = "/relaxed_ik/joint_angle_solutions";
    [SerializeField]
    string m_RosServiceName = "/unity/record";

    [SerializeField]
    string m_PubTopicName = "/unity/demonstration";

    // Publishing rate information 
    [SerializeField]
    double m_PublishRateHz = 100f;
    double m_LastPublishTimeSeconds;
    double PublishPeriodSeconds => 1.0f / m_PublishRateHz;
    bool ShouldPublishMessage => Clock.FrameStartTimeInSeconds - PublishPeriodSeconds > m_LastPublishTimeSeconds;

    JointStateMsg m_State = new JointStateMsg();

    bool picking = false;
    bool placing = false; 

    // Start is called before the first frame update
    void Start()
    {
        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.RegisterRosService<RecordRequest, RecordResponse>(m_RosServiceName);
        m_Ros.Subscribe<JointStateMsg>(m_SubTopicName, GetJointAngles);
        m_Ros.RegisterPublisher<DemonstrationMsg>(m_PubTopicName);
    }

    private void Update()
    {
        if (ShouldPublishMessage)
        {
            PublishMessage();
        }
    }

    void GetJointAngles(JointStateMsg msg)
    {
        m_State = msg; 
    }
    public void SignalToRecord()
    {
        var request = new RecordRequest();
        request.begin.data = true; 

        m_Ros.SendServiceMessage<RecordResponse>(m_RosServiceName, request, ReceivedResponse);
    }
    void PublishMessage()
    {
        var publishTime = Clock.time;

        DemonstrationMsg msg = new DemonstrationMsg
        {
            header = new HeaderMsg(),
            joint_states = m_State,
            pick = new BoolMsg(picking),
            place = new BoolMsg(placing)
        };

        m_Ros.Publish(m_PubTopicName, msg);
        m_LastPublishTimeSeconds = publishTime;
        placing = false; 
    }
    public void ReceivedResponse(RecordResponse response)
    {
        Debug.Log("Received response: " + response.received.data);
    }

    public void pick()
    {
        picking = true;
        placing = false; 
    }

    public void place()
    {
        placing = true;
        picking = false; 
    }

}
