using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "AeroData/AirFoil", order = 1)]
public class AirFoil : ScriptableObject
{
    public float minAlpha;
    public float maxAlpha;
    public List<Vector3> data = new List<Vector3>();

    public void initialize()
    {
        minAlpha = data[0].x;
        maxAlpha = data[data.Count - 1].x;
    }
    public (float lift, float drag) sample(float alpha)
    {
        int maxIdx = data.Count - 1;
        float t = Mathf.InverseLerp(minAlpha, maxAlpha, alpha) * maxIdx;
        float integer = Mathf.Floor(t);
        float fractional = t-integer;
        int index = (int)integer;
        var value = (index < maxIdx) ? Vector3.Lerp(data[index], data[index+1],fractional) : data[maxIdx];
        return (value.y, value.z);
    }
}
