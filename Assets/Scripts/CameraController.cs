using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private CinemachineCamera cinemachineCamera;
    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 12f;

    private CinemachineOrbitalFollow cinemachineOrbitalFollow;
    private Vector3 targetFollowOffset;

    private void Start()
    {
        cinemachineOrbitalFollow = cinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
        targetFollowOffset = cinemachineOrbitalFollow.TargetOffset;
    }
    private void Update()
    {
        HandleCameramMovement();
        HandleCameraRotation();
        HandleCameraZoom();
    }

    private void HandleCameramMovement()
    {
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();

        float moveSpeed = 10f;

        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleCameraRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();

        float rotationSpeed = 100f;

        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    private void HandleCameraZoom()
    {
        float zoomIncreaseAmount = 1f;

        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;

        float zoomSpeed = 5f;

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        cinemachineOrbitalFollow.TargetOffset = Vector3.Lerp(cinemachineOrbitalFollow.TargetOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }
}
