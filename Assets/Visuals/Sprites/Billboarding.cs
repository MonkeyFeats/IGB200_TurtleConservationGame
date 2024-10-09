using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private bool fullRotation = false;
    private void Start()
    {
        // Find the main camera in the scene
        mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        // Ensure the camera is not null
        if (mainCamera != null)
        {
            Vector3 lookDirection = mainCamera.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(lookDirection);

            if (!fullRotation)
            {

                transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z);
            }
        }
    }
}
