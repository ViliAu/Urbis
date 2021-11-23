 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PathCreator))]
public class RoadCreator : MonoBehaviour {

    [Range(.05f, 1.5f)]
    public float spacing = 1;
    public float roadWidth = 0.5f;
    public bool autoUpdate;
    public float tiling = 1;

    List<Vector3> vList = new List<Vector3>();

    public void UpdateRoad() {
        Path path = GetComponent<PathCreator>().path;
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Path.Point> usedPoints = new List<Path.Point>();
        CreateRoadMeshData(path.StartPoint, ref usedPoints, ref verts, ref tris);
        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void CreateRoadMeshData(Path.Point start, ref List<Path.Point> usedPoints, ref List<Vector3> verts, ref List<int> tris) {
        // In this case we've come to an end of the path branch
        if (start == null || usedPoints.Contains(start)) {
            for (int i = 0; i < 6; i++) {
                tris.RemoveAt(tris.Count-1);
            }
            return;
        }
        usedPoints.Add(start);
        for(int i = 0; i < start.next.Count; i++) {
            CalculateVerticesByPoint(start.bezierPoints[i], ref verts, ref tris);
            CreateRoadMeshData(start.next[i], ref usedPoints, ref verts, ref tris);
        }
        if (tris.Count > 6) {
            for (int i = 0; i < 6; i++) {
                tris.RemoveAt(tris.Count-1);
            }
        }
    }

    private void CalculateVerticesByPoint(Vector3[] points, ref List<Vector3> vertices, ref List<int> tris) {
        for(int i = 0; i < points.Length; i++) {
            Vector3 forward = Vector2.zero;
            if (i < points.Length - 1) {
                forward += points[(i + 1) % points.Length] - points[i];
            }
            if (i > 0) {
                forward += points[i] - points[(i - 1 + points.Length)%points.Length];
            }

            forward.Normalize();
            Vector3 left = new Vector3(-forward.y, forward.x);

            // Left vertex
            vertices.Add(points[i] + left * roadWidth * .5f);

            // Right vertex
            vertices.Add(points[i] - left * roadWidth * .5f);

            if (i < points.Length - 1) {
                tris.Add(vertices.Count-2);
                tris.Add(vertices.Count-2 + 2);
                tris.Add(vertices.Count-2 + 1);

                tris.Add(vertices.Count-2 + 1);
                tris.Add(vertices.Count-2 + 2);
                tris.Add(vertices.Count-2 + 3);
            }
            
        
        // Connect to next piece
        tris.Add(vertices.Count-2);
        tris.Add(vertices.Count-2 + 2);
        tris.Add(vertices.Count-2 + 1);

        tris.Add(vertices.Count-2 + 1);
        tris.Add(vertices.Count-2 + 2);
        tris.Add(vertices.Count-2 + 3);
        }
    }

    /*
    public void UpdateRoad() {
        Path path = GetComponent<PathCreator>().path;
        List<Vector2> pointsList = new List<Vector2>();
        path.GetAllEvalPoints(ref pointsList);
        Vector2[] points = pointsList.ToArray();
        GetComponent<MeshFilter>().mesh = CreateRoadMesh(points);

        int textureRepeat = Mathf.RoundToInt(tiling * points.Length * spacing * .05f);
        GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(1, textureRepeat);
    }

    Mesh CreateRoadMesh(Vector2[] points)
    {
        Vector3[] verts = new Vector3[points.Length * 2];
        Vector2[] uvs = new Vector2[verts.Length];
        int numTris = 2 * (points.Length - 1);
        int[] tris = new int[2 * (points.Length - 1) * 3];
        int vertIndex = 0;
        int triIndex = 0;

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 forward = Vector2.zero;
            if (i < points.Length - 1)
            {
                forward += points[(i + 1)%points.Length] - points[i];
            }
            if (i > 0)
            {
                forward += points[i] - points[(i - 1 + points.Length)%points.Length];
            }

            forward.Normalize();
            Vector2 left = new Vector2(-forward.y, forward.x);

            verts[vertIndex] = points[i] + left * roadWidth * .5f;
            verts[vertIndex + 1] = points[i] - left * roadWidth * .5f;

            float completionPercent = i / (float)(points.Length - 1);
            float v = 1 - Mathf.Abs(2 * completionPercent - 1);
            uvs[vertIndex] = new Vector2(0, v);
            uvs[vertIndex + 1] = new Vector2(1, v);

            if (i < points.Length - 1)
            {
				tris[triIndex] = vertIndex;
                tris[triIndex + 1] = vertIndex + 2;
				tris[triIndex + 2] = vertIndex + 1;

				tris[triIndex + 3] = vertIndex + 1;
                tris[triIndex + 4] = vertIndex + 2;
                tris[triIndex + 5] = vertIndex + 3;
            }

            vertIndex += 2;
            triIndex += 6;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;

        return mesh;
    }*/
}
