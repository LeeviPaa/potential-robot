using System.Collections.Generic;
using UnityEngine;

namespace PotentialRobot.UI.ColorPalettes
{
    public interface IColorPalette
    {
        List<Color> Colors { get; }
    }
}
