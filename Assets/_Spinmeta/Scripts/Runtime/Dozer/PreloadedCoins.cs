using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Dozer
{
    public class PreloadedCoins : MonoBehaviour
    {
        public List<CollectableItem> GetPreloadedCoins()
        {
            List<CollectableItem> result = new List<CollectableItem>();
            GetComponentsInChildren<CollectableItem>(result);
            return result;
        }
    }
}