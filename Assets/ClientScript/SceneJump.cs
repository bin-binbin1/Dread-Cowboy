using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneJump : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	/// <summary>
	/// ��ת��������
	/// </summary>
	public void JumpToMain()//��һ������ 
	{
		SceneManager.LoadScene(2);
	}
	/// <summary>
	/// ��ת������������
	/// </summary>
	public void JumpToSign() //�ڶ�������
	{
		SceneManager.LoadScene(1);

	}
}


