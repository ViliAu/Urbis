using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour {

    [HideInInspector] public Path path;

    [Range(3, 30)]public int segmentsPerLine = 10; 

    public bool drawAbsolutePath = true;
    public bool drawEvaluationPath = false;
    public bool drawHandles = true;
    public Color absolutePathColor = Color.green;
    public Color evaluationPathColor = Color.white;
    public Color bezierHandleColor = Color.red;
    public Color activeHandleColor = Color.yellow;

    public void CreatePath() {
        path = new Path(transform.position);
    }

    private void OnDrawGizmosSelected() {
        if (drawEvaluationPath) {
            Gizmos.color = evaluationPathColor;
            foreach(Path.Point p in path.GetAllPoints(path.StartPoint)) {
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