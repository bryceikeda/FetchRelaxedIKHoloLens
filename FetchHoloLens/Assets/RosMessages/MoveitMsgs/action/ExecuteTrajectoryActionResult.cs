using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Std;
using RosMessageTypes.Actionlib;

namespace RosMessageTypes.MoveitMsgs
{
    public class ExecuteTrajectoryActionResult : ActionResult<ExecuteTrajectoryResult>
    {
        public const string k_RosMessageName = "moveit_msgs-3175099a2b7fa75f74c36850a716b0fe603c8947/ExecuteTrajectoryActionResult";
        public override string RosMessageName => k_RosMessageName;


        public ExecuteTrajectoryActionResult() : base()
        {
            this.result = new ExecuteTrajectoryResult();
        }

        public ExecuteTrajectoryActionResult(HeaderMsg header, GoalStatusMsg status, ExecuteTrajectoryResult result) : base(header, status)
        {
            this.result = result;
        }
        public static ExecuteTrajectoryActionResult Deserialize(MessageDeserializer deserializer) => new ExecuteTrajectoryActionResult(deserializer);

        ExecuteTrajectoryActionResult(MessageDeserializer deserializer) : base(deserializer)
        {
            this.result = ExecuteTrajectoryResult.Deserialize(deserializer);
        }
        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.status);
            serializer.Write(this.result);
        }


#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
