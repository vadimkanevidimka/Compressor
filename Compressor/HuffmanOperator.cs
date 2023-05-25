using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compressor
{
    public class HuffmanOperator
    {
        private const byte ENCODING_TABLE_SIZE = 255;//длина таблицы
        private HuffmanTree mainHuffmanTree;//дерево Хаффмана (используется только для сжатия)
        private string mystring;//исходное сообщение
        private int[] freqArray;//частотаная таблица
        private string[] encodingArray;//кодировочная таблица
        private double ratio;//коэффициент сжатия 
        public string compressedstring { get => getCompressedstring(); }

        public HuffmanOperator(HuffmanTree MainHuffmanTree)
        {//for compress
            this.mainHuffmanTree = MainHuffmanTree;

            mystring = mainHuffmanTree.getOriginalstring();

            encodingArray = mainHuffmanTree.getEncodingArray();

            freqArray = mainHuffmanTree.getFrequenceArray();
        }

        public HuffmanOperator() { }//Для извлечения;

        //---------------------------------------Сжатие-----------------------------------------------------------
        private string getCompressedstring()
        {
            string compressed = "";
            string intermidiate = "";//промежуточная строка(без добавочных нулей)
            Console.WriteLine("=============================Сжатие=======================");
            displayEncodingArray();
            for (int i = 0; i < mystring.Length; i++)
            {
                intermidiate += encodingArray[mystring.ElementAt(i)];
            }
            //Мы не можем писать бит в файл. Поэтому нужно сделать длину сообщения кратной 8=>
            //нужно добавить нули в конец(можно 1, нет разницы)
            byte counter = 0;//количество добавленных в конец нулей (байта в полне хватит: 0<=counter<8<255)
            for (int Length = intermidiate.Length, delta = 8 - Length % 8;
                    counter < delta; counter++)
            {//delta - количество добавленных нулей
                intermidiate += "0";
            }

            //склеить кол-во добавочных нулей в бинарном предаствлении и промежуточную строку 
            compressed = string.Format(Convert.ToString(counter & 0xff)).Replace(" ", "0") + intermidiate;

            //идеализированный коэффициент
            setCompressionRatio();
            Console.WriteLine("===============================================================");
            return compressed;
        }

        private void setCompressionRatio()
        {//посчитать идеализированный коэффициент 
            double sumA = 0, sumB = 0;//A-the original sum
            for (int i = 0; i < ENCODING_TABLE_SIZE; i++)
            {
                if (freqArray[i] != 0)

                {
                    sumA += 8 * freqArray[i];
                    sumB += encodingArray[i].Length * freqArray[i];
                }
            }
            ratio = sumA / sumB;
        }

        public byte[] getBytedMsg()
        {//final compression
            string compressedstring = getCompressedstring();
            byte[] compressedBytes = new byte[compressedstring.Length / 8];
            for (int i = 1; i < compressedBytes.Length; i++)
            {
                compressedBytes[i] = Convert.ToByte(compressedstring.Substring(i * 8, 8), 2);
            }
            return compressedBytes;
        }
        //---------------------------------------Завершение сжатия----------------------------------------------------------------
        //------------------------------------------------------------Извлечение-----------------------------------------------------
        public string extract(string compressed, string[] newEncodingArray)
        {
            string decompressed = ""; 
            string current = "";
            string delta = "";
            encodingArray = newEncodingArray;

            displayEncodingArray();
            //получить кол-во вставленных нулей
            //for (int i = 0; i < 8; i++)
            //    delta += compressed[i];
            //int ADDED_ZEROES = Convert.ToInt32(delta, 2);

            for (int i = 0, l = compressed.Length; i < l; i++)
            {
                //i = 8, т.к. первым байтом у нас идет кол-во вставленных нулей
                current += compressed.ElementAt(i);
                for (int j = 0; j < ENCODING_TABLE_SIZE; j++)
                {
                    if (encodingArray[j] != null)
                    {
                        if (current == encodingArray[j].Substring(1, encodingArray[j].Length-1))
                        {//если совпало
                            decompressed += (char)j;//то добавляем элемент
                            current = "";//и обнуляем текущую строку
                        }
                    }
                }
            }

            return decompressed;
        }

        public string getEncodingTable()
        {
            string enc = "";
            for (int i = 0; i < encodingArray.Length; i++)
            {
                if (freqArray[i] != 0)
                    enc += (char)i + encodingArray[i] + '\n';
            }
            return enc;
        }

        public double getCompressionRatio()
        {
            return ratio;
        }


        public void displayEncodingArray()
        {//для отладки
            Console.WriteLine("======================Encoding table====================");
            for (int i = 0; i < ENCODING_TABLE_SIZE; i++)
            {
                //if (freqArray[i] != 0) {
                Console.Write((char)i + " ");
                Console.Write(encodingArray[i]);
                //}
            }
            Console.WriteLine("\n========================================================");
        }
    }

}
