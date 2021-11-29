using UnityEngine;
using System.Collections;
using UnityEditor;

public class MattiEditor : EditorWindow {

    private Texture2D texA;
    private Texture2D prevTexA;
    private Texture2D texB;
    private Texture2D prevTexB;
    private string messageText;
    private string headerText;

    // Stylesd
    private GUIStyle buttonStyle;
    private GUIStyle headerStyle;
    private GUIStyle msgStyle;
    private GUIStyle texStyle;

    // Colors
    private Color colDefaultButtonBackground;

    [MenuItem("Window/Alpha Inverter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindowWithRect<MattiEditor>(new Rect(0, 0, 300, 800));
    }

    void SetupStyles()
    {
        colDefaultButtonBackground = GUI.backgroundColor;
        int fixedWidth = 250;

        // Button
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedWidth = fixedWidth;
        buttonStyle.fixedHeight = 35;
        buttonStyle.fontSize = 16;
        buttonStyle.alignment = TextAnchor.MiddleCenter;

        // Title label
        headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontSize = 25;
        headerStyle.fontStyle = FontStyle.Bold;

        // Message label
        msgStyle = new GUIStyle(GUI.skin.label);
        msgStyle.alignment = TextAnchor.MiddleCenter;
        msgStyle.fontSize = 15;

        // Texture style
        texStyle = new GUIStyle(GUI.skin.button);
        texStyle.alignment = TextAnchor.MiddleCenter;
        texStyle.fixedWidth = fixedWidth;
        texStyle.fixedHeight = fixedWidth;
    }

    void OnGUI()
    {
        float spacing = 15f;
        SetupStyles();

        // Title
        GUILayout.Space(spacing);
        GUILayout.Label(headerText, headerStyle);

        GUILayout.Space(spacing);

        // Texture button A
        GUILayout.BeginHorizontal();
        GUILayout.Space(Screen.width / 2 - 250 / 2);
        if (GUILayout.Button(prevTexA, texStyle))
        {
            EditorGUIUtility.ShowObjectPicker<Texture2D>(null, false, "", 69);
        }
        GUILayout.EndHorizontal();

        // Update selected texture A
        if (Event.current.commandName == "ObjectSelectorUpdated")
        {
            texA = EditorGUIUtility.GetObjectPickerObject() as Texture2D;
        }
        UpdatePreviewTexture(false);

        // Texture button B
        GUILayout.BeginHorizontal();
        GUILayout.Space(Screen.width / 2 - 250 / 2);
        if (GUILayout.Button(prevTexB, texStyle))
        {
            EditorGUIUtility.ShowObjectPicker<Texture2D>(null, false, "", 69);
        }
        GUILayout.EndHorizontal();

        // Update selected texture A
        if (Event.current.commandName == "ObjectSelectorUpdated")
        {
            texB = EditorGUIUtility.GetObjectPickerObject() as Texture2D;
        }
        UpdatePreviewTexture(true);

        GUILayout.Space(spacing);

        // Invert button
        GUILayout.BeginHorizontal();
        CenterButton();
        if (GUILayout.Button("Invert Alpha Channel", buttonStyle))
        {
        }
        GUILayout.EndHorizontal();
        GUI.enabled = true;

        GUILayout.Space(spacing);

        // Message label
        GUILayout.Label(messageText, msgStyle);
    }

    void UpdatePreviewTexture(bool ab)
    {
        Texture2D tex = ab == false ? texA : texB;
        Texture2D prevTex = ab == false ? prevTexA : prevTexB;
        if (tex == null)
        {
            prevTex = null;
            return;
        }

        // Make grayscale preview texture from selected textures alpha
        // if it's marked as readable 
        if (tex.isReadable)
        {
            prevTex = new Texture2D(tex.width, tex.height);
            Color[] pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i].r = pixels[i].a;
                pixels[i].g = pixels[i].a;
                pixels[i].b = pixels[i].a;
            }

            prevTex.SetPixels(pixels);
            prevTex.Apply();
        }

        // Otherwise just use the preview texture
        else
        {
            prevTex = tex;
        }
    }

    void CenterButton()
    {
        GUILayout.Space(Screen.width / 2 - buttonStyle.fixedWidth / 2);
    }
}