using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidobjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask portalLayer;
    [SerializeField] LayerMask triggerLayer;
    [SerializeField] LayerMask ledgeLayer;
    [SerializeField] LayerMask waterLayer;

   
    public static GameLayers i { get; set; }

    private void Awake()
    {
        i = this;
    }
    public LayerMask SolidLayer { get => solidobjectsLayer; }
    public LayerMask InteractableLayer { get => interactableLayer; }
    public LayerMask GrassLayer
    {
        get => grassLayer;
    }
    public LayerMask PlayerLayer
    {
        get => playerLayer;
    }
    public LayerMask FovLayer
    {
        get => fovLayer;
    }

    public LayerMask PortalLayer
    {
        get => portalLayer;
    }

    public LayerMask LedgeLayer => ledgeLayer;
    public LayerMask WaterLayer => waterLayer;
    public LayerMask TriggerableLayers 
    {
        get => grassLayer | fovLayer | portalLayer | triggerLayer | waterLayer;
    }
}
