using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{

    Camera cam;
    public PlayerController player;

    Vector3 startPosition;

    public float cameraSpeed = 1f;

    void Start()
    {
        cam = GetComponent<Camera>();
        startPosition = transform.position;
    }
    void Update()
    {
        var worldPos = transform.position - startPosition;
        var screenPos = cam.WorldToViewportPoint(worldPos);
        var playerPos = cam.WorldToViewportPoint(player.transform.position);

        var movement = Vector3.Lerp(screenPos, playerPos, Mathf.Clamp01(cameraSpeed * Time.deltaTime));

        var world = cam.ViewportToWorldPoint(movement);

        transform.position = world + startPosition;
    }
}
