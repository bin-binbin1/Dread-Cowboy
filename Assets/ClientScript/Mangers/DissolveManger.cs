using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveManger : MonoBehaviour
{
    private GameObject receiveCube;
    void Start()
    {
        receiveCube = GameObject.Find("Client");
    }
    public void Leave()
    {
        receiveCube.SendMessage("DissolveTeam");
    }
}
