using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance;

    public Animator mAnimator;

    [TextArea(3,10)]
    public string[] mStageTexts;

    public Text mText;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        //mAnimator.SetBool("FadeIn", false);
        DontDestroyOnLoad(gameObject);
    }


    public void DisplayText(int pTextToDisplay)
    {
        if (pTextToDisplay < 0)
            return;
        mAnimator.SetBool("FadeIn", true);
        StopAllCoroutines();
        StartCoroutine(TypeByChar(mStageTexts[pTextToDisplay]));
    }

    IEnumerator TypeByChar(string pScentence)
    {
        mText.text = "";
        foreach(char fLetter in pScentence.ToCharArray())
        {
            mText.text += fLetter;
            yield return null;
        }

        yield return new WaitForSeconds(3f);
        mAnimator.SetBool("FadeIn", false);
    }
}
