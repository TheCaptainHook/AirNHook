using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Puzzle_1_Net : NetworkBehaviour
{
    Puzzle_1 puzzle;
    Puzzle_1 Puzzle
    {
        get
        {
            if (puzzle == null) puzzle = GetComponent<Puzzle_1>();
            return puzzle;
        }
    }
    [SerializeField] Puzzle_1_Button button;

    #region Animation Sync

    [SyncVar(hook = nameof(OnRateChanged))]
    private float chargingRate;

    [Server]
    public void SetRate(float rate)
    {
        chargingRate = rate;
    }
    private void CmdSetRate(float rate)
    {
        SetRate(rate);
    }

    public void HandleSetRate(float rate)
    {
        if (isServer)
        {
            SetRate(rate);
        }
        else
        {
            CmdSetRate(rate);
        }
    }
    #endregion


    public void OnRateChanged(float old,float newVal)
    {
        button.SetAnimation(newVal);
      
    }



    [Command(requiresAuthority = false)]
    public void CmdCharging()
    {
        RpcCharging();
    }
    [ClientRpc]
    private void RpcCharging()
    {
        Puzzle.Charging();
    }
}
