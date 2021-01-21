using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    public string[] allScenes;
    public Animator fadeScrenAnim;
    public SpriteRenderer fadeScren;
    public static Scene_Manager instance;
    public string scene;

    IEnumerator LoadLevel(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        fadeScrenAnim.SetTrigger("fadeOut");
        operation.allowSceneActivation = false;
        while (operation.isDone == false)
        {           
            if (operation.progress >= 0.9f)
            {
                if (fadeScren.color.a == 1)
                {
                    operation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }
    void Awake()
    {
        instance = this;
        transform.gameObject.SetActive(false);
    }
    private void Start()
    {
        StartCoroutine(LoadLevel(scene));
    }
    public void CallNewScene(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName));
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void BackMenu()
    {
        SceneManager.LoadScene(0);
    }
}
