using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollCam : MonoBehaviour
{
    [SerializeField]
    private float minZoom;

    [SerializeField]
    private float maxZoom;

    [SerializeField]
    [Range(1, 5)]
    private float zoomStep;

    private void Update()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        float size = Camera.main.orthographicSize;

        if (scroll < 0)
            size *= zoomStep;

        if (scroll > 0)
            size /= zoomStep;

        size = Mathf.Clamp(size, minZoom, maxZoom);

        Camera.main.orthographicSize = size;
    }
}
