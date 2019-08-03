using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnergyType { None, Light, Damage, Key };

[System.Serializable]
public class HaloRefs
{
    public string name;
    public GameObject gameobject;
}

[ExecuteInEditMode]
public class EnergyContainer : MonoBehaviour
{
    [Header("Energy settings")]
    public EnergyType energyType;
    public float maxEnergyPool = 100f;
    public float energy = 100f;

    public Renderer orbMesh;
    public Light orbLight;

    
    public HaloRefs[] halos;

    public void Start()
    {
        RecalculateAspect();
    }

    #if (UNITY_EDITOR)
    public void Update()
    {
        RecalculateAspect();
    }
    #endif

    public void RecalculateAspect()
    {
        switch (energyType)
        {
            case EnergyType.None:
                orbMesh.material.SetColor("_EmissionColor", new Color(0.1f, 0.1f, 0.1f));
                orbMesh.material.SetColor("_Color", Color.black);
                orbLight.color = new Color(0.4f, 0.4f, 0.4f);
                orbLight.intensity = 0.3f;
                orbLight.range = 5;
                ActivateHalo("None");
                break;

            case EnergyType.Light:
                orbMesh.material.SetColor("_EmissionColor", new Color(1, 1, 1) * 2);
                orbMesh.material.SetColor("_Color", new Color(1, 1, 1));
                orbLight.intensity = 1.5f;
                orbLight.color = new Color(1, 1, 1);
                orbLight.range = 9;
                ActivateHalo("Light");
                break;

            case EnergyType.Damage:
                orbMesh.material.SetColor("_EmissionColor", new Color(0.75f, 0, 0.09f) * 2);
                orbMesh.material.SetColor("_Color", new Color(0.75f, 0, 0.09f));
                orbLight.color = new Color(0.75f, 0, 0.09f);
                orbLight.intensity = 1;
                orbLight.range = 6;
                ActivateHalo("Damage");
                break;

            case EnergyType.Key:
                orbMesh.material.SetColor("_EmissionColor", new Color(1f, 0.88f, 0.17f) * 1.41f);
                orbMesh.material.SetColor("_Color", new Color(0.75f, 0.66f, 0.13f));
                orbLight.color = new Color(1, 0.92f, 0.34f);
                orbLight.intensity = 1;
                orbLight.range = 6;
                ActivateHalo("Key");
                break;
        }

        
        //orbMesh.material.SetColor ("_EmissionColor", Color.black);
    }

    public void ActivateHalo(string energyName)
    {
        for(int i=0; i<halos.Length; i++)
        {
            halos[i].gameobject.SetActive(halos[i].name == energyName);
        }
    }
}
