using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using Torrent.Helpers.Utils;

namespace Torrent.Extensions.BEncode
{
    using BencodeNET;
    using BencodeNET.Objects;
    using Helpers.StringHelpers;
    using Infrastructure.Interfaces;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    public static class BObjectExtensions
    {
        public static List<T> ToList<T>(this IBObject obj) where T : IBListLoadable, new()
        {
            var list = new List<T>();
            foreach (var item in (BList) obj)
            {
                var newItem = new T();
                newItem.LoadFromBList((BList) item);
                list.Add(newItem);
            }
            return list;
        }
        public static T[] ToArray<T>(this IBObject obj) where T : IBListLoadable, new()
            => obj.ToList<T>().ToArray();
        public static bool ToBoolean(this IBObject obj)
        {
            long num = (BNumber)obj;
            if (num == 0)
            {
                return false;
            }
            if (num == 1)
            {
                return true;
            }
            Debugger.Break();
            return false;
        }
        public static string CalculateFileguard(this BDictionary dict)
        {
            var stream = new MemoryStream();            
            if (dict.ContainsKey(".fileguard"))
            {
                dict.Remove(".fileguard");
            }
            dict.EncodeToStream(stream);
            return HashUtils.SHA1(stream);
        }
        public static BDictionary SetFileguard(this BDictionary dict)
        {
            var fileGuard = dict.CalculateFileguard();
            dict[".fileguard"] = new BString(fileGuard);
            return dict;
        }
        public static TimeSpan ToTimeSpan(this IBObject obj)
            => TimeSpan.FromSeconds((BNumber)obj);
        public static byte[] ToBytes(this IBObject obj)
            => ((BString)obj).Value;
        public static string ToHex(this IBObject obj)
            => ((BString)obj).Value.ToHex();
        public static List<string> ToList(this IBObject obj)
            => ((BList)obj).Select(s => s.ToString()).ToList();
        public static ObservableCollection<string> ToObservableCollection(this IBObject obj)
            => new ObservableCollection<string>(((BList)obj).Select(s => s.ToString()));
        public static DateTime ToDateTime(this IBObject obj)
            => DateUtils.FromUnix((BNumber)obj);
    }
    public static class ListExtensions
    {
        public static BList ToBList<T>(this IEnumerable<T> list) where T : IBListLoadable
        {
            var blist = new BList();
            foreach (var item in list)
            {
                blist.Add(item.ToBList());
            }
            return blist;
        }
        public static BList ToBList(this IEnumerable<string> list)
        {
            var blist = new BList();
            foreach (var item in list)
            {
                blist.Add(item);
            }
            return blist;
        }
    }
    public static class DateTimeExtensions
    {
        public static BNumber ToBNumber(this DateTime date)
            => new BNumber(DateUtils.ToUnix(date));
    }
    public static class BooleanExtensions
    {
        public static BNumber ToBNumber(this bool value)
            => value ? 1 : 0;
    }
}
