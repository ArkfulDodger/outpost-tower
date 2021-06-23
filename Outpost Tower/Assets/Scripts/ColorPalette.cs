using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "~/Unity/GMTK Game Jam 2021 Game/Assets/Scripts/ColorPalette.cs/ColorPalette", order = 0)]
public class ColorPalette : ScriptableObject
{
    public Color EngagedColor;
    public Color EligibleNodeColor;
    public Color InactiveColor;
    public Color SelectedOutlineColor;
    public Color LiveConnectionOutlineColor;
    public Color LiveConnectionColor;
    public Color Invisible;
    public Color RadiusColor;
    public Color DeadColor;
}

