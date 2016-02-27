
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using System.Diagnostics;

namespace wUAL.WPF
{
    public class GridViewSelectionInfo<T>
    {
        public GridViewCell Cell;
        public List<T> Selection = new List<T>();
        public GridViewSelectionInfo(GridViewCell cell, IList<T> selection = null)
        {
            Reset(cell, selection);
        }
        public void Add(T item)
            => Selection.Add(item);
        public int Count
            => Selection.Count;
        public void Reset(GridViewCell cell, IList<T> selection = null)
        {
            this.Cell = cell;
            Selection.Clear();
            if (selection != null)
            {
                Selection.AddRange(selection);
            }
        }
    }
    public class GridMultiEditBehavior : Behavior<RadGridView>
    {
        public RadGridView Grid
            => AssociatedObject;
        public bool AutoCommit { get; set; } 
            = true;

        private static readonly IDictionary<string, GridViewSelectionInfo<GridViewCellInfo>> allSelectedCells = new Dictionary<string, GridViewSelectionInfo<GridViewCellInfo>>();
        private static readonly IDictionary<string, GridViewSelectionInfo<object>> allSelectedItems = new Dictionary<string, GridViewSelectionInfo<object>>();

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AutoCommit)
            {
                Grid.CellEditEnded += RadGridViewCellEditEnded;
            }
            Grid.BeginningEdit += GridBeginningEdit;
        }
        static string GetKey(GridViewCell cell)
            => GetKey(cell.ParentOfType<RadGridView>());
        static string GetKey(RadGridView grid)
            => string.IsNullOrEmpty(grid.Name) ? $"HASH*{grid.GetHashCode()}" : grid.Name;
        public static IEnumerable<T> GetSelectedItems<T>(GridViewCell cell)
        {
            var key = GetKey(cell.ParentOfType<RadGridView>());
            if (allSelectedItems.ContainsKey(key))
            {
                return allSelectedItems[key].Selection.Cast<T>();
            }
            else if (allSelectedCells.ContainsKey(key))
            {
                return allSelectedCells[key].Selection.Select(s => (T)s.Column.DataContext);
            }
            Debugger.Break();
            return new T[0];
        }
        static void GridBeginningEdit(object sender, GridViewBeginningEditRoutedEventArgs e)
        {
            var grid = (RadGridView)sender;
            var cell = e.Cell;
            var gridKey = GetKey(cell);

            switch (grid.SelectionUnit)
            {
                case GridViewSelectionUnit.FullRow:
                    if (allSelectedItems.ContainsKey(gridKey))
                    {
                        allSelectedItems[gridKey].Reset(cell, grid.SelectedItems);
                    }
                    else
                    {
                        allSelectedItems[gridKey] = new GridViewSelectionInfo<object>(cell, grid.SelectedItems);
                    }
                    break;

                case GridViewSelectionUnit.Cell:
                    if (allSelectedCells.ContainsKey(gridKey))
                    {
                        allSelectedCells[gridKey].Reset(cell, grid.SelectedCells);
                    }
                    else
                    {
                        allSelectedCells[gridKey] = new GridViewSelectionInfo<GridViewCellInfo>(cell, grid.SelectedCells);
                    }
                    break;
            }
        }
        private static void RadGridViewCellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {            
            if (e.EditAction == GridViewEditAction.Commit)
            {
                CommitMultiEdit(e);
            }            
        }
        private static void CommitMultiEdit(GridViewCellEditEndedEventArgs e)
            => CommitMultiEdit(e.Cell, e.NewData, null, false);
        public static void CommitMultiEdit(RadGridView grid, object newValue, object dataContext = null, bool includeSelectedCell=true)
            => CommitMultiEdit(GetKey(grid), newValue, dataContext, includeSelectedCell);
        public static void CommitMultiEdit(GridViewCell cell, object newValue, object dataContext = null, bool includeSelectedCell = true)
            => CommitMultiEdit(GetKey(cell), newValue, dataContext, includeSelectedCell);
        private static void CommitMultiEdit(string gridKey, object newValue, object dataContext = null, bool includeSelectedCell = true)
        {
            if (allSelectedCells.ContainsKey(gridKey))
            {
                CellMultiEdit(allSelectedCells[gridKey], newValue, includeSelectedCell);
            }
            else if (allSelectedItems.ContainsKey(gridKey))
            {
                FullRowMultiEdit(allSelectedItems[gridKey], newValue, dataContext, includeSelectedCell);
            } 
            else { 
                Debugger.Break();
                return;
            }            
        }
        private static void CellMultiEdit(GridViewSelectionInfo<GridViewCellInfo> selectionInfo, object newValue, bool includeSelectedCell = true)
        {
            if (selectionInfo.Count < (includeSelectedCell ? 1 : 2))
            {
                return;
            }

            foreach (var selected in selectionInfo.Selection)
            {
                if ((!includeSelectedCell && selectionInfo.Cell.Equals(selected)) || selected.Column.IsReadOnly)
                {
                    continue;
                }

                var item = selected.Item;
                var bindingPath = ((GridViewBoundColumnBase)(selected.Column)).DataMemberBinding.Path;
                if (bindingPath.PathParameters.Any())
                {
                    continue;
                }

                var pathProperties = GetPathProperties(item.GetType(), bindingPath.Path);
                SetCellPathValue(item, pathProperties, newValue);

                //var canAssignValue = true;
                //if (selected is System.ComponentModel.IEditableObject)
                //{

                //}
                //else if (selected is Telerik.Pivot.Core.IEditable)
                //{

                //}
                //else
                //{
                //    canAssignValue = false;
                //}

                //if (item is ICanMultiEditProperty)
                //{
                //    var model = item as ICanMultiEditProperty;
                //    var descriptors = model.PropertyAccessDescriptors();
                //    var key = pathProperties.Info[0].Name;

                //    Func<bool> func;
                //    if (descriptors.TryGetValue(key, out func))
                //    {
                //        if (!func())
                //        {
                //            canAssignValue = false;
                //            // If this func return false, it means we can't copy the value to it
                //        }
                //    }
                //}

                //if (canAssignValue)
                //{
                //    SetCellPathValue(item, pathProperties, newValue);
                //}
            }
        }

        private static void SetCellPathValue(object o, PathProperties pathProperties, object value)
        {
            var target = o;
            for (var i = 0; i < pathProperties.Info.Count - 1; i++)
            {
                var pval = pathProperties.Info[i].GetValue(target, null);
                if (pval == null)
                {
                    return;
                }

                target = pval;
            }

            if (target != null)
            {
                if (pathProperties.PropertyIndex != null)
                {
                    var index = pathProperties.PropertyIndex.ToArray();
                    var obj = pathProperties.Info[pathProperties.Info.Count - 1].GetValue(target, null);
                    if (value == null)
                    {
                        Debugger.Break();
                    }
                    obj.GetType().GetProperty("Item").SetValue(obj, value, index);
                }
                else
                {
                    if (value == null)
                    {
                        Debugger.Break();
                    }
                    pathProperties.Info[pathProperties.Info.Count - 1].SetValue(target, value, null);
                }
            }
        }
        
        private static void FullRowMultiEdit(GridViewSelectionInfo<object> selectionInfo, object newValue, object dataContext = null, bool includeSelectedCell = true)
        {
            if (selectionInfo.Count < (includeSelectedCell ? 1 : 2))
            {
                return;
            }
            
            var underlyingRow = dataContext ?? selectionInfo.Cell.DataContext;
            var bindingPath = selectionInfo.Cell.DataColumn.DataMemberBinding.Path;
            if (bindingPath.PathParameters.Any())
            {
                return;
            }

            var pathProperties = GetPathProperties(underlyingRow.GetType(), bindingPath.Path);

            foreach (var selected in selectionInfo.Selection)
            {
                if (includeSelectedCell || !underlyingRow.Equals(selected))
                {
                    SetMultiPathValue(selected, pathProperties, newValue);
                }
                //SetMultiPathValue(selected, pathProperties, newValue);
                //var canAssignValue = true;
                //if (selected is System.ComponentModel.IEditableObject)
                //{

                //}
                //else if (selected is Telerik.Pivot.Core.IEditable)
                //{

                //}
                //else
                //{
                //    // canAssignValue = false;
                //}
                ////if (selected is IEditable)
                ////{
                ////    if (!(selected as IEditable).IsEditable)
                ////    {
                ////        canAssignValue = false;
                ////    }
                ////}

                //if (selected is ICanMultiEditProperty)
                //{
                //    var model = selected as ICanMultiEditProperty;
                //    var descriptors = model.PropertyAccessDescriptors();
                //    var key = bindingPath.Path;

                //    Func<bool> func;
                //    if (descriptors.TryGetValue(key, out func))
                //    {
                //        if (!func())
                //        {
                //            canAssignValue = false;
                //            // If this func return false, it means we can't copy the value to it
                //        }
                //    }
                //}

                //if (canAssignValue)
                //{
                //    SetMultiPathValue(selected, pathProperties, newValue);
                //}
            }
        }

        private static void SetMultiPathValue(object o, PathProperties pathProperties, object value)
        {
            var target = o;
            for (var i = 0; i < pathProperties.Info.Count - 1; i++)
            {
                var pval = pathProperties.Info[i].GetValue(target, null);
                if (pval == null)
                {
                    return;
                }

                target = pval;
            }

            if (target != null)
            {
                if (value == null)
                {
                    //Debugger.Break();
                }
                else {
                    pathProperties.Info[pathProperties.Info.Count - 1].SetValue(target, value, null);
                }
            }
        }

        private static PathProperties GetPathProperties(Type type, string propertyPath)
        {
            var result = new PathProperties();

            var currentType = type;
            var pathElements = propertyPath.Split('.');

            foreach (var t in pathElements)
            {
                var name = t;
                if (t.EndsWith("]"))
                {
                    var bracketBegin = t.IndexOf('[');
                    var bracketEnd = t.IndexOf(']');

                    name = t.Substring(0, bracketBegin);

                    var index = Convert.ToInt32(t.Substring(bracketBegin + 1, bracketEnd - bracketBegin - 1));
                    result.PropertyIndex.Add(index);
                }

                var pi = currentType.GetProperty(name);
                result.Info.Add(pi);

                currentType = pi.PropertyType;
            }

            return result;
        }
    }

    public class PathProperties
    {
        public IList<PropertyInfo> Info { get; set; }
        public IList<object> PropertyIndex { get; set; }
        public PathProperties()
        {
            Info = new List<PropertyInfo>();
            PropertyIndex = new List<object>();
        }
    }

    public interface ICanMultiEditProperty
    {
        Dictionary<string, Func<bool>> PropertyAccessDescriptors();
    }
}
