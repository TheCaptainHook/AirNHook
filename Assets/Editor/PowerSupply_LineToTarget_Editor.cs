
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

    //  public  bool IsMethodRegistered()
    // {
    //     if (EditorApplication.update == null) return false;

    //     foreach (var d in EditorApplication.update.GetInvocationList())
    //     {
    //         Debug.Log(d.Method.Name);
    //         if (d.Method.Name == "Refrash") // 메서드 이름으로 확인
    //         {
    //             Debug.Log("✅ powerSupply_LineToTarget.Refrash is registered in EditorApplication.update.");
    //             return true;
    //         }
    //     }

    //     Debug.Log("❌ powerSupply_LineToTarget.Refrash is NOT registered in EditorApplication.update.");
    //     return false;
    // }

    private void Activate()
    {
        // if(powerSupply_LineToTarget != null) return;
        // powerSupply_LineToTarget = (PowerSupply_LineToTarget)target;

        if(!powerSupply_LineToTarget.IsObjectVisibleInInspector()) return;

        // if(IsMethodRegistered()) return;


        powerSupply_LineToTarget.Setting();
        powerSupply_LineToTarget.StartRefrash();
        // EditorApplication.update += powerSupply_LineToTarget.Refrash;
    }

    // private void OnEnable(){
    //     powerSupply_LineToTarget = (PowerSupply_LineToTarget)target;
    //     if(!powerSupply_LineToTarget.IsObjectVisibleInInspector()) return;

    //     powerSupply_LineToTarget.Setting();

    //     EditorApplication.update += powerSupply_LineToTarget.Refrash;

    // }
    // private void OnDisable(){
    //     if(!powerSupply_LineToTarget.IsObjectVisibleInInspector()) return;
    //     EditorApplication.update -= powerSupply_LineToTarget.Refrash;
    //     powerSupply_LineToTarget.Destroy();
    // }

    private void Deactivate()
    {
        if(powerSupply_LineToTarget == null) return;
        
        if(!powerSupply_LineToTarget.IsObjectVisibleInInspector()) return;
        powerSupply_LineToTarget.StopRefrash();
        // EditorApplication.update -= powerSupply_LineToTarget.Refrash;

        // EditorApplication.delayCall += () =>
        //     {
        //         if (powerSupply_LineToTarget != null)
        //         {
        //             // DestroyImmediate(powerSupply_LineToTarget);
        //             powerSupply_LineToTarget.Destroy();
        //             powerSupply_LineToTarget = null;
        //         }
        //     };

        // powerSupply_LineToTarget.Destroy();
        // powerSupply_LineToTarget = null;
    }

}





