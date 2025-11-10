using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDatabaseSO", menuName = "Scriptable Objects/ProjectileDatabaseSO")]
public class ProjectileDatabaseSO : ScriptableObject
{
    public List<ProjectileDataSO> projectileData;
}
