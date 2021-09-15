using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Программа семейного учёта
namespace FamilyStatistics
{
    class Program
    {
        // Запоминание позиций горизонтальных пунктов
        static int[] horizontalPositions;
        // Запоминание вертикальных пунктов
        static int[] verticalPositions;
        // Последние выбранные вертикальные и горизонтальные
        static int lastHorizontal;
        static int lastVertical;

        // Текущие доходы
        static int[][][] currentIncome = {
            new int[31][], new int[29][], new int[31][],
            new int[30][], new int[31][], new int[30][],
            new int[31][], new int[31][], new int[30][],
            new int[31][], new int[30][], new int[31][]
        };

        // Текущие расходы
        static int[][][] currentCost = {
            new int[31][], new int[29][], new int[31][],
            new int[30][], new int[31][], new int[30][],
            new int[31][], new int[31][], new int[30][],
            new int[31][], new int[30][], new int[31][]
        };

        // Месяцы
        static string[] months = {
                "Январь", "Февраль", "Март",
                "Апрель", "Май", "Июнь",
                "Июль", "Август", "Сентябрь",
                "Октябрь", "Ноябрь", "Декабрь", "Отмена" };

        // Главное меню
        static string[] mainMenu = { "Доходы", "Расходы", "Статистика", "Выход" };
        // Доходы
        static string[] extraMenu1 = { "Зарплата", "Соц. выплаты", "Пенсии", "Подарки", "Бизнес", "Другое", "Назад" };
        // Расходы
        static string[] extraMenu2 =
            {
                "Квартплата", "Коммуналка", "Еда",
                "Подарки",  "Развлечения", "Проезд",
                "Одежда", "Бизнес", "Другое", "Назад"
                };
        // Статистика
        static string[] extraMenu3 = { "Годовая статистика", "Статистика за месяц", "Назад" };

        // Какой тип бюджета сейчас
        static string currentBudgetType;
        // Какое подменю мы выбрали
        static int selType;
        // Какое подменю мы выбрали в строке
        static string selTypeString;

