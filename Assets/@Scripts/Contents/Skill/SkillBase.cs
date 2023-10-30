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
public class SkillBase : BaseController
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
    public Data.SkillData _skillData;
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

    public float TotalDamage { get; set; } = 0;
    public bool IsLearnedSkill { get { return Level > 0; } }


    public virtual void OnChangedSkillData() { }

    protected virtual void GenerateProjectile(CreatureController Owner, string prefabName, Vector3 startPos, Vector3 dir, Vector3 targetPos, SkillBase skill)
    {
        // ProjectileController pc = Managers.Object.Spawn<ProjectileController>(startPos, prefabName: prefabName);
        // pc.SetInfo(Owner, startPos, dir, targetPos, skill);
    }

    protected void HitEvent(Collider2D collision)
    {

    }

}
