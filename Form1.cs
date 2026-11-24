
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ScientificCalculator
{
    public partial class Form1 : Form
    {
        private TextBox display = new();
        private ListBox history = new();
        private bool degrees = true;
        private bool newInput = true;
        private string pendingOperation = "";
        private double firstNumber = 0;

        public Form1()
        {
            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "Advanced Scientific Calculator";
            Size = new Size(1050, 700);
            MinimumSize = new Size(850, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(18, 20, 30);
            ForeColor = Color.White;

            var main = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(15)
            };

            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 76));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24));
            Controls.Add(main);

            var calculator = new Panel { Dock = DockStyle.Fill };

            display = new TextBox
            {
                Dock = DockStyle.Top,
                Height = 85,
                Font = new Font("Segoe UI", 27, FontStyle.Bold),
                Text = "0",
                TextAlign = HorizontalAlignment.Right,
                ReadOnly = true,
                BackColor = Color.FromArgb(35, 39, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            calculator.Controls.Add(display);

            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Top = 100,
                ColumnCount = 6,
                RowCount = 7,
                Padding = new Padding(0, 10, 0, 0)
            };

            for (int i = 0; i < 6; i++)
                grid.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 100f / 6));

            for (int i = 0; i < 7; i++)
                grid.RowStyles.Add(
                    new RowStyle(SizeType.Percent, 100f / 7));

            string[,] keys =
            {
                { "DEG", "(", ")", "DEL", "AC", "÷" },
                { "sin", "cos", "tan", "√", "x²", "×" },
                { "asin", "acos", "atan", "x³", "xʸ", "−" },
                { "log", "ln", "π", "e", "1/x", "+" },
                { "7", "8", "9", "%", "n!", "−" },
                { "4", "5", "6", "±", "Ans", "+" },
                { "1", "2", "3", "0", ".", "=" }
            };

            for (int r = 0; r < 7; r++)
            {
                for (int c = 0; c < 6; c++)
                {
                    string key = keys[r, c];

                    var btn = new Button
                    {
                        Text = key,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(4),
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI", 13, FontStyle.Bold),
                        BackColor = GetButtonColor(key),
                        ForeColor = Color.White,
                        Cursor = Cursors.Hand
                    };

                    btn.FlatAppearance.BorderSize = 0;
                    btn.Click += (s, e) => HandleInput(key);
                    grid.Controls.Add(btn, c, r);
                }
            }

            calculator.Controls.Add(grid);
            main.Controls.Add(calculator, 0, 0);

            var historyPanel = new Panel { Dock = DockStyle.Fill };

            var historyTitle = new Label
            {
                Text = "CALCULATION HISTORY",
                Dock = DockStyle.Top,
                Height = 45,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.LightBlue,
                TextAlign = ContentAlignment.MiddleCenter
            };

            history = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(35, 39, 55),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };

            var clearHistory = new Button
            {
                Text = "Clear History",
                Dock = DockStyle.Bottom,
                Height = 45,
                BackColor = Color.FromArgb(170, 55, 65),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            clearHistory.Click += (s, e) => history.Items.Clear();

            historyPanel.Controls.Add(history);
            historyPanel.Controls.Add(historyTitle);
            historyPanel.Controls.Add(clearHistory);

            main.Controls.Add(historyPanel, 1, 0);
        }

        private Color GetButtonColor(string key)
        {
            if (key == "=")
                return Color.FromArgb(35, 125, 220);

            if ("÷×−+".Contains(key) && key.Length == 1)
                return Color.FromArgb(100, 75, 180);

            if (key == "AC" || key == "DEL")
                return Color.FromArgb(180, 65, 75);

            if (char.IsDigit(key[0]) || key == ".")
                return Color.FromArgb(48, 54, 70);

            return Color.FromArgb(45, 85, 115);
        }

        private void HandleInput(string key)
        {
            try
            {
                if (key == "AC")
                {
                    display.Text = "0";
                    firstNumber = 0;
                    pendingOperation = "";
                    newInput = true;
                    return;
                }

                if (key == "DEL")
                {
                    if (display.Text.Length > 1)
                        display.Text = display.Text[..^1];
                    else
                        display.Text = "0";
                    return;
                }

                if (key == "DEG")
                {
                    degrees = !degrees;
                    UpdateModeButton();
                    return;
                }

                if (key == "Ans")
                {
                    return;
                }

                if (key == "π" || key == "e")
                {
                    display.Text = key == "π"
                        ? Math.PI.ToString(CultureInfo.InvariantCulture)
                        : Math.E.ToString(CultureInfo.InvariantCulture);
                    newInput = true;
                    return;
                }

                if (key == "(" || key == ")")
                {
                    if (newInput && key == "(")
                        display.Text = "";
                    display.Text += key;
                    newInput = false;
                    return;
                }

                if ("0123456789.".Contains(key) && key.Length == 1)
                {
                    if (newInput || display.Text == "0")
                    {
                        display.Text = key == "." ? "0." : key;
                        newInput = false;
                    }
                    else if (key != "." || !display.Text.Contains('.'))
                    {
                        display.Text += key;
                    }
                    return;
                }

                if (key == "+" || key == "−" ||
                    key == "×" || key == "÷")
                {
                    if (pendingOperation != "" && !newInput)
                        Calculate();

                    firstNumber = ReadDisplay();
                    pendingOperation = key;
                    newInput = true;
                    return;
                }

                if (key == "=")
                {
                    Calculate();
                    return;
                }

                double n = ReadDisplay();
                double result = n;

                switch (key)
                {
                    case "√": result = Math.Sqrt(n); break;
                    case "x²": result = Math.Pow(n, 2); break;
                    case "x³": result = Math.Pow(n, 3); break;
                    case "sin": result = Math.Sin(ToRadians(n)); break;
                    case "cos": result = Math.Cos(ToRadians(n)); break;
                    case "tan":
                        if (Math.Abs(Math.Cos(ToRadians(n))) < 1e-12)
                            throw new Exception("Undefined tangent.");
                        result = Math.Tan(ToRadians(n));
                        break;
                    case "asin":
                        result = FromRadians(Math.Asin(n)); break;
                    case "acos":
                        result = FromRadians(Math.Acos(n)); break;
                    case "atan":
                        result = FromRadians(Math.Atan(n)); break;
                    case "log": result = Math.Log10(n); break;
                    case "ln": result = Math.Log(n); break;
                    case "1/x": result = 1 / n; break;
                    case "%": result = n / 100; break;
                    case "±": result = -n; break;
                    case "n!":
                        result = Factorial(n); break;
                    case "xʸ":
                        firstNumber = n;
                        pendingOperation = "^";
                        newInput = true;
                        return;
                }

                SetResult(result, key + "(" + n + ")");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Calculation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Calculate()
        {
            if (pendingOperation == "")
                return;

            double secondNumber = ReadDisplay();
            double result = 0;

            switch (pendingOperation)
            {
                case "+": result = firstNumber + secondNumber; break;
                case "−": result = firstNumber - secondNumber; break;
                case "×": result = firstNumber * secondNumber; break;
                case "÷":
                    if (secondNumber == 0)
                        throw new Exception("Cannot divide by zero.");
                    result = firstNumber / secondNumber;
                    break;
                case "^": result = Math.Pow(firstNumber, secondNumber); break;
            }

            string expression = firstNumber + " " +
                pendingOperation + " " + secondNumber;

            SetResult(result, expression);

            pendingOperation = "";
            newInput = true;
        }

        private double ReadDisplay()
        {
            if (!double.TryParse(display.Text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture, out double n))
                throw new Exception("Invalid number.");

            return n;
        }

        private void SetResult(double result, string expression)
        {
            if (double.IsNaN(result) || double.IsInfinity(result))
                throw new Exception("Invalid mathematical result.");

            display.Text = result.ToString("G12",
                CultureInfo.InvariantCulture);

            history.Items.Insert(0, expression + " = " + display.Text);
            newInput = true;
        }

        private double ToRadians(double n)
        {
            return degrees ? n * Math.PI / 180 : n;
        }

        private double FromRadians(double n)
        {
            return degrees ? n * 180 / Math.PI : n;
        }

        private double Factorial(double n)
        {
            if (n < 0 || n > 170 || n != Math.Floor(n))
                throw new Exception(
                    "Factorial requires a whole number from 0 to 170.");

            double result = 1;

            for (int i = 2; i <= (int)n; i++)
                result *= i;

            return result;
        }

        private void UpdateModeButton()
        {
            foreach (Control control in Controls)
                FindModeButton(control);
        }

        private void FindModeButton(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Button btn && btn.Text == "DEG")
                    btn.Text = degrees ? "DEG" : "RAD";

                FindModeButton(control);
            }
        }
    }
}
