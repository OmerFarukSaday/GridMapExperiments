using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons_Manager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FirstLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void SecondLevel()
    {
        SceneManager.LoadScene(2);
    }

    public void ThirdLevel()
    {
        SceneManager.LoadScene(3);
    }

    public void FourthLevel()
    {
        SceneManager.LoadScene(4);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void HowToPlay()
    {
        SceneManager.LoadScene(5);
    }
}
