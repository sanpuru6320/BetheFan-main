using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractShowUI : MonoBehaviour, Interactable
{
    [SerializeField] string uiTag;
    GameObject uiContainer = null;

    private void Awake()
    {
        uiContainer = GameObject.FindGameObjectWithTag(uiTag);
    }

    public IEnumerator Interact(Transform initiator)
    {
        uiContainer.SetActive(true);
        yield break;
    }
}
