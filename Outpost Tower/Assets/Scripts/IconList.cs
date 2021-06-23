using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "IconList", menuName = "", order = 1)]
public class IconList : ScriptableObject
{
    [Serializable]
    public struct Icon
    {
        public NodeType type;
        public Sprite sprite;
    }

    public List<Icon> icons = new List<Icon>();
}
