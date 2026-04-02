using System;
using System.Reflection;
using _Main.Scripts.Multiplayer.Multiplayer.Modes;
using Newtonsoft.Json;
using Steamworks.Data;
using Unity.VisualScripting;
using UnityEngine;

namespace Other
{
    public static class MiscGlobal 
    {
        public static T Deserialize<T>(string input)
        {
            if (typeof(T) == typeof(string)) return (T)(object)input;
            return JsonConvert.DeserializeObject<T>(input);
        }

        public static void DestroyNetworkGameManager()
        {
       //     Object.Destroy(NetworkGameManager.Instance.gameObject);
        }
        public static Texture2D ConvertSteamPicture( Image image )
        {
            // Create a new Texture2D
            var avatar = new Texture2D( (int)image.Width, (int)image.Height, TextureFormat.ARGB32, false );
	
            // Set filter type, or else its really blury
            avatar.filterMode = FilterMode.Trilinear;

            // Flip image
            for ( int x = 0; x < image.Width; x++ )
            {
                for ( int y = 0; y < image.Height; y++ )
                {
                    var p = image.GetPixel( x, y );
                    avatar.SetPixel( x, (int)image.Height - y, new UnityEngine.Color( p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f ) );
                }
            }
	
            avatar.Apply();
            return avatar;
        }

        public static GameModeCategory GetGameMode(InGameMode mode)
        {
            GameModeCategory mainCategory = mode switch
            {
                InGameMode.Standard_Mode => GameModeCategory.Arcade_Physics,
                InGameMode.Lap_Mode => GameModeCategory.Arcade_Physics,
                InGameMode.Fall_Balls => GameModeCategory.Fall_Game,
                InGameMode.Team_Match => GameModeCategory.Arcade_Physics,
                InGameMode.Team_Soccer_Match => GameModeCategory.Arcade_Physics,
                _ => GameModeCategory.Arcade_Physics
            };

            return mainCategory;
        }
        
        public static string GetGameModeIsTeam(InGameMode mode)
        {
            var isTeam = mode switch
            {
                InGameMode.Standard_Mode => "STANDART_MODE",
                InGameMode.Lap_Mode => "STANDART_MODE",
                InGameMode.Fall_Balls => "STANDART_MODE",
                InGameMode.Team_Match => "TEAM_MODE",
                InGameMode.Team_Soccer_Match => "TEAM_MODE",
                _ => "STANDART_MODE"
            };

            return isTeam;
        }
    }
}