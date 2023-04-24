using System.Collections.ObjectModel;

namespace GeekDesk.ViewModel.Temp
{
    public class GradientBGParamList
    {


        private static ObservableCollection<GradientBGParam> gradientBGParams;

        static GradientBGParamList()
        {
            //gradientBGParams = (ObservableCollection<GradientBGParam>)ConfigurationManager.GetSection("SystemBGs")
            gradientBGParams = new ObservableCollection<GradientBGParam>
            {
                new GradientBGParam("魅惑妖术", "#EE9CA7", "#FFDDE1"),
                new GradientBGParam ("森林之友", "#EBF7E3", "#A8E4C0"),
                new GradientBGParam("完美谢幕", "#D76D77", "#FFAF7B")
            };
        }

        public static ObservableCollection<GradientBGParam> GradientBGParams
        {
            get
            {
                return gradientBGParams;
            }
            set
            {
                gradientBGParams = value;
            }
        }

    }
}
