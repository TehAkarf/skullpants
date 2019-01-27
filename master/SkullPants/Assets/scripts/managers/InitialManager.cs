using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialManager : MonoBehaviour
{
    //variables to represent the canvasGroups

    public GameObject mUnityIcon;
    public GameObject mGGJIcon;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("StartSceneOne");
    }

    protected IEnumerator StartSceneOne()
    {
        //Pull up a wait for one seconds
        //Pop the Unity Logo
        mUnityIcon.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        mUnityIcon.SetActive(false);

        //Pop the Global Game Jam Logo
        mGGJIcon.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        mGGJIcon.SetActive(false);

        //Let the game Manager know its transitioning
        GameManager.Instance.mTransitioning = true;

        //Load SceneFader Loading Screen.
        yield return StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.Loading));

        //Load the next scene
        yield return SceneManager.LoadSceneAsync("Scene1");

        //Unload SceneFader Loading Screen.
        yield return StartCoroutine(ScreenFader.FadeSceneIn());

        //Let the gameManager know it is done transitioning
        GameManager.Instance.mTransitioning = false;




    }
}
