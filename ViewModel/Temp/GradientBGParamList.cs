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
                new GradientBGParam("诸神黄昏", "#FCCF31", "#F55555"),
                new GradientBGParam ("森林之友", "#EBF7E3", "#A8E4C0"),
                new GradientBGParam("魅惑妖术", "#FFDDE1", "#EE9CA7"),
                new GradientBGParam("魅惑妖术", "#D2F6FF", "#91B0E4")
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
