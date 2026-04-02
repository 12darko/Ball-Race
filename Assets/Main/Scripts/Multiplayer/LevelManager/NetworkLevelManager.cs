using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.Multiplayer.LevelManager
{
    public class NetworkLevelManager : NetworkSceneManagerDefault
    {
        protected override IEnumerator LoadSceneCoroutine(SceneRef sceneRef, NetworkLoadSceneParameters sceneParams)
        {
            Debug.Log($"[NetworkLevelManager] Loading scene {sceneRef} (index={sceneRef.AsIndex})");

          //  LoadingUI.Instance?.Show("Loading...");

            // ✅ Unity async load
            var async = SceneManager.LoadSceneAsync(sceneRef.AsIndex, LoadSceneMode.Single);
            async.allowSceneActivation = true;

            while (!async.isDone)
            {
                float p = Mathf.Clamp01(async.progress / 0.9f);
              //  LoadingUI.Instance?.SetProgress(p);
                yield return null;
            }

            // Fusion pipeline için 1 frame bekle
            yield return null;

            // ✅ Barrier'a "ben yüklendim" bildir
           // var barrier = Object.FindObjectOfType<SceneLoadBarrier>();
      //      barrier?.NotifyLocalLoaded();

         //   LoadingUI.Instance?.SetProgress(1f);
         //   LoadingUI.Instance?.Hide();

            Debug.Log($"[NetworkLevelManager] Scene loaded {sceneRef} done");
        }
    }
}