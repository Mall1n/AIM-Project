using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ShipConfiguration : MonoBehaviour
{
    [SerializeField] private PlayerShipMovement playerShipMovement;
    [SerializeField][Range(0, 1)] private float mobility;
    public List<Engine> engines; // make privatet
    public List<Gun> easyGuns; // make private
    public List<Gun> heavyGuns; // make private
    [SerializeField] private Item[] inventory;

    public void SetInventoryCapacity(int capacity)
    {
        int capacityNow = inventory.Count(s => s != null);
        if (capacity >= capacityNow)
            Array.Resize(ref inventory, capacity);
        else 
            throw new IndexOutOfRangeException($"Inventory capacity = {capacityNow} tried resize to {capacity}");
    }

#if UNITY_EDITOR

    public bool showDefaultInspector = true;
    public bool showEditor = true;

    // FOR ENGINE
    public List<Transform> transformsEngines;
    public List<int> IDEngines;

    // FOR EASY GUNS
    public List<Transform> transformEasyGuns;
    public List<int> IDEasyGuns;

    // FOR HEAVY GUNS
    public List<Transform> transformHeavyGuns;
    public List<int> IDHeavyGuns;

#endif


    private void Start()
    {
        UpdateEmptyLists();
        FullUpdateShipValues();
    }

    public void FullUpdateShipValues()
    {
        float powerEngines = GetPowerEngines();

        float maxSpeed = powerEngines / 20;
        float acceleration = powerEngines / 25;
        float _mobility = mobility;

        SendValuesToMovement(maxSpeed, acceleration, _mobility);
    }

    private void SendValuesToMovement(float maxSpeed, float acceleration, float mobility)
    {
        if (playerShipMovement != null)
            playerShipMovement.UpdateValues(maxSpeed, acceleration, mobility);
    }

    private void DeleteEmpty<T>(List<T> list) => list.RemoveAll(s => s == null);

    private void UpdateEmptyLists()
    {
        DeleteEmpty(engines);
        DeleteEmpty(easyGuns);
        DeleteEmpty(heavyGuns);
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
