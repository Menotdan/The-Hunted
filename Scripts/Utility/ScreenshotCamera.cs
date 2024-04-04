using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScreenshotCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Take Screenshot")]
    void screenshot()
    {
        Take_Screenshot("Assets\\Icons\\InventoryIcons\\item.png");
    }

    void Take_Screenshot(string full_path)
    {
        Camera camera = Camera.main;

        RenderTexture rt = new RenderTexture(256, 256, 24);
        camera.targetTexture = rt;

        Texture2D screenshot = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        camera.Render();
        RenderTexture.active = rt;

        screenshot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;

        if (Application.isEditor)
        {
            DestroyImmediate(rt);
        }
        else
        {
            Destroy(rt);
        }

        byte[] bytes = screenshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(full_path, bytes);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}
