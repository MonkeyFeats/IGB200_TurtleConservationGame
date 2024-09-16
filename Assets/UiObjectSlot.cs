using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiObjectSlot : MonoBehaviour
{
    public Image objectImage;
    public TextMeshProUGUI objectCounter;

    public void SetSlot(Sprite objectSprite, int count)
    {
        objectImage.sprite = objectSprite;
        objectCounter.text = count.ToString();
    }

    public void UpdateCounter(int count)
    {
        objectCounter.text = count.ToString();
    }
}