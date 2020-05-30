using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CardBase : MonoBehaviour
{
    public static string[] Fields;
    public static List<string[]> CardCollection;

    public static void Parse(string file)
    {
        StreamReader sr = null;
        bool unlock = false;
        while (!unlock)
        {
            try
            {
                sr = new StreamReader(file);
                unlock = true;
            }
            catch(IOException ex)
            {
                unlock = false;
                Debug.Log(ex.Message);
            }
        }
        
        string temp = sr.ReadLine();
        Fields = temp.Split('\t');
        while (!sr.EndOfStream)
        {
            string line = sr.ReadLine();
            string[] data = line.Split('\t');
            CardCollection.Add(data);
        }
        sr.Close();
    }

    private static int FieldIndex(string Field)
    {
        for (int i = 0; i < Fields.Length; i++)
        {
            if (Fields[i] == Field)
            {
                return i;
            }
        }
        return -1;
    }

    private static string[] FieldData(List <string[]> Collection, string Field)
    {
        int index = FieldIndex(Field);
        if (index != -1)
        {
            for (int i = 0; i < Collection.Count; i++)
            {
                if (Collection[i][index] == Field)
                {
                    return Collection[i];
                }
            }
        }
        return null;
    }

    public static string FieldValue(string[] FieldData, string Field)
    {
        int index = FieldIndex(Field);
        if (index != -1)
            return FieldData[index];
        return "";
    }

    public static int FieldDataIndex(List<string[]> Collection, string Field, string Value)
    {
        int index = FieldIndex(Field);
        for (int i = 0; i < Collection.Count; i++)
        {
            if (Collection[i][index] == Value)
            {
                return i;
            }
        }
        return -1;
    }

    public static List<string[]>GetDataByField(List <string[]> Collection, string Field, string Value, bool Multiple)
    {
        int index = FieldIndex(Field);
        List<string[]> Found = new List<string[]>();

        for (int i = 0; i < Collection.Count; i++)
        {
            if (Collection[i][index]==Value)
            {
                Found.Add(Collection[i]);
                if (!Multiple)
                {
                    return Found;
                }
            }
        }
        return Found;
    }

    public static int GetIndex(string Name)
    {
        for(int i = 0; i< CardCollection.Count; i++)
        {
            if (CardBase.FieldValue(CardCollection[i], Global.NameHeader) == Name)
            {
                return i;
            }
        }
        return -1;
    }
}
