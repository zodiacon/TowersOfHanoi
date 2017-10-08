using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows;

namespace TowersOfHanoi {
	class HanoiViewModel : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void RaisePropertyChanged(string name) {
			var pc = PropertyChanged;
			if(pc != null)
				pc(this, new PropertyChangedEventArgs(name));
		}

		int _discs;

		public int Discs {
			get { return _discs; }
			set {
				if(value < 2 || value > 16 || _discs == value)
					return;
				_discs = value;
				_tower = new HanoiTower(value);
				RaisePropertyChanged(nameof(Discs));
				RaisePropertyChanged(nameof(MoveCounter));
				Moves = null;
				IsRunning = false;
				IsPaused = false;
			}
		}

		double _speed = 1.0;

		public double Speed {
			get { return _speed; }
			set {
				_speed = value;
				RaisePropertyChanged(nameof(Speed));
			}
		}

		public int MoveCounter {
			get { return _tower.MoveCounter; }
		}

		IEnumerable<HanoiMove> _moves;
		HanoiTower _tower;
		ICommand _solveCommand;
		ICommand _resetCommand;
		ICommand _pauseCommand;

		internal IEnumerable<HanoiMove> Moves {
			get { return _moves; }
			set {
				if(_moves != value) {
					_moves = value;
					RaisePropertyChanged("Moves");
				}
			}
		}

		public HanoiViewModel(int discs) {
			_tower = new HanoiTower(discs);
			Discs = discs;
		}

		public ICommand SolveCommand {
			get {
				return _solveCommand ?? (_solveCommand =
					new RelayCommand(() => {
						Moves = _tower.Solve();
						IsRunning = true;
						IsSolved = false;
						IsPaused = false;
					}, () => !IsRunning));
			}
		}

		public ICommand ResetCommand {
			get {
				return _resetCommand ?? (_resetCommand =
					new RelayCommand(() => {
						_tower = new HanoiTower(Discs);
						IsRunning = IsPaused = false;
						IsSolved = false;
						Moves = null;
						RaisePropertyChanged("MoveCounter");
					}, () => IsRunning || IsSolved));
			}
		}

		public ICommand PauseCommand {
			get {
				return _pauseCommand ?? (_pauseCommand =
					new RelayCommand(() => {
						IsPaused = !IsPaused;
					}, () => IsRunning && !IsSolved));
			}
		}

		bool _isRunning, _isPaused, _isSolved;

		public bool IsSolved {
			get { return _isSolved; }
			set {
				if(_isSolved != value) {
					_isSolved = value;
					RaisePropertyChanged("IsSolved");
				}
			}
		}

		public bool IsPaused {
			get { return _isPaused; }
			set {
				_isPaused = value;
				RaisePropertyChanged("IsPaused");
			}
		}

		public bool IsRunning {
			get { return _isRunning; }
			set {
				if(_isRunning != value) {
					_isRunning = value;
					RaisePropertyChanged("IsRunning");
				}
			}
		}

		public void MakeMove() {
			RaisePropertyChanged("MoveCounter");
		}

		internal void MovesDone() {
			IsRunning = false;
			IsSolved = true;
		}
	}
}
