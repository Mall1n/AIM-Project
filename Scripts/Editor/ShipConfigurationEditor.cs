using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


[CustomEditor(typeof(ShipConfiguration))]
public class ShipConfigurationEditor : Editor
{
    private bool showDefaultInspector = true;
    private ShipConfiguration shipConfiguration;

    private List<Engine> engines;
    //private float powerTest = 0;

    private List<Transform> enginesTransforms;
    private List<Engine.EngineType> engineTypes;

    //private Engine.EngineType engineType;




    private void OnEnable()
    {
        shipConfiguration = target as ShipConfiguration;
        if (shipConfiguration == null)
            return;

        UpdateValues();

        Undo.undoRedoPerformed += UpdateValues;
    }

    public override void OnInspectorGUI()
    {
        //UpdateValues();

        DrawEditorInspector();

        ShowDefaultInspector();
    }

    private void ShowDefaultInspector()
    {
        EditorGUI.BeginChangeCheck();

        showDefaultInspector = EditorGUILayout.Foldout(shipConfiguration.EditorFolduot, "Show Default Inspector");
        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log(showDefaultInspector);
            Undo.RecordObject(shipConfiguration, $"Foldout ShowDefaultInspector");
            shipConfiguration.EditorFolduot = showDefaultInspector;
        }
        if (showDefaultInspector)
            DrawDefaultInspector();
    }

    private void DrawEditorInspector()
    {
        ShowEngineParameters();
    }

    private void UpdateValues()
    {
        engines = new List<Engine>(shipConfiguration.engines);
        enginesTransforms = new List<Transform>(shipConfiguration.transformsForEngines);
        engineTypes = new List<Engine.EngineType>(shipConfiguration.engineTypes);
        //Debug.Log("UpdateValues");
    }

    private void ShowEngineParameters()
    {
        GUILayout.BeginHorizontal(EditorStyles.helpBox);

        EditorGUI.BeginChangeCheck();
        GUILayout.Button("Add Engine");
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(shipConfiguration, $"Add Engine");
            shipConfiguration.transformsForEngines.Add(null);
            shipConfiguration.engines.Add(null);
            shipConfiguration.engineTypes.Add(0);
            UpdateValues();
        }

        EditorGUI.BeginChangeCheck();
        GUILayout.Button("Remove Engine");
        if (EditorGUI.EndChangeCheck())
        {
            int index = shipConfiguration.transformsForEngines.Count;
            if (index != 0)
            {
                index -= 1;
                Undo.RegisterFullObjectHierarchyUndo(shipConfiguration.gameObject, $"Remove Engine");
                if (shipConfiguration.transformsForEngines[index] != null && shipConfiguration.engineTypes[index] != 0)
                    DestroyObject(shipConfiguration.transformsForEngines[index].GetComponentInChildren<Engine>()?.gameObject);
                shipConfiguration.engines.RemoveAt(index);
                shipConfiguration.transformsForEngines.RemoveAt(index);
                shipConfiguration.engineTypes.RemoveAt(index);
                UpdateValues();
            }
        }

        GUILayout.EndHorizontal();

        //GUILayout.Label("Some");

        //Debug.Log($"enginesTransforms.Count = {enginesTransforms.Count} | engines.Count = {engines.Count}");
        for (int i = 0; i < enginesTransforms.Count; i++)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();

            enginesTransforms[i] = (Transform)EditorGUILayout.ObjectField(shipConfiguration.transformsForEngines[i], typeof(Transform), true);
            Transform engineTransform = enginesTransforms[i];
            if (EditorGUI.EndChangeCheck())
            {
                if (engineTransform != null)
                {
                    TypeOfPartShip typeOfPartShip = engineTransform.GetComponent<TypeOfPartShip>();
                    if (typeOfPartShip != null && typeOfPartShip.typeOfPart == TypeOfPartShip.TypeOfPart.Engine
                        && !shipConfiguration.transformsForEngines.Contains(engineTransform))
                    {
                        Undo.RegisterCompleteObjectUndo(shipConfiguration, $"Transform set");
                        shipConfiguration.transformsForEngines[i] = engineTransform;
                    }
                }
                else if (shipConfiguration.transformsForEngines[i] != null)
                {
                    Undo.RegisterFullObjectHierarchyUndo(shipConfiguration.gameObject, $"Transform delete");
                    DestroyObject(shipConfiguration.transformsForEngines[i].GetComponentInChildren<Engine>()?.gameObject);
                    shipConfiguration.engineTypes[i] = Engine.EngineType.Unknown;
                    shipConfiguration.transformsForEngines[i] = null;
                }
                UpdateValues();
            }

            if (engineTransform != null)
            {
                EditorGUI.BeginChangeCheck();
                engineTypes[i] = (Engine.EngineType)EditorGUILayout.EnumPopup(shipConfiguration.engineTypes[i]);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterFullObjectHierarchyUndo(shipConfiguration.gameObject, $"Change Engine Type");
                    shipConfiguration.engineTypes[i] = engineTypes[i];

                    if (shipConfiguration.engineTypes[i] != 0)
                    {
                        GameObject LoadedEngine = Resources.Load(Engine.PathToEngines[(int)Engine.EngineType.FirstEngine], typeof(GameObject)) as GameObject;
                        Instantiate(LoadedEngine, enginesTransforms[i]);
                        Debug.Log("Inst");
                    }
                    else if (shipConfiguration.engineTypes[i] == 0)
                    {
                        DestroyObject(enginesTransforms[i].GetComponentInChildren<Engine>()?.gameObject);
                    }
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }

    private void DestroyObject(GameObject gameObject)
    {
        Debug.Log("Destroy");
        if (gameObject != null)
            DestroyImmediate(gameObject);

    }

    private void GUILayoutSpace()
    {
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
    }

    private T ShowObject<T>(T _object, string text) where T : Behaviour
    {
        Undo.RecordObject(shipConfiguration, text);
        return (T)EditorGUILayout.ObjectField(_object, typeof(T), false);
    }

    private void ShowFloatField(ref float numberOut, string variableName)
    {
        EditorGUI.BeginChangeCheck();
        float number = EditorGUILayout.FloatField(variableName, numberOut);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(shipConfiguration, variableName);
            numberOut = number;
        }
    }

    private void GUICheckBox(string text)
    {
        EditorGUI.BeginChangeCheck();

        //showDefaultInspector = 
        bool _showDefaultInspector = EditorGUILayout.Toggle(text, showDefaultInspector);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(shipConfiguration, $"GUICheckBox | {text}");
            Debug.Log("RecordObject");
            showDefaultInspector = _showDefaultInspector;
        }
    }

}
