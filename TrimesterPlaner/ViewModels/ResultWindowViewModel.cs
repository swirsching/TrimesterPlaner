using Svg;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class ResultWindowViewModel : BindableBase
    {
        public ResultWindowViewModel(IEntwicklungsplanManager entwicklungsplanManager) : base()
        {
            entwicklungsplanManager.EntwicklungsplanChanged += (data, result) =>
            {
                Title = data?.Title ?? "";
                Result = result;
            };
        }

        private string _Title = string.Empty;
        public string Title
        {
            get => _Title;
            set => SetProperty(ref _Title, value);
        }

        private SvgDocument? _Result = null;
        public SvgDocument? Result
        {
            get => _Result;
            set => SetProperty(ref _Result, value);
        }
    }
}