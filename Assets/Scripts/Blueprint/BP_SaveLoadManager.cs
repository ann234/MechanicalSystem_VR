using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;

public class BP_SaveLoadManager : MonoBehaviour
{
    [SerializeField]
    private BP_Gear m_bp_gear;

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.items = array;
        return JsonUtility.ToJson(wrapper);
    }
    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }

    public void saveAll()
    {
        foreach (BP_Object bp in FindObjectsOfType<BP_Object>())
        {
            string saveData = JsonUtility.ToJson(bp);
            File.WriteAllText(Application.dataPath + "/Saves/" + bp.name + bp.GetInstanceID().ToString() + ".json", saveData);
        }
        print("Save data");
    }

    public void loadAll()
    {
        //  load objects
        string[] files = Directory.GetFiles(Application.dataPath + "/Saves/");
        BP_Object[] objs = new BP_Object[files.Length];

        //  instantiate
        foreach (string file in files)
        {
            if (Path.GetExtension(file) == ".json")
            {
                BP_Gear obj = JsonUtility.FromJson<BP_Gear>(file);
                switch(obj.m_type)
                {
                    case BP_Object.type.Gear:
                        BP_Gear gear = Instantiate<BP_Gear>(m_bp_gear);
                        gear = JsonUtility.FromJson<BP_Gear>(file);
                        break;
                    case BP_Object.type.Link:
                        break;
                    case BP_Object.type.Joint:
                        break;
                    case BP_Object.type.Shaft:
                        break;
                    case BP_Object.type.PinFollower:
                        break;
                    case BP_Object.type.SlottedBar:
                        break;
                    case BP_Object.type.EndEffector:
                        break;
                }
            }
        }

        print("load data");
    }

    // Use this for initialization
    void Start()
    {
        //string json = JsonUtility.ToJson(this);
        ////File.Create(Application.dataPath + "/Saves/data.json");
        //File.WriteAllText(Application.dataPath + "/Saves/data.json", json);
        //print(Application.dataPath + "/Saves/ 에 데이터 저장?");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
