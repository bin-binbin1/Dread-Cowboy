using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public int rounds;//�غ���
    public float gamingTime;//һ�غ���Ϸʱ��
    public float waitingTime;//��Ϸ�����ȴ�ʱ��
    private int playerPosition;
    private int[] scores,choices,platforms;
    private int platform, itemType;
    // Start is called before the first frame update
    void Start()
    {
        scores = new int[4];
        platforms = new int[7];
    }
    public void gameStart(string message)
    {
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
        //�޸ĳ���������׼��
        //��ʾ��ɫ���֣���
    }
    public void roundStart(string message)
    {
        platform = message[0] - '0';
        itemType = message[1] - '0';
        //�ڷ���Ʒ


    }
    public void getChoices(string message)//equal to roundEnd
    {
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
