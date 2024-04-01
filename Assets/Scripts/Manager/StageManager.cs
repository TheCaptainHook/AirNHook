using Mirror;

public class StageManager
{
    //public int stage;
    // TODO 로비 이름으로 변경
    public string stageName = "Lobby";
    
    public void LoadMap()
    {
        if(!stageName.Equals("Lobby"))
            Managers.Game.CurrentState = GameState.Game;
        MapEditor.Instance.LoadMap(stageName);

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            var list = MapEditor.Instance.curMap.FindObject_Vector2(307);

            foreach (var key in list)
            {
                var obj = ResourceManager.Instantiate(Managers.Network.spawnPrefabDict["Key"]);
                obj.transform.position = key;
                NetworkServer.Spawn(obj);
            }
        }
    }
}
