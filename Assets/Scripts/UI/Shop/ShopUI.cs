using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopUI : MonoBehaviour
{
    [SerializeField] string shopName;

    [Header("References")]
    [SerializeField] TMP_Text goldText;
    [SerializeField] TMP_Text shopPresenceText;
    [SerializeField] TMP_Text shopNameText;

    // public bool IsPresent
    // {
    //     get
    //     {
    //         return IsPresent;
    //     }
    //     set
    //     {
    //         IsPresent = value;
    //         shopPresenceText.enabled = IsPresent;
    //     }
    // }

    [Header("Debug")]
    [SerializeField] bool isPresent;

    private void Awake()
    {
        GoldManager.GoldChanged.AddListener((gold) =>
        {
            goldText.text = gold.ToString();
        });
    }

    private void Start()
    {
        shopNameText.text = shopName;
    }


    private void Update()
    {
        shopPresenceText.enabled = isPresent;
    }

}
