using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Inventory/Equipment")]
public class EquipmentItem : Item
{
    public EquipmentSlot slot;
    public GameObject prefab;
    public int armorClass;
    public int damage;
    public float hitChance;
}
