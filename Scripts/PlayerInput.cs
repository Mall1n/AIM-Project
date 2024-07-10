using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private bool isInventoryOpen = false;
    [SerializeField]UI_Player ui_Player;

    private void Start() 
    {
        if (ui_Player == null)
            throw new System.NullReferenceException($"ui_Player is NULL in script {nameof(PlayerInput)}");
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            isInventoryOpen = !isInventoryOpen;
            ui_Player.ChangeStateInventory(isInventoryOpen);
        }
    }
}
