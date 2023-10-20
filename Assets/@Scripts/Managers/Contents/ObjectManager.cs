using Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public HeroController Hero { get; private set; }
    public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();

    public ObjectManager()
    {
        Init();
    }

    public void Init()
    {

    }

    public void Clear()
    {
        Monsters.Clear();
    }

    public void LoadMap(string mapName)
    {
        GameObject objMap = Managers.Resource.Instantiate(mapName);
        objMap.transform.position = Vector3.zero;
        objMap.name = "@Map";
    }

    public void ShowDamageFont(Vector2 pos, float damage, float healAmount, Transform parent, bool isCritical = false)
    {
        string prefabName;
        if (isCritical)
            prefabName = "CriticalDamageFont";
        else
            prefabName = "DamageFont";

        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        DamageFont damageText = go.GetOrAddComponent<DamageFont>();
        damageText.SetInfo(pos, damage, healAmount, parent, isCritical);
    }

    public T Spawn<T>(Vector3 position, int templateID = 0, string prefabName = "") where T : BaseController
    {
        System.Type type = typeof(T);

        if (type == typeof(HeroController))
        {
            GameObject go = Managers.Resource.Instantiate(Managers.Data.CreatureDic[templateID].PrefabLabel);
            go.transform.position = position;
            HeroController hc = go.GetOrAddComponent<HeroController>();
            hc.SetInfo(templateID);
            Hero = hc;
            Managers.Game.Hero = hc;

            return hc as T;
        }
        else if (type == typeof(GR_Tree))
        {
            GameObject go = Managers.Resource.Instantiate("GR_Tree");
            go.transform.position = position;
            GR_Tree gt = go.GetOrAddComponent<GR_Tree>();

            return gt as T;
        }
        else if (type == typeof(GR_Mine))
        {
            GameObject go = Managers.Resource.Instantiate("GR_Mine");
            go.transform.position = position;
            GR_Mine gt = go.GetOrAddComponent<GR_Mine>();

            return gt as T;
        }
        return null;
    }

    public void Despawn<T>(T obj) where T : BaseController
    {
        System.Type type = typeof(T);

        if (type == typeof(PlayerController))
        {
            // ?
        }
        
        else if (type == typeof(MonsterController))
        {
            Monsters.Remove(obj as MonsterController);
            Managers.Resource.Destroy(obj.gameObject);
        }
    }
}
