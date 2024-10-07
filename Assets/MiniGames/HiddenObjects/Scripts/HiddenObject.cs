using UnityEngine;

public class HiddenObject : MonoBehaviour
{
    private HiddenObjectGameManager gameController;
    private string objectName;

    void Start()
    {
        // Find the game controller in the scene
        gameController = FindObjectOfType<HiddenObjectGameManager>();
        objectName = gameObject.name.Replace("(Clone)", "").Trim(); // Clean name for prefab
    }

    void OnMouseDown()
    {
        // Call the game controller to handle object removal
        gameController.FindObject(gameObject, objectName);
    }
}
