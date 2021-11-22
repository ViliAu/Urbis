using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadCreator))]
public class RoadEditor : Editor {

    RoadCreator creator;
    RoadData road;

    private List<RoadData.Point> points = new List<RoadData.Point>();

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
            road.AddPoint(mousePos);
            points = road.GetAllPoints(road.StartPoint);
        }
        // Making a node active
        else if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.control) {
            float minDistance = 0.05f;
            foreach(RoadData.Point p in points) {
                float dst = Vector2.Distance(mousePos, p.position);
                if (dst < minDistance) {
                    Undo.RecordObject(creator, "Set Point Active");
                    road.ActivePoint = p;
                    break;
                }
            }
        }
        // Deleting points
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.alt) {
            float minDistance = 0.05f;
            foreach(RoadData.Point p in points) {
                float dst = Vector2.Distance(mousePos, p.position);
                if (dst < minDistance) {
                    Undo.RecordObject(creator, "Delete Point");
                    road.RemovePoint(p);
                    points = road.GetAllPoints(road.StartPoint);
                    break;
                }
            }
        }
    }

    void Draw() {
        if (points.Count == 0)
            points = road.GetAllPoints(road.StartPoint);
        Handles.color = Color.red;
        // TODO: Rewrite this loop
        double t3 = EditorApplication.timeSinceStartup;
        foreach (RoadData.Point p in points) {
            Vector3 newAnc1 = Handles.FreeMoveHandle(p.controlPoint1, Quaternion.identity, 0.1f, Vector2.zero, Handles.CylinderHandleCap);
            Vector3 newAnc2 = Handles.FreeMoveHandle(p.controlPoint2, Quaternion.identity, 0.1f, Vector2.zero, Handles.CylinderHandleCap);
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

            Handles.DrawLine(p.controlPoint1, p.position);
            Handles.DrawLine(p.position, p.controlPoint2);

            foreach(RoadData.Point poi in p.next) {
                Handles.DrawBezier(p.position, poi.position, p.controlPoint2, poi.controlPoint1, Color.green, null, 2);
            }
        }
    }

    private void OnEnable() {
        creator = (RoadCreator)target;
        if (creator.path == null) {
            creator.CreatePath();
        }
        road = creator.path;
    }
}
