using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mogre.Builder
{
    class Misc
    {
        /// <summary>
        /// Make paths bullet-proof for usage with Mercurial (HG). 
        /// This is important when a directory contains a space symbol.
        /// </summary>
        /// <param name="path">Input path</param>
        /// <returns>bullet-proof path</returns>
        public static String HgPathSecurity(String path)
        {
            // check for spaces in source path  (can happen for a clone on local hard disk)
            if (Regex.IsMatch(path, " "))
            {
                // remove backslash from end 
                // because it causes a conflict with the following quote symbol (Mercurial assumes that \" is an escaped quote)
                path = path.TrimEnd('\\');

                // add quote symbols
                path = "\"" + path + "\"";

                //Console.WriteLine("MODIFIED HG PATH:  " + path);
            }
            return path;
        }
    }
}
