using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Std;
using RosMessageTypes.Actionlib;

namespace RosMessageTypes.MoveitMsgs
{
    public class MoveGroupActionFeedback : ActionFeedback<MoveGroupFeedback>
    {
        public const string k_RosMessageName = "moveit_msgs-3175099a2b7fa75f74c36850a716b0fe603c8947/MoveGroupActionFeedback";
        public override string RosMessageName => k_RosMessageName;


        public MoveGroupActionFeedback() : base()
        {
            this.feedback = new MoveGroupFeedback();
        }

        public MoveGroupActionFeedback(HeaderMsg header, GoalStatusMsg status, MoveGroupFeedback feedback) : base(header, status)
        {
            this.feedback = feedback;
        }
        public static MoveGroupActionFeedback Deserialize(MessageDeserializer deserializer) => new MoveGroupActionFeedback(deserializer);

        MoveGroupActionFeedback(MessageDeserializer deserializer) : base(deserializer)
        {
            this.feedback = MoveGroupFeedback.Deserialize(deserializer);
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
