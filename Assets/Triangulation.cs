using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
public class Triangulation : MonoBehaviour
{
    public float size;
    public int n;
    public GameObject meshcell;
    public float scale;
    public float amplitude;
    public GameObject turncontroller;
    private float radius;
    private float radiussq;
    private List<Vector3> points = new List<Vector3>();
    private List<tria> lasttr = new List<tria>();
    private List<tria> triangles = new List<tria>();
    private List<Vector2> uvmap = new List<Vector2>();
    private GameObject cell;
    private HashSet<edge> edges = new HashSet<edge>();
    private HashSet<edge> alledges = new HashSet<edge>();
    private float ofsetx,ofsety;
    private Turncontroller turnrefr;

    private Mesh mesh;
    private List<Vector3> cellpoints;
    private List<int> celltrias;
    private List<int> neighbours = new List<int>();
    private Vector3 p;

    private List<Geometrycell> geocells = new List<Geometrycell>();
    private List<int> queue = new List<int>();
    
    private class Geometrycell
    {
        public bool visited = false;
        public List<Vector3> cellpoints = new List<Vector3>();
        public List<int> celltrias = new List<int>();
        public Vector3 pos = new Vector3();
        public List<int> neighbours = new List<int>();
        public List<int> validneighbours = new List<int>();
        public int index = new int();
        public Meshcell refr;
    }

    public float origindistancesq(Vector3 p)
    {
        return p.x*p.x+p.z*p.z;
    }
    public bool intriangle(Vector3 p,int it)
    {
        tria tr = triangles[it];
        Vector3 a = points[tr.a];
        Vector3 b = points[tr.b];
        Vector3 c = points[tr.c];
        return (Vector3.Cross(b-a, p-a).y > 0 && 
                Vector3.Cross(c-b, p-b).y > 0 &&
                Vector3.Cross(a-c, p-c).y > 0);
    }
    private bool incircumcircle(Vector3 p,int it)
    {
        tria tr = triangles[it];
        if ((p.x - tr.cmc.x) * (p.x - tr.cmc.x) + (p.z - tr.cmc.z) * (p.z - tr.cmc.z) <= tr.rsq)
            return true;
        return false;
    }

    public Meshcell getrefrencefromindex(int index)
    {
        return geocells[index].refr;
    }
    private float falloff(Vector3 p)
    {
        if (origindistancesq(p) <= 1.5f*radiussq)
            return 0;
        if (origindistancesq(p) >= 7 * radiussq)
            return 1.1f;
        return Mathf.Sqrt(origindistancesq(p) - 1.5f*radiussq)*0.01f;
    }

    private List<Vector3> tempdebugpoints = new List<Vector3>();
    private void OnDrawGizmos()
    {
        foreach (Vector3 temppoint in tempdebugpoints)
        {
            Gizmos.color = UnityEngine.Color.yellow;
            Gizmos.DrawSphere(temppoint, 1);
        }
    }
    private float calc_height(Vector3 p)
    {
        
        float nx = 1f * p.x / size ;
        float ny = 1f * p.z / size ;
        nx = Mathf.Clamp(nx, -1f, 1f);
        ny = Mathf.Clamp(ny, -1f, 1f);
        float d = amplitude*(1 - nx * nx) * (1 - ny * ny)/2 - 5f - new_falloff(p);
        tempdebugpoints.Add(new Vector3(p.x,d,p.z));
        //return amplitude*((Mathf.PerlinNoise(p.x/scale+ofsetx,p.z/scale+ofsety)-0.35f)-falloff(p));
        return d;

    }

    private float new_falloff(Vector3 p)
    {
        float fall = 0f;
        if(p.x>size)
        {
            fall+=10*p.x/size;
        }
        if(p.z>size)
        {
            fall+=10*p.z/size;
        }
        if (-p.x > size)
        {
            fall += 10 * (-p.x) / size;
        }
        if (-p.z > size)
        {
            fall += 10 * (-p.z) / size;
        }
        return fall;
    }

