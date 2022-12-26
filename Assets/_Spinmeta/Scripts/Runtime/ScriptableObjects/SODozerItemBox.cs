using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LP.Dozer
{
    [CreateAssetMenu(fileName = "SODzoerItemBox", menuName = "Spinmeta/Dozer/Factory/SODozerItemBox")]
    public class SODozerItemBox : ScriptableObject
    {
        [Serializable]
        public class Item
        {
            public ItemType type;
            [Min(1)] public int weight = 1;
            [Min(1)] public int poolSize = 10;
            public CollectableItem prefab;
        }


        public List<Item> items;
        private WeightedRandom<ItemType> _generator;

        public ItemType GetRandomItemType()
        {

            if (_generator == null)
            {
                _generator = new WeightedRandom<ItemType>();
                foreach (var item in items)
                {
                    _generator.Add(item.type, item.weight);
                }
            }

            return _generator.GetRandom();
        }
    }
}