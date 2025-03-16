using Svg;
using TrimesterPlaner.Services;
using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class ResultViewModel : BindableBase
    {
        public ResultViewModel(IPlaner planer)
        {
            planer.PlanChanged += (data, result) => Result = result;
        }

        private SvgDocument? _Result = null;
        public SvgDocument? Result
        {
            get => _Result;
            set => SetProperty(ref _Result, value);
        }
    }
}