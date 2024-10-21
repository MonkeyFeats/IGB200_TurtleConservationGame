using UnityEngine;

public class HiddenObject : MonoBehaviour
{
    private HiddenObjectGameManager gameController;
    private string objectName;
    public GameObject deathObject;

    void Start()
    {
        // Find the game controller in the scene
        gameController = FindObjectOfType<HiddenObjectGameManager>();
        objectName = gameObject.name.Replace("(Clone)", "").Trim(); // Clean name for prefab
    }

    void OnMouseDown()
    {
        // Call the game controller to handle object removal
        Instantiate(deathObject, transform.position, Quaternion.identity);
        gameController.FindObject(gameObject, objectName);
        Destroy(gameObject);
    }

}
