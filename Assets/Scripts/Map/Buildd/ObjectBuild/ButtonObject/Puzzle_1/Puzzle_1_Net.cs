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
