
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PowerSupply_LineToTarget))]
public class PowerSupply_LineToTarget_Editor : Editor
{
    private PowerSupply_LineToTarget powerSupply_LineToTarget;

    private void OnEnable(){
        powerSupply_LineToTarget = (PowerSupply_LineToTarget)target;
        if(!powerSupply_LineToTarget.IsObjectVisibleInInspector()) return;

        powerSupply_LineToTarget.Setting();

        EditorApplication.update += powerSupply_LineToTarget.Refrash;

    }
    private void OnDisable(){
        if(!powerSupply_LineToTarget.IsObjectVisibleInInspector()) return;
        EditorApplication.update -= powerSupply_LineToTarget.Refrash;
        powerSupply_LineToTarget.Destroy();
    }

}
