using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnergyContainer))]
public class Switch : Interactable
{
    private EnergyContainer m_EnergyContainer;

    public string hintActivate = "Activate switch";

    public bool isPowered = false;

    // Start is called before the first frame update
    void Start()
    {
        m_EnergyContainer = GetComponent<EnergyContainer>();
        if(m_EnergyContainer.energyType == EnergyType.Key && m_EnergyContainer.energy > 0)
        {
            SetPowered(true);
        }
        else
        {
            SetPowered(false);
        }
    }

    public override bool CanBeUsed(Character player)
    {
        Orb orbRef = player.GetOrb();

        return (orbRef != null && orbRef.isInHand && !isPowered && orbRef.energyContainer.energyType == EnergyType.Key && orbRef.energyContainer.energy > 0);
    }

    public override string GetHintText(Character player)
    {
        return "";
    }

    public override void Interact(Character player)
    {
        Orb orbRef = player.GetOrb();

        m_EnergyContainer.SetEnergy(orbRef.energyContainer.energy, orbRef.energyContainer.energyType);
        orbRef.energyContainer.SetEnergy(0, EnergyType.None);
        SetPowered(true);
    }

    public void SetPowered(bool status)
    {
        isPowered = status;
    }
}
