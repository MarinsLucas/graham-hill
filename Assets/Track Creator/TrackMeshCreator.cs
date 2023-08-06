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

    //Funções não utilizadas mais
    /* private void GetVerts()
    {
        m_vertsP1 = new List<Vector3>(); 
        m_vertsP2 = new List<Vector3>(); 

        float step = 1f/(float) resolution ;
    
        for(int i = 0; i<resolution ; i++)
        {
            float t = step*i; 
            m_splineSampler.SampleSplineWidth(t, out Vector3 p1, out Vector3 p2);
            m_vertsP1.Add(p1);
            m_vertsP2.Add(p2);
        }
        vertsCount = m_vertsP1.Count + m_vertsP2.Count; 
    }

    private void GetVerts1()
    {
        m_vertsP1 = new List<Vector3>(); 
        m_vertsP2 = new List<Vector3>(); 

        float partition = 1f;
        if(track_repartions != 0)
        partition = 1f/track_repartions;
        float k = 0f; 

        while(k <= 1f + partition)
        {
            Vector3 aux_p1, aux_p2, aux_p3;
            m_splineSampler.Sample(k, out aux_p1);
            m_splineSampler.Sample(k+partition/2f, out aux_p2);
            m_splineSampler.Sample(k+partition, out aux_p3);

            aux_p2 = aux_p2 - aux_p1;
            aux_p3 = aux_p3 - aux_p1;
            float cos = Vector3.Angle(aux_p2, aux_p3); 
            //float cos = Mathf.Cos(Vector3.Angle(aux_p2, aux_p3)* Mathf.PI/180);
            if(cos < 15f) cos = 0.2f; 
            else if(cos < 50f) cos = 1f; 
            else cos = 1f; 

            float step = 1f/(float) (resolution*cos);
            float t = k; 

            for(int i = 0; t< k + partition; i++)
            {
                t += step; 
                m_splineSampler.SampleSplineWidth(t, out Vector3 p1, out Vector3 p2);
                m_vertsP1.Add(p1);
                m_vertsP2.Add(p2);
            }

            k += partition; 
        }
                vertsCount = m_vertsP1.Count + m_vertsP2.Count; 

    } */

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
                float step = max_step* (m_splineSampler.ClosestKnot(t)/250f);
                
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

    private void BuildMesh()
    {
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        int offset = 0; 

        int vertsAcumulator = 0; 
        int splineOffset = 0;
         for(int currentSplineIndex = 0; currentSplineIndex < m_splineSampler.NumSplines; currentSplineIndex++)
        {
            splineOffset = vertsAcumulator;
            splineOffset += currentSplineIndex;

            for(int currentSplinePoint = 1; currentSplinePoint < vertsCountBySpline[currentSplineIndex]-1; currentSplinePoint++)
            {
                int vertoffset = splineOffset + currentSplinePoint;

                Vector3 p1 = m_vertsP1[vertoffset -1] - transform.position; 
                Vector3 p2 = m_vertsP2[vertoffset -1] - transform.position; 
                Vector3 p3 = m_vertsP1[vertoffset] - transform.position; 
                Vector3 p4 = m_vertsP2[vertoffset] - transform.position; 

                p1 = ConcatenateWithTerrain(p1);
                p2 = ConcatenateWithTerrain(p2);
                p3 = ConcatenateWithTerrain(p3);
                p4 = ConcatenateWithTerrain(p4); 

                int t1 = offset + 0; 
                int t2 = offset + 2; 
                int t3 = offset + 3; 

                int t4 = offset + 3; 
                int t5 = offset + 1; 
                int t6 = offset + 0; 

                offset += 4; 

                verts.AddRange(new List<Vector3> {p1, p2, p3, p4});
                tris.AddRange(new List<int> {t1, t2, t3, t4, t5, t6});
            }

            vertsAcumulator += vertsCountBySpline[currentSplineIndex];

        }
         
        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        m_meshFilter.mesh = m;
    }

    /* for(int i = 1; i <= length; i++)
        {
            Vector3 p1 = m_vertsP1[i-1] - transform.position;
            Vector3 p2 = m_vertsP2[i-1] - transform.position;
            Vector3 p3, p4; 

            if(i == length)
            {
                p3 = m_vertsP1[0] - transform.position;
                p4 = m_vertsP2[0] - transform.position;
            }else
            {
                p3 = m_vertsP1[i] - transform.position;
                p4 = m_vertsP2[i] - transform.position;
            }         
            
            offset = 4*(i-1);

            p1 = ConcatenateWithTerrain(p1);
            p2 = ConcatenateWithTerrain(p2);
            p3 = ConcatenateWithTerrain(p3);
            p4 = ConcatenateWithTerrain(p4);

            int t1 = offset + 0; 
            int t2 = offset + 2; 
            int t3 = offset + 3; 

            int t4 = offset + 3; 
            int t5 = offset + 1; 
            int t6 = offset + 0; 

            verts.AddRange(new List<Vector3> {p1, p2, p3, p4});
            tris.AddRange(new List<int> {t1, t2, t3, t4, t5, t6});
        } 
 */
    private void OnDrawGizmos()
    {
        if(drawnGizmos)
        {
            
            for(int i = 0; i < m_vertsP1.Count; i++)
            {
                Handles.SphereHandleCap(0, m_vertsP1[i], Quaternion.identity, 0.5f, EventType.Repaint);
                Handles.SphereHandleCap(0, m_vertsP2[i], Quaternion.identity, 0.5f, EventType.Repaint);
            }
        
        }
    } 

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
