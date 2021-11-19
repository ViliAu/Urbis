using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DropTable))]
public class DropTableEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        (target as DropTable).RollDrop();
    }
}
