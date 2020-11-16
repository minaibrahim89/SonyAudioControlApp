using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SonyAudioControl.ViewModels;

namespace SonyAudioControl.TemplateSelectors
{
    public class SoundSettingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EnumTemplate { get; set; }
        public DataTemplate IntegerTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (!(item is SoundSettingViewModel setting))
                return null;

            return setting.TargetType switch
            {
                "enumTarget" => EnumTemplate,
                "booleanTarget" => EnumTemplate,
                "integerTarget" => IntegerTemplate,
                _ => DefaultTemplate
            };
        }
    }
}
