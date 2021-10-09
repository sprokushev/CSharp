using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSVCopy
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Count()==3)
            {

                string FromFile = args[0];
                string ToFile = args[1];
                int SizePartInByte = int.Parse(args[2]);
                

                if ( File.Exists(FromFile) )
                {
                    var FromFileInfo = new FileInfo(FromFile);
                    long CountParts = FromFileInfo.Length / SizePartInByte;
                    if ( FromFileInfo.Length > SizePartInByte * CountParts ) CountParts++;


                    string ToFilePart;
                    int readCount = 0;
                    int Offset = 0;
                    byte[] buffer = new byte[SizePartInByte];

                    using (Stream from = new FileStream(FromFile, FileMode.Open)) 
                    for (int i = 0; i < CountParts; i++)
                    {
                            Offset = i * SizePartInByte;

                            if ((readCount = from.Read(buffer, 0, SizePartInByte)) != 0)
                            {

                                ToFilePart = Path.Combine(Path.GetDirectoryName(ToFile), Path.GetFileNameWithoutExtension(ToFile) + i.ToString() + Path.GetExtension(ToFile));

                                using (Stream to = new FileStream(ToFilePart, FileMode.OpenOrCreate))
                                {
                                    to.Write(buffer, 0, readCount);
                                    Console.WriteLine("copying");
                                }

                            }
                    }

                }


                

            }


        }
    }
}
