using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Data;
using System.ComponentModel;

public class DataTransformer : EditorWindow
{
#if UNITY_EDITOR
    #region Functions
    [MenuItem("Tools/DeleteGameData ")]
    public static void DeleteGameData()
    {
        PlayerPrefs.DeleteAll();
        string path = Application.persistentDataPath + "/SaveData.json";
        if (File.Exists(path))
            File.Delete(path);
    }

    public static T ConvertValue<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
            return default(T);

        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        return (T)converter.ConvertFromString(value);
    }

    public static List<T> ConvertList<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
            return new List<T>();

        return value.Split('&').Select(x => ConvertValue<T>(x)).ToList();
    }
    #endregion
  
    [MenuItem("Tools/ParseExcel %#K")]
    public static void ParseExcel()
    {
        ParseSkillData("Skill");
        ParseCreatureData("Creature");
        ParseGatheringResourcesData("GatheringResources");
        ParseDropItemData("DropItem");
        Debug.Log("Complete DataTransformer");
    }
    
    static void ParseSkillData(string filename)
    {
        SkillDataLoader loader = new SkillDataLoader();

        #region ExcelData
        string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/Excel/{filename}Data.csv").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;
            SkillData skillData = new SkillData();
            skillData.DataId = ConvertValue<int>(row[i++]);
            skillData.Name = ConvertValue<string>(row[i++]);
            skillData.ClassName = ConvertValue<string>(row[i++]);
            skillData.Description = ConvertValue<string>(row[i++]);
            skillData.PrefabLabel = ConvertValue<string>(row[i++]);
            skillData.IconLabel = ConvertValue<string>(row[i++]);
            skillData.SoundLabel = ConvertValue<string>(row[i++]);
            skillData.AnimName = ConvertValue<string>(row[i++]);
            skillData.CoolTime = ConvertValue<float>(row[i++]);
            skillData.DamageMultiplier = ConvertValue<float>(row[i++]);
            skillData.ProjectileSpacing = ConvertValue<float>(row[i++]);
            skillData.Duration = ConvertValue<float>(row[i++]);
            skillData.RecognitionRange = ConvertValue<float>(row[i++]);
            skillData.NumProjectiles = ConvertValue<int>(row[i++]);
            skillData.CastingSound = ConvertValue<string>(row[i++]);
            skillData.AngleBetweenProj = ConvertValue<float>(row[i++]);
            skillData.AttackInterval = ConvertValue<float>(row[i++]);
            skillData.NumBounce = ConvertValue<int>(row[i++]);
            skillData.BounceSpeed = ConvertValue<float>(row[i++]);
            skillData.BounceDist = ConvertValue<float>(row[i++]);
            skillData.NumPenerations = ConvertValue<int>(row[i++]);
            skillData.CastingEffect = ConvertValue<int>(row[i++]);
            skillData.HitSoundLabel = ConvertValue<string>(row[i++]);
            skillData.ProbCastingEffect = ConvertValue<float>(row[i++]);
            skillData.HitEffect = ConvertValue<int>(row[i++]);
            skillData.ProbHitEffect = ConvertValue<float>(row[i++]);
            skillData.ProjRange = ConvertValue<float>(row[i++]);
            skillData.MinCoverage = ConvertValue<float>(row[i++]);
            skillData.MaxCoverage = ConvertValue<float>(row[i++]);
            skillData.RoatateSpeed = ConvertValue<float>(row[i++]);
            skillData.ProjSpeed = ConvertValue<float>(row[i++]);
            skillData.ScaleMultiplier = ConvertValue<float>(row[i++]);
            loader.skills.Add(skillData);
        }
        #endregion

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}/@Resources/Data/JsonData/{filename}Data.json", jsonStr);
        AssetDatabase.Refresh();
    }

    static void ParseCreatureData(string filename)
    {
        CreatureDataLoader loader = new CreatureDataLoader();

        #region ExcelData
        string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/Excel/{filename}Data.csv").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');

            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;
            CreatureData cd = new CreatureData();
            cd.DataId = ConvertValue<int>(row[i++]);
            cd.DescriptionTextID = ConvertValue<string>(row[i++]);
            cd.PrefabLabel = ConvertValue<string>(row[i++]);
            cd.MaxHp = ConvertValue<float>(row[i++]);
            cd.MaxHpBonus = ConvertValue<float>(row[i++]);
            cd.Atk = ConvertValue<float>(row[i++]);
            cd.AtkRange = ConvertValue<float>(row[i++]);
            cd.AtkBonus = ConvertValue<float>(row[i++]);
            cd.Def = ConvertValue<float>(row[i++]);
            cd.MoveSpeed = ConvertValue<float>(row[i++]);
            cd.TotalExp = ConvertValue<float>(row[i++]);
            cd.HpRate = ConvertValue<float>(row[i++]);
            cd.AtkRate = ConvertValue<float>(row[i++]);
            cd.DefRate = ConvertValue<float>(row[i++]);
            cd.MoveSpeedRate = ConvertValue<float>(row[i++]);
            cd.SpriteName = ConvertValue<string>(row[i++]);
            cd.AnimatorName = ConvertValue<string>(row[i++]);
            cd.SkillIdList = ConvertList<int>(row[i++]);
            cd.DropItemId = ConvertValue<int>(row[i++]);
            loader.creatures.Add(cd);
        }

        #endregion

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}/@Resources/Data/JsonData/{filename}Data.json", jsonStr);
        AssetDatabase.Refresh();
    }
    
    static void ParseGatheringResourcesData(string filename)
    {
        GatheringResourceDataLoader loader = new GatheringResourceDataLoader();

        #region ExcelData
        string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/Excel/{filename}Data.csv").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');

            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;
            GatheringResourceData gr = new GatheringResourceData();
            gr.DataId = ConvertValue<int>(row[i++]);
            gr.DescriptionTextID = ConvertValue<string>(row[i++]);
            gr.PrefabLabel = ConvertValue<string>(row[i++]);
            gr.MaxHp = ConvertValue<float>(row[i++]);
            gr.ResourceAmount = ConvertValue<int>(row[i++]);
            gr.RegenTime = ConvertValue<float>(row[i++]);
            gr.SpriteName = ConvertValue<string>(row[i++]);
            gr.DropItemId = ConvertValue<int>(row[i++]);
            loader.creatures.Add(gr);
        }

        #endregion

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}/@Resources/Data/JsonData/{filename}Data.json", jsonStr);
        AssetDatabase.Refresh();
    }
    
    static void ParseDropItemData(string filename)
    {
        DropItemDataLoader loader = new DropItemDataLoader();

        #region ExcelData
        string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/Excel/{filename}Data.csv").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');

            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;
            DropItemData gr = new DropItemData();
            gr.DataId = ConvertValue<int>(row[i++]);
            gr.DescriptionTextID = ConvertValue<string>(row[i++]);
            gr.PrefabLabel = ConvertValue<string>(row[i++]);
            gr.SpriteName = ConvertValue<string>(row[i++]);
            gr.Amount = ConvertValue<int>(row[i++]);
            loader.creatures.Add(gr);
        }

        #endregion

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}/@Resources/Data/JsonData/{filename}Data.json", jsonStr);
        AssetDatabase.Refresh();
    }
#endif

}