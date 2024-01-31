using System.Collections.Generic;
using UnityEngine;

public class RenderQueue : MonoBehaviour
{
    public static RaymarchingObject[] Objects
    {
        get
        {
            Sort();
            var objects = new RaymarchingObject[_renderers.Count];
            for (var i = 0; i < _renderers.Count; i++)
            {
                objects[i] = _renderers[i].Object;
            }
            return objects;
        }
    }
    private static List<RaymarchingRenderer> _renderers = new List<RaymarchingRenderer>();

    public static void AddToRender(RaymarchingRenderer renderer)
    {
        _renderers.Add(renderer);
        Sort();
    }

    public static void RemoveFromRender(RaymarchingRenderer renderer)
    {
        _renderers.Remove(renderer);
    }

    private static void Sort()
    {
        _renderers.Sort((a, b) => a.OperationType.CompareTo(b.OperationType));
        for (var i = 0; i < _renderers.Count; i++)
        {
            var parent = FindParent(_renderers[i].Transform);
            var index = _renderers.IndexOf(parent);
            if (index == -1)
            {
                continue;
            }
            var temp = _renderers[i];
            _renderers.RemoveAt(i);
            index = _renderers.IndexOf(parent);
            _renderers.Insert(index + 1, temp);
        }

        RaymarchingRenderer FindParent(Transform transform)
        {
            var parent = transform.parent;
            if (parent == null)
            {
                return null;
            }
            return parent.GetComponent<RaymarchingRenderer>();
        }
    }
}