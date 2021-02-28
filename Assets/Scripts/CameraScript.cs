using UnityEngine;
using System.Collections;
using System;

public class CameraScript : MonoBehaviour {

    public float moveSpeed = 3;

    public bool enableScrolling = true;
    public float scrollSpeed = 100;
    public float smoothTime = .5f;

    private Vector3 velocity;
    private Vector3 wantedCamPos;

    void Start()
    {
        wantedCamPos = transform.position;
    }

    void Update ()
    {
        Movement();

        if(enableScrolling && Input.GetMouseButtonDown(2))
        {
            StartCoroutine(Dragging());
        }

        transform.position = Vector3.SmoothDamp(transform.position, wantedCamPos, ref velocity, smoothTime);
    }

    IEnumerator Dragging()
    {
        wantedCamPos = transform.position;

        Vector2 lastPos = new Vector2(Input.mousePosition.x / Screen.height, Input.mousePosition.y / Screen.height);

        while(!Input.GetMouseButtonUp(2))
        {
            Vector2 curPos = new Vector2(Input.mousePosition.x / Screen.height, Input.mousePosition.y / Screen.height);
            
            wantedCamPos += (curPos.x - lastPos.x) * transform.right * scrollSpeed + (curPos.y - lastPos.y) * new Vector3(transform.forward.x, 0, transform.forward.z).normalized * scrollSpeed;
            lastPos = curPos;

            yield return null;
        }
    }

    void Movement()
    {
        Vector3 input = Vector3.zero;

        if (Input.GetKey(KeyCode.A)) input -= transform.right;
        if (Input.GetKey(KeyCode.D)) input += transform.right;
        if (Input.GetKey(KeyCode.W)) input += new Vector3(transform.forward.x, 0, transform.forward.z);
        if (Input.GetKey(KeyCode.S)) input -= new Vector3(transform.forward.x, 0, transform.forward.z);

        wantedCamPos += Time.deltaTime * moveSpeed * input.normalized;
    }
}
