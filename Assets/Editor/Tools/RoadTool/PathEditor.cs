using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor {

    PathCreator creator;
    Path path;

    private List<Path.Point> points = new List<Path.Point>();

    private void OnSceneGUI() {
        Input();
        Draw();    
    }

    void Input() {
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
        // Adding points
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift) {
            Undo.RecordObject(creator, "Delete Point");
            path.AddPoint(mousePos);
            points = path.GetAllPoints(path.StartPoint);
        }
        // Making a node active
        else if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.control) {
            float minDistance = 0.1f;
            foreach(Path.Point p in points) {
                float dst = Vector2.Distance(mousePos, p.position);
                if (dst < minDistance) {
                    Debug.Log("a");
                    Undo.RecordObject(creator, "Set Point Active");
                    path.ActivePoint = p;
                    break;
                }
            }
        }
        // Deleting points
        else if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.alt) {
            float minDistance = 0.1f;
            foreach(Path.Point p in points) {
                float dst = Vector2.Distance(mousePos, p.position);
                if (dst < minDistance) {
                    Undo.RecordObject(creator, "Delete Point");
                    path.RemovePoint(p);
                    points = path.GetAllPoints(path.StartPoint);
                    break;
                }
            }
        }
        if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.B) {
            path.ActivePoint.bezier = !path.ActivePoint.bezier;
            points = path.GetAllPoints(path.StartPoint);
        }
        if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.N) {
            creator.drawEval = !creator.drawEval;
        }
        HandleUtility.AddDefaultControl(0);
    }

    void Draw() {
        
        // TODO: Rewrite this loop
        foreach (Path.Point p in points) {
            Handles.color = Color.red;
            Vector3 newAnc1 = Handles.FreeMoveHandle(p.controlPoint1, Quaternion.identity, 0.05f, Vector2.zero, Handles.CylinderHandleCap);
            Vector3 newAnc2 = Handles.FreeMoveHandle(p.controlPoint2, Quaternion.identity, 0.05f, Vector2.zero, Handles.CylinderHandleCap);
            Handles.color = path.ActivePoint == p ? Color.yellow : Color.red;
            Vector3 newPos = Handles.FreeMoveHandle(p.position, Quaternion.identity, 0.1f, Vector2.zero, Handles.CylinderHandleCap);

            if (newAnc1 != p.controlPoint1) {
                Undo.RecordObject(creator, "Move Anchor1");
                p.MovePosition(newAnc1, 0);
            }
            else if (p.position != newPos) {
                Undo.RecordObject(creator, "Move Point");
                p.MovePosition(newPos, 1);
            }
            else if (newAnc2 != p.controlPoint2) {
                Undo.RecordObject(creator, "Move Anchor2");
                p.MovePosition(newAnc2, 2);
            }

            Handles.color = Color.black;
            Handles.DrawLine(p.controlPoint1, p.position);
            Handles.DrawLine(p.position, p.controlPoint2);

            foreach(Path.Point poi in p.next) {
                if (poi.bezier)
                    Handles.DrawBezier(p.position, poi.position, p.controlPoint2, poi.controlPoint1, Color.green, null, 2);
                else {
                    Handles.color = Color.green;
                    Handles.DrawLine(p.position, poi.position);
                }
            }
        }
    }

    private void OnEnable() {
        creator = (PathCreator)target;
        if (creator.path == null) {
            creator.CreatePath();
        }
        path = creator.path;
        points = path.GetAllPoints(path.StartPoint);
    }
}
