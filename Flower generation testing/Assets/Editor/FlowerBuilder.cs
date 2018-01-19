using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class FlowerBuilder : EditorWindow {

    GameObject previewprefab;
    public Texture2D previewprefabtex;
    int previewrefreshtimermax = 100;
    int previewrefreshtimer = 100;

    public int toolbarint;
    public string[] toolbarstrings = {"OPTIONS", "EDITOR"};

    public string filepath;
    public string prefabname;

    public int curcarpels;
    public int curstemens;
    public int curpetals;
    public int curstem;

    public Mesh[] carpelsarray;
    public Mesh[] stemensarray;
    public Mesh[] petalsarray;
    public Mesh[] stemarray;


    Vector2 previewsize = new Vector2(100, 100);
    public Texture2D carpelspreviewtex;
    public Texture2D stemenspreviewtex;
    public Texture2D petalpreviewtex;
    public Texture2D stempreviewtex;

    public Object[] carpelsobjectarray;
    public Object[] stemensobjectarray;
    public Object[] petalsobjectarray;
    public Object[] stemobjectarray;

    public Material[] carpelsmats;
    public Material[] stemensmats;
    public Material[] petalsmats;
    public Material[] stemmats;

    [MenuItem("FlowerBuilder/FlowerBuilder")]
    private static void Init()
    {
        FlowerBuilder window = (FlowerBuilder)EditorWindow.GetWindow(typeof(FlowerBuilder));
        window.Show();
        window.filepath = FlowerBuilderSerialization.LoadSerializedVariables()[0];
        window.prefabname = FlowerBuilderSerialization.LoadSerializedVariables()[1];
        window.LoadMeshArrays();
        window.LoadMaterialArrays();
        
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("flower builder editor by roel zwakman");
        GUILayout.Space(20);
        CreatePreviewPrefab();
        toolbarint = GUILayout.Toolbar(toolbarint, toolbarstrings);

        switch (toolbarint)
        {

            case 0:
            #region Editor variables UI

            if (GUILayout.Button("Select folder to save prefabs in"))
            {
                filepath = EditorUtility.OpenFolderPanel("Select prefab folder", "", "");
                //SerializeVariables();
            }
            GUILayout.Label("Saving prefabs to: " + filepath);
            GUILayout.Space(10);


            GUILayout.BeginHorizontal();
            GUILayout.Label("Name of prefab:      ");
            prefabname = EditorGUILayout.TextField("", prefabname);
            GUILayout.EndHorizontal();

        
            if (GUILayout.Button("Save preferences"))
            {
                //SerializeVariables();
                FlowerBuilderSerialization.SerializeVariables(filepath, prefabname);
            }

            GUILayout.Label("Profile file is saved as flowerbuilderprofile.txt in the project root folder. It is also loaded from here.");

            #endregion
            break;

            case 1:
            #region Prefab creation UI
            GUILayout.Space(10);
            GUILayout.Space(10);
            GUILayout.Space(10);

            GUILayout.Label("FLOWER CREATION");
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Carpels:  ");
            curcarpels = EditorGUILayout.IntSlider(curcarpels, 0, carpelsarray.Length);
            if (curcarpels > 0)
            {
                carpelspreviewtex = AssetPreview.GetAssetPreview(carpelsobjectarray[curcarpels - 1]);
                GUILayout.Box(carpelspreviewtex, GUILayout.Width(previewsize.x), GUILayout.Height(previewsize.y));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Stemens:");
            curstemens = EditorGUILayout.IntSlider(curstemens, 0, stemensarray.Length);
            if (curstemens > 0)
            {
                stemenspreviewtex = AssetPreview.GetAssetPreview(stemensobjectarray[curstemens - 1]);
                GUILayout.Box(stemenspreviewtex, GUILayout.Width(previewsize.x), GUILayout.Height(previewsize.y));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Petals:    ");
            curpetals = EditorGUILayout.IntSlider(curpetals, 0, petalsarray.Length);
            if (curpetals > 0)
            {
                petalpreviewtex = AssetPreview.GetAssetPreview(petalsobjectarray[curpetals - 1]);
                GUILayout.Box(petalpreviewtex, GUILayout.Width(previewsize.x), GUILayout.Height(previewsize.y));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Stems:    ");
            curstem = EditorGUILayout.IntSlider(curstem, 0, stemarray.Length);
            if (curstem > 0)
            {
                stempreviewtex = AssetPreview.GetAssetPreview(stemobjectarray[curstem - 1]);
                GUILayout.Box(stempreviewtex, GUILayout.Width(previewsize.x), GUILayout.Height(previewsize.y));
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Create prefab"))
            {
                if(curcarpels != 0 && curstemens != 0 && curpetals != 0 && curstem != 0)
                CreatePrefab(prefabname, curcarpels - 1, curstemens - 1, curpetals - 1, curstem - 1);
            }
            GUILayout.Label("PREVIEW OF PREFAB");
            previewprefabtex = AssetPreview.GetAssetPreview(previewprefab);
            GUILayout.Box(previewprefabtex, GUILayout.Width(previewsize.x), GUILayout.Height(previewsize.y));

            #endregion
            break;

        }
    }

    #region mesh loading

    private void LoadMeshArrays()
    {
        carpelsarray = LoadMeshesFromFolder("Meshes/carpels");
        stemensarray = LoadMeshesFromFolder("Meshes/stemens");
        petalsarray = LoadMeshesFromFolder("Meshes/petals");
        stemarray = LoadMeshesFromFolder("Meshes/stems");

        carpelsobjectarray = LoadMeshesFromFolderAsObject("Meshes/carpels");
        stemensobjectarray = LoadMeshesFromFolderAsObject("Meshes/stemens");
        petalsobjectarray = LoadMeshesFromFolderAsObject("Meshes/petals");
        stemobjectarray = LoadMeshesFromFolderAsObject("Meshes/stems");
    }

    private void LoadMaterialArrays()
    {
        carpelsmats = LoadMaterialsFromFolder("Meshes/carpels/Materials");
        stemensmats = LoadMaterialsFromFolder("Meshes/stemens/Materials");
        petalsmats = LoadMaterialsFromFolder("Meshes/petals/Materials");
        stemmats = LoadMaterialsFromFolder("Meshes/stems/Materials");
    }

    private Material[] LoadMaterialsFromFolder(string materialFileLocation)
    {
        Object[] _objectarray;
        Material[] _matarray;
        _objectarray = Resources.LoadAll(materialFileLocation, typeof(Material));
        _matarray = new Material[_objectarray.Length];
        for (int i = 0; i < _objectarray.Length; i++)
        {
            _matarray[i] = _objectarray[i] as Material;
            Debug.Log(_matarray[i].name);
        }
        
        return _matarray;
    }

    private Object[] LoadMeshesFromFolderAsObject(string meshFileLocation)
    {
        Object[] _objectarray;
        _objectarray = Resources.LoadAll(meshFileLocation, typeof(Mesh));
        return _objectarray;
    }

    private Mesh[] LoadMeshesFromFolder(string meshFileLocation) /////Laad alle meshes in de opgegeven folders en returnt een Mesh array
    {
        Object[] _objectarray;
        Mesh[] _mesharray;
        _objectarray = Resources.LoadAll(meshFileLocation, typeof(Mesh));
        _mesharray = new Mesh[_objectarray.Length];
        for (int i = 0; i < _objectarray.Length; i++)
        {
            _mesharray[i] = _objectarray[i] as Mesh;
        }

        return _mesharray;
    }

    #endregion

    #region prefab creation

    private string ParseFilepathGlobalToLocal(string _fp) ////Parses string from C://Folder/Folder/Project/Assets/Something/file format to Assets/Something/file
    {
        string _parsedFP = _fp;
        int _assetsstringIndex = _parsedFP.IndexOf("Assets");
        _parsedFP = _parsedFP.Remove(0, _assetsstringIndex);
        return _parsedFP;
    }

    private void CreatePrefab(string _prefabName, int _carpelMeshI, int _stemensMeshI, int _petalsMeshI, int _stemMeshI)
    {
        GameObject _flowerGO = new GameObject(_prefabName);

        Mesh _carpelMesh = carpelsarray[_carpelMeshI];
        Mesh _stemensMesh = stemensarray[_stemensMeshI];
        Mesh _petalsMesh = petalsarray[_petalsMeshI];
        Mesh _stemMesh = stemarray[_stemMeshI];

        GameObject _carpelGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MeshRenderer _carpelRenderer = _carpelGO.GetComponent<MeshRenderer>();
        _carpelGO.GetComponent<MeshFilter>().mesh = _carpelMesh;
        _carpelGO.name = "carpel";

        SerializedObject obj = new SerializedObject(_carpelRenderer);
        SerializedProperty serializedProperty = obj.FindProperty("material");
        _carpelRenderer.material = carpelsmats[_carpelMeshI];
        obj.ApplyModifiedProperties();

        GameObject _stemensGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MeshRenderer _stemensRenderer = _stemensGO.GetComponent<MeshRenderer>();
        _stemensGO.GetComponent<MeshFilter>().mesh = _stemensMesh;
        _stemensGO.name = "stemens";

        obj = new SerializedObject(_stemensRenderer);
        serializedProperty = obj.FindProperty("material");
        _stemensRenderer.material = stemensmats[_stemensMeshI];
        obj.ApplyModifiedProperties();

        GameObject _petalsGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MeshRenderer _petalsRenderer = _petalsGO.GetComponent<MeshRenderer>();
        _petalsGO.GetComponent<MeshFilter>().mesh = _petalsMesh;
        _petalsGO.name = "petals";

        obj = new SerializedObject(_petalsRenderer);
        serializedProperty = obj.FindProperty("material");
        _petalsRenderer.material = petalsmats[_petalsMeshI];
        obj.ApplyModifiedProperties();

        GameObject _stemGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MeshRenderer _stemRenderer = _stemGO.GetComponent<MeshRenderer>();
        _stemGO.GetComponent<MeshFilter>().mesh = _stemMesh;
        _stemGO.name = "stem";

        obj = new SerializedObject(_stemRenderer);
        serializedProperty = obj.FindProperty("material");
        _stemRenderer.material = stemmats[_stemMeshI];
        obj.ApplyModifiedProperties();

        _carpelGO.transform.SetParent(_flowerGO.transform);
        _stemensGO.transform.SetParent(_flowerGO.transform);
        _petalsGO.transform.SetParent(_flowerGO.transform);
        _stemGO.transform.SetParent(_flowerGO.transform);

        try
        { 
            GameObject prefab = PrefabUtility.CreatePrefab(ParseFilepathGlobalToLocal(filepath) + "/" + _flowerGO.name + ".prefab", _flowerGO);
            
            if (_flowerGO.name == "__previewprefab__") /////if this prefab is the previewprefab
            {
                previewprefab = prefab;
            }
            else
            {
                Debug.Log("Constructed prefab named " + _flowerGO.name + " in the folder " + ParseFilepathGlobalToLocal(filepath));
            }
            DestroyImmediate(_flowerGO);
            
        }
        catch (System.Exception e)
        {
            Debug.Log("Something was wrong with the folder you wanted to save the prefab in. Are you sure it's not empty? " + "Exception: " + e);
        }
    }

    private void CreatePreviewPrefab()
    {
        previewrefreshtimer--;
        if (curcarpels != 0 && curstemens != 0 && curpetals != 0 && curstem != 0 && previewrefreshtimer <= 0)
        { 
            CreatePrefab("__previewprefab__", curcarpels - 1, curstemens - 1, curpetals - 1, curstem - 1);
            previewrefreshtimer = previewrefreshtimermax;
        }
    }

    #endregion
}

