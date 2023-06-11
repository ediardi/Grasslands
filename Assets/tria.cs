using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tria
{
    public int a;
    public int b;
    public int c;
    public edge edge1;
    public edge edge2;
    public edge edge3;
    public Vector3 cmc;
    public float rsq;

    public tria(int indexa, int indexb, int indexc)
    {
        this.a = indexa;
        this.b = indexb;
        this.c = indexc;
        this.cmc.y = 0;
    }
    public tria(int indexa, int indexb, int indexc,Vector3 a, Vector3 b, Vector3 c, List<tria>lasttr)
    {
        this.a = indexa;
        this.b = indexb;
        this.c = indexc;
        cmc.x= ((a.x * a.x + a.z * a.z) * (b.z - c.z) + (b.x * b.x + b.z * b.z) * (c.z - a.z) + (c.x * c.x + c.z * c.z) * (a.z - b.z))
            / ((a.x * (b.z - c.z) + b.x * (c.z - a.z) + c.x * (a.z - b.z)) * 2);
        cmc.y = 0;
        cmc.z= ((a.x * a.x + a.z * a.z) * (c.x - b.x) + (b.x * b.x + b.z * b.z) * (a.x - c.x) + (c.x * c.x + c.z * c.z) * (b.x - a.x)) 
            / ((a.x * (b.z - c.z) + b.x * (c.z - a.z) + c.x * (a.z - b.z)) * 2);
        rsq = (a.x - cmc.x) * (a.x - cmc.x) + (a.z - cmc.z) * (a.z - cmc.z);

        edge1 = new edge(indexa, indexb, this);
        edge2 = new edge(indexb, indexc, this);
        edge3 = new edge(indexc, indexa, this);

        lasttr[indexa] = this;
        lasttr[indexb] = this;
        lasttr[indexc] = this;
    }

    public tria next(int i,HashSet<edge>alledges)
    {
        if (edge1.b == i)
            return edge1.twin(alledges).origin;
        if (edge2.b == i)
            return edge2.twin(alledges).origin;
        if (edge3.b == i)
            return edge3.twin(alledges).origin;
        return this;
    }

    public void addedges(HashSet<edge>edges)
    {
        edge1.advance(edges);
        edge2.advance(edges);
        edge3.advance(edges);
    }

    public int neighbour(int i, HashSet<edge> alledges)
    {
        if (edge1.b == i)
            return edge1.a;
        if (edge2.b == i)
            return edge2.a;
        if (edge3.b == i)
            return edge3.a;
        return i;
    }
}
