using Mirror;

public class StageManager
{
    //public int stage;
    // TODO 로비 이름으로 변경
    public string stageName = "Tutorial_1";
    
    // TODO 처음에는 Lobby로 입장하게 만들고, 이후에 GameManager의 GameState에 따라 작동하도록 작성
    public void LoadMap()
    {
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
