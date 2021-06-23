using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Audio;
using TMPro;

public enum Phase {Connection, Action};
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public ColorPalette colorDefaults;
    public bool isNewGame;
    public bool gameActive;

    //basic rules
    public bool usingNames;
    public bool usingListenFeature;

    public int round;
    public Phase phase;

    public int signalStrengthRemaining;
    public int totalSignalStrength;
    public int baseSignalStrength;
    public int usedSignalStrength;
    public int bonusSignalStrength;
    public int startingSignalStrength = 0;
    public int signalStrengthPerRound = 1;

    public int medicalRemaining;
    public int totalMedical;
    public int suppliesRemaining;
    public int totalSupplies;
    public int lowestSupply;

    public TMP_Text signalAmtText;
    public TMP_Text medAmtText;
    public TMP_Text suppAmtText;
    
    public GameObject cursorObject;

    public List<Node> currentNetworkNodes = new List<Node>();
    public List<Connector> currentNetworkConnectors = new List<Connector>();
    public Node focusedNode;
    public GameObject Nodes;
    public int nodeCount;

    
    
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        EventBroker.AddConnector += AddToUsedSignalStrength;
        EventBroker.RemoveConnector += SubtractUsedSignalStrength;
        EventBroker.AddConnector += UpdateResourcesForConnections;
        EventBroker.RemoveConnector += UpdateResourcesForConnections;
        EventBroker.UseMedical += UseMedical;
        EventBroker.UseSupplies += UseSupplies;
        EventBroker.StartPlay += StartPlay;
        EventBroker.CheckWin += CheckWin;
    }

    private void OnDisable()
    {
        EventBroker.AddConnector -= AddToUsedSignalStrength;
        EventBroker.RemoveConnector -= SubtractUsedSignalStrength;
        EventBroker.AddConnector -= UpdateResourcesForConnections;
        EventBroker.RemoveConnector -= UpdateResourcesForConnections;
        EventBroker.UseMedical -= UseMedical;
        EventBroker.UseSupplies -= UseSupplies;
        EventBroker.StartPlay -= StartPlay;
        EventBroker.CheckWin -= CheckWin;
    }

    private void Start()
    {
        if (isNewGame)
            SetNewGame();

        nodeCount = Nodes.transform.childCount;
    }

    void SetNewGame()
    {
        round = 1;
        phase = Phase.Connection;
        bonusSignalStrength = 0;
        usedSignalStrength = 0;
        UpdateSignalStrength();
        focusedNode = null;
        
        EventBroker.ResetForNewGameCall();
    }

    void StartPlay()
    {
        gameActive = true;
        cursorObject.SetActive(true);
    }

    void UpdateSignalStrength()
    {
        baseSignalStrength = startingSignalStrength + (round * signalStrengthPerRound);
        totalSignalStrength = baseSignalStrength + bonusSignalStrength;
        signalStrengthRemaining = totalSignalStrength - usedSignalStrength;

        // if (signalStrengthRemaining == 0)
        //     EventBroker.ConnectionPrimedCall();

        signalAmtText.text = signalStrengthRemaining.ToString() + " / " + totalSignalStrength.ToString();
    }

    void AddToUsedSignalStrength()
    {
        usedSignalStrength++;
        UpdateSignalStrength();
    }

    void SubtractUsedSignalStrength()
    {
        usedSignalStrength--;
        UpdateSignalStrength();
    }

    void UpdateResourceText()
    {
        medAmtText.text = medicalRemaining + " / " + totalMedical;
        suppAmtText.text = suppliesRemaining + " / " + totalSupplies;
    }

    public bool HealingNeeded()
    {
        foreach (Node node in currentNetworkNodes)
        {
            if (node.type == NodeType.Civilian && node.medicalCount < node.medicalMax)
            {
                return true;
            }
        }
        return false;
    }

    public void MedicalActionOn()
    {
        Node.assessingMedical = true;
        EventBroker.AssessingMedicalCall();
    }

    public void MedicalActionOff()
    {
        Node.assessingMedical = false;
        EventBroker.DisengageMedicalCall();
    }

    public void SuppliesActionOn()
    {
        Node.assessingSupplies = true;
        GetLowestSupply();
        EventBroker.AssessingSuppliesCall();
    }

    void GetLowestSupply()
    {
        int lowestNum = 100;

        foreach (Node node in currentNetworkNodes)
        {
            if (node.type == NodeType.Civilian)
            {
                lowestNum = Mathf.Min(lowestNum, node.supplyCount);
            }
        }

        lowestSupply = lowestNum;
    }

    public void SuppliesActionOff()
    {
        Node.assessingSupplies = false;
        EventBroker.DisengageSuppliesCall();
    }

    void UseMedical()
    {
        medicalRemaining--;
        if (medicalRemaining <= 0)
            EventBroker.DisengageMedicalCall();
        UpdateResourceText();
        EventBroker.UpdateActionsCall();
        EventBroker.DisplayNodeCall();
    }

    void UseSupplies()
    {
        suppliesRemaining--;
        if (suppliesRemaining <= 0)
            EventBroker.DisengageSuppliesCall();
        GetLowestSupply();
        UpdateResourceText();
        EventBroker.UpdateLowestSupplyCall();
        EventBroker.UpdateActionsCall();
        EventBroker.DisplayNodeCall();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            EventBroker.MouseDownCall();
        else if (Input.GetMouseButtonUp(0))
            EventBroker.MouseUpCall();
    }

    public void CompletePhase()
    {
        switch (phase)
        {
            case Phase.Connection:
            {
                phase = Phase.Action;
                foreach (Connector connector in currentNetworkConnectors)
                {
                    connector.SetColor(colorDefaults.LiveConnectionOutlineColor);
                }
                UpdateResourcesForConnections();
                EventBroker.ConnectCall();
                break;
            }

            case Phase.Action:
            {
                Node.assessingMedical = false;
                Node.assessingSupplies = false;
                phase = Phase.Connection;
                foreach (Connector connector in currentNetworkConnectors)
                {
                    connector.SetColor(colorDefaults.EngagedColor);
                }
                round++;
                UpdateSignalStrength();
                EventBroker.ActionDoneCall();
                break;
            }
        }
        
    }

    void CheckWin()
    {
        if (currentNetworkNodes.Count == nodeCount)
            EventBroker.WinCall();
    }

    void UpdateResourcesForConnections()
    {
        int medTally = 0;
        int supplyTally = 0;
        int signalTally = 0;

        foreach (Node node in currentNetworkNodes)
        {
            if (phase == Phase.Action || node.activated)
            {
                switch (node.type)
                {
                    case NodeType.Medical:
                    {
                        medTally += node.resourceAmount;
                        break;
                    }
                    case NodeType.Supply:
                    {
                        supplyTally += node.resourceAmount;
                        break;
                    }
                    case NodeType.Energy:
                    {
                        signalTally += node.resourceAmount;
                        break;
                    }
                }
            }
        }

        totalMedical = medTally;
        totalSupplies = supplyTally;
        medicalRemaining = medTally;
        suppliesRemaining = supplyTally;

        UpdateResourceText();

        if (bonusSignalStrength != signalTally)
        {
            bonusSignalStrength = signalTally;
            UpdateSignalStrength();
        }
    }
}
