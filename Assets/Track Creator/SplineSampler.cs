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
    [SerializeField] private bool debug; 
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
    
    public void SampleDoubleSpline(int index, float t, out Vector3 p1, out Vector3 p2)
    {
        int mainCurve = shortestCurve();
        //Pontos da curva menor (pontos espalhados normalmente)
        m_splineContainer.Evaluate(mainCurve, t, out position, out tangent, out upVector);
        p1 = position;

        if(debug)
            Debug.DrawRay(p1, -Vector3.Cross(tangent, upVector).normalized*100f, Color.red);

        m_splineContainer.Evaluate(index + 1, t, out position, out tangent, out upVector);
        p2 = position; 
        /* m_splineContainer.Evaluate(mainCurve+1, oppositePoint(p1, -Vector3.Cross(tangent, upVector).normalized, mainCurve+1, t), out position, out tangent, out upVector);
        p2 = position;  */
    }


    public float oppositePoint(Vector3 p1, Vector3 ortogonal, int index, float t)
    {   
        /* float tMin = 0;
        float tMax = 1; 
        
        int maxitt = 10000;  
        
        float3 pontoAtual;
        for(int i = 0; i < maxitt; i++)
        {
            float tMid= (tMin + tMax)*0.5f;
            m_splineContainer.Evaluate(index, tMid, out position, out tangent, out upVector);
            Vector3 direcaoAtual = position - (float3)p1;
            float dot = Vector3.Dot(direcaoAtual, ortogonal);
            
            if(dot > 0.7f)
                return tMid;
            
            if(dot < 0)
                tMin = tMid;
            else
                tMax = tMid; 
        }
        return (tMin + tMax)*0.5f; */ 

        float maxDot = -1.0f;
        float tfinal = 0.0f;
        for(float i = 0; i<1.0f; i+= 0.01f)
        {
            m_splineContainer.Evaluate(index, i, out position, out tangent, out upVector);
            float distance = Vector3.Dot(ortogonal, new float3(p1.x, p1.y, p1.z)-position);

            if(distance > maxDot)
            {
                maxDot = distance;
                tfinal = i; 
            }
        }
        return tfinal; 
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

    public int shortestCurve()
    { 
        int shortestIndex = m_splineContainer.CalculateLength(0) > m_splineContainer.CalculateLength(1) ? 1 : 0;  
        return shortestIndex;
    }
}
