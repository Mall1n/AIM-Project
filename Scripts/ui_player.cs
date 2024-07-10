using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_Player : MonoBehaviour
{

    private ProgressBar progressBar_Shield;
    private const string progressBar_Shield_Name = "Shield";

    private VisualElement inventoryBackground;
    private const string visualElement_InventoryBackground_Name = "BlackgroundShading";

    [SerializeField] ShipConfiguration shipConfiguration;
    [SerializeField] PlayerShipMovement playerShipMovement;

    // LOG..............................................................
    private Label labelLog;
    private const string label_Log_Name = "Log";
    private void LogLabelStats()
    {
        labelLog.text = $"Ship Configuration:\nMass = {shipConfiguration.FullMass}\nMobility = {shipConfiguration.Mobility}\nEngine Acceleration = {shipConfiguration.EnginesAccelerationPower}\nMax Speed = {shipConfiguration.MaxSpeed}\n\n" +
        $"Player Ship Movement:\nSpeed = {playerShipMovement.Rb.velocity.magnitude}\nAcceleration = {playerShipMovement.Acceleration}";
    }
    // LOG..............................................................

    private void OnEnable()
    {
        if (shipConfiguration == null)
            throw new System.NullReferenceException("shipConfiguration was null in UI player");
        if (playerShipMovement == null)
            throw new System.NullReferenceException("playerShipMovement was null in UI player");

        ShipConfiguration.onChangedShiledHP += UpdateProgressBarValue_Shield;
        var uiDocument = GetComponent<UIDocument>();

        progressBar_Shield = uiDocument.rootVisualElement.Q<ProgressBar>(progressBar_Shield_Name);

        inventoryBackground = uiDocument.rootVisualElement.Q<VisualElement>(visualElement_InventoryBackground_Name);

        labelLog = uiDocument.rootVisualElement.Q<Label>(label_Log_Name);
        //LogLabelStats();
    }
    private void Start()
    {
        SetProgressBarShield();
    }

    private void Update()
    {
        LogLabelStats();
    }

    private void SetProgressBarShield()
    {
        progressBar_Shield.highValue = shipConfiguration.GetHighValueShield();
        progressBar_Shield.value = shipConfiguration.GetHighValueShield(); // change
    }

    private void OnDisable()
    {
        ShipConfiguration.onChangedShiledHP -= UpdateProgressBarValue_Shield;
    }

    private void UpdateProgressBarValue_Shield(float value)
    {
        progressBar_Shield.value = value;
    }

    public void ChangeStateInventory(bool state)
    {
        inventoryBackground.visible = state;
    }

}
