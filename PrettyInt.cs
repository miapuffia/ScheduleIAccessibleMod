using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedTasksMod {
	public class PrettyInt {
		private int value;

		public PrettyInt(int value) {
			this.value = value;
		}

		public static PrettyInt operator ++(PrettyInt obj) {
			obj.value = ++obj.value;
			return obj;
		}

		public override string ToString() {
			return value.ToString().PadLeft(2, '0');
		}
	}
}
