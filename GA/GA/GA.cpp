// GA.cpp : �������̨Ӧ�ó������ڵ㡣
//

#include "stdafx.h"

using namespace std;

typedef struct node{
    int *squence;
    double fit;
}Gene;

class GA
{
    private:
    	int m;//��Ⱥ��С
    	Gene gene[100];
    	int GeneLength;
    	double Pm;
    public:
        GA(int,double); //��ֻ�Ǻ���������������ĺ����嶨����Ҫ���ⲿд
         
        void GAMain();
    	double Fitness(int x);
        void qsort(Gene *,int,int);
        int *Code(int x);
        int Decode(int *a);
};

GA::GA(int size,double ProbMutute)//���캯������ʼ������
{
    //������Ⱥ��С�����������Ⱥ
	m=size;
    Pm=ProbMutute;
    GeneLength=5;
    srand((int)time(0));
    for (int i=0;i<m;i++)
    {
        int tmp=random(31);
        gene[i].squence=Code(tmp);
        gene[i].fit=Fitness(tmp);
    }
	/*cout<<"ԭʼ"<<endl;
	 for (int i=0;i<m;i++)
        {
            cout<<gene[i].fit<<" ";
            for (int j=0;j<GeneLength;j++)
                cout<<gene[i].squence[j];
            cout<<endl;
        }
	  cout<<endl;*/
}
void GA::GAMain()
{
    int tmax=100,flag=1,t=0;
    double FitMax=0;
    while (t<tmax)//�Ŵ��㷨����������������
    {
      qsort(gene,0,m-1);
      //����
         Gene tmpGene[100];
         //�������̶�
         double FitSum=0,pi[100];
         for (int i=0;i<m;i++)
          FitSum+=gene[i].fit;
         pi[0]=gene[0].fit/FitSum;
         for (int i=1;i<m;i++)
         {
          double pp=gene[i].fit/FitSum;
          pi[i]=pp+pi[i-1];
         }
         srand((int)time(0));
         for (int i=0;i<m;i++)
         {
            double tmp=random(100)/100.0;
            int copy=0;
            for (int j=0;j<m;j++)
                if (tmp<=pi[j])
                {copy=j;break;}
            tmpGene[i]=gene[copy];
         }
		
      //����
         int CrossPos=2;//���ý����λ��
         for (int i=0;i<m/2;i++)
         {
             int j=i+m/2;
             int tmpNum=0;
             for (int k=CrossPos;k<GeneLength;k++)
             {
                tmpNum=tmpGene[i].squence[k];
                tmpGene[i].squence[k]=tmpGene[j].squence[k];
                tmpGene[j].squence[k]=tmpNum;
             }
         }
      //����
         srand((int)time(0));
         for (int i=0;i<m;i++)
          for (int j=0;j<GeneLength;j++)
          {
            double tmp=random(100)/100.0;
            if (tmp<=Pm)
                tmpGene[i].squence[j]=(tmpGene[i].squence[j]+1)%2;
          }
      //������Ӧ�Ȳ�����
         for (int i=0;i<m;i++)
         {
            tmpGene[i].fit=Fitness(Decode(tmpGene[i].squence));
            gene[i]=tmpGene[i];
         }
        for (int i=0;i<m;i++)
        {
            cout<<gene[i].fit<<" ";
            for (int j=0;j<GeneLength;j++)
                cout<<gene[i].squence[j];
            cout<<endl;
        }
		cout<<endl;
	  t++;
	}
	qsort(gene,0,m-1);
	int maxAns=Decode(gene[m-1].squence);
	cout<<maxAns<<endl;
}
void GA::qsort(Gene *a,int st,int ed)//����Ӧ������Ŀ���
{
    if (st>=ed)
        return;
    int i=st,j=ed;
    Gene tmp=a[i];
    while(i<j)
    {
        while (i<j&&a[j].fit>=tmp.fit)
            j--;
        if (i<j)
            a[i]=a[j];
        while (i<j&&a[i].fit<=tmp.fit)
            i++;
        if (i<j)
            a[j]=a[i];
    }
    a[i]=tmp;
    qsort(a,st,i-1);
    qsort(a,i+1,ed);
}
int *GA::Code(int x)
{
    int tmp=x,i=0;
    int *a = new int[GeneLength]();//���ص���ָ�룬���������ڴ��е�λ��
    while (tmp>0&&i<GeneLength)
    {
       a[i++]=tmp%2;
       tmp/=2;
    }
    return a;
}
int GA::Decode(int *a)
{ 
    int num=0,tmp=1;
    for (int i=0;i<GeneLength;i++)
    {
        num+=tmp*a[i];
        tmp*=2;
    }
    return num;
}
double GA::Fitness(int x)
{
    return x*x;
}

int _tmain(int argc, _TCHAR* argv[])
{
	
    GA ga=GA(30,0.001);
	ga.GAMain();
    return 0;
}

