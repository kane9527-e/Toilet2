using System;
using GameMain.Scripts.Definition.Constant;
using UnityEngine;
using UnityGameFramework.Runtime;

public static class DataTableExtension
{
    private const string DataRowClassPrefixName = "GameMain.Scripts.DataTable.DR";
    internal static readonly char[] DataSplitSeparators = { '\t' };
    internal static readonly char[] DataTrimSeparators = { '\"' };

    public static void LoadDataTable(this DataTableComponent dataTableComponent, string dataTableName,
        string dataTableAssetName, object userData)
    {
        if (string.IsNullOrEmpty(dataTableName))
        {
            Log.Warning("Data table name is invalid.");
            return;
        }

        var splitedNames = dataTableName.Split('_');
        if (splitedNames.Length > 2)
        {
            Log.Warning("Data table name is invalid.");
            return;
        }

        var dataRowClassName = DataRowClassPrefixName + splitedNames[0];
        var dataRowType = Type.GetType(dataRowClassName);
        if (dataRowType == null)
        {
            Log.Warning("Can not get data row type with class name '{0}'.", dataRowClassName);
            return;
        }

        var name = splitedNames.Length > 1 ? splitedNames[1] : null;
        var dataTable = dataTableComponent.CreateDataTable(dataRowType, name);
        dataTable.ReadData(dataTableAssetName, Constant.AssetPriority.DataTableAsset, userData);
    }

    public static Color32 ParseColor32(string value)
    {
        var splitedValue = value.Split(',');
        return new Color32(byte.Parse(splitedValue[0]), byte.Parse(splitedValue[1]), byte.Parse(splitedValue[2]),
            byte.Parse(splitedValue[3]));
    }

    public static Color ParseColor(string value)
    {
        var splitedValue = value.Split(',');
        return new Color(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]),
            float.Parse(splitedValue[3]));
    }

    public static Quaternion ParseQuaternion(string value)
    {
        var splitedValue = value.Split(',');
        return new Quaternion(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]),
            float.Parse(splitedValue[3]));
    }

    public static Rect ParseRect(string value)
    {
        var splitedValue = value.Split(',');
        return new Rect(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]),
            float.Parse(splitedValue[3]));
    }

    public static Vector2 ParseVector2(string value)
    {
        var splitedValue = value.Split(',');
        return new Vector2(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]));
    }

    public static Vector3 ParseVector3(string value)
    {
        var splitedValue = value.Split(',');
        return new Vector3(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]));
    }

    public static Vector4 ParseVector4(string value)
    {
        var splitedValue = value.Split(',');
        return new Vector4(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]),
            float.Parse(splitedValue[3]));
    }
}