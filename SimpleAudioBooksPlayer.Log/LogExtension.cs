using System;
using System.Collections.Generic;
using System.Reflection;
using NLog;
using SimpleAudioBooksPlayer.Log.Models;

namespace SimpleAudioBooksPlayer.Log
{
    public static class LogExtension
    {
        private static readonly Dictionary<Assembly, Logger> AllLoggers = new Dictionary<Assembly, Logger>();

        public static void SetupLogger(Assembly assembly, LoggerMembers member)
        {
            if (!AllLoggers.ContainsKey(assembly))
                AllLoggers.Add(assembly, LoggerService.GetLogger(member));
        }

        public static void LogByObject(this object obj, string message) => obj.GetType().LogByType(message);
        public static void LogByObject(this object obj, Exception exception) => obj.GetType().LogByType(exception);
        public static void LogByObject(this object obj, Exception exception, string extraMessage) => obj.GetType().LogByType(exception, extraMessage);

        public static void LogByType(this Type type, string message)
        {
            var logger = AllLoggers[GetAssembly(type)];
            logger.Info(message);
        }

        public static void LogByType(this Type type, Exception exception)
        {
            var logger = AllLoggers[GetAssembly(type)];
            logger.Info(exception);
        }

        public static void LogByType(this Type type, Exception exception, string extraMessage)
        {
            var logger = AllLoggers[GetAssembly(type)];
            logger.Info(exception, extraMessage);
        }

        private static Assembly GetAssembly(Type type)
        {
            return type.Assembly;
        }
    }
}