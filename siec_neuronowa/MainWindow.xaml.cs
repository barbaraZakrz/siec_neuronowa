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
using System.Globalization;

namespace siec_neuronowa
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        public static Siec siec = new Siec(null);

        public static double funkcja(double wyjscie)
        {
            //tu bedzie funkcja aktywacji 
            return wyjscie;
        }

        public class Neuron
        {
            List<double> wagi = new List<double>();

            public Neuron(int ile)
            {
                Random random = new Random();
                for (int i =0; i<ile+1; i++)
                {
                    wagi.Add(random.NextDouble());
                }
            }

            public double wyjscie(List<double> wejscia)
            {
                if (wejscia.Count != wagi.Count-1)
                {
                    return 0;
                }
                double wynik = 0;

                for (int i = 0; i < wagi.Count-1; i++)
                {
                    wynik += wejscia[i] * wagi[i];
                }

                wynik += 1 * wagi[wagi.Count - 1];

                return wynik;
            }
        }

        public class Warstwa
        {
            List<Neuron> neurony = new List<Neuron>();

            public Warstwa(int ile, int wejscia)
            {
                for (int i =0; i< ile; i++)
                {
                    Neuron neuron = new Neuron(wejscia);
                    neurony.Add(neuron);
                }
            }

            public List<double> wyjscia(List<double> wejscia)
            {
                List<double> wynik = new List<double>();
                for (int i=0; i< neurony.Count; i++)
                {
                    wynik.Add(neurony[i].wyjscie(wejscia));
                }
                return wynik;
            }
        }


        public class Siec
        {
            List<Warstwa> warstwy = new List<Warstwa>();

            public Siec(List<int> ile)
            {
                if (ile != null)
                {
                    for (int i = 0; i < ile.Count - 1; i++)
                    {
                        Warstwa warstwa = new Warstwa(ile[i + 1], ile[i]);
                        warstwy.Add(warstwa);
                    }
                }
            }

            public List<double> wyjscia(List<double> wejscia)
            {
                List<double> wynik = wejscia;
                for (int i =0; i<warstwy.Count; i++)
                {
                    wynik = warstwy[i].wyjscia(wynik);
                }
                return wynik;
            }
        }


        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateBTN_Click(object sender, RoutedEventArgs e)
        {
            string text = Struktura.Text;
            string[] textSplit = text.Split(' ');
            List<int> struktura = new List<int>();
            for (int i = 0; i < (textSplit.Length); i++)
            {
                struktura.Add(int.Parse(textSplit[i]));
            }
            Siec nowaSiec = new Siec(struktura);
            siec = nowaSiec;
        }

        private void RunBTN_Click(object sender, RoutedEventArgs e)
        {
            string text = Inputs.Text;
            string[] textSplit = text.Split(' ');
            List<double> inputs = new List<double>();
            for (int i = 0; i < (textSplit.Length); i++)
            {
                inputs.Add(double.Parse(textSplit[i], CultureInfo.InvariantCulture));
            }
            List<double> outputs = siec.wyjscia(inputs);
            string wyniki = "";
            for (int i = 0; i < outputs.Count; i++)
            {
                wyniki += outputs[i].ToString("0.##");
                wyniki += " "; 
            }
            Outputs.Text = wyniki; 
            
        }

        private void Struktura_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Inputs_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Outputs_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
