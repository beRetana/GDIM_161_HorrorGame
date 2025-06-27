using MessengerSystem;
using UnityEngine;
using UnityEngine.UI;

public class MouseUI : MonoBehaviour
{
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color interactedColor;
    [SerializeField] private Image mouseImage;

    void Start()
    {
        mouseImage = GetComponent<Image>();
    }

    public void InteractionEffect()
    {
        mouseImage.color = interactedColor;
    }

    public void DefaultEffect()
    {
        mouseImage.color = defaultColor;
    }

}
