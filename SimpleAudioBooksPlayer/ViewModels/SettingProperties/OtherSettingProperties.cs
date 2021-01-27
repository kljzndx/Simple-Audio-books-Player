using System;
using HappyStudio.UwpToolsLibrary.Auxiliarys;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Attributes;

namespace SimpleAudioBooksPlayer.ViewModels.SettingProperties
{
    public class OtherSettingProperties : SettingsBase
    {
        public static readonly OtherSettingProperties Current = new OtherSettingProperties();

        [SettingFieldByNormal(nameof(UpdateLogVersion), "")] private string _updateLogVersion;

        [SettingFieldByNormal(nameof(IsPaid), false)] private bool _isPaid;
        [SettingFieldByNormal(nameof(PaymentChecked), false)] private bool _paymentChecked;
        [SettingFieldByNormal(nameof(IsShowWelcomeTip), true)] private bool _isShowWelcomeTip;

        private OtherSettingProperties() : base("Other")
        {
        }

        public DateTime ExitTime { get; set; }

        public string UpdateLogVersion
        {
            get => _updateLogVersion;
            set => SetSetting(ref _updateLogVersion, value);
        }

        public bool IsPaid
        {
            get => _isPaid;
            set => SetSetting(ref _isPaid, value);
        }

        public bool PaymentChecked
        {
            get => _paymentChecked;
            set => SetSetting(ref _paymentChecked, value);
        }

        public bool IsShowWelcomeTip
        {
            get => _isShowWelcomeTip;
            set => SetSetting(ref _isShowWelcomeTip, value);
        }
    }
}