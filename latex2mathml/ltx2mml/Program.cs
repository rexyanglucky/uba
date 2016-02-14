/*  
    This file is part of Latex2MathML.

    Latex2MathML is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Latex2MathML is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Latex2MathML.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Text;
using Latex2MathML;

namespace ltx2mml
{
    class Program
    {
        static void Main(string[] args)
        {

#if MONO
		LatexToMathMLConverter.GhostScriptBinaryPath = "gs";	
#else
            LatexToMathMLConverter.GhostScriptBinaryPath = @"C:\\Program Files (x86)\\gs\\gs8.64\\bin\\gswin32c.exe";
#endif
            var lmm = new LatexToMathMLConverter(
                "E:\\source.txt",
                Encoding.GetEncoding(1251),
               "E:\\source.xml");
            lmm.ValidateResult = true;
            lmm.Convert();



        }
    }
}
