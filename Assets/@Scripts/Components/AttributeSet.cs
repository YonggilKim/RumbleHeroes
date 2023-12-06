using UnityEngine;

[System.Serializable]
public class AttributeData
{
    /// <summary>
    /// CurrentValue를 조작하여 현재 상태를 반영하고,
    /// BaseValue는 기본 속성을 나타내며 변경이 즉시 반영되지 않음.
    /// 이렇게 함으로써 특정 효과나 능력에 의한 수정이 끝난 후에도 기본 속성 값을 유지할 수 있도록 함
    /// </summary>
    [SerializeField] 
    private float _baseValue;
    [SerializeField] 
    private float _currentValue;

    public float BaseValue
    {
        get => _baseValue;
        set => _baseValue = value;
    }

    public float CurrentValue
    {
        get => _currentValue;
        set => _currentValue = value;
    }
};

public class AttributeSet : MonoBehaviour
{
    #region GameplayAttributeData
    public AttributeData MaxHp = new AttributeData();
    public AttributeData Hp = new AttributeData();
    public AttributeData MaxHpBonusRate = new AttributeData();
    public AttributeData HealBonusRate = new AttributeData();
    public AttributeData HpRegen = new AttributeData();
    public AttributeData Atk = new AttributeData();
    public AttributeData AttackRate = new AttributeData();
    public AttributeData Def = new AttributeData();
    public AttributeData DefRate = new AttributeData();
    public AttributeData CriRate = new AttributeData();
    public AttributeData CriDamage = new AttributeData();
    public AttributeData DamageReduction = new AttributeData();
    public AttributeData MoveSpeedRate = new AttributeData();
    public AttributeData MoveSpeed= new AttributeData();
    #endregion
    
    protected virtual bool PreGameplayEffectExecute() { return true; }
    protected virtual void PostGameplayEffectExecute() { }
    protected virtual void PreAttributeChange(BaseController target, float newValue) { }
    protected virtual void PostAttributeChange(BaseController target, float OldValue, float NewValue)
    {
    }

}
