using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnergyType { None, Light, Damage, Key };



public class EnergyContainer : MonoBehaviour
{
    [Header("Energy settings")]
    public EnergyType energyType;
    public float maxEnergyPool = 100f;
    public float energy = 100f;

    public Renderer orbMesh;
    

    public void Start()
    {
        RecalculateAspect();
    }

    public void RecalculateAspect()
    {
        //orbMesh.material.SetColor ("_EmissionColor", Color.black);
    }
}
