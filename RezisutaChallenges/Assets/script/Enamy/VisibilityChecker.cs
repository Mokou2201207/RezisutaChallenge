using UnityEngine;

public class VisibilityChecker : MonoBehaviour
{
    private MannequinEnamy parentScript;

    void Start()
    {
        parentScript = GetComponentInParent<MannequinEnamy>();
    }

    void OnBecameVisible()
    {
        if (parentScript != null) parentScript.isInScreen = true;
    }

    void OnBecameInvisible()
    {
        if (parentScript != null) parentScript.isInScreen = false;
    }
}