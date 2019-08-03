using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual bool CanBeUsed(Character player)
    {
        return false;
    }

    public virtual string GetHintText(Character player)
    {
        return "empty";
    }

    public virtual void Interact(Character player)
    {

    }
}
