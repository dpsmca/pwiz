using pwiz.Common.Collections;
using pwiz.Common.SystemUtil;

namespace pwiz.Skyline.Model.ComplexPrecursors
{
    public class ComplexPrecursor : Immutable
    {
        public ImmutableList<ImmutableList<IntermediatePrecursor>> IntermediatePrecursors { get; private set; }

        protected bool Equals(ComplexPrecursor other)
        {
            return Equals(IntermediatePrecursors, other.IntermediatePrecursors);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ComplexPrecursor) obj);
        }

        public override int GetHashCode()
        {
            return IntermediatePrecursors.GetHashCode();
        }
    }
}
