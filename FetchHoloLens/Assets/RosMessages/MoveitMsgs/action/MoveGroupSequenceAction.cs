using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;


namespace RosMessageTypes.MoveitMsgs
{
    public class MoveGroupSequenceAction : Action<MoveGroupSequenceActionGoal, MoveGroupSequenceActionResult, MoveGroupSequenceActionFeedback, MoveGroupSequenceGoal, MoveGroupSequenceResult, MoveGroupSequenceFeedback>
    {
        public const string k_RosMessageName = "moveit_msgs-3175099a2b7fa75f74c36850a716b0fe603c8947/MoveGroupSequenceAction";
        public override string RosMessageName => k_RosMessageName;


        public MoveGroupSequenceAction() : base()
        {
            this.action_goal = new MoveGroupSequenceActionGoal();
            this.action_result = new MoveGroupSequenceActionResult();
            this.action_feedback = new MoveGroupSequenceActionFeedback();
        }

        public static MoveGroupSequenceAction Deserialize(MessageDeserializer deserializer) => new MoveGroupSequenceAction(deserializer);

        MoveGroupSequenceAction(MessageDeserializer deserializer)
        {
            this.action_goal = MoveGroupSequenceActionGoal.Deserialize(deserializer);
            this.action_result = MoveGroupSequenceActionResult.Deserialize(deserializer);
            this.action_feedback = MoveGroupSequenceActionFeedback.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.action_goal);
            serializer.Write(this.action_result);
            serializer.Write(this.action_feedback);
        }

    }
}
