using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeOfPartShip : MonoBehaviour
{
    public TypeOfPart typeOfPart = TypeOfPart.Unknown;
    public enum TypeOfPart
    {
        Unknown = 0,
        Engine = 1,
        EasyGun = 2,
        HeavyGun = 3
    }
}
