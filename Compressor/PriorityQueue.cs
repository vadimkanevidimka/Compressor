using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compressor
{
    class PriorityQueue
    {
        private List<BinaryTree> data;//список очереди
        internal int nElems;//кол-во элементов в очереди

        public PriorityQueue()
        {
            data = new List<BinaryTree>();
            nElems = 0;
        }

        public void insert(BinaryTree newTree)
        {//вставка
            if (nElems == 0)
                data.Add(newTree);
            else
            {
                for (int i = 0; i < nElems; i++)
                {
                    if (data[i].getFrequence() > newTree.getFrequence())
                    {//если частота вставляемого дерева меньше 
                        data.Insert(i, newTree);//чем част. текущего, то cдвигаем все деревья на позициях справа на 1 ячейку                   
                        break;//затем ставим новое дерево на позицию текущего
                    }
                    if (i == nElems - 1)
                        data.Add(newTree);
                }
            }
            nElems++;//увеличиваем кол-во элементов на 1
        }

        public BinaryTree remove()
        {//удаление из очереди
            BinaryTree tmp = data[0];//копируем удаляемый элемент
            data.RemoveAt(0);//собственно, удаляем
            nElems--;//уменьшаем кол-во элементов на 1
            return tmp;//возвращаем удаленный элемент(элемент с наименьшей частотой)
        }
    }
}
