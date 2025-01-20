using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace cars
{
    public partial class Form1 : Form
    {
        public bool pressA;
        public bool pressD;
        public int kadr;
        public Bitmap screen;
        public Graphics screenG;
        public Car playerCar;
        public List<Car> cars;

        public Form1()
        {
            InitializeComponent();
            ShowStartWindow();
        }

        private void ShowStartWindow()
        {
            // Открытие начального окна
            Form startForm = new Form
            {
                Text = "StreetMax",
                Size = new Size(400, 427),
                StartPosition = FormStartPosition.CenterScreen,
                BackgroundImage = Image.FromFile("m5m.jpg"), // Установите путь к изображению
                BackgroundImageLayout = ImageLayout.Stretch // Растягиваем изображение по размеру формы
            };

            // Создание кнопки "Играть"
            Button playButton = new Button
            {
                Text = "Играть",
                Size = new Size(100, 40),
                Location = new Point(145, 110), // Позиция кнопки
                BackColor = Color.FromArgb(200, 255, 255, 255), // Полупрозрачный фон
                FlatStyle = FlatStyle.Flat, // Убираем обводку
                Cursor = Cursors.Hand // Устанавливаем указатель мыши на "руку"
            };
            playButton.Click += (s, e) =>
            {
                startForm.Close();
                StartGame();
            };

            // Создание кнопки "Правила игры"
            Button rulesButton = new Button
            {
                Text = "Правила игры",
                Size = new Size(100, 40),
                Location = new Point(145, 175), // Позиция кнопки
                BackColor = Color.FromArgb(200, 255, 255, 255), // Полупрозрачный фон
                FlatStyle = FlatStyle.Flat, // Убираем обводку
                Cursor = Cursors.Hand // Устанавливаем указатель мыши на "руку"
            };
            
            rulesButton.Click += (s, e) =>
            {
                MessageBox.Show("Управление:\n- Нажмите 'A', чтобы двигаться влево.\n- Нажмите 'D', чтобы двигаться вправо.\nЦель: Избегайте столкновений с другими машинами!", "Правила игры");
            };
            Button Close = new Button
            {
                Text = "Выйти из игры",
                Size = new Size(100, 40),
                Location = new Point(145, 240), // Позиция кнопки
                BackColor = Color.FromArgb(200, 255, 255, 255), // Полупрозрачный фон
                FlatStyle = FlatStyle.Flat, // Убираем обводку
                Cursor = Cursors.Hand // Устанавливаем указатель мыши на "руку"
            };
            Close.Click += (s, e) =>
            {
                startForm.Close(); // Закрываем стартовую форму
                Application.Exit(); // Закрываем всё приложение

            };
            // Добавление кнопок на форму
            startForm.Controls.Add(Close);
            startForm.Controls.Add(playButton);
            startForm.Controls.Add(rulesButton);

            // Показ формы
            startForm.ShowDialog();
        }







            private void StartGame()
        {
            // Инициализация объектов и ресурсов игры
            pressA = false;
            pressD = false;
            kadr = 0;
            StartPosition = FormStartPosition.CenterScreen;
            screen = new Bitmap(500, 500);
            screenG = Graphics.FromImage(screen);

            // Инициализация игровых объектов после запуска
            try
            {

                playerCar = new Car(250, 400, Pics.textureRandCar);
                cars = new List<Car>();
                pictureBox1.Image = screen;
                timer1.Start(); // Запускаем таймер игры
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
                Close(); // Закрываем приложение, если текстуры не загружены
            }
        }

        private void ShowGameOverWindow()
        {
            timer1.Stop(); // Остановка таймера игры

            DialogResult result = MessageBox.Show("                 Вы врезались!\n                Начать заново?", "Игра окончена", MessageBoxButtons.YesNo);
            
            if (result == DialogResult.Yes)
            {
                StartGame(); // Перезапуск игры
            }
            else
            {
                Close(); // Закрытие приложения
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (playerCar == null || cars == null)
            {
                timer1.Stop();
                return;
            }

            this.Text = kadr.ToString();
            kadr++;
            pictureBox1.Invalidate();
            screenG.DrawImage(Pics.textureRoad, 0, kadr * 5 % 500 - 500);
            screenG.DrawImage(Pics.textureRoad, 0, kadr * 5 % 500 - 0);
            screenG.DrawImage(playerCar.pic, playerCar.x, playerCar.y);
            foreach (Car c in cars) screenG.DrawImage(c.pic, c.x, c.y);
            foreach (Car c in cars) c.y += 3;
            for (int i = 0; i < cars.Count; i++) if (cars[i].y > 800) cars.RemoveAt(i);
            if (Rand.GetInt(0, 50) == 45) cars.Add(new Car(Rand.GetInt(0, 300), 10, Pics.textureRandCar));

            foreach (Car c in cars)
            {
                int difX = Math.Abs(playerCar.x - c.x);
                int difY = Math.Abs(playerCar.y - c.y);
                if (difX < 40 && difY < 80)
                {
                    ShowGameOverWindow(); 
                }
            }
            if (pressA) playerCar.x -= 3;
            if (pressD) playerCar.x += 3;
            if (playerCar.x < -3) playerCar.x = -3;
            if (playerCar.x > 440) playerCar.x = 440;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A) pressA = true;
            if (e.KeyCode == Keys.D) pressD = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A) pressA = false;
            if (e.KeyCode == Keys.D) pressD = false;
        }
    }

    public class Car
    {
        public int x;
        public int y;
        public Bitmap pic;

        public Car(int x, int y, Bitmap pic)
        {
            this.x = x;     // Устанавливает начальную координату X автомобиля.
            this.y = y;     // Устанавливает начальную координату Y автомобиля.
            this.pic = pic; // Устанавливает изображение автомобиля.
        }
    }

    public static class Pics
    {
        public static Bitmap textureCar1 = (Bitmap)Image.FromFile(Directory.GetCurrentDirectory() + "//car1.png");
        public static Bitmap textureCar2 = (Bitmap)Image.FromFile(Directory.GetCurrentDirectory() + "//car2.png");
        public static Bitmap textureCar3 = (Bitmap)Image.FromFile(Directory.GetCurrentDirectory() + "//car3.png");
        public static Bitmap textureCar4 = (Bitmap)Image.FromFile(Directory.GetCurrentDirectory() + "//car4.png");
        public static Bitmap textureCar5 = (Bitmap)Image.FromFile(Directory.GetCurrentDirectory() + "//car5.png");
        public static Bitmap textureCar6 = (Bitmap)Image.FromFile(Directory.GetCurrentDirectory() + "//car6.png");
        public static Bitmap textureRoad = (Bitmap)Image.FromFile(Directory.GetCurrentDirectory() + "//doroga.png");

        public static Bitmap textureRandCar
        {
            get
            {
                int r = Rand.GetInt(0, 6);
                return r == 0 ? textureCar1 : r == 1 ? textureCar2 : r == 2 ? textureCar3 : r == 3 ? textureCar4 : r == 4 ? textureCar5 : textureCar6;
            }
        }
    }

    public static class Rand
    {
        public static Random rand = new Random();

        public static int GetInt(int min, int max)
        {
            return rand.Next(min, max);
        }
    }
}
