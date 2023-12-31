using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines; 
using Unity.Mathematics; 
using UnityEditor; 

[ExecuteInEditMode()]
public class SplineSampler : MonoBehaviour
{
    [SerializeField] private SplineContainer m_splineContainer; 
    [SerializeField] private int m_splineIndex; 
    [SerializeField] private float m_width;
    public int NumSplines; 

    float3 position; 
    float3 tangent; 
    float3 upVector; 

    float3 p1, p2; 

    private void Update()
    {  
        NumSplines = m_splineContainer.Splines.Count;
    }
    
    //função que retorna a posição do ponto t na spline
    public void Sample(float t, out Vector3 p1)
    {
        m_splineContainer.Evaluate(m_splineIndex, t, out position, out tangent, out upVector);
        p1 = position; 
    }

    //função que define a largura da pista
    public void SampleSplineWidth(int index, float t, out Vector3 p1, out Vector3 p2)
    {
        m_splineContainer.Evaluate(index, t, out position, out tangent, out upVector);

        float3 right = Vector3.Cross(tangent, upVector).normalized; 
        p1 = position + (right * m_width);
        p2 = position + (-right  * m_width);
    }
    
    public float ClosestKnot(float t)
    {
        m_splineContainer.Evaluate(m_splineIndex, t, out position, out tangent, out upVector); 

        float closest = 10000f; 

        for(int i = 0; i < m_splineContainer.Spline.Count; i++)
        {
            Vector3 pos = position; 
            if(Vector3.Distance((Vector3)position-transform.position, m_splineContainer.Spline[i].Position ) < closest)
            {
                closest = Vector3.Distance((Vector3)position-transform.position, m_splineContainer.Spline[i].Position);
            };

        }
        return closest; 
    }

    public SplineContainer GetContainer()
    {
        return m_splineContainer;
    }
}