        static void Main(string[] args)
        {
            // Заполняем доходы
            for (int i = 0; i < currentIncome.Length; i++)
            {
                for (int j = 0; j < currentIncome[i].Length; j++)
                {
                    currentIncome[i][j] = new int[currentIncome.Length - 1];
                }
            }
            // Заполняем расходы
            for (int i = 0; i < currentCost.Length; i++)
            {
                for (int j = 0; j < currentCost[i].Length; j++)
                {
                    currentCost[i][j] = new int[currentCost.Length - 1];
                }
            }

            while (true)
            {
                Console.Clear();
                int n1 = SelectHorizontal(5, 2, mainMenu, 1, true);
                // Первое подменю
                switch (n1)
                {
                    case 0:
                        int incExtra = SelectVertical(horizontalPositions[lastHorizontal], 4, extraMenu1);
                        if (incExtra < extraMenu1.Length - 1)
                        {
                            selType = incExtra;
                            selTypeString = extraMenu1[selType];
                            IncomeMenu();
                        }
                        break;
                    case 1:
                        int costExtra = SelectVertical(horizontalPositions[lastHorizontal], 4, extraMenu2);
                        if (costExtra < extraMenu2.Length - 1)
                        {
                            selType = costExtra;
                            selTypeString = extraMenu2[selType];
                            CostsMenu();
                        }
                        break;
                    case 2:
                        int choosedStats = SelectVertical(horizontalPositions[lastHorizontal], 4, extraMenu3);
                        if(choosedStats < extraMenu3.Length - 1)
                        {
                            switch (choosedStats)
                            {
                                case 0:
                                    YearCalculation();
                                    break;
                                case 1:
                                    GeneralCalculation();
                                    break;
                                case 2:
                                    return;
                            }
                        }
                        break;
                    case 3:
                        string[] confirmExit = { "Подтвердить", "Отмена" };
                        int action = SelectVertical(horizontalPositions[lastHorizontal], 4, confirmExit);
                        {
                            switch(action)
                            {
                                case 0:
                                    return;
                                case 1:
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        // Ввод текущего месяца и дня
        static int[] ReadUserData()
        {
            int[] selected = new int[3];

            // Выбор месяца
            Console.Clear();
            Console.SetCursorPosition(5, 1);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Выберите месяц...");
            int selMonth = SelectVertical(5, 2, months);

            if (selMonth == 12)
                return selected = new int[3]{ -1, -1, -1 };

            // Выбор дня
            int selDay = 0;
            while (selDay <= 0 || selDay > DaysInMonth(selMonth))
            {
                Console.Clear();
                Console.SetCursorPosition(5, 2);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("Выбранный месяц: " + months[selMonth]);
                Console.SetCursorPosition(5, 3);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Введите день: ");
                selDay = Convert.ToInt32(Console.ReadLine());
            }

            selected[0] = selMonth;
            selected[1] = selDay;
            selected[2] = selType;

            return selected;
        }

        // Годовой подсчёт
        static void YearCalculation()
        {
            // Доходы
            long totalIncome = 0;
            // Расходы
            long totalCost = 0;
            // Прибыль
            long totalSum = 0;
            // Самый большой доход
            long highestIncome = 0;
            // Самый маленький доход
            long lowestIncome = long.MaxValue;
            // Самый большой расход
            long highestCost = 0;
            // Самый маленький расход
            long lowestCost = long.MaxValue;

            // Подсчитываем общие доходы
            for(int i = 0; i < currentIncome.Length; i++)
            {
                // Подсчёт за все дни
                for (int j = 0; j < currentIncome[i].Length; j++)
                {
                    for (int k = 0; k < currentIncome[i][j].Length; k++)
                    {
                        if (currentIncome[i][j][k] > highestIncome)
                        {
                            highestIncome = currentIncome[i][j][k];
                        }
                        if (currentIncome[i][j][k] < lowestIncome)
                        {
                            lowestIncome = currentIncome[i][j][k];
                        }

                        totalIncome += currentIncome[i][j][k];
                    }
                }
            }

            // Подсчитываем общие расходы
            for (int i = 0; i < currentCost.Length; i++)
            {
                // Подсчёт за все дни
                for (int j = 0; j < currentCost[i].Length; j++)
                {
                    for (int k = 0; k < currentCost[i][j].Length; k++)
                    {
                        if (currentCost[i][j][k] > highestCost)
                        {
                            highestCost = currentCost[i][j][k];
                        }
                        if (currentCost[i][j][k] < lowestCost)
                        {
                            lowestCost = currentCost[i][j][k];
                        }

                        totalCost += currentCost[i][j][k];
                    }
                }
            }

            // Подсчитываем чистую прибыль
            totalSum = totalIncome - totalCost;

            // Считаем тип бюджета
            if (totalSum == 0)
                currentBudgetType = "Сбалансированный";
            if (totalIncome > totalCost)
                currentBudgetType = "Избыточный";
            if (totalCost > totalIncome)
                currentBudgetType = "Дефицитный";

            // Выводим информацию
            Console.Clear();
            Console.SetCursorPosition(5, 1);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Приведена общая статистика за год...");
            // Тип бюджета
            Console.SetCursorPosition(5, 2);
            Console.Write("Годовой бюджет: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(currentBudgetType);
            // Общие доходы
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.SetCursorPosition(5, 3);
            Console.Write("Общий доход за все дни: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(totalIncome);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" руб.");
            // Макс. и мин. доходы
            Console.SetCursorPosition(5, 4);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Самый большой доход: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(highestIncome);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" руб.");
            Console.SetCursorPosition(5, 5);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Самый маленький доход: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(lowestIncome);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" руб.");
            // Общие расходы
            Console.SetCursorPosition(5, 7);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Общий расход за все дни: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(totalCost);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" руб.");
            // Макс. и мин. расходы
            Console.SetCursorPosition(5, 8);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Самый большой расход: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(highestCost);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" руб.");
            Console.SetCursorPosition(5, 9);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Самый маленький расход: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(lowestCost);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" руб.");
            // Общая прибыль
            Console.SetCursorPosition(5, 11);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Общая прибыль за все дни: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(totalSum);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" руб.");
            // Выход
            Console.SetCursorPosition(5, 13);
            Console.WriteLine("Нажмите клавишу 'Enter' для подтверждения...");
            Console.ReadLine();
        }

        // Подробный отсчёт 
        static void GeneralCalculation()
        {
            // Общие показатели за месяц
            int totalMonthIncome = 0;
            int totalMonthCost = 0;
            int totalMonthClearIncome = 0;

            // Выбор месяца
            Console.Clear();
            Console.SetCursorPosition(5, 2);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Выберите месяц...");
            Console.ForegroundColor = ConsoleColor.White;
            int month = SelectVertical(5, 3, months);

            if (month == 12)
                return;

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.SetCursorPosition(5, 1);
            Console.Write("Полные данные за " + months[month] + "...");
            Console.SetCursorPosition(5,2);

            int totalLength = 3;

            for(int i = 0; i < currentIncome[month].Length; i++)
            {
                int dayIncome = 0;
                int dayCost = 0;
                int dayClearIncome = 0;

                // Считаем дневной доход
                for(int d = 0; d < currentIncome[month][i].Length; d++)
                {
                    dayIncome += currentIncome[month][i][d];
                }

                // Считаем дневной расход
                for (int d = 0; d < currentCost[month][i].Length; d++)
                {
                    dayCost += currentCost[month][i][d];
                }

                // Считаем прибыль
                dayClearIncome = dayIncome - dayCost;

                // Прибавляем к общей статистике показатели
                totalMonthIncome += dayIncome;
                totalMonthCost += dayCost;
                totalMonthClearIncome += dayClearIncome;

                totalLength++;
                Console.SetCursorPosition(5, i + totalLength);
                Console.Write(i+1 + " день.");

                // Разделитель
                totalLength++;
                DrawHorizontalLine(5, i + totalLength, 70, 0);
                totalLength++;
                // -------------

                Console.SetCursorPosition(5, i + totalLength);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Доходы: " + dayIncome);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" руб.");
                Console.ForegroundColor = ConsoleColor.DarkGreen;

                // Доходы
                Console.ForegroundColor = ConsoleColor.White;
                for(int c = 0; c < currentIncome[month][i].Length; c++)
                {
                    if (currentIncome[month][i][c] != 0)
                    {
                        totalLength++;
                        Console.SetCursorPosition(5, i + totalLength);
                        Console.Write(extraMenu1[c] + ": " + currentIncome[month][i][c]);
                        Console.Write(" руб.");
                    }
                }
                Console.ForegroundColor = ConsoleColor.White;

                // Разделитель
                totalLength++;
                DrawHorizontalLine(5, i + totalLength, 70, 1);
                totalLength++;
                // -------------
                Console.SetCursorPosition(5, i + totalLength);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Расходы: " + dayCost);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" руб.");
                Console.ForegroundColor = ConsoleColor.DarkGreen;

                // Расходы
                Console.ForegroundColor = ConsoleColor.White;
                for (int c = 0; c < currentCost[month][i].Length; c++)
                {
                    if(currentCost[month][i][c] != 0)
                    {
                        totalLength++;
                        Console.SetCursorPosition(5, i + totalLength);
                        Console.Write(extraMenu2[c] + ": " + currentCost[month][i][c]);
                        Console.Write(" руб.");
                    }
                }
                Console.ForegroundColor = ConsoleColor.White;

                // Считаем тип бюджета
                if (dayClearIncome == 0)
                    currentBudgetType = "Сбалансированный";
                if (dayIncome > dayCost)
                    currentBudgetType = "Избыточный";
                if (dayCost > dayIncome)
                    currentBudgetType = "Дефицитный";

                // Разделитель
                totalLength++;
                DrawHorizontalLine(5, i + totalLength, 70, 1);
                totalLength++;
                // -------------
                Console.SetCursorPosition(5, i + totalLength);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Общая прибыль за день: " + dayClearIncome);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" руб.");
                totalLength++;
                Console.SetCursorPosition(5, i + totalLength);
                Console.Write("Тип дневного бюджета: " + currentBudgetType);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                totalLength++;
                DrawHorizontalLine(5, i + totalLength, 70, 0);
            }

            // Считаем тип месячного бюджета
            if (totalMonthClearIncome == 0)
                currentBudgetType = "Сбалансированный";
            if (totalMonthIncome > totalMonthCost)
                currentBudgetType = "Избыточный";
            if (totalMonthCost > totalMonthIncome)
                currentBudgetType = "Дефицитный";

            totalLength += 33;
            Console.ForegroundColor = ConsoleColor.White;
            DrawHorizontalLine(5, totalLength, 70, 0);
            DrawVerticalLine(75, totalLength + 1, 5, 0);
            DrawVerticalLine(4, totalLength + 1, 5, 0);
            totalLength++;
            Console.SetCursorPosition(5, totalLength);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" Общая статистика за месяц");
            totalLength++;
            Console.SetCursorPosition(5, totalLength);
            Console.WriteLine(" Тип месячного бюджета: " + currentBudgetType);
            totalLength++;
            Console.SetCursorPosition(5, totalLength);
            Console.Write(" Общие доходы за месяц: " + totalMonthIncome);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" руб.");
            totalLength++;
            Console.SetCursorPosition(5, totalLength);
            Console.Write(" Общие расходы за месяц: " + totalMonthCost);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" руб.");
            totalLength++;
            Console.SetCursorPosition(5, totalLength);
            Console.Write(" Общая прибыль за месяц: " + totalMonthClearIncome);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" руб.");
            totalLength++;
            Console.ForegroundColor = ConsoleColor.White;
            DrawHorizontalLine(5, totalLength, 70, 0);
            totalLength += 3;
            Console.SetCursorPosition(5, totalLength);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Нажмите 'Enter' для завершения...");
            Console.ReadLine();
        }

        // Меню доходов
        static void IncomeMenu()
        {
            string[] actions = { "Добавить новое", "Изменить текущее", "Отмена" };

            int[] answer = ReadUserData();
            
            // Возврат в главное меню
            if(answer[0] == -1 && answer[1] == -1)
            {
                return;
            }

            answer[1]--;

            while (true)
            {
                // Информация о происходящем
                Console.Clear();
                Console.SetCursorPosition(5, 2);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Выбранная дата: {0:d2}/{1:d2}/2018", answer[1]+1, months[answer[0]]);
                Console.SetCursorPosition(5, 3);
                Console.WriteLine("Выбранный тип: " + selTypeString);
                Console.SetCursorPosition(5, 4);
                Console.WriteLine("Текущее состояние: " + currentIncome[answer[0]][answer[1]][answer[2]]);
                Console.SetCursorPosition(5, 5);
                Console.ForegroundColor = ConsoleColor.White;
                // Действие с данными
                int action = SelectHorizontal(5, 6, actions, 1, false);
                switch (action)
                {
                    case 0:
                        Console.SetCursorPosition(5, 7);
                        Console.Write("Введите значение для изменения: ");
                        currentIncome[answer[0]][answer[1]][answer[2]] += Convert.ToInt32(Console.ReadLine());
                        break;
                    case 1:
                        Console.SetCursorPosition(5, 7);
                        Console.Write("Введите новое значение: ");
                        currentIncome[answer[0]][answer[1]][answer[2]] = Convert.ToInt32(Console.ReadLine());
                        break;
                    case 2:
                        return;
                }
            }
        }

        // Меню расходов
        static void CostsMenu()
        {
            string[] actions = { "Добавить новое", "Изменить текущее", "Отмена" };

            int[] answer = ReadUserData();

            // Возврат в главное меню
            if (answer[0] == -1 && answer[1] == -1)
            {
                return;
            }

            answer[1]--;

            while (true)
            {
                // Информация о происходящем
                Console.Clear();
                Console.SetCursorPosition(5, 2);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Выбранная дата: {0:d2}/{1:d2}/2018", answer[1]+1, months[answer[0]]);
                Console.SetCursorPosition(5, 3);
                Console.WriteLine("Выбранный тип: " + selTypeString);
                Console.SetCursorPosition(5, 4);
                Console.WriteLine("Текущее состояние: -" + currentCost[answer[0]][answer[1]][answer[2]]);
                Console.SetCursorPosition(5, 5);
                Console.ForegroundColor = ConsoleColor.White;
                // Действие с данными
                int action = SelectHorizontal(5, 6, actions, 1, false);
                switch (action)
                {
                    case 0:
                        Console.SetCursorPosition(5, 7);
                        Console.Write("Введите значение для изменения: ");
                        currentCost[answer[0]][answer[1]][answer[2]] += Convert.ToInt32(Console.ReadLine());
                        break;
                    case 1:
                        Console.SetCursorPosition(5, 7);
                        Console.Write("Введите новое значение: ");
                        currentCost[answer[0]][answer[1]][answer[2]] += Convert.ToInt32(Console.ReadLine());
                        break;
                    case 2:
                        return;
                }
            }
        }

        // Определить сколько дней в месяце
        static int DaysInMonth(int month)
        {
            // Можно if, switch, формулой
            int[] dm = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            return dm[month];
        }

        // Выбор горизонтального меню
        static int SelectHorizontal(int x, int y, string[] mas, int space, bool isSave)
        {
            int var = 0;

            if (isSave)
                var = lastHorizontal;

            while (true)
            {
                if (var < 0)
                    var = mas.Length - 1;
                if (var > mas.Length - 1)
                    var = 0;

                DrawHorizontalMenu(x, y, mas, space, var);

                ConsoleKeyInfo info = Console.ReadKey(true);

                switch (info.Key)
                {
                    case ConsoleKey.LeftArrow:
                        var--;
                        break;
                    case ConsoleKey.RightArrow:
                        var++;
                        break;
                    case ConsoleKey.Enter:
                        if(isSave)
                            lastHorizontal = var;
                        return var;
                }
            }
        }

        // Выбор вертикального меню
        static int SelectVertical(int x, int y, string[] mas)
        { 
            int current = 0;

            while (true)
            {
                if (current < 0)
                    current = mas.Length - 1;
                if (current > mas.Length - 1)
                    current = 0;

                DrawVerticalMenu(x, y, mas, current);

                ConsoleKeyInfo info = Console.ReadKey(true);

                switch(info.Key)
                {
                    case ConsoleKey.UpArrow:
                        current--;
                        break;
                    case ConsoleKey.DownArrow:
                        current++;
                        break;
                    case ConsoleKey.Enter:
                        return current;
                }
            }
        }

        // Прорисовка горизонтального меню
        static void DrawHorizontalMenu(int x, int y, string[] mas, int space, int active)
        {
            horizontalPositions = new int[mas.Length];

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < mas.Length; i++)
            {
                horizontalPositions[i] = x;

                Console.SetCursorPosition(x, y);
                x += mas[i].Length + space;

                Console.Write(mas[i]);
            }
            if (active != -1)
            {
                Console.SetCursorPosition(horizontalPositions[active], y);
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(mas[active]);
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Прорисовка вертикального меню
        static void DrawVerticalMenu(int x, int y, string[] mas, int active)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < mas.Length; i++)
            {
                Console.SetCursorPosition(x, y + i);

                Console.Write(mas[i]);
            }
            Console.SetCursorPosition(x, y + active);
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(mas[active]);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Рисуем горизонтальную линию
        static void DrawHorizontalLine(int x, int y, int length, int sign)
        {
            string text = "";

            switch(sign)
            {
                case 0:
                    text = "═";
                    break;
                case 1:
                    text = "─";
                    break;
            }

            Console.SetCursorPosition(x, y);
            for(int l = 0; l < length; l++)
            {
                Console.Write(text);
            }
        }

        // Рисуем вертикальную линию
        static void DrawVerticalLine(int x, int y, int length, int sign)
        {
            string text = "";

            switch (sign)
            {
                case 0:
                    text = "║";
                    break;
                case 1:
                    text = "│";
                    break;
            }

            for (int l = 0; l < length; l++)
            {
                Console.SetCursorPosition(x, y + l);
                Console.Write(text);
            }
        }
    }
}
