using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static IEnumerator FadeSceneIn()
    {
        CanvasGroup canvasGroup;
        if ( Instance.mFaderCanvasGroup.alpha > 0.1f )
            canvasGroup = Instance.mFaderCanvasGroup;
        else
            canvasGroup = Instance.mLoadingCanvasGroup;

        yield return Instance.StartCoroutine( Instance.Fade( 0f, canvasGroup ) );

        canvasGroup.gameObject.SetActive( false );
    }

    public static IEnumerator FadeSceneOut( FadeType fadeType = FadeType.Black )
    {
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
}
