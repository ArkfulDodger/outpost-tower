using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour
{
    LineRenderer lineRenderer;
    public Node origin;
    public Node destination;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetOrigin(Node node)
    {
        origin = node;
        lineRenderer.SetPosition(0, node.transform.position);
    }

    public void SetDestination(Node node)
    {
        destination = node;
        lineRenderer.SetPosition(1, node.transform.position);
    }

    public void SetColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
}
