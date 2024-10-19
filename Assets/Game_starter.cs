using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using System;

public class Game_starter : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject Base_mesh;
    [SerializeField] private GameObject Turncontrollerprefab;
    [SerializeField] private TMP_InputField _seedInput;
    [SerializeField] private GameObject conection_text;
    GameObject Turncontroller;

    [ClientRpc]
    public void RevealmapClientRpc(ulong Id,int seed,int playercount)
    {

        Base_mesh.SetActive(true);
        Base_mesh.GetComponent<Triangulation>().Setupgame(Id, playercount);
        Base_mesh.GetComponent<Triangulation>().Createmap(seed);
        //Turncontroller.GetComponent<Turncontroller>().callback = Base_mesh.GetComponent<Triangulation>();
        conection_text.SetActive(false);
    }

    public void Startgame()
    {
        Turncontroller = Instantiate(Turncontrollerprefab);
        int playercount = NetworkManager.Singleton.ConnectedClientsIds.Count;
        Debug.Log(playercount);
        Turncontroller.GetComponent<Turncontroller>().playercount = playercount;
        Turncontroller.GetComponent<NetworkObject>().Spawn();
        ulong Id = Turncontroller.GetComponent<NetworkObject>().NetworkObjectId;
        int seed = 42;
        if (Int32.TryParse(_seedInput.text, out int j))
        {
            Debug.Log(j);
            seed = j;
        }
        else
        {
            Debug.Log("String could not be parsed.");
        }
        RevealmapClientRpc(Id,seed,playercount);
    }
}
