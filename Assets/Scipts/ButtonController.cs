using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public GameObject networkManager;
    public Button team2help;
    public Button help2team;
    public GameObject team, help;
    public Button[] playerPlatforms;
    public Button[] centrePlatforms;
    public Button exitGame, matchingGame;
    public Text matchingContext;
    public GameObject dialog;
    public Button searchPeople, acceptInv, rejectInv;
    public Button destroyTeam, leaveTeam;
    public Button items;
    public Button continueGame, closeGame;
    private bool isleader=true,isMatching=false;
    // Start is called before the first frame update
    void Start()
    {
        team2help.onClick.AddListener(swapScene);
        help2team.onClick.AddListener(asd);
        leaveTeam.onClick.AddListener(LeaveTeam);
    }
    public void LeaveTeam()
    {
        networkManager.SendMessage("DissolveTeam");
    }
    void swapScene()
    {
        team.SetActive(false);
        help.SetActive(true);
    }
    void asd()
    {
        team.SetActive(true);
        help.SetActive(false);
    }
    void CanMatch()// ?
    {
         networkManager.SendMessage("CanMatch");
    }
    public void IsLeader(bool l)
    {
        isleader = l;
    }
    public void OnClickMatching()
    {
        if (isleader)
        {
            if (isMatching)
            {
                StopMatch();
            }
            else
            {
                StartMatch();
            }
            isMatching = !isMatching;
        }


    }
    void StartMatch()
    {
        networkManager.SendMessage("SendStartMatch");
        matchingContext.text = "Stop Matching";
        dialog.transform.GetComponent<CanvasGroup>().alpha = 1;
    }
    void StopMatch()
    {
        networkManager.SendMessage("SendStopMatch");
        matchingContext.text = "Start Matching";
        dialog.transform.GetComponent<CanvasGroup>().alpha = 0;
    }
}
