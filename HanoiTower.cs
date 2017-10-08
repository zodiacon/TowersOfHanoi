using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TowersOfHanoi {
	class HanoiTower {
		public int Discs { get; private set; }
		int _moveCounter;

		public HanoiTower(int discs) {
			Discs = discs;
		}

		public IEnumerable<HanoiMove> Solve() {
			_moveCounter = 0;
			return MoveDiscs(Discs, 1, 3, 2);			
		}

		public int MoveCounter {
			get { return _moveCounter; }
		}

		private IEnumerable<HanoiMove> MoveDiscs(int discs, int from, int to, int via) {
			if(discs == 1) {
				_moveCounter++;
				yield return new HanoiMove(from, to);
				yield break;
			}
			foreach(var move in MoveDiscs(discs - 1, from, via, to)) {
				yield return move;
			}
			foreach(var move in MoveDiscs(1, from, to, 0)) {
				yield return move;
			}
			foreach(var move in MoveDiscs(discs - 1, via, to, from)) {
				yield return move;
			}
		}

	}

}
