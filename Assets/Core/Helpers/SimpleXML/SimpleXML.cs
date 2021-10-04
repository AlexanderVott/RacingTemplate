using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleXML
{
	public class XMLDoc
	{
		public Dictionary<string, string> attributes = new Dictionary<string, string>();
		public Dictionary<string, string> keyValues = new Dictionary<string, string>();
		private bool m_buildKeyValues = false;

		public XMLDoc parentNode;
		public string nodeName;
		public string nodeValue;

		public List<XMLDoc> childs = new List<XMLDoc>();

		public XMLDoc this[string nodeNameParamter] => childs.Find(x => x.nodeName == nodeNameParamter.ToLower());
		public XMLDoc this[Predicate<XMLDoc> match] => childs.Find(match);

		public XMLDoc(XMLDoc parent, bool buildKeyValues)
		{
			parentNode = parent;
			m_buildKeyValues = buildKeyValues;
		}

		public XMLDoc(string source, bool buildKeyValues)
		{
			m_buildKeyValues = buildKeyValues;

			int pos = 0;
			BuildFromString(source, ref pos);
			TrimRoot();
		}

		public string GetAttribute(string key)
		{
			attributes.TryGetValue(key.ToLower(), out var result);
			return result;
		}

		public bool HasAttribute(string key) 
            => attributes.ContainsKey(key.ToLower());

        public XMLDoc FindChild(string nodeName)
		{
			if (childs.Count == 0)
			{
				return null;
			}

			nodeName = nodeName.ToLower();
			foreach (var itr in childs)
			{
				if (itr.nodeName == nodeName)
				{
					return itr;
				}
			}

			return null;
		}

		public XMLDoc NextSibling()
		{
			if (parentNode == null)
			{
				return null;
			}

			var i = parentNode.childs.IndexOf(this);
			if (i == -1)
			{
				return null;
			}
			++i;
			if (i >= parentNode.childs.Count)
			{
				return null;
			}
			return parentNode.childs[i];
		}

		public bool GetAttribDef(string name, bool def) 
            => attributes.TryGetValue(name.ToLower(), out var tmp)
                   ? bool.TryParse(tmp, out var result) ? result : def
                   : def;

        public string GetAttribDef(string name, string def) 
            => attributes.TryGetValue(name.ToLower(), out var tmp) ? tmp : def;

        public int GetAttribDef(string name, int def) 
            => attributes.TryGetValue(name.ToLower(), out var tmp)
                   ? int.TryParse(tmp, out var result) ? result : def
                   : def;

        public float GetAttribDef(string name, float def) 
            => attributes.TryGetValue(name.ToLower(), out var tmp)
                   ? float.TryParse(tmp, out var result) ? result : def
                   : def;

        public string GetValByKey(string name, string def) 
            => keyValues.TryGetValue(name.ToLower(), out var tmp) ? tmp : def;

        public bool GetValByKey(string name, bool def) 
            => keyValues.TryGetValue(name.ToLower(), out var tmp)
                   ? bool.TryParse(tmp, out var result) ? result : def
                   : def;

        public int GetValByKey(string name, int def) 
            => keyValues.TryGetValue(name.ToLower(), out var tmp)
                   ? int.TryParse(tmp, out var result) ? result : def
                   : def;

        public float GetValByKey(string name, float def) 
            => keyValues.TryGetValue(name.ToLower(), out var tmp)
                   ? float.TryParse(tmp, out var result) ? result : def
                   : def;

        private void TrimRoot() {
            if (childs.Count != 1)
                return;

            var tmp = childs[0];
            var tmpChilds = childs;
            childs = childs[0].childs;

            var tmpAtrr = tmp.attributes;
            tmp.attributes = attributes;
            attributes = tmpAtrr;

            var tmpKeyVals = tmp.keyValues;
            tmp.keyValues = keyValues;
            keyValues = tmpKeyVals;

            tmp.childs = tmpChilds;
            tmpChilds.Clear();

            foreach (var itr in childs)
                itr.parentNode = this;
        }

		private XMLDoc AddChild()
		{
			var result = new XMLDoc(this, m_buildKeyValues);
			childs.Add(result);
			return result;
		}

		private void BuildFromString(string FS, ref int pos)
		{
			var stopChars = new HashSet<char>() { '!', '?' };

			Func<int, char, int> Find = (from, subchar) =>
			{
				var result = from;
				var inSafeZone = false;
				while (result < FS.Length)
				{
					if (FS[result] == '"')
					{
						inSafeZone = !inSafeZone;
					}

					if ((FS[result] == subchar) && !inSafeZone)
					{
						return result;
					}
					++result;
				}
				return result;
			};

			Func<int, char, int> FindBack = (from, subchar) =>
			{
				var result = from;
				var inSafeZone = false;
				while (result >= 0)
				{
					if (FS[result] == '"')
					{
						inSafeZone = !inSafeZone;
					}

					if ((FS[result] == subchar) && !inSafeZone)
					{
						return result;
					}
					--result;
				}
				return result;
			};

			int opt;
			int t;
			var clt = pos;
			while (clt < FS.Length)
			{
				opt = clt;

				// find open tag
				opt = Find(opt, '<');

				// finish
				if (opt + 1 >= FS.Length)
				{
					pos = opt + 1;
					return;
				}

				if ((opt + 1 < FS.Length) && (FS[opt + 1] == '/'))
				{
					// try get value
					t = FindBack(opt - 1, '>');
					if (t != opt - 1)
					{
						nodeValue = FS.Substring(t + 1, opt - t - 1).Trim();
					}

					// get out here
					pos = opt + 1;
					return;
				}

				// skip
				while ((opt + 1 < FS.Length) && (stopChars.Contains(FS[opt + 1])))
				{
					if ((opt + 3 < FS.Length) && (FS[opt + 2] == '-') && (FS[opt + 3] == '-'))
					{
						opt += 3;
						while (opt < FS.Length)
						{
							++opt;
							opt = Find(opt, '>');
							if ((FS[opt - 1] == '-') && (FS[opt - 2] == '-'))
							{
								opt = Find(opt, '<');
								break;
							}
						}
					}
					else
					{
						++opt;
						opt = Find(opt, '<');
					}
				}

				if (opt >= FS.Length)
				{
					break;
				}

				// find close tag
				clt = Find(opt + 1, '>');
				if (FS[clt - 1] == '/')
				{
					AddChild().ConstructFrom(FS.Substring(opt + 1, clt - opt - 2));
					pos = clt + 1;
				}
				else
				{
					var child = AddChild();
					child.ConstructFrom(FS.Substring(opt + 1, clt - opt - 1));
					pos = clt + 1;
					child.BuildFromString(FS, ref pos);
				}

				clt = pos;
			}
		}

		private enum ConstructFromState { SearchName, SearchAttrName, SearchAttrValue };
		private void ConstructFrom(string source)
		{
			var state = ConstructFromState.SearchName;
			int c = 0;
			int s = 0;
			int last_e = 0;

			string attrName = "";
			string attrValue = "";
			string keyName = "";
			string keyValue = "";

			var stopChars = new HashSet<char>() { ' ', '=', (char)9, (char)10, (char)13 };
			var delimeterChars = new HashSet<char>() { ' ', (char)9, (char)10, (char)13 };

			Action XTrimLeft = () =>
			{
				while ((c < source.Length) && (stopChars.Contains(source[c])))
				{
					++c;
					++s;
				}
			};

			Action XTrimRight = () =>
			{
				while ((c < source.Length) && (!stopChars.Contains(source[c])))
				{
					++c;
				}
			};

			Func<bool> CheckForEqual = () =>
			{
				last_e = c;
				while ((last_e < source.Length) && (delimeterChars.Contains(source[last_e])))
				{
					++last_e;
				}

				return source[last_e] == '=';
			};

			if (string.IsNullOrEmpty(source))
			{
				Debug.LogWarning("XML: Void group detected");
			}

			while (c < source.Length)
			{
				s = c;
				switch (state)
				{
					case ConstructFromState.SearchName:
						{
							XTrimLeft();
							XTrimRight();
							if ((c + 1 < source.Length) && (CheckForEqual()))
							{
								state = ConstructFromState.SearchAttrValue;
								attrName = source.Substring(s, c - s);
								c = last_e;
							}
							else
							{
								state = ConstructFromState.SearchAttrName;
								nodeName = source.Substring(s, c - s).ToLower();
							}
						}
						break;

					case ConstructFromState.SearchAttrName:
						{
							XTrimLeft();
							XTrimRight();
							if ((c + 1 < source.Length) && (CheckForEqual()))
							{
								state = ConstructFromState.SearchAttrValue;
								attrName = source.Substring(s, c - s);
								c = last_e;
							}
							else
							{
								// void attrib
								state = ConstructFromState.SearchAttrName;
								attrName = source.Substring(s, c - s);
								attrValue = "";
								attributes.Add(attrName.ToLower(), attrValue);
							}
						}
						break;

					case ConstructFromState.SearchAttrValue:
						{
							XTrimLeft();
							if (source[s] != '"')
							{
								Debug.LogWarning("XML: Expected value not found");
								state = ConstructFromState.SearchAttrName;
							}
							else
							{
								if (c + 1 <= source.Length)
								{
									++c;
								}
								else
								{
									Debug.LogWarning("XML: Value not closed");
									return;
								}

								while ((c < source.Length) && (source[c] != '"'))
								{
									++c;
								}

								if (source[c] != '"')
								{
									Debug.LogWarning("XML: Value not closed");
									return;
								}
								else
								{
									attrValue = source.Substring(s + 1, c - s - 1);
									attrValue = attrValue.Replace("&quot;", "\"");
									attributes.Add(attrName.ToLower(), attrValue);
									if (m_buildKeyValues)
									{
										switch (attrName)
										{
											case "key":
												keyName = attrValue.ToLower();
												break;
											case "val":
												keyValue = attrValue;
												break;
										}
									}

									state = ConstructFromState.SearchAttrName;
									++c;
								}
							}
						}
						break;
				}
			}

			if ((parentNode != null) && (m_buildKeyValues))
			{
				if (!string.IsNullOrEmpty(keyName))
				{
					parentNode.keyValues[keyName] = keyValue;
				}
			}
		}
	}
}