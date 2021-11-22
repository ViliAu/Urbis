using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading;
using System.Collections.Generic;

// Disable unreachable code detected warning if not fading
#pragma warning disable 0162

public class IconCreatorV2 : EditorWindow
{

    // Settings
    const int passAmount = 3;
    const bool fading = true;
    const float forwardOffset = 5;
    const float upOffset = 0;
    static Vector2 size = new Vector2(128, 128);
    static float padding = 0.01f;
    static Color outlineColor = new Color(1, 1, 1, 1);

    const string path = "Assets\\Resources\\Textures\\UI\\Icons\\Generated\\";

    [MenuItem("Urbis/Generate Icons V2")]
    static void StartCreating() {
        // Setup camera
        Camera camera = new GameObject("Kamppi").AddComponent<Camera>();
        camera.transform.position += Vector3.up * 100;
        camera.orthographic = true;
        camera.orthographicSize = 0.3f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(1, 0, 1, 1);

        // Scan all the items from items folder
        Entity[] objs = Resources.LoadAll<Entity>("Prefabs");
        if (objs.Length == 0)
        {
            Debug.Log("No prefabs found");
            return;
        }

        RenderTexture texture = new RenderTexture((int)size.x, (int)size.y, 24);
        foreach (Entity e in objs) {
            // Setup gameobject
            GameObject go = Instantiate(e.gameObject, Vector3.zero, Quaternion.identity);
            MeshFilter m = go.transform.GetComponentInChildren<MeshFilter>();
            Bounds bounds = m.sharedMesh.bounds;
            go.transform.position = camera.transform.position - bounds.center + camera.transform.forward * 1;
            //camera.transform.position = go.transform.position + bounds.center + go.transform.forward * forwardOffset;
            //camera.transform.LookAt(go.transform.position + bounds.center);

            // Get scale depending on the gheometry
            /* EHKÄ HULLU FIKSI
                1. ota isompi luku size x ja y arvoista
                2. Jaa ne sillee et outista tulee 1
                3. applyy scale
            */
            
            // Wide
            float scale = 0;
            if (bounds.size.x > bounds.size.y) {
                scale = Vector3.Distance(camera.ViewportToWorldPoint(new Vector3(padding, 0.5f, 1)),
                    camera.ViewportToWorldPoint(new Vector3(1f - padding, 0.5f, 1))) / bounds.size.x;
            }
            else {
                scale = Vector3.Distance(camera.ViewportToWorldPoint(new Vector3(0.5f, padding, 1)),
                    camera.ViewportToWorldPoint(new Vector3(0.5f, 1f - padding, 1))) / bounds.size.y;
            }
            //Apply the scale to the model.
            go.transform.localScale = Vector3.one * scale;
            go.transform.position = camera.transform.position - bounds.center * scale + camera.transform.forward * 1;
            camera.targetTexture = texture;
            camera.Render();

            //return;

            // Create texture from camera view
            Texture2D tex = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
            RenderTexture.active = texture;
            tex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);

            // Set transparent bg
            Color32[] colors = tex.GetPixels32();
            int i = 0;
            Color32 transparent = colors[i];
            for (; i < colors.Length; i++)
            {
                if (colors[i].Equals(transparent))
                {
                    colors[i] = new Color32();
                }
            }
            // Set outline
            colors = DrawOutline(colors);

            tex.SetPixels32(colors);

            // Free renderTex
            RenderTexture.active = null;

            // Save as png
            string fullPath = path + e.entityName + ".png";
            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(fullPath, bytes);

            // Save as sprite
            AssetDatabase.ImportAsset(fullPath);
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(fullPath);
            ti.textureType = TextureImporterType.Sprite;
            ti.SaveAndReimport();
            DestroyImmediate(go);
        }
        DestroyImmediate(camera.gameObject);
        return;
    }

    static Color32[] DrawOutline(Color32[] pixels) {
        int texLength = pixels.Length;
        int width = (int)Mathf.Sqrt(texLength);
        Color olColor = outlineColor;
        for (int i = 1; i <= passAmount; i++)
        {
            if (fading)
                olColor.a = (1 / (float)i);
            List<int> indexList = new List<int>();
            for (int j = 0; j < texLength; j++)
            {
                if (pixels[j].a != 0 && !indexList.Contains(j))
                {
                    // UP
                    if (j > width)
                    { // EDGE CHECK
                        if (pixels[j - width].a == 0)
                        {
                            pixels[j - width] = olColor;
                            indexList.Add(j - width);
                        }
                    }

                    // DOWN
                    if (j < texLength - width)
                    { // EDGE CHECK
                        if (pixels[j + width].a == 0)
                        {
                            pixels[j + width] = olColor;
                            indexList.Add(j + width);
                        }
                    }

                    // RIGHT
                    if (j % width != 127)
                    { // EDGE CHECK
                        if (pixels[j + 1].a == 0)
                        {
                            pixels[j + 1] = olColor;
                            indexList.Add(j + 1);
                        }
                    }

                    // LEFT
                    if (j % width != 0)
                    { // EDGE CHECK
                        if (pixels[j - 1].a == 0)
                        {
                            pixels[j - 1] = olColor;
                            indexList.Add(j - 1);
                        }
                    }
                }
            }
        }
        return pixels;
    }
}