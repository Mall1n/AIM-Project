using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public string Name;
    //public string Name { get => name; }
    public float power; // private
    private float energyUsage;
    private float mass;
    public EngineType engineType = 0;



    public Transform transformParent; // private

    public enum EngineType
    {
        Unknown = 0,
        FirstEngine = 1
    }

    public static List<string> PathToEngines = new List<string>(){
                                                                    "",
                                                                    "Ship Assets/Engines/First Engine"
                                                                };

    private void Start()
    {

    }

    public override string ToString()
    {
        return $"Object Name = {transform.gameObject.name} | power = {power} | energyUsage = {energyUsage} | mass = {mass} | transformParent = {transformParent}";
    }
}
