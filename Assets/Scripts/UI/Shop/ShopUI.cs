using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ShopUI : MonoBehaviour
{
    [SerializeField] string shopName;

    [Header("References")]
    [SerializeField] RectTransform layout;
    [SerializeField] TMP_Text goldText;
    [SerializeField] TMP_Text gemText;
    [SerializeField] TMP_Text shopPresenceText;
    [SerializeField] TMP_Text shopNameText;

    [Header("Debug")]
    [SerializeField] bool isPresent;

    private RectTransform[] layoutGroups;

    private void Awake()
    {
        var children = layout.GetComponentsInChildren<LayoutGroup>();
        layoutGroups = children.Select(layout => layout.GetComponent<RectTransform>()).ToArray();

        GoldManager.GoldChanged.AddListener((gold) =>
        {
            foreach (var group in layoutGroups)
            {
                group.gameObject.SetActive(false);
                group.gameObject.SetActive(true);
            }

            goldText.text = gold.ToString();
        });

        GoldManager.GemChanged.AddListener((gem) =>
        {
            foreach (var group in layoutGroups)
            {
                group.gameObject.SetActive(false);
                group.gameObject.SetActive(true);
            }

            gemText.text = gem.ToString();
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
