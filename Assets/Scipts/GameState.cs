using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    public GameObject networkManager;
    public int rounds;//回合数
    public float gamingTime;//一回合游戏时间
    public float waitingTime;//游戏结束等待时间
    private int playerPosition;
    private int[] scores,platforms,lastScores;
    private int platform, itemType,lastItemType;
    public GameObject[] specialItems;
    public Transform[] platformPositions;
    public int[] A, B;
    public Button[] playerPlatforms;
    public Button[] centrePlatforms;
    private int round;
    private int useItem;
    public delegate void platformEvent();
    public float jinkuaijiabei=1.5f;
    // Start is called before the first frame update
    void Start()
    {
        platformEvent[] platformEvents = new platformEvent[7];
        scores = new int[4];
        platforms = new int[7];
        lastScores = new int[4];
        for(int i = 0; i <7; i++)
        {
            platformEvents[i] = () =>
            {
                networkManager.SendMessage("makeChoice", i + 1);
            };
        }
        for(int i = 0; i < 4; i++)
        {
            playerPlatforms[i].onClick.AddListener(()=>platformEvents[i]());
        }
        for(int i = 0; i < 3; i++)
        {
            centrePlatforms[i].onClick.AddListener(() => platformEvents[i + 4]());
        }
    }
    public void gameStart(string message)
    {   
        lastItemType= 0;
        round = 0;
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
        if (lastItemType =='1')
        {//牛
            if (useItem == playerPosition)
            {
                //可以用牛////////////////////////////////
            }
        }else if (lastItemType =='2')
        {
            playerPlatforms[useItem].gameObject.SetActive(false);
        }
    }
    public void getChoices(string message)//equal to roundEnd
    {
        round++;
        for(int i = 0; i < scores.Length; i++)
        {
            scores[i] = 0;
        }
        if (itemType > 0)
        {
            specialItems[itemType - 1].SetActive(false);
        }
        for(int i = 0; i < 4; i++)
        {
            platforms[message[i]-'0'] += 1;
        }
        if (itemType == 1)
        {//牛
            platforms[message[5]]++;
        }
        for(int i = 0;i<platforms.Length; i++)
        {
            if (platforms[i] ==1)
            {
                int index=getIndexFromString(message, i+1);
                if (i>=4)
                {//挖
                    scores[index] = A[i - 4] * round + B[i - 4] + lastScores[index];
                    if (itemType == 3&& useItem==index)
                    {
                        scores[index] += (A[i - 4] * round + B[i - 4])/2;
                    }
                }
                else
                {//偷
                    scores[index] += lastScores[index]+lastScores[i];
                    scores[i] -= lastScores[i];
                }
                //score++
            }
            else if (platforms[i]>1)
            {

                //conflicts//////////////////////////////////////
            }
        }
        if (platforms[platform]==1)
        {
            useItem = getIndexFromString(message, platform);
            
        }
        for(int i=0;i<scores.Length;i++)
        {
            lastScores[i] = scores[i];
        }
        if (lastItemType == '2')
        {
            playerPlatforms[useItem].gameObject.SetActive(true);
        }
        lastItemType = itemType;
        
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
        for(int i=0;i< scores.Length;i++)
        {
            ;////////////////////////////////
        }
    }
    
}
