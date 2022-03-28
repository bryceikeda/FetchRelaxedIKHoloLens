//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Moveit
{
    [Serializable]
    public class CheckIfRobotStateExistsInWarehouseRequest : Message
    {
        public const string k_RosMessageName = "moveit_msgs/CheckIfRobotStateExistsInWarehouse";
        public override string RosMessageName => k_RosMessageName;

        public string name;
        public string robot;

        public CheckIfRobotStateExistsInWarehouseRequest()
        {
            this.name = "";
            this.robot = "";
        }

        public CheckIfRobotStateExistsInWarehouseRequest(string name, string robot)
        {
            this.name = name;
            this.robot = robot;
        }

        public static CheckIfRobotStateExistsInWarehouseRequest Deserialize(MessageDeserializer deserializer) => new CheckIfRobotStateExistsInWarehouseRequest(deserializer);

        private CheckIfRobotStateExistsInWarehouseRequest(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.name);
            deserializer.Read(out this.robot);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.name);
            serializer.Write(this.robot);
        }

        public override string ToString()
        {
            return "CheckIfRobotStateExistsInWarehouseRequest: " +
            "\nname: " + name.ToString() +
            "\nrobot: " + robot.ToString();
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
