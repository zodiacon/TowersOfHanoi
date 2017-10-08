using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TowersOfHanoi {
	sealed class HanoiMove {
		public readonly int From;
		public readonly int To;

		public HanoiMove(int from, int to) {
			From = from; To = to;
		}

		public override string ToString() {
			return string.Format("Move {0} to {1}", From, To);
		}
	}
}
