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
    private Engine[] engines;
    private Transform[] transformsEngine;
    private List<int> IDEngines;

    // EASY GUNS
    private Gun[] easyGuns;
    private Transform[] transformsEasyGun;
    private List<int> IDEasyGuns;

    // EASY GUNS
    private Gun[] heavyGuns;
    private Transform[] transformsHeavyGun;
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
        // UpdateValues();

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
        ShowParameters<Engine, int, ItemID.Engine>(
        ref shipConfiguration.engines, ref shipConfiguration.transformsEngine, ref shipConfiguration.IDEngines,
        ref engines, ref transformsEngine, ref IDEngines,
        ItemShip.Type.Engine, ItemID.PathToEngines);

        GUILayout.Space(10);

        ShowParameters<Gun, int, ItemID.EasyGun>(
        ref shipConfiguration.easyGuns, ref shipConfiguration.transformsEasyGun, ref shipConfiguration.IDEasyGuns,
        ref easyGuns, ref transformsEasyGun, ref IDEasyGuns,
        ItemShip.Type.EasyGun, ItemID.PathToEasyGuns);

        GUILayout.Space(10);

        ShowParameters<Gun, int, ItemID.HeavyGun>(
        ref shipConfiguration.heavyGuns, ref shipConfiguration.transformsHeavyGun, ref shipConfiguration.IDHeavyGuns,
        ref heavyGuns, ref transformsHeavyGun, ref IDHeavyGuns,
        ItemShip.Type.HeavyGun, ItemID.PathToHeavyGuns);
    }

    private void ShowParameters<T, V, U>(
    ref T[] shipParts, ref Transform[] shipPartTransforms, ref List<V> shipIDParts,
    ref T[] thisShipParts, ref Transform[] thisShipPartTransforms, ref List<V> thisShipIDParts,
    ItemShip.Type typeOfPartShipTarget, List<string> PathToObjects)
    where T : ItemShip
    where V : struct
    where U : Enum
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);

        for (int i = 0; i < thisShipPartTransforms.Length; i++)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.BeginVertical();

            thisShipPartTransforms[i] = (Transform)EditorGUILayout.ObjectField(shipPartTransforms[i], typeof(Transform), true);
            Transform partTransform = thisShipPartTransforms[i];
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
                            GameObject previousObject = thisShipPartTransforms[i].GetComponentInChildren<T>()?.gameObject;
                            if (previousObject != null)
                                DestroyObject<T>(previousObject, i, ref shipParts);

                            ShipSpot itemPart = partTransform.GetComponent<ShipSpot>();
                            SetObject<T>(LoadedPrefab, thisShipPartTransforms[i], i, ref shipParts, itemPart.Side);
                            shipIDParts[i] = thisShipIDParts[i];
                        }
                    }
                    else if (idPart == 0)
                    {
                        GameObject previousObject = thisShipPartTransforms[i].GetComponentInChildren<T>()?.gameObject;
                        if (previousObject != null)
                            DestroyObject<T>(previousObject, i, ref shipParts);
                        shipIDParts[i] = thisShipIDParts[i];
                    }
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        GUILayout.EndHorizontal();
    }

    private void DestroyObject<T>(GameObject gameObject, int indexEngine, ref T[] targetList) where T : ItemShip
    {
        DestroyImmediate(gameObject);
        targetList[indexEngine] = null;
    }

    private void SetObject<T>(GameObject instantObject, Transform parent, int indexEngine, ref T[] targetList, ItemShip.Side side) where T : ItemShip
    {
        if (instantObject == null || parent == null) return;

        UnityEngine.Object prefab = PrefabUtility.InstantiatePrefab(instantObject, parent);
        T _object = parent.GetComponentInChildren<T>();
        targetList[indexEngine] = _object;

        _object.ItemSide = side;
        if ((int)side == 1)
        {
            Transform transform = prefab.GetComponentInChildren<Transform>();
            if (transform != null)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void UpdateValues()
    {
        engines = new Engine[shipConfiguration.engines.Length];
        shipConfiguration.engines.CopyTo(engines, 0);
        transformsEngine = new Transform[shipConfiguration.transformsEngine.Length];
        shipConfiguration.transformsEngine.CopyTo(transformsEngine, 0);
        IDEngines = new List<int>(shipConfiguration.IDEngines);

        easyGuns = new Gun[shipConfiguration.easyGuns.Length];
        shipConfiguration.easyGuns.CopyTo(easyGuns, 0);
        transformsEasyGun = new Transform[shipConfiguration.transformsEasyGun.Length];
        shipConfiguration.transformsEasyGun.CopyTo(transformsEasyGun, 0);
        IDEasyGuns = new List<int>(shipConfiguration.IDEasyGuns);

        heavyGuns = new Gun[shipConfiguration.heavyGuns.Length];
        shipConfiguration.heavyGuns.CopyTo(heavyGuns, 0);
        transformsHeavyGun = new Transform[shipConfiguration.transformsHeavyGun.Length];
        shipConfiguration.transformsHeavyGun.CopyTo(transformsHeavyGun, 0);
        IDHeavyGuns = new List<int>(shipConfiguration.IDHeavyGuns);
        //Debug.Log("UpdateValues");

        showDefaultInspector = shipConfiguration.showDefaultInspector;
        showEditor = shipConfiguration.showEditor;
    }

}
