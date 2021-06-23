using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighlightText : MonoBehaviour
{
    TMP_Text text;

    private void Awake()
    {
        text = transform.GetComponentInChildren<TMP_Text>();
    }

    public void Highlight()
    {
        text.color = Color.white;
    }

    public void Dim()
    {
        text.color = Color.gray;
    }
}
