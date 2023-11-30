using System;
using Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public HeroController Hero { get; private set; }
    public HashSet<HeroController> Heros { get; } = new HashSet<HeroController>();
    public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();
    public HashSet<InteractionObject> InteractionObjects { get; } = new HashSet<InteractionObject>();

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
            Heros.Add(hc);
            return hc as T;
        }

        if (type == typeof(GatheringResource))
        {
            GameObject go = Managers.Resource.Instantiate(Managers.Data.GatheringResourceDic[templateID].PrefabLabel);
            go.transform.position = position;
            GatheringResource gr = go.GetOrAddComponent<GatheringResource>();
            gr.SetInfo(templateID);
            InteractionObjects.Add(gr);
            return gr as T;
        }

        if (type == typeof(MonsterController))
        {
            GameObject go = Managers.Resource.Instantiate(Managers.Data.CreatureDic[templateID].PrefabLabel);
            go.transform.position = position;
            MonsterController mc = go.GetOrAddComponent<MonsterController>();
            mc.SetInfo(templateID);
            Monsters.Add(mc);
            InteractionObjects.Add(mc);
            return mc as T;
        }

        if (type == typeof(DropItemController))
        {
            GameObject go = Managers.Resource.Instantiate(Managers.Data.DropItemDic[templateID].PrefabLabel);
            go.transform.position = position;
            DropItemController di = go.GetOrAddComponent<DropItemController>();
            return di as T;
        }

        if (type == typeof(RangeArcProjectile))
        {
            GameObject go = Managers.Resource.Instantiate(prefabName);
            go.transform.position = position;
            RangeArcProjectile di = go.GetOrAddComponent<RangeArcProjectile>();
            
            return di as T;
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
        else if (type == typeof(InteractionObject))
        {
            Managers.Resource.Destroy(obj.gameObject);
        }
        else if (type == typeof(DropItemController))
        {
            Managers.Resource.Destroy(obj.gameObject);
        }
        else if (type == typeof(RangeArcProjectile))
        {
            Managers.Resource.Destroy(obj.gameObject);
        }
    }

    public InteractionObject GetInteracctionTarget(InteractionObject scanner, Vector3 pivotPosition)
    {
        int scanRange = Define.SCAN_RANGE_MONSTER;
        
        if (scanner.ObjectType == EObjectType.Hero)
        {
            scanRange = Define.SCAN_RANGE_HERO;
            pivotPosition = Managers.Game.Leader.GatheringPoint;
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll((Vector2)pivotPosition, scanRange);
        List<InteractionObject> targets = new List<InteractionObject>();

        foreach (var collider in hitColliders)
        {
            InteractionObject interactionObject = collider.GetComponent<InteractionObject>();
            if (interactionObject && interactionObject.Attribute.Hp.CurrentValue > 0)
                targets.Add(interactionObject);
        }

        switch (scanner.ObjectType)
        {
            case EObjectType.Hero:
                //targets 에서 ObjectType가 Monster, GatheringResource만 추출
                List<InteractionObject> monsters = targets
                    .Where(target => target.ObjectType == EObjectType.Monster)
                    .OrderBy(target => (pivotPosition - target.CenterPosition).sqrMagnitude)
                    .ThenBy(target => target.LockedOnCount)
                    .ToList();

                if (monsters.Count > 0)
                {
                    foreach (var monster in monsters.Where(monster => monster.LockedOnCount < 3))
                    {
                        monster.LockedOnCount++;
                        return monster;
                    }

                    //이미 거리에 따라 정렬되어 있기 때문에 가장 첫번째 인덱스 리턴
                    monsters[0].LockedOnCount++;
                    return monsters[0];
                }
                List<InteractionObject> resources = targets
                    .Where(target => target.ObjectType == EObjectType.GatheringResources)
                    .OrderBy(target => (pivotPosition - target.CenterPosition).sqrMagnitude)
                    .ThenBy(target => target.LockedOnCount)
                    .ToList();
                if (resources.Count > 0)
                {
                    foreach (var resource in resources.Where(resource => resource.LockedOnCount < 3))
                    {
                        resource.LockedOnCount++;
                        return resource;
                    }

                    resources[0].LockedOnCount++;
                    return resources[0];
                }
                // 주변에 아무것도 없으면 null
                return null;
            
            case EObjectType.Monster:
                List<InteractionObject> heroes = targets
                    .Where(target => target.ObjectType == EObjectType.Hero)
                    .OrderBy(target => (pivotPosition - target.CenterPosition).sqrMagnitude)
                    .ThenBy(target => target.LockedOnCount)
                    .ToList();
                
                if (heroes.Count > 0)
                {
                    foreach (var hero in heroes.Where(hero => hero.LockedOnCount < 3))
                    {
                        hero.LockedOnCount++;
                        return hero;
                    }

                    heroes[0].LockedOnCount++;
                    return heroes[0];
                }

                return null;

            default:
                return null;
        }
    }
}