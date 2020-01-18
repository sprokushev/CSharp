using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace PSVClassLibrary
{
    /// <summary>Абстраткный класс игры "Крестики-Нолики"</summary>
    /// <seealso cref="PSVClassLibrary.TicTacToeGameConsole"/>
    /// <seealso cref="PSVClassLibrary.TicTacToeGameDesktop"/>
    /// <seealso cref="PSVClassLibrary.TicTacToeGameWeb"/>
    /// <seealso cref="PSVClassLibrary.TicTacToeGameTelegram"/>
    public abstract class TicTacToeGame
    {

        // ИГРОКИ

        /// <summary>Варианты игроков</summary>
        public enum PlayerTypes
        {
            /// <summary>Человек</summary>
            Human,
            /// <summary>Компьютер</summary>
            Machine
        }

        /// <summary>Класс "Игрок"</summary>
        public class Player
        {
            /// <summary>Тип игрока</summary>
            public PlayerTypes PlayerType;
            /// <summary>Чем игрок играет (Крестик или Нолик)</summary>
            public CellValues CellValue;
            /// <summary>  Номер игрока по порядку</summary>
            public int Order;
            /// <summary>Список возможных победных вариантов для следущего хода</summary>
            public List<Cell[]> Variants;
            /// <summary>Рекомендация на следующий ход</summary>
            public Cell NextCell;
            /// <summary>История ходов игрока в текущей игре</summary>
            public List<Step> History;
            /// <summary>Результат игры для данного игрока</summary>
            public ResultTypes Result;

            /// <summary>Инициализация экземпляра класса <see cref="Player"/></summary>
            /// <param name="order">Номер игрока по порядку</param>
            /// <param name="playerType">Тип игрока</param>
            /// <param name="cellValue">Чем играет игрок (Крестик или Нолик)</param>
            public Player(int order, PlayerTypes playerType, CellValues cellValue)
            {
                this.PlayerType = playerType;
                this.CellValue = cellValue;
                this.Order = order;
                this.Result = ResultTypes.Play;
                History = new List<Step>();
                Variants = null;
                NextCell = null;
            }

            public override string ToString()
            {
                return PlayerType==PlayerTypes.Human ? "ЧЕЛОВЕК" : "КОМПЬЮТЕР";
            }
        }

        /// <summary>Кол-во игроков в игре</summary>
        const int CountPlayers = 2;

        /// <summary>Игроки</summary>
        public Player[] Players { get; protected set; } = new Player[CountPlayers];

        /// <summary>Текущий игрок</summary>
        public int CurrentPlayer { get; protected set; }


        /// <summary>Инициализация игроков при старте игры</summary>
        /// <param name="startCellType">Чем ходит первый игрок (по умолчанию "Крестик")</param>
        /// <param name="playerTypes">Типы игроков - по порядку</param>
        protected void InitPlayers(CellValues startCellType, params PlayerTypes[] playerTypes)
        {

            if (playerTypes == null)
            {
                Players[0] = new Player(order: 1, PlayerTypes.Human, startCellType);
                Players[1] = new Player(order: 2, PlayerTypes.Machine, startCellType == CellValues.Cross ? CellValues.Zero : CellValues.Cross);
            }
            else if (playerTypes.Length == 1)
            {
                Players[0] = new Player(order: 1, playerTypes[0], startCellType);
                Players[1] = new Player(order: 2, PlayerTypes.Machine, startCellType == CellValues.Cross ? CellValues.Zero : CellValues.Cross);
            }
            else if (playerTypes.Length == 2)
            {
                Players[0] = new Player(order: 1, playerTypes[0], startCellType);
                Players[1] = new Player(order: 2, playerTypes[1], startCellType == CellValues.Cross ? CellValues.Zero : CellValues.Cross);
            }
            else
            {
                throw new ArgumentException("В текущую реализацию игры могут играть только 2 игрока!");
            };

        }




        // ИГРОВОЕ ПОЛЕ

        /// <summary>Варианты значений ячеек</summary>
        public enum CellValues
        {
            /// <summary>Крестик</summary>
            Cross = -1,
            /// <summary>Пусто</summary>
            Empty = 0,
            /// <summary>Нолик</summary>
            Zero = 1
        }

        /// <summary>Класс "Ячейка игрового поля"</summary>
        public class Cell 
        {
            /// <summary>Размерность игрового поля</summary>
            private int Dimension;
            /// <summary>Строка</summary>
            public int Line;
            /// <summary>Колонка</summary>
            public int Column;
            /// <summary>Значение в ячейке (Крестик, Нолик или пустое)</summary>
            public CellValues CellValue;
            /// <summary>Свойство NumCell собирает номер ячейки на игровом поле из Line и Column. В случае присоения значения свойству NumCell - соответствующими значениями заполняются Line и Column</summary>
            public int NumCell
            {
                get { return Line * Dimension + Column + 1; }
                set { Line = (value - 1) / this.Dimension; Column = (value - 1) % this.Dimension; }
            }

            /// <summary>Инициализация класса <see cref="Step"/></summary>
            /// <param name="dimension">Размерность игрового поля</param>
            /// <param name="numCell">Номер ячейки на поле</param>
            /// <param name="cellValue">Тип заполнения (Крестик или Нолик)</param>
            public Cell(int dimension, int numCell, CellValues cellValue = CellValues.Empty)
            {
                this.Dimension = dimension;
                this.NumCell = numCell;
                this.CellValue = cellValue;
            }
            /// <summary>Инициализация класса <see cref="Step"/></summary>
            /// <param name="dimension">Размерность игрового поля</param>
            /// <param name="line">Номер строки на поле</param>
            /// <param name="column">Номер колонки на поле</param>
            /// <param name="cellValue">  Тип заполнения (Крестик или Нолик)</param>
            public Cell(int dimension, int line, int column, CellValues cellValue = CellValues.Empty)
            {
                this.Dimension = dimension;
                this.Line = line;
                this.Column = column;
                this.CellValue = cellValue;
            }

            /// <summary>Переопределение Cell.ToString</summary>
            public override string ToString() => CellAsString(CellValue);

            /// <summary>Текстовое отображение значения ячейки</summary>
            /// <param name="cellValue">The cell value.</param>
            /// <returns></returns>
            public static string CellAsString(CellValues cellValue)
            {
                switch (cellValue)
                {
                    case CellValues.Cross:
                        return "X";
                        break;
                    case CellValues.Empty:
                        return " ";
                        break;
                    case CellValues.Zero:
                        return "O";
                        break;
                    default:
                        return " ";
                        break;
                }
            }

        }


        /// <summary>Класс "Игровое поле"</summary>
        public class TableGame
        {
            /// <summary>Размерность игрового поля</summary>
            public int Dimension;

            /// <summary>Минимальный номер ячейки, разрешенный для хода игрока</summary>
            public int MinNumCell;

            /// <summary>Максимальный номер ячейки, разрешенный для хода игрока</summary>
            public int MaxNumCell;

            /// <summary>  Ячейки игрового поля</summary>
            public Cell[,] Cells;

            /// <summary>Найти ячейку по номеру, вернуть ссылку на эту ячейку</summary>
            /// <param name="numCell">Номер ячейки</param>
            /// <returns></returns>
            public Cell CellByNum(int numCell)
            {
                for (int i=0; i<Dimension; i++)
                {
                    for (int j=0; j<Dimension; j++)
                    {
                        if (Cells[i, j].NumCell == numCell)
                            return Cells[i, j];
                    }
                }
                return null;
            }



            public TableGame(int dimension)
            {
                this.Dimension = dimension;
                this.MinNumCell = 1;
                this.MaxNumCell = this.Dimension * this.Dimension;

                //Инициализация игрового поля    
                Cells = new Cell[this.Dimension, this.Dimension];
                for (int i = 0; i < this.Dimension; i++)
                {
                    for (int j = 0; j < this.Dimension; j++)
                    {
                        Cells[i, j] = new Cell(this.Dimension, i, j, CellValues.Empty);
                    }
                }

            }

            /// <summary>Варианты отображения TableGame.ToString()</summary>
            public enum TableDraw
            {
                /// <summary>пустое игровое поле</summary>
                Empty,
                /// <summary>Пронумерованное игровое поле</summary>
                Numbers,
                /// <summary>Текущая игровая ситуация</summary>
                Values
            }

            /// <summary>Текстовое отображение игрового поля</summary>
            public override string ToString()
            {
                return this.ToString(TableDraw.Values);
            }

            public new string ToString(TableDraw tableDraw)
            {
                string res = "";
                res += "—————————————" + Environment.NewLine;
                for (int i = 0; i < Dimension; i++)
                {
                    res += "|";
                    for (int j = 0; j < Dimension; j++)
                    {
                        res += " ";
                        switch (tableDraw)
                        {
                            case TableDraw.Empty:
                                res += " ";
                                break;
                            case TableDraw.Numbers:
                                res += Cells[i, j].NumCell;
                                break;
                            case TableDraw.Values:
                                res += Cells[i, j];
                                break;
                            default:
                                res += " ";
                                break;
                        };
                        res+=" |";
                    }
                    res += Environment.NewLine;
                    res += "—————————————" + Environment.NewLine;
                }
                return res;
            }


        }
        /// <summary>Текущее игровое поле</summary>
        public TableGame Table { get; protected set; }



             




        // ХОДЫ ИГРОКА

        /// <summary>класс "Ход игрока"</summary>
        public class Step
        {
            /// <summary>Ячейка на игровом поле</summary>
            public Cell Cell;
            /// <summary>Результат хода</summary>
            public ResultTypes Result;

            /// <summary>Инициализация класса <see cref="Step"/></summary>
            /// <param name="dimension">Размерность игрового поля</param>
            /// <param name="numCell">Номер ячейки на поле</param>
            /// <param name="cellValue">Тип заполнения (Крестик или Нолик)</param>
            public Step(int dimension, int numCell, CellValues cellValue)
            {
                this.Cell = new Cell(dimension, numCell, cellValue);
                this.Result = ResultTypes.Play;
            }

        }


        /// <summary>  Максимальная длина победной линии</summary>
        /// <value>The maximum victory line.</value>
        public int MaxVictoryLine { get; protected set; }

        /// <summary>Победные варианты для следующего хода</summary>
        public List<Cell[]> VictoryVariants { get; protected set; }

        /// <summary>Инициализация коллекции победных вариантов</summary>
        protected virtual void InitVictoryVariants()
        {
            int i;
            int j;

            VictoryVariants = new List<Cell[]>();
            Cell[] cell;

            for (i = 0; i < Table.Dimension; i++) 
            {
                //добавляем строки
                cell = new Cell[Table.Dimension];
                for (j = 0; j < Table.Dimension; j++)
                {
                    cell[j] = new Cell(Table.Dimension, i, j);
                }
                VictoryVariants.Add(cell);


                //добавляем столбцы
                cell = new Cell[Table.Dimension];
                for (j = 0; j < Table.Dimension; j++)
                {
                    cell[j] = new Cell(Table.Dimension, j, i);
                }
                VictoryVariants.Add(cell);

            }

            //добавляем дигонали
            cell = new Cell[Table.Dimension];
            j = 0;
            for (i = 0; i < Table.Dimension; i++)
            {
                cell[i] = new Cell(Table.Dimension, i, j);
                j++;
            }
            VictoryVariants.Add(cell);

            cell = new Cell[Table.Dimension];
            j = Table.Dimension-1;
            for (i = 0; i < Table.Dimension; i++)
            {
                cell[i] = new Cell(Table.Dimension, i, j);
                j--;
            }
            VictoryVariants.Add(cell);
            
        }



        // ИГРА

        /// <summary>Варианты состояний игры</summary>
        public enum StatusTypes
        {
            /// <summary>Старт игры</summary>
            Start,
            /// <summary>Рекомендации для игрока</summary>
            Recomend,
            /// <summary>Ход игрока</summary>
            Turn,
            /// <summary>Завершение игры</summary>
            Finish
        }

        /// <summary>Варианты результатов хода</summary>
        public enum ResultTypes
        {
            /// <summary>Продолжаем играть</summary>
            Play,
            /// <summary>Игра завершилась без победы кого-либо</summary>
            NoVictory,
            /// <summary>Игра завершилась победой игрока</summary>
            Victory,
            /// <summary>Игра завершилась поражением игрока</summary>
            Loss
        }

        /// <summary>Текущий статус игры</summary>
        public StatusTypes Status { get; protected set; }

        /// <summary>Конструктор экземпляра класса <see cref="TicTacToeGame"/></summary>
        /// <param name="dimension">Размерность игрового поля (по умолчанию = 3)</param>
        /// <param name="startCellType">Чем ходит первый игрок (по умолчанию "Крестик")</param>
        /// <param name="playerTypes">Типы игроков - по порядку</param>
        public TicTacToeGame(int dimension = 3, CellValues startCellType = CellValues.Cross, int maxVictoryLine=3, params PlayerTypes[] playerTypes)
        {
            //Инициализация игрового поля    
            Table = new TableGame(dimension);

            // Генерация победных вариантов
            MaxVictoryLine = maxVictoryLine;
            InitVictoryVariants();

            // Инициализация игроков
            InitPlayers(startCellType, playerTypes);
            CurrentPlayer = 0;

            // Первоначальный вывод на экран
            Status = StatusTypes.Start;
            Draw();
        }


        /// <summary>Начать игру</summary>
        public void Start()
        {
            ResultTypes result;

            do
            {

                for (int i = 0; i < CountPlayers; i++)
                {
                    CurrentPlayer = i;

                    // Запрос рекомендации
                    Status = StatusTypes.Recomend;
                    Recomendation(Players[i]);
                    if (Players[i].NextCell==null)
                    {
                        // если отсутствует победный вариант, значит никто не может выиграть, завершаем игру!
                        Players[i].Result = ResultTypes.NoVictory;
                        Players[i == 0 ? 1 : 0].Result = ResultTypes.NoVictory;
                        break;
                    }
                    else Draw();

                    // Ход игрока
                    Status = StatusTypes.Turn;
                    result = Turn(Players[i]);
                    Draw();
                    // Проверка результата хода
                    if (result == ResultTypes.Play)
                    {
                        // продолжаем играть, ход переходит к следующему игроку
                    }
                    else if (result == ResultTypes.Victory)
                    {
                        // текущий игрок выиграл, завершаем игру
                        Players[i].Result = ResultTypes.Victory;
                        Players[i == 0 ? 1 : 0].Result = ResultTypes.Loss;
                        break;
                    }
                    else if (result == ResultTypes.NoVictory)
                    {
                        // никто не выиграл, завершаем игру
                        Players[i].Result = ResultTypes.NoVictory;
                        Players[i == 0 ? 1 : 0].Result = ResultTypes.NoVictory;
                        break;
                    }
                    else
                    {
                        // текущий игрок проиграл, завершаем игру
                        Players[i].Result = ResultTypes.Loss;
                        Players[i == 0 ? 1 : 0].Result = ResultTypes.Victory;
                        break;
                    }
                }

                if (Players[CurrentPlayer].Result != ResultTypes.Play)
                    break;

            } while (true);

            Status = StatusTypes.Finish;
            Draw();
        }


        /// <summary>Генерация для игрока рекомендаций на следующий ход</summary>
        /// <param name="player">Экземпляр класса Player</param>
        protected void Recomendation(Player player)
        {
            // Обнуляем список рекомендованных вариантов
            player.Variants = new List<Cell[]>();
            player.NextCell = null;

            // Исходя из текущей игровой ситуации выделим возможные победные варианты
            bool add;

            foreach (var vv in VictoryVariants)
            {
                // перебираем победные варианты
                if ((vv != null) && (vv.Length > 0))
                {
                    add = true;

                    var excludeCells = from c in vv where Table.CellByNum(c.NumCell).CellValue != CellValues.Empty && Table.CellByNum(c.NumCell).CellValue != player.CellValue select c;

                    foreach (var c in excludeCells)
                    {
                        add = false;
                        break;
                    }

                    if (add)
                        // если для данного варианта на игровом поле все ячейки либо пустые, либо со значением текущего игрока - добавляем вариант в коллекцию возможных вариантов для текущего игрока
                        player.Variants.Add(vv);
                }

            }


            // По умолчанию из возможных победных вариантов выберим первый и вернем первую ссылку на пустую ячейку
            foreach (var vv in player.Variants)
            {
                var emptyCells = from c in vv where Table.CellByNum(c.NumCell).CellValue == CellValues.Empty select c;

                foreach (var c in emptyCells)
                {
                    player.NextCell = c;
                    break;
                }

                break;
            }

            // Реализация победной стратегии


        }

        /// <summary>Ход</summary>
        /// <param name="player">Экземпляр класса Player</param>
        protected ResultTypes Turn(Player player)
        {
            Cell next = AskPlayer(player);

            if (next==null) 
                return ResultTypes.Loss;

            Step curStep = new Step(Table.Dimension, next.NumCell, player.CellValue);

            // проставить ход в игровой таблице
            Table.Cells[next.Line, next.Column].CellValue = player.CellValue;

            // проверить результат хода
            curStep.Result = Check(player);

            // сохранить ход в истории
            player.History.Add(curStep);

            return curStep.Result;
        }

        /// <summary>Проверка результатов хода</summary>
        /// <param name="player">Экземпляр класса Player</param>
        /// <returns>
        ///   <para>ResultTypes.Victory - победа игрока</para>
        ///   <para>ResultTypes.Play - продолжаем играть</para>
        ///   <para>ResultTypes.NoVictory - игра завершилась без победы</para>
        ///   <para></para>
        /// </returns>
        protected ResultTypes Check(Player player)
        {
            ResultTypes res = ResultTypes.Play;

            
            // Проверим, есть ли победная ситуация
            CellValues seek;

            foreach (var variants in VictoryVariants)
            {
                if ((variants!=null) && (variants.Length > 0))
                {

                    seek = Table.CellByNum(variants[0].NumCell).CellValue;
                    res = ResultTypes.Victory;
                    foreach (var VictoryCell in variants)
                    {
                        if (Table.Cells[VictoryCell.Line, VictoryCell.Column].CellValue != player.CellValue)
                        {
                            res = ResultTypes.Play;
                            break;
                        }
                    }

                    if (res == ResultTypes.Victory)
                        return res;
                }
            }

            // Проверим наличие пустых ячеек. Если есть - продолжаем играть.
            for (int i = 0; i < Table.Dimension; i++)
            {
                for (int j = 0; j < Table.Dimension; j++)
                {
                    if (Table.Cells[i, j].CellValue == CellValues.Empty)
                    {
                        return ResultTypes.Play;
                        break;
                    }
                }
            }

            return ResultTypes.NoVictory;
        }




        // ВИЗУАЛИЗАЦИЯ

        /// <summary>Запрос следующего хода игрока, возвращает экземпляр Cell</summary>
        /// <param name="player">Экземпляр игрока</param>
        /// <returns></returns>
        protected virtual Cell AskPlayer(Player player) 
        {
            // по умолчанию реализация только для компьютера
            return player.NextCell;
        }


        /// <summary>Отрисовка игрового интерфейса с учетом текущей игровой ситуации</summary>
        protected abstract void Draw();       
        

    }

    /// <summary>Консольная версия игры "Крестики-Нолики"</summary>
    /// <seealso cref="PSVClassLibrary.TicTacToeGame" />
    public class TicTacToeGameConsole : TicTacToeGame
    {

        /// <summary>Конструктор экземпляра класса <see cref="TicTacToeGameConsole"/></summary>
        /// <param name="dimension">Размерность игрового поля (по умолчанию = 3)</param>
        /// <param name="startCellType">Чем ходит первый игрок (по умолчанию "Крестик")</param>
        /// <param name="playerTypes">Типы игроков - по порядку</param>
        public TicTacToeGameConsole(int dimension = 3, CellValues startCellType = CellValues.Cross, int maxVictoryLine=3, params PlayerTypes[] playerTypes) : base(dimension, startCellType, maxVictoryLine, playerTypes)
        { 
        }
        
        protected override Cell AskPlayer(Player player)
        {
            
            Cell cell;


            if (player.PlayerType == PlayerTypes.Human)
            {
                // играет человек
                Console.Write($"Игрок {player.Order} ({Cell.CellAsString(player.CellValue)}), сделайте свой ход: ");
                do
                {
                    int input = 0;

                    if (int.TryParse(Console.ReadLine(), out input) && (input >= Table.MinNumCell) && (input <= Table.MaxNumCell))
                    {
                        cell = Table.CellByNum(input);
                        if (cell.CellValue == CellValues.Empty)
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Ячейка занята, повторите!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверная позиция, повторите!");
                    }

                } while (true);
            }
            else
            {
                // играет компьютер
                // использовать рекомендуемый ход
                cell = player.NextCell;
            }

            return cell;
        }

        protected override void Draw()
        {

            void DrawHelp()
            {
                Console.WriteLine("Координаты клеток:");
                Console.WriteLine(Table.ToString(TableGame.TableDraw.Numbers));
                Console.WriteLine();
            }


            void DrawTable()
            {
                Console.WriteLine("Текущая игровая ситуация:");
                Console.WriteLine(Table);
                Console.WriteLine();
            }


            if (Status == StatusTypes.Start)
            {
                Console.Clear();
                Console.WriteLine("НОВАЯ ИГРА!");
                Console.WriteLine();
                DrawHelp();
                DrawTable();
            }

            if (Status == StatusTypes.Recomend)
            {
                Console.WriteLine($"Рекомендуется следующий ход: {Players[CurrentPlayer].NextCell?.NumCell}");
            }

            if (Status == StatusTypes.Turn)
            {
                Console.Clear();
                DrawHelp();
                DrawTable();
            }

            if (Status == StatusTypes.Finish)
            {
                if (Players[CurrentPlayer].Result == ResultTypes.Victory)
                    Console.WriteLine($"Победил игрок {Players[CurrentPlayer].Order} ({Cell.CellAsString(Players[CurrentPlayer].CellValue)})!");

                if (Players[CurrentPlayer].Result == ResultTypes.NoVictory)
                    Console.WriteLine($"Никто не победил!");
            }
        }

    }

    /// <summary>Декстопная версия игры "Крестики-Нолики"</summary>
    /// <seealso cref="PSVClassLibrary.TicTacToeGame" />
    public class TicTacToeGameDesktop : TicTacToeGame
    {
        public TicTacToeGameDesktop(int dimension = 3, CellValues startCellType = CellValues.Cross, int maxVictoryLine=3, params PlayerTypes[] playerTypes) : base(dimension, startCellType, maxVictoryLine, playerTypes)
        {
        }

        protected override Cell AskPlayer(Player player)
        {
            throw new NotImplementedException();
        }

        protected override void Draw()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>Web-версия игры "Крестики-Нолики"</summary>
    /// <seealso cref="PSVClassLibrary.TicTacToeGame" />
    public class TicTacToeGameWeb : TicTacToeGame
    {
        public TicTacToeGameWeb(int dimension = 3, CellValues startCellType = CellValues.Cross, int maxVictoryLine=3, params PlayerTypes[] playerTypes) : base(dimension, startCellType, maxVictoryLine, playerTypes)
        {
        }

        protected override Cell AskPlayer(Player player)
        {
            throw new NotImplementedException();
        }

        protected override void Draw()
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>Telegram-версия игры "Крестики-Нолики"</summary>
    /// <seealso cref="PSVClassLibrary.TicTacToeGame" />
    public class TicTacToeGameTelegram : TicTacToeGame
    {
        public TicTacToeGameTelegram(int dimension = 3, CellValues startCellType = CellValues.Cross, int maxVictoryLine=3, params PlayerTypes[] playerTypes) : base(dimension, startCellType, maxVictoryLine, playerTypes)
        {
        }

        protected override Cell AskPlayer(Player player)
        {
            throw new NotImplementedException();
        }

        protected override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
