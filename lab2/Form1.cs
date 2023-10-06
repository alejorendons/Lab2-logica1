using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace lab2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.panelGrafico2.Paint += new System.Windows.Forms.PaintEventHandler(this.panelGrafico2_Paint);

        }
        /// <summary>
        /// Evento que se dispara cuando el contenido del textBox1 cambia.
        /// Valida el contenido y cambia el color del texto según su validez.
        /// </summary>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!validar(textBox1.Text))
            {
                textBox1.ForeColor = Color.Red;
            }
            else
            {
                textBox1.ForeColor = Color.Black;
            }
        }
        /// <summary>
        /// Evento que se dispara cuando el contenido del textBox2 cambia.
        /// Valida el contenido y cambia el color del texto según su validez.
        /// </summary>
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!validar(textBox2.Text))
            {
                textBox2.ForeColor = Color.Red;  // Cambia el color del texto a rojo si la función no es válida
            }
            else
            {
                textBox2.ForeColor = Color.Black;  // Restablece el color del texto si la función es válida
            }
        }
        /// <summary>
        /// Evento que se dispara al hacer clic en el botón de solución.
        /// Compara las dos ecuaciones lineales y muestra el resultado en el control "respuesta".
        /// </summary>
        private void btnSolucion_Click(object sender, EventArgs e)
        {
            panelGrafico2.Invalidate();
            string funcion1 = textBox1.Text.Trim();
            string funcion2 = textBox2.Text.Trim();

            if (!validar(funcion1) || !validar(funcion2))
            {
                respuesta.Text = "Por favor, ingresa funciones válidas.";
                return;
            }

            bool success1 = ExtraerValores(funcion1, out double m1, out double b1);
            bool success2 = ExtraerValores(funcion2, out double m2, out double b2);

            if (!success1 || !success2)
            {
                respuesta.Text = "Las funciones ingresadas no son válidas.";
                return;
            }

            if (m1 == m2)
            {
                respuesta.Text = b1 == b2 ? "Las líneas son idénticas." : "Las líneas son paralelas.";
            }
            else if (m1 * m2 + 1 == 0)
            {
                respuesta.Text = "Las líneas son perpendiculares.";
            }
            else
            {
                double x = (b2 - b1) / (m1 - m2);
                double y = m1 * x + b1;
                respuesta.Text = $"Las líneas se intersectan en: ({x}, {y})";
            }
        }

        /// <summary>
        /// Evento que se dispara al hacer clic en el control "respuesta".
        /// Muestra un mensaje con la relación entre las dos rectas.
        /// </summary>
        private void respuesta_Click(object sender, EventArgs e)
        {            
            double m1, b1, m2, b2;
            bool success1 = ExtraerValores(textBox1.Text, out m1, out b1);
            bool success2 = ExtraerValores(textBox2.Text, out m2, out b2);

            if (!success1 || !success2)
            {
                MessageBox.Show("Por favor, ingrese ecuaciones válidas.");
                return;
            }

            
            if (m1 == m2)
            {
                MessageBox.Show(b1 == b2 ? "Las rectas son idénticas." : "Las rectas son paralelas.");
                return;
            }

            
            double x = (b2 - b1) / (m1 - m2);
            double y = m1 * x + b1;

            
            if (m1 * m2 == -1)
            {
                MessageBox.Show($"Las rectas son perpendiculares y se cruzan en el punto ({x}, {y}).");
                return;
            }

            MessageBox.Show($"Las rectas se cruzan en el punto ({x}, {y}).");
        }
        /// <summary>
        /// Evento que se dispara al dibujar en el panel gráfico.
        /// Dibuja los ejes y las líneas correspondientes a las ecuaciones ingresadas.
        /// </summary>
        private void panelGrafico2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int width = panelGrafico2.Width;
            int height = panelGrafico2.Height;

            // Establece rango y factor de escala
            double xMin = -10;
            double xMax = 10;
            double yMin = -10;
            double yMax = 10;

            double xScale = width / (xMax - xMin);
            double yScale = height / (yMax - yMin);

            // Función para transformar coordenadas matemáticas a coordenadas de píxeles
            PointF ToPixel(double x, double y)
            {
                return new PointF((float)((x - xMin) * xScale), (float)(height - (y - yMin) * yScale));
            }

            // Dibuja los ejes X e Y
            g.DrawLine(Pens.Black, ToPixel(xMin, 0), ToPixel(xMax, 0)); // Eje X
            g.DrawLine(Pens.Black, ToPixel(0, yMin), ToPixel(0, yMax)); // Eje Y

            // Dibuja marcas (ticks) en los ejes
            for (double i = xMin; i <= xMax; i++)
            {
                g.DrawLine(Pens.Black, ToPixel(i, -0.2), ToPixel(i, 0.2)); // Ticks para el eje X
            }

            for (double j = yMin; j <= yMax; j++)
            {
                g.DrawLine(Pens.Black, ToPixel(-0.2, j), ToPixel(0.2, j)); // Ticks para el eje Y
            }

            // El código para graficar las líneas de las ecuaciones
            double m1, b1, m2, b2;
            bool success1 = ExtraerValores(textBox1.Text, out m1, out b1);
            bool success2 = ExtraerValores(textBox2.Text, out m2, out b2);

            if (success1)
            {
                PointF p1 = ToPixel(xMin, m1 * xMin + b1);
                PointF p2 = ToPixel(xMax, m1 * xMax + b1);
                g.DrawLine(Pens.Red, p1, p2);
            }

            if (success2)
            {
                PointF p1 = ToPixel(xMin, m2 * xMin + b2);
                PointF p2 = ToPixel(xMax, m2 * xMax + b2);
                g.DrawLine(Pens.Blue, p1, p2);
            }
        }
        /// <summary>
        /// Extrae los valores m (pendiente) y b (ordenada al origen) de una ecuación lineal.
        /// </summary>
        /// <param name="ecuacion">La ecuación lineal en forma de cadena.</param>
        /// <param name="m">La pendiente extraída.</param>
        /// <param name="b">La ordenada al origen extraída.</param>
        /// <returns>True si la extracción fue exitosa, false en caso contrario.</returns>

        private bool ExtraerValores(string ecuacion, out double m, out double b)
        {
            m = 0;
            b = 0;
            ecuacion = ecuacion.ToLower().Replace(" ", "");

            // Patrón para detectar la forma Y = x
            Match matchSimple = Regex.Match(ecuacion, @"^y=x$");
            Match matchGeneral = Regex.Match(ecuacion, @"y=(-?\d*,?\d*|-?\d+/\d+)x([+\-]\d*,?\d*|-?\d+/\d+)?$");
            Match matchPuntoPendiente = Regex.Match(ecuacion, @"y([+\-]\d*,?\d*|-?\d+/\d+)=(-?\d*,?\d*|-?\d+/\d+)\(x([+\-]\d*,?\d*|-?\d+/\d+)\)$");

            if (matchSimple.Success)
            {
                m = 1;
                b = 0;
                return true;
            }
            else if (matchGeneral.Success)
            {
                m = ConvertirFraccionADecimal(matchGeneral.Groups[1].Value);
                b = matchGeneral.Groups[2].Success ? ConvertirFraccionADecimal(matchGeneral.Groups[2].Value) : 0;
                return true;
            }
            else if (matchPuntoPendiente.Success)
            {
                double y1 = ConvertirFraccionADecimal(matchPuntoPendiente.Groups[1].Value);
                m = ConvertirFraccionADecimal(matchPuntoPendiente.Groups[2].Value);
                double x1 = ConvertirFraccionADecimal(matchPuntoPendiente.Groups[3].Value);
                b = y1 - m * x1;
                return true;
            }

            return false;
        }
        /// <summary>
        /// Convierte una cadena que representa un número (con coma como separador decimal o en forma fraccional) a su valor decimal.
        /// </summary>
        /// <param name="valor">La cadena a convertir.</param>
        /// <returns>El valor decimal correspondiente.</returns>
        private double ConvertirFraccionADecimal(string valor)
        {
            if (valor.Contains("/"))
            {
                var partes = valor.Split('/');
                return double.Parse(partes[0].Replace(',', '.')) / double.Parse(partes[1].Replace(',', '.'));
            }
            return double.Parse(valor.Replace(',', '.'));
        }
        /// <summary>
        /// Valida si una función es una ecuación lineal válida.
        /// </summary>
        /// <param name="funcion">La función a validar.</param>
        /// <returns>True si la función es válida, false en caso contrario.</returns>

        private bool validar(string funcion)
        {
            funcion = funcion.Replace(" ", "");
            // Patrones modificados para incluir comas como separadores decimales
            string patron1 = @"^y=(-?\d*,?\d*|-?\d+/\d+)x([+\-]\d*,?\d*|-?\d+/\d+)?$";
            string patron2 = @"^y([+\-]\d*,?\d*|-?\d+/\d+)=(-?\d*,?\d*|-?\d+/\d+)\(x([+\-]\d*,?\d*|-?\d+/\d+)\)$";

            Regex regex1 = new Regex(patron1, RegexOptions.IgnoreCase);
            Regex regex2 = new Regex(patron2, RegexOptions.IgnoreCase);

            return regex1.IsMatch(funcion) || regex2.IsMatch(funcion);
        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }  
}