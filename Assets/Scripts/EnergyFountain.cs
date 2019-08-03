using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnergyContainer))]
public class EnergyFountain : Interactable
{
    private EnergyContainer m_EnergyContainer;

    public string hintRefillOrb = "Refill orb";

    // Start is called before the first frame update
    void Start()
    {
        m_EnergyContainer = GetComponent<EnergyContainer>();
    }

    public override bool CanBeUsed(Character player)
    {
        Orb orbRef = player.GetOrb();

        return (orbRef != null && orbRef.isInHand && m_EnergyContainer.energyType != EnergyType.None && m_EnergyContainer.energy > 0);
    }

    public override string GetHintText(Character player)
    {
        return "";
    }

    public override void Interact(Character player)
    {
        Orb orbRef = player.GetOrb();

        orbRef.energyContainer.SetEnergy(m_EnergyContainer.energy, m_EnergyContainer.energyType);
    }
}
