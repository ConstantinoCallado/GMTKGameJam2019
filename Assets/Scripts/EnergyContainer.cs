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

    public ParticleSystem particles;
    public ParticleSystem explosionParticles;

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

    public void SetEnergy(float amount, EnergyType type)
    {
        energy = 0;
        AddEnergy(amount, type);
    }

    public void AddEnergy(float amount, EnergyType type)
    {
        energyType = type;

        if(type != EnergyType.None)
        {
            energy = Mathf.Clamp(energy + amount, 0, maxEnergyPool);
        }
        else
        {
            energy = 0;
        }

        RecalculateAspect();
    }

    public void DrainEnergy(float amount)
    {
        if(energy > 0 && energyType != EnergyType.None)
        {
            if(explosionParticles)
            {
                explosionParticles.Emit(75);
            }
        }

        energy = Mathf.Clamp(energy - amount, 0, maxEnergyPool);

        if(energy == 0)
        {
            energyType = EnergyType.None;
        }

        RecalculateAspect();
    }

    public void RecalculateAspect()
    {
        switch (energyType)
        {
            case EnergyType.None:
                orbMesh.sharedMaterial = Resources.Load("Material/Orbs/None", typeof(Material)) as Material;
                orbLight.color = new Color(0.4f, 0.4f, 0.4f);
                orbLight.intensity = 0.3f;
                orbLight.range = 5;
                ActivateHalo("None");
                if (particles) 
                {
                    var em = particles.emission;
                    em.enabled = false;
                }
                break;

            case EnergyType.Light:
                orbMesh.sharedMaterial = Resources.Load("Material/Orbs/Light", typeof(Material)) as Material;
                orbLight.color = new Color(1, 1, 1);
                orbLight.intensity = 1.5f;
                orbLight.range = 9;
                ActivateHalo("Light");
                if (particles)
                {
                    var em = particles.emission;
                    em.enabled = true;
                    particles.startColor = orbLight.color;
                }
                if(explosionParticles)
                {
                    explosionParticles.startColor = orbLight.color;
                }
                break;

            case EnergyType.Damage:
                orbMesh.sharedMaterial = Resources.Load("Material/Orbs/Damage", typeof(Material)) as Material;
                orbLight.color = new Color(0.75f, 0, 0.09f);
                orbLight.intensity = 1;
                orbLight.range = 6;
                ActivateHalo("Damage");
                if (particles)
                {
                    var em = particles.emission;
                    em.enabled = true;
                    particles.startColor = orbLight.color;
                }
                if (explosionParticles)
                {
                    explosionParticles.startColor = orbLight.color;
                }
                break;

            case EnergyType.Key:
                orbMesh.sharedMaterial = Resources.Load("Material/Orbs/Key", typeof(Material)) as Material;
                orbLight.color = new Color(1, 0.92f, 0.34f);
                orbLight.intensity = 1;
                orbLight.range = 6;
                ActivateHalo("Key");
                if (particles)
                {
                    var em = particles.emission;
                    em.enabled = true;
                    particles.startColor = orbLight.color;
                }
                if (explosionParticles)
                {
                    explosionParticles.startColor = orbLight.color;
                }
                break;
        }
    }

    public void ActivateHalo(string energyName)
    {
        for(int i=0; i<halos.Length; i++)
        {
            halos[i].gameobject.SetActive(halos[i].name == energyName);
        }
    }
}
