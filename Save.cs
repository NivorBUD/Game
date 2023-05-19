using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Game;

public class Save
{
    private static string FileText;
    public static Dictionary<string, string> SaveInfo;

    public Save()
    {
        MakeSaveInfo();
    }

    public void RewriteSave(int[] saveInfo)
    {
        string str = "";
        foreach (int e in saveInfo)
            str += e.ToString() + " ";
        using (StreamWriter file = new StreamWriter(@"..\..\Saves\Save.txt"))
        {
            file.Write(str);
        }
        MakeSaveInfo();
    }

    private static void MakeSaveInfo(DirectoryInfo savesDirectory = null)
    {
        var way = @"..\..\Saves\";
        if (savesDirectory == null)
            savesDirectory = new DirectoryInfo(way);
        var save = savesDirectory.GetFiles("*.txt").First();
        var saveInfo = File.ReadAllText(save.FullName).Split();

        SaveInfo = new();
        SaveInfo["IsFirstRace"] = saveInfo[0];
        SaveInfo["IsFirstWin"] = saveInfo[1];
        SaveInfo["Balance"] = saveInfo[2];
        SaveInfo["StartPlace"] = saveInfo[3];
        SaveInfo["Speed"] = saveInfo[4];
        SaveInfo["DRSTime"] = saveInfo[5];
        SaveInfo["DRSBoost"] = saveInfo[6];
        SaveInfo["Boost"] = saveInfo[7];
        SaveInfo["Control"] = saveInfo[8];
    }
}