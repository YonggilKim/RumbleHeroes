using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static Define;

public class SkillBook : MonoBehaviour
{
    [SerializeField]
    private List<SkillBase> _skillList = new List<SkillBase>();
    public List<SkillBase> SkillList { get { return _skillList; } } 
    public List<SequenceSkill> SequenceSkills { get; } = new List<SequenceSkill>();
    public List<SkillBase> ActivatedSkills
    {
        get { return SkillList.Where(skill => skill.IsLearnedSkill).ToList(); }
    }
    public SkillBase BaseAttackSkill { get; set; }

    public event Action UpdateSkillUi;
    public CreatureController _owner;

    public void Awake()
    {
        _owner = GetComponent<CreatureController>();
    }

    public void Init()
    {
        //SkillList.Clear();
        //SupportSkills.Clear();
        //ActivatedSkills.Clear();
        //SavedSkill.Clear();
    }

    public void LoadSkill(Define.ESkillType skillType, int level)
    {
        // AddSkill(skillType);
    }

    public void AddSkill(int skillId = 0)
    {
        string className = Managers.Data.SkillDic[skillId].ClassName;

        SkillBase skill = gameObject.AddComponent(Type.GetType(className)) as SkillBase;
        if (skill)
        {
            skill.SetInfo(skillId);
            SkillList.Add(skill);
            if (skillId == _owner.CreatureData.SkillIdList[0])
                BaseAttackSkill = skill;
        }

        
        // if (skillType == ESkillType.FrozenHeart || skillType == ESkillType.SavageSmash || skillType == ESkillType.EletronicField)
        // {
        //     GameObject go = Managers.Resource.Instantiate(skillType.ToString(), gameObject.transform);
        //     if (go != null)
        //     {
        //         SkillBase skill = go.GetOrAddComponent<SkillBase>();
        //         SkillList.Add(skill);
        //         if(SavedBattleSkill.ContainsKey(skillType))
        //             SavedBattleSkill[skillType] = skill.Level;
        //         else
        //             SavedBattleSkill.Add(skillType, skill.Level);
        //     }
        // }
        // else
        // {
        //     // AddComponent만 하면됌
        //     SequenceSkill skill = gameObject.AddComponent(Type.GetType(className)) as SequenceSkill;
        //     if (skill != null)
        //     {
        //         skill.ActivateSkill();
        //         skill.Owner = GetComponent<CreatureController>();
        //         skill.DataId = skillId;
        //         SkillList.Add(skill);
        //         SequenceSkills.Add(skill);
        //     }
        //     else
        //     {
        //         RepeatSkill skillbase = gameObject.GetComponent(Type.GetType(className)) as RepeatSkill;
        //         SkillList.Add(skillbase);
        //         if (SavedBattleSkill.ContainsKey(skillType))
        //             SavedBattleSkill[skillType] = skillbase.Level;
        //         else
        //             SavedBattleSkill.Add(skillType, skillbase.Level);
        //     }
        // }
    }

    public void AddActiavtedSkills(SkillBase skill)
    {
        ActivatedSkills.Add(skill);
    }

    int _sequenceIndex = 0;

    public void StartNextSequenceSkill()
    {
        if (_stopped)
            return;
        if (SequenceSkills.Count == 0)
            return;

        SequenceSkills[_sequenceIndex].DoSkill(OnFinishedSequenceSkill);
    }

    void OnFinishedSequenceSkill()
    {
        _sequenceIndex = (_sequenceIndex + 1) % SequenceSkills.Count;
        StartNextSequenceSkill();
    }

    bool _stopped = false;

    public void StopSkills()
    {
        _stopped = true;

        foreach (var skill in ActivatedSkills)
        {
            skill.StopAllCoroutines();
        }
    }

    public void LevelUpSkill(Define.ESkillType skillType)
    {
        for (int i = 0; i < SkillList.Count; i++)
        {

        }
    }

    public void OnSkillBookChanged()
    {
        UpdateSkillUi?.Invoke();
    }

    public void Clear()
    {
    }

}
