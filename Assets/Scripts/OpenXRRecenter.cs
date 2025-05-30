using UnityEngine;

public class OpenXRRecenter : MonoBehaviour
{
    public Transform origin;  
    public Transform target;  
    public Transform head;    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))  
        {
            Recenter();
        }
    }

    void Recenter()
    {
        Vector3 offset = head.position - origin.position;
        offset.y = 0;
        origin.position = target.position - offset;

        Vector3 targetForward = target.forward;
        targetForward.y = 0;
        Vector3 cameraForward = head.forward;
        cameraForward.y = 0;

        float angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);

        origin.RotateAround(head.position, Vector3.up, angle);

        Debug.Log("front setting");
    }
}
