using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using GALib;

namespace DMCourseDesign
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //公用变量区
        IO IOFor12;
        IO IOFor3;
        private void InputDataFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt文件(*.txt)|*.txt";
            string s_FileName = null;
            if (openFileDialog1.ShowDialog().Value == true)
            {
                s_FileName = openFileDialog1.FileName;
            }
            InputDataFileBlock.Text = s_FileName;
        }

        private void OutputDataFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt文件(*.txt)|*.txt";
            string s_FileName = null;
            if (openFileDialog1.ShowDialog().Value == true)
            {
                s_FileName = openFileDialog1.FileName;
            }
            OutputDataFileBlock.Text = s_FileName;
        }

        private void DataInputEnsureButton_Click(object sender, RoutedEventArgs e)
        {
            if (InputDataFileBlock.Text != "" && OutputDataFileBlock.Text != "")
            {
                IOFor12 = new IO(InputDataFileBlock.Text, OutputDataFileBlock.Text);
                IOFor12.ReadDataForNN();
                MessageBox.Show("数据读取完毕！","通知");
            }
            else
            {
                MessageBox.Show("请输入完整的数据地址!","警告");
            }
            StartTrainNNButton.IsEnabled = true;
            InputDimensionTextBox.Text = Convert.ToString(IOFor12.XDimension);
            OutputDimensionTextBox.Text = Convert.ToString(IOFor12.TDimension);
        }

        private void NNZHUANJIA_Checked(object sender, RoutedEventArgs e)
        {
            HiddenBlock.IsEnabled = true;
            NNIterBlock.IsEnabled = true;
        }

        private void NNJIANDAN_Checked(object sender, RoutedEventArgs e)
        {
            HiddenBlock.IsEnabled = false;
            NNIterBlock.IsEnabled = false;
            HiddenBlock.Text = "20";
            NNIterBlock.Text = "50000";
        }

        private void StartTrainNNButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("神经网络训练中，关闭此通知后请耐心等待！", "通知");
            
            //训练神经网络
            int hidden = Convert.ToInt32(HiddenBlock.Text);
            int iter_max = Convert.ToInt32(NNIterBlock.Text);
            
            BPNet bp = new BPNet("NetWeight.txt", IOFor12.XDimension, hidden, IOFor12.TDimension, iter_max);
            bp.TrainBPNet(IOFor12.XData, IOFor12.TData);
            MessageBox.Show("神经网络训练完毕!\n网络权值已保存在NetWeight.txt中！\n神经网络训练结果已保存在根目录中!"
                ,"通知");
            StartTrainNNButton.Content = "开始训练";
            StartTrainNNButton.IsEnabled = true;
        }

        private void ReadNNButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt文件(*.txt)|*.txt";
            string s_FileName = null;
            if (openFileDialog1.ShowDialog().Value == true)
            {
                s_FileName = openFileDialog1.FileName;
            }
            LoadNNTextbox.Text = s_FileName;
            if (LoadNNTextbox.Text!=""&&InputDataFileBlock.Text!=""&&OutputDataFileBlock.Text!="")
            GAButton.IsEnabled = true;
        }

        private void GAButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoadNNTextbox.Text != "" &&
                InputDimensionTextBox.Text != "" &&
                OutputDimensionTextBox.Text != "" &&
                PopulationSizeTextBox.Text != "" &&
                MutationProbTextBox.Text != "" &&
                GAIterTextBox.Text != "" &&
                AccuracyTextBox.Text != "")
            {
                string NetPath = LoadNNTextbox.Text;
                int inputDim = Convert.ToInt32(InputDimensionTextBox.Text);
                int outputDim = Convert.ToInt32(OutputDimensionTextBox.Text);
                int populationSize = Convert.ToInt32(PopulationSizeTextBox.Text);
                double mutationProb = Convert.ToDouble(MutationProbTextBox.Text);
                int ga_iter_max = Convert.ToInt16(GAIterTextBox.Text);
                double accuracy = Convert.ToDouble(AccuracyTextBox.Text);
                GENE.GeneLength = IOFor12.XDimension;//基因序列长度
                GA_12 ga_12 = new GA_12(populationSize, mutationProb, ga_iter_max, NetPath, inputDim, outputDim, accuracy);
                ga_12.GASolver();
                string answer = ga_12.DisplayAnswer();
                ResultTextBox.Text = answer;
                displayWholeAnswerPathTextBox.Text="保存在根目录中的遗传算法结果1.xls中！";
            }
            else
            {
                MessageBox.Show("请填写完整条件数据!", "警告");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DataDefine dd = new DataDefine();
            dd.Show();
        }

        private void inputAlldataButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt文件(*.txt)|*.txt";
            string s_FileName = null;
            if (openFileDialog1.ShowDialog().Value == true)
            {
                s_FileName = openFileDialog1.FileName;
            }
            ALLdataFilePathTextbox.Text = s_FileName;
        }

        private void GA3StartButton_Click(object sender, RoutedEventArgs e)
        {
            string[] str = StocksIndexTextBox.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);;
            int[] stocks = new int[str.Length];
            for (int i = 0; i < str.Length; i++)
                stocks[i] = Convert.ToInt32(str[i]);
            IOFor3 = new IO(ALLdataFilePathTextbox.Text);
            IOFor3.ReadAllData();
            GENE.GeneLength = stocks.Length;
            double c=Convert.ToDouble(TouzieTextBox.Text);
            double mutationProb=Convert.ToDouble(MutationProbTextBox_Copy.Text);
            int poplationSize=Convert.ToInt32(PopulationSizeTextBox_Copy.Text);
            int ga_iter_max=Convert.ToInt32(GAIterTextBox_Copy.Text);
            double GDQY=Convert.ToDouble(GudongQuanyiTextBox.Text);
            GA_3 ga_3 = new GA_3(IOFor3.AllData, stocks, c, GDQY, 
                poplationSize, mutationProb, ga_iter_max);
            ga_3.GASolver();
            string answer = ga_3.DisplayAnswer();
            answer += "\n完整结果保存在根目录中的遗传算法结果2.xls中！";
            DisanwenResultTextBox.Text = answer;
        }

       
    }
}
