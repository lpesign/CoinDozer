using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Dozer
{
    public class UserItem
    {
        public string ID { get; private set; }
        public int Count { get; private set; }

        public event Action<UserItem> OnValueChanged;

        public UserItem(string id, int value)
        {
            ID = id;
            Count = value;
        }

        public void SetValue(int value)
        {
            Count = value;

            OnValueChanged?.Invoke(this);
        }

        public bool Use()
        {
            if (Count == 0)
            {
                return false;
            }

            SetValue(Count - 1);
            return true;
        }
    }

    public class User
    {
        private static User _me;
        public static User Me { get => _me ?? (_me = new User()); }

        public string ID { get; private set; }
        public string Nickname { get; private set; }
        public string Token { get; private set; }
        public int Coin { get; private set; }
        public int MaxCoin { get; private set; }
        public bool IsLogin { get => !string.IsNullOrEmpty(Token); }

        public event Action<User> OnCoinChanged;
        public event Action<UserItem> OnItemChanged;

        private List<UserItem> _items = new List<UserItem>();

        public User()
        {
            SetUserID("Unknown");
            _items = new List<UserItem>();
        }

        public void Logout()
        {
            SetToken(null);
        }

        public User SetUserID(string userID)
        {
            ID = userID;
            return this;
        }

        public User SetNickname(string nickName)
        {
            Nickname = nickName;
            return this;
        }

        public User SetToken(string token)
        {
            Token = token;

            if (IsLogin)
            {
                DozerProtocol.Client.AddBearerToken(token);
                SavedData.SaveAccessToken(token);
            }
            else
            {
                DozerProtocol.Client.RemoveToken();
                SavedData.DeleteAccessToken();
            }

            return this;
        }

        public User SetData(UserData data)
        {
            SetMaxCoin(data.maxCoin);
            SetCoin(data.coin);

            _items.Clear();
            data.items ??= UserData.CreateNewItems();
            foreach (var item in data.items)
            {
                var userItem = new UserItem(item.id, item.value);
                userItem.OnValueChanged += (UserItem userItem) =>
                {
                    OnItemChanged?.Invoke(userItem);
                };

                _items.Add(userItem);
            }

            return this;
        }

        public UserItem GetItem(string id)
        {
            return _items.Find(item => item.ID == id);
        }

        public List<UserItem> GetItems()
        {
            return new List<UserItem>(_items);
        }

        public UserData GetUserData()
        {
            var data = new UserData()
            {
                coin = Coin,
                maxCoin = MaxCoin,
                items = _items.ConvertAll(item => new ItemData() { id = item.ID, value = item.Count }).ToArray()
            };

            Debug.Log("GetUserData: " + data.items[0].value);

            return data;
        }

        public User SetMaxCoin(int value)
        {
            MaxCoin = value;
            return this;
        }


        public User SetCoin(int value)
        {
            value = Mathf.Clamp(value, 0, MaxCoin);

            if (value == Coin)
            {
                return this;
            }

            Coin = value;

            OnCoinChanged?.Invoke(this);

            return this;
        }

        public void EarnCoin(int value)
        {
            SetCoin(Coin + value);
        }

        public void PayCoin(int value)
        {
            SetCoin(Coin - value);
        }
    }
}