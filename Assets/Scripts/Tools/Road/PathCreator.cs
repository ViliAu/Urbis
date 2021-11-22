using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour {

    [HideInInspector]
    public Path path;

    public bool drawEval = false;

    public void CreatePath() {
        path = new Path(transform.position);
    }

    private void OnDrawGizmosSelected() {
        float spacing = 0.1f;
        float resolution = 1;
        Vector2[] p = path.CalculateEvenlySpacedPoints(spacing, resolution);
        for(int i = 0; i < p.Length-1; i++) {
            Gizmos.DrawLine(p[i], p[i+1]);
        }
    }
}