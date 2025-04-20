using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    [Tooltip("Type of damage to apply in this area.")]
    public DamageType damageType;

    [Tooltip("Amount of damage to apply.")]
    public float damageAmount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Check the type of damage and apply it accordingly
            switch (damageType)
            {
                case DamageType.Stamina:
                    PlayerVitals.instance.DrainStamina(damageAmount);
                    Debug.Log("Player entered stamina drain area.");
                    break;

                case DamageType.Magic:
                    PlayerVitals.instance.UseMagic(damageAmount);
                    Debug.Log("Player entered magic damage area.");
                    break;

                case DamageType.Health:
                    PlayerVitals.instance.DamageHealth(damageAmount);
                    Debug.Log("Player entered health damage area.");
                    break;

                case DamageType.All:
                    PlayerVitals.instance.DamageHealth(damageAmount);
                    PlayerVitals.instance.UseMagic(damageAmount);
                    PlayerVitals.instance.DrainStamina(damageAmount);
                    Debug.Log("Player entered damage all area.");
                    break;
            }
        }
    }
}

public enum DamageType
{
    Health,
    Stamina,
    Magic,
    All
}
