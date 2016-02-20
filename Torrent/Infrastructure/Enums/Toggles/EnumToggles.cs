using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torrent.Infrastructure.Enums.Toggles
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using AddGenericConstraint;
    using Extensions;
    using global::Torrent.Exceptions;
    using PostSharp.Patterns.Model;
    using Puchalapalli.Dynamic;
    using Serialization;
    [DataContract(Namespace = NamespaceAttribute.Default)]
    [PostSharp.Patterns.Model.NotifyPropertyChanged]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class EnumToggles<[AddGenericConstraint(typeof(Enum))] TEnum, TResult>
        : SimpleExpando, IEnumToggles<TEnum, TResult>, IDebuggerDisplay, INotifyPropertyChanged
        where TEnum : struct
        where TResult : struct
        //where TSettings : IEnumTogglesContainer<TEnum, TEnum>, new()
    {
        Type _genericType, _toggleType = null;
        [PostSharp.Patterns.Model.IgnoreAutoChangeNotification]
        protected virtual object[] DebuggerDisplayProperties 
            => new object[] { };
        #region Types
        [Browsable(false)]
        protected Type Type { get; }
        [Browsable(false)]
        Type GenericType
            => this._genericType ?? (this._genericType 
            = this.Type?.GetBaseGenericTypeDefinition());
        [Browsable(false)]
        Type EnumType { get; }
            = typeof(TEnum);
        [Browsable(false)]
        Type ResultType { get; }
            = typeof(TResult);
        [SafeForDependencyAnalysis]
        [Browsable(false)]
        public Type ToggleType
            => this._toggleType ?? this.MakeToggleType(this.GenericType);
        [Browsable(false)]
        bool IsToggleType
            => ResultType == typeof(bool);
        [Browsable(false)]
        Type MakeToggleType<TSettings>()
            => MakeToggleType(typeof(TSettings));
        [Browsable(false)]
        Type MakeToggleType(Type baseType)
            => (baseType?.MakeGenericTypeFromBase(this.EnumType, typeof(bool)));
        #endregion
        [Browsable(false)]
        private TEnum Flag { get; set; }
        [Browsable(false)]
        private bool HasFlag { get; set; } = false;
        public EnumToggles() { Type = this.GetType(); }
        public EnumToggles(IEnumToggles<TEnum, TEnum> toggles, TEnum flag) : base(toggles)
        {
            Type = this.GetType();
            SetFlag(flag);
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        //public void SetToggles<TOther>(IEnumToggles<TEnum, TOther> toggles, TEnum flag)
        //    where TOther : struct
        public void SetToggles(IEnumToggles<TEnum, TEnum> toggles, TEnum flag)
        {
            //if (typeof(TOther) != typeof(TEnum))
            //{
            //    throw new EnumToggleStateException($"{GenericType.Name}: Cannot create toggle from derived instance");
            //}
            base.Initialize(toggles);
            SetFlag(flag);
        }

        [Browsable(false)]
        internal protected void SetFlag(TEnum flag)
        {
            Flag = flag;
            HasFlag = true;
            SetterEnabled = false;
        }
        /// <summary>
        /// Try to retrieve a member by name first from instance properties
        /// followed by the collection entries.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="objResult"></param>
        /// <returns></returns>
        [Browsable(false)]
        public override bool TryGetMember(GetMemberBinder binder, out object objResult)
        {
            var success = base.TryGetMember(binder, out objResult);
            if (!HasFlag || !success)
            {
                return success;
            }

            var nested = objResult as IEnumToggles<TEnum>;
            if (nested != null)
            {
                objResult = nested.GetActiveToggles(Flag);
                return true;
            }
            var value = (TEnum)objResult;
            var result = value.Has(Flag);
            objResult = result;
            return true;
        }
        //public TSettings CreateToggle<TSettings>(TEnum flag)
        //    where TSettings : IEnumToggles<TEnum, bool>, new()
        //{
        //    if (IsToggleType)
        //    {
        //        throw new EnumToggleStateException($"{GenericType.Name}: Cannot create toggle from derived instance");
        //    }
        //    var toggles = Activator.CreateInstance(
        //        this.MakeToggleType<TSettings>());
        //    (toggles as IEnumToggles<TEnum, bool>).SetToggles(this, flag);// new object[] { this, flag });
        //    return (TSettings) toggles;
        //}
        #region Interfaces
        #region Interfaces: INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => NotifyPropertyChangedBase.DoOnPropertyChanged(this, PropertyChanged, propertyName.Split(';'));
        #endregion
        #region Interfaces: IDebuggerDisplay
        public virtual string DebuggerDisplay(int level = 1)
            => this.DebuggerDisplaySimple(level);
        public virtual string DebuggerDisplaySimple(int level = 1)
            => $"[{this.GenericType.Name} [{string.Join(", ", this.DebuggerDisplayProperties)}]";
        #endregion
        #endregion
    }
    public class EnumToggles<[AddGenericConstraint(typeof(Enum))] TEnum>
        : EnumToggles<TEnum, TEnum>, IEnumToggles<TEnum>
        where TEnum : struct
        //where TSettings : IEnumTogglesContainer<TEnum, TEnum>, new()
    {
        
    }
}
