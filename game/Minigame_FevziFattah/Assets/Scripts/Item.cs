using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    [TextArea(1, 10)]
    public string itemName;
    [TextArea(1, 10)]
    public string description;
    public Sprite icon;
}
