using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class DrawFieldOfView : MonoBehaviour
{
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float transparencyValue = 0.1f; // 0 = completely transparent; 1 = completely opaque
    [SerializeField]
    private float lineWidthStart = 0.25f;
    [SerializeField]
    private float lineWidthEnd = 0.25f;

    private EnemyFieldOfView FoV;
    private GameObject visionArc;
    private GameObject lineObject;
    private LineRenderer lineRenderer;
    [SerializeField]
    private Material fovMat;
    [SerializeField]
    private Material lineMat;

    void Start()
    {
        FoV = GetComponent<EnemyFieldOfView>();
        if (FoV == null)
        {
            Debug.LogError("FoV can't be null!");
            return;
        }
        if(fovMat == null)
        {
            Debug.LogError("TransparentMat can't be null!");
            return;
        }

        fovMat.color = new Color(1, 1, 1, transparencyValue);

        visionArc = new GameObject("FoV_Arc");
        visionArc.transform.position = new (FoV.transform.position.x, FoV.transform.position.y + 0.4385f, FoV.transform.position.z);
        MeshFilter mFilter = visionArc.AddComponent<MeshFilter>();
        MeshRenderer mRenderer = visionArc.AddComponent<MeshRenderer>();
        mRenderer.material = fovMat;



        Mesh filledArc = new Mesh();
        BuildArcMesh(filledArc, FoV.GetViewAngle(), FoV.GetViewRadius(), 100);
        mFilter.mesh = filledArc;

        visionArc.layer = LayerMask.NameToLayer("Ignore Raycast");

        lineObject = new GameObject("LineObject");
        lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMat;
        lineRenderer.startWidth = lineWidthStart;
        lineRenderer.endWidth = lineWidthEnd;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;



    }

    void LateUpdate()
    {
        visionArc.transform.position = new(FoV.transform.position.x, FoV.transform.position.y + 0.4385f, FoV.transform.position.z);
        visionArc.transform.rotation = FoV.transform.rotation;
        if(FoV.GetVisibleTarget() != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, new(FoV.transform.position.x, FoV.transform.position.y + 0.439f, FoV.transform.position.z));
            lineRenderer.SetPosition(1, new (FoV.GetVisibleTarget().position.x, FoV.transform.position.y + 0.439f, FoV.GetVisibleTarget().position.z));
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    void BuildArcMesh(Mesh mesh, float angle, float radius, int resolution)
    {
        Vector3[] vertices = new Vector3[resolution + 2];
        int[] triangles = new int[(resolution + 1) * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0, n = resolution + 1; i < n; i++)
        {
            float t = i / (float)resolution;
            float ang = t * angle - angle / 2;

            Vector3 dir = new Vector3(Mathf.Sin(ang * Mathf.Deg2Rad), 0, Mathf.Cos(ang * Mathf.Deg2Rad));
            vertices[i + 1] = dir * radius;

            if (i < resolution)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
