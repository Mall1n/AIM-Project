using UnityEngine;
using System;
using System.Collections.Generic;

public class ShipConfiguration : MonoBehaviour
{
    private bool editorFolduot = true;
    public bool EditorFolduot { get => editorFolduot; set => editorFolduot = value; }
    //[SerializeField] 
    public List<Engine> engines; // make privatet
    public List<Gun> guns; // make privatet
    // public void EngineAdd(Engine engine) => engines.Add(engine);
    // public void EngineAdd() => engines.Add(new Engine());
    // public void EngineRemove(int index) => engines.RemoveAt(index);

#if UNITY_EDITOR

    // FOR ENGINE
    public List<Transform> transformsForEngines;
    public List<int> IDEngines;

    // FOR GUNS
    public List<Transform> transformsForGuns;
    public List<int> IDGuns;

#endif

    // public int GetEnginesCount => engines.Length;

    // public Engine GetEngine(int i) => engines[i];
    // public void SetEngine(Engine engine, int i)
    // {
    //     if (i >= engines.Length)
    //         throw new IndexOutOfRangeException($"Length = {engines.Length} => Index = {i}");
    //     engines[i] = engine;
    // }

    // public Engine[] GetEngines => engines;



    void Start()
    {

    }

    public float GetPowerEngines()
    {
        float result = 0;
        foreach (Engine item in engines)
        {
            if (item.Power < 0)
                throw new ArgumentOutOfRangeException($"Engine powel lower 0 ({item.Power})");

            result += item.Power;
        }
        return result;
    }

}
