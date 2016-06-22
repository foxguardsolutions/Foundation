using System.Linq;
using System.Reflection;

using Ninject;
using Ninject.Selection.Heuristics;

namespace FGS.Pump.Extensions.DI
{
    public static class KernelExtensions
    {
        public static void RegisterWellKnownMemberForInjection(this IKernel kernel, MemberInfo member)
        {
            var heuristic = GetOrAddWellKnownMemberInjectionHeuristic(kernel);
            heuristic.Add(member);
        }

        private static WellKnownMemberInjectionHeuristic GetOrAddWellKnownMemberInjectionHeuristic(this IKernel kernel)
        {
            var result = GetWellKnownMemberInjectionHeuristic(kernel);

            if (result == null)
            {
                kernel.Components.Add<IInjectionHeuristic, WellKnownMemberInjectionHeuristic>();
                result = GetWellKnownMemberInjectionHeuristic(kernel);
            }

            return result;
        }

        private static WellKnownMemberInjectionHeuristic GetWellKnownMemberInjectionHeuristic(IKernel kernel)
        {
            return Enumerable.OfType<WellKnownMemberInjectionHeuristic>(kernel.Components.GetAll<IInjectionHeuristic>()).SingleOrDefault();
        }
    }
}