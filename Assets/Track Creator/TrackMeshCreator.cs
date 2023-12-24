using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines; 
using Unity.Mathematics; 
using UnityEditor; 
using System.Linq; 

[ExecuteInEditMode()]
public class TrackMeshCreator : MonoBehaviour
{
    [SerializeField] private int resolution; 
    
    private List<Vector3> m_vertsP1;
    private List<Vector3> m_vertsP2;
    private SplineSampler m_splineSampler;
    [SerializeField]private MeshFilter m_meshFilter; 

    [SerializeField] private float concatenate_max_distance;

    private int[] vertsCountBySpline;

    [Header("Debug")]
    public bool drawnGizmos; 
    public int vertsCount; 

    private void Start()
    {
        m_splineSampler = this.transform.GetComponent<SplineSampler>(); 
    }

    private void Update()
    {
        GetVerts2(); 
        BuildMesh();  
    }

    private void GetVerts2()
    {        
        m_vertsP1 = new List<Vector3>(); 
        m_vertsP2 = new List<Vector3>(); 
        vertsCountBySpline = new int[m_splineSampler.NumSplines];

        float min_step = 1f/(float) resolution ;
        float max_step = 1f/(float)(resolution/20);

        Vector3 p1, p2; 

        float t = 0; 
        for(int j = 0; j < m_splineSampler.NumSplines; j++)
        {
            vertsCountBySpline[j] = 0; 
            for(int i = 0; t<1f ; i++)
            {
                //Aqui que fica o critério de espaçamento entre nós
                float step = max_step* (m_splineSampler.ClosestKnot(t)/resolution);
                
                t += (step < min_step ? min_step : (step>max_step?max_step:step)); 

                m_splineSampler.SampleSplineWidth(j, t, out p1, out p2);
                m_vertsP1.Add(p1);
                m_vertsP2.Add(p2);
                vertsCountBySpline[j] += 1; 
            }

            m_splineSampler.SampleSplineWidth(j, 1f, out p1, out p2);
            m_vertsP1.Add(p1);
            m_vertsP2.Add(p2);
            t = 0;
            vertsCountBySpline[j] += 1; 
        }
        vertsCount = m_vertsP1.Count + m_vertsP2.Count; 

    }

    //Função que cria a malha
    private void BuildMesh()
    {
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        int offset = 0; 

        int vertsAcumulator = 0; 
        int splineOffset = 0;
        
        //Esse loop percorre todos as splines do m_splineSampler
         for(int currentSplineIndex = 0; currentSplineIndex < m_splineSampler.NumSplines; currentSplineIndex++)
        {
            splineOffset = vertsAcumulator;
            //splineOffset += currentSplineIndex;

            //Esse loop percorre cada ponto 
            for(int currentSplinePoint = 1; currentSplinePoint < vertsCountBySpline[currentSplineIndex]-1; currentSplinePoint++)
            {
                int vertoffset = splineOffset + currentSplinePoint;

                //Vertices do triangulo 
                Vector3 p1 = m_vertsP1[vertoffset -1] - transform.position; 
                Vector3 p2 = m_vertsP2[vertoffset -1] - transform.position; 
                Vector3 p3 = m_vertsP1[vertoffset] - transform.position; 
                Vector3 p4 = m_vertsP2[vertoffset] - transform.position; 

                //Concatena a malha com o terreno
                p1 = ConcatenateWithTerrain(p1);
                p2 = ConcatenateWithTerrain(p2);
                p3 = ConcatenateWithTerrain(p3);
                p4 = ConcatenateWithTerrain(p4); 

                //Triangulo 1
                int t1 = offset + 0; 
                int t2 = offset + 2; 
                int t3 = offset + 3; 
                
                //Triangulo 2
                int t4 = offset + 3; 
                int t5 = offset + 1; 
                int t6 = offset + 0; 

                offset += 4; 
                
                //Adiciona os vértices e os triangulos nas respectivas listas
                verts.AddRange(new List<Vector3> {p1, p2, p3, p4});
                tris.AddRange(new List<int> {t1, t2, t3, t4, t5, t6});
            }
            
            //Se a spline for fechada, eu pego o último ponto da spline e o primeiro ponto da spline e junto
            if (m_splineSampler.GetContainer()[currentSplineIndex].Closed)
            {
                int vertoffset = splineOffset + vertsCountBySpline[currentSplineIndex] - 1; // Último ponto da spline

                // Vértices do triângulo para fechar a malha
                Vector3 p1 = m_vertsP1[vertoffset] - transform.position; //ultimo
                Vector3 p2 = m_vertsP2[vertoffset] - transform.position; //ultimo
                Vector3 p3 = m_vertsP1[splineOffset] - transform.position; //primeiro
                Vector3 p4 = m_vertsP2[splineOffset] - transform.position; //primeiro

                // Concatena a malha com o terreno
                p1 = ConcatenateWithTerrain(p1);
                p2 = ConcatenateWithTerrain(p2);
                p3 = ConcatenateWithTerrain(p3);
                p4 = ConcatenateWithTerrain(p4);

                // Triângulo 1 para fechar a malha
                int t1 = offset + 0;
                int t2 = offset + 2;
                int t3 = offset + 3;

                // Triângulo 2 para fechar a malha
                int t4 = offset + 3;
                int t5 = offset + 1;
                int t6 = offset + 0;

                offset += 4;

                // Adiciona os vértices e os triângulos nas respectivas listas
                verts.AddRange(new List<Vector3> { p1, p2, p3, p4 });
                tris.AddRange(new List<int> { t1, t2, t3, t4, t5, t6 });
            }

            vertsAcumulator += vertsCountBySpline[currentSplineIndex];
        }

        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        m_meshFilter.mesh = m;
    }

    //Pinta os pontos da pista
    private void OnDrawGizmos()
    {
        if(drawnGizmos)
        {
            if(m_vertsP1 != null)
            for(int i = 0; i < m_vertsP1.Count; i++)
            {
                Handles.SphereHandleCap(0, m_vertsP1[i], Quaternion.identity, 0.5f, EventType.Repaint);
                Handles.SphereHandleCap(0, m_vertsP2[i], Quaternion.identity, 0.5f, EventType.Repaint);
            }
        
        }
    } 

    //Função que concatena os pontos da malha da pista ao terreno abaixo. 
    //concatenate_max_distance é o valor máximo de deslocamento do ponto da pista para a interpolação com o terreno. 
    private Vector3 ConcatenateWithTerrain(Vector3 pos)
    {
        RaycastHit hit; 
        int layerMask = 1 << 3;
        if(Physics.Raycast(pos + transform.position + Vector3.up*concatenate_max_distance, transform.TransformDirection(-Vector3.up), out hit, concatenate_max_distance, layerMask))
        {
            //Debug.DrawRay(pos + transform.position + Vector3.up*concatenate_max_distance, transform.TransformDirection(-Vector3.up)*hit.distance, Color.yellow);
            pos.y += concatenate_max_distance - hit.distance + 0.01f; 
        }
        else if(Physics.Raycast(pos + transform.position, transform.TransformDirection(-Vector3.up), out hit, concatenate_max_distance, layerMask))
        {
            //Debug.DrawRay(pos + transform.position, transform.TransformDirection(-Vector3.up)*hit.distance, Color.yellow);
            pos.y -= hit.distance + 0.01f; 
        }
        
        pos.y += 0.1f; 


        return pos; 
    }

}
