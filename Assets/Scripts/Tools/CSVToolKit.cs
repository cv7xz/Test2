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
            
        foreach(var con in content)   //con ��һ�� List<string>����   ÿһ����Ԫ����string
        {
            StaticNumber.brokenBaseDamage.Add(float.Parse(con[1]));
        }
    }
}
