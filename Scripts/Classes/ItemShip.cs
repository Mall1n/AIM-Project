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

    // [SerializeField] private protected bool leftSide = false;
    // public bool LeftSide { get => leftSide; set => leftSide = value; }

    [SerializeField] private Side itemSide = 0;
    public Side ItemSide { get => itemSide; set { itemSide = value; } }
    public enum Side : int
    {
        None = 0,
        Left = 1,
        Right = 2,
    }

}
