using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compressor
{
    public class FileInputHelper
    {
        private FileStream fileInputStream;
        private StreamReader _streamreader;
        public FileInputHelper(FileStream Fstream)
        {
            fileInputStream = Fstream;
	    }

        public FileInputHelper(StreamReader streamReader)
        {
            _streamreader = streamReader;
        }

        public byte ReadByte()
        {
            if (fileInputStream.CanRead)
            {
    	        int cur = fileInputStream.ReadByte();
                return (byte)cur;
            }
            return 0;
        }
    
        public string? ReadLine()
        {
            if (!_streamreader.EndOfStream)
            {
                string? a = _streamreader.ReadLine();
                return a;
            }
            return null;
        }

        public void close()
        {
            fileInputStream.Close();
        }
    }
}
