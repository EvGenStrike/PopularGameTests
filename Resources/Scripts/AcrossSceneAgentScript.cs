using UnityEngine;

public class AcrossSceneAgentScript : MonoBehaviour
{
    private static bool isCreated = false;
    private static GameObject instance;

    public string TestName;
    public string TestDifficulty;

    public void SetTestName(string testName)
    {
        TestName = testName;
    }

    public void SetTestDifficulty(string testDifficulty)
    {
        TestDifficulty = testDifficulty;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (isCreated)
        {
            Destroy(instance);
            instance = gameObject;
        }
        if (instance == null)
        {
            instance = gameObject;
            isCreated = true;
        }
    }
}
