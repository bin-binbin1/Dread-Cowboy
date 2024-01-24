using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManger : MonoBehaviour
{
    int playerid;
    string gamestate;

    public void StartGame(object[] obj)
    {
        playerid=(int)obj[1];
        gamestate=(string)obj[0];
    }
}
