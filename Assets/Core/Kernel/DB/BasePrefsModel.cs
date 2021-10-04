using System;
using System.IO;
using System.Text;
using RedDev.Helpers.Extensions;
using UnityEngine;

namespace RedDev.Kernel.DB
{
	[Serializable]
	public abstract class BasePrefsModel : IBasePrefsModel
	{
		private const string _allowedChars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

		#region Fields and properties
		public bool isSaveAllowed { get; private set; } = true;

		private bool _isLastLoadFailed = false;

		public bool isLastLoadFailed
		{
			get => _isLastLoadFailed;
			set => _isLastLoadFailed = value;
		}

		private string _modelName = null;
		public string modelName => _modelName == null ? _modelName = GetModelName() : _modelName;

		[SerializeField]
		private ulong playerId = 0;
		#endregion

		#region Virtual methods
		/// <summary>
		/// Метод создаёт пустую модель, сохраняет её и загружает заново.
		/// </summary>
		public virtual void Reset()
		{
			var defaultData = Activator.CreateInstance(GetType()) as BasePrefsModel;
			defaultData.isSaveAllowed = true;
			defaultData.Save();
			Load();
			PostLoad();
		}

		public virtual void Load()
		{
			_isLastLoadFailed = false;
			var tmpSaveAllowed = isSaveAllowed;
			isSaveAllowed = false;
			try
			{
				var path = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "Prefs";
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);

				var fileName = path + Path.AltDirectorySeparatorChar + modelName;
				if (!File.Exists(fileName))
				{
					Reset();
					return;
				}

				string source = null;
				try
				{
					source = Encoding.UTF8.GetString(File.ReadAllBytes(fileName));
				}
				catch (Exception except)
				{
					isLastLoadFailed = true;
                    Prod.LogError($"[System] Fail read data: {except.Message}");
					Reset();
				}

				if (!String.IsNullOrEmpty(source))
				{
					isSaveAllowed = false;
					JsonUtility.FromJsonOverwrite(source, this);
					isSaveAllowed = true;
					if (playerId != 0) // TODO: overlay player id
					{
						isLastLoadFailed = true;
						Reset();
					}
				}
			}
			catch
			{
				isLastLoadFailed = true;
                Prod.LogError($"[System] Data loss: cant load {GetType().Name}");
				Reset();
			}
			isSaveAllowed = tmpSaveAllowed;
		}

		public virtual void Save()
		{
			try
			{
				if (isSaveAllowed)
				{
					playerId = 0;// TODO: overlay player id
					var path = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "Prefs";
					if (!Directory.Exists(path))
						Directory.CreateDirectory(path);

					var fileName = path + Path.AltDirectorySeparatorChar + modelName;
					if (File.Exists(fileName))
						File.Delete(fileName);

                    using var fs = new FileStream(fileName, FileMode.Create);
                    using var binWriter = new BinaryWriter(fs);
                    var source = JsonUtility.ToJson(this);
                    binWriter.Write(Encoding.UTF8.GetBytes(source));
                }
			}
			catch (Exception except)
			{
                Prod.LogError($"[System] Data loss: can't save {GetType().Name}: {except.Message}");
			}
		}

		public virtual void PostLoad() { }
		#endregion

		#region Private methods for properties
		private string GetModelName()
		{
			var bytes = BitConverter.GetBytes(GetType().Name.GetHashCode()).Add(BitConverter.GetBytes(0));
			return EncodeName(bytes);
		}

		private string EncodeName(byte[] bytes)
		{
			var result = "";
			foreach (var b in bytes)
				result += _allowedChars[b % _allowedChars.Length];
			return result;
		}
		#endregion
	}
}