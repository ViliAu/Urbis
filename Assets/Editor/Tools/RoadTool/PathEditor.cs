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
            float minDistance = 0.1f;
            foreach(Path.Point p in points) {
                float dst = Vector2.Distance(mousePos, p.position);
                if (dst < minDistance) {
                    Undo.RecordObject(creator, "Set Point Active");
                    path.Connect(path.ActivePoint, p);
                    return;
                }
            }
            Undo.RecordObject(creator, "Add Point");
            path.AddPoint(mousePos);
            points = path.GetAllPoints(path.StartPoint);
        }
        // Making a node active
        else if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0/* && guiEvent.control*/) {
            float minDistance = 0.1f;
            foreach(Path.Point p in points) {
                float dst = Vector2.Distance(mousePos, p.position);
                if (dst < minDistance) {
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
        if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.Backspace) {
            Undo.RecordObject(creator, "Delete Point");
            path.RemovePoint(path.ActivePoint);
            points = path.GetAllPoints(path.StartPoint);
        }
        // Override mouse default
        HandleUtility.AddDefaultControl(0);
    }

    void Draw() {
        // TODO: Rewrite this loop
        foreach (Path.Point p in points) {
            Handles.color = path.ActivePoint == p ? creator.activeHandleColor : creator.bezierHandleColor;
            Vector3 newPos = Handles.FreeMoveHandle(p.position, Quaternion.identity, 0.1f, Vector2.zero, Handles.CylinderHandleCap);

            Vector3 control1 = Vector3.zero, control2 = Vector3.zero;
            if (creator.drawHandles) {
                // Draw handle points
                control1 = Handles.FreeMoveHandle(p.controlPoint1, Quaternion.identity, 0.05f, Vector2.zero, Handles.CylinderHandleCap);
                control2 = Handles.FreeMoveHandle(p.controlPoint2, Quaternion.identity, 0.05f, Vector2.zero, Handles.CylinderHandleCap);
                
                // Draw handle lines
                Handles.color = Color.black;
                Handles.DrawLine(p.controlPoint1, p.position);
                Handles.DrawLine(p.position, p.controlPoint2);
            }

            if (creator.drawHandles && control1 != p.controlPoint1) {
                Undo.RecordObject(creator, "Move Anchor1");
                p.MovePosition(control1, 0);
            }
            else if (p.position != newPos) {
                Undo.RecordObject(creator, "Move Point");
                p.MovePosition(newPos, 1);
            }
            else if (creator.drawHandles && control2 != p.controlPoint2) {
                Undo.RecordObject(creator, "Move Anchor2");
                p.MovePosition(control2, 2);
            }            

            // Draw absolute path lines and set path eval points
            p.bezierPoints.Clear();
            foreach(Path.Point poi in p.next) {
                if (poi.bezier) {
                    if (creator.drawAbsolutePath)
                        Handles.DrawBezier(p.position, poi.position, p.controlPoint2, poi.controlPoint1, creator.absolutePathColor, null, 2);
                    p.bezierPoints.Add(Handles.MakeBezierPoints(p.position, poi.position, p.controlPoint2, poi.controlPoint1, creator.segmentsPerLine));
                }
                else {
                    Handles.color = creator.absolutePathColor;
                    if (creator.drawAbsolutePath)
                        Handles.DrawLine(p.position, poi.position);
                    List<Vector3> linePoints = new List<Vector3>();
                    for (int i = 0; i < creator.segmentsPerLine; i++) {
                        linePoints.Add(p.position+(poi.position-p.position)*i/(creator.segmentsPerLine-1));
                    }
                    p.bezierPoints.Add(linePoints.ToArray());
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
