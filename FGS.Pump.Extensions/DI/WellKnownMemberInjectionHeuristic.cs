using System.Collections.Generic;
using System.Reflection;

using Ninject.Components;
using Ninject.Selection.Heuristics;

namespace FGS.Pump.Extensions.DI
{
    public class WellKnownMemberInjectionHeuristic : NinjectComponent, IInjectionHeuristic
    {
        private readonly List<MemberInfo> _wellKnownMembers = new List<MemberInfo>();

        public void Add(MemberInfo member)
        {
            if (!_wellKnownMembers.Contains(member))
                _wellKnownMembers.Add(member);
        }

        public void Remove(MemberInfo member)
        {
            if (_wellKnownMembers.Contains(member))
                _wellKnownMembers.Remove(member);
        }

        #region Implementation of IInjectionHeuristic

        public bool ShouldInject(MemberInfo member)
        {
            return _wellKnownMembers.Contains(member);
        }

        #endregion
    }
}
