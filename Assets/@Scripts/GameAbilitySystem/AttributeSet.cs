public class GameplayAttributeData
{
    /// <summary>
    /// CurrentValue를 조작하여 현재 상태를 반영하고,
    /// BaseValue는 기본 속성을 나타내며 변경이 즉시 반영되지 않음.
    /// 이렇게 함으로써 특정 효과나 능력에 의한 수정이 끝난 후에도 기본 속성 값을 유지할 수 있도록 함
    /// </summary>
    public float BaseValue { get; set; }
    public float CurrentValue { get; set; }

    public GameplayAttributeData(float DefaultValue = 0)
    {
        BaseValue = DefaultValue;
        CurrentValue = DefaultValue;
    }
};

public class AttributeSet 
{
    #region GameplayAttributeData
    public GameplayAttributeData MaxHp { get; set; } = new GameplayAttributeData();
    public GameplayAttributeData Hp { get; set; } = new GameplayAttributeData();
    public GameplayAttributeData MaxHpBonusRate { get; set; } = new GameplayAttributeData();
    public GameplayAttributeData HealBonusRate { get; set; } = new GameplayAttributeData();
    public GameplayAttributeData HpRegen { get; set; } = new GameplayAttributeData();
    public GameplayAttributeData Atk { get; set; } = new GameplayAttributeData();
    public GameplayAttributeData AttackRate { get; set; } = new GameplayAttributeData();
    public GameplayAttributeData Def { get; set; } = new GameplayAttributeData();
    public GameplayAttributeData DefRate { get; set; } = new GameplayAttributeData();
    public GameplayAttributeData CriRate { get; set; } = new GameplayAttributeData();
    public GameplayAttributeData CriDamage { get; set; } = new GameplayAttributeData();
    public GameplayAttributeData DamageReduction { get; set; } = new GameplayAttributeData();
    public GameplayAttributeData MoveSpeedRate { get; set; } = new GameplayAttributeData();
    public GameplayAttributeData MoveSpeed { get; set; }  = new GameplayAttributeData();
    #endregion
    
    protected virtual bool PreGameplayEffectExecute() { return true; }
    protected virtual void PostGameplayEffectExecute() { }
    protected virtual void PreAttributeChange(BaseController target, float newValue) { }
    protected virtual void PostAttributeChange(BaseController target, float OldValue, float NewValue)
    {
    }

}
