// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private string[] levelScenes = { "Level_01", "Level_02", "Level_03" };
    private int currentLevel = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // 注册
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /* ****** 新增：场景加载后同步索引 ****** */
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int index = System.Array.IndexOf(levelScenes, scene.name);
        if (index != -1) currentLevel = index;   // 只在关卡表里才更新
    }

    /* 离开场景时注销，防止重复 */
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelScenes.Length) return;
        if (SceneManager.GetActiveScene().name == levelScenes[index]) return;

        currentLevel = index;
        SceneManager.LoadScene(levelScenes[index]);
    }

    public void LoadNextLevel()
    {
        int next = (currentLevel + 1) % levelScenes.Length;
        LoadLevel(next);
    }

    public void ReloadCurrentLevel() => LoadLevel(currentLevel);
}