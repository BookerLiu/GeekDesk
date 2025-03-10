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
                new GradientBGParam("1E7BFD15-92CE-1332-F583-94E1C39729FF", "魅惑妖术", "#EE9CA7", "#FFDDE1"),
                new GradientBGParam ("F54F9B03-8F50-8FFB-54C6-1BCCA03BF0F8", "森林之友", "#EBF7E3", "#A8E4C0"),
                new GradientBGParam("36C16080-0516-0DAC-FE09-721CC6AB57A4", "完美谢幕", "#D76D77", "#FFAF7B")
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
