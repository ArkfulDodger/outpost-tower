using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDUpdater : MonoBehaviour
{
    public TMP_Text DayPhaseText;
    public TMP_Text ButtonText;
    public TMP_Text SignalText;
    public TMP_Text MedicalText;
    public TMP_Text SuppliesText;
    public TMP_Text SignalAmountText;
    public TMP_Text MedicalAmountText;
    public TMP_Text SuppliesAmountText;
    public GameObject ActionsPanel;
    public GameObject DeathPanel;
    public GameObject WinPanel;

    public GameObject ButtonContainer;
    public TMP_Text ActionInstructionText;


    // Node Display Items
    public GameObject iconContainer;
    public Image iconImage;
    public IconList iconList;
    public GameObject typeContainer;
    public TMP_Text typeText;
    public GameObject nameContainer;
    public TMP_Text nameText;
    public GameObject connectionContainer;
    public TMP_Text connectionText;
    public GameObject resourcesContainer;
    public TMP_Text resourcesText;
    public GameObject statusContainer;
    public TMP_Text statusHeaderText;
    public TMP_Text medicalStatusText;
    public TMP_Text suppliesStatusText;
    public GameObject listenButton;


    // Action Display Items
    public GameObject medicalButton;
    public GameObject suppliesButton;


    // Audio
    public AudioSource audioSource;
    public AudioClip changePhase;
    public AudioClip connectSound;
    public AudioClip disconnectSound;
    public AudioClip supplymedSound;





    private void OnEnable()
    {
        EventBroker.Connect += ToActionPhase;
        EventBroker.ActionDone += ToConnectionPhase;
        EventBroker.ResetForNewGame += ToConnectionPhase;
        EventBroker.ResetForNewGame += UpdateNodeDisplay;
        EventBroker.DisplayNode += UpdateNodeDisplay;
        EventBroker.AddConnector += UpdateNodeDisplay;
        EventBroker.RemoveConnector += UpdateNodeDisplay;
        EventBroker.UpdateActions += UpdateActionDisplay;
        EventBroker.AddConnector += ConnectButtonIn;
        EventBroker.Death += Death;
        EventBroker.Win+= Win;
        EventBroker.AddConnector += ConnectNoise;
        EventBroker.RemoveConnector += DisconnectNoise;
        EventBroker.UseMedical += SupplyUse;
        EventBroker.UseSupplies += SupplyUse;
    }

    private void OnDisable()
    {
        EventBroker.Connect -= ToActionPhase;
        EventBroker.ActionDone -= ToConnectionPhase;
        EventBroker.ResetForNewGame -= ToConnectionPhase;
        EventBroker.ResetForNewGame -= UpdateNodeDisplay;
        EventBroker.DisplayNode -= UpdateNodeDisplay;
        EventBroker.AddConnector -= UpdateNodeDisplay;
        EventBroker.RemoveConnector -= UpdateNodeDisplay;
        EventBroker.UpdateActions -= UpdateActionDisplay;
        EventBroker.AddConnector -= ConnectButtonIn;
        EventBroker.Death -= Death;
        EventBroker.Win -= Win;
        EventBroker.AddConnector -= ConnectNoise;
        EventBroker.RemoveConnector -= DisconnectNoise;
        EventBroker.UseMedical -= SupplyUse;
        EventBroker.UseSupplies -= SupplyUse;
    }

    void ToActionPhase()
    {
        DayPhaseText.text = "Day " + GameManager.instance.round + ": Action Phase";
        ButtonText.text = "END DAY";
        SignalText.color = GameManager.instance.colorDefaults.EngagedColor;
        SignalText.fontStyle = FontStyles.Normal;
        MedicalText.color = Color.white;
        MedicalText.fontStyle = FontStyles.Bold;
        SuppliesText.color = Color.white;
        SuppliesText.fontStyle = FontStyles.Bold;
        SignalAmountText.color = GameManager.instance.colorDefaults.EngagedColor;
        SignalAmountText.fontStyle = FontStyles.Normal;
        MedicalAmountText.color = Color.white;
        MedicalAmountText.fontStyle = FontStyles.Bold;
        SuppliesAmountText.color = Color.white;
        SuppliesAmountText.fontStyle = FontStyles.Bold;
        if (GameManager.instance.medicalRemaining > 0 || GameManager.instance.suppliesRemaining > 0)
            ActionInstructionText.text = "Select a Civilian";
        else
            ActionInstructionText.text = "No Resources Remain Today";
        ActionsPanel.SetActive(true);
        UpdateActionDisplay();
        audioSource.PlayOneShot(changePhase);
    }

    void ToConnectionPhase()
    {
        DayPhaseText.text = "Day " + GameManager.instance.round + ": Connection Phase";
        ButtonText.text = "CONNECT";
        ButtonContainer.SetActive(false);
        SignalText.color = Color.white;
        SignalText.fontStyle = FontStyles.Bold;
        MedicalText.color = GameManager.instance.colorDefaults.EngagedColor;
        MedicalText.fontStyle = FontStyles.Normal;
        SuppliesText.color = GameManager.instance.colorDefaults.EngagedColor;
        SuppliesText.fontStyle = FontStyles.Normal;
        SignalAmountText.color = Color.white;
        SignalAmountText.fontStyle = FontStyles.Bold;
        MedicalAmountText.color = GameManager.instance.colorDefaults.EngagedColor;
        MedicalAmountText.fontStyle = FontStyles.Normal;
        SuppliesAmountText.color = GameManager.instance.colorDefaults.EngagedColor;
        SuppliesAmountText.fontStyle = FontStyles.Normal;
        ActionsPanel.SetActive(false);
        DeathPanel.SetActive(false);
        WinPanel.SetActive(false);
        audioSource.PlayOneShot(changePhase);
    }

    void ConnectNoise()
    {
        audioSource.PlayOneShot(connectSound);
    }

    void DisconnectNoise()
    {
        audioSource.PlayOneShot(disconnectSound);
    }

    void Death()
    {
        DeathPanel.SetActive(true);
        audioSource.PlayOneShot(changePhase);
    }

    void Win()
    {
        WinPanel.SetActive(true);
        audioSource.PlayOneShot(changePhase);
    }

    void SupplyUse()
    {
        audioSource.PlayOneShot(supplymedSound);
    }

    void ConnectButtonIn()
    {
        ButtonContainer.SetActive(true);
    }

    void UpdateActionDisplay()
    {
        if (GameManager.instance.phase == Phase.Action)
        {
            // if out of resources, say so
            if (GameManager.instance.medicalRemaining == 0 && GameManager.instance.suppliesRemaining == 0)
            {
                ActionInstructionText.gameObject.SetActive(true);
                ActionInstructionText.text = "No Resources Remain Today";

                medicalButton.SetActive(false);
                suppliesButton.SetActive(false);
            }
            else
            {
                ActionInstructionText.gameObject.SetActive(false);

                if (GameManager.instance.suppliesRemaining > 0)
                    suppliesButton.SetActive(true);
                else
                {
                    suppliesButton.SetActive(false);
                    EventBroker.DisengageSuppliesCall();
                }

                if (GameManager.instance.medicalRemaining > 0 && GameManager.instance.HealingNeeded())
                    medicalButton.SetActive(true);
                else
                {
                    medicalButton.SetActive(false);
                    EventBroker.DisengageMedicalCall();
                }
            }
        }
    }

    void UpdateNodeDisplay()
    {
        Node node = GameManager.instance.focusedNode;

        // if (GameManager.instance.phase == Phase.Action)
        //     UpdateActionDisplay();

        // turn display on or off depending on presence of node
        if (node == null)
        {
            TurnOffSideDisplay();
            return;
        }
        else
        {
            TurnOnSideDisplay(node);
        }

        // get icon
        UpdateIcon(node);

        // get title
        typeText.text = node.activated ? node.type.ToString() : "Unknown";

        // can return early here if node not activated
        if (!node.activated)
            return;

        // set display name
        nameText.text = node.displayName;

        // set connection text
        SetConnectionText(node);

        // set resources text
        SetResourcesText(node);

        // set status text
        SetStatusText(node);
    }

    void TurnOffSideDisplay()
    {
        iconContainer.SetActive(false);
        typeContainer.SetActive(false);
        nameContainer.SetActive(false);
        connectionContainer.SetActive(false);
        resourcesContainer.SetActive(false);
        statusContainer.SetActive(false);
        listenButton.SetActive(false);
    }

    void TurnOnSideDisplay(Node node)
    {
        // use an icon and type for all nodes
        iconContainer.SetActive(true);
        typeContainer.SetActive(true);

        // only include more details on activated nodes
        if (node.activated)
        {
            // include name field if names are in play
            nameContainer.SetActive(GameManager.instance.usingNames? true : false);

            // include connection status if not Home
            connectionContainer.SetActive(node.type != NodeType.Home ? true : false);

            // inlude Resources for all non-Civilian nodes
            resourcesContainer.SetActive(node.type != NodeType.Civilian ? true: false);

            // include Status for Civilian only
            statusContainer.SetActive(node.type == NodeType.Civilian ? true : false);

            // include Listen if non-Home and listen feature is in use
            listenButton.SetActive(node.type != NodeType.Home && GameManager.instance.usingListenFeature ? true : false);
        }
        else
        {
            nameContainer.SetActive(false);
            connectionContainer.SetActive(false);
            resourcesContainer.SetActive(false);
            statusContainer.SetActive(false);
            listenButton.SetActive(false);
        }

    }

    void UpdateIcon(Node node)
    {
        if (node.activated)
        {
            foreach (var icon in iconList.icons)
            {
                if (icon.type == node.type)
                {
                    iconImage.sprite = icon.sprite;
                    return;
                }
            }
        }
        else
        {
            iconImage.sprite = iconList.icons[iconList.icons.Count - 1].sprite;
        }
    }

    void SetConnectionText(Node node)
    {
        if (!node.engaged)
            connectionText.text = "Not Connected";
        else if (node.engaged && !node.connected && !node.dead)
            connectionText.text = "Connecting...";
        else if (node.connected && !node.dead)
            connectionText.text = "Connected";
        else if (node.dead)
            connectionText.text = "No Response";
    }

    void SetResourcesText(Node node)
    {
        if (node.type == NodeType.Civilian)
            return;

        string description;

        switch (node.type)
        {
            case NodeType.Home:
            {
                description = "Signal Strength increases by " + GameManager.instance.signalStrengthPerRound.ToString() + " each day";
                break;
            }

            case NodeType.Medical:
            {
                description = node.resourceAmount.ToString() + " Medical per day";
                break;
            }

            case NodeType.Mobile:
            {
                description = "Can change location to bridge connection gaps";
                break;
            }

            case NodeType.Supply:
            {
                description = node.resourceAmount.ToString() + " Supplies per day";
                break;
            }

            case NodeType.Weather:
            {
                description = "Advance storm notice";
                break;
            }

            case NodeType.Energy:
            {
                description = "Additional " + node.resourceAmount.ToString() + " Signal Strength while connected";
                break;
            }

            default:
            {
                description = "";
                break;
            }
        }

        resourcesText.text = description;
    }

    void SetStatusText(Node node)
    {
        // set header based on whether currently connected
        statusHeaderText.text = node.connected && !node.dead ? "STATUS": "STATUS (Last Known)";

        int medCount = node.connected && !node.dead ? node.medicalCount : node.lastMedicalCount;
        string medDesc;

        switch (medCount)
        {
            case 5:
            {
                medDesc = "Good";
                break;
            }
            case 4:
            {
                medDesc = "Poor";
                break;
            }

            case 3:
            {
                medDesc = "Bad";
                break;
            }

            case 2:
            {
                medDesc = "Urgent";
                break;
            }

            case 1:
            {
                medDesc = "Dying";
                break;
            }

            default:
            {
                medDesc = "???";
                break;
            }
        }

        medicalStatusText.text = "Health: " + medDesc;


        int supplyNum = node.connected && !node.dead ? node.supplyCount : node.lastSupplyCount;
        string dayWord = supplyNum == 1 ? "Day" : "Days";

        if (supplyNum >= 0)
            suppliesStatusText.text = "Supplies: " + supplyNum.ToString() + " " + dayWord;
        else
            suppliesStatusText.text = "Supplies: ???";
    }
}
