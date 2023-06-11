using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Game_starter : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject Base_mesh;
    [SerializeField] private GameObject Turncontrollerprefab;
    GameObject Turncontroller;

    [ClientRpc]

    public void RevealmapClientRpc(ulong Id)
    {
        Base_mesh.SetActive(true);
        Base_mesh.GetComponent<Triangulation>().Startgame(Id);
        //Turncontroller.GetComponent<Turncontroller>().callback = Base_mesh.GetComponent<Triangulation>();
    }
    public void Startgame()
    {
        Turncontroller = Instantiate(Turncontrollerprefab);
        Turncontroller.GetComponent<Turncontroller>().playercount = 2;
        Turncontroller.GetComponent<NetworkObject>().Spawn();
        ulong Id = Turncontroller.GetComponent<NetworkObject>().NetworkObjectId;
        RevealmapClientRpc(Id);
    }
}
