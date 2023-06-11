using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class edge
{
    public int a,b;
    public edge e=null;
    public tria origin;
    public void advance(HashSet<edge>edges)
    {
        if(edges.Contains(this.twin(edges)))
        {
            edges.Remove(this.twin(edges));
        }
        else
        {
            edges.Add(this);
        }
    }

    public edge(int a, int b,tria tr)
    {
        this.a = a;
        this.b = b;
        origin = tr;
    }

    public edge(int a, int b)
    {
        this.a = a;
        this.b = b;
    }
    public edge twin(HashSet<edge>edges)
    {
        if(e==null)
            edges.TryGetValue(new edge(this.b,this.a),out e);
        return e;
    }
    public override bool Equals(object obj) 
    { 
        if (obj is edge edge)
        {
            return (this.a == edge.a) && (this.b == edge.b);
        }
        return false;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(a, b);
    }
}
