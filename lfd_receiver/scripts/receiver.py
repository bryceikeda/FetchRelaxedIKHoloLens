#!/user/bin/env python3

import rospy
import sys

from lfd_receiver.srv import Record, RecordRequest, RecordResponse
from lfd_receiver.msg import Demonstration
from sensor_msgs.msg import JointState
from std_msgs.msg import Bool

def callback(data):
    rospy.loginfo(data)

def handle_request(req):
    print(req.begin.data)

    response = RecordResponse()
    response.received.data = True

    return response

def main():
    rospy.init_node('lfd_receiver')
    
    rospy.Subscriber('/unity/demonstration', Demonstration, callback)
    rospy.Service('/unity/record', Record, handle_request)

    print("Ready for record trigger")
    rospy.spin()


if __name__ == "__main__":
   main()
