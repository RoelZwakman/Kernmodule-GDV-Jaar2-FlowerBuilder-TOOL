using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FlowerBuilderSerialization
{
    public static void SerializeVariables(string _filepath, string _prefabname)
    {
        using (StreamWriter sw = new StreamWriter("flowerbuilderprofile.txt"))
        {
            sw.WriteLine(_filepath);
            sw.WriteLine(_prefabname);




            sw.Flush();
            sw.Close();
        }
    }

    public static string[] LoadSerializedVariables()
    {
        string[] strArrayToReturn = new string[2];

        try
        {
            using (StreamReader sr = new StreamReader("flowerbuilderprofile.txt"))
            {
                strArrayToReturn[0] = sr.ReadLine();
                strArrayToReturn[1] = sr.ReadLine();

                sr.Close();
            }
        }

        catch (System.Exception e)
        {
            Debug.Log("No flowerbuilderprofile.txt found in the root folder.");
        }

        return strArrayToReturn;
    }


}
