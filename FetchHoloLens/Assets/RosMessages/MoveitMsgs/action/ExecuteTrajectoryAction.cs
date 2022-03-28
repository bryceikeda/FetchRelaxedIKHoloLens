using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;


namespace RosMessageTypes.MoveitMsgs
{
    public class ExecuteTrajectoryAction : Action<ExecuteTrajectoryActionGoal, ExecuteTrajectoryActionResult, ExecuteTrajectoryActionFeedback, ExecuteTrajectoryGoal, ExecuteTrajectoryResult, ExecuteTrajectoryFeedback>
    {
        public const string k_RosMessageName = "moveit_msgs-3175099a2b7fa75f74c36850a716b0fe603c8947/ExecuteTrajectoryAction";
        public override string RosMessageName => k_RosMessageName;


        public ExecuteTrajectoryAction() : base()
        {
            this.action_goal = new ExecuteTrajectoryActionGoal();
            this.action_result = new ExecuteTrajectoryActionResult();
            this.action_feedback = new ExecuteTrajectoryActionFeedback();
        }

        public static ExecuteTrajectoryAction Deserialize(MessageDeserializer deserializer) => new ExecuteTrajectoryAction(deserializer);

        ExecuteTrajectoryAction(MessageDeserializer deserializer)
        {
            this.action_goal = ExecuteTrajectoryActionGoal.Deserialize(deserializer);
            this.action_result = ExecuteTrajectoryActionResult.Deserialize(deserializer);
            this.action_feedback = ExecuteTrajectoryActionFeedback.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.action_goal);
            serializer.Write(this.action_result);
            serializer.Write(this.action_feedback);
        }

    }
}
