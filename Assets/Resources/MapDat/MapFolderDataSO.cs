using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapFolderData", menuName = "MapData/Folder List")]
public class MapFolderDataSO : ScriptableObject
{
  public List<int> mainMapfolderNames = new();
}
