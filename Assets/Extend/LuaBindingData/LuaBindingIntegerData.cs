using System;
using UnityEngine;
using XLua;

namespace Extend.LuaBindingData {
	[Serializable, UnityEngine.Scripting.Preserve]
	public class LuaBindingIntegerData : LuaBindingDataBase {
		public int Data;
		public override void ApplyToLuaInstance(LuaTable instance) {
			instance.Set(FieldName, Data);
		}
	}
}