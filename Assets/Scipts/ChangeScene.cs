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
    public void Click()
    {
        StartCoroutine(changeCoroutine());
    }

    private IEnumerator changeCoroutine()
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = backgroundToMove.transform.position; // 注意这里的获取方式

        while (elapsedTime < timeDuration)
        {
            // 计算当前帧的平移距离
            float t = elapsedTime / timeDuration;
            float currentMove = Mathf.Lerp(0, moveLength, t);

            // 计算新的 Y 坐标
            float newY = initialPosition.y + currentMove;

            // 使用新的位置更新物体的 Y 坐标
            backgroundToMove.transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);

            // 等待到下一帧
            yield return null;

            // 更新已经经过的时间
            elapsedTime += Time.deltaTime;
        }

        // 确保在 timeDuration 内移动到指定位置
        backgroundToMove.transform.position = new Vector3(initialPosition.x, initialPosition.y + moveLength, initialPosition.z);

        SceneManager.LoadScene(nextSceneName);
    }

}
