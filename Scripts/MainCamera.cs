using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour
{
    [SerializeField] private bool cameraSmoothMode = true;
    [SerializeField][Range(0, 1)] private float smoothSpeed = 0.15f;
    [SerializeField] private Transform moveTo;
    [SerializeField] private float mixCameraScale = 1;
    [SerializeField] private float maxCameraScale = 10;
    [SerializeField][Range(0.5f, 5.0f)] private float scrollSpeed = 2.5f;
    [SerializeField][Range(1f, 100f)] private float backgroudDistance = 50f;
    [SerializeField] private GameObject backgroud;

    private Camera _camera;
    private Material backgroudMaterial;

    void Start()
    {
        _camera = GetComponent<Camera>();
        if (backgroud != null)
            backgroudMaterial = backgroud.GetComponent<MeshRenderer>()?.materials[0];

        if (mixCameraScale > maxCameraScale)
            throw new IndexOutOfRangeException($"mixCameraScale ({mixCameraScale}) was large than maxCameraScale ({maxCameraScale})");
    }


    private void Update()
    {
        ScrollCameraView();

        BackgroudMove();
    }

    private void FixedUpdate()
    {
        CameraMoveTo();
    }

    private void ScrollCameraView()
    {
        float scrollwheel = Input.GetAxis("Mouse ScrollWheel");

        if (scrollwheel < 0)
        {
            _camera.orthographicSize -= scrollwheel * scrollSpeed;
            if (_camera.orthographicSize > maxCameraScale)
                _camera.orthographicSize = maxCameraScale;

        }
        else if (scrollwheel > 0)
        {
            _camera.orthographicSize -= scrollwheel * scrollSpeed;
            if (_camera.orthographicSize < mixCameraScale)
                _camera.orthographicSize = mixCameraScale;
        }
    }

    private void CameraMoveTo()
    {
        if (moveTo == null)
            return;

        if (cameraSmoothMode)
        {
            Vector3 from = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Vector3 to = new Vector3(moveTo.transform.position.x, moveTo.transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(from, to, smoothSpeed);

            //transform.LookAt(moveTo.transform);
        }
        else
        {
            transform.position = new Vector3(moveTo.transform.position.x, moveTo.transform.position.y, -10);
        }
    }

    private void BackgroudMove()
    {
        if (backgroud == null || backgroudDistance == 1)
            return;

        Vector2 camPos = _camera.transform.position;
        backgroud.transform.position = new Vector3(
        camPos.x - (camPos.x * (1 / backgroudDistance)),
        camPos.y - (camPos.y * (1 / backgroudDistance)),
        backgroud.transform.position.z);
    }

}
