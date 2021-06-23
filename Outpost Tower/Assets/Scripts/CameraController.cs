using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    CinemachineVirtualCamera virtualCamera;
    public float speed = 2f;

    public float zoomSpeed = 1f;
    public float maxZoom = 10.4f;
    public float defaultZoom = 5;
    public float minZoom = 2f;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(moveX, moveY) * speed * Time.deltaTime;

        transform.position += (Vector3)move;

        if (Input.GetKey(KeyCode.E))
            ZoomIn();
        else if (Input.GetKey(KeyCode.Q))
            ZoomOut();
    }

    public void ZoomIn()
    {
        float zoom = zoomSpeed * Time.deltaTime;
        virtualCamera.m_Lens.OrthographicSize = Mathf.Max(virtualCamera.m_Lens.OrthographicSize - zoom, minZoom);
    }

    public void ZoomOut()
    {
        float zoom = zoomSpeed * Time.deltaTime;
        virtualCamera.m_Lens.OrthographicSize = Mathf.Min(virtualCamera.m_Lens.OrthographicSize + zoom, maxZoom);
    }
}
