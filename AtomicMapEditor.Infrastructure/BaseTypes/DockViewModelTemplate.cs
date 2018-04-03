using Prism.Mvvm;

namespace Ame.Infrastructure.BaseTypes
{
    public abstract class DockViewModelTemplate : BindableBase
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        #endregion constructor & destructer


        #region properties

        private bool isActive = false;
        public bool IsActive
        {
            get { return this.isActive; }
            set { SetProperty(ref this.isActive, value); }
        }

        private bool isSelected = false;
        public bool IsSelected
        {
            get { return this.isSelected; }
            set { SetProperty(ref this.isSelected, value); }
        }

        private string title;
        public string Title
        {
            get { return this.title; }
            set { SetProperty(ref this.title, value); }
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
