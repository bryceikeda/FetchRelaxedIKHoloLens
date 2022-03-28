using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Std;
using RosMessageTypes.Actionlib;

namespace RosMessageTypes.MoveitMsgs
{
    public class PickupActionFeedback : ActionFeedback<PickupFeedback>
    {
        public const string k_RosMessageName = "moveit_msgs-3175099a2b7fa75f74c36850a716b0fe603c8947/PickupActionFeedback";
        public override string RosMessageName => k_RosMessageName;


        public PickupActionFeedback() : base()
        {
            this.feedback = new PickupFeedback();
        }

        public PickupActionFeedback(HeaderMsg header, GoalStatusMsg status, PickupFeedback feedback) : base(header, status)
        {
            this.feedback = feedback;
        }
        public static PickupActionFeedback Deserialize(MessageDeserializer deserializer) => new PickupActionFeedback(deserializer);

        PickupActionFeedback(MessageDeserializer deserializer) : base(deserializer)
        {
            this.feedback = PickupFeedback.Deserialize(deserializer);
        }
        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.status);
            serializer.Write(this.feedback);
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
