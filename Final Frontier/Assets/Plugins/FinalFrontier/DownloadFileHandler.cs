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
            Debug.print(uwr.error);
            success = false;
            error = uwr.error;
        }
        else
        {
            Debug.print("File successfully downloaded and saved to " + Path.Combine(Global.Root, FilePath));
            success = true;
        }
    }

    public bool GetFileFromURL(string URL, string FilePath)
    {
        StartCoroutine(DownloadFile(URL, FilePath));
        return success;
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