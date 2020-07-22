using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableContainer", menuName = "BMFrameWork/Scriptable/Container", order = 1)]
public class ScriptableContainer : ScriptableObject
{
    [SerializeField] private List<ScriptableReference> scriptableReferences = new List<ScriptableReference>();

    public void Register()
    {
        for (int i = 0; i < scriptableReferences.Count; ++i)
        {
            scriptableReferences[i].Register();
        }
    }

    public void UnRegister()
    {
        for (int i = scriptableReferences.Count - 1; i >= 0; --i)
        {
            scriptableReferences[i].Unregister();
        }
    }

    public void DoUpdate(float deltaTime)
    {
        foreach (ScriptableReference scriptableReference in scriptableReferences)
        {
            scriptableReference.DoUpdate(deltaTime);
        }
    }

    public void DoUpdate(int deltaTime)
    {
        foreach (ScriptableReference scriptableReference in scriptableReferences)
        {
            scriptableReference.DoUpdate(deltaTime);
        }
    }
}