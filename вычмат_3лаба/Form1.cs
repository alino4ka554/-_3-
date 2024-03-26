using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace вычмат_3лаба
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                float[] x = textBox1.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(str => float.Parse(str.Trim())).ToArray();
                float[] fx = textBox2.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(str => float.Parse(str.Trim())).ToArray();
                float[] a = textBox3.Text.Split(new[] { '+','x', '^', ' ', '*' }, StringSplitOptions.RemoveEmptyEntries).Select(str => float.Parse(str.Trim())).ToArray();
                float[] interval = textBox4.Text.Split(new[] { ';',' ' }, StringSplitOptions.RemoveEmptyEntries).Select(str => float.Parse(str.Trim())).ToArray();
                float[] interval2 = textBox5.Text.Split(new[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(str => float.Parse(str.Trim())).ToArray();
                /*for (int i = 0; i < 5; i++)
                {
                    textBox4.Text += $"{a[i].ToString()} ";

                }*/
                List<int> index = new List<int>();  
                foreach(int indexChecked in checkedListBox1.CheckedIndices) //смотрим какой график выбрал пользователь
                {
                   index.Add(indexChecked);    
                }
                ChartForm chart = new ChartForm(index, x, fx, a, interval, interval2);
                chart.Show();
               
            }
            catch
            {
                MessageBox.Show("Ошибка!");
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
    class ChartForm : Form
    {
        public float[] gaus(float[][] a, float[] b)
        {
            int n = a.Length;
            for (int i = 0; i < n - 1; i++)
            {
                //выбор главного элемента по столбцу 
                //поиск главного элемента в текущем столбце
                int maxRow = i;
                float maxVal = Math.Abs(a[i][i]);
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(a[k][i]) > maxVal)
                    {
                        maxVal = Math.Abs(a[k][i]);
                        maxRow = k;
                    }
                }
                //обмен строк, если главный элемент не находится в текущей строке
                if (maxRow != i)
                {
                    float[] tempA = a[i];
                    a[i] = a[maxRow];
                    a[maxRow] = tempA;

                    float tempB = b[i];
                    b[i] = b[maxRow];
                    b[maxRow] = tempB;
                }

                //прямой ход метода Гаусса
                for (int k = i + 1; k < n; k++)
                {
                    float factor = -a[k][i] / a[i][i];
                    for (int j = i; j < n; j++)
                    {
                        a[k][j] += factor * a[i][j];

                    }
                    b[k] += factor * b[i];
                }
            }


            //обратный ход метода Гаусса
            float[] x = new float[n];
            for (int i = n - 1; i >= 0; i--)
            {
                float sum = 0;
                for (int j = i + 1; j < n; j++)
                {
                    sum += a[i][j] * x[j];
                }
                x[i] = (b[i] - sum) / a[i][i];
            }

            return x;
        }
        public float kvadrat(float xi, float[] x, float[] fx, int step) //рассчет сглаживающего многочлена
        {
            int n = x.Length;
            float[] c = new float[2 * step + 1]; float[] d = new float[step + 1];
            float[][] a = new float[step + 1][];
            float[] b = new float[step + 1];
            float[] ai;
            for (int m = 0; m <= 2 * step; m++)
            {
                for (int k = 0; k < n; k++)
                {
                    c[m] += (float)Math.Pow(x[k], m); //рассчитываем с
                    if (m <= step)
                    {
                        d[m] += fx[k] * (float)Math.Pow(x[k], m); //рассчитываем д
                    }
                }
            }

            for (int i = 0; i <= step; i++)
            {
                a[i] = new float[step + 1]; //Выделяем память для внутреннего массива a[i]
                for (int j = 0; j <= step; j++)
                {
                    a[i][j] = c[i + j]; //заполняем матрицу а нужными значениями с
                }

                b[i] = d[i]; //заполняем б значениями д

            }

            ai = gaus(a, b); //находим коэффициенты многочлена
            float sum = ai[0];
            for (int i = 1; i <= step; i++) //подставляем значения х в многочлен м возвращаем значение функции от х
            {
                float l = 1;
                for (int j = 1; j <= i; j++)
                {
                    l *= xi;
                }
                sum += ai[i] * l;
            }
            return sum;

        }

        public float lagrange(float xi, float[] x, float[] fx) //рассчет интерпол многолчена лагранжа
        {
            float sum = 0;
            int n = x.Length;
            for (int i = 0; i < n; i++) //подставляем значения х в многочлен
            {
                float l = 1;
                for (int j = 0; j < n; j++)
                {
                    if (j != i)
                    {
                        l *= (xi - x[j]) / (x[i] - x[j]);
                    }
                }
                sum += l * fx[i];
            }

            return sum;
        }

        static float newton(float xi, float[] x, float[] fx) //расчет интерпол многочлена ньютона
        {
            int n = x.Length;
            float sum = fx[0];
            for (int i = 1; i < n; ++i) //подставляем значения х в многочлен
            {

                float F = 0;
                for (int j = 0; j <= i; ++j)
                {

                    float den = 1;
                    for (int k = 0; k <= i; ++k)
                        if (k != j)
                            den *= (x[j] - x[k]);
                    F += fx[j] / den;
                }

                for (int k = 0; k < i; ++k)
                    F *= (xi - x[k]);
                sum += F;
            }
            return sum;
        }

        public float sgl3(float x) //сглаживающий многочлен рассчитанный вручную (нет)
        {
            return (34.176F - 35.279F * x + 10.235F * x * x - 0.838F * x * x * x);
        }

        public float sgl2(float x)
        {
            return (-19.318F + 9.261F * x - 0.852F * x * x);
        }

        public float sgl1(float x)
        {
            return (-4.651F + 1.511F * x);
        }

        public float system(float x)
        {
            return (251.2F - 272.083F * x + 99.292F * x * x - 14.666F * x * x * x + 0.758F * x * x * x * x);
        }

        public float step4(float x, float[] a)
        {
            return (a[0] * x * x * x * x + a[1] * x * x * x + a[2] * x * x + a[3] * x + a[4]);
        }

        public ChartForm(List<int> type, float[] x, float[] fx, float[] a, float[] interval, float[] interval2)
        {
            this.Size = new Size(800, 600);
            //создаем элемент Chart
            Chart myChart = new Chart();
            //кладем его на форму и растягиваем на все окно.
            
            myChart.Parent = this;
            myChart.Dock = DockStyle.Fill;
            //добавляем в Chart область для рисования графиков
            myChart.ChartAreas.Add(new ChartArea("Math functions"));
            //Создаем и настраиваем набор точек для рисования графика
            myChart.Legends.Add(new Legend("MyLegend"));

            Series lagran = new Series("Lagrange");
            lagran.ChartType = SeriesChartType.Line;
            lagran.ChartArea = "Math functions";
            lagran.LegendText = "Интерполяционный многочлен Лагранжа";
            lagran.BorderWidth = 12;

            Series newt = new Series("Newton");
            newt.ChartType = SeriesChartType.Line;
            newt.ChartArea = "Math functions";
            newt.Color = Color.Red;
            newt.LegendText = "Интерполяционный многочлен Ньютона";
            newt.BorderWidth = 8;

            Series step1 = new Series("Step1");
            step1.ChartType = SeriesChartType.Line;
            step1.ChartArea = "Math functions";
            step1.Color = Color.Green;
            step1.LegendText = "Сглаживающий многочлен 1 степени";
            step1.BorderWidth = 12;

            Series step2 = new Series("Step2");
            step2.ChartType = SeriesChartType.Line;
            step2.ChartArea = "Math functions";
            step2.Color = Color.Gray;
            step2.LegendText = "Сглаживающий многочлен 2 степени";
            step2.BorderWidth = 8;

            Series step3 = new Series("Step3");
            step3.ChartType = SeriesChartType.Line;
            step3.ChartArea = "Math functions";
            step3.Color = Color.Pink;
            step3.LegendText = "Сглаживающий многочлен 3 степени";
            step3.BorderWidth = 8;

            Series step3_h = new Series("Step3_h");
            step3_h.ChartType = SeriesChartType.Line;
            step3_h.ChartArea = "Math functions";
            step3_h.Color = Color.Red;
            step3_h.LegendText = "Сглаживающий многочлен 3 степени (вручную)";
            step3_h.BorderWidth = 2;

            Series step2_h = new Series("Step2_h");
            step2_h.ChartType = SeriesChartType.Line;
            step2_h.ChartArea = "Math functions";
            step2_h.Color = Color.DarkSlateGray;
            step2_h.LegendText = "Сглаживающий многочлен 2 степени (вручную)";
            step2_h.BorderWidth = 2;

            Series step1_h = new Series("Step1_h");
            step1_h.ChartType = SeriesChartType.Line;
            step1_h.ChartArea = "Math functions";
            step1_h.Color = Color.HotPink;
            step1_h.LegendText = "Сглаживающий многочлен 1 степени (вручную)";
            step1_h.BorderWidth = 6;

            Series syst = new Series("System");
            syst.ChartType = SeriesChartType.Line;
            syst.ChartArea = "Math functions";
            syst.Color = Color.Black;
            syst.LegendText = "Интерполяционный многочлен (по системе)";
            syst.BorderWidth = 2;

            Series step_4 = new Series("Step4");
            step_4.ChartType = SeriesChartType.Line;
            step_4.ChartArea = "Math functions";
            step_4.Color = Color.Black;
            step_4.LegendText = "Произвольный многочлен 4 степени";
            step_4.BorderWidth = 2;


            foreach (int i in type)
            {
                for (float xi = interval2[0]; xi < interval2[1]; xi += 0.01F) //подставляем х в нужную функцию и строим график
                {
                    if (i == 0)
                    {
                        lagran.Points.AddXY(xi, lagrange(xi, x, fx)); //строим многочлен лагранжа
                        
                    }
                    if (i == 1)
                    {
                        newt.Points.AddXY(xi, newton(xi, x, fx)); //строим многочлен ньютона
                    }
                    if (i == 2)
                    {
                        step3.Points.AddXY(xi, kvadrat(xi, x, fx, 3)); //строим сглаживающий многочлен 
                    }
                    if (i == 3)
                    {
                        step2.Points.AddXY(xi, kvadrat(xi, x, fx, 2)); //строим сглаживающий многочлен 
                    }
                    if (i == 4)
                    {
                        step1.Points.AddXY(xi, kvadrat(xi, x, fx, 1)); //строим сглаживающий многочлен 
                    }
                    if (i == 6)
                    {
                        step3_h.Points.AddXY(xi, sgl3(xi)); //строим сглаживающий многочлен 
                    }
                    if (i == 7)
                    {
                        step2_h.Points.AddXY(xi, sgl2(xi)); //строим сглаживающий многочлен 
                    }
                    if (i == 8)
                    {
                        step1_h.Points.AddXY(xi, sgl1(xi)); //строим сглаживающий многочлен 
                    }
                    if (i == 9)
                    {
                        syst.Points.AddXY(xi, system(xi)); //строим сглаживающий многочлен 
                    }

                    
                }
            
                if (i == 5)
                {
                    for(float xi = interval[0]; xi < interval[1]; xi+= 0.01F) 
                    {
                        step_4.Points.AddXY(xi, step4(xi, a)); //строим сглаживающий многочлен 
                    }
                    
                }

                if (i == 0) myChart.Series.Add(lagran);
                if (i == 1) myChart.Series.Add(newt);
                if (i == 2) myChart.Series.Add(step3);
                if (i == 3) myChart.Series.Add(step2);
                if (i == 4) myChart.Series.Add(step1);
                if (i == 5) myChart.Series.Add(step_4);
                if (i == 6) myChart.Series.Add(step3_h);
                if (i == 7) myChart.Series.Add(step2_h);
                if (i == 8) myChart.Series.Add(step1_h);
                if (i == 9) myChart.Series.Add(syst);
                
            }
            Series points = new Series("Точки");
            points.ChartType = SeriesChartType.Point; 
            Color selectedColor = Color.Black;
            points.MarkerStyle = MarkerStyle.Circle;
            points.MarkerSize = 16; points.MarkerColor = Color.Black;
            for (int j = 0; j < x.Length; j++)
            {
                float xi = x[j];
                float yi = fx[j];
                
                //points.IsValueShownAsLabel = true;
                points.ToolTip = "X = #VALX, Y = #VALY";
                points.Points.AddXY(xi, yi);

            } 
            myChart.Series.Add(points);
        }
    }
}
