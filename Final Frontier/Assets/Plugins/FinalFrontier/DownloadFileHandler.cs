using System.Collections;
using System.IO;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class DownloadFileHandler : MonoBehaviour
{
    private string url;
    private Stream stream;
    private bool success;
    private string error;

    IEnumerator DownloadFile(string URL, string FilePath, Action<UnityWebRequest> callback)
    {
        var uwr = new UnityWebRequest(URL, UnityWebRequest.kHttpVerbGET);
        uwr.downloadHandler = new DownloadHandlerFile(Path.Combine(Global.Root, FilePath));
        yield return uwr.SendWebRequest();
        callback(uwr);
    }

    public bool GetFileFromURL(string URL, string FilePath)
    {
        bool rslt = true;
        if (!File.Exists(FilePath))
        {
            StartCoroutine(DownloadFile(URL, FilePath, (UnityWebRequest req) =>
            {
                if (req.isNetworkError || req.isHttpError)
                {
                    Debug.Log($"{req.error}: {req.downloadHandler.text}");
                    rslt = false;
                }
                else
                {
                    while (!req.downloadHandler.isDone) { }
                    rslt = true;
                }
            }));
        }
        else
            rslt = true;
        return rslt;
    }

    public bool GetFileFromURL(int CardIndex)
    {
        string image;
        if (CardIndex == -1)
            image = "cardback";
        else
            image = CardBase.FieldValue(CardBase.CardCollection[CardIndex], "ImageFile");
        string url = Global.CardGeneralURLs + @"/" + image + ".jpg";
        string path = Global.ImageDir + @"\" + image + ".jpg";
        return GetFileFromURL(url, path);
    }
    
    public bool SaveFile(string URL, string FileName)
    {

        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }
        string dir = Path.GetDirectoryName(FileName);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string name = Path.GetFileNameWithoutExtension(FileName);
        return GetFileFromURL(URL, FileName);
    }
}