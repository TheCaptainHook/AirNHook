
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PowerSupply_LineToTarget))]
public class PowerSupply_LineToTarget_Editor : Editor
{
    private PowerSupply_LineToTarget powerSupply_LineToTarget;


    void OnEnable()
    {
        powerSupply_LineToTarget = (PowerSupply_LineToTarget)target;
    }

    public override void OnInspectorGUI()
    {

        if(GUILayout.Button("활성화"))
        {
            Activate();
        }

        if(GUILayout.Button("비활성화"))
        {
            Deactivate();
        }

    }

    private void Activate()
    {

        if(!powerSupply_LineToTarget.IsObjectVisibleInInspector()) return;

        powerSupply_LineToTarget.Setting();
        powerSupply_LineToTarget.StartRefrash();    }


    private void Deactivate()
    {
        if(powerSupply_LineToTarget == null) return;
        
        if(!powerSupply_LineToTarget.IsObjectVisibleInInspector()) return;
        powerSupply_LineToTarget.StopRefrash();

    }

}





