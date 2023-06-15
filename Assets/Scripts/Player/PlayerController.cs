using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Animator playerAnimator;
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private float cameraTurnSpeed;
    [SerializeField]
    private float cameraMinTurnAngle;
    [SerializeField]
    private float cameraMaxTurnAngle;
    [SerializeField]
    private float playerDistanceY;
    [SerializeField]
    private float playerDistanceZ;
    [SerializeField]
    private float blendSpeed = 0.1f;
    private float cameraRotX;
    private float cameraRotY;


    void Update()
    {
        MoveAndRotateCamera();
        UpdateMovementInput();
    }

    private void UpdateMovementInput()
    {
        float velocityX = Input.GetAxis("Horizontal");
        float velocityZ = Input.GetAxis("Vertical");

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && velocityZ >= 0)
        {
            velocityX *= 2;
            velocityZ *= 2;
        }

        playerAnimator.SetFloat("VelocityX", velocityX, blendSpeed, Time.deltaTime);
        playerAnimator.SetFloat("VelocityZ", velocityZ, blendSpeed, Time.deltaTime);
    }

    public void MoveAndRotateCamera()
    {
        // get the mouse inputs
        cameraRotY = Input.GetAxis("Mouse X") * cameraTurnSpeed;
        cameraRotX += Input.GetAxis("Mouse Y") * cameraTurnSpeed;
        // clamp the vertical rotation
        cameraRotX = Mathf.Clamp(cameraRotX, cameraMinTurnAngle, cameraMaxTurnAngle);
        // rotate the camera and the player
        playerCamera.transform.eulerAngles = new Vector3(-cameraRotX, playerCamera.transform.eulerAngles.y + cameraRotY, 0);
        playerTransform.eulerAngles = new Vector3(0, playerTransform.eulerAngles.y + cameraRotY, 0);
        // move the camera position
        Vector3 offset = new(0f, playerDistanceY, playerDistanceZ);
        playerCamera.transform.position = playerTransform.position - playerCamera.transform.forward * offset.z + playerCamera.transform.up * offset.y;
    }
}
