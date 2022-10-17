using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Errors
{
    GooglePlayConnectionNeeded,
    InternetConnectionNeededGeneric,
    InternetConnectionInBalanceScene,
}

public class ErrorUi : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    GameObject errorSign;

    [SerializeField][TextArea] string googlePlayConnectionNeededDescription;
    [SerializeField][TextArea] string internetConnectionNeededDescription;
    [SerializeField][TextArea] string internetConnectionInBalanceSceneDescription;
    [SerializeField][TextArea] string defaultDescription;

    private void Awake()
    {
        errorSign = transform.GetChild(0).gameObject;
    }

    public void ShowError(Errors error)
    {
        errorSign.SetActive(true);

        switch (error)
        {   
            case Errors.GooglePlayConnectionNeeded:
                text.text = googlePlayConnectionNeededDescription;
                break;
            case Errors.InternetConnectionNeededGeneric:
                text.text = internetConnectionNeededDescription;
                break;
            case Errors.InternetConnectionInBalanceScene:
                text.text = internetConnectionInBalanceSceneDescription;
                break;
            default:
                text.text = defaultDescription;
                break;
        }
    }

    public void Return()
    {
        errorSign.SetActive(false);
    }
}