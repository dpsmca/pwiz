
using System;
using System.Collections.Generic;
using System.Linq;
using pwiz.Common.Chemistry;
using pwiz.Common.Collections;
using pwiz.Common.SystemUtil;
using pwiz.Skyline.Model.Crosslinking;
using pwiz.Skyline.Model.DocSettings;
using pwiz.Skyline.Util;

namespace pwiz.Skyline.Model.ComplexPrecursors
{
    public class IntermediatePrecursor : Immutable
    {
        public IntermediatePrecursor(int msLevel, ComplexFragmentIon complexFragmentIon)
        {
            if (complexFragmentIon == null)
            {
                throw new ArgumentNullException(nameof(complexFragmentIon));
            }
            MsLevel = msLevel;
            ComplexFragmentIon = complexFragmentIon;
        }

        public int MsLevel { get; private set; }

        public ComplexFragmentIon ComplexFragmentIon { get; private set; }

        protected bool Equals(IntermediatePrecursor other)
        {
            return ComplexFragmentIon.Equals(other.ComplexFragmentIon) && Equals(MsLevel, other.MsLevel);
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
                return (ComplexFragmentIon.GetHashCode() * 397) ^ MsLevel.GetHashCode();
            }
        }

        public SignedMz CalculateMz(SrmSettings settings, ExplicitMods explicitMods)
        {
            var mass = ComplexFragmentIon.GetFragmentMass(settings, explicitMods);
            return new SignedMz(ComplexFragmentIon.PrimaryTransition.Adduct.MzFromNeutralMass(mass));
        }

        public override string ToString()
        {
            string result = ComplexFragmentIon.GetFragmentIonName() + Transition.GetMassIndexText(ComplexFragmentIon.PrimaryTransition.MassIndex) + Transition.GetChargeIndicator(ComplexFragmentIon.Adduct);
            if (MsLevel == 2)
            {
                return result;
            }
            return string.Format("MsLevel {0}:", MsLevel) + result;
        }
#if false
        private IntermediatePrecursor()
        {

        }
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (ComplexFragmentIon != null)
            {
                throw new InvalidOperationException();
            }

            MsLevel = reader.GetIntAttribute(ATTR.ms_level);

        }

        private enum ATTR
        {
            fragment_type,
            measured_ion_name,
            decoy_mass_shift,
            mass_index,
            fragment_ordinal,
            product_charge,
            orphaned_crosslink_ion,
            ms_level
        }

        public void WriteXml(XmlWriter writer)
        {
            var transition = ComplexFragmentIon.PrimaryTransition;
            writer.WriteAttribute(ATTR.ms_level, MsLevel);
            writer.WriteAttribute(ATTR.fragment_type, transition.IonType);
            if (transition.IsCustom())
            {
                if (!(transition.CustomIon is SettingsCustomIon))
                {
                    transition.CustomIon.WriteXml(writer, transition.Adduct);
                }
                else
                {
                    writer.WriteAttributeString(ATTR.measured_ion_name, transition.CustomIon.Name);
                }
            }
            writer.WriteAttributeNullable(ATTR.decoy_mass_shift, transition.DecoyMassShift);
            // NOTE: MassIndex is the peak index in the isotopic distribution of the precursor.
            //       0 for monoisotopic peaks and for non "precursor" ion types.
            if (transition.MassIndex != 0)
                writer.WriteAttribute(ATTR.mass_index, transition.MassIndex);
            if (!transition.IsCustom())
            {
                writer.WriteAttribute(ATTR.fragment_ordinal, transition.Ordinal);
            }
            writer.WriteAttribute(ATTR.product_charge, transition.Charge);
            if (ComplexFragmentIon.IsOrphan)
            {
                writer.WriteAttribute(ATTR.orphaned_crosslink_ion, true);
            }
            DocumentWriter.WriteTransitionLosses(writer, ComplexFragmentIon.Losses);
            DocumentWriter.WriteLinkedIons(writer, ComplexFragmentIon.NeutralFragmentIon);
        }
#endif
        public static SrmDocument AddIntermediatePrecursors(SrmDocument document,
            IdentityPath transitionGroupIdentityPath, ICollection<Transition> transitions)
        {
            var peptideDocNode = (PeptideDocNode) document.FindNode(transitionGroupIdentityPath.Parent);
            var transitionGroupDocNode = (TransitionGroupDocNode) peptideDocNode.FindNode(transitionGroupIdentityPath.Child);
            int newMsLevel;
            if (transitionGroupDocNode.IntermediatePrecursors.Any())
            {
                newMsLevel = transitionGroupDocNode.IntermediatePrecursors.Max(ip => ip.MsLevel) + 1;
            }
            else
            {
                newMsLevel = 2;
            }
            var intermediatePrecursors = new List<IntermediatePrecursor>();
            foreach (var transition in transitions)
            {
                var transitionDocNode = (TransitionDocNode) transitionGroupDocNode.FindNode(transition);
                var intermediatePrecursor = new IntermediatePrecursor(newMsLevel, transitionDocNode.ComplexFragmentIon);
                intermediatePrecursors.Add(intermediatePrecursor);
            }

            var transitionHashSet = transitions.ToHashSet(new IdentityEqualityComparer<Transition>());
            transitionGroupDocNode = transitionGroupDocNode.ChangeIntermediatePrecursors(intermediatePrecursors);
            transitionGroupDocNode = (TransitionGroupDocNode) transitionGroupDocNode.ChangeChildren(
                transitionGroupDocNode.Transitions
                    .Where(t => !transitionHashSet.Contains(t.Transition)).Cast<DocNode>().ToList());
            return (SrmDocument) document.ReplaceChild(transitionGroupIdentityPath.Parent, transitionGroupDocNode);
        }
    }

    public struct IntermediatePrecursorMz
    {
        public IntermediatePrecursorMz(int msLevel, SignedMz mz)
        {
            MsLevel = msLevel;
            Mz = mz;
        }

        public int MsLevel { get; }
        public SignedMz Mz { get; }

        public static long GetHash(IEnumerable<IntermediatePrecursorMz> mzs)
        {
            var doubleArray = mzs.OrderBy(mz => mz.Mz).Select(mz => mz.Mz.RawValue).ToArray();
            return AdlerChecksum.MakeForBuff(PrimitiveArrays.ToBytes(doubleArray));
        }
    }
}
