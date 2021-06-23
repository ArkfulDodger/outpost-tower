using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum NodeType {Home, Civilian, Medical, Supply, Weather, Energy, Mobile, Unknown};
public class Node : MonoBehaviour
{
    public NodeType type;
    public bool startActive;
    public string displayName;
    public bool dead;
    public static bool Win;
    public int resourceAmount = 3;

    // Status Variables
    public int medicalMax = 5;
    public int medicalCount = 5;
    public int supplyCount = 15;
    public int lastMedicalCount = -10;
    public int lastSupplyCount = -10;
    public int lowestSupply;

    // Game Variable Rules
    int civilianMinSupply = 0;
    int civilianMaxSupply = 15;
    int hospitalMin = 1;
    int hospitalMax = 8;
    int supplyMin = 3;
    int supplyMax = 8;
    int energyMin = 2;
    int energyMax = 5;


    public static bool assessingMedical;
    public static bool assessingSupplies;

    GameObject badge;
    SpriteRenderer badgeRenderer;
    SpriteRenderer iconRenderer;
    SpriteRenderer badgeOutlineRenderer;
    GameObject defaultNode;
    SpriteRenderer defaultNodeRenderer;
    SpriteRenderer defaultNodeOutlineRenderer;
    SpriteRenderer radiusRenderer;

    CircleCollider2D activeCollider;
    CircleCollider2D nodeCollider;
    CircleCollider2D badgeCollider;

    Animator animator;

    public float radius;

    public bool activated;
    public bool hovered;
    public bool selected;
    public static bool selectionMade;
    public static Node selectedNode;
    public static float activeRadius;
    public bool eligibleTarget;
    public bool engaged;
    public bool connected;

    public List<Node> establishedConnectedNodes = new List<Node>();
    public Dictionary<Node, Connector> establishedConnections = new Dictionary<Node, Connector>();
    public List<Node> tempConnectedNodes = new List<Node>();
    public Dictionary<Node, Connector> tempConnections = new Dictionary<Node, Connector>();
    public List<Node> sourceNodes = new List<Node>();
    public List<Node> receiverNodes = new List<Node>();


    private void Awake()
    {
        // Get all node object references
        badge = transform.Find("Badge").gameObject;
        badgeRenderer = badge.GetComponent<SpriteRenderer>();
        iconRenderer = badge.transform.Find("Icon").GetComponent<SpriteRenderer>();
        badgeOutlineRenderer = badge.transform.Find("Outline").GetComponent<SpriteRenderer>();
        defaultNode = transform.Find("DefaultNode").gameObject;
        defaultNodeRenderer = defaultNode.GetComponent<SpriteRenderer>();
        defaultNodeOutlineRenderer = defaultNode.transform.Find("Outline").GetComponent<SpriteRenderer>();
        radiusRenderer = transform.parent.Find("Radius").GetComponent<SpriteRenderer>();
        nodeCollider = defaultNode.GetComponent<CircleCollider2D>();
        badgeCollider = badge.GetComponent<CircleCollider2D>();
        activeCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        activeCollider.isTrigger = true;

        radius = transform.parent.Find("Radius").localScale.x * transform.parent.Find("Radius").GetComponent<CircleCollider2D>().radius;
    }

    private void OnEnable()
    {
        EventBroker.ResetForNewGame += Reset;
        EventBroker.MouseDown += ClickResponse;
        EventBroker.Connect += ActivateConnection;
        EventBroker.NodeSelectionChange += SetEligibility;
        EventBroker.ActionDone += DeactivateConnection;
        EventBroker.AssessingMedical += AssessingMedical;
        EventBroker.AssessingSupplies += AssessingSupplies;
        EventBroker.UpdateLowestSupply += UpdateLowestSupply;
        EventBroker.DisengageMedical += StopAssessingMedical;
        EventBroker.DisengageSupplies += StopAssessingSupplies;
    }

