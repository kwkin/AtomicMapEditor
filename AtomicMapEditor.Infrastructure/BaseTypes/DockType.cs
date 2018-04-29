using System;
using System.Reflection;

namespace Ame.Infrastructure.BaseTypes
{
    public enum DockType
    {
        [DockContentId("Clipboard")]
        Clipboard,

        [DockContentId("ItemEditor")]
        ItemEditor,

        [DockContentId("ItemList")]
        ItemList,

        [DockContentId("LayerList")]
        LayerList,

        [DockContentId("Minimap")]
        Minimap,

        [DockContentId("SelectedBrush")]
        SelectedBrush,

        [DockContentId("SessionView")]
        SessionView,

        [DockContentId("Toolbox")]
        Toolbox,

        [DockContentId("MapEditor")]
        MapEditor,

        [DockContentId("Unknown")]
        Unknown
    }
    
    public static class DockTypeUtils
    {
        #region methods

        public static string GetId(DockType contentId)
        {
            DockContentIdAttribute attr = GetAttr(contentId);
            return attr.Id;
        }

        public static DockType GetById(string idString)
        {
            DockType selectedId = DockType.Unknown;
            DockType[] dockContentIds = (DockType[])Enum.GetValues(typeof(DockType));
            foreach (DockType id in dockContentIds)
            {
                DockContentIdAttribute attr = GetAttr(id);
                if (attr.Id == idString)
                {
                    selectedId = id;
                    break;
                }
            }
            return selectedId;
        }

        private static DockContentIdAttribute GetAttr(DockType p)
        {
            return (DockContentIdAttribute)Attribute.GetCustomAttribute(ForValue(p), typeof(DockContentIdAttribute));
        }

        private static MemberInfo ForValue(DockType p)
        {
            return typeof(DockType).GetField(Enum.GetName(typeof(DockType), p));
        }

        #endregion methods
    }

    internal class DockContentIdAttribute : Attribute
    {
        #region constructor

        internal DockContentIdAttribute(string id)
        {
            this.Id = id;
        }

        #endregion constructor


        #region properties

        public string Id { get; private set; }

        #endregion properties
    }
}
