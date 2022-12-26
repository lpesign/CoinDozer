using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LP.Dozer
{

    public static class Constants
    {

        public static class Address
        {
            public const string API = "http://7luck-dev.spinmeta.io:3001/api/";
        }

        public static class SceneName
        {
            public const string Start = "StartScene";
            public const string Game = "GameScene";
        }

        public static class PhysicsLayer
        {
            public static int DEFAULT = 0;
            public static int IGNORE_RAYCAST = 2;
            public static int PUSHER = 10;
            public static int FLOOR = 11;
            public static int SPECIALWALL = 12;
            public static int TAP = 15;
            public static int ITEM = 20;
        }

        public static class LayerMask
        {
            public static int TAP = 1 << PhysicsLayer.TAP;
        }

        public static class SortingLayer
        {
            public const string DEFAULT = "Default";
        }

        public static class ErrorCode
        {
            public const int TOKEN_INVALID = 101;
            public const int JWT_ERROR_CODE = 102;
            public const int EMAIL_NOT_FOUND = 302;
        }
    }
}