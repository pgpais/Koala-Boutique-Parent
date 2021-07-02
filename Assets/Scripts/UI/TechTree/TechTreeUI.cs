using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechTreeUI : MonoBehaviour
{
    [SerializeField] TechUI techUIPrefab;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseCommunicator.LoggedOut.AddListener(OnLogout);

        foreach (var keyValuePair in UnlockablesManager.instance.Unlockables)
        {
            Instantiate(techUIPrefab, transform).InitUI(keyValuePair.Value);
        }

        // UnlockablesManager.instance.Unlockables["Dummy"].Unlocked = true;
        // UnlockablesManager.instance.Unlockables["Dummy"].UnlockableUpdated.Invoke(UnlockablesManager.instance.Unlockables["Dummy"]);
    }

    void OnLogout()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
