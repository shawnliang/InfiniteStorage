#region

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading;
using Waveface.Model;

#endregion

namespace Waveface.ClientFramework
{
	public class BunnyService : Service
	{
		private bool _isRecving;
		private bool _syncEnabled;

		private ReceivingStatus _recvStatus = new ReceivingStatus();

		public bool IsRecving
		{
			get { return _isRecving; }

			set
			{
				if (_isRecving != value)
				{
					_isRecving = value;
					OnPropertyChanged("IsRecving");
				}
			}
		}

		public ReceivingStatus RecvStatus
		{
			get { return _recvStatus; }
			set
			{
				if (_recvStatus != value)
				{
					_recvStatus = value;
					OnPropertyChanged("RecvStatus");
				}
			}
		}

		public bool SyncEnabled
		{
			get { return _syncEnabled; }
			set
			{
				if (_syncEnabled != value)
				{
					_syncEnabled = value;
					StationAPI.SetDeviceSyncOption(ID, _syncEnabled);
					OnPropertyChanged("SyncEnabled");
				}
			}
		}

		public BunnyService(IServiceSupplier supplier, string devFolderName, string deviceId, bool syncEnabled)
			: base(deviceId, supplier, devFolderName)
		{
			Uri = new Uri(Path.Combine(BunnyDB.ResourceFolder, devFolderName));
			_syncEnabled = syncEnabled;
			SetContents(PopulateContent);
		}

		private void PopulateContent(ObservableCollection<IContentEntity> content)
		{
			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select [name] from [Folders] " +
					                  "where parent_folder = @parent " +
					                  "order by name desc";

					cmd.Parameters.Add(new SQLiteParameter("@parent", Name));

					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var dir = reader.GetString(0);
							content.Add(new BunnyContentGroup(Name, dir, ID) { Service = this });
						}
					}
				}
			}
		}

		public override void Refresh()
		{
			// Truely refresh underlying contents because there is no good way to determine if a 
			// content group really "eqauls" to another content group.
			//
			// (Comparing name and id is insufficient because the contained items can be different.
			// Unless comparing the contained contents, but which indicates recursively enumerating contained contents,
			// no way to truly comparing content groups.)
			m_ObservableContents.Clear();
			PopulateContent(m_ObservableContents);
		}

		#region Public Method

		public override bool Equals(object obj)
		{
			//檢查參數是否為null
			if (obj == null)
				return false;

			//檢查是否與自身是相同物件
			if (ReferenceEquals(this, obj))
				return true;

			//檢查是否型態相等
			var value = obj as IService;

			if (value == null)
				return false;

			//比較內容是否相等
			return Name == value.Name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		#endregion
	}

	public class ReceivingStatus : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private int _total;
		private int _received;
		private bool _is_preparing;
		private bool _is_receiving;

		public int Total
		{
			get { return _total; }
			set 
			{
				if (_total != value)
				{
					_total = value;
					notifyPropertyChanged("Total");
				}
			}
		}

		public int Received
		{
			get { return _received; }
			set
			{
				if (_received != value)
				{
					_received = value;
					notifyPropertyChanged("Received");
				}
			}
		}

		public bool IsPreparing
		{
			get { return _is_preparing; }
			set
			{
				if (_is_preparing != value)
				{
					_is_preparing = value;
					notifyPropertyChanged("IsPreparing");
				}
			}
		}

		public bool IsReceiving
		{
			get { return _is_receiving; }
			set
			{
				if (_is_receiving != value)
				{
					_is_receiving = value;
					notifyPropertyChanged("IsReceiving");
				}
			}
		}

		private void notifyPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}