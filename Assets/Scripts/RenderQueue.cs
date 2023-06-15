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
        /*for(var i = 0; i < _renderers.Count; i++)
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
        }*/
        _renderers.Sort((a, b) => b.OperationType.CompareTo(a.OperationType));
        var list = new List<RaymarchingRenderer>();
        for (var i = 0; i < _renderers.Count; i++)
        {
            if(_renderers[i].Transform.parent == null)
            {
                list.Add(_renderers[i]);
                for(var j = 0; j < _renderers.Count; j++)
                {
                    if(_renderers[j].Transform.parent == _renderers[i].Transform)
                    {
                        list.Add(_renderers[j]);
                    }
                }
            }
        }
        _renderers = list;

            RaymarchingRenderer FindParent(Transform transform)
        {
            var parent = transform.parent;
            if (parent == null)
            {
                var renderer = transform.GetComponent<RaymarchingRenderer>();
                return renderer == null ? null : renderer;
            }
            var parentParent = FindParent(parent);
            if (parentParent == null)
            {
                var renderer = transform.GetComponent<RaymarchingRenderer>();
                return renderer == null ? null : renderer;
            }
            if(parentParent == parent)
            {
                return null;
            }
            return parentParent;
        }
    }
}