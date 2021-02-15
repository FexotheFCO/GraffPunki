using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Paintable : MonoBehaviour
{
    public GameObject brush;
    public Material brushColor;
    public List<Material> Colors;
    public Camera playerCamera;
    public float brushSize = 0.1f;
    public RenderTexture RTexture;
    public Material FinishedMaterial;
    public GameObject graffitiPrefab;



    List<GameObject> decals = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        setBrushColor();
    }

    // Update is called once per frame
    public void changeColor(int index)
    {
        brushColor = Colors[index];
        setBrushColor();
    }
    private void setBrushColor()
    {
        Renderer renderer = brush.GetComponent<Renderer>();
        renderer.enabled = true;
        renderer.sharedMaterial = brushColor;
    }
    private void createDecal(bool offSet, RaycastHit hit)
    {
        var decal = Instantiate(brush, this.transform);
        decal.transform.position = hit.point;
        decal.transform.Rotate(90, 0, 0);

        Vector3 newPosition = decal.transform.position;

        if (offSet)
        {
            newPosition.y += 0.000001f;
        }
        else
        {
            newPosition.y = 1;
        }
        
        decal.transform.position = newPosition;

        decals.Add(decal);
        decal.transform.localScale *= brushSize;
    }
    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
    public IEnumerator TakeSnapshot()
    {
        yield return frameEnd;

        RenderTexture.active = RTexture;
        Texture2D tex = new Texture2D(RTexture.width, RTexture.height);
        tex.ReadPixels(new Rect(0, 0, RTexture.width, RTexture.height), 0, 0);
        tex.Apply();
        File.WriteAllBytes(Application.dataPath + "/Resources/" + "graffitiTexture.png", tex.EncodeToPNG());

        //AssetDatabase.Refresh();
        /*
        Material newMaterial = new Material(Shader.Find("Standard"));

        newMaterial.SetFloat("_Mode", 3);

        newMaterial.EnableKeyword("_Emission");
        newMaterial.SetColor("_EMISSION", Color.white);
        newMaterial.SetTexture("_EmissionMap", tex);
        Debug.Log(newMaterial.IsKeywordEnabled("_EMISSION"));
        //Transparent
        

        AssetDatabase.CreateAsset(newMaterial, "Assets/Materiales/Graffitis/SavedGraffitis/" + "newSavedMaterial" + ".mat");
        AssetDatabase.SaveAssets();

        GameObject newGraffiti = Instantiate(graffitiPrefab);
        MeshRenderer renderer = newGraffiti.GetComponent<MeshRenderer>();
        renderer.enabled = true;
        renderer.material = newMaterial;
        renderer.sharedMaterial = newMaterial;


        
        PrefabUtility.SaveAsPrefabAsset(newGraffiti, "Assets/Materiales/Graffitis/SavedGraffitis/" + "newSavedGraffiti" + ".prefab");*/
    }
    public void saveGraffiti()
    {
        StartCoroutine(TakeSnapshot());
        //Object savedGraffiti = EditorUtility.CreateEmptyPrefab("Assets/Materiales/Graffitis/SavedGraffitis/" + "newSavedGraffiti" + ".prefab");
    }
    public void changeSceneToGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool isRaycastHiting = Physics.Raycast(ray, out hit, 1000f);
            if(isRaycastHiting && hit.transform.gameObject.layer == LayerMask.NameToLayer("PainteableWall") && hit.transform.gameObject.layer != LayerMask.NameToLayer("GraffitiPaint"))
            {
                createDecal(false, hit);
            }
            else if(isRaycastHiting && hit.transform.gameObject.layer == LayerMask.NameToLayer("GraffitiPaint") && hit.transform.gameObject.GetComponent<Renderer>().sharedMaterial.name != brushColor.name)
            {
                createDecal(true, hit);
            }
        }
        /*if (Input.GetKeyDown(KeyCode.Y))
        {
            GameObject newGraffiti = Instantiate(graffitiPrefab);
            MeshRenderer renderer = newGraffiti.GetComponent<MeshRenderer>();
            renderer.enabled = true;
            renderer.material = FinishedMaterial;
            renderer.sharedMaterial = FinishedMaterial;

            PrefabUtility.SaveAsPrefabAsset(newGraffiti, "Assets/Materiales/Graffitis/SavedGraffitis/" + "newSavedGraffiti" + ".prefab");
            //Object savedGraffiti = EditorUtility.CreateEmptyPrefab("Assets/Materiales/Graffitis/SavedGraffitis/" + "newSavedGraffiti" + ".prefab");

        }*/
        //StartCoroutine(TakeSnapshot());
        //Material newMaterial = redMaterial;
        //RenderTexture.active = RT_Red;

        //light.enabled = false;
        /*
        var texture2D = new Texture2D(RT_Red.width, RT_Red.height);
        texture2D.ReadPixels(new Rect(0, 0, RT_Red.width, RT_Red.height), 0, 0);
        texture2D.Apply();

        redMaterial.SetTexture("_MainTex", texture2D);
        redMaterial.SetTexture("_EmissionMap", texture2D);
        */
        //Renderer renderer = GetComponent<Renderer>();
        //renderer.enabled = true;
        //renderer.sharedMaterial = newMaterial;
        //objectMeshRenderer.material = newMaterial;

        /*foreach(GameObject decal in decals)
        {
            Destroy(decal);
        }*/
        //light.enabled = true;
        /*
        RaycastHit hit;
        if (, distancia))
        {
            Debug.Log(hit);
            Activador activador = hit.transform.GetComponent<Activador>();
            if (activador != null)
            {
                Debug.Log(activador);
                activador.activar();
            }
        }*/
    }
    /*WaitForSeconds waitTime = new WaitForSeconds(0.1F);
    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
    public IEnumerator TakeSnapshot()
    {
        yield return waitTime;
        yield return frameEnd;

        var texture2D = new Texture2D(RT_Red.width, RT_Red.height);
        texture2D.ReadPixels(new Rect(0, 0, RT_Red.width, RT_Red.height), 0, 0);
        texture2D.Apply();

        redMaterial.SetTexture("_MainTex", texture2D);
        redMaterial.SetTexture("_EmissionMap", texture2D);
    }*/
}
