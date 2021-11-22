using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading;
using System.Collections.Generic;

// Disable unreachable code detected warning if not fading
#pragma warning disable 0162

public class IconCreator : EditorWindow {

    // Settings
    const int passAmount = 3;
    const bool fading = true;
    static Color outlineColor = new Color(0, 1, 0, 1);

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

            tex = ClipPixels(tex);

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
        return tex;
    }

    static Texture2D ClipPixels(Texture2D tex) {
        Color[] pixels = tex.GetPixels();
        Color clipColor = new Color(0, 0, 0, 1);
        for (int i = 0; i < pixels.Length; i++) {
            if (pixels[i].a == 1 ) {
                clipColor = pixels[i];
                break;
            }
        }
        // Delete pixels that are similiar to clipColor
        for (int i = 0; i < pixels.Length; i++) {
            if (pixels[i] == clipColor) {
                pixels[i].a = 0;
            }
        }
        // Draw outline
        pixels = DrawOutline(pixels);
        tex.SetPixels(pixels);
        return tex;
    }

    static Color[] DrawOutline(Color[] pixels) {
        Debug.Log("WTF");
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

    static float ColorPerimeter(Color[] pixels, Color alphaColor, int i, int width) {
        int texLength = pixels.Length;
        int texSide = 128;
        // Avoid ArgumentOutOfRangeExeption
        if (i + width > texLength-1 || i - width < 0 || i + texSide * width > texLength-1 || i - texSide * width < 0 ) {
            return 0;
        }
        // Check front
        for (int j = 0; j < width; j++) {
            if (pixels[i+j] != alphaColor && pixels[i+j].a == 1) {
                return 1f/* - (float)j / (float)width*/;
            }
        }
        
        // Check back
        for (int j = 0; j < width; j++) {
            if (pixels[i-j] != alphaColor && pixels[i-j].a == 1) {
                return 1f/* - (float)j / (float)width*/;
            }
        }
        // Check up
        for (int j = 0; j < width; j++) {
            if (pixels[i+j*texSide] != alphaColor && pixels[i+j*texSide].a == 1) {
                return 1f/* - (float)j / (float)width*/;
            }
        } 
        // Check down
        for (int j = 0; j < width; j++) {
            if (pixels[i-j*texSide] != alphaColor && pixels[i-j*texSide].a == 1) {
                return 1f/* - (float)j / (float)width*/;
            }
        }
        return 0;
    }
}