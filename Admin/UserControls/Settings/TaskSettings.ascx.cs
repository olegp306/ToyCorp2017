using System;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Core.Scheduler;
using Resources;

namespace Admin.UserControls.Settings
{
    public partial class TaskSettings : System.Web.UI.UserControl
    {
        public string ErrMessage = Resources.Resource.Admin_CommonSettings_InvalidTask;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {        
        }

        private void LoadData()
        {
            var settings = AdvantShop.Core.Scheduler.TaskSettings.Settings;
            ddlTypeHtml.Items.Clear();
            ddlTypeHtml.Items.Add(new ListItem { Text = Resource.Admin_CommonSettings_NotSelected, Value = TimeIntervalType.None.ToString() });
            ddlTypeHtml.Items.Add(new ListItem { Text = Resource.Admin_CommonSettings_InDays, Value = TimeIntervalType.Days.ToString() });
            ddlTypeHtml.Items.Add(new ListItem { Text = Resource.Admin_CommonSettings_InHours, Value = TimeIntervalType.Hours.ToString(), Selected = true });

            ddlTypeXml.Items.Clear();
            ddlTypeXml.Items.Add(new ListItem { Text = Resource.Admin_CommonSettings_NotSelected, Value = TimeIntervalType.None.ToString() });
            ddlTypeXml.Items.Add(new ListItem { Text = Resource.Admin_CommonSettings_InDays, Value = TimeIntervalType.Days.ToString() });
            ddlTypeXml.Items.Add(new ListItem { Text = Resource.Admin_CommonSettings_InHours, Value = TimeIntervalType.Hours.ToString(), Selected = true });

            ddlTypeYandex.Items.Clear();
            ddlTypeYandex.Items.Add(new ListItem { Text = Resource.Admin_CommonSettings_NotSelected, Value = TimeIntervalType.None.ToString() });
            ddlTypeYandex.Items.Add(new ListItem { Text = Resource.Admin_CommonSettings_InDays, Value = TimeIntervalType.Days.ToString() });
            ddlTypeYandex.Items.Add(new ListItem { Text = Resource.Admin_CommonSettings_InHours, Value = TimeIntervalType.Hours.ToString(), Selected = true });

            ddlTypeGoogleBase.Items.Clear();
            ddlTypeGoogleBase.Items.Add(new ListItem { Text = Resource.Admin_CommonSettings_NotSelected, Value = TimeIntervalType.None.ToString() });
            ddlTypeGoogleBase.Items.Add(new ListItem { Text = Resource.Admin_CommonSettings_InDays, Value = TimeIntervalType.Days.ToString() });
            ddlTypeGoogleBase.Items.Add(new ListItem { Text = Resource.Admin_CommonSettings_InHours, Value = TimeIntervalType.Hours.ToString(), Selected = true });

            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i].JobType == typeof(GenerateHtmlMapJob).ToString())
                {
                    chbEnabledHtml.Checked = settings[i].Enabled;
                    txtTimeIntervalHtml.Text = settings[i].TimeInterval.ToString();
                    ddlTypeHtml.SelectedValue = settings[i].TimeType.ToString();
                    txtHoursHtml.Text = settings[i].TimeHours.ToString();
                    txtMinutesHtml.Text = settings[i].TimeMinutes.ToString();
                }

                if (settings[i].JobType == typeof(GenerateXmlMapJob).ToString())
                {
                    chbEnabledXml.Checked = settings[i].Enabled;
                    txtTimeIntervalXml.Text = settings[i].TimeInterval.ToString();
                    ddlTypeXml.SelectedValue = settings[i].TimeType.ToString();
                    txtHoursXml.Text = settings[i].TimeHours.ToString();
                    txtMinutesXml.Text = settings[i].TimeMinutes.ToString();
                }

