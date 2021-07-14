using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/CheeseToggle", 32)]
public class CheeseToggle : Toggle
{
    [SerializeField] Sprite onSprite;
    [SerializeField] Sprite offSprite;

    [SerializeField] Image image;

    protected override void Awake()
    {
        base.Awake();
        onValueChanged.AddListener(ToggleSprites);
        if (image == null)
        {
            image = GetComponent<Image>();
        }
    }

    void ToggleSprites(bool state)
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        if (state)
        {
            image.sprite = onSprite;
        }
        else
        {
            image.sprite = offSprite;
        }
    }
}
