using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Turncontroller : MonoBehaviour
{
    public int playercount;
    public int setupcount = 0;
    public int currentplayer = 0;
    public TMP_Text templatetext;
    public Canvas Canvas;
    public GameObject pawn;
    public List<Meshcell> playertiles = new List<Meshcell>();
    public List<GameObject> pawns = new List<GameObject>();
    public List<Color> playercolors = new List<Color>();
    public List<int> scores = new List<int>();
    public List<TMP_Text> scoretexts = new List<TMP_Text>();

    private void Awake()
    {
        playercolors.Add(Color.white);
        playercolors.Add(Color.black);
        playercolors.Add(Color.blue);
        Canvas=Instantiate(Canvas);
    }
    public void invoke(Meshcell the)
    {
        if(setupcount<playercount)
        {
            //pawn.GetComponent<MeshRenderer>().material.color = colors[setupcount];
            setupcount++;
            playertiles.Add(the);
            pawns.Add(Instantiate(pawn,the.transform.position,Quaternion.identity));
            pawns[setupcount-1].GetComponent<MeshRenderer>().material.color = playercolors[setupcount-1];
            scores.Add(0);
            scoretexts.Add(Instantiate(templatetext,templatetext.transform.position,Quaternion.identity,Canvas.transform));
            scoretexts[setupcount - 1].GetComponent<RectTransform>().anchoredPosition = new Vector2(10f,-setupcount*40f);
            scoretexts[setupcount - 1].GetComponent<RectTransform>().pivot = new Vector2(0,0.5f);
            if (the.taken == false)
                scores[setupcount - 1]++;
            the.takeside(playercolors[setupcount - 1]);
            scoretexts[setupcount - 1].text = "Player " + setupcount + " score: " + scores[setupcount - 1];
            if(setupcount == playercount)
            {
                playertiles[currentplayer].makeavailable();
            }
        }
        else
        {
            if(the.available==true)
            {
                pawns[currentplayer].transform.position = the.transform.position;
                playertiles[currentplayer].makeunavailable();
                playertiles[currentplayer] = the;
                if (the.taken == false)
                    scores[currentplayer]++;
                the.takeside(playercolors[currentplayer]);
                scoretexts[currentplayer].text = "Player " + (currentplayer + 1) + " score: " + scores[currentplayer];
                currentplayer++;
                if(currentplayer==playercount)
                    currentplayer = 0;
                playertiles[currentplayer].makeavailable();
            }
        }
    }
}
