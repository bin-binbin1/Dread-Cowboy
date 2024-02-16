using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InviteManger : MonoBehaviour
{
    public GameObject networkManager;
    public TMP_InputField friendID;
    public TextMeshProUGUI info;
    int lastid=0;
    public float timer = 0f;
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            networkManager.SendMessage("IsFull");
        });
    }
    void Update()
    {
        timer += Time.deltaTime;
    }
    public void CanInvite(bool isfull)
    {
        if (!isfull)
        {

            if (friendID.text.Length != 0)
            {
                int frid;
                if (!int.TryParse(friendID.text,out frid))
                    info.text = "请输入数字";
                else
                {
                    frid = int.Parse(friendID.text); Debug.Log(frid);
                    if (lastid == frid && timer <10f)
                    {
                        info.text = "10s内只能邀请该玩家一次";
                    }
                    else
                    {
                        timer = 0f;
                        networkManager.SendMessage("InviteFriend", frid);
                        lastid = frid;
                    }
                }
            }
        }
        else
        {
            info.text = "人员已满";
        }
    }


}
