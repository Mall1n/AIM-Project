using System;
using UnityEngine;

public class PlayerFire : Fire
{
    public static Action onPlayerFired;
    public static Action onPlayerStopFired;
    //public bool isLeftClicking => Input.GetMouseButton(0);


    void Update()
    {
        GetMouseInput();
    }

    // private void FixedUpdate()
    // {
    //     //GunFire();
    // }


    private void GetMouseInput()
    {
        GetMouseLeftButtonDown();

        GetMouseLeftButtonUp();
    }

    private void GetMouseLeftButtonUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            onPlayerStopFired?.Invoke();
        }
    }

    private void GetMouseLeftButtonDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            onPlayerFired?.Invoke();
        }
    }
}
