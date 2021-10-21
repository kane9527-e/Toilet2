using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("name", "sceneId", "_dataDic")]
	public class ES3UserType_Progress : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Progress() : base(typeof(GameMain.Scripts.Base.Struct.Progress)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (GameMain.Scripts.Base.Struct.Progress)obj;
			
			writer.WriteProperty("name", instance.name, ES3Type_string.Instance);
			writer.WriteProperty("sceneId", instance.sceneId, ES3Type_int.Instance);
			writer.WritePrivateField("_dataDic", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (GameMain.Scripts.Base.Struct.Progress)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "name":
						instance.name = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "sceneId":
						instance.sceneId = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "_dataDic":
					reader.SetPrivateField("_dataDic", reader.Read<System.Collections.Generic.Dictionary<System.String, System.Object>>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new GameMain.Scripts.Base.Struct.Progress();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_ProgressArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ProgressArray() : base(typeof(GameMain.Scripts.Base.Struct.Progress[]), ES3UserType_Progress.Instance)
		{
			Instance = this;
		}
	}
}