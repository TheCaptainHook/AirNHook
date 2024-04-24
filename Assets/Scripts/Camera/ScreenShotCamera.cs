using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
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
        File.WriteAllBytes($"{path}/Test.png", bytes);

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

    //public async void ScreenShot()
    //{
    //    string path = Path.Combine(Application.dataPath, "UserMapData");
    //    Task delay = Task.Run(() => Task.Delay(1));
    //    Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
    //    RenderTexture.active = rt;
    //    texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
    //    texture.Apply();

    //    byte[] bytes = texture.EncodeToPNG();
    //    await delay;

    //    Debug.Log("딜레이");
    //    File.WriteAllBytes($"{path}/Test.png", bytes);

    //}




    //public void StartScreenShotRender()
    //{
    //    StartCoroutine(StartRender());
    //}

    IEnumerator StartRender()
    {
        string path = Path.Combine(Application.dataPath, "UserMapData");
        yield return new WaitForSeconds(0.5f);
        File.WriteAllBytes($"{path}/Test.png", resultBytes);
        yield return new WaitForSeconds(0.5f);
        
        gameObject.SetActive(false);
    }

}
