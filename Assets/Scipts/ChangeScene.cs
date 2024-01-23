using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{

    public string nextSceneName;
    public float timeDuration; // 渐变持续时间
    public Button startGameButton;
    public GameObject backgroundToMove;
    public float moveLength;
    private void Start()
    {
        startGameButton.onClick.AddListener(Click);
    }
    private void Click()
    {
        changeCoroutine();
    }

    private IEnumerator changeCoroutine()
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = backgroundToMove.GetComponent<Transform>().transform.position;

        while (elapsedTime < timeDuration)
        {
            // 计算当前帧的平移距离
            float t = elapsedTime / timeDuration;
            float currentMove = Mathf.Lerp(0, moveLength, t);

            // 使用Translate方法平移物体
            transform.Translate(Vector3.up * currentMove * Time.deltaTime);

            // 等待到下一帧
            yield return null;

            // 更新已经经过的时间
            elapsedTime += Time.deltaTime;
        }

        // 确保在timeDuration内移动到指定位置
        transform.position = initialPosition + Vector3.up * moveLength;

        SceneManager.LoadScene(nextSceneName);
    }

}
