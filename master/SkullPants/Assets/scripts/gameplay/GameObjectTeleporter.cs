using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectTeleporter : MonoBehaviour
{
    protected static GameObjectTeleporter mInstance;

    //protected PlayerInput mPlayerInput;
    protected bool mTransitioning;

    public static GameObjectTeleporter Instance
    {
        get
        {
            if (mInstance != null)
                return mInstance;

            mInstance = FindObjectOfType<GameObjectTeleporter>();

            if (mInstance != null)
                return mInstance;

            GameObject gameObjectTeleporter = new GameObject("GameObjectTeleporter");
            mInstance = gameObjectTeleporter.AddComponent<GameObjectTeleporter>();

            return mInstance;
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

        //m_PlayerInput = FindObjectOfType<PlayerInput>();
    }

    public static bool Transitioning
    {
        get { return Instance.mTransitioning; }
    }


    public static void Teleport(TransitionPoint transitionPoint)
    {
        /*
        Transform destinationTransform = Instance.GetDestination(transitionPoint.transitionDestinationTag).transform;
        Instance.StartCoroutine(Instance.Transition(transitionPoint.transitioningGameObject, true, transitionPoint.resetInputValuesOnTransition, destinationTransform.position, true));
        */
    }

    public static void Teleport(GameObject transitioningGameObject, Transform destination)
    {
        Instance.StartCoroutine(Instance.Transition(transitioningGameObject, false, false, destination.position, false));
    }

    public static void Teleport(GameObject transitioningGameObject, Vector3 destinationPosition)
    {
        Instance.StartCoroutine(Instance.Transition(transitioningGameObject, false, false, destinationPosition, false));
    }

    protected IEnumerator Transition(GameObject transitioningGameObject, bool releaseControl, bool resetInputValues, Vector3 destinationPosition, bool fade)
    {
        mTransitioning = true;

        if (releaseControl)
        {
            /*
            if (m_PlayerInput == null)
                m_PlayerInput = FindObjectOfType<PlayerInput>();
            m_PlayerInput.ReleaseControl(resetInputValues);
            */
        }

        if (fade)
            yield return StartCoroutine(ScreenFader.FadeSceneOut());

        transitioningGameObject.transform.position = destinationPosition;

        if (fade)
            yield return StartCoroutine(ScreenFader.FadeSceneIn());

        if (releaseControl)
        {
            /*
            m_playerInput.GainControl();
            */
        }

        mTransitioning = false;
    }

    /*
    protected SceneTransitionDestination GetDestination(SceneTransitionDestination.DestinationTag destinationTag)
    {
        SceneTransitionDestination[] entrances = FindObjectsOfType<SceneTransitionDestination>();
        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
                return entrances[i];
        }
        Debug.LogWarning("No entrance was found with the " + destinationTag + " tag.");
        return null;
    }
    */
}
