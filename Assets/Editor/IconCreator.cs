using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading;
using System.Collections.Generic;

// Disable unreachable code detected warning if not fading
#pragma warning disable 0162

public class IconCreator : EditorWindow {

    // Settings
    const int passAmount = 2;
    const bool fading = true;
    static Color outlineColor = new Color(1, 1, 1, 1);

    [MenuItem("Urbis/Generate Icons")]
    static void StartCreating() {
        // Scan all the items from items folder
        Entity[] objs = Resources.LoadAll<Entity>("Prefabs");
        if (objs.Length == 0) {
            Debug.Log("No prefabs found");
            return;
        }

        // Save each icon as PNG
        for (int i = 0; i < objs.Length; i++) {
            Texture2D tex = GetPreviewTexture(objs[i].gameObject);
            if (tex == null) {
                Debug.LogWarning("Gameobject "+objs[i].entityName+" didn't have a preview texture");
                continue;
            }

            tex = ProcessTexture(tex);

            // Encode icon to png
            byte[] decodedPhoto = tex.EncodeToPNG();
            if (decodedPhoto == null) {
                Debug.LogWarning("Failed to encode to PNG!");
                continue;
            }

            // Save the file as PNG
            const string path = "Assets\\Resources\\Textures\\UI\\Icons\\Generated\\";
            string fileName = objs[i].entityName + ".png";
            File.WriteAllBytes(path + fileName, decodedPhoto);
        }

        // Refresh project panel
        AssetDatabase.Refresh();
        ChangeToSprite(ref objs);
    }

    static void ChangeToSprite(ref Entity[] objs) {
        for (int i = 0; i < objs.Length; i++) {
            string path = "Assets\\Resources\\Textures\\UI\\Icons\\Generated\\";
            string fileName = objs[i].entityName + ".png";
            TextureImporter importer = AssetImporter.GetAtPath(path + fileName) as TextureImporter;
            if (importer != null) {
                importer.textureType = TextureImporterType.Sprite;
                importer.SaveAndReimport();
            }

            else {
                Debug.LogWarning("Couldn't change texture import settings");
            }
        }
    }

    static Texture2D GetPreviewTexture(Object obj) {
        Texture2D tex = null;
        int iterations = 250;

        while (tex == null && iterations > 0) {
            iterations--;
            tex = AssetPreview.GetAssetPreview(obj);
            Thread.Sleep(1);
        }
        Texture2D newTex = new Texture2D(tex.width, tex.height);
        newTex.SetPixels(tex.GetPixels());
        return newTex;
    }

    static Texture2D ProcessTexture(Texture2D tex) {
        Color[] pixels = tex.GetPixels();
        // Remove bg
        pixels = ApplyTransparency(pixels);
        // Draw outline
        pixels = DrawOutline(pixels);
        tex.SetPixels(pixels);
        return tex;
    }

    static Color[] ApplyTransparency(Color[] pixels) {
        int texLength = pixels.Length;
        int width = (int)Mathf.Sqrt(texLength);
        // Get bg color
        Color clipColor = new Color(0, 0, 0, 1);
        for (int i = 0; i < texLength; i++) {
            if (pixels[i].a == 1 ) {
                clipColor = pixels[i];
                break;
            }
        }
        // Remove bg based on passamount
        List<int> indexList = new List<int>();
        for (int i = 0; i < texLength; i++) {
            // Clippable pixel found
            if (pixels[i] == clipColor) {
                int score = 0;
                if (i > width) { // EDGE CHECK
                    if (pixels[i - width] == clipColor) {
                        score++;
                    }
                }
                else {
                    score++;
                }

                // DOWN
                if (i < texLength-width) { // EDGE CHECK
                    if (pixels[i + width] == clipColor) {
                        score++;
                    }
                }
                else {
                    score++;
                }

                // RIGHT
                if (i % width != 127) { // EDGE CHECK
                    if (pixels[i + 1] == clipColor) {
                        score++;
                    }
                }
                else {
                    score++;
                }

                // LEFT
                if (i % width != 0) { // EDGE CHECK
                    if (pixels[i - 1] == clipColor) {
                        score++;
                    }
                }
                else {
                    score++;
                }
                if (score > 2) {
                    indexList.Add(i);
                }
            }
        }
        foreach (int i in indexList) {
            pixels[i].a = 0;
        }
        return pixels;
    }

    static Color[] DrawOutline(Color[] pixels) {
        int texLength = pixels.Length;
        int width = (int)Mathf.Sqrt(texLength);
        Color olColor = outlineColor;
        for (int i = 1; i <= passAmount; i++) {
            if (fading)
                olColor.a = (1/(float)i);
            List<int> indexList = new List<int>();
            for (int j = 0; j < texLength; j++) {
                if (pixels[j].a != 0 && !indexList.Contains(j)) {
                    // UP
                    if (j > width) { // EDGE CHECK
                        if (pixels[j - width].a == 0) {
                            pixels[j - width] = olColor;
                            indexList.Add(j - width);
                        }
                    }

                    // DOWN
                    if (j < texLength-width) { // EDGE CHECK
                        if (pixels[j + width].a == 0) {
                            pixels[j + width] = olColor;
                            indexList.Add(j + width);
                        }
                    }

                    // RIGHT
                    if (j % width != 127) { // EDGE CHECK
                        if (pixels[j + 1].a == 0) {
                            pixels[j + 1] = olColor;
                            indexList.Add(j + 1);
                        }
                    }

                    // LEFT
                    if (j % width != 0) { // EDGE CHECK
                        if (pixels[j - 1].a == 0) {
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