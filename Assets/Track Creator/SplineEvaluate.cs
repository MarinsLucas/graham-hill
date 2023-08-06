using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; 
using UnityEngine.Splines; 

public class SplineEvaluate : MonoBehaviour
{
    public bool IsScaled;

    //Função para ler informações sobre curva
    public bool Evaluate<T>(T spline, float t, out float3 position, out float3 tangent, out float3 upVector) where T : ISpline
    {
        if(spline == null)
        {
            position = float3.zero; 
            tangent = new float3(0, 0, 1);
            upVector = new float3(0, 1, 1);
        }

        if(IsScaled)
        {
            using var nativeSpline = new NativeSpline(spline, transform.localToWorldMatrix);
            return SplineUtility.Evaluate(nativeSpline, t, out position, out tangent, out upVector);
        }

        var evaluationStatus = SplineUtility.Evaluate(spline, t, out position, out tangent, out upVector);
        if(evaluationStatus)
        {
            position = transform.TransformPoint(position);
            tangent = transform.TransformPoint(tangent);
            upVector = transform.TransformPoint(upVector);
        }
        return evaluationStatus; 
    } 
}
