using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pwiz.Common.Collections;

namespace pwiz.ProteowizardWrapper
{
    public class PrecursorsByMsLevel
    {
        public static readonly PrecursorsByMsLevel EMPTY =
            new PrecursorsByMsLevel(ImmutableList.Empty<ImmutableList<MsPrecursor>>());
        private ImmutableList<ImmutableList<MsPrecursor>> _precursorsByLevel;

        public PrecursorsByMsLevel(IEnumerable<ImmutableList<MsPrecursor>> levels)
        {
            _precursorsByLevel = ImmutableList.ValueOf(levels);
        }

        public PrecursorsByMsLevel(MsPrecursor precursor) : this(ImmutableList.Singleton(ImmutableList.Singleton(precursor)))
        {

        }

        public static PrecursorsByMsLevel FromMs1Precursors(IEnumerable<MsPrecursor> precursors)
        {
            return new PrecursorsByMsLevel(ImmutableList.Singleton(ImmutableList.ValueOf(precursors)));
        }

        public ImmutableList<MsPrecursor> GetPrecursors(int msLevel)
        {
            int index = msLevel - 1;
            if (index < 0 || index >= _precursorsByLevel.Count)
            {
                return ImmutableList<MsPrecursor>.EMPTY;
            }

            return _precursorsByLevel[index];
        }

        public ImmutableList<MsPrecursor> LastPrecursors()
        {
            return GetPrecursors(_precursorsByLevel.Count);
        }

        public int HighestMsLevel
        {
            get
            {
                return _precursorsByLevel.Count + 1;
            }
        }
    }
}
