using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Waveface.ClientFramework;
using Waveface.Model;

namespace Waveface.Client
{
	public partial class FavoriteListBox : ListBox
	{
		public event EventHandler DeleteFavoriteInvoked;

		private Point m_startPoint;

		public FavoriteListBox()
		{
			InitializeComponent();
		}

		protected void OnDeleteFavoriteInvoked(EventArgs e)
		{
			if (DeleteFavoriteInvoked == null)
				return;

			DeleteFavoriteInvoked(this, e);
		}

		private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
		}

		private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			OnDeleteFavoriteInvoked(EventArgs.Empty);
		}

		private void UserControl_MouseMove(object sender, MouseEventArgs e)
		{
			Point _mousePos = e.GetPosition(null);
			Vector _diff = m_startPoint - _mousePos;

			if (e.LeftButton == MouseButtonState.Pressed &&
				(Math.Abs(_diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
				 Math.Abs(_diff.Y) > SystemParameters.MinimumVerticalDragDistance))
			{
				if (SelectedItem != null)
				{
					BunnyLabelContentGroup _contentGroup = SelectedItem as BunnyLabelContentGroup;

					if (_contentGroup != null)
					{
						try
						{
							string _tempPathBase = Path.GetTempPath() + "Waveface Photos" + "\\";
							string _path = _tempPathBase + "\\" + Regex.Replace(_contentGroup.Name, @"[?:\/*""<>|]", "") + "\\";

							DirectoryInfo _dir = Directory.CreateDirectory(_path);

							List<string> _files = new List<string>();

							foreach (IContentEntity _entity in _contentGroup.Contents)
							{
								_files.Add(_entity.Uri.LocalPath);
							}

							foreach (string _s in _files)
							{
								File.Copy(_s, Path.Combine(_path, Path.GetFileName(_s)), true);
							}

							DataObject _dragData = new DataObject();
							_dragData.SetData(DataFormats.FileDrop, new[] { _path });
							DragDrop.DoDragDrop(this, _dragData, DragDropEffects.Copy);

							_dir.Delete(true);
							Directory.Delete(_tempPathBase);
						}
						catch
						{
						}
					}
				}
			}
		}

		private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			m_startPoint = e.GetPosition(null);
		}

		private void UserControl_DragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(IEnumerable<IContentEntity>)))
			{
				e.Effects = DragDropEffects.Copy;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
		}
	}
}