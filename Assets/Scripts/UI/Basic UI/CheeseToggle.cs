using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("UI/CheeseToggle", 32)]
public class CheeseToggle : Toggle
{
    [SerializeField] Sprite onSprite;
    [SerializeField] Sprite offSprite;

    [SerializeField] Image image;

    AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        // onValueChanged.AddListener(ToggleSprites);
        if (image == null)
        {
            image = GetComponent<Image>();
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        ToggleSprites(isOn);
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

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        Debug.Log("on click");
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public void PlayAudionInChildren()
    {
        var audioSource = GetComponentsInChildren<AudioSource>();
        for (int i = 1; i < audioSource.Length; i++)
        {
            var audio = audioSource[i];
            audio.Play();
        }


    }
}
