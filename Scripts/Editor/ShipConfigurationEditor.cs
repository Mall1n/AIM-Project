using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Unity.VisualScripting;


[CustomEditor(typeof(ShipConfiguration))]
public class ShipConfigurationEditor : Editor
{
    private bool showDefaultInspector = true;
    private ShipConfiguration shipConfiguration;


    // ENGINES
    private List<Engine> engines;
    private List<Transform> transformEngines;
    private List<int> IDEngines;

    //GUNS
    private List<Gun> guns;
    private List<Transform> transformGuns;
    private List<int> IDGuns;




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
        ShowParameters<Engine, int, ItemID.Engine>("Add Engine", "Remove Engine",
        ref shipConfiguration.engines, ref shipConfiguration.transformsForEngines, ref shipConfiguration.IDEngines,
        ref engines, ref transformEngines, ref IDEngines, TypeOfPartShip.TypeOfPart.Engine, ItemID.PathToEngines);

        GUILayout.Space(10);

        ShowParameters<Gun, int, ItemID.Gun>("Add Easy Gun", "Remove Easy Gun",
        ref shipConfiguration.guns, ref shipConfiguration.transformsForGuns, ref shipConfiguration.IDGuns,
        ref guns, ref transformGuns, ref IDGuns, TypeOfPartShip.TypeOfPart.EasyGun, ItemID.PathToGuns);

        GUILayout.Space(10);

        ShowParameters<Gun, int, ItemID.Gun>("Add Heavy Gun", "Remove Heavy Gun",
        ref shipConfiguration.guns, ref shipConfiguration.transformsForGuns, ref shipConfiguration.IDGuns,
        ref guns, ref transformGuns, ref IDGuns, TypeOfPartShip.TypeOfPart.HeavyGun, ItemID.PathToGuns);
    }


    private enum EnumParams
    {
        Engine = 1, Gun = 2
    }

    private void ShowParameters<T, V, U>(string addButtonText, string removeButtonText,
    ref List<T> shipParts, ref List<Transform> shipPartTransforms, ref List<V> shipIDParts,
    ref List<T> thisShipParts, ref List<Transform> thisShipPartTransforms, ref List<V> thisShipIDParts,
    TypeOfPartShip.TypeOfPart typeOfPartShipTarget, List<string> PathToObjects)
    where T : Behaviour
    where V : struct
    where U : Enum
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        GUILayout.Button(addButtonText);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(shipConfiguration, addButtonText);
            shipParts.Add(null);
            shipPartTransforms.Add(null);
            shipIDParts.Add((V)(object)0);
            UpdateValues();
        }

        EditorGUI.BeginChangeCheck();
        GUILayout.Button(removeButtonText);
        if (EditorGUI.EndChangeCheck())
        {
            int index = shipPartTransforms.Count;
            if (index != 0)
            {
                Undo.RegisterFullObjectHierarchyUndo(shipConfiguration.gameObject, removeButtonText);
                index -= 1;
                if (shipPartTransforms[index] != null && (int)(object)shipIDParts[index] != 0)
                    DestroyObj<T>(shipPartTransforms[index].GetComponentInChildren<Engine>()?.gameObject, index, ref shipParts);
                shipParts.RemoveAt(index);
                shipPartTransforms.RemoveAt(index);
                shipIDParts.RemoveAt(index);
                UpdateValues();
            }
        }

        GUILayout.EndHorizontal();

        for (int i = 0; i < thisShipPartTransforms.Count; i++)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();

            thisShipPartTransforms[i] = (Transform)EditorGUILayout.ObjectField(shipPartTransforms[i], typeof(Transform), true);
            Transform engineTransform = thisShipPartTransforms[i];
            if (EditorGUI.EndChangeCheck())
            {
                if (engineTransform != null)
                {
                    TypeOfPartShip typeOfPartShip = engineTransform.GetComponent<TypeOfPartShip>();
                    if (typeOfPartShip != null && typeOfPartShip.typeOfPart == typeOfPartShipTarget
                        && !shipPartTransforms.Contains(engineTransform) && (int)(object)thisShipIDParts[i] == 0)
                    {
                        Undo.RegisterCompleteObjectUndo(shipConfiguration, $"Transform set");
                        shipPartTransforms[i] = engineTransform;
                    }
                }
                else if (shipPartTransforms[i] != null)
                {
                    Undo.RegisterFullObjectHierarchyUndo(shipConfiguration.gameObject, $"Transform delete");
                    DestroyObj<T>(shipPartTransforms[i].GetComponentInChildren<Engine>()?.gameObject, i, ref shipParts);
                    shipIDParts[i] = (V)(object)ItemID.Engine.Unknown;
                    shipPartTransforms[i] = null;
                }
                UpdateValues();
            }

            if (engineTransform != null)
            {
                EditorGUI.BeginChangeCheck();
                thisShipIDParts[i] = (V)(object)EditorGUILayout.EnumPopup((U)(object)shipIDParts[i]);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterFullObjectHierarchyUndo(shipConfiguration.gameObject, $"Change Engine Type");
                    shipIDParts[i] = thisShipIDParts[i];

                    int idPart = (int)(object)shipIDParts[i];
                    if (idPart != 0)
                    {
                        GameObject LoadedEngine = Resources.Load(PathToObjects[idPart], typeof(GameObject)) as GameObject;
                        SetObj<T>(LoadedEngine, thisShipPartTransforms[i], i, ref shipParts);
                    }
                    else if (idPart == 0)
                    {
                        DestroyObj<T>(thisShipPartTransforms[i].GetComponentInChildren<Engine>()?.gameObject, i, ref shipParts);
                    }
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        GUILayout.EndHorizontal();
    }

    private void DestroyObj<T>(GameObject gameObject, int indexEngine, ref List<T> targetList) where T : Behaviour
    {
        if (gameObject == null) return;
        DestroyImmediate(gameObject);
        targetList[indexEngine] = null;
        // Debug.Log("Destroy");
    }

    private void SetObj<T>(GameObject instantObject, Transform parent, int indexEngine, ref List<T> targetList) where T : Behaviour
    {
        if (instantObject == null || parent == null) return;
        //Instantiate(instantObject, parent);
        PrefabUtility.InstantiatePrefab(instantObject, parent);
        T engine = parent.GetComponentInChildren<T>();
        targetList[indexEngine] = engine;
        //Debug.Log("SetEngine");
    }

    private void UpdateValues()
    {
        engines = new List<Engine>(shipConfiguration.engines);
        transformEngines = new List<Transform>(shipConfiguration.transformsForEngines);
        IDEngines = new List<int>(shipConfiguration.IDEngines);

        guns = new List<Gun>(shipConfiguration.guns);
        transformGuns = new List<Transform>(shipConfiguration.transformsForGuns);
        IDGuns = new List<int>(shipConfiguration.IDGuns);
        //Debug.Log("UpdateValues");
    }

}
