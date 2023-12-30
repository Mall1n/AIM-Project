using UnityEngine;

public abstract class ItemShip : Item
{
    //[Header("Item Type")]
    [SerializeField] private Type itemType = 0;
    public Type ItemType { get => itemType; }
    public enum Type : int
    {
        Unknown = 0,
        Engine = 1,
        EasyGun = 2,
        HeavyGun = 3
    }

    [SerializeField] private protected bool leftSide = false;
    public bool LeftSide { get => leftSide; set => leftSide = value; }

}
