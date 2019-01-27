using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string mNextScene = "Scene1";
    // Start is called before the first frame update
    void Start()
    {
        //if(ScreenFader.Instance.mIsFading)
        StartCoroutine(ScreenFader.FadeSceneIn());
        
    }
    IEnumerator FadeSceneIn()
    {
        while(ScreenFader.Instance.mIsFading)
        {
            yield return null;
        }
        PlayerInput.Instance.GainControl();
        yield return StartCoroutine(ScreenFader.FadeSceneIn()); 
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            ChangeToNextScene();
        }
    }

    private void ChangeToNextScene()
    {
        StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.Loading));
        SceneManager.LoadSceneAsync(mNextScene);
        PlayerInput.Instance.ReleaseControl();
    }
}
