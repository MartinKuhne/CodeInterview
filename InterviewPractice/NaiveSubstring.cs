using System;

namespace InterviewPractice
{
    public static class NaiveSubstringHelper
    {
        /// <summary>
        /// Determines if the string represented by source contains the substring represented by pattern
        /// </summary>
        /// <returns>
        /// true if pattern is contained in source, false otherwise
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool ContainsBruteForce(char[] source, char[] pattern)
        {
            var sourceLength = source.Length;
            var patternLength = pattern.Length;

            if (patternLength > sourceLength || patternLength == 0)
            {
                return false;
            }

            // 01234567
            // ABBAANNA
            //     ANNA
            // the last possible location the substring ANNA could be found is 4
            var lastIndex = sourceLength - patternLength;
            for (var index = 0; index <= lastIndex; index++)
            {
                var innerIndex = index;
                while (innerIndex < index + patternLength)
                {
                    if (source[innerIndex] != pattern[innerIndex - index])
                    {
                        break;
                    }
                    innerIndex++;
                }
                if (innerIndex - index == patternLength)
                {
                    return true;
                }
            }

            return false;
        }
    }
}