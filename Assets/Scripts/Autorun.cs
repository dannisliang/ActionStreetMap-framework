using Mercraft.Scene.Builders;
using UnityEngine;
using UnityEditor;


namespace Mercraft.Scene
{
    [InitializeOnLoad]
    public class Autorun
    {
        static Autorun()
        {
            Debug.Log("Autorun!");
            EditorApplication.update += RunOnce;
        }

        private static void RunOnce()
        {
            Debug.Log("RunOnce!");
            EditorApplication.update -= RunOnce;
        }
    }
}