using UnityEditor;
using UnityEngine;

public class GameScene : BaseScene
{
    private void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        Debug.Log("@>> GameScene Init()");
        base.Init();
        ESceneType = Define.EScene.GameScene;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Managers.UI.ShowSceneUI<UI_Joystick>();
        
        Managers.Map.LoadMap("BaseMap");
        HeroController LeaderHero = Managers.Object.Spawn<HeroController>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(0,-3,0)), 201000);
        Managers.Object.Spawn<HeroController>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(0,-3,0)), 201001).MyLeader = LeaderHero;
        Managers.Object.Spawn<HeroController>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(0,-3,0)), 201002).MyLeader = LeaderHero;;
        Managers.Object.Spawn<HeroController>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(0,-3,0)), 201004).MyLeader = LeaderHero;;
        Managers.Object.Spawn<HeroController>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(0,-3,0)), 201006).MyLeader = LeaderHero;;
        Managers.Object.Spawn<HeroController>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(0,-3,0)), 201008).MyLeader = LeaderHero;;
        Managers.Object.Spawn<HeroController>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(0,-3,0)), 201009).MyLeader = LeaderHero;;

        foreach (var spawnInfo in Managers.Map.GatheringResourceSpawnInfos)
        {
            Managers.Object.Spawn<GatheringResource>(Managers.Map.CurrentGrid.GetCellCenterWorld(spawnInfo.SpawnPos), spawnInfo.DataId);
        }
        
        foreach (var spawnInfo in Managers.Map.MonsterSpawnInfos)
        {
            Managers.Object.Spawn<MonsterController>(Managers.Map.CurrentGrid.GetCellCenterWorld(spawnInfo.SpawnPos), spawnInfo.DataId);
        }

    }
        
    public override void Clear()
    {

    }

}
