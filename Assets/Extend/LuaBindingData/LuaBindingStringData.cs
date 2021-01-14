using System;
using XLua;

namespace Extend.LuaBindingData {
	[Serializable, UnityEngine.Scripting.Preserve]
	public class LuaBindingStringData : LuaBindingDataBase {
		public string Data;
		public override void ApplyToLuaInstance(LuaTable instance) {
			instance.Set(FieldName, Data);
		}
	}
}