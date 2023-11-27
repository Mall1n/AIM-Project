using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemID
{
    public enum Engine : int
    {
        Unknown = 0,
        FirstEngine = 1
    }

    public static List<string> PathToEngines = new List<string>(){
                                                                    "",
                                                                    "Ship Assets/Engines/First Engine"
                                                                };


    public enum Gun : int
    {
        Unknown = 0,
        FirstGun = 1
    }
    
    public static List<string> PathToGuns = new List<string>(){
                                                                    "",
                                                                    "Ship Assets/Guns/First Gun"
                                                                };


    private enum ItemType
    {
        Unknown = 0,
        Garbage = 1
    }
}



