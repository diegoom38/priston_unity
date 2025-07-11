using UnityEngine;

public class SwimMovement : MonoBehaviour
{
    public Movement movementScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            movementScript.SetSwimming(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            movementScript.SetSwimming(false);
        }
    }
}