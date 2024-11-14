using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IconGenerator : MonoBehaviour
{
    Camera cam;
    public string pathFolder;
    public List<GameObject> sceneObjects;
    public List<InventoryItem> dataObjects;
    public string namePrefix;
    public string nameSuffix;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    [ContextMenu("Screenshot")]
    private void ProcessScreenshots()
    {
        StartCoroutine(Screenshot());
    }

    private IEnumerator Screenshot()
    {
        for(int i = 0; i < sceneObjects.Count; i++)
        {
            GameObject obj = sceneObjects[i];

            obj.gameObject.SetActive(true);

            yield return null;

            TakeScreenshot($"{Application.dataPath}/{pathFolder}/{namePrefix}{obj.name}{nameSuffix}_Icon.png");

            yield return null;
            obj.gameObject.SetActive(false);

            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/{pathFolder}/{namePrefix}{obj.name}{nameSuffix}_Icon.png");
            if (s != null && dataObjects != null && dataObjects.Count > i)
            {
                InventoryItem data = dataObjects[i];

                data.itemIcon = s;
                EditorUtility.SetDirty(data);
            }

            yield return null;
        }
    }
        
    void TakeScreenshot(string fullPath)
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        RenderTexture rt = new RenderTexture(256, 256, 24);
        cam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        cam.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        cam.targetTexture = null;
        RenderTexture.active = null;

        if (Application.isEditor)
        {
            DestroyImmediate(rt);
        }
        else
        {
            Destroy(rt);
        }

        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}
