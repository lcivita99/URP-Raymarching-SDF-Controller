// using UnityEngine;
// using UnityEditor;
// using System;
// using State_Machines.AniMUH;
//
// [InitializeOnLoad]
// public class EditorFunctionRunner
// {
//     private const float interval = 0.1f;
//     private static DateTime lastExecutionTime;
//     
//     private static bool isEnabled = true;
//
//     // public static StateManager animuhManager;
//
//     static EditorFunctionRunner()
//     {
//         EditorApplication.update += OnUpdate;
//         lastExecutionTime = DateTime.Now;
//     }
//
//     static void OnUpdate()
//     {
//         if (!isEnabled) return;
//         try
//         {
//             if ((DateTime.Now - lastExecutionTime).TotalSeconds >= interval)
//             {
//                 GameObject.Find("AniMUH Controller").GetComponent<StateManager>().SetUniforms();
//                 lastExecutionTime = DateTime.Now;
//             }
//         }
//         catch
//         {
//             
//         }
//         
//     }
//     
//     // Method to toggle the script on/off
//     [MenuItem("Custom/Enable Function Runner")]
//     private static void EnableFunctionRunner()
//     {
//         isEnabled = true;
//     }
//
//     [MenuItem("Custom/Disable Function Runner")]
//     private static void DisableFunctionRunner()
//     {
//         isEnabled = false;
//     }
// }