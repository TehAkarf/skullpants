using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialManager : MonoBehaviour
{
    //variables to represent the canvasGroups

    public CanvasGroup mUnityCanvasGroup;
    public CanvasGroup mGGJCanvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("StartSceneOne");
        
    }

    protected IEnumerator StartSceneOne()
    {
        //Pull up a wait for one seconds
        //Pop the Unity Logo
        yield return new WaitForSeconds(3.0f);

        //Pop the Global Game Jam Logo
        yield return new WaitForSeconds(3.0f);

        //Load game?

        //Load the next scene
        //yield return SceneManager.LoadSceneAsync("Scene1");
    }
}
