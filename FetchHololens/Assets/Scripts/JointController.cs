using UnityEngine;
public class JointController : MonoBehaviour
{
    public enum JointType { Prismatic, Continuous, Revolute};
    public enum MovementAxis { x, y, z };

    [SerializeField]
    public JointType jointType;

    [SerializeField]
    public MovementAxis axis;

    [SerializeField]
    float lowerLimit;
    [SerializeField]
    float upperLimit;

    [SerializeField]
    Vector3 prismaticStartPos;
    [SerializeField]
    Vector3 rotationStartDeg;

    public Vector3 position; 

    public Transform jointTransform; 

    private void Awake()
    {
        jointTransform = gameObject.transform;
        prismaticStartPos = jointTransform.localPosition;
        rotationStartDeg = jointTransform.localEulerAngles; 
    }
    public void SetJointPosition(float jointValue)
    {
        if (jointType == JointType.Prismatic)
        {
            SetJointPositionPrismatic(ClampValue(jointValue));
        }
        else if (jointType == JointType.Revolute) 
        {
            SetJointRotation(ClampValue(jointValue)); 
        }
        else
        {
            SetJointRotation(jointValue);
        }
    }

    void SetJointPositionPrismatic(float jointValue)
    {
        if (axis == MovementAxis.x)
        {
            jointTransform.localPosition = new Vector3(prismaticStartPos.x + jointValue, prismaticStartPos.y, prismaticStartPos.z); 
            position = new Vector3(prismaticStartPos.x + jointValue, prismaticStartPos.y, prismaticStartPos.z);
        }
        else if (axis == MovementAxis.y)
        {
            jointTransform.localPosition = new Vector3(prismaticStartPos.x, prismaticStartPos.y + jointValue, prismaticStartPos.z);
            position = new Vector3(prismaticStartPos.x, prismaticStartPos.y + jointValue, prismaticStartPos.z);
        }
        else
        {
            jointTransform.localPosition = new Vector3(prismaticStartPos.x, prismaticStartPos.y, prismaticStartPos.z + jointValue);
            position = new Vector3(prismaticStartPos.x, prismaticStartPos.y, prismaticStartPos.z + jointValue);
        }
    }

    void SetJointRotation(float jointValue)
    {
        if (axis == MovementAxis.x)
        {
            jointTransform.localRotation = Quaternion.Euler(rotationStartDeg.x + jointValue * Mathf.Rad2Deg, rotationStartDeg.y, rotationStartDeg.z);
            position = new Vector3(rotationStartDeg.x + jointValue * Mathf.Rad2Deg, rotationStartDeg.y, rotationStartDeg.z); 
        }
        else if (axis == MovementAxis.y)
        {
            jointTransform.localRotation = Quaternion.Euler(rotationStartDeg.x, rotationStartDeg.y + jointValue * Mathf.Rad2Deg, rotationStartDeg.z);
            position = new Vector3(rotationStartDeg.x, rotationStartDeg.y + jointValue * Mathf.Rad2Deg, rotationStartDeg.z);
        }
        else
        {
            jointTransform.localRotation = Quaternion.Euler(rotationStartDeg.x, rotationStartDeg.y, rotationStartDeg.x + jointValue * Mathf.Rad2Deg);
            position = new Vector3(rotationStartDeg.x, rotationStartDeg.y, rotationStartDeg.x + jointValue * Mathf.Rad2Deg);
        }
    }

    float ClampValue(float jointValue)
    {
        if (jointValue > upperLimit)
        {
            return upperLimit; 
        }
        else if (jointValue < lowerLimit)
        {
            return lowerLimit;
        }

        return jointValue; 
    }

}
