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
	/// 跳转至主界面
	/// </summary>
	public void JumpToMain()//第一个场景 
	{
		SceneManager.LoadScene(2);
	}
	/// <summary>
	/// 跳转至输入名字那
	/// </summary>
	public void JumpToSign() //第二个场景
	{
		SceneManager.LoadScene(1);

	}
}


