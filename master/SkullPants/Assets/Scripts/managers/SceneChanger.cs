using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public string mNextScene = "Scene1";
    // Start is called before the first frame update
    private void Start()
    {
        PlayerInput.Instance.ReleaseControl();
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
        PlayerInput.Instance.ReleaseControl();
        StartCoroutine( ScreenFader.Instance.ChangeScene());
    }
}
