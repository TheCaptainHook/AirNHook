using System.Collections;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

public class ScreenShotCamera : MonoBehaviour
{
    public RenderTexture rt;
    public byte[] resultBytes;

    public async Task<byte[]> ScreenShot()
    {
        await Delay();

        string path = Path.Combine(Application.dataPath, "UserMapData");
        Task<byte[]> encodingTask = null;

        Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        RenderTexture.active = rt;
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture.Apply();


        encodingTask = EncodeToPNG(texture);
        byte[] bytes = await encodingTask;


        Debug.Log(encodingTask);

        return bytes;

    }


    private async Task<byte[]> EncodeToPNG(Texture2D texture)
    {
        Debug.Log("Encode");
        return texture.EncodeToPNG();
    }

    public async Task Delay()
    {
        rt = Resources.Load<RenderTexture>("RenderTexture/ScreenShotRenderTexture");
        Task delayTask = Task.Delay(1000);
        await delayTask;

    }



}
