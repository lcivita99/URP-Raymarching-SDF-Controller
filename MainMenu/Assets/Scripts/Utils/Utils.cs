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