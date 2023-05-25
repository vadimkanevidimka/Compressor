using System.Data;
using System.IO;

namespace Compressor
{
    public class Program
    {
        private const byte ENCODING_TABLE_SIZE = 255;

        public static void Main(string[] args)
        {
            try
            {//указываем инструкцию с помощью аргументов командной строки
                ChooseTheOption();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("Неверный формат ввода аргументов ");
                Console.WriteLine("Читайте Readme.txt");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static string EnterComressFile(ConsoleKey consoleKey)
        {
            string? path = ""; 
            switch (consoleKey)
            {
                case ConsoleKey.C:
                    Console.WriteLine("\nВведите путь к файлу который должен быть сжат");
                    path = Console.ReadLine();
                    break;
                case ConsoleKey.D:
                    Console.WriteLine("\nВведите путь к файлу который должен быть разархивирован");
                    path = Console.ReadLine() + '|';
                    Console.WriteLine("\nВведите путь к файлу таблицы для сжатого файла");
                    path += Console.ReadLine();
                break;
            }
            Console.WriteLine("===============================================================");
            try
            {
                if (path.Length == 0)
                {
                    throw new DataException();
                }
                else
                {
                    return path;
                }
            }
            catch (DataException ex)
            {
                Console.WriteLine($"{path} не верный или файл не существует");
                return ex.Message;
            }
        }

        private static void ChooseTheOption()
        {
            while (true)
            {
                Console.WriteLine("Выберите действие которое хотите сделать с файлом. Нажмите нужную клавишу\nC: Сжать файл\nD: Разархивировать файл\n");
                ConsoleKeyInfo a = Console.ReadKey();
                switch (a.Key)
                {
                    case ConsoleKey.C:
                        compress(EnterComressFile(a.Key));
                        Console.WriteLine("Файл был сжат");
                        break;
                    case ConsoleKey.D:
                        string[] paths = EnterComressFile(a.Key).Split('|', StringSplitOptions.None);
                        extract(paths[0], paths[1]);
                        Console.WriteLine("Файл был разархивирован");
                        break;
                    default: break;
                }
            }
        }


        public static void compress(string stringPath)
        {
            List<string> stringList = new List<string>();
            string s = "";
            string compressedFilePath, tableFilePath;

            try
            {
                stringList = File.ReadAllLines(stringPath).ToList();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Неверный путь, или такого файла не существует!");
                return;
            }

            foreach (string item in stringList)
            {
                s += item;
                s += '\n';
            }

            HuffmanOperator _operator = new HuffmanOperator(new HuffmanTree(s));

            compressedFilePath = stringPath.Remove(stringPath.IndexOf('.'), stringPath.Length - stringPath.IndexOf('.')) + ".cpr";
            FileStream compressedFile = File.Create(compressedFilePath);
            try
            {
                FileOutputHelper foh = new FileOutputHelper(compressedFile);
                foh.writeBytes(_operator.getBytedMsg());
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            //create file with encoding table:

            tableFilePath = stringPath.Remove(stringPath.IndexOf('.'), stringPath.Length - stringPath.IndexOf('.')) + ".table.txt";
            FileStream tableFile = File.Create(tableFilePath);

            try
            {
                FileOutputHelper foh = new FileOutputHelper(tableFile);
                foh.writeString(_operator.getEncodingTable());
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("===============================================================");
            Console.WriteLine("Путь к сжатому файлу: " + compressedFilePath);
            Console.WriteLine("Путь к кодировочной таблице " + tableFilePath);
            Console.WriteLine("Без таблицы файл будет невозможно извлечь!");

            double idealRatio = Math.Round(_operator.getCompressionRatio() * 100) / (double)100;//идеализированный коэффициент
            double realRatio = Math.Round((double)File.Open(stringPath, FileMode.Open).Length
                    / ((double)compressedFile.Length + (double)tableFile.Length) * 100) / (double)100;//настоящий коэффициент

            Console.WriteLine("Идеализированный коэффициент сжатия равен " + idealRatio);
            Console.WriteLine("Коэффициент сжатия с учетом кодировочной таблицы " + realRatio);
            compressedFile.Close();
            tableFile.Close();
            Console.WriteLine("===============================================================");
        }

        public static void extract(String filePath, String tablePath)
        {
            HuffmanOperator _operator = new HuffmanOperator();
            FileStream compressedFile = File.Open(filePath, FileMode.Open);
            FileStream tableFile = File.Open(tablePath, FileMode.Open);
            FileStream extractedFile = File.Create(filePath + ".xtr");
            string compressed = "";
            string[] encodingArray = new String[ENCODING_TABLE_SIZE];
            //read compressed file
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!check here:
            FileInputHelper fi = new FileInputHelper(compressedFile);
            byte b;
            fi.ReadByte();
            while (true)
            {
                if (compressedFile.Position != compressedFile.Length)
                {
                    b = fi.ReadByte();//method returns eofexception
                    compressed += Convert.ToString(b, 2).PadLeft(8, '0');
                }
                else break;
            }

            //--------------------

            //read encoding table:
            try
            {
                FileInputHelper fih = new FileInputHelper(new StreamReader(tableFile));
                fih.ReadLine();//skip first empty string
                encodingArray[(byte)'\n'] = fih.ReadLine();//read code for '\n'
                while (true)
                {
                    string? s = fih.ReadLine();
                    if (s == null) break;
                    encodingArray[(byte)s[0]] = s.Substring(0, s.Length);
                }
            } catch (EndOfStreamException ignore) { }
            //extract:
            try
            {
                using (FileOutputHelper fo = new FileOutputHelper(extractedFile))
                {
                    fo.writeString(_operator.extract(compressed, encodingArray));
                } 
            }
            catch (IOException ex)
            {

            }

            Console.WriteLine("Путь к распакованному файлу " + extractedFile.Name);
        }
    }
}

