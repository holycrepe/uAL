using System;
using System.ComponentModel;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Torrent.Helpers.Utils;

namespace wUAL.Extensions
{
    public static class RadGridViewExtensions
    {
        public static ColumnGroupDescriptor MakeColumnGroupDescriptor(this RadGridView grid, string name, string image = null, bool ascending = true)
        {
            image = image ?? name;
            var descriptor = new ColumnGroupDescriptor();
            descriptor.Column = grid.Columns[name];
            descriptor.DisplayContent = ResourceUtils.GetColumnGroupHeader(image);
            descriptor.SortDirection = ascending ? ListSortDirection.Ascending : ListSortDirection.Descending;
            return descriptor;
        }
        public static RadGridView AddColumnGroupDescriptor(this RadGridView grid, string name, string image = null, bool ascending = true)
        {
            grid.GroupDescriptors.Add(grid.MakeColumnGroupDescriptor(name, image, ascending));
            return grid;
        }
        public static ColumnSortDescriptor MakeColumnSortDescriptor(this RadGridView grid, string name, bool ascending = true)
        {
            var descriptor = new ColumnSortDescriptor();
            descriptor.Column = grid.Columns[name];
            descriptor.SortDirection = ascending ? ListSortDirection.Ascending : ListSortDirection.Descending;
            return descriptor;
        }
        public static RadGridView AddColumnSortDescriptor(this RadGridView grid, string name, bool ascending = true)
        {
            grid.SortDescriptors.Add(grid.MakeColumnSortDescriptor(name, ascending));
            return grid;
        }
        public static RadGridView AddColumnSortDescriptors(this RadGridView grid, params string[] names)
        {
            foreach (var name in names)
            {
                grid.SortDescriptors.Add(grid.MakeColumnSortDescriptor(name));
            }
            return grid;
        }
    }
}
