using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public int rounds;//回合数
    public float gamingTime;//一回合游戏时间
    public float waitingTime;//游戏结束等待时间
    private int playerPosition;
    private int[] scores,platforms;
    private int platform, itemType;
    public GameObject[] specialItems;
    public Transform[] platformPositions;
    // Start is called before the first frame update
    void Start()
    {
        scores = new int[4];
        platforms = new int[7];
        
    }
    public void gameStart(string message)
    {
        foreach(Transform t in platformPositions)
        {
            t.gameObject.SetActive(false);
        }
        foreach(GameObject t in specialItems)
        {
            t.gameObject.SetActive(false);
        }
        Debug.Log("gameStart!");
        playerPosition = message[0] - '0';
        for(int i=0;i<scores.Length; i++)
        {
            scores[i] = 0;
        }
        for(int i=0;i< platforms.Length; i++)
        {
            platforms[i] = 0;
        }
        //loadScene
        //changeName
    }
    public void roundStart(string message)
    {
        platform = message[0] - '0'-1;
        itemType = message[1] - '0';
        //摆放物品
        if(itemType>0)
        {
            specialItems[itemType-1].transform.position=platformPositions[platform].position;
            specialItems[itemType-1].SetActive(true);
            
        }

    }
    public void getChoices(string message)//equal to roundEnd
    {
        if (itemType > 0)
        {
            specialItems[itemType - 1].SetActive(false);
        }
        for(int i = 0; i < 4; i++)
        {
            platforms[message[i]-'0'] += 1;
        }
        for(int i = 0;i<platforms.Length; i++)
        {
            if (platforms[i] ==1)
            {
                //score++
            }
            else if (platforms[i]>1)
            {
                //conflicts
            }
        }
        if (platforms[platform]==1)
        {
            //getIndexFromString(message, platform);
        }
        
    }
    private int getIndexFromString(string s, int a)
    {
        for(int i = 0; i < s.Length; i++)
        {
            if (s[i] == a+'0')
                return i;
        }
        return -1;
    }
    public void gameEnd(string message)
    {
        //show scores;
    }
    
}
