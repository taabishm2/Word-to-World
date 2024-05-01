using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class PointToAdd : MonoBehaviour
{
    private XRRayInteractor rayInteractor;

    void Start()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
    }

    void Update()
    {
        RaycastHit hit;
        bool isHit = rayInteractor.TryGetCurrent3DRaycastHit(out hit);

        if (isHit)
        {
            // The ray hit an object
            Vector3 hitPoint = hit.point;
            Debug.Log("Hit point coordinates: " + hitPoint);
        }
        else
        {
            // The ray did not hit any object
            Debug.Log("The ray did not hit any object.");
        }
    }
}