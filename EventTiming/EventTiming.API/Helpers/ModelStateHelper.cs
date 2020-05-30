using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;

namespace EventTiming.API
{
    public class ModelStateHelper
    {
        public static string GetErrors(ModelStateDictionary state)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var errorItem in state.Values)
            {

                foreach (var error in errorItem.Errors)
                {
                    if (!string.IsNullOrEmpty(error.ErrorMessage))
                        sb.AppendLine(error.ErrorMessage);
                    else
                        sb.AppendLine(error.Exception.Message);
                }


            }
            return sb.ToString();
        }
    }
}
