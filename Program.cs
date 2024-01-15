using System;

class ReversiGame
{
    static char[,] board = new char[8, 8]; // Игровое поле
    static char currentPlayer = 'X'; // Текущий игрок (X или O)

    static void Main()
    {
        InitializeBoard();
        PrintBoard();

        while (true)
        {
            Console.WriteLine($"Ходит игрок {currentPlayer}");

            if (currentPlayer == 'X')
            {
                // Ход пользователя
                MakePlayerMove();
            }
            else
            {
                // Ход бота
                MakeBotMove();
            }

            PrintBoard();

            // Проверка на конец игры
            if (IsGameOver())
            {
                Console.WriteLine("Игра окончена!");
                PrintScores();
                break;
            }

            // Смена игрока
            currentPlayer = (currentPlayer == 'X') ? 'O' : 'X';
        }
    }

    static void InitializeBoard()
    {
        // Инициализация игрового поля
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                board[i, j] = ' ';
            }
        }

        // Начальные фишки
        board[3, 3] = board[4, 4] = 'X';
        board[3, 4] = board[4, 3] = 'O';
    }

    static void PrintBoard()
    {
        // Вывод игрового поля на консоль
        Console.WriteLine("  a b c d e f g h");
        for (int i = 0; i < 8; i++)
        {
            Console.Write($"{8 - i} ");
            for (int j = 0; j < 8; j++)
            {
                Console.Write($"{board[i, j]} ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    static void MakePlayerMove()
    {
        bool validMove = false;
        do
        {
            Console.Write("Введите ваш ход (например, e3): ");
            string input = Console.ReadLine().ToLower();

            if (input.Length == 2 && input[0] >= 'a' && input[0] <= 'h' && input[1] >= '1' && input[1] <= '8')
            {
                int row = 8 - (input[1] - '0');
                int col = input[0] - 'a';

                // Проверка на корректность хода
                if (IsValidMove(row, col))
                {
                    // Делаем ход
                    MakeMove(row, col);
                    validMove = true;
                }
                else
                {
                    Console.WriteLine("Некорректный ход. Попробуйте еще раз.");
                }
            }
            else
            {
                Console.WriteLine("Некорректный ввод. Попробуйте еще раз.");
            }
        } while (!validMove);
    }

    static bool IsValidMove(int row, int col)
    {
        // Проверка, что клетка пуста
        if (board[row, col] != ' ')
            return false;

        char opponentPlayer = (currentPlayer == 'X') ? 'O' : 'X';

        // Проверка наличия фишек вокруг выбранной клетки
        if (!HasAdjacentOpponentPiece(row, col, opponentPlayer))
            return false;

        // Проверка наличия возможности захвата фишек
        return CanCapture(row, col, opponentPlayer);
    }

    static bool CanCapture(int row, int col, char opponentPlayer)
    {
        // Проверка возможности захвата в каждом из восьми направлений
        return CanCaptureInDirection(row, col, -1, 0, opponentPlayer) ||
               CanCaptureInDirection(row, col, 1, 0, opponentPlayer) ||
               CanCaptureInDirection(row, col, 0, -1, opponentPlayer) ||
               CanCaptureInDirection(row, col, 0, 1, opponentPlayer) ||
               CanCaptureInDirection(row, col, -1, -1, opponentPlayer) ||
               CanCaptureInDirection(row, col, -1, 1, opponentPlayer) ||
               CanCaptureInDirection(row, col, 1, -1, opponentPlayer) ||
               CanCaptureInDirection(row, col, 1, 1, opponentPlayer);
    }

    static bool CanCaptureInDirection(int startRow, int startCol, int rowDirection, int colDirection, char opponentPlayer)
    {
        int currentRow = startRow + rowDirection;
        int currentCol = startCol + colDirection;

        // Проверка на выход за границы поля
        if (currentRow < 0 || currentRow >= 8 || currentCol < 0 || currentCol >= 8)
            return false;

        // Проверка, что первая фишка в данном направлении противника
        if (board[currentRow, currentCol] != opponentPlayer)
            return false;

        // Проверка возможности захвата фишек в данном направлении
        while (board[currentRow, currentCol] == opponentPlayer)
        {
            currentRow += rowDirection;
            currentCol += colDirection;

            // Проверка на выход за границы поля
            if (currentRow < 0 || currentRow >= 8 || currentCol < 0 || currentCol >= 8)
                return false;
        }

        // Если достигнута фишка текущего игрока, то возвращаем true, иначе false
        return board[currentRow, currentCol] == currentPlayer;
    }

    static bool HasAdjacentOpponentPiece(int row, int col, char opponentPlayer)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int adjacentRow = row + i;
                int adjacentCol = col + j;

                // Проверка на выход за границы поля
                if (adjacentRow < 0 || adjacentRow >= 8 || adjacentCol < 0 || adjacentCol >= 8)
                    continue;

                // Проверка, что соседняя клетка содержит фишку противника
                if (board[adjacentRow, adjacentCol] == opponentPlayer)
                    return true;
            }
        }

        return false;
    }

    static void MakeMove(int row, int col)
    {
        board[row, col] = currentPlayer;
        CaptureOpponentPieces(row, col);
    }

    static bool IsGameOver()
    {
        // Проверка на конец игры
        // Возможные условия: отсутствие пустых клеток или отсутствие возможных ходов для обоих игроков
        return !HasEmptyCells() || (!CanMakeMove('X') && !CanMakeMove('O'));
    }

    static bool HasEmptyCells()
    {
        // Проверка на наличие пустых клеток на игровом поле
        foreach (char cell in board)
        {
            if (cell == ' ')
                return true;
        }
        return false;
    }

    static bool CanMakeMove(char player)
    {
        // Проверка наличия возможных ходов для игрока
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (IsValidMove(i, j))
                {
                    return true;
                }
            }
        }
        return false;
    }

    static void CaptureOpponentPieces(int row, int col)
    {
        char opponentPlayer = (currentPlayer == 'X') ? 'O' : 'X';

        TryCaptureInDirection(row, col, -1, 0, opponentPlayer);
        TryCaptureInDirection(row, col, 1, 0, opponentPlayer);
        TryCaptureInDirection(row, col, 0, -1, opponentPlayer);
        TryCaptureInDirection(row, col, 0, 1, opponentPlayer);
        TryCaptureInDirection(row, col, -1, -1, opponentPlayer);
        TryCaptureInDirection(row, col, -1, 1, opponentPlayer);
        TryCaptureInDirection(row, col, 1, -1, opponentPlayer);
        TryCaptureInDirection(row, col, 1, 1, opponentPlayer);
    }

    static void TryCaptureInDirection(int startRow, int startCol, int rowDirection, int colDirection, char opponentPlayer)
    {
        int currentRow = startRow + rowDirection;
        int currentCol = startCol + colDirection;

        // Проверка на выход за границы поля
        if (currentRow < 0 || currentRow >= 8 || currentCol < 0 || currentCol >= 8)
        {
            return;
        }

        // Проверка, что первая фишка в данном направлении противника
        if (board[currentRow, currentCol] != opponentPlayer)
        {
            return;
        }

        List<Tuple<int, int>> capturedPieces = new List<Tuple<int, int>>();

        // Захват фишек противника
        while (board[currentRow, currentCol] == opponentPlayer)
        {
            capturedPieces.Add(Tuple.Create(currentRow, currentCol));
            currentRow += rowDirection;
            currentCol += colDirection;

            // Проверка на выход за границы поля
            if (currentRow < 0 || currentRow >= 8 || currentCol < 0 || currentCol >= 8)
            {
                // Не достигли фишки текущего игрока, отменяем захват
                capturedPieces.Clear();
                return;
            }
        }

        // Если достигнута фишка текущего игрока, то захватываем фишки
        if (board[currentRow, currentCol] == currentPlayer)
        {
            foreach (var piece in capturedPieces)
            {
                board[piece.Item1, piece.Item2] = currentPlayer;
            }
        }
    }

    static void PrintScores()
    {
        // Подсчет и вывод счета игры
        int xCount = 0, oCount = 0;

        foreach (char cell in board)
        {
            if (cell == 'X')
                xCount++;
            else if (cell == 'O')
                oCount++;
        }

        Console.WriteLine($"Счет: Игрок X - {xCount}, Игрок O - { oCount}");
    }

    static void MakeBotMove()
    {
        int bestRow = -1, bestCol = -1;
        int maxCaptures = -1;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (IsValidMove(i, j))
                {
                    int captures = CountCaptures(i, j);
                    if (captures > maxCaptures)
                    {
                        maxCaptures = captures;
                        bestRow = i;
                        bestCol = j;
                    }
                }
            }
        }

        if (bestRow != -1 && bestCol != -1)
        {
            MakeMove(bestRow, bestCol);
            Console.WriteLine($"Бот сделал ход {Convert.ToChar('a' + bestCol)}{8 - bestRow}");
        }
    }

    static int CountCaptures(int row, int col)
    {
        int totalCaptures = 0;

        char opponentPlayer = (currentPlayer == 'X') ? 'O' : 'X';

        // Подсчет захватов в каждом направлении
        totalCaptures += CountCapturesInDirection(row, col, -1, 0, opponentPlayer); // Вверх
        totalCaptures += CountCapturesInDirection(row, col, 1, 0, opponentPlayer);  // Вниз
        totalCaptures += CountCapturesInDirection(row, col, 0, -1, opponentPlayer); // Влево
        totalCaptures += CountCapturesInDirection(row, col, 0, 1, opponentPlayer);  // Вправо
        totalCaptures += CountCapturesInDirection(row, col, -1, -1, opponentPlayer); // Вверх-влево
        totalCaptures += CountCapturesInDirection(row, col, -1, 1, opponentPlayer);  // Вверх-вправо
        totalCaptures += CountCapturesInDirection(row, col, 1, -1, opponentPlayer);  // Вниз-влево
        totalCaptures += CountCapturesInDirection(row, col, 1, 1, opponentPlayer);   // Вниз-вправо

        return totalCaptures;
    }

    static int CountCapturesInDirection(int startRow, int startCol, int rowDirection, int colDirection, char opponentPlayer)
    {
        int currentRow = startRow + rowDirection;
        int currentCol = startCol + colDirection;
        int captures = 0;

        // Проверка на выход за границы поля
        if (currentRow < 0 || currentRow >= 8 || currentCol < 0 || currentCol >= 8)
        {
            return 0;
        }

        // Проверка, что первая фишка в данном направлении противника
        if (board[currentRow, currentCol] != opponentPlayer)
        {
            return 0;
        }

        // Подсчет захватов в данном направлении
        while (board[currentRow, currentCol] == opponentPlayer)
        {
            captures++;
            currentRow += rowDirection;
            currentCol += colDirection;

            // Проверка на выход за границы поля
            if (currentRow < 0 || currentRow >= 8 || currentCol < 0 || currentCol >= 8)
            {
                return captures;
            }
        }

        // Если достигнута фишка текущего игрока, то возвращаем количество захватов
        return (board[currentRow, currentCol] == currentPlayer) ? captures : 0;
    }
}
