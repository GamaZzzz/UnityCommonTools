using UnityEngine.UI;
using UnityEngine;
public class UIImage : Widget
{
    [SerializeField]
    private Image original;

    public void SetSprite(Sprite sprite)
    {
        original.sprite = sprite;
    }

    public void SetColor(Color color)
    {
        original.color = color;
    }

    public void SetMaterial(Material mt)
    {
        original.material = mt;
    }
}
