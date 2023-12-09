using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Unity.VisualScripting;


[CustomEditor(typeof(ShipConfiguration))]
public class ShipConfigurationEditor : Editor
{
    private bool showDefaultInspector = true;
    private bool showEditor = true;

    private ShipConfiguration shipConfiguration;


    // ENGINES
    private List<Engine> engines;
    private List<Transform> transformEngines;
    private List<int> IDEngines;

    // EASY GUNS
    private List<Gun> easyGuns;
    private List<Transform> transformEasyGuns;
    private List<int> IDEasyGuns;

    //EASY GUNS
    private List<Gun> heavyGuns;
    private List<Transform> transformHeavyGuns;
    private List<int> IDHeavyGuns;




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
        ShowEditor();

        ShowDefaultInspector();
    }

    private void ShowEditor()
    {
        Foldout(ref showEditor, ref shipConfiguration.showEditor, "Show Editor");
        if (showEditor)
            DrawEditorInspector();
    }

    private void ShowDefaultInspector()
    {
        Foldout(ref showDefaultInspector, ref shipConfiguration.showDefaultInspector, "Show Default Inspector");
        if (showDefaultInspector)
            DrawDefaultInspector();
    }

    private void Foldout(ref bool thisBool, ref bool Bool, string text)
    {
        EditorGUI.BeginChangeCheck();

        thisBool = EditorGUILayout.Foldout(Bool, text);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(shipConfiguration, $"{text} changed");
            Bool = thisBool;
        }
    }

    private void DrawEditorInspector()
    {
        ShowParameters<Engine, int, ItemID.Engine>("Add Engine", "Remove Engine",
        ref shipConfiguration.engines, ref shipConfiguration.transformsEngines, ref shipConfiguration.IDEngines,
        ref engines, ref transformEngines, ref IDEngines, ItemShip.Type.Engine, ItemID.PathToEngines);

        GUILayout.Space(10);

        ShowParameters<Gun, int, ItemID.EasyGun>("Add Easy Gun", "Remove Easy Gun",
        ref shipConfiguration.easyGuns, ref shipConfiguration.transformEasyGuns, ref shipConfiguration.IDEasyGuns,
        ref easyGuns, ref transformEasyGuns, ref IDEasyGuns, ItemShip.Type.EasyGun, ItemID.PathToEasyGuns);

        GUILayout.Space(10);

        ShowParameters<Gun, int, ItemID.EasyGun>("Add Heavy Gun", "Remove Heavy Gun",
        ref shipConfiguration.heavyGuns, ref shipConfiguration.transformHeavyGuns, ref shipConfiguration.IDHeavyGuns,
        ref heavyGuns, ref transformHeavyGuns, ref IDHeavyGuns, ItemShip.Type.HeavyGun, ItemID.PathToHeavyGuns);
    }

    private void ShowParameters<T, V, U>(string addButtonText, string removeButtonText,
    ref List<T> shipParts, ref List<Transform> shipPartTransforms, ref List<V> shipIDParts,
    ref List<T> thisShipParts, ref List<Transform> thisShipPartTransforms, ref List<V> thisShipIDParts,
    ItemShip.Type typeOfPartShipTarget, List<string> PathToObjects)
    where T : ItemShip
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
                    DestroyObj<T>(shipPartTransforms[index].GetComponentInChildren<T>()?.gameObject, index, ref shipParts);
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
            Transform partTransform = thisShipPartTransforms[i];
            if (EditorGUI.EndChangeCheck())
            {
                if (partTransform != null)
                {
                    ShipSpot itemPart = partTransform.GetComponent<ShipSpot>();
                    if (itemPart != null && itemPart.Type == typeOfPartShipTarget
                        && !shipPartTransforms.Contains(partTransform) && (int)(object)thisShipIDParts[i] == 0)
                    {
                        Undo.RegisterCompleteObjectUndo(shipConfiguration, $"Transform set");
                        shipPartTransforms[i] = partTransform;
                    }
                }
                else if (shipPartTransforms[i] != null)
                {
                    Undo.RegisterFullObjectHierarchyUndo(shipConfiguration.gameObject, $"Transform delete");
                    DestroyObj<T>(shipPartTransforms[i].GetComponentInChildren<T>()?.gameObject, i, ref shipParts);
                    shipIDParts[i] = (V)(object)ItemID.Engine.Unknown;
                    shipPartTransforms[i] = null;
                }
                UpdateValues();
            }

            if (partTransform != null)
            {
                EditorGUI.BeginChangeCheck();
                thisShipIDParts[i] = (V)(object)EditorGUILayout.EnumPopup((U)(object)shipIDParts[i]);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterFullObjectHierarchyUndo(shipConfiguration.gameObject, $"Change Engine Type");
                    int idPart = (int)(object)thisShipIDParts[i];
                    if (idPart != 0)
                    {
                        GameObject LoadedPrefab = Resources.Load(PathToObjects[idPart], typeof(GameObject)) as GameObject;
                        T componentT = LoadedPrefab.GetComponent<T>();
                        if (componentT != null && componentT.ItemType == typeOfPartShipTarget)
                        {
                            ShipSpot itemPart = partTransform.GetComponent<ShipSpot>();
                            SetObj<T>(LoadedPrefab, thisShipPartTransforms[i], i, ref shipParts, (itemPart != null && itemPart.LeftSide == true));
                            shipIDParts[i] = thisShipIDParts[i];
                        }
                    }
                    else if (idPart == 0)
                    {
                        DestroyObj<T>(thisShipPartTransforms[i].GetComponentInChildren<T>()?.gameObject, i, ref shipParts);
                        shipIDParts[i] = thisShipIDParts[i];
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

    private void SetObj<T>(GameObject instantObject, Transform parent, int indexEngine, ref List<T> targetList, bool scaleReverse) where T : Behaviour
    {
        if (instantObject == null || parent == null) return;

        UnityEngine.Object prefab = PrefabUtility.InstantiatePrefab(instantObject, parent);
        T engine = parent.GetComponentInChildren<T>();
        targetList[indexEngine] = engine;
        if (scaleReverse)
        {
            Transform transform = prefab.GetComponentInChildren<Transform>();
            if (transform != null)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        //Debug.Log("SetEngine");
    }

    private void UpdateValues()
    {
        engines = new List<Engine>(shipConfiguration.engines);
        transformEngines = new List<Transform>(shipConfiguration.transformsEngines);
        IDEngines = new List<int>(shipConfiguration.IDEngines);

        easyGuns = new List<Gun>(shipConfiguration.easyGuns);
        transformEasyGuns = new List<Transform>(shipConfiguration.transformEasyGuns);
        IDEasyGuns = new List<int>(shipConfiguration.IDEasyGuns);

        heavyGuns = new List<Gun>(shipConfiguration.heavyGuns);
        transformHeavyGuns = new List<Transform>(shipConfiguration.transformHeavyGuns);
        IDHeavyGuns = new List<int>(shipConfiguration.IDHeavyGuns);
        //Debug.Log("UpdateValues");
        showDefaultInspector = shipConfiguration.showDefaultInspector;
        showEditor = shipConfiguration.showEditor;
    }

}
