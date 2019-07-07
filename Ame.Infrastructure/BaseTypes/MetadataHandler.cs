using Ame.Infrastructure.Attributes;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Ame.Infrastructure.BaseTypes
{
    public class MetadataHandler
    {
        #region fields

        #endregion fields


        #region Constructor

        public MetadataHandler(IContainsMetadata item)
            : this(item, new ObservableCollection<MetadataProperty>())
        {
        }

        public MetadataHandler(IContainsMetadata item, ObservableCollection<MetadataProperty> properties)
        {
            this.Item = item ?? throw new ArgumentNullException("item is null");

            this.MetadataList = new ObservableCollection<MetadataProperty>();
            AddMetadata(this.Item);
            AddMetadata(properties);

            this.MetadataCollection = new ListCollectionView(this.MetadataList);
            this.MetadataCollection.GroupDescriptions.Add(new PropertyGroupDescription("Type"));

            this.SelectedMetadata.PropertyChanged += SelectedMetadataChanged;

            this.AddCustomMetaDataCommand = new DelegateCommand(() => AddNewCustomProperty());
            this.RemoveCustomMetadataCommand = new DelegateCommand(() => RemoveCustomProperty());
            this.MoveMetadataUpCommand = new DelegateCommand(() => MoveCurrentMetadataUp());
            this.MoveMetadataDownCommand = new DelegateCommand(() => MoveCurrentMetadataDown());
        }

        #endregion Constructor


        #region Properties

        public ICommand AddCustomMetaDataCommand { get; private set; }
        public ICommand RemoveCustomMetadataCommand { get; private set; }
        public ICommand MoveMetadataUpCommand { get; private set; }
        public ICommand MoveMetadataDownCommand { get; private set; }

        public ICollectionView MetadataCollection { get; set; }
        public ObservableCollection<MetadataProperty> MetadataList { get; set; }
        public BindableProperty<MetadataProperty> SelectedMetadata { get; set; } = BindableProperty<MetadataProperty>.Prepare();
        public BindableProperty<bool> IsCustomSelected { get; set; } = BindableProperty<bool>.Prepare();

        private IContainsMetadata Item { get; set; }

        #endregion Properties


        #region methods

        public void AddMetadata(MetadataProperty metadata)
        {
            this.MetadataList.Add(metadata);
        }

        public void AddMetadata(ObservableCollection<MetadataProperty> metadataList)
        {
            this.MetadataList.AddRange(metadataList);
        }

        public void AddNewCustomProperty()
        {
            int customCount = this.MetadataList.Count(p => p.Type == MetadataType.Custom);
            string customName = string.Format("Custom #{0}", customCount);
            MetadataProperty property = new MetadataProperty(customName, "", MetadataType.Custom);
            this.Item.CustomProperties.Add(property);
            this.MetadataList.Add(property);
        }

        public void RemoveCustomProperty()
        {
            RemoveCustomProperty(this.SelectedMetadata.Value);
        }

        public void RemoveCustomProperty(MetadataProperty property)
        {
            if (property.Type == MetadataType.Custom)
            {
                this.MetadataList.Remove(property);
            }
        }

        public void MoveCurrentMetadataUp()
        {
            MoveMetadataUp(this.MetadataCollection.CurrentItem as MetadataProperty);
        }

        public void MoveMetadataUp(MetadataProperty currentItem)
        {
            int currentIndex = this.MetadataList.IndexOf(currentItem);
            if (currentIndex == -1)
            {
                return;
            }
            MetadataType currentItemType = currentItem.Type;

            int propertyIndex = 0;
            int statisticIndex = this.MetadataList.Count(p => p.Type == MetadataType.Property) + propertyIndex;
            int customIndex = this.MetadataList.Count(p => p.Type == MetadataType.Statistic) + statisticIndex;
            int lowestIndex = 0;
            switch (currentItemType)
            {
                case MetadataType.Property:
                    lowestIndex = propertyIndex;
                    break;

                case MetadataType.Statistic:
                    lowestIndex = statisticIndex;
                    break;

                case MetadataType.Custom:
                    lowestIndex = customIndex;
                    break;

                default:
                    break;
            }
            if (currentIndex > lowestIndex)
            {
                this.MetadataList.Move(currentIndex, currentIndex - 1);
                this.MetadataCollection.Refresh();
            }
        }

        public void MoveCurrentMetadataDown()
        {
            MoveMetadataDown(this.MetadataCollection.CurrentItem as MetadataProperty);
        }

        public void MoveMetadataDown(MetadataProperty currentItem)
        {
            int currentIndex = this.MetadataList.IndexOf(currentItem);
            if (currentIndex == -1)
            {
                return;
            }
            MetadataType currentItemType = currentItem.Type;

            int propertyIndex = 0;
            int statisticIndex = this.MetadataList.Count(p => p.Type == MetadataType.Property) + propertyIndex;
            int customIndex = this.MetadataList.Count(p => p.Type == MetadataType.Statistic) + statisticIndex;
            int highestIndex = 0;
            switch (currentItemType)
            {
                case MetadataType.Property:
                    highestIndex = statisticIndex - 1;
                    break;

                case MetadataType.Statistic:
                    highestIndex = customIndex - 1;
                    break;

                case MetadataType.Custom:
                    highestIndex = this.MetadataList.Count - 1;
                    break;

                default:
                    break;
            }
            if (currentIndex < highestIndex)
            {
                this.MetadataList.Move(currentIndex, currentIndex + 1);
                this.MetadataCollection.Refresh();
            }
        }

        private void SelectedMetadataChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.SelectedMetadata.Value != null)
            {
                this.IsCustomSelected.Value = this.SelectedMetadata.Value.Type == MetadataType.Custom ? true : false;
            }
        }

        private void AddMetadata(IContainsMetadata item)
        {
            this.MetadataList.AddRange(MetadataPropertyUtils.GetPropertyList(item));
            foreach (MetadataProperty property in item.CustomProperties)
            {
                this.MetadataList.Add(property);
            }
        }

        #endregion methods
    }
}
