using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    //Enums for types of screenfades
    public enum FadeType
    {
        Black, Loading, GameOver, Win
    }

    //Variables to hold the values for this class
    protected static ScreenFader sInstance; //singleton

    public float mFadeDuration = 1f;  //How long should a fade take?

    public bool mIsFading;  //Are we fading right now

    const int mMaxSortingLayer = 32767;

    //Groups should match the enum fadetypes
    public CanvasGroup mFaderCanvasGroup;
    public CanvasGroup mLoadingCanvasGroup;
    public CanvasGroup mGameOverCanvasGroup;
    public CanvasGroup mWinCanvasGroup;


    int mNextScene = 0;
    enum SceneNames
    {
        Initial,Scene1, Scene2, Scene3, Scene4, Ending
    }
    SceneNames mCurrentScene = SceneNames.Initial;


    public static ScreenFader Instance
    {
        get
        {
            if ( sInstance != null )
                return sInstance;

            sInstance = FindObjectOfType<ScreenFader>();

            if ( sInstance != null )
                return sInstance;

            Create();

            return sInstance;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public static void Create()
    {
        ScreenFader controllerPrefab = Resources.Load<ScreenFader>( "ScreenFader" );
        sInstance = Instantiate( controllerPrefab );
    }

    protected IEnumerator Fade( float finalAlpha, CanvasGroup canvasGroup )
    {
        mIsFading = true;
        canvasGroup.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs( canvasGroup.alpha - finalAlpha ) / mFadeDuration;
        while (!Mathf.Approximately( canvasGroup.alpha, finalAlpha ) )
        {
            canvasGroup.alpha = Mathf.MoveTowards( canvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime );
            yield return null;
        }
        canvasGroup.alpha = finalAlpha;
        mIsFading = false;
        canvasGroup.blocksRaycasts = false;
    }

    public static void SetAlpha( float alpha )
    {
        Instance.mFaderCanvasGroup.alpha = alpha;
    }

    //Loading becomes invisible
    public static IEnumerator FadeSceneIn()
    {
        CanvasGroup canvasGroup;
        if ( Instance.mFaderCanvasGroup.alpha > 0.1f )
            canvasGroup = Instance.mFaderCanvasGroup;
        else
            canvasGroup = Instance.mLoadingCanvasGroup;

        yield return Instance.StartCoroutine( Instance.Fade( 0f, canvasGroup ) );

        canvasGroup.gameObject.SetActive( false );

        //after fadng in, display a start of level text if any
        Instance.DisplayLevelText(Instance.mCurrentScene, true);

        if (PlayerInput.Instance)
            PlayerInput.Instance.GainControl();
    }

    //Loading becomes visible
    public static IEnumerator FadeSceneOut( FadeType fadeType = FadeType.Black )
    {
        //before fading out, display an end of level text if any
        Instance.DisplayLevelText(Instance.mCurrentScene, false);
        if(Instance.mCurrentScene != SceneNames.Initial)
            yield return new WaitForSeconds(4f);

        CanvasGroup canvasGroup;
        switch ( fadeType )
        {
            case FadeType.Black:
                canvasGroup = Instance.mFaderCanvasGroup;
                break;
            case FadeType.GameOver:
                canvasGroup = Instance.mGameOverCanvasGroup;
                break;
            case FadeType.Win:
                canvasGroup = Instance.mWinCanvasGroup;
                break;
            default:
                canvasGroup = Instance.mLoadingCanvasGroup;
                break;
        }

        canvasGroup.gameObject.SetActive( true );
        yield return Instance.StartCoroutine( Instance.Fade( 1f, canvasGroup ) );
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mCurrentScene = (SceneNames)mNextScene;
        mNextScene++;
        if (mNextScene >= 6)
            mNextScene = 0;

        if (mIsFading)
            StartCoroutine(WaitForFade());
        else
            StartCoroutine(ScreenFader.FadeSceneIn());
        if(mCurrentScene != SceneNames.Initial)
            MusicManagerScript.sInstance.PlayMusic(mNextScene - 1);
    }

    IEnumerator WaitForFade()
    {
        while(mIsFading)
        {
            yield return null;
        }

        yield return StartCoroutine(ScreenFader.FadeSceneIn());
    }

    public IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(1f);

        SceneNames fSceneName = (SceneNames)mNextScene;
        yield return StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.Loading));

        SceneManager.LoadSceneAsync(fSceneName.ToString());
        yield return null;
    }


    void DisplayLevelText(SceneNames pDisplayScene, bool pStartOfLevel)
    {
        int fText = -1;
        switch (pDisplayScene)
        {
            case SceneNames.Scene1:
                SoundManager.sInstance.PlaySound(1);
                if (pStartOfLevel)
                    fText = 0;
                else
                    fText = 1;
                    break;
            case SceneNames.Scene2:
                SoundManager.sInstance.PlaySound(1);
                if (pStartOfLevel)
                    fText = 2;
                else
                    fText = 3;
                break;
            case SceneNames.Scene3:
                    fText = 4;
                    SoundManager.sInstance.PlaySound(1);                
                break;      
            case SceneNames.Scene4:
                //do not play sound at first text of final scene
                if (pStartOfLevel)
                    fText = 5;
                else
                {
                    fText = 6;
                    SoundManager.sInstance.PlaySound(1);
                }
                break;
        }
        TextManager.Instance.DisplayText(fText);
    }
}
