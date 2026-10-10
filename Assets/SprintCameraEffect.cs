using UnityEngine;
using Unity.Cinemachine;

public class SprintCameraEffect : MonoBehaviour
{
    public CinemachineCamera cinemachineCamera;

    public float normalFOV = 55f;
    public float sprintFOV = 65f;
    public float transitionSpeed = 5f;

    private bool isSprinting;

    void Start()
    {
        if (cinemachineCamera != null)
        {
            cinemachineCamera.Lens.FieldOfView = normalFOV;
        }
    }

    void Update()
    {
        if (cinemachineCamera == null)
            return;

        float targetFOV = isSprinting ? sprintFOV : normalFOV;

        var lens = cinemachineCamera.Lens;
        lens.FieldOfView = Mathf.Lerp(
            lens.FieldOfView,
            targetFOV,
            Time.deltaTime * transitionSpeed
        );

        cinemachineCamera.Lens = lens;
    }

    public void SetSprinting(bool sprinting)
    {
        isSprinting = sprinting;
    }
}

