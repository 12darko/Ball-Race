using UnityEngine;

namespace _Main.EMA.Scripts.MyShortcuts
{
    public class ScreenShot
    {
        public static Texture2D GetTextureFromCamera(Camera mCamera)
        {
            Rect rect = new Rect(0, 0, mCamera.pixelWidth, mCamera.pixelHeight);
            RenderTexture renderTexture = new RenderTexture(mCamera.pixelWidth, mCamera.pixelHeight, 24);
            Texture2D screenShot = new Texture2D(mCamera.pixelWidth, mCamera.pixelHeight, TextureFormat.RGBA32, false);

            mCamera.targetTexture = renderTexture;
            mCamera.Render();

            RenderTexture.active = renderTexture;

            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            mCamera.targetTexture = null;
            RenderTexture.active = null;
            return screenShot;
        }

        public static Sprite TextureToSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(texture.width / 2f, texture.height / 2f));
        }
    }
}