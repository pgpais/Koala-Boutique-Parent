using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] GameObject fadePanel;

    private void OnEnable()
    {
        fadePanel.SetActive(true);
    }

    private void OnDisable()
    {
        fadePanel.SetActive(false);
    }
}
