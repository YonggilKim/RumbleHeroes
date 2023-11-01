using Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{

    public Dictionary<int, Data.SkillData> SkillDic { get; private set; } = new Dictionary<int, Data.SkillData>();
    public Dictionary<int, Data.CreatureData> CreatureDic { get; private set; } = new Dictionary<int, Data.CreatureData>();
    public Dictionary<int, Data.GatheringResourceData> GatheringResourceDic { get; private set; } = new Dictionary<int, Data.GatheringResourceData>();

    public void Init()
    {
        CreatureDic = LoadJson<Data.CreatureDataLoader, int, Data.CreatureData>("CreatureData").MakeDict();
        GatheringResourceDic = LoadJson<Data.GatheringResourceDataLoader, int, Data.GatheringResourceData>("GatheringResourcesData").MakeDict();
        // SkillDic = LoadJson<Data.SkillDataLoader, int, Data.SkillData>("SkillData").MakeDict();
        // LevelDataDic = LoadJson<Data.LevelDataLoader, int, Data.LevelData>("LevelData").MakeDict();
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
		TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{path}");
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
	}


}