                if (settings[i].JobType == typeof(GenerateYandexMarketJob).ToString())
                {
                    chbEnabledYandex.Checked = settings[i].Enabled;
                    txtTimeIntervalYandex.Text = settings[i].TimeInterval.ToString();
                    ddlTypeYandex.SelectedValue = settings[i].TimeType.ToString();
                    txtHoursYandex.Text = settings[i].TimeHours.ToString();
                    txtMinutesYandex.Text = settings[i].TimeMinutes.ToString();
                }
                if (settings[i].JobType == typeof(GenerateGoogleBaseJob).ToString())
                {
                    chbEnabledGoogleBase.Checked = settings[i].Enabled;
                    txtTimeIntervalGoogleBase.Text = settings[i].TimeInterval.ToString();
                    ddlTypeGoogleBase.SelectedValue = settings[i].TimeType.ToString();
                    txtHoursGoogleBase.Text = settings[i].TimeHours.ToString();
                    txtMinutesGoogleBase.Text = settings[i].TimeMinutes.ToString();
                }
            }
        }

        public bool SaveData()
        {
            if (!ValidateData())
                return false;

            var settings = new AdvantShop.Core.Scheduler.TaskSettings();
            var item = new TaskSetting();
            //html
            item.Enabled = chbEnabledHtml.Checked;
            item.JobType = typeof(GenerateHtmlMapJob).ToString();
            item.TimeInterval = txtTimeIntervalHtml.Text.TryParseInt();
            item.TimeType = (TimeIntervalType)Enum.Parse(typeof(TimeIntervalType), ddlTypeHtml.SelectedValue, true);
            if (item.TimeType == TimeIntervalType.Days)
            {
                item.TimeHours = txtHoursHtml.Text.TryParseInt();
                item.TimeMinutes = txtMinutesHtml.Text.TryParseInt();
            }
            settings.Add(item);
            //xml
            item = new TaskSetting();
            item.Enabled = chbEnabledXml.Checked;
            item.JobType = typeof(GenerateXmlMapJob).ToString();
            item.TimeInterval = txtTimeIntervalXml.Text.TryParseInt();
            item.TimeType = (TimeIntervalType)Enum.Parse(typeof(TimeIntervalType), ddlTypeXml.SelectedValue, true);
            if (item.TimeType == TimeIntervalType.Days)
            {
                item.TimeHours = txtHoursXml.Text.TryParseInt();
                item.TimeMinutes = txtMinutesXml.Text.TryParseInt();
            }
            settings.Add(item);
            //yandex
            item = new TaskSetting();
            item.Enabled = chbEnabledYandex.Checked;
            item.JobType = typeof(GenerateYandexMarketJob).ToString();
            item.TimeInterval = txtTimeIntervalYandex.Text.TryParseInt();
            item.TimeType = (TimeIntervalType)Enum.Parse(typeof(TimeIntervalType), ddlTypeYandex.SelectedValue, true);
            if (item.TimeType == TimeIntervalType.Days)
            {
                item.TimeHours = txtHoursYandex.Text.TryParseInt();
                item.TimeMinutes = txtMinutesYandex.Text.TryParseInt();
            }
            settings.Add(item);

            //googlebase
            item = new TaskSetting();
            item.Enabled = chbEnabledGoogleBase.Checked;
            item.JobType = typeof(GenerateGoogleBaseJob).ToString();
            item.TimeInterval = txtTimeIntervalGoogleBase.Text.TryParseInt();
            item.TimeType = (TimeIntervalType)Enum.Parse(typeof(TimeIntervalType), ddlTypeGoogleBase.SelectedValue, true);
            if (item.TimeType == TimeIntervalType.Days)
            {
                item.TimeHours = txtHoursGoogleBase.Text.TryParseInt();
                item.TimeMinutes = txtMinutesGoogleBase.Text.TryParseInt();
            }
            settings.Add(item);

            AdvantShop.Core.Scheduler.TaskSettings.Settings = settings;
            TaskManager.TaskManagerInstance().ManagedTask(settings);
            LoadData();
            return true;
        }

        private bool ValidateData()
        {
            if (!(txtTimeIntervalHtml.Text.IsInt() && txtHoursHtml.Text.IsInt() && txtMinutesHtml.Text.IsInt() &&
                  txtTimeIntervalXml.Text.IsInt() && txtHoursXml.Text.IsInt() && txtMinutesXml.Text.IsInt() &&
                  txtTimeIntervalYandex.Text.IsInt() && txtHoursYandex.Text.IsInt() && txtMinutesYandex.Text.IsInt() &&
                  txtTimeIntervalGoogleBase.Text.IsInt() && txtHoursGoogleBase.Text.IsInt() && txtMinutesGoogleBase.Text.IsInt()
                  ))
            {
                ErrMessage = Resource.Admin_CommonSettings_TasksWrongNumberFormat;
                return false;
            }

            if (!(RangeCheck(txtTimeIntervalHtml.Text.TryParseInt(), 0, 23)
                  && RangeCheck(txtHoursHtml.Text.TryParseInt(), 0, 23)
                  && RangeCheck(txtMinutesHtml.Text.TryParseInt(), 0, 59)

                  && RangeCheck(txtTimeIntervalXml.Text.TryParseInt(), 0, 23)
                  && RangeCheck(txtHoursXml.Text.TryParseInt(), 0, 23)
                  && RangeCheck(txtMinutesXml.Text.TryParseInt(), 0, 59)

                  && RangeCheck(txtTimeIntervalYandex.Text.TryParseInt(), 0, 23)
                  && RangeCheck(txtHoursYandex.Text.TryParseInt(), 0, 23)
                  && RangeCheck(txtMinutesYandex.Text.TryParseInt(), 0, 59)

                  && RangeCheck(txtTimeIntervalGoogleBase.Text.TryParseInt(), 0, 23)
                  && RangeCheck(txtHoursGoogleBase.Text.TryParseInt(), 0, 23)
                  && RangeCheck(txtMinutesGoogleBase.Text.TryParseInt(), 0, 59)

                 ))
            {
                ErrMessage = Resource.Admin_CommonSettings_TasksWrongNumberFormat;
                return false;
            }

            return true;
        }

        private bool RangeCheck(int value, int left, int right)
        {
            return left <= value && value <= right;
        }
    }
}