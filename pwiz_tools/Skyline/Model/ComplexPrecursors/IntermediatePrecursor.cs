
using pwiz.Common.SystemUtil;
using pwiz.Skyline.Model.DocSettings;

namespace pwiz.Skyline.Model.ComplexPrecursors
{
    public class IntermediatePrecursor : Immutable
    {
        public IntermediatePrecursor(Transition transition, TransitionLosses losses)
        {
            Transition = transition;
            Losses = losses;
        }

        public Transition Transition { get; private set; }

        public TransitionLosses Losses { get; private set; }

        protected bool Equals(IntermediatePrecursor other)
        {
            return Transition.Equals(other.Transition) && Equals(Losses, other.Losses);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntermediatePrecursor) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Transition.GetHashCode() * 397) ^ (Losses?.GetHashCode() ?? 0);
            }
        }
    }
}
