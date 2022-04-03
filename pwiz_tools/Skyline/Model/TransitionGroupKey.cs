using pwiz.Common.Collections;
using pwiz.Skyline.Model.ComplexPrecursors;

namespace pwiz.Skyline.Model
{
    public class TransitionGroupKey
    {
        public TransitionGroupKey(TransitionGroup transitionGroup,
            ImmutableList<IntermediatePrecursor> intermediatePrecursors)
        {
            TransitionGroup = transitionGroup;
            IntermediatePrecursors = intermediatePrecursors;
        }

        public TransitionGroup TransitionGroup
        {
            get;
        }

        public ImmutableList<IntermediatePrecursor> IntermediatePrecursors { get; }

        protected bool Equals(TransitionGroupKey other)
        {
            return TransitionGroup.Equals(other.TransitionGroup) &&
                   IntermediatePrecursors.Equals(other.IntermediatePrecursors);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TransitionGroupKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TransitionGroup.GetHashCode() * 397) ^ IntermediatePrecursors.GetHashCode();
            }
        }
    }
}
