using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStats", menuName = "Scriptable Objects/MainMenu/AniMuhSO")]
public class AniMUHSO : ScriptableObject
{
    
    [Header("Move")] public float minMoveSpeed;
    public float maxMoveSpeed;
    public float movementDragStrength;

    [Header("Jump")] public float jumpForce;
    public float jumpMinMoveSpeed, jumpMaxMoveSpeed, jumpDragStrength;

    [Header("Physics")] [Range(0.0f, 3.0f)] public float gravScale;
    [Range(0.0f, 10.0f)] public float weight;
}