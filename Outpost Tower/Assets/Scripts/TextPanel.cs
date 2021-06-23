using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPanel : MonoBehaviour
{
    public TMP_Text bodyText;
    public GameObject button;
    public TMP_Text buttonText;
    int tutorialIndex = 0;
    bool tutorialFinished;
    bool selectionUp;
    bool hidden;

    [TextArea]
    public List<string> openingText = new List<string>();

    [TextArea] public string defaultConnectionText;
    [TextArea] public string HoverNodeText;
    [TextArea] public string SelectNodeText;
    [TextArea] public string defaultActionText;
    [TextArea] public string medicalText;
    [TextArea] public string supplyText;


    private void OnEnable()
    {
        EventBroker.ResetForNewGame += Reset;
        EventBroker.OnHover += Hover;
        EventBroker.ExitHover += HoverExit;
        EventBroker.SelectionOn += Select;
        EventBroker.SelectionOff += Deselect;
        EventBroker.Connect += ToAction;
        EventBroker.ActionDone += ToConnection;
        EventBroker.AssessingMedical += MedicalUp;
        EventBroker.AssessingSupplies += SupplyUp;
        EventBroker.DisengageMedical += MedSupDown;
        EventBroker.DisengageSupplies += MedSupDown;
    }

    private void OnDisable()
    {
        EventBroker.ResetForNewGame -= Reset;
        EventBroker.OnHover -= Hover;
        EventBroker.ExitHover -= HoverExit;
        EventBroker.SelectionOn -= Select;
        EventBroker.SelectionOff -= Deselect;
        EventBroker.Connect -= ToAction;
        EventBroker.ActionDone -= ToConnection;
        EventBroker.AssessingMedical += MedicalUp;
        EventBroker.AssessingSupplies += SupplyUp;
        EventBroker.DisengageMedical += MedSupDown;
        EventBroker.DisengageSupplies += MedSupDown;
    }

    private void Reset()
    {
        tutorialFinished = false;
        button.SetActive(true);
        buttonText.text = "NEXT";
        tutorialIndex = 0;

        bodyText.text = openingText[tutorialIndex];
    }

    void Hover()
    {
        if (tutorialFinished && GameManager.instance.phase == Phase.Connection && !selectionUp)
        {
            bodyText.text = HoverNodeText;
        }
    }

    void HoverExit()
    {
        if (tutorialFinished && GameManager.instance.phase == Phase.Connection && !selectionUp)
        {
            bodyText.text = defaultConnectionText;
        }
    }

    void Select()
    {
        if (tutorialFinished && GameManager.instance.phase == Phase.Connection)
        {
            selectionUp = true;
            bodyText.text = SelectNodeText;
        }
    }

    void Deselect()
    {
        if (tutorialFinished && GameManager.instance.phase == Phase.Connection)
        {
            selectionUp = false;
            bodyText.text = defaultConnectionText;
        }
    }

    void ToConnection()
    {
        bodyText.text = defaultConnectionText;
    }

    void ToAction()
    {
        bodyText.text = defaultActionText;
    }

    void MedicalUp()
    {
        bodyText.text = medicalText;
    }

    void SupplyUp()
    {
        bodyText.text = supplyText;
    }

    void MedSupDown()
    {
        if (GameManager.instance.phase == Phase.Action)
            bodyText.text = defaultActionText;
        else
            bodyText.text = "";
    }


    public void ButtonAction()
    {
        if (!tutorialFinished)
        {
            tutorialIndex++;
            if (tutorialIndex < openingText.Count)
                bodyText.text = openingText[tutorialIndex];
            else
            {
                tutorialFinished = true;
                buttonText.text = "HIDE";
                bodyText.text = defaultConnectionText;

                EventBroker.StartPlayCall();
            }
        }
        else
        {
            if (!hidden)
            {
                hidden = true;
                bodyText.gameObject.SetActive(false);
                buttonText.text = "HELP";
            }
            else
            {
                hidden = false;
                bodyText.gameObject.SetActive(true);
                buttonText.text = "HIDE";
            }
        }

    }

}
