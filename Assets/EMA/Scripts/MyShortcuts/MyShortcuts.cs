using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EMA.Scripts.MyShortcuts
{
    public static class MyShortcuts
    {
        public static T RandomEnumValue<T>()
        {
            var _values = Enum.GetValues(typeof(T));
            var _random = UnityEngine.Random.Range(0, _values.Length);
            return (T)_values.GetValue(_random);
        }
        
        public static bool GetRandomBoolByChance(float chanceByPercent)
        {
            var _rnd = Random.Range(0f, 1f);

            return _rnd <= chanceByPercent;
        }
        
        public static T GetRandomObjectOfList<T>(List<T> list)
        {
            var _rand = Random.Range(0, list.Count);

            return list[_rand];
        }
        
        public static T GetRandomObjectOfList<T>(T[] list)
        {
            var _rand = Random.Range(0, list.Length);

            return list[_rand];
        }

        public static void GetScreenShot()
        {
            ScreenCapture.CaptureScreenshot((System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+ "/ScreenShots/" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".png"));
            //UnityEditor.AssetDatabase.Refresh();
        }
        
        public static void DeleteChildrenOfList(Transform parent)
        {
            while (parent.childCount != 0) {
                MonoBehaviour.DestroyImmediate(parent.GetChild(0).gameObject);
            }
        }
    }
}
