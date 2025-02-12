using TrimesterPlaner.Utilities;

namespace TrimesterPlaner.ViewModels
{
    public class ResultWindowViewModel : BindableBase
    {
        public ResultWindowViewModel(IEntwicklungsplanManager entwicklungsplanManager) : base()
        {
            entwicklungsplanManager.EntwicklungsplanChanged += (data, result) => Title = data?.Title ?? "";
        }

        private string _Title = string.Empty;
        public string Title
        {
            get => _Title;
            set => SetProperty(ref _Title, value);
        }
    }
}