    public void Setupgame(ulong Id,int playercount)
    {
        NetworkObject tempturnc = NetworkManager.Singleton.SpawnManager.SpawnedObjects[Id];
        turnrefr = tempturnc.GetComponent<Turncontroller>();
        turnrefr.callback = this;
        turnrefr.playercount = playercount;
    }

    public void Createmap(int seed)
    {
        radius = size / 3;

        Vector3 p0 = new Vector3(-size, 0, -size);
        Vector3 p1 = new Vector3(size, 0, -size);
        Vector3 p2 = new Vector3(size, 0, size);
        Vector3 p3 = new Vector3(-size, 0, size);

        points.Add(p0);
        points.Add(p1);
        points.Add(p2);
        points.Add(p3);
        lasttr.Add(null);
        lasttr.Add(null);
        lasttr.Add(null);
        lasttr.Add(null);
        geocells.Add(null);
        geocells.Add(null);
        geocells.Add(null);
        geocells.Add(null);

        uvmap.Add(new Vector2(0, 0));
        uvmap.Add(new Vector2(1, 0));
        uvmap.Add(new Vector2(1, 1));
        uvmap.Add(new Vector2(0, 1));

        tria t1, t2;
        t1 = new tria(0, 2, 1, p0, p2, p1, lasttr);
        triangles.Add(t1);

        t2 = new tria(2, 0, 3, p2, p0, p3, lasttr);
        triangles.Add(t2);

        int pointsin = 0;
        p = new Vector3(0, 0, 0);
        radiussq = radius * radius;
        Random.InitState(seed);
        ofsetx = Random.Range(-1000,1000);
        ofsety = Random.Range(-1000, 1000);
        tria tr;

        int it;
        while (pointsin < n)
        {
            p.x = Random.Range(-size, size);
            p.z = Random.Range(-size, size);
            p.y = calc_height(p);
            points.Add(p);
            geocells.Add(new Geometrycell());
            lasttr.Add(null);
            uvmap.Add(new Vector2((p.x + size) / size / 2, (p.z + size) / size / 2));

            for (it = 0; it < triangles.Count; it++)
            {
                if (incircumcircle(p, it))
                {
                    tr = triangles[it];
                    tr.addedges(edges);
                    triangles.RemoveAt(it);
                    it--;
                }
            }

            foreach (edge e in edges)
            {
                triangles.Add(new tria(e.a, e.b, points.Count - 1, points[e.a], points[e.b], p, lasttr));
            }
            edges.Clear();


            if (origindistancesq(p) < radiussq)
            {
                pointsin++;
            }
        }
        foreach (tria trr in triangles)
        {
            alledges.Add(trr.edge1);
            alledges.Add(trr.edge2);
            alledges.Add(trr.edge3);
        }

        int i;
        int last = 4;
        for (i = 4; i < points.Count; i++)
        {
            makecell(i);
            //geocellls[i]
            if (!issand(i))
            {
                last = i;
            }
            else
            {
                maketile(false, i);
            }
        }
        it = 0;
        queue.Add(last);
        geocells[last].visited = true;

        while (it < queue.Count)
        {
            i = queue[it];
            var current = geocells[i];
            foreach (int ii in current.neighbours)
            {
                if (!issand(ii))
                {
                    current.validneighbours.Add(ii);
                    if (geocells[ii].visited == false)
                    {
                        geocells[ii].visited = true;
                        queue.Add(ii);
                    }
                }
            }
            it++;
        }
        StartCoroutine(buildboard());
    }
    private void Awake()
    {

    }

