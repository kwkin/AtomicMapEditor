using Prism.Mvvm;

namespace Ame.Infrastructure.BaseTypes
{
    public class EditorViewModelTemplate : BindableBase
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        #endregion constructor & destructer


        #region properties

        private bool _IsActive = false;

        public bool IsActive
        {
            get { return _IsActive; }
            set { SetProperty(ref _IsActive, value); }
        }

        private bool _IsSelected = false;

        public bool IsSelected
        {
            get { return _IsSelected; }
            set { SetProperty(ref _IsSelected, value); }
        }

        private string _ContentId = null;

        public string ContentId
        {
            get { return _ContentId; }
            set { SetProperty(ref _ContentId, value); }
        }

        private string _Title;

        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }

        #endregion properties


        #region methods

        #endregion methods
    }
}
