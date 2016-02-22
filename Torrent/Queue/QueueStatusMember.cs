using PostSharp.Patterns.Model;
using System;

namespace Torrent.Queue
{
    [NotifyPropertyChanged]
    public class QueueStatusMember
        : IComparable<QueueStatusMember>, IComparable<int>, IComparable,
            IEquatable<int>, IEquatable<QueueStatusMember>, IEquatable<string>
    {
        public string Name { get; }

        public string Title { get; }

        public int Value { get; }

        public QueueStatusMember(string name, int value) : this(name, name, value) {}

        public QueueStatusMember(string name, string title, int value)
        {
            this.Name = name.Replace(" ", "");
            this.Title = title;
            this.Value = value;
        }

        public QueueStatusMember(string name, QueueStatusMember previous, int offset = 1)
            : this(name, name, previous, offset) {}

        public QueueStatusMember(string name, string title, QueueStatusMember previous, int offset = 1)
        {
            this.Name = name.Replace(" ", "");
            this.Title = title;
            if (previous == null)
            {
                throw new ArgumentNullException(nameof(previous),
                                                $"{nameof(QueueStatusMember)}.ctor({nameof(name)}, {nameof(previous)}, {nameof(offset)})");
            }
            this.Value = previous.Value + offset;
        }

        public override string ToString()
            => Title;

        public override int GetHashCode()
            => Name.GetHashCode() ^ Title.GetHashCode() ^ Value;

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }
            var member = o as QueueStatusMember;
            if (member != null)
            {
                return this.Equals(member);
            }
            var name = o as string;
            if (name != null)
            {
                return this.Equals(name);
            }
            var value = o as int?;
            if (value.HasValue)
            {
                return this.Equals(value.Value);
            }
            return false;
        }

        public bool Equals(QueueStatusMember other)
            => other.Name == this.Name && other.Value == this.Value;

        public bool Equals(string other)
            => other == this.Name || other == this.Title;

        public bool Equals(int other)
            => other == this.Value;

        #region IComparable implementation

        int IComparable<QueueStatusMember>.CompareTo(QueueStatusMember other)
            => other == null ? 1 : this.Value.CompareTo(other.Value);


        int IComparable<int>.CompareTo(int other)
            => this.Value.CompareTo(other);

        int IComparable.CompareTo(object obj)
            => CompareTo(obj);

        public int CompareTo(object obj)
        {
            var member = obj as QueueStatusMember;
            if (member != null)
            {
                return this.Value.CompareTo(member.Value);
            }
            if (obj is int)
            {
                return this.Value.CompareTo((int) obj);
            }
            throw new NotImplementedException();
        }

        public int CompareTo(QueueStatusMember obj)
            => this.Value.CompareTo(obj.Value);

        public int CompareTo(int obj)
            => this.Value.CompareTo(obj);

        public static implicit operator int(QueueStatusMember member)
        {
            if (member == null)
            {
                throw new NotImplementedException();
            }
            return member.Value;
        }

        public static bool operator ==(QueueStatusMember a, QueueStatusMember b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if ((a == null) || (b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator ==(int a, QueueStatusMember b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(b, null))
            {
                return false;
            }

            // If one is null, but not both, return false.
            if (((object) a == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b.Value);
        }

        public static bool operator ==(QueueStatusMember a, int b)
            => b == a;

        public static bool operator !=(QueueStatusMember a, QueueStatusMember b)
            => !(a == b);

        public static bool operator !=(QueueStatusMember a, int b)
            => !(a == b);

        public static bool operator !=(int a, QueueStatusMember b)
            => !(a == b);


        public static bool operator <(QueueStatusMember operand1, QueueStatusMember operand2)
            => operand1.CompareTo(operand2) < 0;

        public static bool operator <(int operand1, QueueStatusMember operand2)
            => operand1.CompareTo(operand2) < 0;

        public static bool operator <(QueueStatusMember operand1, int operand2)
            => operand1.CompareTo(operand2) < 0;

        public static bool operator >(QueueStatusMember operand1, QueueStatusMember operand2)
            => operand1.CompareTo(operand2) > 0;

        public static bool operator >(int operand1, QueueStatusMember operand2) => operand1.CompareTo(operand2) > 0;

        public static bool operator >(QueueStatusMember operand1, int operand2) => operand1.CompareTo(operand2) > 0;

        public static bool operator <=(QueueStatusMember operand1, QueueStatusMember operand2)
            => operand1.CompareTo(operand2) <= 0;

        public static bool operator <=(int operand1, QueueStatusMember operand2) => operand1.CompareTo(operand2) <= 0;

        public static bool operator <=(QueueStatusMember operand1, int operand2) => operand1.CompareTo(operand2) <= 0;

        public static bool operator >=(QueueStatusMember operand1, QueueStatusMember operand2)
            => operand1.CompareTo(operand2) >= 0;

        public static bool operator >=(int operand1, QueueStatusMember operand2) => operand1.CompareTo(operand2) >= 0;

        public static bool operator >=(QueueStatusMember operand1, int operand2) => operand1.CompareTo(operand2) >= 0;

        #endregion

        static QueueStatusRegion? _error = null;

        static QueueStatusRegion error
        {
            get
            {
                if (!_error.HasValue)
                {
                    _error = new QueueStatusRegion(QueueStatus.LoadError, QueueStatus.Invalid);
                }
                return _error.Value;
            }
        }

        static QueueStatusRegion? _loadError = null;

        static QueueStatusRegion loadError
        {
            get
            {
                if (!_loadError.HasValue)
                {
                    _loadError = new QueueStatusRegion(error.Start, QueueStatus.TorrentInfoError);
                }
                return _loadError.Value;
            }
        }

        static QueueStatusRegion? _torrentError = null;

        static QueueStatusRegion torrentError
        {
            get
            {
                if (!_torrentError.HasValue)
                {
                    _torrentError = new QueueStatusRegion(loadError.End, error.End);
                }
                return _torrentError.Value;
            }
        }

        static QueueStatusMember validStart => QueueStatus.Pending;
        static QueueStatusRegion? _invalid = null;

        static QueueStatusRegion invalid
        {
            get
            {
                if (!_invalid.HasValue)
                {
                    _invalid = new QueueStatusRegion(QueueStatus.Invalid, validStart);
                }
                return _invalid.Value;
            }
        }

        static QueueStatusRegion? _dupe = null;

        static QueueStatusRegion dupe
        {
            get
            {
                if (!_dupe.HasValue)
                {
                    _dupe = new QueueStatusRegion(QueueStatus.Dupe, validStart);
                }
                return _dupe.Value;
            }
        }

        public QueueStatusMember SetReady() => this == QueueStatus.Queued ? QueueStatus.Ready : this;
        bool isBetween(QueueStatusRegion region) => region.Start <= this && this < region.End;

        [SafeForDependencyAnalysis]
        public bool IsPending => this == QueueStatus.Pending;

        [SafeForDependencyAnalysis]
        public bool IsActive => this == QueueStatus.Active;

        [SafeForDependencyAnalysis]
        public bool IsRunning => this.IsPending || this.IsActive;

        [SafeForDependencyAnalysis]
        public bool IsInProgress => (!this.IsSuccess && validStart <= this);

        [SafeForDependencyAnalysis]
        public bool IsSuccess => this == QueueStatus.Success;

        [SafeForDependencyAnalysis]
        public bool IsInvalid => isBetween(invalid);

        [SafeForDependencyAnalysis]
        public bool IsDupe => isBetween(dupe);

        [SafeForDependencyAnalysis]
        public bool IsActivatable => (this == QueueStatus.Ready || this == QueueStatus.Queued);

        [SafeForDependencyAnalysis]
        public bool IsQueued => (this == QueueStatus.Queued);

        [SafeForDependencyAnalysis]
        public bool IsError => isBetween(error);

        [SafeForDependencyAnalysis]
        public bool IsLoadError => isBetween(loadError);

        [SafeForDependencyAnalysis]
        public bool IsTorrentError => isBetween(torrentError);
    }
}