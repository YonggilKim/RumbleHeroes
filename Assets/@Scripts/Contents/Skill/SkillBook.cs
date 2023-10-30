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

    public event Action UpdateSkillUi;
    public EObjectType _ownerType;

    public void Awake()
    {
        _ownerType = GetComponent<CreatureController>().ObjectType;

    }

    public void Init()
    {
        //SkillList.Clear();
        //SupportSkills.Clear();
        //ActivatedSkills.Clear();
        //SavedSkill.Clear();
    }
    public void SetInfo(EObjectType type)
    {
        _ownerType = type;
    }

    public void LoadSkill(Define.ESkillType skillType, int level)
    {
        //모든스킬은 0으로 시작함. 레벨 수 만큼 레벨업ㅎ ㅏ기
        AddSkill(skillType);
        for(int i = 0; i < level; i++)
            LevelUpSkill(skillType);

    }

    public void AddSkill(Define.ESkillType skillType, int skillId = 0)
    {
        string className = skillType.ToString();

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
