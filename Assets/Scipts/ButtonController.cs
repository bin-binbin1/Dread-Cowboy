using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public GameObject networkManager;
    public Button team2help;
    public Button help2team;
    public GameObject team, help;
    public Button exitGame, matchingGame;
    public TextMeshProUGUI matchingContext;

    public Button acceptInv, rejectInv;
    public Button leaveTeam;
    public Button items;
    public Button continueGame, closeGame,peButton;
    private bool isleader=true,isMatching=false;
    // Start is called before the first frame update
    void Start()
    {
        team2help.onClick.AddListener(swapScene);
        help2team.onClick.AddListener(asd);
        leaveTeam.onClick.AddListener(LeaveTeam);
        matchingGame.onClick.AddListener(OnClickMatching);
        closeGame.onClick.AddListener(closeApp);
        peButton.onClick.AddListener(() =>
        {
            networkManager.SendMessage("pe");
        });
        acceptInv.onClick.AddListener(() =>
        {
            networkManager.SendMessage("Agree");
        });
    }
    public void closeApp()
    {
        Application.Quit();
    }
    public void LeaveTeam()
    {
        networkManager.SendMessage("leave");
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
        Debug.Log("Clicked Match");
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
    }
    void StopMatch()
    {
        networkManager.SendMessage("SendStopMatch");
        matchingContext.text = "Start Matching";

    }
}
