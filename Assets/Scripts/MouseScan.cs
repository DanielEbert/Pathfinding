using UnityEngine;
using System.Collections;

public class MouseScan : MonoBehaviour {

    public LayerMask mousePosRaycastLM;

    [Header("Components")]
    public Camera mainCamera;

	void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //Vector3 hitPos = MousePosRaycast();
        }
    }

    Vector3 MousePosRaycast()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, 500, mousePosRaycastLM))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
}
