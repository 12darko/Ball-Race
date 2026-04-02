using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Fusion;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Multiplayer.Mutlplayer.NetworkMisc
{
    public static class Misc
    {
        public static Vector3 GetRandomSpawnPoint()
        {
            return new Vector3(Random.Range(-8, 5), 0.99f, 0);
        }
        public static Vector3 GetRandomForPlayerSpawnPoint(NetworkRunner runner,PlayerRef playerRef)
        {
            return new Vector3((playerRef.RawEncoded % runner.Config.Simulation.PlayerCount) *1, 1.29f, Random.Range(-170, -180));
        }
        
        public static Vector3 GetSpecificPlayerSpawnPoint(Vector3 playerPos)
        {
            return playerPos;
        }
        public static Vector3 GetRandomForPlayerDeadAreaSpawnPoint()
        {
            return new Vector3(Random.Range(-108,-144), 0.99f, Random.Range(9,-17));
        }
        
        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }
        
        private const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
            + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
            + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
        
        
        public static T Deserialize<T>(string input)
        {
            if (typeof(T) == typeof(string)) return (T)(object)input;
            return JsonConvert.DeserializeObject<T>(input);
        }
        
        public static void ClearChildren(this Transform t)
        {
            var children = t.Cast<Transform>().ToArray();
 
            foreach (var child in children)
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }
        
        public static bool ValidateEmail (string email)
        {
            if (email != null)
                return Regex.IsMatch (email, MatchEmailPattern);
            else
                return false;
        }
        
           
        public static IEnumerator ScrollMessageBoxToBottom (ScrollRect scrollRect) {
            // Wait for end of frame AND force update all canvases before setting to bottom.
            yield return new WaitForEndOfFrame ();
            Canvas.ForceUpdateCanvases ();
            scrollRect.verticalNormalizedPosition = 0f;
            Canvas.ForceUpdateCanvases ();
        }
        
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input[0].ToString().ToUpper() + input.Substring(1)
            };
    }
}