using System;
using System.Linq;
using System.Reflection;
using RedDev.Helpers.Extensions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RedDev.Editor
{
	/// <summary>
	/// See: https://www.bbsmax.com/A/GBJrZ9waJ0/
	/// also: https://github.com/valyard/ReorderableListExample
	///			http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
	/// Foldout списка основан на Homebrew
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CustomArrayEditor<T>
	{
		private Object _target;
		private Type _typeTarget;
		private string _header;
		private string _description;
		private ReorderableList _list;
		public ReorderableList list => _list;
		private FieldInfo[] _fields;
		private Func<T[]> _getter;
		private Action<T[]> _setter;

		private GUIStyle _descriptionStyle;
		private GUIStyle _indexStyle;
		private GUIStyle _firstLineStyle, _secondLineStyle;
		private GUIStyle _foldoutStyle;

		private bool _isExpanded = true;
		private Color _colorFoldout;

		public Action<int> onAddedElement;

		public CustomArrayEditor(Object target, Func<T[]> getter, Action<T[]> setter, bool draggable = true)
		{
			_target = target;
			_typeTarget = typeof(T);
			_getter = getter;
			_setter = setter;

			_colorFoldout = !EditorGUIUtility.isProSkin 
								? new Color(0.2f, 0.2f, 0.2f, 1f) 
								: new Color(1, 1, 1, 0.1f);

			_list = new ReorderableList(_getter(), _typeTarget, true, true, true, true)
			{ 
				drawHeaderCallback = DrawHeaderHandler,
				drawElementCallback = DrawElementHandler,
				onAddCallback = AddItemHandler,
				onRemoveCallback = RemoveItemHandler,
				headerHeight = EditorGUIUtility.singleLineHeight * 2f
			};

			_header = _typeTarget.Name + "(s)";
			_fields = _typeTarget.GetFields();
			_description = String.Join(" | ", _fields.Select(x => x.Name).ToArray());
		}

		private void AddItemHandler(ReorderableList list)
		{
			Undo.RecordObject(_target, typeof(T).Name + " added item");
			var itemList = _getter().ToList();
			if (list.count == 0)
			{
				var item = Activator.CreateInstance<T>();
				itemList.Add(item);
				_setter(itemList.ToArray());
				list.list = _getter();
				onAddedElement.SafeCall(0);
			}
			else
			{
				var selectedIndex = list.index > 0 ? list.index : list.count - 1;
				var item = Activator.CreateInstance<T>();
				itemList.Insert(selectedIndex + 1, item);
				_setter(itemList.ToArray());
				list.list = _getter();
				onAddedElement.SafeCall(selectedIndex + 1);
			}
		}

		private void RemoveItemHandler(ReorderableList list)
		{
			if (list.index < 0)
				return;
			Undo.RecordObject(_target, _typeTarget.Name + " removed item");
			var items = _getter().ToList();
			items.RemoveAt(list.index);
			_setter(items.ToArray());
			list.list = _getter();
		}

		private void DrawHeaderHandler(Rect rect)
		{
			if (_descriptionStyle == null)
			{
				_descriptionStyle = new GUIStyle(GUI.skin.label);
				_descriptionStyle.alignment = TextAnchor.UpperCenter;
			}

			GUI.Label(rect, _description, _descriptionStyle);
		}

		private void DrawElementHandler(Rect rect, int index, bool isActive, bool isFocused)
		{
			EditorGUI.BeginChangeCheck();
			var item = DrawItem(rect, _getter()[index], index);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(_target, typeof(T).Name + " changed");
				_getter()[index] = item;
				EditorUtility.SetDirty(_target);
			}
		}

		private T DrawItem(Rect rect, T item, int index)
		{
			if (_indexStyle == null)
			{
				_indexStyle = new GUIStyle(GUI.skin.label);
				_indexStyle.normal.textColor = Color.grey;
			}

			GUI.Label(rect, index.ToString(), _indexStyle);

			rect.height = EditorGUIUtility.singleLineHeight;
			var widthArea = rect.width;
			var widthField = widthArea / _fields.Length;

			var obj = (object)item;
			foreach (var field in _fields)
			{
				var value = field.GetValue(obj);

				rect.width = widthField;

				if (field.FieldType == typeof(int))
					value = EditorGUI.IntField(rect, "", (int)value);
				else if (field.FieldType == typeof(float))
					value = EditorGUI.FloatField(rect, "", (float)value);
				else if (field.FieldType == typeof(bool))
					value = EditorGUI.Toggle(rect, "", (bool)value);
				else if (field.FieldType == typeof(Color))
					value = EditorGUI.ColorField(rect, "", (Color)value);
				else if (field.FieldType == typeof(string))
					value = EditorGUI.TextField(rect, "", (string)value);
				else if (field.FieldType == typeof(Vector2))
					value = EditorGUI.Vector2Field(rect, "", (Vector2)value);
				else if (field.FieldType == typeof(Vector3))
					value = EditorGUI.Vector3Field(rect, "", (Vector3)value);
				else if (field.FieldType == typeof(Vector4))
					value = EditorGUI.Vector4Field(rect, "", (Vector4)value);
				else if (field.FieldType == typeof(Quaternion))
					value = Quaternion.Euler(EditorGUI.Vector3Field(rect, "", ((Quaternion)value).eulerAngles));
				else if (field.FieldType.BaseType == typeof(Enum))
					value = EditorGUI.EnumPopup(rect, "", (Enum)value);
				else if (field.FieldType.BaseType == typeof(Object))
					value = EditorGUI.ObjectField(rect, "", (Object)value, field.FieldType, false);

				field.SetValue(obj, value);
				rect.x += rect.width;
			}
			return (T)obj;
		}

		public void HandleDraw()
		{
			if (_foldoutStyle == null)
				MakeFoldoutStyle();
			var rect = EditorGUILayout.BeginVertical();

			EditorGUILayout.Space();

			EditorGUI.DrawRect(new Rect(rect.x - 1, rect.y - 1, rect.width + 1, rect.height + 1), _colorFoldout);

			_isExpanded = EditorGUILayout.Foldout(_isExpanded, _header, true, _foldoutStyle != null ? _foldoutStyle : EditorStyles.foldout);
			
			EditorGUILayout.EndVertical();

			if (_isExpanded)
				_list.DoLayoutList();
		}

		private void MakeFoldoutStyle()
		{
			var uiTexIn = Resources.Load<Texture2D>("IN foldout focus-6510");
			var uiTexInOn = Resources.Load<Texture2D>("IN foldout focus on-5718");

			var colorOn = Color.white;

			_foldoutStyle = new GUIStyle(EditorStyles.foldout)
			{
				overflow = new RectOffset(-10, 0, 3, 0),
				padding = new RectOffset(25, 0, -3, 0),

				active = {textColor = colorOn, background = uiTexIn},
				onActive = {textColor = colorOn, background = uiTexInOn},
				focused = {textColor = colorOn, background = uiTexIn},
				onFocused = {textColor = colorOn, background = uiTexInOn}
			};
		}
	}
}