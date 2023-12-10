

public class ItemInvertory : Item
{
    private protected enum ItemType : int
    {
        Unknown = 0,
        Garbage = 1,
        Bullet = 2
    }

    private ItemType itemType;

}
