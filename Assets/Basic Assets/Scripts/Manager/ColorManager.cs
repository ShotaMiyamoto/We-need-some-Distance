using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : SingletonMonoBehaviour<ColorManager>
{
    [SerializeField] private Color defaultColor = default;
    public Color GetDefaultColor { get { return defaultColor; } }


    [SerializeField] private Color goalBellColor = default;
    public Color GetGoalBellColor { get { return goalBellColor; } }


    [SerializeField] private Color mainColor = default;
    public Color GetMainColor { get { return mainColor; } }


    [SerializeField] private Color subColor = default;
    public Color GetSubColor { get { return subColor; } }


    [SerializeField] private Color obstacleColor = default;
    public Color GetObstacleColor { get { return obstacleColor; } }


    [SerializeField] private Color wallColor = default;
    public Color GetWallColor { get { return wallColor; } }

}
