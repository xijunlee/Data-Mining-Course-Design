using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Neuro;
using AForge.Neuro.Learning;
using System.IO;
using ClassLibTest;


namespace BPNETWORK_CS_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //读入训练数据
            StreamReader sr = new StreamReader("XData1.txt");
            var list = new List<double[]>();//列表的元素是一个个数组，这里用到数组是为了操作方便
            

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine().Split(new[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);//读入一行字符串
                //将字符串按空格切分,将切分成m个数
                var arr = new double[line.Length];//arr是用来装上述m个数的数组
                for (int i = 0; i < line.Length; i++) arr[i] = Convert.ToDouble(line[i]);
                list.Add(arr);//将数组arr添加到数组列表中去
            }
            double [][] trainInput = list.ToArray();
            list.Clear();
            sr.Close();

            sr = new StreamReader("TData1.txt");
            list = new List<double[]>();//列表的元素是一个个数组，这里用到数组是为了操作方便

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine().Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);//读入一行字符串
                //将字符串按空格切分,将切分成m个数
                var arr = new double[line.Length];//arr是用来装上述m个数的数组
                for (int i = 0; i < line.Length; i++) arr[i] = Convert.ToDouble(line[i]);
                list.Add(arr);//将数组arr添加到数组列表中去
            }
            double[][] trainOutput = list.ToArray();
            list.Clear();
            sr.Close(); 

            ActivationNetwork network = new ActivationNetwork(new SigmoidFunction(2), 12, 15, 5);

            BackPropagationLearning teacher = new BackPropagationLearning(network);

            teacher.LearningRate = 0.1;
            teacher.Momentum = 0;
            int iteration = 1;
            double error = 1.0;
            while (iteration<50000)
            {
                error = teacher.RunEpoch(trainInput, trainOutput);
                iteration++;
            }

            StreamWriter sw=new StreamWriter("result.txt",false);

            for (int i=0;i<132;i++)
            {
                var result = network.Compute(trainInput[i]);
                for (int j = 0; j < 5; j++)
                {
                    sw.Write(result[j].ToString() + '\t');
                }
                sw.WriteLine();
            }
        }
    }
}
