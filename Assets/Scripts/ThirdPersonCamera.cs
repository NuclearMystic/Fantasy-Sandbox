using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 1.5f, -3f);
    public float smoothSpeed = 5f;
    public float rotationSpeed = 3f;

    [Header("Zoom Settings")]
    public float minZoom = 1.5f;
    public float maxZoom = 6f;
    public float zoomSpeed = 2f;

    private float currentZoom;
    private float yaw = 0f;
    private float pitch = 15f;

    private bool cameraControlEnabled = true;

    void Start()
    {
        SetCameraControl(true);
        currentZoom = offset.magnitude;
    }

    void LateUpdate()
    {
        if (cameraControlEnabled)
        {
            HandleCameraRotation();
            HandleCameraZoom();
        }

        UpdateCameraPosition();

        Vector3 forward = Camera.main.transform.forward;
        Shader.SetGlobalVector("_CameraForward", new Vector4(forward.x, forward.y, forward.z, 0));
    }

    public void SetCameraControl(bool isEnabled)
    {
        cameraControlEnabled = isEnabled;

        // Set cursor based on the same value — if camera is enabled, cursor should be locked
        UIController.Instance?.SetCursorState(isEnabled);
    }

    void HandleCameraRotation()
    {
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, -30f, 60f);
    }

    void HandleCameraZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    void UpdateCameraPosition()
    {
        Vector3 desiredOffset = new Vector3(0, 0, -currentZoom);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredCameraPos = target.position + rotation * desiredOffset;

        Vector3 direction = (desiredCameraPos - target.position).normalized;
        float targetDistance = currentZoom;
        float sphereRadius = 0.5f;

        if (Physics.SphereCast(target.position, sphereRadius, direction, out RaycastHit hit, currentZoom))
        {
            targetDistance = hit.distance - sphereRadius;
        }

        targetDistance = Mathf.Clamp(targetDistance, minZoom, maxZoom);

        Vector3 finalCameraPos = target.position + rotation * new Vector3(0, 0, -targetDistance);
        transform.position = Vector3.Lerp(transform.position, finalCameraPos, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position);
    }
}
