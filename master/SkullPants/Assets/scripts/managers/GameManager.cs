using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    //Scene Transition Game Manager Section
    protected static GameManager sInstance;

    //public SceneTransitionDestination mInitialSceneTransitionDestination;

    protected Scene mCurrentZoneScene;
    //protected SceneTransitionDestination.DestinationTag mZoneRestartDestinationTag;
    public bool mTransitioning;

    //Async loads load the screen fader and tranistion to the next scene.
    public static GameManager Instance
    {
        get
        {
            if (sInstance != null)
                return sInstance;

            sInstance = FindObjectOfType<GameManager>();

            if (sInstance != null)
                return sInstance;

            Create();

            return sInstance;
        }
    }

    public static GameManager Create()
    {
        GameObject sceneControllerGameObject = new GameObject("GameManager");
        sInstance = sceneControllerGameObject.AddComponent<GameManager>();

        return sInstance;
    }

    void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        //mPlayerInput = FindObjectOfType<PlayerInput>();

        ScreenFader.SetAlpha(1f);
        StartCoroutine(ScreenFader.FadeSceneIn());
        mCurrentZoneScene = SceneManager.GetActiveScene();
        /*
        if (mInitialSceneTransitionDestination != null)
        {
            //SetEnteringGameObjectLocation(mInitialSceneTransitionDestination);
            ScreenFader.SetAlpha(1f);
            StartCoroutine(ScreenFader.FadeSceneIn());
            //mInitialSceneTransitionDestination.OnReachDestination.Invoke();
        }
        else
        {
            mCurrentZoneScene = SceneManager.GetActiveScene();
            //mZoneRestartDestinationTag = SceneTransitionDestination.DestinationTag.A;
        }
        */
    }

    public static void TransitionToScene(TransitionPoint transitionPoint)
    {
        //Instance.StartCoroutine(Instance.Transition(transitionPoint.newSceneName, transitionPoint.resetInputValuesOnTransition, transitionPoint.transitionDestinationTag, transitionPoint.transitionType));
    }

    /*
    public static SceneTransitionDestination GetDestinationFromTag(SceneTransitionDestination.DestinationTag destinationTag)
    {
        return Instance.GetDestination(destinationTag);
    }
    */

    protected IEnumerator Transition(string newSceneName, bool resetInputValues, /*SceneTransitionDestination.DestinationTag destinationTag,*/ TransitionPoint.TransitionType transitionType = TransitionPoint.TransitionType.DifferentZone)
    {
        mTransitioning = true;
        //PersistentDataManager.SaveAllData();

        //if (mPlayerInput == null)
        //    mPlayerInput = FindObjectOfType<PlayerInput>();
        //mPlayerInput.ReleaseControl(resetInputValues);
        yield return StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.Loading));
        //PersistentDataManager.ClearPersisters();
        yield return SceneManager.LoadSceneAsync(newSceneName);
        //m_PlayerInput = FindObjectOfType<PlayerInput>();
        //m_PlayerInput.ReleaseControl(resetInputValues);
        //PersistentDataManager.LoadAllData();
        //SceneTransitionDestination entrance = GetDestination(destinationTag);
        //SetEnteringGameObjectLocation(entrance);

        //SetupNewScene(transitionType, entrance);
        //if (entrance != null)
        //    entrance.OnReachDestination.Invoke();

        yield return StartCoroutine(ScreenFader.FadeSceneIn());

        //mPlayerInput.GainControl();

        mTransitioning = false;
    }
}