    void generateuv()
    {
        uvmap = new List<Vector2>();
        foreach( var ps in cellpoints)
        {
            uvmap.Add(new Vector2(ps.x/scale,ps.z/scale));
        }
    }
    void maketile(bool sand, int ii)
    {
        mesh = new Mesh();
        int counts = cellpoints.Count;
        float factor = 25f;
        ///*
        cellpoints.Add(cellpoints[1] + Vector3.down * factor);
        for(int i=2; i < counts; i++)
        {
            //makequad()
            cellpoints.Add(cellpoints[i] + Vector3.down * factor);

            celltrias.Add(i-1);
            celltrias.Add(cellpoints.Count - 2);
            celltrias.Add(i);

            celltrias.Add(cellpoints.Count - 2);
            celltrias.Add(cellpoints.Count - 1);
            celltrias.Add(i);
        }
        //*/
        mesh.vertices = cellpoints.ToArray();
        generateuv();
        mesh.uv = uvmap.ToArray();
        mesh.triangles = celltrias.ToArray();
        cell = Instantiate(meshcell, p, Quaternion.identity);

        cell.GetComponent<MeshFilter>().mesh = mesh;
        cell.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        cell.GetComponent<MeshFilter>().mesh.RecalculateTangents();
        cell.GetComponent<MeshCollider>().sharedMesh = mesh;

        geocells[ii].refr = cell.GetComponent<Meshcell>();

        if (sand == true)
        {
            cellpoints.RemoveAt(0);
            cell.GetComponent<border_renderer>().points = cellpoints.GetRange(0,counts-1);
            cell.GetComponent<Meshcell>().goback=this;
            cell.GetComponent<Meshcell>().index = ii;
            cell.GetComponent<Meshcell>().turncontroller=turnrefr;
        }
        else
        {
            Destroy(cell.GetComponent<border_renderer>());
            cell.GetComponent<Meshcell>().makesand();
            Destroy(cell.GetComponent<Meshcell>());
        }
    }

    public void highlightneighbours(int index)
    {
        var cell = geocells[index];
        Meshcell ncell;
        foreach( var i in cell.validneighbours)
        {
            ncell = geocells[i].refr;
            if (ncell.occupied == false)
            {
                ncell.floatfor(true);
                ncell.colorup();
                ncell.available = true;
            }
        }
    }

    public void lowlightneighbours(int index)
    {
        var cell = geocells[index];
        Meshcell ncell;
        foreach (var i in cell.validneighbours)
        {
            ncell = geocells[i].refr;
            if (ncell.occupied == false)
            {
                ncell.floatfor(false);
                ncell.colordown();
                ncell.available = false;
            }
        }
    }

    bool issand(int i)
    {
        try
        {
            foreach (var ps in geocells[i].cellpoints)
            {
                if (ps.y + geocells[i].pos.y < 0f)
                    return true;
            }
            return false;
        }
        catch { 
            return false; 
        }
    }
    void makecell(int i)
    {
        cellpoints = new List<Vector3>();
        celltrias = new List<int>();
        neighbours = new List<int>();
        p = new Vector3(0, 0, 0);
        p = points[i];
        geocells[i].pos = p;
        cellpoints.Add(p - p);

        tria t1 = lasttr[i];
        t1.cmc.y = calc_height(t1.cmc);
        cellpoints.Add(t1.cmc - p);
        neighbours.Add(t1.neighbour(i, alledges));
        tria t2;
        tria tr = t1.next(i, alledges);
        do
        {
            //add point
            tr.cmc.y = calc_height(tr.cmc);
            cellpoints.Add(tr.cmc - p);
            neighbours.Add(tr.neighbour(i, alledges));
            //make triangle
            celltrias.Add(cellpoints.Count - 2);
            celltrias.Add(cellpoints.Count - 1);
            celltrias.Add(0);
            //next
            t2 = tr;
            tr = tr.next(i, alledges);
        }
        while (t2 != t1);

        geocells[i].cellpoints = cellpoints;
        geocells[i].celltrias = celltrias;
        geocells[i].neighbours = neighbours;
    }
    IEnumerator buildboard()
    {
        for (int i = 0; i < queue.Count; i++)
        {
            cellpoints = geocells[queue[i]].cellpoints;
            celltrias = geocells[queue[i]].celltrias;
            p = geocells[queue[i]].pos;
            maketile(true, queue[i]);
            //starter(cellpoints);
            yield return new WaitForSeconds(0.015f);
        }
    }
}


