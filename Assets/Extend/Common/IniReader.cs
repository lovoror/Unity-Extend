﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using XLua;

namespace Extend.Common {
	[LuaCallCSharp]
	public class IniRead {
		private class Section {
			public string Name { get; }

			public Dictionary<string, string> KeyValue { get; } = new Dictionary<string, string>();

			public Section(string n) {
				Name = n;
			}
		}

		private readonly List<Section> iniSections = new List<Section>();
		[BlackList]
		public static IniRead Parse(TextReader reader) {
			var ini_reader = new IniRead();
			string line;
			while( ( line = reader.ReadLine() ) != null ) {
				line = TrimComment(line);
				if( line.Length == 0 ) continue;
				if( line.StartsWith("[") && line.Contains("]") ) {
					var index = line.IndexOf(']');
					var name = line.Substring(1, index - 1).Trim();
					var foundSection = ini_reader.iniSections.Find(x => x.Name == name);
					if( foundSection == null )
						ini_reader.iniSections.Add(new Section(name));
					else
						continue;
				}

				if( line.Contains("=") ) {
					var index = line.IndexOf('=');
					var key = line.Substring(0, index).Trim();
					var value = line.Substring(index + 1).Trim();
					ini_reader.iniSections.Last().KeyValue.Add(key, value);
				}
			}

			return ini_reader;
		}

		private static string TrimComment(string s) {
			if( s.Contains(";") ) {
				var index = s.IndexOf(';');
				s = s.Substring(0, index).Trim();
			}

			return s;
		}

		public int GetInt(string section, string key) {
			return int.Parse(iniSections.Find(x => x.Name == section).KeyValue[key]);
		}

		public bool GetBool(string section, string key) {
			return ( iniSections.Find(x => x.Name == section).KeyValue[key] == "true" ) || 
			       ( iniSections.Find(x => x.Name == section).KeyValue[key] == "1" );
		}

		public string GetString(string section, string key) {
			return iniSections.Find(x => x.Name == section).KeyValue[key];
		}

		public double GetDouble(string section, string key) {
			return double.Parse(iniSections.Find(x => x.Name == section).KeyValue[key]);
		}

		private IniRead() {
		}
	}
}