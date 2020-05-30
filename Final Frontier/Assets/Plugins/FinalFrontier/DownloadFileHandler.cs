using System.Collections;
using System.IO;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadFileHandler : MonoBehaviour
{
    private string url;
    private Stream stream;
    private bool success;
    private string error;

    IEnumerator DownloadFile(string URL, string FilePath)
    {
        var uwr = new UnityWebRequest(URL, UnityWebRequest.kHttpVerbGET);
        uwr.downloadHandler = new DownloadHandlerFile(Path.Combine(Global.Root, FilePath));
        yield return uwr.SendWebRequest();
        
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            success = false;
            error = uwr.error;
        }
        else
        {
            Debug.Log("File successfully downloaded and saved to " + Path.Combine(Global.Root, FilePath));
        }
    }

    public bool GetFileFromURL(string URL, string FilePath)
    {
        error = "";
        if (!File.Exists(FilePath))
        {

            StartCoroutine(DownloadFile(URL, FilePath));
            return success;
        }
        else
        {
            return true;
        }
    }

    public bool GetFileFromURL(int CardIndex)
    {
        string image = CardBase.FieldValue(CardBase.CardCollection[CardIndex], "ImageFile");
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