using System;
using System.Collections;
using System.Collections.Generic;
using MissionSystem.Runtime.Scripts.ScriptableObject;
using UnityEngine;

[Serializable]
public class MissionLine
{
    private List<MissionConfig> _currentMissions = new List<MissionConfig>();
    private List<MissionConfig> _completeMissions = new List<MissionConfig>();
    public List<MissionConfig> currentMissions => _currentMissions;
    public List<MissionConfig> completeMissions => _completeMissions;

}