using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class DrawFieldOfView : MonoBehaviour
{

    [SerializeField]
    private EnemyFieldOfView FoV;
    [SerializeField]
    private float transparencyValue = 0.5f; // 0 = completely transparent; 1 = completely opaque
    [SerializeField]
    private GameObject visionArc;
    [SerializeField]
    private GameObject lineObject;
    [SerializeField]
    private LineRenderer lineRenderer;

    void Start()
    {
        FoV = GetComponent<EnemyFieldOfView>();

        if (FoV == null)
        {
            return;
        }

        Material transparentMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        
        transparentMat.SetFloat("_Surface", 1.0f);
        transparentMat.SetFloat("_DstBlendAlpha", 10f);
        transparentMat.SetFloat("_DstBlend", 10f);
        transparentMat.color = new Color(1, 1, 1, transparencyValue);

        visionArc = new GameObject("FoV_Arc");
        visionArc.transform.position = FoV.transform.position;
        MeshFilter mFilter = visionArc.AddComponent<MeshFilter>();
        MeshRenderer mRenderer = visionArc.AddComponent<MeshRenderer>();
        mRenderer.material = transparentMat;

        Mesh filledArc = new Mesh();
        BuildArcMesh(filledArc, FoV.GetViewAngle(), FoV.GetViewRadius(), 100);
        mFilter.mesh = filledArc;

        visionArc.layer = LayerMask.NameToLayer("Ignore Raycast");


        Material redMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        redMat.color = new Color(1, 0, 0, 1);

        lineObject = new GameObject("LineObject");
        lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = redMat;
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
        
    }

    void Update()
    {
        visionArc.transform.position = FoV.transform.position;
        visionArc.transform.rotation = FoV.transform.rotation;
        if(FoV.GetVisibleTarget() != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, FoV.transform.position);
            lineRenderer.SetPosition(1, FoV.GetVisibleTarget().position);
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
