using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMapToggle : MonoBehaviour
{
    public Camera mainCamera;
    public float normalZoom = 5f;
    public float zoomedOut = 20f;
    public float zoomSpeed = 5f;

    private float targetZoom;
    private bool isZoomedOut = false;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        targetZoom = normalZoom;
        mainCamera.orthographicSize = normalZoom;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.M))
        {
            isZoomedOut = !isZoomedOut;
            targetZoom = isZoomedOut ? zoomedOut : normalZoom;
        }

        if (!Mathf.Approximately(mainCamera.orthographicSize, targetZoom))
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
        }
    }
}


