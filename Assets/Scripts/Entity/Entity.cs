using System.Globalization;
using UnityEngine;
using Mirror;

public class Entity : NetworkBehaviour {

    TextInfo t = new CultureInfo("en-US", false).TextInfo;

    [Header("Entity Settings")]
    public string entityName = "changeme";

    public string GetReadableEntityName() {
        return t.ToTitleCase(entityName).Replace('_', ' ');
    }

}