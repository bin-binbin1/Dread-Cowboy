using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoManger : MonoBehaviour
{   public float timer = 10f;
    private GameObject receiveCube;
    // Start is called before the first frame update
    void Start()
    {
        receiveCube = GameObject.Find("Client");
    }
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            Destroy(gameObject);
            receiveCube.SendMessage("Refuse");
        }
    }
   public void refuse()
    {
        int id = int.Parse(gameObject.name);
        Destroy(gameObject);
        receiveCube.SendMessage("Refuse",id);
    }
   public void agree()
    {
        int id = int.Parse(gameObject.name);
        Text text = gameObject.GetComponentInChildren<Text>();
        Destroy(gameObject);
        object[] obj = new object[2];
        obj[0] = id;
        obj[1] = text.text;
        Debug.Log("leadername:"+text.text);
        receiveCube.SendMessage("Agree",obj);
    }
    
}
