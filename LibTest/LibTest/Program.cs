using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Neuro;
using AForge.Neuro.Learning;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace LibTest
{
    public class IO
    {
        private string TrainInputFile;//输入数据来源
        private string TrainOutputFile;//输出数据来源
        public static string[] XTitle;
        public static string[] TTitle;
        public double[][] XData;
        public double[][] TData;
        public double[][] AllData;
        public string[] AllTitle;
        public int XDimension;
        public int TDimension;
        public string AllDataFile;

        public IO(string trainInputFile, string trainOutputFile)
        {
            TrainInputFile = trainInputFile;
            TrainOutputFile = trainOutputFile;
        }

        public IO(string allDataFile)
        {
            AllDataFile = allDataFile;
        }

        public void ReadAllData()
        {
            Console.WriteLine("读取训练数据中...\n");
            StreamReader sr = new StreamReader(AllDataFile, System.Text.Encoding.Default);
            var list = new List<double[]>();//列表的元素是一个个数组，这里用到数组是为了操作方便

            AllTitle = sr.ReadLine().Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);//读入标题

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine().Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);//读入一行字符串
                //将字符串按空格切分,将切分成m个数
                var arr = new double[line.Length];//arr是用来装上述m个数的数组
                for (int i = 0; i < line.Length; i++) arr[i] = Convert.ToDouble(line[i]);
                list.Add(arr);//将数组arr添加到数组列表中去
            }
            AllData = list.ToArray();
            list.Clear();
            sr.Close();
            Console.WriteLine("读取完毕!\n");
        }

        public void ReadDataForNN()
        {
            Console.WriteLine("读入训练数据中...\n");
            //读入训练数据
            StreamReader sr = new StreamReader(TrainInputFile, System.Text.Encoding.Default);
            var list = new List<double[]>();//列表的元素是一个个数组，这里用到数组是为了操作方便

            XTitle = sr.ReadLine().Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);//读入标题
            
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine().Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);//读入一行字符串
                //将字符串按空格切分,将切分成m个数
                var arr = new double[line.Length];//arr是用来装上述m个数的数组
                for (int i = 0; i < line.Length; i++) arr[i] = Convert.ToDouble(line[i]);
                list.Add(arr);//将数组arr添加到数组列表中去
            }
            XData = list.ToArray();
            list.Clear();
            sr.Close();

            sr = new StreamReader(TrainOutputFile, System.Text.Encoding.Default);
            list = new List<double[]>();//列表的元素是一个个数组，这里用到数组是为了操作方便

            TTitle = sr.ReadLine().Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);//读入标题

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine().Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);//读入一行字符串
                //将字符串按空格切分,将切分成m个数
                var arr = new double[line.Length];//arr是用来装上述m个数的数组
                for (int i = 0; i < line.Length; i++) arr[i] = Convert.ToDouble(line[i]);
                list.Add(arr);//将数组arr添加到数组列表中去
            }
            TData = list.ToArray();
            list.Clear();
            sr.Close();
            Console.WriteLine("读取完毕!\n");
            XDimension = XData[0].Length;
            TDimension = TData[0].Length;
        }
        public static void WriteTitleToXls(string WriteFilePath, string[] data)
        {

            StreamWriter sw = new StreamWriter(WriteFilePath, false);
            for (int j = 0; j < data.Length; j++)
            {
                sw.Write(data[j] + "\t");
            }
            sw.WriteLine();
            sw.Close();
        }
        
    }

    public class GENE
    {
        public static int GeneLength;//基因链长度，整个类只有一份，并且是常量，不能更改
        public double[] GeneSerial;
        public double Fitness;
        public GENE(int length)
        {
            GeneSerial = new double[length];
            Fitness = 0;
        }
    }
   
    public class BPNet
    {
        public string SaveNetFile;//保存网络权值的文件
        private int BP_Iter_Max;
        private int InputDimension;
        private int OutputDimension;
        private int HiddenDimension;
        

        public BPNet(string saveNetFile, int inputDimension,int hiddenDimension, 
            int outputDimension, int bp_iter_Max)
        {
            InputDimension = inputDimension;
            OutputDimension = outputDimension;
            HiddenDimension = hiddenDimension;
          

            SaveNetFile = saveNetFile;
            BP_Iter_Max = bp_iter_Max;
            
        }

        public void TrainBPNet(double [][]trainInput,double [][]trainOutput)//建立BP神经网络，以获得适应度函数
        {
            Console.WriteLine("神经网络训练开始:\n");

            ActivationNetwork network = new ActivationNetwork(new SigmoidFunction(1.5), InputDimension, HiddenDimension, OutputDimension);//定义网络
            BackPropagationLearning teacher = new BackPropagationLearning(network);

            teacher.LearningRate = 0.1;
            teacher.Momentum = 0;
            int iteration = 1;
            double error = 1.0;
            Console.WriteLine("神经网络训练中...\n");
            while (iteration < BP_Iter_Max)
            {
                error = teacher.RunEpoch(trainInput, trainOutput);
                iteration++;
            }
            Console.WriteLine("训练完毕，神经网络权值保存在\\bin\\" + SaveNetFile + "中\n");
            SaveNet(network, SaveNetFile);

            Console.WriteLine("训练结果已保存在神经网络训练结果.xls中！\n");
            IO.WriteTitleToXls("神经网络训练结果.xls", IO.TTitle);
            StreamWriter sw = new StreamWriter("神经网络训练结果.xls", true);
            for (int i = 0; i < trainInput.Length; i++)
            {
                var result = network.Compute(trainInput[i]);
                for (int j = 0; j < result.Length; j++)
                    sw.Write(Convert.ToString(result[j]) + "\t");
                sw.WriteLine();
            }

            sw.WriteLine("\n误差=" + Convert.ToString(error));
            sw.Close();
        }

        /// <summary>
        /// Save the network
        /// </summary>
        /// <param name="Net">The network to save</param>

        public void SaveNet(ActivationNetwork Net, string FilePath)
        {
            FileStream fs = new FileStream(FilePath, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, Net);
            fs.Close();
        }
    }

    public class GA_12
    {
        public GA_12(string saveNetFile,int inputDimension,int outputDimension)//默认构造函数
        {
            SaveNetFile = saveNetFile;
            InputDimension = inputDimension;
            OutputDimension = outputDimension;
            Good = 0.8;

            PopulationSize = 50;
            MutationProb = 0.001;
            GA_Iter_Max = 1000;
            Population = new GENE[PopulationSize];
            BestPerson = new GENE[1000];
            for (int i = 0; i < PopulationSize; i++)
            {
                Population[i] = new GENE(GENE.GeneLength);
                BestPerson[i] = new GENE(GENE.GeneLength);
            }
        }

        public GA_12(int populationSize, double mutationProb, int ga_iter_Max, string saveNetFile, 
            int inputDimension, int outputDimension,double good)//构造函数重载
        {
            SaveNetFile = saveNetFile;
            InputDimension = inputDimension;
            OutputDimension = outputDimension;
            Good = good;

            PopulationSize = populationSize;
            MutationProb = mutationProb;
            GA_Iter_Max = ga_iter_Max;
            //CrossProb = crossProb;
            Population = new GENE[PopulationSize];
            BestPerson = new GENE[1000];
            for (int i = 0; i < PopulationSize; i++)
            {
                Population[i] = new GENE(GENE.GeneLength);
                BestPerson[i] = new GENE(GENE.GeneLength);
            }
        }

        private int PopulationSize;//种群大小
        private double MutationProb;//变异概率

        private GENE[] Population;
        private GENE[] BestPerson;
        private int BestCount;

        private int InputDimension;
        private int OutputDimension;
        private int GA_Iter_Max;
        private string SaveNetFile;
        private double Good;

        private ActivationNetwork network;//网络的声明
       
        public void GASolver()
        {
            GAInit();//一系列初始化工作
            
            int iter = 0;
            BestCount = 0;

            Console.WriteLine("遗传算法迭代求解中...\n");

            while  (iter<GA_Iter_Max&&BestCount < 10)
            {
                
                Replicate();

                FitnessSort(Population, 0, PopulationSize - 1);
                if (IsBiggerThanBounder(Population[PopulationSize - 1].Fitness) &&
                    IsDifferentAnswer(Population[PopulationSize - 1].Fitness))
                {
                    BestPerson[BestCount++] = Population[PopulationSize - 1];
                }
                
                Cross();

                FitnessSort(Population, 0, PopulationSize - 1);
                if (IsBiggerThanBounder(Population[PopulationSize - 1].Fitness) &&
                    IsDifferentAnswer(Population[PopulationSize - 1].Fitness))
                {
                    BestPerson[BestCount++] = Population[PopulationSize - 1];
                }
                
                Mutation();
                FitnessSort(Population, 0, PopulationSize - 1);
                if (IsBiggerThanBounder(Population[PopulationSize - 1].Fitness) && 
                    IsDifferentAnswer(Population[PopulationSize - 1].Fitness))
                {
                    BestPerson[BestCount++] = Population[PopulationSize - 1];
                }
                
                iter++;
            }
           
        }

        private bool IsBiggerThanBounder(double x)
        {
            bool flag = false;
            if (x - OutputDimension*Good > 0.000001)
                flag = true;
            return flag;
        }

        private bool IsDifferentAnswer(double x)
        {
            bool flag=true;

            for (int i = 0; i < BestCount; i++)
                if (abs(x - BestPerson[i].Fitness) < 0.001)
                { flag = false; break; }
            
            return flag;
        }

        private double abs(double x)
        {
            if (x >= 0)
                return x;
            else return -x;
        }

        public string DisplayAnswer()
        {
            
                IO.WriteTitleToXls("遗传算法结果1.xls", IO.XTitle);
                StreamWriter sw = new StreamWriter("遗传算法结果1.xls", true);

                FitnessSort(Population,0,PopulationSize-1);
                
                for (int i=PopulationSize-1;i>=0;i--)
                {
                    for (int j=0;j<GENE.GeneLength;j++)
                       sw.Write(Convert.ToString(Population[i].GeneSerial[j]) + "\t");
                       sw.WriteLine("适应度="+Convert.ToString(Population[i].Fitness));
                }
                sw.Close();

                Console.WriteLine("遗传算法完整结果保存在遗传算法结果1.xls中\n");

                FitnessSort(BestPerson, 0, BestCount - 1);
                string ResultRe=null;

                for (int i = BestCount - 1; i >= 0; i--)
                {
                    ResultRe+="适应度:"+Convert.ToString(BestPerson[i].Fitness)+"\n";
                }

                return ResultRe;
         
        }

        private void Replicate()
        {
            GENE[] NextPopulation = new GENE[PopulationSize];//声明下一代种群
            for (int i = 0; i < PopulationSize; i++)//对种群中每个个体进行声明
                NextPopulation[i] = new GENE(GENE.GeneLength);

            double[] pi = new double[PopulationSize];
            double FitnessSum = 0;
            //复制
            FitnessSum = 0;
            FitnessSort(Population, 0, PopulationSize - 1);
            //采取“保留最佳精英策略”，将最优个体直接替代最差个体;
            //但最差个体仍有机会参与复制
            NextPopulation[0] = Population[PopulationSize - 1];
            //构造轮盘赌
            for (int i = 0; i < PopulationSize; i++)
                FitnessSum += Population[i].Fitness;
            
            pi[0] = Population[0].Fitness / FitnessSum;
            for (int i = 1; i < PopulationSize; i++)
                pi[i] = Population[i].Fitness / FitnessSum + pi[i - 1];

            Random rd = new Random();
            for (int i = 1; i < PopulationSize; i++)
            {
                double tmp = rd.NextDouble();
                int copy = 0;
                for (int j = 0; j < PopulationSize; j++)
                    if (tmp <= pi[j])
                    { copy = j; break; }
                NextPopulation[i] = Population[copy];
            }

            for (int i = 0; i < PopulationSize; i++)
                Population[i] = NextPopulation[i];

            //调试
            for (int i = 0; i < PopulationSize; i++)
                Population[i].Fitness = FitnessFunc(Population[i]);
        }

        private void Cross()
        {
            int[] hash = new int[PopulationSize];
            Random rd = new Random();
            //交叉
            for (int i = 0; i < PopulationSize; i++)
                hash[i] = 0;
            hash[0] = 1;
            //最佳个体不参与交叉操作
            for (int i = 1; i < PopulationSize / 2; i++)
            {
                hash[i] = 1;
                int j = 0;//hash[0] = 1;
                while (hash[j] == 1)
                {
                    j = rd.Next(PopulationSize / 2, PopulationSize);
                    //找到一个hash[j]==0的，就跳出
                }
                hash[j] = 1;
                //Xi,Xj进行交叉
                double a = rd.NextDouble();
                if (i != 0 && j != 0)
                {
                    for (int k = 0; k < GENE.GeneLength; k++)
                    {
                        Population[i].GeneSerial[k] = a * Population[i].GeneSerial[k] + (1.0 - a) * Population[j].GeneSerial[k];
                        Population[j].GeneSerial[k] = a * Population[j].GeneSerial[k] + (1.0 - a) * Population[i].GeneSerial[k];
                    }
                }
            }

            //调试
            for (int i = 0; i < PopulationSize; i++)
                Population[i].Fitness = FitnessFunc(Population[i]);
        }

        private void Mutation()
        {
            Random rd = new Random();
            //变异
            for (int k = 0; k < PopulationSize * GENE.GeneLength * MutationProb; k++)
            {
                int i = rd.Next(0, PopulationSize);//随机抽取个体Xi
                int ik = rd.Next(0, GENE.GeneLength);//随机选取需要变异的基因位
                double vk = rd.NextDouble();//产生变异值
                Population[i].GeneSerial[ik] = vk;
            }

            //调试
            for (int i = 0; i < PopulationSize; i++)
                Population[i].Fitness = FitnessFunc(Population[i]);
        }

        private void GAInit()
        {
            Console.WriteLine("加载神经网络...\n");
            LoadNet(SaveNetFile);

            Console.WriteLine("进化计算开始:\n");
            Console.WriteLine("初始化种群\n");
            //随机初始化种群个体的基因序列
            Random rd = new Random();
            for (int i = 0; i < PopulationSize; i++)
            {
                for (int j = 0; j < GENE.GeneLength; j++)
                {
                    Population[i].GeneSerial[j] = rd.NextDouble();//随机初始化每个基因
                }
                //计算个体的适应度
                Population[i].Fitness = FitnessFunc(Population[i]);
                //Console.WriteLine(Population[i].Fitness);
            }

        }

        /// <summary>
        /// Load a network
        /// </summary>
        /// <param name="FilePath">The path to the binary network file</param>
        /// <returns></returns>
        private void LoadNet(string FilePath)
        {
            FileStream fs = new FileStream(FilePath, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            ActivationNetwork net = (ActivationNetwork)formatter.Deserialize(fs);
            fs.Close();
            network = net;
        }

        private double FitnessFunc(GENE x)//适应度函数，index为染色体编号
        {
            var result = network.Compute(x.GeneSerial);//计算对应函数值
            double tmp = 0;
            for (int i = 0; i < result.Length; i++)
                tmp += result[i];
            return tmp;
        }

        private void FitnessSort(GENE[] a, int st, int ed)//适应度排序
        {
            if (st >= ed)
                return;
            int i = st, j = ed;
            GENE tmp = a[i];
            while (i < j)
            {
                //while (i < j && CompareSmall(a[j].Fitness,tmp.Fitness))
                while (i < j && a[j].Fitness >= tmp.Fitness)
                    j--;
                if (i < j)
                    a[i] = a[j];
                //while (i < j && CompareSmall(tmp.Fitness,a[i].Fitness))
                while (i < j && a[i].Fitness < tmp.Fitness) 
                    i++;
                if (i < j)
                    a[j] = a[i];
            }
            a[i] = tmp;
            FitnessSort(a, st, i - 1);
            FitnessSort(a, i + 1, ed);
        }
       
    }

    public class GA_3
    {
        private int[] InveseIndex;//表示指定投资的股票序号,默认是132支股票
        private double C;//自定义投资额
        private double [][]StockData;//所有股票的数据

        private int PopulationSize;//种群大小
        private double MutationProb;//变异概率
        private GENE[] Population;//种群,只是声明了，但是还未定义
        private GENE[] BestAnswer;
        private int BestCount;
        private int GA_Iter_Max;//遗传算法最大迭代次数
        private double EquityRatio;

        public GA_3(double[][] stockData, int[] inveseIndex)
        {
            InveseIndex = inveseIndex;
            C = 100;
            StockData = stockData;
            PopulationSize = 50;
            MutationProb = 0.001;
            GA_Iter_Max = 100;
            EquityRatio = 50.0;

            BestAnswer = new GENE[1000];
            Population = new GENE[PopulationSize];

            for (int i = 0; i < PopulationSize; i++)
            {
                Population[i] = new GENE(GENE.GeneLength);
            }
            for (int i = 0; i < 1000; i++)
            {
                BestAnswer[i] = new GENE(GENE.GeneLength);
            }
        }

        public GA_3(double[][] stockData, int[] inveseIndex, double c,double equityRatio,
            int populationSize,double mutationPro,int ga_iter_max)//构造函数重载
        {
            InveseIndex = inveseIndex;
            C = c;
            StockData = stockData;
            PopulationSize = populationSize;
            MutationProb = mutationPro;
            GA_Iter_Max = ga_iter_max;
            EquityRatio = equityRatio;

            Population = new GENE[PopulationSize];
            BestAnswer = new GENE[1000];

            for (int i = 0; i < PopulationSize; i++)
            {
                Population[i] = new GENE(GENE.GeneLength);
            }
            for (int i = 0; i < 1000; i++)
            {
                BestAnswer[i] = new GENE(GENE.GeneLength);
            }
            
        }

        public void GASolver()
        {
            BestCount = 0;
            int iter = 0;
            GAInit();

            Console.WriteLine("遗传算法迭代求解中...\n");

            while (iter < GA_Iter_Max)
            {
                Replicate();
                
                Cross();
                
                Mutation();
                
                iter++;
            }
        }

        public string DisplayAnswer()
        {
            StreamWriter sw = new StreamWriter("遗传算法结果2.xls", false);
            
            string title = "适应度\t";

            for (int i = 0; i < InveseIndex.Length; i++)
            {
                title += "第" + Convert.ToString(InveseIndex[i]) + "支股票\t";
            }
            sw.WriteLine(title);
            FitnessSort(Population, 0, PopulationSize - 1);
            for (int i = PopulationSize - 1; i >= 0; i--)
            {
                sw.Write(Convert.ToString(Population[i].Fitness) + "\t");
                for (int j = 0; j < GENE.GeneLength; j++)
                    sw.Write(Convert.ToString(Population[i].GeneSerial[j]) + "\t");
                sw.WriteLine();
            }
            sw.Close();

            Console.WriteLine("遗传算法完整结果保存在遗传算法结果1.xls中\n");

            string ReResult = null;
            for (int i = PopulationSize - 1; i >= 0; i--)
            {
                bool flag = false;
                for (int j = 0; j < BestCount; j++)
                    if (abs(Population[i].Fitness - BestAnswer[j].Fitness) < 0.1)//表明在队列中已经出现过
                    {
                        flag = true;
                        break;
                    }
                if (flag == false)
                {
                    BestAnswer[BestCount++] = Population[i];
                }
            }

            FitnessSort(BestAnswer, 0, BestCount - 1);
            for (int i=BestCount-1;i>=0;i--)
                ReResult+="适应度:"+Convert.ToString(BestAnswer[i].Fitness)+"\n";

            return ReResult;  
        }

        private void Replicate()
        {
            GENE[] NextPopulation = new GENE[PopulationSize];//声明下一代种群
            for (int i = 0; i < PopulationSize; i++)//对种群中每个个体进行声明
                NextPopulation[i] = new GENE(GENE.GeneLength);

            double[] pi = new double[PopulationSize];
            double FitnessSum = 0;
            //复制
            FitnessSum = 0;
            FitnessSort(Population, 0, PopulationSize - 1);
            //采取“保留最佳精英策略”，将最优个体直接替代最差个体;
            //但最差个体仍有机会参与复制
            NextPopulation[0] = Population[PopulationSize - 1];
            //构造轮盘赌
            for (int i = 0; i < PopulationSize; i++)
                FitnessSum += Population[i].Fitness;

            pi[0] = Population[0].Fitness / FitnessSum;
            for (int i = 1; i < PopulationSize; i++)
                pi[i] = Population[i].Fitness / FitnessSum + pi[i - 1];

            Random rd = new Random();
            for (int i = 1; i < PopulationSize; i++)
            {
                double tmp = rd.NextDouble();
                int copy = 0;
                for (int j = 0; j < PopulationSize; j++)
                    if (tmp <= pi[j])
                    { copy = j; break; }
                NextPopulation[i] = Population[copy];
            }

            for (int i = 0; i < PopulationSize; i++)
                Population[i] = NextPopulation[i];

            //调试
            for (int i = 0; i < PopulationSize; i++)
                Population[i].Fitness = FitnessFunc(Population[i]);
        }

        private void Cross()
        {
            int[] hash = new int[PopulationSize];
            Random rd = new Random();
            //交叉
            for (int i = 0; i < PopulationSize; i++)
                hash[i] = 0;
            hash[0] = 1;
            //最佳个体不参与交叉操作
            for (int i = 1; i < PopulationSize / 2; i++)
            {
                hash[i] = 1;
                int j = 0;//hash[0] = 1;
                while (hash[j] == 1)
                {
                    j = rd.Next(PopulationSize / 2, PopulationSize);
                    //找到一个hash[j]==0的，就跳出
                }
                hash[j] = 1;
                //Xi,Xj进行交叉
                double a = rd.NextDouble();
                if (i != 0 && j != 0)
                {
                    for (int k = 0; k < GENE.GeneLength; k++)
                    {
                        Population[i].GeneSerial[k] = a * Population[i].GeneSerial[k] + (1.0 - a) * Population[j].GeneSerial[k];
                        Population[j].GeneSerial[k] = a * Population[j].GeneSerial[k] + (1.0 - a) * Population[i].GeneSerial[k];
                    }
                }
            }

            //调试
            for (int i = 0; i < PopulationSize; i++)
                Population[i].Fitness = FitnessFunc(Population[i]);
        }

        private void Mutation()
        {
            Random rd = new Random();
            //变异
            for (int k = 0; k < PopulationSize * GENE.GeneLength * MutationProb; k++)
            {
                int i = rd.Next(0, PopulationSize);//随机抽取个体Xi
                int ik = rd.Next(0, GENE.GeneLength);//随机选取需要变异的基因位
                double vk = rd.NextDouble();//产生变异值
                Population[i].GeneSerial[ik] = vk * Population[i].GeneSerial[ik];
            }

            //调试
            for (int i = 0; i < PopulationSize; i++)
                Population[i].Fitness = FitnessFunc(Population[i]);
        }

        private void GAInit()
        {
            Console.WriteLine("进化计算开始:\n");
            Console.WriteLine("初始化种群\n");
            //随机初始化种群个体的基因序列
            Random rd = new Random();
            
            for (int i = 0; i < PopulationSize; i++)
            {
                
                double[] p = new double[GENE.GeneLength];
                double tmp = 0;
                for (int j = 0; j < GENE.GeneLength; j++)
                {
                    p[j] = rd.NextDouble();
                    tmp += p[j];
                }

                for (int j = 0; j < GENE.GeneLength; j++)
                    p[j] = p[j] / tmp;

                for (int j = 0; j < GENE.GeneLength; j++)
                    Population[i].GeneSerial[j] = C * p[j];
         
                //计算个体的适应度
                Population[i].Fitness = FitnessFunc(Population[i]);
                //Console.WriteLine(Population[i].Fitness);
            }

        }

        private double FitnessFunc(GENE x)//适应度函数
        {
            double tmp = 0;
            for (int i = 0; i < GENE.GeneLength; i++)
                tmp += x.GeneSerial[i];
            if (tmp > C)
                return 0;
            double tmp1 = 0;
            double alpha = 3.0;
            for (int i = 0; i < GENE.GeneLength; i++)
                tmp1 += x.GeneSerial[i] * StockData[InveseIndex[i]][3];


            double tmp2 = C;
            double beta = 3.0;
            for (int i = 0; i < GENE.GeneLength; i++)
                tmp2 -= x.GeneSerial[i];

            double tmp3 = 0;
            double gama = 1.0;
            for (int i = 0; i < GENE.GeneLength; i++)
                tmp3 += x.GeneSerial[i] * abs(StockData[InveseIndex[i]][4] - EquityRatio) / EquityRatio;

            return alpha * tmp1 + beta * tmp2 - gama * tmp3;
        }

        private void FitnessSort(GENE[] a, int st, int ed)//适应度排序
        {
            if (st >= ed)
                return;
            int i = st, j = ed;
            GENE tmp = a[i];
            while (i < j)
            {
                //while (i < j && CompareSmall(a[j].Fitness,tmp.Fitness))
                while (i < j && a[j].Fitness >= tmp.Fitness)
                    j--;
                if (i < j)
                    a[i] = a[j];
                //while (i < j && CompareSmall(tmp.Fitness,a[i].Fitness))
                while (i < j && a[i].Fitness < tmp.Fitness)
                    i++;
                if (i < j)
                    a[j] = a[i];
            }
            a[i] = tmp;
            FitnessSort(a, st, i - 1);
            FitnessSort(a, i + 1, ed);
        }

        private double abs(double x)
        {
            if (x >= 0)
                return x;
            else return - x;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //求解1、2问，读入所需数据
            IO IOFor12 = new IO("xdata2.txt", "tdata2.txt");
            IOFor12.ReadDataForNN();

            //训练神经网络
            BPNet bp = new BPNet("net.txt", IOFor12.XDimension, 19, IOFor12.TDimension, 50000);
            bp.TrainBPNet(IOFor12.XData, IOFor12.TData);

            //////1、2问，遗传算法求解
            //GENE.GeneLength = IOFor12.XDimension;//基因序列长度
            //GA_12 ga_12 = new GA_12(100, 0.001, 100, "net.txt", IOFor12.XDimension, IOFor12.TDimension, 0.7);
            //ga_12.GASolver();
            //string answer = ga_12.DisplayAnswer();
            //Console.Write(answer);


            ////求解第3问，读入所需数据
            //IO IOFor3 = new IO("alldata.txt");
            //IOFor3.ReadAllData();
            ////第三问，遗传算法求解
            //const int StocksCount = 5;
            //int[] stocksIndex = new int[StocksCount];//自己选择要投资的股票
            //for (int i = 0; i < StocksCount; i++)
            //    stocksIndex[i] = i;
            //GENE.GeneLength = StocksCount;//基因序列长度
            ////GA_3 ga_3 = new GA_3(IOFor3.AllData, stocksIndex, 100, 0.5, 50, 0.001, 1000);
            //GA_3 ga_3 = new GA_3(IOFor3.AllData, stocksIndex);
            //ga_3.GASolver();
            //string answer = null;
            //answer=ga_3.DisplayAnswer();
            //Console.Write(answer);
        }
    }
}
