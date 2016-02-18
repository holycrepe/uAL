using System;
using System.Dynamic;
using AddGenericConstraint;
using Puchalapalli.Dynamic;

namespace Torrent.Infrastructure.Enums.Toggles
{
    public interface IEnumToggles<[AddGenericConstraint(typeof(Enum))]
    TEnum, TResult>
      where TEnum : struct
      where TResult : struct
    {        
        Type ToggleType { get; }

        //TSettings CreateToggle<TSettings>(TEnum flag) where TSettings : IEnumToggles<TEnum, bool>, new();
        void SetToggles(IEnumToggles<TEnum, TEnum> toggles, TEnum flag);
        //void SetToggles<TOther>(IEnumToggles<TEnum, TOther> toggles, TEnum flag)
        //    where TOther : struct;
        bool TryGetMember(GetMemberBinder binder, out object objResult);
    }
    public interface IEnumToggles<[AddGenericConstraint(typeof(Enum))]
    TEnum> : IEnumToggles<TEnum, TEnum>
        where TEnum : struct
        //where TSettings : IEnumTogglesContainer<TEnum, TEnum>, new()
    {

    }
}