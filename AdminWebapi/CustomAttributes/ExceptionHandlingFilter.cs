using System;
using System.IO;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using BusinessLogic.Exceptions;

namespace AdminWebapi
{
    public class ExceptionHandlingFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var excpParam = JsonConvert.SerializeObject(context.ActionContext.ActionArguments.Values);
            var uri = context.ActionContext.Request.RequestUri.OriginalString;
            var excpMsg = context.Exception.Message;
            var excpStackTrace = context.Exception.StackTrace;
            var excpInnerMessage = ((context.Exception.InnerException != null && context.Exception.InnerException.Message != null) ? (context.Exception.InnerException.Message.ToLower().Contains("inner exception") ? context.Exception.InnerException.InnerException.Message : context.Exception.InnerException.Message) : "NULL");

            if (context.Exception is BusinessException)
            {
                string directory = Path.GetPathRoot(Environment.CurrentDirectory) + "\\AdminApiExceptionLog";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                string filePath = directory + "\\errors_" + DateTime.Now.Date.ToString("MM-dd-yyyy") + ".txt";
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Message :" + excpMsg  + Environment.NewLine + Environment.NewLine+ "URI :" + uri + Environment.NewLine + Environment.NewLine + "Request parameters :" + excpParam + Environment.NewLine + Environment.NewLine + "StackTrace :" + excpStackTrace + Environment.NewLine + Environment.NewLine +
                       "///------------------Inner Exception------------------///" + Environment.NewLine + excpInnerMessage + Environment.NewLine +
                       "Date :" + DateTime.Now.ToString() + Environment.NewLine + Environment.NewLine + "-------------------------------------------------------||||||||||||---End Current Exception---||||||||||||||||-------------------------------------------------------" + Environment.NewLine);
                }

            }
            //Log Critical errors
            string directoryOther = Path.GetPathRoot(Environment.CurrentDirectory) + "\\AdminApiExceptionLog";
            if (!Directory.Exists(directoryOther))
            {
                Directory.CreateDirectory(directoryOther);
            }
            string filePathOther = directoryOther + "\\errors_" + DateTime.Now.Date.ToString("MM-dd-yyyy") + ".txt";
            using (StreamWriter writer = new StreamWriter(filePathOther, true))
            {
                writer.WriteLine("Message :" + excpMsg + Environment.NewLine + Environment.NewLine + "URI :" + uri + Environment.NewLine + Environment.NewLine + "Request parameters :" + excpParam + Environment.NewLine + Environment.NewLine + "StackTrace :" + excpStackTrace + Environment.NewLine + Environment.NewLine +
                   "///------------------Inner Exception------------------///" + Environment.NewLine + excpInnerMessage + "" + Environment.NewLine +
                   "Date :" + DateTime.Now.ToString() + Environment.NewLine + Environment.NewLine + "-------------------------------------------------------||||||||||||---End Current Exception---||||||||||||||||-------------------------------------------------------" + Environment.NewLine);
            }

        }
    }

}