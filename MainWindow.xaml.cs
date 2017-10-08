using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System;
using System.Reflection;
using System.Windows.Documents;
using System.Windows.Input;

namespace TowersOfHanoi {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		struct PoleInfo {
			Stack<FrameworkElement> _discs;

			public void Add(FrameworkElement element) {
				if(_discs == null)
					_discs = new Stack<FrameworkElement>();
				_discs.Push(element);
			}

			public FrameworkElement Top {
				get { return _discs.Peek(); }
			}

			internal void Remove() {
				if(_discs != null)
					_discs.Pop();
			}

			public int Count {
				get { return _discs == null ? 0 : _discs.Count; }
			}
		}

		HanoiViewModel _vm;
		Storyboard _animation;
		FrameworkElement _movingDisc;

		const double DiscHeight = 60;

		public MainWindow() {
			InitializeComponent();

			DataContext = _vm = new HanoiViewModel(3);
			_animation = (Storyboard)FindResource("moveStoryboard");
			_vm.PropertyChanged += _vm_PropertyChanged;
			Init();
		}

		IEnumerator<HanoiMove> _currentMoveEnum;
		PoleInfo[] _poles;

		void _vm_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			switch(e.PropertyName) {
				case "Discs":
					Init();
					break;

				case "Moves":
					Init();
					if(_vm.Moves != null) {
						_currentMoveEnum = _vm.Moves.GetEnumerator();
						if(_currentMoveEnum.MoveNext())
							MakeMove(_currentMoveEnum.Current);
					}
					break;

				case "IsPaused":
					if(_movingDisc != null) {
						if(_vm.IsPaused)
							_animation.Pause(_movingDisc);
						else
							_animation.Resume(_movingDisc);
					}
					break;

			}
		}


		private void MakeMove(HanoiMove move) {
			int from = move.From - 1, to = move.To - 1;
			var disc = _movingDisc = _poles[from].Top;
			var start = GetPositionInPole(disc, from);
			var end = GetPositionInPole(disc, to);
			var g = FindResource("animGeometry") as PathGeometry;
			g.Figures[0].StartPoint = start;
			// update geometry
			var poly = g.Figures[0].Segments[0] as PolyLineSegment;
			poly.Points[0] = new Point(start.X, 1100);
			poly.Points[1] = new Point(end.X, 1100);
			poly.Points[2] = end;

			EventHandler completed = null;
			completed = (s, e) => {
				_animation.Completed -= completed;
				if(_vm.Moves != null) {
					_poles[to].Add(disc);
					_poles[from].Remove();
					Canvas.SetLeft(disc, Canvas.GetLeft(disc));
					Canvas.SetBottom(disc, Canvas.GetBottom(disc));
					// next move
					if(_currentMoveEnum != null)
						if(_currentMoveEnum.MoveNext())
							MakeMove(_currentMoveEnum.Current);
						else {
							_vm.MovesDone();
							CommandManager.InvalidateRequerySuggested();
						}

				}
			};
			_animation.Completed += completed;
			_vm.MakeMove();
			// start a controllable animation
			_animation.Begin(disc, true);
		}

		private Point GetPositionInPole(FrameworkElement disc, int pole) {
			return new Point(200 + 400 * pole - disc.Width / 2, DiscHeight * _poles[pole].Count);
		}

		private void Init() {
			_currentMoveEnum = null;
			if(_movingDisc != null)
				_animation.Remove(_movingDisc);
			_movingDisc = null;
			_canvas.Children.RemoveRange(3, _canvas.Children.Count - 3);
			double width = 250, left = 200 - width / 2, bottom = 0;
			_poles = new PoleInfo[3];
			for(int i = 0; i < _vm.Discs; i++) {
				var disc = new DiscControl {
					Text = (_vm.Discs - i).ToString(),
					FontSize = 30,
					Width = width,
					Height = DiscHeight,
					Foreground = Brushes.White
				};
				_canvas.Children.Add(disc);
				Canvas.SetLeft(disc, left);
				Canvas.SetBottom(disc, bottom);
				width -= 10;
				bottom += DiscHeight;
				left += 5;
				_poles[0].Add(disc);
			}
		}

	}
}
