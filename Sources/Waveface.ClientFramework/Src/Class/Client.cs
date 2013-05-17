using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.Model;

namespace Waveface.ClientFramework
{
	public class Client
	{
		#region Static Var
        private static Client _default;
        #endregion


		#region Var
		private string _labelID;
		#endregion


		#region Public Static Property
		public static Client Default
        { 
            get
            {
                return _default ?? (_default = new Client());
            }
        }
        #endregion


		#region Private Property
		private string m_LabelID 
		{
			get
			{
				return _labelID ?? (_labelID = GetDefaultLabelID());
			}
		}
		#endregion


		#region Private Method
		private string GetDefaultLabelID()
		{
			var json = StationAPI.GetAllLables();
			var labelID = JObject.Parse(json)["labels"][0]["label_id"].ToString();
			return labelID;
		}
		#endregion


		#region Public Method
		public void Tag(IContentEntity content)
		{
			StationAPI.Tag(content.ID, m_LabelID);
		}

		public void UnTag(IContentEntity content)
		{
			StationAPI.UnTag(content.ID, m_LabelID);
		}
		#endregion
	}
}
