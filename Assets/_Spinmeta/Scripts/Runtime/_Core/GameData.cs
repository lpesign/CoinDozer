using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LP.Dozer
{
    [Serializable]
    public class GameData
    {
        public Dictionary<string, CollectableItemData> coinDatas;

        public GameData()
        {
            coinDatas = new Dictionary<string, CollectableItemData>();
        }

        public void AddItem(CollectableItemData data)
        {
            if (coinDatas.ContainsKey(data.id) == false)
            {
                coinDatas.Add(data.id, data);
            }
        }

        public void RemoveItem(CollectableItemData data)
        {
            if (coinDatas.ContainsKey(data.id))
            {
                coinDatas.Remove(data.id);
            }
        }

        public void Clear()
        {
            coinDatas.Clear();
        }
    }

    [Serializable]
    public struct UserData
    {
        public int maxCoin;
        public int coin;

        public ItemData[] items;

        public static UserData CreateNewUser()
        {
            return new UserData()
            {
                maxCoin = 1000,
                coin = 500,
                items = CreateNewItems()
            };
        }

        public static ItemData[] CreateNewItems()
        {
            Debug.Log("CreateNewItems");
            return new ItemData[]{
                new ItemData(){ id = DozerSkillWall.Data.ID, value = 5 },
                new ItemData(){ id = DozerSkillEarthQuake.Data.ID, value = 6 },
                new ItemData(){ id = DozerSKillCoinShower.Data.ID, value = 7 },
                new ItemData(){ id = DozerSkillPowerPush.Data.ID, value = 8 },
            };
        }
    }

    [Serializable]
    public struct ItemData
    {
        public string id;
        public int value;
    }

}