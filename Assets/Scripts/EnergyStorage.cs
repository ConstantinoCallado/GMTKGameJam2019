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

    // Start is called before the first frame update
    void Start()
    {
        m_EnergyContainer = GetComponent<EnergyContainer>();
    }

    public override bool CanBeUsed(Character player)
    {
        Orb orbRef = player.GetOrb();

        return (orbRef != null && orbRef.isInHand);
    }

    public override string GetHintText(Character player)
    {
        return "";
    }

    public override void Interact(Character player)
    {
        Orb orbRef = player.GetOrb();

        EnergyType storedEnergyType = m_EnergyContainer.energyType;
        float storedEnergyValue = m_EnergyContainer.energy;

        m_EnergyContainer.SetEnergy(orbRef.energyContainer.energy, orbRef.energyContainer.energyType);

        orbRef.energyContainer.SetEnergy(storedEnergyValue, storedEnergyType);
    }
}
