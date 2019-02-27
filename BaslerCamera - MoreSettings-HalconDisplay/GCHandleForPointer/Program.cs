using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;   //引入GCHandle类

namespace GCHandleForPointer
{
    class Program
    {
        static void Main(string[] args)
        {
            //定义一个byte类型的数组
            byte[] array = { 1, 2, 3, 4, 5 };
            //定义一个GCHandle类的对象，锁住array中的数据
            GCHandle hand = GCHandle.Alloc(array, 
                GCHandleType.Pinned);
            //获取锁住数据的指针
            IntPtr arrayPointer = hand.AddrOfPinnedObject();
            //打印获取的内存地址
            Console.WriteLine("通过指针打印数据：{0}",
                arrayPointer.ToString());
            //完成后释放内存
            if (hand.IsAllocated) hand.Free();

            Console.ReadKey();
        }
    }
}
