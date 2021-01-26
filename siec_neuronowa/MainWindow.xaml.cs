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
        public static double mikro = 0.1;
        public static double beta = 1;
        public static bool flag = false;

        public static double funkcja(double wyjscie)
        {
            double wynik = 1 / (1 + Math.Pow(Math.E, (-1 * beta * wyjscie)));
            return wynik;
        }

        public class Neuron
        {
            List<double> wagi = new List<double>();
            double ostatnieWyjscie = 0;
            List<double> ostatnieWejscia = new List<double>();
            public Neuron(int ile)
            {
                Random random = new Random();
                for (int i =0; i<ile+1; i++)
                {
                    wagi.Add(random.NextDouble()*2-1);
                }
            }

            public double wyjscie(List<double> wejscia)
            {
                ostatnieWejscia = wejscia;
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

                ostatnieWyjscie = wynik;

                return wynik;
            }

            public List<double> wsteczna(double korekta)
            {
                List<double> wynik = new List<double>();
                korekta = korekta * beta * ostatnieWyjscie * (1 - ostatnieWyjscie);
                
                for (int i = 0; i < wagi.Count-1; i++)
                {
                    wynik.Add(korekta * wagi[i]);
                    wagi[i] = wagi[i] + korekta * ostatnieWejscia[i];
                }
                wagi[wagi.Count - 1] = wagi[wagi.Count - 1] + korekta * 1;

                return wynik;
            }
        }

        public class Warstwa
        {
            List<Neuron> neurony = new List<Neuron>();
            public List<double> ostatnieWyjscia = new List<double>();
            public List<double> ostatnieWejscia = new List<double>();

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
                ostatnieWejscia = wejscia;
                List<double> wynik = new List<double>();
                for (int i=0; i< neurony.Count; i++)
                {
                    wynik.Add(neurony[i].wyjscie(wejscia));
                }
                ostatnieWyjscia = wynik;
                return wynik;
            }

            public List<double> wsteczna(List<double> korektyWyjsc)
            {                
                List<double> bledyWstecz = new List<double>();

                for(int i=0; i< ostatnieWejscia.Count; i++)
                {
                    bledyWstecz.Add(0);
                }

                for(int i=0; i< neurony.Count; i++)
                {
                    List<double> neuronWstecz = neurony[i].wsteczna(korektyWyjsc[i]);
                    for (int j = 0; j < ostatnieWejscia.Count; j++)
                    {
                        bledyWstecz[j] += neuronWstecz[j];
                    }
                }

                return bledyWstecz;
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

            public void wsteczna(List<double> wejscia, List<double> wyjsciaP)
            {
                //wyjsciaP = próbki uczące, wyjsciaN = wyjscia z sieci przed propagacją
                List<double> wyjsciaN = siec.wyjscia(wejscia);
                List<double> korektyWyjsc = new List<double>();
                for (int i = 0; i < wyjsciaP.Count; i++)
                {
                    korektyWyjsc.Add(mikro * (wyjsciaP[i] - wyjsciaN[i]));
                }

                for (int i = warstwy.Count - 1; i>=0; i--)
                {
                    korektyWyjsc = warstwy[i].wsteczna(korektyWyjsc);
                }
            }
        }


        public List<double> probka()
        {
            List<bool> dane = new List<bool>();
            Random rand = new Random();
            dane.Add(rand.Next(2) == 0);
            dane.Add(rand.Next(2) == 0);
            dane.Add(dane[0] ^ dane[1]);
            List<double> wynik = new List<double>();
            for(int i = 0; i < 3; i++)
            {
                wynik.Add(Convert.ToDouble(dane[i]));
            }
            return wynik;
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

        private void UczBTN_Click(object sender, RoutedEventArgs e)
        {
            int iloscPowt = int.Parse(Repeats.Text);
            List<double> wejscia = new List<double>();
            List<double> wyjscia = new List<double>();
            List<double> dane = new List<double>();
            wejscia.Add(0);
            wejscia.Add(0);
            wyjscia.Add(0);

            Random rand = new Random();

            for (int i = 0; i < iloscPowt; i++)
            {
                dane = probka();
                wejscia[0] = dane[0];
                wejscia[1] = dane[1];
                wyjscia[0] = dane[2];
                siec.wsteczna(wejscia, wyjscia);
            }
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

        private void Repeats_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
