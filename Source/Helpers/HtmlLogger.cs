using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Mogre.Builder
{
class HtmlLogger
{
    // IMPROVEMENT IDEAS for HTML logger:
    // * add content of used config file to the end
    // * add MogreBuilder revision
    //    * Get plain revision number:  hg identify --num
    //    * Get more details:           hg parent
    // * add useful HTML links (forum, bugtracker, etc.)
    // * add checkbox to display/hide the time stamps (needs JavaScript)
    // * copy log file to binary output directory
    // * add logging support to class FileCopyTask




    private StreamWriter logFile;
    private Int64 startTime = DateTime.Now.Ticks;
    private String consoleCommand;
    private String lineBuffer = "";
    private Char[] trimChar_spaceNL = { ' ', '\r', '\n' };
    private Char[] trimChar_NL = { '\r', '\n' };

    /// <summary>
    /// Create a logfile in HTML format.
    /// </summary>
    /// <param name="file">file name including path</param>
    /// <param name="startTime">Start time in ticks</param>
    /// <param name="consoleCommand">Command and parameters of the MogreBuilder start from console</param>
    public HtmlLogger(String file, Int64 startTime, String consoleCommand)
    {
        // inputManager.BuildOutputDirectory   PATH
        logFile = new StreamWriter(file, false, Encoding.UTF8);
        this.consoleCommand = consoleCommand;
        BeginHTML();
    }



    public void CloseLogfile()
    {
        EndHTML();
        //logFile.Flush();
        logFile.Close();
    }



    /// <summary>
    /// Create the first lines of the HTML document.
    /// </summary>
    private void BeginHTML()
    {
        String head = @"
        <!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">
        <HTML>
          <HEAD>
            <META http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
            <TITLE>MogreBuilder Logfile ##BUILD_DATE##</TITLE>
            <STYLE type=""text/css"">
              body { background-color: #000; font-family: monospace; color: #FFF;}
              .TimeStamp   { color: #888; vertical-align: top; padding-right: 15px }

              // colours of .NET type ConsoleColor
              .Black       { color: #000; }
              .DarkBlue    { color: #008; }
              .DarkGreen   { color: #080; }
              .DarkCyan    { color: #088; }
              .DarkRed     { color: #800; }
              .DarkMagenta { color: #808; }
              .DarkYellow  { color: #880; }
              .Gray        { color: #CCC; }
              .DarkGray    { color: #888; }
              .Blue        { color: #00F; }
              .Green       { color: #0F0; }
              .Cyan        { color: #0FF; }
              .Red         { color: #F00; }
              .Magenta     { color: #F0F; }
              .Yellow      { color: #FF0; }
              .White       { color: #FFF; }              
              
              
            </STYLE>
            
          </HEAD>
          <BODY>
            
            <H1>MogreBuilder logfile</H1>
            <p>##BUILD_DATE##</p>
            <p>Command: ##COMMAND##</p>

            <TABLE>
        ";
        

        // set placeholders
        head = Regex.Replace(head, "##BUILD_DATE##", String.Format("{0:00}-{1:00}-{2:00} {3:00}:{4:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute));
        head = Regex.Replace(head, "##COMMAND##", consoleCommand);

        logFile.WriteLine(head);
    } // BeginHTML()



    /// <summary>
    /// Create the last lines of the HTML document.
    /// </summary>
    private void EndHTML()
    {
        String hEnd = @"
            </TABLE>
          </BODY>
        </HTML>
        ";
        
        logFile.WriteLine(hEnd);
    }



    /// <summary>
    /// Write a line to the log file
    /// </summary>
    /// <param name="text">Text to write</param>
    /// <param name="colour">Colour of type ConsoleColor</param>
    public void WriteLine(String text, ConsoleColor colour)
    {
        // flush text buffer if needed
        if (lineBuffer != "")
        {
            String buffOut = lineBuffer;
            lineBuffer = "";
            WriteLine(buffOut, ConsoleColor.Gray);
        }

        String output = @"
              <TR>
                <TD class=""TimeStamp"">##TIME##</TD>
                <TD><span class=""##COLOUR##"">##TEXT##</span></TD>
              </TR>
        ";

        // remove some whitespaces
        output.TrimStart(trimChar_NL);
        output.TrimEnd(trimChar_spaceNL);

        // add line break tags (for multiple lines)
        text = Regex.Replace(text, @"(\r?\n)", "$1<br/>");

        // add space tags for multiple spaces
        text = Regex.Replace(text, @"  ", " &nbsp;");

        // insert content
        output = Regex.Replace(output, "##COLOUR##", colour.ToString());
        output = Regex.Replace(output, "##TEXT##", text);
        
        TimeSpan duration = TimeSpan.FromTicks(DateTime.Now.Ticks - startTime);
        output = Regex.Replace(output, "##TIME##", String.Format("{0:0}:{1:00}:{2:00}", duration.Hours, duration.Minutes, duration.Seconds));

        output = Regex.Replace(output, "##BUFFERED_TEXT##", lineBuffer);
        lineBuffer = "";

        logFile.WriteLine(output);

    } // WriteLine()



//    /// <summary>
//    /// Write a line to the log file
//    /// </summary>
//    /// <param name="text">Text to write</param>
//    /// <param name="colour">Colour of type ConsoleColor</param>
//    public void WriteLine(String text, ConsoleColor colour)
//    {
//        String output = @"
//              <TR>
//                <TD class=""DarkGray"">##TIME##</TD>
//                <TD>##BUFFERED_TEXT##<span class=""##COLOUR##"">##TEXT##</span></TD>
//              </TR>
//        ";

//        // add line break tags (for multiple lines)
//        text = Regex.Replace(text, @"(\r?\n)", "$1<br/>");

//        // add space tags for multiple spaces
//        text = Regex.Replace(text, @"  ", " &nbsp;");

//        // insert content
//        output = Regex.Replace(output, "##COLOUR##", colour.ToString());
//        output = Regex.Replace(output, "##TEXT##", text);
        
//        TimeSpan duration = TimeSpan.FromTicks(DateTime.Now.Ticks - startTime);
//        output = Regex.Replace(output, "##TIME##", String.Format("{0:0}:{1:00}:{2:00}", duration.Hours, duration.Minutes, duration.Seconds));

//        output = Regex.Replace(output, "##BUFFERED_TEXT##", lineBuffer);
//        lineBuffer = "";

//        logFile.WriteLine(output);

//    } // WriteLine()



    /// <summary>
    /// Write text without linebreak to the log file. 
    /// The text will be buffered until the next call of "WriteLine()".
    /// </summary>
    public void Write(String text)
    {
        lineBuffer += text;
    } // Write()



    /// <summary>
    /// Add a blank line for better overview. 
    /// If the line buffer contains text, it will be flushed before the blank line.
    /// </summary>
    public void WriteBlankLine()
    {
        // flush text buffer if needed
        if (lineBuffer != "")
            WriteLine("", ConsoleColor.Gray);

        String output = @"
              <TR>
                <TD>&nbsp;</TD>
                <TD>&nbsp;</TD>
              </TR>
        ";
        logFile.WriteLine(output);

    } // WriteBlankLine()





} // class HtmlLogger

} // namespace