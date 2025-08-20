using UnityEngine;
#if UNITY_EDITOR
[ExecuteInEditMode]
public class ShowLaser : MonoBehaviour
{
    [ReadOnly]
    public LaserObject laserObject;

    public void Setting() {
        laserObject = GetComponent<LaserObject>();
        laserObject.Editor_Awake();
    }

}
#endif
