using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Random = UnityEngine.Random;
using Transform = UnityEngine.Transform;

public static class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static Color HexToColor(string color)
    {
        Color parsedColor;

        if (color.Contains("#") == false)
            ColorUtility.TryParseHtmlString("#" + color, out parsedColor);
        else
            ColorUtility.TryParseHtmlString(color, out parsedColor);

        return parsedColor;
    }

    // Animator 컴포넌트 내에 특정 애니메이션 클립이 존재하는지 확인하는 함수
    public static bool HasAnimationClip(Animator animator, string clipName)
    {
        if (animator.runtimeAnimatorController == null)
        {
            return false;
        }

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return true;
            }
        }

        return false;
    }

    public static ESkillType GetSkillTypeFromInt(int value)
    {
        foreach (ESkillType skillType in Enum.GetValues(typeof(ESkillType)))
        {
            int minValue = (int)skillType;
            int maxValue = minValue + 5; // 100501~ 100506 사이 값이면 100501값 리턴

            if (value >= minValue && value <= maxValue)
            {
                return skillType;
            }
        }

        Debug.LogError($" Faild add skill : {value}");
        return ESkillType.None;
    }

    //Enum값중 랜덤값 반환
    public static T GetRandomEnumValue<T>() where T : struct, Enum
    {
        Type type = typeof(T);

        if (!_enumDict.ContainsKey(type))
            _enumDict[type] = Enum.GetValues(type);

        Array values = _enumDict[type];

        int index = UnityEngine.Random.Range(0, values.Length);
        return (T)values.GetValue(index);
    }

    //string값 으로 Enum값 찾기
    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static Vector2 GenerateRandomPositionOnCircle(Vector2 center, float radius)
    {
        int randomUnit = Random.Range(0, 10);
        float randomAngle = randomUnit * 36f;
        float radians = Mathf.Deg2Rad * randomAngle;
        float x = center.x + radius * Mathf.Cos(radians);
        float y = center.y + radius * Mathf.Sin(radians);

        return new Vector2(x, y);
    }
    
    #region Size

    public static long OneGB = 1000000000;
    public static long OneMB = 1000000;
    public static long OneKB = 1000;
    /// <summary> 바이트 <paramref name="byteSize"/> 사이즈에 맞게끔 적절한 단위 <see cref="ESizeUnits"/> 타입을 가져온다 </summary>
    public static ESizeUnits GetProperByteUnit(long byteSize)
    {
        if (byteSize >= OneGB)
            return ESizeUnits.GB;
        else if (byteSize >= OneMB)
            return ESizeUnits.MB;
        else if (byteSize >= OneKB)
            return ESizeUnits.KB;
        return ESizeUnits.Byte;
    }
    
    /// <summary> 바이트를 <paramref name="byteSize"/> <paramref name="unit"/> 단위에 맞게 숫자를 변환한다 </summary>
    public static long ConvertByteByUnit(long byteSize, ESizeUnits unit)
    {
        return (long)((byteSize / (double)System.Math.Pow(1024, (long)unit)));
    }

    /// <summary> 바이트를 <paramref name="byteSize"/> 단위와 함께 출력이 가능한 문자열 형태로 변환한다 </summary>
    public static string GetConvertedByteString(long byteSize, ESizeUnits unit, bool appendUnit = true)
    {
        string unitStr = appendUnit ? unit.ToString() : string.Empty;
        return $"{ConvertByteByUnit(byteSize, unit).ToString("0.00")}{unitStr}";
    }
    #endregion

}