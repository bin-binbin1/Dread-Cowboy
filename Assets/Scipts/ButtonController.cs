using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public Button team2help;
    public Button help2team;
    public GameObject team, help;
    // Start is called before the first frame update
    void Start()
    {
        team2help.onClick.AddListener(swapScene);
        help2team.onClick.AddListener(asd);
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

}
