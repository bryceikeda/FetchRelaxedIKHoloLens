//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.FetchUnity
{
    [Serializable]
    public class FetchMoverServiceRequest : Message
    {
        public const string k_RosMessageName = "fetch_teleop/FetchMoverService";
        public override string RosMessageName => k_RosMessageName;

        public FetchMoveitJointsMsg joints_input;
        public Geometry.PoseMsg pick_pose;
        public Geometry.PoseMsg place_pose;

        public FetchMoverServiceRequest()
        {
            this.joints_input = new FetchMoveitJointsMsg();
            this.pick_pose = new Geometry.PoseMsg();
            this.place_pose = new Geometry.PoseMsg();
        }

        public FetchMoverServiceRequest(FetchMoveitJointsMsg joints_input, Geometry.PoseMsg pick_pose, Geometry.PoseMsg place_pose)
        {
            this.joints_input = joints_input;
            this.pick_pose = pick_pose;
            this.place_pose = place_pose;
        }

        public static FetchMoverServiceRequest Deserialize(MessageDeserializer deserializer) => new FetchMoverServiceRequest(deserializer);

        private FetchMoverServiceRequest(MessageDeserializer deserializer)
        {
            this.joints_input = FetchMoveitJointsMsg.Deserialize(deserializer);
            this.pick_pose = Geometry.PoseMsg.Deserialize(deserializer);
            this.place_pose = Geometry.PoseMsg.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.joints_input);
            serializer.Write(this.pick_pose);
            serializer.Write(this.place_pose);
        }

        public override string ToString()
        {
            return "FetchMoverServiceRequest: " +
            "\njoints_input: " + joints_input.ToString() +
            "\npick_pose: " + pick_pose.ToString() +
            "\nplace_pose: " + place_pose.ToString();
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
