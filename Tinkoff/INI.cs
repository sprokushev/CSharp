using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace PSVClassLibrary
{
    public class INI
    {

        const string pattern = @"
(?<Section>                                                              (?# Start of a non ini file section)
  (?<SectionName>[\w ]+)\s*                                              (?# Capture section name)
     {                                                                   (?# Match but don't capture beginning of section)
        (?<SectionBody>                                                  (?# Capture section body. Section body can be empty)
         (?<SectionLine>\s*                                              (?# Capture zero or more line(s) in the section body)
         (?:                                                             (?# A line can be either a key/value pair, a comment or a function call)
            (?<KeyValuePair>(?<Key>[\w\[\]]+)\s*=\s*(?<Value>[\w-]*))    (?# Capture key/value pair. Key and value are sub-captured separately)
            |
            (?<Comment>/\*.+?\*/)                                        (?# Capture comment)
            |
            (?<FunctionCall>[\w]+\(\))                                   (?# Capture function call. A function can't have parameters though)
         )\s*                                                            (?# Match but don't capture white characters)
         )*                                                              (?# Zero or more line(s), previously mentionned in comments)
        )
     }                                                                   (?# Match but don't capture beginning of section)
)
|
(?<Section>                                                              (?# Start of an ini file section)
  \[(?<SectionName>[\w ]+)\]                                             (?# Capture section name)
  (?<SectionBody>                                                        (?# Capture section body. Section body can be empty)
     (?<SectionLine>                                                     (?# Capture zero or more line(s) in the section body. Only key/value pair allowed.)
        \s*(?<KeyValuePair>(?<Key>[\w\[\]]+)\s*=\s*(?<Value>[\w-]+))\s*  (?# Capture key/value pair. Key and value are sub-captured separately)
     )*                                                                  (?# Zero or more line(s), previously mentionned in comments)
  )
)
";
        // список ключей
        // Value=Params["Section"]["Key"]
        public Dictionary<string, Dictionary<string, string>> Params { set; get; }

        public INI(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    string data = System.IO.File.ReadAllText(file);


                    Params          = (from Match m in Regex.Matches(data,
                                            pattern,
                                            RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline)
                      select new
                       {
                          Section = m.Groups["Section"].Value,

                            kvps = (from cpKey in m.Groups["Key"].Captures.Cast().Select((a, i) => new { a.Value, i })
                                     join cpValue in m.Groups["Value"].Captures.Cast().Select((b, i) => new { b.Value, i }) on cpKey.i equals cpValue.i
                                     select new KeyValuePair(cpKey.Value, cpValue.Value)).OrderBy(_ => _.Key)
                                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)

                       }).ToDictionary(itm => itm.Section, itm => itm.kvps);
                }
    
            }
            catch (Exception)
            {
                Params = null;
            }
        }

    }
}

/*


        const string pattern = @"
^                           # Beginning of the line
((?:\[)                     # Section Start
     (?<Section>[^\]]*)     # Actual Section text into Section Group
 (?:\])                     # Section End then EOL/EOB
 (?:[\r\n]{0,}|\Z))         # Match but don't capture the CRLF or EOB
 (                          # Begin capture groups (Key Value Pairs)
  (?!\[)                    # Stop capture groups if a [ is found; new section
  (?<Key>[^=]*?)            # Any text before the =, matched few as possible
  (?:=)                     # Get the = now
  (?<Value>[^\r\n]*)        # Get everything that is not an Line Changes
  (?:[\r\n]{0,4})           # MBDC \r\n
  )+                        # End Capture groups";

        // список ключей
        // Value=Params["Section"]["Key"]
        public Dictionary<string, Dictionary<string, string>> Params { set; get; }

    }
}



                        Params = (from Match m in Regex.Matches(File.ReadAllText(file), pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline)
                              select new
                              {
                                  Section = m.Groups["Section"].Value,

                                  kvps = (from cpKey in m.Groups["Key"].Captures.Cast<Capture>().Select((a, i) => new { a.Value, i })
                                          join cpValue in m.Groups["Value"].Captures.Cast<Capture>().Select((b, i) => new { b.Value, i }) on cpKey.i equals cpValue.i
                                          select new KeyValuePair<string, string>(cpKey.Value, cpValue.Value)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)

                              }).ToDictionary(itm => itm.Section, itm => itm.kvps);

*/
