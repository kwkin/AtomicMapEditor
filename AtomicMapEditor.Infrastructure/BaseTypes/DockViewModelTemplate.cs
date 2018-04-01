using Prism.Mvvm;

namespace AtomicMapEditor.Infrastructure.BaseTypes
{
    public abstract class DockViewModelTemplate : BindableBase
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

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }

        public abstract DockType DockType { get; }

        public string ContentId
        {
            get { return DockTypeUtils.GetId(this.DockType); }
        }


        #endregion properties


        #region methods

        #endregion methods
    }
}
