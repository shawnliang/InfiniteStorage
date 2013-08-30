#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Waveface.Model;

#endregion

namespace Waveface.Client
{
	public partial class P_ItemUC : UserControl
	{
		public string YMD { get; set; }
		public int VideosCount { get; set; }
		public int PhotosCount { get; set; }
		public bool Changed { get; set; }
		public MainWindow MyMainWindow { get; set; }
		public IService CurrentDevice { get; set; }

		private int m_currentIndex;

		public List<FileEntry> FileEntrys
		{
			get { return m_fileEntrys; }
			set
			{
				m_oldFileEntrysCount = m_fileEntrys.Count;

				m_fileEntrys = value;

				Changed = m_oldFileEntrysCount != m_fileEntrys.Count;
			}
		}

		private int m_oldFileEntrysCount;
		private List<FileEntry> m_fileEntrys = new List<FileEntry>();
		private int m_delayMouseMoveCount;
		private int DELAY = 22;

		public P_ItemUC()
		{
			InitializeComponent();
		}

		public void SetUI()
		{
			GetCounts();

			SetInfor();

			FileEntrys.Reverse();

			SetImage(m_currentIndex);
		}

		private void SetImage(int index)
		{
			tbTitle.Text = YMD;
			tbCount.Text = (index + 1) + "/" + FileEntrys.Count;

			FileEntry _fileEntry = FileEntrys[index];

			rectMask.Visibility = Visibility.Collapsed;

			switch (_fileEntry.type)
			{
				case 0:
					{
						string _sPath = _fileEntry.tiny_path.Replace(".tiny.", ".medium.");

						if (!File.Exists(_sPath))
						{
							_sPath = _fileEntry.tiny_path.Replace(".tiny.", ".small.");

							if (!File.Exists(_sPath))
							{
								_sPath = _fileEntry.tiny_path;

								if (!File.Exists(_sPath))
								{
									_sPath = _fileEntry.s92_path;
								}
							}

							rectMask.Visibility = Visibility.Visible;
						}

						BitmapImage _bi = new BitmapImage();
						_bi.BeginInit();
						_bi.UriSource = new Uri(_sPath, UriKind.Absolute);
						_bi.EndInit();

						image.Source = _bi;
					}

					break;
				case 1:
					{
						BitmapImage _vidoeThumb = new BitmapImage();
						_vidoeThumb.BeginInit();

						if (File.Exists(_fileEntry.s92_path))
						{
							_vidoeThumb.UriSource = new Uri(_fileEntry.s92_path, UriKind.Absolute);

							rectMask.Visibility = Visibility.Visible;
						}
						else if (File.Exists(_fileEntry.tiny_path))
						{
							_vidoeThumb.UriSource = new Uri(_fileEntry.tiny_path, UriKind.Absolute);
						}
						else
						{
							_vidoeThumb.UriSource = new Uri("pack://application:,,,/Ren/Images/video_130x110.png");
						}

						_vidoeThumb.EndInit();

						image.Source = _vidoeThumb;
					}

					break;
			}
		}

		private void SetInfor()
		{
		}

		public void GetCounts()
		{
			VideosCount = 0;
			PhotosCount = 0;

			foreach (FileEntry _file in FileEntrys)
			{
				if (_file.type == 0)
				{
					PhotosCount++;
				}
				else
				{
					VideosCount++;
				}
			}
		}

		private void image_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (((++m_delayMouseMoveCount) % DELAY) == 0)
			{
				SetImage((++m_currentIndex) % FileEntrys.Count);
			}
		}

		private void image_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			//m_delayMouseMoveCount = 0;
		}
	}
}