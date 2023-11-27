using UnityEngine;

public class TypeOfPartShip : MonoBehaviour
{
    public TypeOfPart typeOfPart = TypeOfPart.Unknown;
    public enum TypeOfPart : int
    {
        Unknown = 0,
        Engine = 1,
        EasyGun = 2,
        HeavyGun = 3
    }
}
