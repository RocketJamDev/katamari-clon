using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private double _volume;
    private double _equivalentSphere;
    private Bounds bounds;

    public double Volume {
        get
        {
            return this._volume;

        }
        set
        {
            this._volume = value;
        }
    }

    private void Start()
    {
        Vector3 size = GetMaxBounds().size;
        _volume = size.x * size.y * size.z;
    }

    Bounds GetMaxBounds()
    {
        var b = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            b.Encapsulate(r.bounds);
        }
        return b;
    }
}
