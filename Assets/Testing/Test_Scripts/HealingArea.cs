using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingArea : MonoBehaviour
{
    [Tooltip("Type of healing to apply in this area.")]
    public DamageType healingType;

    [Tooltip("Amount of healing to apply.")]
    public float healingAmount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Check the type of healing and apply it accordingly
            switch (healingType)
            {
                case DamageType.Stamina:
                    PlayerVitals.instance.RestoreStamina(healingAmount);
                    Debug.Log("Player entered stamina restore area.");
                    break;

                case DamageType.Magic:
                    PlayerVitals.instance.ReplenishMagic(healingAmount);
                    Debug.Log("Player entered magic healing area.");
                    break;

                case DamageType.Health:
                    PlayerVitals.instance.RestoreHealth(healingAmount);
                    Debug.Log("Player entered health healing area.");
                    break;

                case DamageType.All:
                    PlayerVitals.instance.RestoreAllVitals();
                    Debug.Log("Player entered restore all area.");
                    break;
            }
        }
    }
}

