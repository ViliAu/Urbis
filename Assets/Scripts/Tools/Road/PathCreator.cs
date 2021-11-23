using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour {

    [HideInInspector] public Path path = null;

    [Tooltip("How accurate the estimation will be")]
    [Range(3, 30)]public int segmentsPerLine = 10;

    [Tooltip("How far apart the road vertices will be")]
    [Range(0.05f, 1f)]public float spacing = 0.1f; 

    public bool drawAbsolutePath = true;
    public bool drawEvaluationPath = false;
    public bool drawHandles = true;
    public bool unityJuttu = false;
    public Color absolutePathColor = Color.green;
    public Color evaluationPathColor = Color.white;
    public Color bezierHandleColor = Color.red;
    public Color activeHandleColor = Color.yellow;

    public void CreatePath() {
        path = new Path(transform.position);
    }

    private void OnDrawGizmosSelected() {
        List<Path.Point> points = new List<Path.Point>();
        path.GetAllPoints(path.StartPoint, ref points);
        if (drawEvaluationPath) {
            Gizmos.color = evaluationPathColor;
            foreach(Path.Point p in points) {
                if (p.bezierPoints != null && p.bezierPoints.Count > 0) {
                    foreach (Vector3[] v in p.bezierPoints) {
                        for (int i = 0; i < v.Length-1; i++) {
                            Gizmos.DrawLine(v[i], v[i+1]);
                        }
                    }
                }
            }
        }
    }
}