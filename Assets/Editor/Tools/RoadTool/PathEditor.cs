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
            RefreshPathPoints();
        }
        // Deleting points
        else if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.alt) {
            float minDistance = 0.1f;
            foreach(Path.Point p in points) {
                float dst = Vector2.Distance(mousePos, p.position);
                if (dst < minDistance) {
                    Undo.RecordObject(creator, "Delete Point");
                    path.RemovePoint(p);
                    RefreshPathPoints();
                    break;
                }
            }
        }
        // Making a node active
        else if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0) {
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

        if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.B) {
            path.ActivePoint.bezier = !path.ActivePoint.bezier;
            RefreshPathPoints();
        }
        if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.Backspace) {
            Undo.RecordObject(creator, "Delete Point");
            path.RemovePoint(path.ActivePoint);
            RefreshPathPoints();
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
                    if (creator.unityJuttu)
                        p.bezierPoints.Add(Handles.MakeBezierPoints(p.position, poi.position, p.controlPoint2, poi.controlPoint1, creator.segmentsPerLine));
                    else
                        p.bezierPoints.Add(CalculateEvenlySpacedPoints(p, poi, creator.spacing, creator.segmentsPerLine));
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

    private void RefreshPathPoints() {
        points.Clear();
        path.GetAllPoints(path.StartPoint, ref points);
    }

    private void OnEnable() {
        creator = (PathCreator)target;
        if (creator.path == null) {
            creator.CreatePath();
        }
        path = creator.path;
        RefreshPathPoints();
    }

    // Redo this calc
    public Vector3[] CalculateEvenlySpacedPoints(Path.Point p1, Path.Point p2, float spacing, float resolution = 10) {
        List<Vector3> evenlySpacedPoints = new List<Vector3>();
        evenlySpacedPoints.Add(p1.position);
        Vector2 previousPoint = p1.position;
        float dstSinceLastEvenPoint = 0;

        float controlNetLength = Vector2.Distance(p1.position, p1.controlPoint2)
            + Vector2.Distance(p1.controlPoint2, p2.controlPoint1) + Vector2.Distance(p2.controlPoint1, p2.position);
        float estimatedCurveLength = Vector2.Distance(p1.position, p2.position) + controlNetLength / 2f;
        int divisions = Mathf.CeilToInt(estimatedCurveLength * resolution);
        float t = 0;
        while (t <= 1) {
            t += 1f/divisions;
            Vector2 pointOnCurve = Bezier.EvaluateCubic(p1.position, p1.controlPoint2, p2.controlPoint1, p2.position, t);
            dstSinceLastEvenPoint += Vector2.Distance(previousPoint, pointOnCurve);

            while (dstSinceLastEvenPoint >= spacing)
            {
                float overshootDst = dstSinceLastEvenPoint - spacing;
                Vector2 newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDst;
                evenlySpacedPoints.Add(newEvenlySpacedPoint);
                dstSinceLastEvenPoint = overshootDst;
                previousPoint = newEvenlySpacedPoint;
            }

            previousPoint = pointOnCurve;
        }


        return evenlySpacedPoints.ToArray();
    }
}
