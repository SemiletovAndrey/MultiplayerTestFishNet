using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemObject", menuName = "Inventory/Item", order = 1)]
public class ItemObject : ScriptableObject
{
    public string itemName;
    public GameObject prefab;
}
