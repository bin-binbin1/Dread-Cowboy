using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    public GameObject networkManager,teamData;
    public int rounds;//�غ���
    public float gamingTime;//һ�غ���Ϸʱ��
    public float waitingTime;//��Ϸ�����ȴ�ʱ��
    private int playerPosition;
    private int[] scores,platforms,lastScores;
    private int platform, itemType,lastItemType;
    public GameObject[] specialItems;
    public Transform[] platformPositions;
    public GameObject teamButtons;
    private Vector3[] positions = new Vector3[7];
    public int[] A, B;
    public Button[] playerPlatforms;
    public Button[] centrePlatforms;
    public Button playAgain, closeWindow;
    private int round;
    private int useItem;
    public delegate void platformEvent();
    public float jinkuaijiabei=1.5f;
    public TextMeshProUGUI[] goldCount,goldMountain;
    private bool canMove;
    public GameObject settlement;
    public GameObject[] ScoreHead;
    public TextMeshProUGUI[] ScoreName, ScoreScore;
    private Vector3[] originHead=new Vector3[4],originName=new Vector3[4],originScore = new Vector3[4];
    public Sprite Door,originDoor;
    public Image[] homes;
    // Start is called before the first frame update
    void Start()
    {
        originDoor = homes[0].sprite;
        playAgain.onClick.AddListener(() =>
        {
            settlement.SetActive(false);
            networkManager.SendMessage("playAgain");
            //����
        });
        closeWindow.onClick.AddListener(() =>
        {
            settlement.SetActive(false);

        });
        for(int i = 0; i < 4; i++)
        {
            Debug.Log(i + ScoreHead[i].name);
            originHead[i] = ScoreHead[i].transform.position;
            originName[i] = ScoreName[i].transform.position;
            originScore[i] = ScoreScore[i].transform.position;
        }
        settlement.SetActive(false);
        foreach(var texts in goldCount)
        {
            texts.gameObject.SetActive(false);
        }
        foreach(var texts in goldMountain)
        {
            texts.gameObject.SetActive(false);
        }
        foreach (var item in specialItems)
        {
            item.gameObject.SetActive(false);   
        }
        platformEvent[] platformEvents = new platformEvent[7];
        scores = new int[4];
        platforms = new int[7];
        lastScores = new int[4];
        for(int i = 0; i < 7; i++)
        {
            positions[i] = platformPositions[i].position;
        }
        for (int i = 0; i < 7; i++)
        {
            int index = i;
            platformEvents[i] = () =>
            {
                if (canMove)
                {
                    networkManager.SendMessage("makeChoice", index + 1);
                    if (index < 4)
                    {
                        Vector3 offset = positions[index];
                        offset.x += index % 2 == 0 ?150:-150;
                        platformPositions[playerPosition].position = offset;
                    }
                    else
                    {
                        platformPositions[playerPosition].position = positions[index];
                    }
                }
            };
        }
        for (int i = 0; i < 4; i++)
        {
            int index = i;
            playerPlatforms[i].onClick.AddListener(() => platformEvents[index]());
        }
        for (int i = 0; i < 3; i++)
        {
            int index = i;
            centrePlatforms[i].onClick.AddListener(() => platformEvents[index + 4]());
        }
        canMove = false;
    }
    public void gameStart(string message)
    {
        settlement.SetActive(false);
        teamButtons.SetActive(false);
        foreach (var texts in goldCount)
        {
            texts.gameObject.SetActive(true);
        }
        foreach (var texts in goldMountain)
        {
            texts.gameObject.SetActive(true);
        }
        lastItemType = 0;
        round = 0;
        for(int i= 0; i < 4; i++)
        {
            platformPositions[i].position = positions[i];
            goldCount[i].text = "���:0";
        }
        foreach(GameObject t in specialItems)
        {
            t.gameObject.SetActive(false);
        }
        Debug.Log("gameStart!");
        playerPosition = message[0] - '0'-1;
        for(int i=0;i<scores.Length; i++)
        {
            scores[i] = 0;
        }
        //loadScene
        //changeName
    }
    public void roundStart(string message)
    {
        round++;
        canMove = true;
        Debug.Log("round start"+message);
        platform = message[0] - '0'-1;
        itemType = message[1] - '0';
        //��ԭ��ɫ
        for (int i = 0; i < 4; i++)
        {
            platformPositions[i].position = positions[i];
            
        }
        //���
        for(int i = 0; i < 3; i++)
        {
            goldMountain[i].text = $"���:{A[i] * round + B[i]}";
        }
        //�ڷ���Ʒ
        if (itemType>0)
        {
            specialItems[itemType-1].transform.position=platformPositions[platform].position;
            specialItems[itemType-1].SetActive(true);
            
        }
        if (lastItemType ==1)
        {//ţ
            if (useItem == playerPosition)
            {
                //������ţ////////////////////////////////
            }
        }else if (lastItemType ==3)
        {
            Debug.Log(useItem);
            playerPlatforms[useItem].gameObject.SetActive(false);
            homes[useItem].sprite = Door;
        }
        for (int i = 0; i < platforms.Length; i++)
        {
            platforms[i] = 0;
        }
    }
    public void getChoices(string message)//equal to roundEnd
    {
        canMove= false;
        
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
            int pos = message[i] - '0' - 1;
            platforms[pos] += 1;
            platformPositions[i].position = positions[pos];
        }
        if (itemType == 1 && message[5]!='0')
        {//ţ
            platforms[message[6]-'0'-1]++;
        }
        for(int i = 0;i<platforms.Length; i++)
        {
            if (platforms[i] ==1)
            {
                int index=getIndexFromString(message, i+1);

                if (i>=4)
                {//��
                    Debug.Log(i);
                    scores[index] = A[i - 4] * round + B[i - 4] ;
                    if (itemType == 2&& useItem==index)
                    {
                        scores[index] += (A[i - 4] * round + B[i - 4])/2;
                    }
                }
                else if(index!=i)
                {//͵
                    scores[index] = lastScores[i];
                    lastScores[i]=0;
                }
                //score++
            }
            else if (platforms[i]>1)
            {

                //conflicts//////////////////////////////////////
            }
        }
        if (lastItemType == 3)
        {
            playerPlatforms[useItem].gameObject.SetActive(true);
            homes[useItem].sprite = originDoor;
        }
        if (platforms[platform]==1)
        {
            useItem = getIndexFromString(message, platform+1);
                
        }
        for(int i=0;i<scores.Length;i++)
        {
            scores[i] += lastScores[i];
            lastScores[i] = scores[i];
            goldCount[i].text = $"���:{scores[i]}$";
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
    public void setName(string name)
    {
        ScoreName[name[0]-'0'].text=name.Substring(1);
    }
    public void gameEnd()
    {
        for (int i = 0; i < 4; i++)
        {
            platformPositions[i].position = positions[i];
        }
        teamButtons.SetActive(true );
        for (int i = 0; i < 4; i++)
        {
            ScoreHead[i].transform.position = originHead[i];
            ScoreName[i].transform.position = originName[i];
            ScoreScore[i].transform.position = originScore[i];
        }
        for (int i=0;i<4;i++)
        {
            ScoreScore[i].text = scores[i].ToString();
        }
        //show scores;
        int[] rank= new int[4] { 0,1,2,3};
        for(int i=0;i<4;i++)
        {
            for(int j = i + 1; j < 4; j++)
            {
                if (scores[i] < scores[j])
                {
                    int a = scores[i];
                    scores[i] = scores[j];
                    scores[j] = a;
                    a=rank[i];
                    rank[i] = rank[j];
                    rank[j] = a;
                }
            }
        }
        Vector3[] tt1=new Vector3[4];
        Vector3[] tt2=new Vector3[4];
        Vector3[] tt3=new Vector3[4];
        for(int i=0;i<4; i++)
        {
            tt1[i] = ScoreHead[i].transform.position;
            tt2[i] = ScoreName[i].transform.position;
            tt3[i] = ScoreScore[i].transform.position;
        }
        for(int i = 0; i < 4; i++)
        {
            ScoreHead[rank[i]].transform.position = tt1[i];
            ScoreName[rank[i]].transform.position= tt2[i];
            ScoreScore[rank[i]].transform.position = tt3[i];
        }
        settlement.SetActive(true );
        foreach (var texts in goldCount)
        {
            texts.gameObject.SetActive(false);
        }
        foreach (var texts in goldMountain)
        {
            texts.gameObject.SetActive(false);
        }
    }
    
}
