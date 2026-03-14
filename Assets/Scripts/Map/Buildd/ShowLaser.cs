using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class ShowLaser : MonoBehaviour
{
    [ReadOnly]
    public LaserObject laserObject;
#if UNITY_EDITOR
    public void Setting()
    {
        laserObject = GetComponent<LaserObject>();
        laserObject.Editor_Awake();
    }
#endif
}
