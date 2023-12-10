using System.Collections.Generic;

public class ItemID
{
    public enum Engine : int
    {
        Unknown = 0,
        FirstEngine = 1,
        FirstEngineTest = 2
    }

    public static List<string> PathToEngines = new List<string>(){
                                                                    "",
                                                                    "Ship Assets/Engines/First Engine",
                                                                    "Ship Assets/Engines/First Engine Test"
                                                                };

    public enum EasyGun : int
    {
        Unknown = 0,
        Single_Barrel = 1,
    }

    public static List<string> PathToEasyGuns = new List<string>(){
                                                                    "",
                                                                    "Ship Assets/Guns/Single-Barrel Gun",
                                                                };

    public enum HeavyGun : int
    {
        Unknown = 0,
        FirstHeavyGun = 1
    }

    public static List<string> PathToHeavyGuns = new List<string>(){
                                                                    "",
                                                                    "Ship Assets/Guns/First Heavy Gun",
                                                                };


}



