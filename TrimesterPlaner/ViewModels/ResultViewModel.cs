using Svg;
using TrimesterPlaner.Services;
using TrimesterPlaner.Utilities;
using Utilities.Extensions;

namespace TrimesterPlaner.ViewModels
{
    public class ResultViewModel : BindableBase
    {
        public ResultViewModel()
        {
            Inject.Require<IPlaner>().PlanChanged += (data, result) => Result = result;
        }

        private SvgDocument? _Result = null;
        public SvgDocument? Result
        {
            get => _Result;
            set => SetProperty(ref _Result, value);
        }
    }
}