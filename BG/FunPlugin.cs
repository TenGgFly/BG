using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MakePower.DataPlat.Essential.BaseInterface;
using System.Windows.Forms;

namespace BG
{
    public class FunPlugin:IFFunctionPlugin,IFPluginMenu,IFPluginMonitor
    {
        public string GetSelfInfo()
        {
            string iInfo = "Name=看门狗插件$";
            iInfo += "NodeName=设备数据管理$";
            iInfo += "Version=20180807.090600.001$";
            return iInfo;
        }
        private database _dataStorage = null;

        public bool Init(out string AErrorMsg)
        {
            AErrorMsg = "";
            if (_dataStorage == null)
            {
                _dataStorage = new database();
            }
            return true;
        }

        public bool Cleanup()
        {
            if (_dataStorage != null)
            {
                _dataStorage.Terminate();
                _dataStorage = null;
            }
            return true;
        }

        public List<ToolStripMenuItem> GetMenuItems()
        {
            List<ToolStripMenuItem> iItem = new List<ToolStripMenuItem>();
            return iItem;
        }

        private Form1 _monitorForm = null;

        public Control OpenMonitor()
        {
            if (_monitorForm == null)
            {
                _monitorForm = new Form1();
            }
            return _monitorForm;
        }

        public bool CloseMonitor()
        {
            if (_monitorForm != null)
            {
                _monitorForm.CloseControlForm();
                _monitorForm = null;
            }
            return false;
        }

        public Form CreateForm()
        {
            return null;
        }

        public bool CloseForm()
        {
            return true;
        }
    }
}
