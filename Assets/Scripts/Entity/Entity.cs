using System.Globalization;
using UnityEngine;

public class Entity : MonoBehaviour {

    TextInfo t = new CultureInfo("en-US", false).TextInfo;

    [Header("Entity Settings")]
    public string entityName = "changeme";

    public string GetReadableEntityName() {
        return t.ToTitleCase(entityName).Replace('_', ' ');
    }

}