    private void OnDisable()
    {
        EventBroker.ResetForNewGame -= Reset;
        EventBroker.MouseDown -= ClickResponse;
        EventBroker.Connect -= ActivateConnection;
        EventBroker.NodeSelectionChange -= SetEligibility;
        EventBroker.ActionDone -= DeactivateConnection;
        EventBroker.AssessingMedical -= AssessingMedical;
        EventBroker.AssessingSupplies -= AssessingSupplies;
        EventBroker.UpdateLowestSupply -= UpdateLowestSupply;
        EventBroker.DisengageMedical += StopAssessingMedical;
        EventBroker.DisengageSupplies += StopAssessingSupplies;
    }

    // Start is called before the first frame update
    void Start()
    {
        activeCollider.isTrigger = false;
        activeCollider.isTrigger = true;

        SetVariables();
    }

    void SetVariables()
    {
        switch (type)
        {
            case NodeType.Civilian:
            {
                float baseNum = Mathf.Lerp(civilianMinSupply, civilianMaxSupply, Vector3.Distance(Vector3.zero, transform.position)/17f);
                int variance = Random.Range(-1, 2);
                int finalNum = Mathf.RoundToInt(baseNum + variance);

                supplyCount = Mathf.Clamp(finalNum, civilianMinSupply, civilianMaxSupply);

                break;
            }

            case NodeType.Medical:
            {
                float baseNum = Mathf.Lerp(hospitalMin, hospitalMax, Vector3.Distance(Vector3.zero, transform.position)/17f);
                int variance = Random.Range(-1, 2);
                int finalNum = Mathf.RoundToInt(baseNum + variance);

                resourceAmount = Mathf.Clamp(finalNum, hospitalMin, hospitalMax);
                
                break;
            }

            case NodeType.Supply:
            {
                float baseNum = Mathf.Lerp(supplyMin, supplyMax, Vector3.Distance(Vector3.zero, transform.position)/17f);
                int variance = Random.Range(-1, 2);
                int finalNum = Mathf.RoundToInt(baseNum + variance);

                resourceAmount = Mathf.Clamp(finalNum, supplyMin, supplyMax);
                
                break;
            }

            case NodeType.Energy:
            {
                float baseNum = Mathf.Lerp(energyMin, energyMax, Vector3.Distance(Vector3.zero, transform.position)/17f);
                int variance = Random.Range(-1, 2);
                int finalNum = Mathf.RoundToInt(baseNum + variance);

                resourceAmount = Mathf.Clamp(finalNum, energyMin, energyMax);
                
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cursor"))
        {
            animator.SetBool("hover", true);
            hovered = true;
            GameManager.instance.focusedNode = this;
            EventBroker.DisplayNodeCall();
            EventBroker.OnHoverCall();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Cursor"))
        {
            animator.SetBool("hover", false);
            hovered = false;
            GameManager.instance.focusedNode = selectionMade ? selectedNode : null;
            EventBroker.DisplayNodeCall();
            EventBroker.ExitHoverCall();
        }
    }

    private void ClickResponse()
    {
        // if mouse is over this node, respond to click
        if (hovered)
        {
            if (GameManager.instance.phase == Phase.Connection)
                ConnectionPhaseClick();
            else if (connected && GameManager.instance.phase == Phase.Action)
            {
                if (assessingMedical && medicalCount < medicalMax)
                    Heal();
                else if (assessingSupplies)
                    GetSupplies();
            }
        }
    }

    void ConnectionPhaseClick()
    {
        // if no node is currently actively selected, select this node
        if (!selectionMade)
            SelectNode();

        else
        {
            // if this is the selected node, deselect
            if (selected)
                DeselectNode();

            else
            {
                // if this is currently engaged with the selected node...
                if (tempConnectedNodes.Contains(selectedNode))
                {
                    Connector connector = tempConnections[selectedNode];

                    // Disconnect them if clicking on the child node
                    if (connector.origin == selectedNode)
                        DisconnectFromOriginNode(selectedNode);

                    // Change selection to parent if clicking on parent node
                    else
                    {
                        selectedNode.DeselectNode();
                        SelectNode();
                    }
                }

                else
                {
                    // if this can engage with the selected node, connect
                    if (eligibleTarget)
                    {
                        if (GameManager.instance.signalStrengthRemaining > 0)
                            ConnectNodes();
                        else
                        {
                            // TODO: Negative Feedback
                        }
                    }

                    // otherwise assume the desired action is to select the new node, and switch
                    else
                    {
                        selectedNode.DeselectNode();
                        SelectNode();
                    }
                }
            }
        }
    }

    void SelectNode()
    {
        selected = true;
        selectionMade = true;
        selectedNode = this;

        // activate radius if a selecting a live node furing Connection Phase, otherwise raduis off
        if (!dead && activated && engaged && GameManager.instance.phase == Phase.Connection)
        {
            radiusRenderer.color = GameManager.instance.colorDefaults.RadiusColor;
            activeRadius = radius;
        }
        else
        {
            defaultNodeOutlineRenderer.color = GameManager.instance.colorDefaults.SelectedOutlineColor;
            activeRadius = 0;
        }

        SetOutlineColor();

        if (GameManager.instance.phase == Phase.Connection)
            SetEligibility();

        EventBroker.NodeSelectionChangeCall();
        EventBroker.SelectionOnCall();
    }

    void DeselectNode()
    {
        selected = false;
        selectionMade = false;
        selectedNode = null;

        // turn off radius
        radiusRenderer.color = GameManager.instance.colorDefaults.Invisible;
        activeRadius = 0;

        SetOutlineColor();

        if (GameManager.instance.phase == Phase.Connection)
            SetEligibility();

        EventBroker.NodeSelectionChangeCall();
        EventBroker.SelectionOffCall();
    }

    void ConnectNodes()
    {
        Connector connector = null;

        // select established connector, or make new one
        if (establishedConnectedNodes.Contains(selectedNode))
        {
            connector = establishedConnections[selectedNode];
        }
        else
        {
            GameObject connectorGO = ObjectPool.SharedInstance.GetPooledObject();
            if (connectorGO != null)
            {
                connector = connectorGO.GetComponent<Connector>();
                connectorGO.SetActive(true);
            }
        }

        // update connector points and color
        connector.SetOrigin(selectedNode);
        connector.SetDestination(this);
        connector.SetColor(GameManager.instance.colorDefaults.EngagedColor);

        // add nodes to each other's temp connector nodes
        tempConnectedNodes.Add(selectedNode);
        selectedNode.tempConnectedNodes.Add(this);

        // add this connector to temp connections
        tempConnections[selectedNode] = connector;
        selectedNode.tempConnections[this] = connector;

        // add this node to current Network
        GameManager.instance.currentNetworkNodes.Add(this);
        GameManager.instance.currentNetworkConnectors.Add(connector);

        // set node to engaged
        engaged = true;
        SetOutlineColor();

        // update selected node if new node is activated
        if (activated)
        {
            selectedNode.DeselectNode();
            SelectNode();
        }

        SetEligibility();

        EventBroker.AddConnectorCall();
    }

    // Disconnect this node from an origin node
    void DisconnectFromOriginNode(Node originNode)
    {
        // get reference to connection to disconnect
        Connector connector = tempConnections[originNode];

        // remove nodes from each other's temp connection nodes
        tempConnectedNodes.Remove(originNode);
        originNode.tempConnectedNodes.Remove(this);

        // remove connection from temp connections
        tempConnections.Remove(originNode);
        originNode.tempConnections.Remove(this);

        // remove connection from global network
        GameManager.instance.currentNetworkConnectors.Remove(connector);

        // disconnect this node from any other node for which it is the origin
        List<Node> childNodesToDisconnect = new List<Node>();
        foreach (Node node in tempConnectedNodes)
        {
            if (tempConnections[node].origin == this)
                childNodesToDisconnect.Add(node);
        }
        foreach (Node node in childNodesToDisconnect)
        {
            node.DisconnectFromOriginNode(this);
        }

        // if no longer engaged with any other nodes, set to not engaged and remove from global network
        if (tempConnectedNodes.Count == 0)
        {
            engaged = false;
            GameManager.instance.currentNetworkNodes.Remove(this);
        }
        else
        {
            Debug.Log("connected to " + tempConnectedNodes.Count + "nodes including " + tempConnectedNodes[0].transform.parent.name);
        }

        // if established connection, revert to inactive color
        if (establishedConnections.ContainsKey(originNode))
        {
            connector.SetColor(GameManager.instance.colorDefaults.InactiveColor);
        }
        // if not established connection, remove connector entirely
        else
        {
            connector.gameObject.SetActive(false);
        }


        SetOutlineColor();
        SetEligibility();

        EventBroker.RemoveConnectorCall();
    }

    void SetEligibility()
    {
        // default to ineligible for connection
        eligibleTarget = false;

        if (connected || GameManager.instance.phase != Phase.Connection)
        {
            SetOutlineColor();
            return;
        }

        // if a node is selected other than this one, check for eligibility
        if (selectionMade && !selected)
        {
            // if within radius distance from selected node and not already connected
            if (Vector3.Distance(selectedNode.transform.position, transform.position) < activeRadius &&
                !tempConnectedNodes.Contains(selectedNode))
            {
                eligibleTarget = true;
            }
        }

        SetOutlineColor();
    }

    void SetOutlineColor()
    {
        SetBadgeColor();

        Color color;

        if (selected || (connected && !dead && type == NodeType.Civilian && assessingMedical && medicalCount < medicalMax) ||
            (connected && !dead && type == NodeType.Civilian && assessingSupplies && supplyCount == GameManager.instance.lowestSupply))
            color = GameManager.instance.colorDefaults.SelectedOutlineColor;
        else if (connected)
            color = GameManager.instance.colorDefaults.LiveConnectionOutlineColor;
        else if (eligibleTarget)
            color = GameManager.instance.colorDefaults.EligibleNodeColor;
        else if (engaged)
            color = GameManager.instance.colorDefaults.EngagedColor;
        else
            color = GameManager.instance.colorDefaults.Invisible;

        if (activated)
            badgeOutlineRenderer.color = color;
        else
            defaultNodeOutlineRenderer.color = color;
    }

    void SetBadgeColor()
    {
        Color color;

        if (dead && activated)
            color = GameManager.instance.colorDefaults.DeadColor;
        else if (connected)
            color = GameManager.instance.colorDefaults.LiveConnectionColor;
        else if (engaged)
            color = GameManager.instance.colorDefaults.EngagedColor;
        else
            color = GameManager.instance.colorDefaults.InactiveColor;

        if (activated)
            badgeRenderer.color = color;
        else
            defaultNodeRenderer.color = color;
    }

    // set node settings for a new game
    void Reset()
    {
        sourceNodes.Clear();
        receiverNodes.Clear();

        if (type == NodeType.Home)
        {
            badge.SetActive(true);
            defaultNode.SetActive(false);
            activated = true;
            engaged = true;
            connected = true;
            activeCollider.radius = badgeCollider.radius * badge.transform.localScale.x;
            activeCollider.isTrigger = false;
            activeCollider.isTrigger = true;
        }
        else
        {
            // set the default node active and badge inactive
            badge.SetActive(false);
            defaultNode.SetActive(true);
            activated = false;
            engaged = false;
            connected = false;
            activeCollider.radius = nodeCollider.radius * defaultNode.transform.localScale.x;
            activeCollider.isTrigger = false;
            activeCollider.isTrigger = true;
        }

        selected = false;

        badgeRenderer.color = GameManager.instance.colorDefaults.LiveConnectionColor;
        badgeOutlineRenderer.color = GameManager.instance.colorDefaults.Invisible;
        defaultNodeRenderer.color = GameManager.instance.colorDefaults.InactiveColor;
        defaultNodeOutlineRenderer.color = GameManager.instance.colorDefaults.Invisible;
        radiusRenderer.color = GameManager.instance.colorDefaults.Invisible;

        if (startActive)
            ActivateNode();
    }


    void ActivateConnection()
    {
        if (selected)
            DeselectNode();

        if (engaged)
        {
            // set as connected
            connected = true;

            // add nodes and connections to established connections
            foreach (Node node in tempConnectedNodes)
            {
                if (!establishedConnectedNodes.Contains(node))
                    establishedConnectedNodes.Add(node);
            }
            foreach (KeyValuePair<Node, Connector> kvp in tempConnections)
            {
                if (!establishedConnections.ContainsKey(kvp.Key))
                    establishedConnections.Add(kvp.Key, kvp.Value);
            }

            // if not already activated, activate
            if (!activated)
                ActivateNode();

            // set badge and outline colors
            SetOutlineColor();
        }

        EventBroker.CheckWinCall();
    }


    void DeactivateConnection()
    {
        if (selected)
            DeselectNode();
        
        if (connected)
        {
            // set as not connected
            connected = false;

            RecordLastKnownValues();

            //set badge and outline colors
            SetOutlineColor();
        }

        DecrementResources();
    }

    void RecordLastKnownValues()
    {
        lastMedicalCount = medicalCount;
        lastSupplyCount = supplyCount;
    }

    void DecrementResources()
    {
        if (type == NodeType.Civilian)
        {
            if (supplyCount <= 0)
            {
                medicalCount = Mathf.Max(0, medicalCount - 1);
                if (medicalCount == 0)
                    Die();
            }
            supplyCount = Mathf.Max(0, supplyCount - 1);
        }
    }

    void Die()
    {
        dead = true;
        BreakAllChildConnections();
        SetOutlineColor();
        EventBroker.DeathCall();
    }

    void AssessingMedical()
    {
        if (assessingSupplies)
            EventBroker.DisengageSuppliesCall();

        if (!assessingMedical)
            assessingMedical = true;
        
        if (type == NodeType.Civilian && connected && !dead && medicalCount < medicalMax)
        {
            animator.SetTrigger("pulse");
            SetOutlineColor();
        }
    }

    void StopAssessingMedical()
    {
        if (assessingMedical)
            assessingMedical = false;
        
        SetOutlineColor();
    }

    void AssessingSupplies()
    {
        if (assessingMedical)
            EventBroker.DisengageMedicalCall();

        if (!assessingSupplies)
            assessingSupplies = true;
        
        lowestSupply = GameManager.instance.lowestSupply;
        SetOutlineColor();

        if (type == NodeType.Civilian && connected && !dead && supplyCount == GameManager.instance.lowestSupply)
        {
            animator.SetTrigger("pulse");
        }
    }

    void StopAssessingSupplies()
    {
        if (assessingSupplies)
            assessingSupplies = false;
        
        SetOutlineColor();
    }

    void UpdateLowestSupply()
    {
        if (lowestSupply != GameManager.instance.lowestSupply)
        {
            lowestSupply = GameManager.instance.lowestSupply;
            SetOutlineColor();

            if (connected && supplyCount == GameManager.instance.lowestSupply)
            {
                animator.SetTrigger("pulse");
            }
        }
    }

    void Heal()
    {
        medicalCount = medicalMax;
        SetOutlineColor();
        animator.SetTrigger("pulse");
        EventBroker.UseMedicalCall();
    }

    void GetSupplies()
    {
        supplyCount++;
        SetOutlineColor();
        animator.SetTrigger("pulse");
        EventBroker.UseSuppliesCall();
    }

    void BreakAllChildConnections()
    {
        List<Node> disconnectFromOrigin = new List<Node>();
        List<Node> disconnectAsOrigin = new List<Node>();
        foreach (KeyValuePair<Node, Connector> kvp in tempConnections)
        {
            if (kvp.Value.origin == this)
                disconnectAsOrigin.Add(kvp.Key);
            else
                disconnectFromOrigin.Add(kvp.Key);
        }
        foreach (Node node in disconnectAsOrigin)
        {
            node.DisconnectFromOriginNode(this);
        }
        // foreach (Node node in disconnectFromOrigin)
        // {
        //     DisconnectFromOriginNode(node);
        // }
    }
    
    void ActivateNode()
    {
            badge.SetActive(true);
            defaultNode.SetActive(false);
            activated = true;
            activeCollider.radius = badgeCollider.radius * badge.transform.localScale.x;
            activeCollider.isTrigger = false;
            activeCollider.isTrigger = true;
    }
}
