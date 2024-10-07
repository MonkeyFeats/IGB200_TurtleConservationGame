using UnityEngine;

public class Floatie : MonoBehaviour
{
    private ConfigurableJoint configurableJoint;

    public float swingMin = -45f; // Minimum rotation angle for the joint
    public float swingMax = 45f;  // Maximum rotation angle for the joint

    // Reference to the previous floatie, to set up the joint
    public void SetupJoint(GameObject previousFloatie)
    {
        if (previousFloatie != null)
        {
            configurableJoint = gameObject.AddComponent<ConfigurableJoint>();
            Rigidbody connectedBody = previousFloatie.GetComponent<Rigidbody>();

            if (connectedBody != null)
            {
                configurableJoint.connectedBody = connectedBody;

                // Set joint to restrict movement to XZ axis only (ignore Y axis rotation)
                configurableJoint.angularXMotion = ConfigurableJointMotion.Limited;
                configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
                configurableJoint.angularZMotion = ConfigurableJointMotion.Limited;

                // Define rotation limits around XZ axis
                SoftJointLimit lowLimit = new SoftJointLimit();
                lowLimit.limit = swingMin;
                configurableJoint.lowAngularXLimit = lowLimit;

                SoftJointLimit highLimit = new SoftJointLimit();
                highLimit.limit = swingMax;
                configurableJoint.highAngularXLimit = highLimit;

                SoftJointLimit zLimit = new SoftJointLimit();
                zLimit.limit = swingMax; // You can control Z axis rotation limit separately
                configurableJoint.angularZLimit = zLimit;

                // Adjust anchor points if needed
                configurableJoint.anchor = Vector3.zero;
                configurableJoint.autoConfigureConnectedAnchor = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            // Call the net closing logic here
           // NetController.Instance.CloseNet();
        }
    }
}
