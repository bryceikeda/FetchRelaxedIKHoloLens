using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Std;
using RosMessageTypes.Actionlib;

namespace RosMessageTypes.MoveitMsgs
{
    public class PlaceActionGoal : ActionGoal<PlaceGoal>
    {
        public const string k_RosMessageName = "moveit_msgs-3175099a2b7fa75f74c36850a716b0fe603c8947/PlaceActionGoal";
        public override string RosMessageName => k_RosMessageName;


        public PlaceActionGoal() : base()
        {
            this.goal = new PlaceGoal();
        }

        public PlaceActionGoal(HeaderMsg header, GoalIDMsg goal_id, PlaceGoal goal) : base(header, goal_id)
        {
            this.goal = goal;
        }
        public static PlaceActionGoal Deserialize(MessageDeserializer deserializer) => new PlaceActionGoal(deserializer);

        PlaceActionGoal(MessageDeserializer deserializer) : base(deserializer)
        {
            this.goal = PlaceGoal.Deserialize(deserializer);
        }
        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.goal_id);
            serializer.Write(this.goal);
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
