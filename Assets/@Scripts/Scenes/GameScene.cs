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
        Managers.Object.Spawn<HeroController>(Vector3.zero, 201000);

        Managers.Object.Spawn<GR_Tree>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(2, 1, 0)));
        Managers.Object.Spawn<GR_Tree>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(3, 1, 0)));
        Managers.Object.Spawn<GR_Tree>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(3, 2, 0)));
        Managers.Object.Spawn<GR_Tree>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(4, 2, 0)));
        Managers.Object.Spawn<GR_Mine>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(1, 2, 0)));
        Managers.Object.Spawn<MonsterController>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(-3, -3, 0)),202001);
        Managers.Object.Spawn<MonsterController>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(-3, -4, 0)),202001);
        Managers.Object.Spawn<MonsterController>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(-3, -5, 0)),202001);
        Managers.Object.Spawn<MonsterController>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(-2, -3, 0)),202001);
        Managers.Object.Spawn<MonsterController>(Managers.Map.CurrentGrid.GetCellCenterWorld(new Vector3Int(-4, -3, 0)),202001);

    }
        
    public override void Clear()
    {

    }

}
