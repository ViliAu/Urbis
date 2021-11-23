using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path {
    [SerializeField] private Point startPoint;
    [SerializeField] private Point activePoint;

    public Point StartPoint {get {return startPoint;}}
    public Point ActivePoint {get {return activePoint;} set {activePoint = value;}}

    public Path(Vector2 center) {
        Point p1 = new Point(null, null, Vector3.left, (Vector3.up+Vector3.left)*0.5f);
        Point p2 = new Point(null, null, Vector3.right, (Vector3.right+Vector3.up)*0.5f);
        p1.next.Add(p2);
        p2.prev.Add(p1);
        startPoint = p1;
        activePoint = p2;
    }

    public List<Point> GetAllPoints(Point start) {
        List<Point> points = new List<Point>();
        if (start == null) {
            return points;
        }
        points.Add(start);
        foreach(Point p in start.next) {
            points.AddRange(GetAllPoints(p));
        }
        return points;
    }

    public void AddPoint(Vector3 pos) {
        Point p = new Point(activePoint, null, pos, (activePoint.controlPoint2 + pos)*0.5f);
        activePoint.next.Add(p);
        activePoint = p;
    }

    public void RemovePoint(Point p) {
        // Clear prev and next nodes
        foreach (Point point in p.prev) {
            point.next.Remove(p);
        }
        foreach (Point point in p.next) {
            point.prev.Remove(p);
        }

        // If the point was connected to ONLY 1 previous and 1 subsequent segment, link them
        if (p.prev.Count == 1 && p.next.Count == 1) {
            // Avoid dupes
            if (!p.prev[0].next.Contains(p.next[0])) {
                p.prev[0].next.Add(p.next[0]);
                p.next[0].prev.Add(p.prev[0]);
            }
        }
        // Delete node
        p.prev.Clear();
        p.next.Clear();
        p = null;
    }

    // TODO: Muuta rekursiiviseksi...
    public Vector2[] CalculateEvenlySpacedPoints(float spacing, float resolution = 1) {
        List<Vector2> evenlySpacedPoints = new List<Vector2>();
        evenlySpacedPoints.Add(startPoint.position);
        Vector2 previousPoint = evenlySpacedPoints[0];
        float dstSinceLastEvenPoint = 0;
        foreach (Point p in GetAllPoints(startPoint)) {
            foreach (Point poi in p.next) {
                if (p.bezier) {
                    float controlNetWeight = Vector2.Distance(p.position, p.controlPoint1) + Vector2.Distance(p.controlPoint1, poi.position)
                        + Vector2.Distance(poi.position, poi.controlPoint1); 
                    float estimatedCurveLenght = Vector2.Distance(p.position,poi.position) + controlNetWeight * 0.5f;
                    int divisions = Mathf.CeilToInt(estimatedCurveLenght * resolution * 10);
                    float t = 0;
                    while (t <= 1) {
                        t += 1f/divisions;
                        Vector2 pointOnCurve = Bezier.EvaluateCubic(p.position, p.controlPoint2, poi.controlPoint1, poi.position, t);
                        dstSinceLastEvenPoint += Vector2.Distance(previousPoint, pointOnCurve);
                        while (dstSinceLastEvenPoint >= spacing) {
                            float overShootDst = dstSinceLastEvenPoint - spacing;
                            Vector2 newEvenlySpacedPoint = pointOnCurve + (previousPoint-pointOnCurve).normalized * overShootDst;
                            evenlySpacedPoints.Add(newEvenlySpacedPoint);
                            dstSinceLastEvenPoint = overShootDst;
                            previousPoint = newEvenlySpacedPoint;
                        }
                        previousPoint = pointOnCurve;
                    }
                }
                // TODO: Linear line calc here
                else {

                }
            }
        }
        return evenlySpacedPoints.ToArray();
    }

    public EvaluationPoint[] CalculateMeshPointsRecursively(float spacing = 0.1f, float resolution = 1) {
        List<EvaluationPoint> evaluationPoints = new List<EvaluationPoint>();
        //evaluationPoints.Add(new EvaluationPoint())
        return null;
    }

    public class Point {
        public Vector3 position, controlPoint1 = Vector3.zero, controlPoint2 = Vector3.zero;
        public List<Point> prev = new List<Point>();
        public List<Point> next = new List<Point>();
        public bool bezier = true;
        public Point (Point prev, Point next, Vector3 pos, Vector3 control) {
            if (prev != null)
                this.prev.Add(prev);
            if (next != null)
                this.next.Add(next);
            this.position = pos;
            this.controlPoint1 = control;
            this.controlPoint2 = this.position + (this.position - this.controlPoint1);
        }

        public void MovePosition(Vector3 newPos, int point) {
            // Point determines wether the new point is going to be the point itself or its anchor points
            Vector3 delta = newPos - position;
            switch(point) {
                case 0: {
                    float dst = (position - controlPoint2).magnitude;
                    Vector3 dir = (position - newPos).normalized;
                    controlPoint1 = newPos;
                    controlPoint2 = position + dir * dst;
                    break;
                }
                case 1: {
                    position = newPos;
                    controlPoint1 += delta;
                    controlPoint2 += delta;
                    break;
                }
                case 2: {
                    float dst = (position - controlPoint1).magnitude;
                    Vector3 dir = (position - newPos).normalized;
                    controlPoint2 = newPos;
                    controlPoint1 = position + dir * dst;
                    break;
                }
            }
        }
    }

    public class EvaluationPoint {
        public Vector3 position;
        public List<EvaluationPoint> next;
        public EvaluationPoint(Vector3 position, EvaluationPoint next) {
            this.position = position;
            this.next.Add(next);
        }
        public EvaluationPoint(Vector3 position, List<EvaluationPoint> next) {
            this.position = position;

        }
    }
}
