using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ui_player : MonoBehaviour
{

    private ProgressBar progressBar_Shield;
    private const string progressBar_Shield_Name = "Shield";

    private void OnEnable()
    {
        ShipConfiguration.onChangedHealth += UpdateProgressBarHealthValue;
        var uiDocument = GetComponent<UIDocument>();

        progressBar_Shield = uiDocument.rootVisualElement.Q<ProgressBar>(progressBar_Shield_Name);

        Debug.Log(progressBar_Shield);

    }

    private void OnDisable()
    {
        ShipConfiguration.onChangedHealth -= UpdateProgressBarHealthValue;
    }

    private void UpdateProgressBarHealthValue(float health)
    {
        progressBar_Shield.value = health;
    }

}
