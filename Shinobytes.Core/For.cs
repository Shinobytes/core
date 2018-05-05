/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;

namespace Shinobytes.Core
{
    public static class For
    {
        /// <summary>
        /// Runs a For loop incrementing the index by 1 every iteration
        /// </summary>
        /// <example>
        ///     For.Incremental(0, 100, (index) => { /* 0. 1. 2.. do something ..98. 99. 100 */ });
        /// </example>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="action"></param>
        public static void Incremental(int start, int end, Action<int> action)
        {
            for (var i = start; i < end; i++) if (action != null) action(i);
        }

        /// <summary>
        /// Runs a For loop decrementing the index by 1 every iteration.        
        /// </summary>
        /// <example>
        ///     For.Decremental(100, 0, (index) => { /* 100. 99. 98.. do something .. 2. 1. 0 */ });
        /// </example>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="action"></param>
        public static void Decremental(int start, int end, Action<int> action)
        {
            for (var i = start; i > end; i--) if (action != null) action(i);
        }

        /// <summary>
        /// Runs a For loop with the supplied conditions
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="condition"></param>
        /// <param name="endOfFor"></param>
        /// <param name="action"></param>
        public static void Do(int start, int end, Func<int, int, bool> condition, Action<int> endOfFor, Action<int> action)
        {
            for (var i = start; condition(i, end); endOfFor(i))
            {
                if (action != null) action(i);
            }
        }
    }
}