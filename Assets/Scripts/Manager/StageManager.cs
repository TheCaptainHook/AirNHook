using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class StageManager : NetworkBehaviour
{
    // SyncVar는 플레이어끼리 해당 변수를 동기화 해줌. NetworkBehaviour필요.
    [SyncVar] public int stage;

    // TODO 처음에는 Lobby로 입장하게 만들고, 이후에 GameManager의 GameState에 따라 작동하도록 작성
}
