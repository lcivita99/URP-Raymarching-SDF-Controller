using System;
using System.Collections;
using System.Collections.Generic;
using State_Machines.AniMUH;
using UnityEngine;

public static class Utils
{
    public static float minimumInputValue = 0.1f;

    public static float Map(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    
    public static float Smootherstep(this float value, float edge0, float edge1)
    {
        // Scale, and clamp x to 0..1 range
        value = Mathf.Clamp01((value - edge0) / (edge1 - edge0));
        // Evaluate polynomial
        return value * value * value * (value * (value * 6 - 15) + 10);
    }

}

[Serializable]
public class SerializableTuple<T1, T2>
{
    [SerializeField]
    private T1 item1;
    [SerializeField]
    private T2 item2;

    public T1 Item1 { get { return item1; } set { item1 = value; } }
    public T2 Item2 { get { return item2; } set { item2 = value; } }

    public SerializableTuple(T1 item1, T2 item2)
    {
        this.item1 = item1;
        this.item2 = item2;
    }
}