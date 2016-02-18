using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torrent.Infrastructure.Enums.Toggles
{
    using System.ComponentModel;
    using System.Dynamic;
    using System.Reflection;
    using System.Runtime.Serialization;
    using AddGenericConstraint;
    using Extensions;
    using global::Torrent.Exceptions;
    using Properties.Settings;
    using Puchalapalli.Dynamic;
    using Serialization;
    using static Serialization.DataContractUtils;
    public class EnumToggleSettings<[AddGenericConstraint(typeof(Enum))] TEnum, TResult>
        : EnumToggles<TEnum, TResult>
        where TEnum : struct
        where TResult : struct
        //where TSettings : IEnumTogglesContainer<TEnum, TEnum>, new()
    {
        #region Load/Save
        public static T Load<T>() where T : EnumToggleSettings<TEnum, TResult>, new()
            => ReadFromFile<T>(BaseSettings.GetPath<T>());
        public virtual void Save()
            => WriteToFile(BaseSettings.GetPath(Type), this);
        //protected static T LoadXml<T>(string name = null) where T : EnumToggleSettings<TEnum, TResult>, new()
        //    => XmlUtils.ReadFromFile<T>(BaseSettings.GetPath<T>());
        //public virtual void SaveXml()
        //    => XmlUtils.WriteToFile(BaseSettings.GetPath(this.Type), this);
        //protected static T LoadDataContract<T>(string name = null) where T : EnumToggleSettings<TEnum, TResult>, new()
        //    => DataContractUtils.ReadFromFile<T>(BaseSettings.GetPath<T>());
        //public virtual void SaveDataContract()
        //    => DataContractUtils.WriteToFile(BaseSettings.GetPath(Type), this);
        #endregion
    }
    public class EnumToggleSettings<[AddGenericConstraint(typeof(Enum))] TEnum>
        : EnumToggles<TEnum, TEnum>, IEnumToggles<TEnum>
        where TEnum : struct
        //where TSettings : IEnumTogglesContainer<TEnum, TEnum>, new()
    {
        
    }
}
