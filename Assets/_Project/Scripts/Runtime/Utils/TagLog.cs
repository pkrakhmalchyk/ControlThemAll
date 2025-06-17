using System;
using UnityEngine;

namespace ControllThemAll.Runtime.Utils
{
    public class TagLog
    {
        private readonly string tag;

        public TagLog(string tag)
        {
            this.tag = "[" + tag + "]";
        }

        [HideInCallstack]
        public void LogError(string message)
        {
            Debug.unityLogger.Log(LogType.Error, tag, message);
        }

        [HideInCallstack]
        public void LogError(Exception e)
        {
            Debug.unityLogger.Log(LogType.Exception, tag, e.ToString());
        }

        [HideInCallstack]
        public void LogWarning(string message)
        {
            Debug.unityLogger.Log(LogType.Warning, tag, message);
        }

        [HideInCallstack]
        public void LogDebug(string message)
        {
            Debug.unityLogger.Log(LogType.Log, tag, message);
        }
    }
}