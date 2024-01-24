using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingManger : MonoBehaviour
{
    private GameObject receiveCube;
    public bool IsMatch=false;
    bool isleader = true;
    private GameObject dialog;
    public Text tt;
    void Start()
    {
        
        receiveCube = GameObject.Find("Client");
        dialog = GameObject.Find("Dialog");
        tt = GetComponentInChildren<Text>();
        tt.text = "Start Matching";
        
    }
    void CanMatch()
    {
         receiveCube.SendMessage("CanMatch");
    }

    void IsLeader(bool l)
    {
        isleader= l;
    }
    
    public void OnClickMatching()
    {
        CanMatch();
        if (!IsMatch)
        {
            if (!isleader)
            {

            }
            else
            {
                StartMatch();
            }
        }
        else
        {StopMatch();
        }
        
    }

    void StartMatch()
    {
        receiveCube.SendMessage("SendStartMatch");
        tt.text = "Stop Matching";
        dialog.transform.GetComponent<CanvasGroup>().alpha = 1; IsMatch = !IsMatch;
    }
    void StopMatch()
    {
        receiveCube.SendMessage("SendStopMatch");
        tt.text = "Start Matching";
        dialog.transform.GetComponent<CanvasGroup>().alpha = 0;
        IsMatch = !IsMatch;
    }
}
