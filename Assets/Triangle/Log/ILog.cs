// -----------------------------------------------------------------------
// <copyright file="ILog.cs" company="">
// Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

namespace UnityEditor.Experimental.U2D.Animation.TriangleNet.Log
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public enum LogLevel
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }

    /// <summary>
    /// A basic log interface.
    /// </summary>
    public interface ILog<T> where T : ILogItem
    {
        void Add(T item);
        void Clear();

        void Info(string message);
        void Error(string message, string info);
        void Warning(string message, string info);

        IList<T> Data { get; }

        LogLevel Level { get; }
    }
}