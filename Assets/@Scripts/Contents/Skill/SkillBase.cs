using System;
using UnityEngine;
[Serializable]
public class SkillStat
{
    // public Define.SkillType SkillType;
    public int Level;
    public float MaxHp;
    public Data.SkillData SkillData;
}
public abstract class SkillBase : BaseController
{
    public CreatureController Owner { get; set; }
    #region Level
    int level = 0;
    public int Level
    {
        get { return level; }
        set { level = value; }
    }
    #endregion
    #region skillData
    [SerializeField]
    private Data.SkillData _skillData;
    public Data.SkillData SkillData 
    {
        get
        { 
            return _skillData;
        }
        set 
        { 
            _skillData = value;
        }
    }
    #endregion

    public Define.ESkillType SkillType;
    protected string AnimationName;
    public bool IsLearnedSkill { get { return Level > 0; } }

    protected override bool Init()
    {
        base.Init();
      
        return true;
    }

    public virtual void OnChangedSkillData() { }

    public abstract void DoSkill(Action callback = null);

    public void SetInfo(int skillId)
    {
        SkillData = Managers.Data.SkillDic[skillId];
        AnimationName = SkillData.AnimName;
        Owner = GetComponent<CreatureController>();
    }

    protected virtual void GenerateProjectile(CreatureController owner, string prefabName, Vector3 startPos, Vector3 dir, Vector3 targetPos, SkillBase skill)
    {
        // ProjectileController pc = Managers.Object.Spawn<ProjectileController>(startPos, prefabName: prefabName);
        // pc.SetInfo(Owner, startPos, dir, targetPos, skill);
    }

    protected void HitEvent(Collider2D collision)
    {

    }

    //하이어라키의 애니메이션에서 받는 이벤트
    public virtual void OnAnimationEvent(int param){ }

}
