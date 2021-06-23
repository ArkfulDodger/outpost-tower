using System;

public static class EventBroker
{
    public static event Action ResetForNewGame;

    public static void ResetForNewGameCall()
    {
        if (ResetForNewGame != null)
            ResetForNewGame();
    }

    public static event Action MouseDown;

    public static void MouseDownCall()
    {
        if (MouseDown != null)
            MouseDown();
    }

    public static event Action MouseUp;

    public static void MouseUpCall()
    {
        if (MouseUp != null)
            MouseUp();
    }

    public static event Action NodeSelectionChange;

    public static void NodeSelectionChangeCall()
    {
        if (NodeSelectionChange != null)
            NodeSelectionChange();
    }

    public static event Action Connect;

    public static void ConnectCall()
    {
        if (Connect != null)
            Connect();
    }

    public static event Action ActionDone;

    public static void ActionDoneCall()
    {
        if (ActionDone != null)
            ActionDone();
    }

    public static event Action DisplayNode;

    public static void DisplayNodeCall()
    {
        if (DisplayNode != null)
            DisplayNode();
    }

    public static event Action AddConnector;

    public static void AddConnectorCall()
    {
        if (AddConnector != null)
            AddConnector();
    }

    public static event Action RemoveConnector;

    public static void RemoveConnectorCall()
    {
        if (RemoveConnector != null)
            RemoveConnector();
    }

    public static event Action UseMedical;

    public static void UseMedicalCall()
    {
        if (UseMedical != null)
            UseMedical();
    }

    public static event Action UseSupplies;

    public static void UseSuppliesCall()
    {
        if (UseSupplies != null)
            UseSupplies();
    }

    public static event Action AssessingMedical;

    public static void AssessingMedicalCall()
    {
        if (AssessingMedical != null)
            AssessingMedical();
    }

    public static event Action AssessingSupplies;

    public static void AssessingSuppliesCall()
    {
        if (AssessingSupplies != null)
            AssessingSupplies();
    }

    public static event Action DisengageMedical;

    public static void DisengageMedicalCall()
    {
        if (DisengageMedical != null)
            DisengageMedical();
    }

    public static event Action DisengageSupplies;

    public static void DisengageSuppliesCall()
    {
        if (DisengageSupplies != null)
            DisengageSupplies();
    }

    public static event Action UpdateActions;

    public static void UpdateActionsCall()
    {
        if (UpdateActions != null)
            UpdateActions();
    }

    public static event Action UpdateLowestSupply;

    public static void UpdateLowestSupplyCall()
    {
        if (UpdateLowestSupply != null)
            UpdateLowestSupply();
    }

    public static event Action ConnectionPrimed;

    public static void ConnectionPrimedCall()
    {
        if (ConnectionPrimed != null)
            ConnectionPrimed();
    }

    public static event Action StartPlay;

    public static void StartPlayCall()
    {
        if (StartPlay != null)
            StartPlay();
    }

    public static event Action OnHover;

    public static void OnHoverCall()
    {
        if (OnHover != null)
            OnHover();
    }

    public static event Action ExitHover;

    public static void ExitHoverCall()
    {
        if (ExitHover != null)
            ExitHover();
    }
    public static event Action SelectionOn;

    public static void SelectionOnCall()
    {
        if (SelectionOn != null)
            SelectionOn();
    }

    public static event Action SelectionOff;

    public static void SelectionOffCall()
    {
        if (SelectionOff != null)
            SelectionOff();
    }

    public static event Action Death;

    public static void DeathCall()
    {
        if (Death != null)
            Death();
    }


    public static event Action CheckWin;

    public static void CheckWinCall()
    {
        if (CheckWin != null)
            CheckWin();
    }


    public static event Action Win;

    public static void WinCall()
    {
        if (Win != null)
            Win();
    }
}
