using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnergyContainer))]
public class EnergyStorage : Interactable
{
    private EnergyContainer m_EnergyContainer;

    public string hintRefillOrb = "Refill orb";
    public string hintStoreEnergy = "Store energy";
    public string hintSwapEnergy = "Swap energy";
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_EnergyContainer = GetComponent<EnergyContainer>();
    }

    public override bool CanBeUsed(Character player)
    {
        Orb orbRef = player.GetOrb();

        return (orbRef != null && orbRef.isInHand && !(orbRef.energyContainer.energy == 0 && m_EnergyContainer.energy == 0));
    }

    public override string GetHintText(Character player)
    {
        return "";
    }

    public override void Interact(Character player)
    {
        audioSource.Play();
        Orb orbRef = player.GetOrb();

        EnergyType storedEnergyType = m_EnergyContainer.energyType;
        float storedEnergyValue = m_EnergyContainer.energy;
        EnergyFountain pickedFromFountain = m_EnergyContainer.pickedFromFountain;

        m_EnergyContainer.SetEnergy(orbRef.energyContainer.energy, orbRef.energyContainer.energyType);
        m_EnergyContainer.pickedFromFountain = orbRef.energyContainer.pickedFromFountain;

        orbRef.energyContainer.SetEnergy(storedEnergyValue, storedEnergyType);
        orbRef.energyContainer.pickedFromFountain = pickedFromFountain;
    }
}
