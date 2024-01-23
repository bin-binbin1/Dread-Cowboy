using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneController : MonoBehaviour
{
    public GameObject game, team;
    // Start is called before the first frame update
    void Start()
    {
        team.SetActive(true);
        game.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
