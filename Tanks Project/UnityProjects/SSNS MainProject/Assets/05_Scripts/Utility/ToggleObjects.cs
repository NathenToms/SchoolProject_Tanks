using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObjects : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToHide;

    public void HideEverything(bool hide)
    {
        if (objectsToHide == null) return;

        foreach (GameObject o in objectsToHide)
        {
            o.SetActive(!hide);
        }
    }
}
