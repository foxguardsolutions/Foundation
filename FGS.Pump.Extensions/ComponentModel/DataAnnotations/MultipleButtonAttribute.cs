using System;
using System.Reflection;
using System.Web.Mvc;

namespace FGS.Pump.Extensions.ComponentModel.DataAnnotations
{
    /// <remarks>Taken and modified from: http://stackoverflow.com/a/7111222/1595510 </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MultipleButtonAttribute : ActionNameSelectorAttribute
    {
        public string ButtonName { get; set; }

        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            var isValidName = false;
            var keyValue = $"{ButtonName}";
            var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);

            if (value != null)
            {
                controllerContext.Controller.ControllerContext.RouteData.Values["action"] = ButtonName;
                isValidName = true;
            }

            return isValidName;
        }
    }
}