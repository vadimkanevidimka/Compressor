using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compressor
{
    public class HuffmanTree
    {
        private const byte ENCODING_TABLE_SIZE = 255;//длина кодировочной таблицы
        private string mystring;//сообщение
        private BinaryTree huffmanTree;//дерево Хаффмана
        private int[] freqArray;//частотная таблица
        private string[] encodingArray;//кодировочная таблица


        //----------------constructor----------------------
        public HuffmanTree(string newstring)
        {
            mystring = newstring;

            freqArray = new int[ENCODING_TABLE_SIZE];
            fillFrequenceArray();

            huffmanTree = getHuffmanTree();

            encodingArray = new string[ENCODING_TABLE_SIZE];
            fillEncodingArray(huffmanTree.getRoot(), "", "");
        }

        //--------------------frequence array------------------------
        private void fillFrequenceArray()
        {
            for (int i = 0; i < mystring.Length; i++)
            {
                int a = (int)mystring[i];
                freqArray[a]++;
            }
        }

        public int[] getFrequenceArray()
        {
            return freqArray;
        }

        //------------------------huffman tree creation------------------
        private BinaryTree getHuffmanTree()
        {
            PriorityQueue pq = new PriorityQueue();
            //алгоритм описан выше
            for (int i = 0; i < ENCODING_TABLE_SIZE; i++)
            {
                if (freqArray[i] != 0)
                {//если символ существует в строке
                    Node newNode = new Node((char)i, freqArray[i]);//то создать для него Node
                    BinaryTree newTree = new BinaryTree(newNode);//а для Node создать BinaryTree
                    pq.insert(newTree);//вставить в очередь
                }
            }

            while (true)
            {
                BinaryTree tree1 = pq.remove();//извлечь из очереди первое дерево.

                try
                {
                    if (pq.nElems == 0)
                    {
                        return tree1;
                    }
                    BinaryTree tree2 = pq.remove();//извлечь из очереди второе дерево

                    Node newNode = new Node();//создать новый Node
                    newNode.addChild(tree1.getRoot());//сделать его потомками два извлеченных дерева
                    newNode.addChild(tree2.getRoot());

                    pq.insert(new BinaryTree(newNode));
                }
                catch (IndexOutOfRangeException e)
                {//осталось одно дерево в очереди
                    return tree1;
                }
            }
        }

        internal BinaryTree getTree()
        {
            return huffmanTree;
        }

        //-------------------encoding array------------------
        void fillEncodingArray(Node node, string codeBefore, string direction)
        {//заполнить кодировочную таблицу
            if (node.isLeaf())
            {
                encodingArray[(int)node.getLetter()] = codeBefore + direction;
            }
            else
            {
                fillEncodingArray(node.getLeftChild(), codeBefore + direction, "0");
                fillEncodingArray(node.getRightChild(), codeBefore + direction, "1");
            }
        }

        public string[] getEncodingArray()
        {
            return encodingArray;
        }

        public void displayEncodingArray()
        {
            fillEncodingArray(huffmanTree.getRoot(), "", "");

            Console.WriteLine("======================Encoding table====================");
            for (int i = 0; i < ENCODING_TABLE_SIZE; i++)
            {
                if (freqArray[i] != 0)
                {
                    Console.Write((char)i + " ");
                    Console.Write(encodingArray[i]);
                }
            }
            Console.WriteLine("========================================================");
        }
        //-----------------------------------------------------
        public string getOriginalstring()
        {
            return mystring;
        }
    }

}
