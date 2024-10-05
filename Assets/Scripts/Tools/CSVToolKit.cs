using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public static class CSVToolKit
{
    public static List<string[]> content = new List<string[]>();
    public static void LoadFile(string path)
    {
        StreamReader sr = null;
        sr = File.OpenText(path);

        string line;
        while((line = sr.ReadLine()) != null)
        {
            content.Add(line.Split(","));
        }
        sr.Close();
            
        foreach(var con in content)   //con 是一行 List<string>类型   每一个单元格是string
        {
            StaticNumber.brokenBaseDamage.Add(float.Parse(con[1]));
        }
    }
}
