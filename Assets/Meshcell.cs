using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class Meshcell : MonoBehaviour
{
    public Material material;
    public int index;
    public float depletiontime = 1f;
    public AnimationCurve curve;
    public AnimationCurve floatingcurve;
    public float animationtime;
    public float hoveranimationtime;
    public Material sandmaterial;
    public Triangulation goback;
    public Turncontroller turncontroller;
    public bool available = false;
    private MeshRenderer rend;
    public UnityEngine.Color color;
    public bool occupied = false;
    public bool taken = false;
    public UnityEngine.Color sandcolor;
    private MeshCollider colliderm;
    private bool hoverstate = false;
    private bool hoverstate2 = false;
    private bool hoverst = false;
    private bool moving = false;
    private bool started = false;
    private Vector3 pos;
    private UnityEngine.Color color2;
    private bool mousein = false;
    void Awake()
    {
        rend = gameObject.GetComponent<MeshRenderer>();
        rend.material = material;
        color = rend.material.color;
        color2 = color;
        colliderm = gameObject.GetComponent<MeshCollider>();
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        pos = transform.position;
    }

    private void Start()
    {
        StartCoroutine(hover());
    }

    IEnumerator hover()
    {
        float time = 0;
        while(time < animationtime)
        {
            time += Time.deltaTime;
            transform.SetPositionAndRotation(new Vector3(pos.x,pos.y+ curve.Evaluate(time / animationtime)*10,pos.z), Quaternion.identity);
            yield return null;
        }
        transform.SetPositionAndRotation(pos, Quaternion.identity);
        started=true;
    }
    private void OnMouseEnter()
    {
        mousein = true;
        setcolor();
        hoverstate2 = true;
        hoverst = hoverstate || hoverstate2;
        if(moving == false&&started)
        {
            StartCoroutine(floatup());
        }
    }
    private void OnMouseExit()
    {
        mousein = false;
        setcolor();
        hoverstate2 = false;
        hoverst = hoverstate || hoverstate2;
        if (moving == false&&started)
        {
            StartCoroutine(floatup());
        }
    }

    public void colorup()
    {
        if (taken == false)
            color2 = (color + 2 * UnityEngine.Color.red) / 2;
        else
            color2 = UnityEngine.Color.gray;
        setcolor();
    }

    public void colordown()
    {
        color2 = color;
        setcolor();
    }

    private void setcolor()
    {
        if(mousein)
            rend.material.SetColor("_Color", (color2 + UnityEngine.Color.yellow) / 2);
        else
            rend.material.SetColor("_Color", color2);
    }

    private void OnMouseUpAsButton()
    {
        if (occupied == false)
        {
            StartCoroutine(deplete());
            rend.material.SetColor("_Color", color);
            turncontroller.invoke(this);
        }
    }

    public void makeavailable()
    {
        goback.highlightneighbours(index);
        occupied = true;
    }

    public void makeunavailable()
    {
        goback.lowlightneighbours(index);
        occupied = false;
    }

    public void floatfor(bool up)
    {
        hoverstate = up;
        hoverst = hoverstate || hoverstate2;
        if (moving == false && started)
        {
            StartCoroutine(floatup());
        }
    }

    public void makesand()
    {
        rend.material.color = sandcolor;
    }
    public void takeside(UnityEngine.Color adcolor)
    {
        if (taken == false)
        {
            StartCoroutine(deplete());
            color = (color + adcolor) / 2;
            color2 = color;
            GetComponent<border_renderer>().color = UnityEngine.Color.white - color + new UnityEngine.Color(0,0,0,1f);
            setcolor();
            taken = true;
        }
        occupied = true;
    }
    private IEnumerator deplete()
    {
        float times = 0f;
        while (times < depletiontime)
        {
            times += Time.deltaTime;
            rend.material.SetFloat("_percentage", 1f - (depletiontime - times) / depletiontime);
            yield return null;
        }
        rend.material.SetFloat("_percentage", 1f);
    }

    private IEnumerator floatup()
    {
        moving = true;
        float time = 0;
        bool last = hoverst;
        while(time<hoveranimationtime)
        {
            if(last!= hoverst)
            {
                last= hoverst;
                time = hoveranimationtime - time;
            }
            time += Time.deltaTime;
            if(time> hoveranimationtime)
                time = hoveranimationtime;
            if(hoverst==true)
            {
                transform.position = pos + Vector3.up * floatingcurve.Evaluate(time / hoveranimationtime);
            }
            else
            {
                transform.position = pos + Vector3.up * floatingcurve.Evaluate(1f-time / hoveranimationtime);
            }
            yield return null;
        }
        if (hoverst == true)
        {
            transform.position = pos + Vector3.up;
        }
        else
        {
            transform.position = pos;
        }
        moving = false;
    }
}
