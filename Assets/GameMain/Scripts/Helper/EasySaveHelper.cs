using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;

public class EasySaveHelper : SettingHelperBase
{
    public override void SetObject(string settingName, object obj)
    {
        ES3.Save(settingName, obj);
    }

    public override int Count
    {
        get => GetAllSettingNames().Count();
    }

    public override bool Load()
    {
        ES3.Init();
        return true;
    }

    public override bool Save()
    {
        return true;
    }

    public override string[] GetAllSettingNames()
    {
        return ES3.GetKeys();
    }

    public override void GetAllSettingNames(List<string> results)
    {
        results = ES3.GetKeys().ToList();
    }

    public override bool HasSetting(string settingName)
    {
        return ES3.KeyExists(settingName);
    }

    public override bool RemoveSetting(string settingName)
    {
        ES3.DeleteKey(settingName);
        return !HasSetting(settingName);
    }

    public override void RemoveAllSettings()
    {
        foreach (var key in GetAllSettingNames())
            ES3.DeleteKey(key);
    }

    public override bool GetBool(string settingName)
    {
        return ES3.Load<bool>(settingName);
    }

    public override bool GetBool(string settingName, bool defaultValue)
    {
        return ES3.Load<bool>(settingName, defaultValue);
    }

    public override void SetBool(string settingName, bool value)
    {
        ES3.Save(settingName, value);
    }

    public override int GetInt(string settingName)
    {
        return ES3.Load<int>(settingName);
    }

    public override int GetInt(string settingName, int defaultValue)
    {
        return ES3.Load<int>(settingName, defaultValue);
    }

    public override void SetInt(string settingName, int value)
    {
        ES3.Save(settingName, value);
    }

    public override float GetFloat(string settingName)
    {
        return ES3.Load<float>(settingName);
    }

    public override float GetFloat(string settingName, float defaultValue)
    {
        return ES3.Load<float>(settingName, defaultValue);
    }

    public override void SetFloat(string settingName, float value)
    {
        ES3.Save(settingName, value);
    }

    public override string GetString(string settingName)
    {
        return ES3.Load<string>(settingName);
    }

    public override string GetString(string settingName, string defaultValue)
    {
        return ES3.Load<string>(settingName, defaultValue);
    }

    public override void SetString(string settingName, string value)
    {
        ES3.Save(settingName, value);
    }

    public override T GetObject<T>(string settingName)
    {
        return ES3.Load<T>(settingName);
    }

    public override object GetObject(Type objectType, string settingName)
    {
        return ES3.Load(settingName);
    }

    public override T GetObject<T>(string settingName, T defaultObj)
    {
        return ES3.Load<T>(settingName, defaultObj);
    }

    public override object GetObject(Type objectType, string settingName, object defaultObj)
    {
        return ES3.Load(settingName, defaultObj);
    }

    public override void SetObject<T>(string settingName, T obj)
    {
        ES3.Save(settingName, obj);
    }
}