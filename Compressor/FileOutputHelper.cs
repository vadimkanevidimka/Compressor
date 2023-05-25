using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compressor
{
    public class FileOutputHelper : IDisposable
    {
        private FileStream fileOutputStream;

        public FileOutputHelper(FileStream fstream)
        {
            fileOutputStream = fstream;
        }

        public void writeByte(byte msg)
        {
            ReadOnlySpan<byte> msgbyte = new ReadOnlySpan<byte>();
            fileOutputStream.Write(msgbyte);
        }

        public void writeBytes(byte[] msg)
        {
            
            fileOutputStream.Write(msg);
        }

        public void writeString(string msg) {
            try 
            {
                byte[] bytes = Encoding.UTF8.GetBytes(msg);
                fileOutputStream.Write(bytes, 0, bytes.Length);
    	    } catch (FileNotFoundException ex) {
                Console.WriteLine("Неверный путь, или такого файла не существует!");
    	    }
        }

        public void Close()
        {
            fileOutputStream.Close();
        }

        public void finalize()
        {
            fileOutputStream.Close();
        }

        public void Dispose()
        {
            finalize();
        }
    }
}
