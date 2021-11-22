using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class RoadData {
    [SerializeField] private Point startPoint;
    [SerializeField] private Point activePoint;

    public Point StartPoint {get {return startPoint;}}
    public Point ActivePoint {get {return startPoint;} set {activePoint = value;}}

    public RoadData(Vector2 center) {
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

    //[System.Serializable]
    public class Point {
        public Vector3 position, controlPoint1 = Vector3.zero, controlPoint2 = Vector3.zero;
        public List<Point> prev = new List<Point>();
        public List<Point> next = new List<Point>();
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

        // Helper functions to see if anchors exist
        public bool Control1() {
            return controlPoint1 != Vector3.zero;
        }
        public bool Control2() {
            return controlPoint2 != Vector3.zero;
        }
    }

}
