using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Data;
using DotSpatial.SDR.Plugins.ALI.Properties;
using SDR.Common;
using SDR.Data.Files;
using SDR.Common.UserMessage;
using ILog = SDR.Common.logging.ILog;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.ALI
{
    public sealed partial class AliPanel : UserControl
    {

        #region Constructors
        public AliPanel()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the datagridview for record display
        /// </summary>
        public DataGridView DataGridDisplay
        {
            get { return aliDGV; }
        }

        #endregion

        #region Methods 

        public void ShowGlobalCadInterface()
        {
            aliTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[0].Width = 100;
            aliTableLayoutPanel.ColumnStyles[1].SizeType = SizeType.AutoSize;
            // populate the combobox with files now
            PopulateComboBox();
        }

        public void ShowStandardInterface()
        {
            aliTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[0].Width = 100;
            aliTableLayoutPanel.ColumnStyles[1].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.ColumnStyles[1].Width = 0;
        }

        private void PopulateComboBox()
        {
            cmbAliCommLog.Items.Clear();
            var files = new List<string>();
            var dir = new DirectoryInfo(SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadArchivePath);
            try
            {
                files.AddRange(dir.GetFiles("*.log").Select(fi => Path.GetFileNameWithoutExtension(fi.Name)));
                // add the current (todays) log to the list
                var curLog = Path.GetFileNameWithoutExtension(
                    SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath) + 
                    DateTime.Now.ToShortDateString().Replace("/", "");

                files.Add(curLog);
                files.Sort();
                files.Reverse();
                cmbAliCommLog.Items.AddRange(files.ToArray());
                cmbAliCommLog.SelectedIndexChanged += CmbAliCommLogOnSelectedIndexChanged;
                // set the selection initiating the datagridview columns
                cmbAliCommLog.SelectedIndex = cmbAliCommLog.FindStringExact(curLog);
            }
            catch (Exception ex)
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Warn("Failed to Locate Archived GlobalCAD Files", ex);
            }
        }

        private void CmbAliCommLogOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            try
            {
                var cmb = (ComboBox)sender;
                // check if this log it todays log
                var itm = cmb.SelectedItem.ToString();
                string log;
                if (itm.EndsWith(DateTime.Now.ToShortDateString().Replace("/", "")))
                {
                    log = SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath;
                }
                else
                {
                    log = SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadArchivePath + "\\" + itm + ".log";
                }
                // populate the collection to the datagridview
                aliDGV.DataSource = GlobalCadLogToCollection(log).ToArray();
            }
            catch (Exception ex)
            {
                var log = AppContext.Instance.Get<ILog>();
                log.Warn("Failed to Populate GlobalCAD File Records to DataView", ex);
            }
        }

        private List<GlocalCadRecord> GlobalCadLogToCollection(string logPath)
        {
            var bindingList = new List<GlocalCadRecord>();
            var newRec = false;
            var rec = new List<string>();

            using (var f = File.OpenText(logPath))
            {
                while (!f.EndOfStream)
                {
                    var l = f.ReadLine();
                    // do basic start end cleanup
                    if (l != null && l.Contains("<Start Data>"))
                    {
                        l = "<Start Data>";
                    }
                    if (l != null && l.Contains("<End Data>"))
                    {
                        l = "<End Data>";
                    }
                    // determine line type and add to our list for processing
                    switch (l)
                    {
                        case "<Start Data>":
                            rec.Clear();
                            newRec = true;
                            rec.Add(l);
                            break;
                        case "<End Data>":
                            rec.Add(l);
                            var obj = ProcessGlocalCadRecord(rec);
                            if (obj != null)
                            {
                                bindingList.Add(obj);
                            }
                            rec.Clear();
                            newRec = false;
                            break;
                        default:
                            if (newRec)
                            {
                                rec.Add(l);
                            }
                            break;
                    }
                }
            }
            return bindingList;
        }

        private GlocalCadRecord ProcessGlocalCadRecord(List<string> recList)
        {
            // fetch the settings for parsing the values from the list
            var pFile = SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadConfigPath;
            var timeLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "TimeLine", pFile)) - 1;
            var timeStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "TimeStart", pFile)) - 1;
            var timeLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "TimeLength", pFile));
            var phoneLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "PhoneLine", pFile)) - 1;
            var phoneStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "PhoneStart", pFile)) - 1;
            var phoneLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "PhoneLength", pFile));
            var houseNumLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "HouseNumberLine", pFile)) - 1;
            var houseNumStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "HouseNumberStart", pFile)) - 1;
            var houseNumLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "HouseNumberLength", pFile));
            var alphaLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "AlphaLine", pFile)) - 1;
            var alphaStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "AlphaStart", pFile)) - 1;
            var alphaLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "AlphaLength", pFile));
            var strPreLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StreetPrefixLine", pFile)) - 1;
            var strPreStartDelta = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StreetPrefixStartDelta", pFile));
            var strPreLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StreetPrefixLength", pFile));
            var strLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StreetLine", pFile)) - 1;
            var strStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StreetStart", pFile)) - 1;
            var strLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StreetLength", pFile));
            var servClsLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "ClassOfServiceLine", pFile)) - 1;
            var servClsStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "ClassOfServiceStart", pFile)) - 1;
            var servClsLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "ClassOfServiceLength", pFile));
            var xLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "XLine", pFile)) - 1;
            var xStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "XStart", pFile)) - 1;
            var xLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "XLength", pFile));
            var yLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "YLine", pFile)) - 1;
            var yStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "YStart", pFile)) - 1;
            var yLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "YLength", pFile));
            var uncLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "UNCLine", pFile)) - 1;
            var uncStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "UNCStart", pFile)) - 1;
            var uncLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "UNCLength", pFile));
            var cityLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "CityLine", pFile)) - 1;
            var cityStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "CityStart", pFile)) - 1;
            var cityLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "CityLength", pFile));
            var stateLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StateLine", pFile)) - 1;
            var stateStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StateStart", pFile)) - 1;
            var stateLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StateLength", pFile));
            
            var gcr = new GlocalCadRecord();
            try
            {
                gcr.Time = recList[timeLine].Substring(timeStart, timeLen).Trim();
                gcr.Phone = recList[phoneLine].Substring(phoneStart, phoneLen).Trim();
                // assemble the address
                gcr.Address = (recList[houseNumLine].Substring(houseNumStart, houseNumLen).Trim() + " " +
                                recList[alphaLine].Substring(alphaStart, alphaLen).Trim()).Trim();
                // check for a street prefix
                var tPrefix = string.Empty;
                if (recList[houseNumLine].Length > houseNumLen)
                {
                    tPrefix = recList[strPreLine].Substring(
                        recList[strPreLine].Length - strPreStartDelta, strPreLen).Trim();
                }
                gcr.Street = (tPrefix + " " + recList[strLine].Substring(strStart, strLen).Trim()).Trim();
                gcr.ServiceClass = recList[servClsLine].Substring(servClsStart, servClsLen).Trim();
                if (gcr.ServiceClass.ToUpper() == "MOBL" ||
                    gcr.ServiceClass.ToUpper() == "WRLS" ||
                    gcr.ServiceClass.ToUpper() == "WPH1" ||
                    gcr.ServiceClass.ToUpper() == "WPH2")
                {
                    gcr.X = recList[xLine].Substring(xStart, xLen).Trim();
                    gcr.Y = recList[yLine].Substring(yStart, yLen).Trim();
                    gcr.Unc = recList[uncLine].Substring(uncStart, uncLen).Trim();
                }
                else
                {
                    gcr.X = string.Empty;
                    gcr.Y = string.Empty;
                    gcr.Unc = string.Empty;
                }
                gcr.State = recList[stateLine].Substring(stateStart, stateLen).Trim();
                gcr.City = recList[cityLine].Substring(cityStart, cityLen).Trim();
            }
            catch (Exception ex)
            {
                var log = AppContext.Instance.Get<ILog>();
                log.Warn("Failed to Parse GlobalCAD Record", ex);
                return null;
            }
            return gcr;
        }

        #endregion

        #region Events

        #endregion

        #region FormEvents

        // TODO: handle all form events

        #endregion

        #region Event Handlers

        // TODO: add all event handlers

        #endregion
    }
}

public class GlocalCadRecord
{
    public string Time { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    [DisplayName(@"Service Class")] 
    public string ServiceClass { get; set; }
    public string X { get; set; }
    public string Y { get; set; }
    public string Unc { get; set; }
    [DisplayName(@"Full Address")] 
    public string FullAddress
    {
        get { return this.Address + " " + this.Street; }
    }
}

