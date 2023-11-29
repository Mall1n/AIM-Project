using UnityEngine;

public abstract class ItemShip : Item
{
    [SerializeField] private Type itemType = 0;
    public Type ItemType { get => itemType; }
    public enum Type : int
    {
        Unknown = 0,
        Engine = 1,
        EasyGun = 2,
        HeavyGun = 3
    }
}
