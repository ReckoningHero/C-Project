public static class GlobalMembers
{
	//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
	//ORIGINAL LINE: #define uFMOD_StopSong() uFMOD_PlaySong(0, 0, 0)
	//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
	//ORIGINAL LINE: #define uFMOD_Rewind() uFMOD_Jump2Pattern(0)
	//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
	//ORIGINAL LINE: #define uFMOD_DEFAULT_VOL uFMOD_MAX_VOL
	//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
	//#pragma comment(lib, "winmm.lib")
	//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
	//#pragma comment(lib, "ufmod.lib")

	public static bool glbl_draw_points = false;

	public static void DrawHLine(System.IntPtr console, COORD coord, ushort len, ushort attrs = BACKGROUND_RED | BACKGROUND_GREEN)
	{
		uint wr;

		FillConsoleOutputCharacterA(console, ' ', len, coord, wr);
		FillConsoleOutputAttribute(console, attrs, len, coord, wr);
	}

	public static void DrawText(System.IntPtr console, COORD coord, ref char text, ushort attrs = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_INTENSITY)
	{
		uint wr;
		uint len = lstrlenW(text);

		WriteConsoleOutputCharacter(console, text, len, coord, wr);
		FillConsoleOutputAttribute(console, attrs, len, coord, wr);
	}

	public static void DrawVLine(System.IntPtr console, COORD coord, ushort len, ushort attrs = BACKGROUND_RED | BACKGROUND_GREEN)
	{
		uint wr;

		for (size_t i = 0; i < len; i++)
		{
			coord.Y++;
			WriteConsoleOutputCharacterA(console, " ", 1, coord, wr);
			WriteConsoleOutputAttribute(console, attrs, 1, coord, wr);
		}
	}

	public static void DrawFigure(System.IntPtr console, FIGURE fig, bool black = false)
	{
		uint wr;
		COORD c = new COORD();
		ushort noattr = 0;

		for (ushort i = 0; i < fig.size.x; i++)
		{
			for (ushort j = 0; j < fig.size.y; j++)
			{
				if (fig.c[i, j] == 1)
				{
					c.X = i + fig.position.X;
					c.Y = j + fig.position.Y;

					if (black)
					{
						WriteConsoleOutputCharacter(console, " ", 1, c, wr);
						WriteConsoleOutputAttribute(console, noattr, 1, c, wr);
					}
					else
					{
						WriteConsoleOutputCharacter(console, glbl_draw_points ? "." : " ", 1, c, wr);
						WriteConsoleOutputAttribute(console, fig.attr, 1, c, wr);
					}
				}
			}
		}
	}

	public static void DrawShadowFigure(System.IntPtr console, byte[,] matrix, FIGURE fig, bool black = false, ushort attr = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_INTENSITY)
	{
		ushort height = 28;
		ushort noattr = 0;
		uint wr;

		for (ushort i = fig.position.X - 26; i < fig.size.x + fig.position.X - 26; i++)
		{
			for (ushort j = fig.position.Y + 1; j < 30; j++)
			{
				if (matrix[i, j] == 1 && j - 1 < height)
				{
					height = j - 1;
				}
			}
		}

		for (ushort i = fig.position.X; i < fig.size.x + fig.position.X; i++)
		{
			COORD c = new COORD(i, height);

			if (black)
			{
				WriteConsoleOutputCharacter(console, " ", 1, c, wr);
				WriteConsoleOutputAttribute(console, noattr, 1, c, wr);
			}
			else
			{
				WriteConsoleOutputCharacter(console, "+", 1, c, wr);
				WriteConsoleOutputAttribute(console, attr, 1, c, wr);
			}
		}
	}

	public static void RotateFigure(FIGURE fig)
	{
		FIGURE newfig = new FIGURE();
		newfig.size.x = fig.size.y;
		newfig.size.y = fig.size.x;
		newfig.attr = fig.attr;
		newfig.position.X = fig.position.X;
		newfig.position.Y = fig.position.Y;

		for (size_t i = 0; i < fig.size.x; i++)
		{
			for (size_t j = 0; j < fig.size.y; j++)
			{
				newfig.c[j, i] = fig.c[i, fig.size.y - j - 1];
			}
		}

//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy((string)fig, (string) newfig, sizeof(FIGURE));
	}

	public static bool FillMatrix(byte[,] matrix, FIGURE fig)
	{
		for (size_t i = 0; i < fig.size.x; i++)
		{
			for (size_t j = 0; j < fig.size.y; j++)
			{
				if (fig.c[i, j] == 1)
				{
					matrix[fig.position.X + i - 26, fig.position.Y + j] = 1;
				}
			}
		}

		return fig.position.Y != 1;
	}

	public static bool GetMatrixHeight(byte[,] matrix, FIGURE fig)
	{
		for (ushort i = 0; i < fig.size.x; i++)
		{
			for (ushort j = fig.size.y; j > 0; j--)
			{
				if (fig.c[i, j - 1] == 1)
				{
					if (matrix[fig.position.X + i - 26, fig.position.Y + j] == 1)
					{
						return true;
					}
				}
			}
		}

		if (fig.position.Y + fig.size.y >= 29)
		{
			for (ushort i = 0; i < fig.size.x; i++)
			{
				if (fig.c[i, fig.size.y - 1] == 1)
				{
					return true;
				}
			}
		}

		return false;
	}


	public static bool GetMatrixRotHeight(byte[,] matrix, FIGURE fig)
	{
		FIGURE rotfig = new FIGURE(fig);
		RotateFigure(rotfig);

		if (rotfig.position.Y + rotfig.size.y >= 30)
		{
			for (ushort i = 0; i < rotfig.size.x; i++)
			{
				if (rotfig.c[i, rotfig.size.y - 1] == 1)
				{
					return true;
				}
			}
		}

		return false;
	}


	public static bool CanMoveLeft(byte[,] matrix, FIGURE fig)
	{
		for (ushort i = 0; i < fig.size.x; i++)
		{
			for (ushort j = 0; j < fig.size.y; j++)
			{
				if (fig.c[i, j] == 1)
				{
					if (matrix[fig.position.X + i - 26 - 1, fig.position.Y + j] == 1)
					{
						return false;
					}
				}
			}
		}

		return true;
	}

	public static bool CanMoveRight(byte[,] matrix, FIGURE fig)
	{
		for (ushort i = fig.size.x; i > 0; i--)
		{
			for (ushort j = 0; j < fig.size.y; j++)
			{
				if (fig.c[i - 1, j] == 1)
				{
					if (matrix[fig.position.X + i - 26, fig.position.Y + j] == 1)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public static ushort GetRightRotationCollision(byte[,] matrix, FIGURE fig)
	{
		FIGURE rotfig = new FIGURE(fig);
		RotateFigure(rotfig);
		ushort coll = 0;

		for (ushort j = 0; j < rotfig.size.y; j++)
		{
			for (ushort i = 0; i < rotfig.size.x; i++)
			{
				if (rotfig.c[i, j] == 1)
				{
					if (matrix[rotfig.position.X + i - 26, rotfig.position.Y + j] == 1 && coll < rotfig.size.x - i)
					{
						coll = rotfig.size.x - i;
					}
				}
			}
		}

		return coll;
	}

	public static void MoveMatrixDown(System.IntPtr buf, byte[,] matrix, ushort height)
	{
		char chr;
		uint wr;
		ushort attr;

		for (ushort j = height; j >= 1; j--)
		{
			for (ushort i = 0; i < 24; i++)
			{
				matrix[i, j] = matrix[i, j - 1];

				if (j > 1)
				{
					COORD c = new COORD(i + 26, j - 1);
					ReadConsoleOutputCharacter(buf, chr, 1, c, wr);
					ReadConsoleOutputAttribute(buf, attr, 1, c, wr);
					c.Y = j;
					WriteConsoleOutputCharacter(buf, chr, 1, c, wr);
					WriteConsoleOutputAttribute(buf, attr, 1, c, wr);
				}
			}
		}
	}

	public static bool CheckMatrix(System.IntPtr buf, byte[,] matrix)
	{
		bool row;
		for (ushort j = 1; j < 29; j++)
		{
			row = true;
			for (size_t i = 0; i < 24; i++)
			{
				if (matrix[i, j] == 0)
				{
					row = false;
					break;
				}
			}

			if (row)
			{
				for (size_t i = 0; i < 24; i++)
				{
					matrix[i, j] = 0;
				}

				COORD coord = new COORD();
				coord.X = 26;
				coord.Y = j;
				DrawHLine(buf, coord, 24, BACKGROUND_RED | BACKGROUND_GREEN | BACKGROUND_BLUE | BACKGROUND_INTENSITY);
				Sleep(200);
				DrawHLine(buf, coord, 24, 0);

				MoveMatrixDown(buf, matrix, j);
				return true;
			}
		}

		return false;
	}

	public static void DebugMatrix(System.IntPtr buf, byte[,] matrix)
	{
		char[] bb = new char[12];
		COORD crd = new COORD(1,1);

		for (size_t j = 1; j < 29; j++)
		{
			for (size_t i = 0; i < 24; i++)
			{
				bb = string.Format("{0:D}", matrix[i, j]);
				DrawText(buf, crd, ref bb, FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE);
				crd.X += 1;
			}
			crd.Y += 1;
			crd.X = 1;
		}
	}

	public static void PrintCount(System.IntPtr buf, uint points)
	{
		char[] bb = new char[24];
		COORD coord = new COORD();
		coord.X = 52;
		coord.Y = 6;

		if (points == 0)
		{
			bb = "�ר�: 0              ";
		}
		else
		{
			bb = string.Format("�ר�: {0:D}", points);
		}

		DrawText(buf, coord, ref bb, FOREGROUND_BLUE | FOREGROUND_GREEN | FOREGROUND_INTENSITY);
	}

	public static void PrintDifficulty(System.IntPtr buf, uint difficulty, uint next)
	{
		char[] bb = new char[36];
		COORD coord = new COORD();
		coord.X = 52;
		coord.Y = 8;

		if (difficulty == 0)
		{
			bb = "���������: 1            ";
		}
		else
		{
			bb = string.Format("���������: {0:D}", difficulty + 1);
		}

		DrawText(buf, coord, ref bb, FOREGROUND_BLUE | FOREGROUND_GREEN | FOREGROUND_INTENSITY);

		if (next != 0)
		{
			coord.Y = 10;
			bb = string.Format("�� ���������: {0:D}     ", next);
			DrawText(buf, coord, ref bb, FOREGROUND_BLUE | FOREGROUND_INTENSITY);
		}
		else
		{
			DrawText(buf, coord, "��������� �����������     ", FOREGROUND_BLUE | FOREGROUND_INTENSITY);
		}
	}

	public static void EmptyScreen(System.IntPtr buf)
	{
		uint wr;

		for (ushort j = 1; j < 29; j++)
		{
			COORD c = new COORD(26, j);
			FillConsoleOutputAttribute(buf, 0, 24, c, wr);
			FillConsoleOutputCharacter(buf, ' ', 24, c, wr);
		}
	}

	public static void EmptyMatrix(byte[][] matrix)
	{
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
		memset(matrix, 0, 24 * 30);
	}

	public static void PrintGameOver(System.IntPtr buf, bool pr = true)
	{
		COORD coord = new COORD();
		coord.X = 52;
		coord.Y = 15;
		if (pr)
		{
			DrawText(buf, coord, "���� ��������! ", FOREGROUND_BLUE | FOREGROUND_INTENSITY);
			coord.Y++;
			DrawText(buf, coord, "ENTER - ������", FOREGROUND_BLUE | FOREGROUND_INTENSITY);
		}
		else
		{
			DrawText(buf, coord, "                      ", FOREGROUND_BLUE | FOREGROUND_INTENSITY);
			coord.Y++;
			DrawText(buf, coord, "                      ", FOREGROUND_BLUE | FOREGROUND_INTENSITY);
		}
	}

	static void Main()
	{
		SetConsoleTitle("Console Tetris 1.0");

		uint[] dif_table = {100, 200, 500, 1000, 1700, 2500, 3500, 5000, 8000, 12000, 18000, 25000, 34000, 42000, 55000, 75000, 100000, 150000, 300000, 0};
		FIGURE[] fig = Arrays.InitializeWithDefaultInstances<FIGURE>(13);
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
		memset((string) fig[0], 0, sizeof(FIGURE) * 13);

		fig[0].attr = BACKGROUND_BLUE | BACKGROUND_INTENSITY;
		fig[0].c[0, 0] = 1;
		fig[0].c[1, 0] = 1;
		fig[0].c[0, 1] = 1;
		fig[0].c[1, 1] = 1;
		fig[0].size.x = 2;
		fig[0].size.y = 2;

		fig[1].attr = BACKGROUND_RED | BACKGROUND_INTENSITY;
		fig[1].c[0, 0] = 1;
		fig[1].c[0, 1] = 1;
		fig[1].c[0, 2] = 1;
		fig[1].c[0, 3] = 1;
		fig[1].size.x = 1;
		fig[1].size.y = 4;

		fig[2].attr = BACKGROUND_RED | BACKGROUND_GREEN | BACKGROUND_INTENSITY;
		fig[2].c[0, 0] = 1;
		fig[2].c[0, 1] = 1;
		fig[2].c[0, 2] = 1;
		fig[2].c[1, 0] = 0;
		fig[2].c[1, 1] = 0;
		fig[2].c[1, 2] = 1;
		fig[2].size.x = 2;
		fig[2].size.y = 3;

		fig[3].attr = BACKGROUND_GREEN | BACKGROUND_INTENSITY;
		fig[3].c[0, 0] = 0;
		fig[3].c[0, 1] = 0;
		fig[3].c[0, 2] = 1;
		fig[3].c[1, 0] = 1;
		fig[3].c[1, 1] = 1;
		fig[3].c[1, 2] = 1;
		fig[3].size.x = 2;
		fig[3].size.y = 3;

		fig[4].attr = BACKGROUND_RED | BACKGROUND_BLUE | BACKGROUND_INTENSITY;
		fig[4].c[0, 0] = 1;
		fig[4].c[0, 1] = 0;
		fig[4].c[1, 0] = 1;
		fig[4].c[1, 1] = 1;
		fig[4].c[2, 0] = 0;
		fig[4].c[2, 1] = 1;
		fig[4].size.x = 3;
		fig[4].size.y = 2;

		fig[5].attr = BACKGROUND_BLUE | BACKGROUND_GREEN | BACKGROUND_INTENSITY;
		fig[5].c[0, 0] = 0;
		fig[5].c[0, 1] = 1;
		fig[5].c[1, 0] = 1;
		fig[5].c[1, 1] = 1;
		fig[5].c[2, 0] = 1;
		fig[5].c[2, 1] = 0;
		fig[5].size.x = 3;
		fig[5].size.y = 2;

		fig[6].attr = BACKGROUND_RED | BACKGROUND_GREEN | BACKGROUND_BLUE | BACKGROUND_INTENSITY;
		fig[6].c[0, 0] = 0;
		fig[6].c[0, 1] = 1;
		fig[6].c[1, 0] = 1;
		fig[6].c[1, 1] = 1;
		fig[6].c[2, 0] = 0;
		fig[6].c[2, 1] = 1;
		fig[6].size.x = 3;
		fig[6].size.y = 2;

		fig[7].attr = BACKGROUND_RED;
		fig[7].c[0, 0] = 1;
		fig[7].c[0, 1] = 1;
		fig[7].size.x = 1;
		fig[7].size.y = 2;

		fig[8].attr = BACKGROUND_BLUE | BACKGROUND_RED;
		fig[8].c[0, 0] = 0;
		fig[8].c[0, 1] = 1;
		fig[8].c[0, 2] = 0;
		fig[8].c[1, 0] = 1;
		fig[8].c[1, 1] = 1;
		fig[8].c[1, 2] = 1;
		fig[8].c[2, 0] = 0;
		fig[8].c[2, 1] = 1;
		fig[8].c[2, 2] = 0;
		fig[8].size.x = 3;
		fig[8].size.y = 3;

		fig[9].attr = BACKGROUND_GREEN;
		fig[9].c[0, 0] = 1;
		fig[9].c[0, 1] = 1;
		fig[9].c[1, 0] = 1;
		fig[9].c[1, 1] = 0;
		fig[9].c[2, 0] = 1;
		fig[9].c[2, 1] = 1;
		fig[9].size.x = 3;
		fig[9].size.y = 2;

		fig[10].attr = BACKGROUND_GREEN | BACKGROUND_BLUE;
		fig[10].c[0, 0] = 1;
		fig[10].c[0, 1] = 0;
		fig[10].c[0, 2] = 0;
		fig[10].c[1, 0] = 1;
		fig[10].c[1, 1] = 1;
		fig[10].c[1, 2] = 1;
		fig[10].c[2, 0] = 0;
		fig[10].c[2, 1] = 0;
		fig[10].c[2, 2] = 1;
		fig[10].size.x = 3;
		fig[10].size.y = 3;


		fig[11].attr = BACKGROUND_RED | BACKGROUND_GREEN | BACKGROUND_BLUE;
		fig[11].c[0, 0] = 1;
		fig[11].c[0, 1] = 1;
		fig[11].c[0, 2] = 1;
		fig[11].c[1, 0] = 1;
		fig[11].c[1, 1] = 1;
		fig[11].c[1, 2] = 1;
		fig[11].c[2, 0] = 1;
		fig[11].c[2, 1] = 1;
		fig[11].c[2, 2] = 1;
		fig[11].size.x = 3;
		fig[11].size.y = 3;

		fig[12].attr = BACKGROUND_BLUE | BACKGROUND_INTENSITY;
		fig[12].c[0, 0] = 0;
		fig[12].c[0, 1] = 0;
		fig[12].c[0, 2] = 1;
		fig[12].c[1, 0] = 1;
		fig[12].c[1, 1] = 1;
		fig[12].c[1, 2] = 1;
		fig[12].c[2, 0] = 1;
		fig[12].c[2, 1] = 0;
		fig[12].c[2, 2] = 0;
		fig[12].size.x = 3;
		fig[12].size.y = 3;

		System.IntPtr buf = CreateConsoleScreenBuffer(GENERIC_READ | GENERIC_WRITE, FILE_SHARE_WRITE | FILE_SHARE_READ, null, CONSOLE_TEXTMODE_BUFFER, null);
		System.IntPtr buf2 = CreateConsoleScreenBuffer(GENERIC_READ | GENERIC_WRITE, FILE_SHARE_WRITE | FILE_SHARE_READ, null, CONSOLE_TEXTMODE_BUFFER, null);
		System.IntPtr hStdout = GetStdHandle(STD_OUTPUT_HANDLE);

		uint wr = 0;

		COORD coord = new COORD();

		CONSOLE_CURSOR_INFO cursor = new CONSOLE_CURSOR_INFO();
		cursor.bVisible = 0;
		cursor.dwSize = 1;

		SetConsoleCursorInfo(buf, cursor);
		SetConsoleCursorInfo(buf2, cursor);

		coord.X = 80;
		coord.Y = 30;
		SetConsoleScreenBufferSize(buf, coord);
		SetConsoleScreenBufferSize(buf2, coord);

		SetConsoleActiveScreenBuffer(buf);

		System.IntPtr console = GetConsoleWindow();
		RECT r = new RECT();
		GetWindowRect(console, r);
		MoveWindow(console, r.left, r.top, 1000, 1000, 1);

		INPUT_RECORD input = new INPUT_RECORD();

		byte[,] matrix = new byte[24, 30];
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
		memset((string) matrix[0, 0], 0, sizeof(byte));

		coord.X = 0;
		coord.Y = 0;
		DrawHLine(buf, coord, 80);
		DrawVLine(buf, coord, 30);

		coord.Y = 29;
		DrawHLine(buf, coord, 80);

		coord.X = 79;
		coord.Y = 0;
		DrawVLine(buf, coord, 30);

		coord.X = 50;
		DrawVLine(buf, coord, 30);

		coord.X = 25;
		DrawVLine(buf, coord, 30);

		coord.X = 50;
		coord.Y = 4;
		DrawHLine(buf, coord, 29);

		coord.X = 56;
		coord.Y = 2;
		DrawText(buf, coord, "������ 1.0 (c) dx");

		for (size_t i = 0; i < 13; i++)
		{
			fig[i].position.X = 62;
			fig[i].position.Y = 19;
		}

		coord.X = 52;
		coord.Y = 20;
		DrawText(buf, coord, "�����:");

		coord.Y = 25;
		DrawText(buf, coord, "F   - ������� ���������");
		coord.Y = 26;
		DrawText(buf, coord, "P   - �����");
		coord.Y = 27;
		DrawText(buf, coord, "M   - ������ ���/����");
		coord.Y = 28;
		DrawText(buf, coord, "ESC - �����");

		PrintCount(buf, 0);

		uint defspeed = 21;
		uint difficulty = 0;
		uint SPEED = defspeed;
		uint points = 0;
		uint combo;
		uint counter = 0;
		uint inputCode;
		uint nextfig;
		PrintDifficulty(buf, difficulty, dif_table[0]);

		bool fig_exists = false;
		bool gameover = false;
		bool changed = false;
		bool play_music = true;
		FIGURE newfig = null;
		FIGURE oldfig = new FIGURE();

		SYSTEMTIME time = new SYSTEMTIME();
		GetSystemTime(time);
		RandomNumbers.Seed(time.wSecond + time.wMinute * 60 + time.wHour * 3600);

		nextfig = RandomNumbers.NextNumber() % 7;

		DebugMatrix(buf, matrix);

		uFMOD_PlaySong((object)1, null, DefineConstants.XM_RESOURCE);

		while (true)
		{
			if (gameover)
			{
				PeekConsoleInput(GetStdHandle(STD_INPUT_HANDLE), input, 1, wr);

				if (wr != 0)
				{
					if (input.EventType == KEY_EVENT)
					{
						if (input.Event.KeyEvent.bKeyDown)
						{
							if (input.Event.KeyEvent.wVirtualKeyCode == VK_RETURN)
							{
								gameover = false;

								EmptyScreen(buf);
								EmptyMatrix((byte) matrix);
								DebugMatrix(buf, matrix);
								PrintDifficulty(buf, 0, dif_table[0]);
								PrintCount(buf, 0);
								PrintGameOver(buf, false);
							}
							else if (input.Event.KeyEvent.wVirtualKeyCode == VK_ESCAPE)
							{
								break;
							}
						}
					}

					FlushConsoleInputBuffer(GetStdHandle(STD_INPUT_HANDLE));
				}
			}
			else
			{
				if (!fig_exists)
				{
					newfig = new FIGURE(fig[nextfig]);
					DrawFigure(buf, fig[nextfig], true);
					nextfig = RandomNumbers.NextNumber() % (difficulty <= 7 ? 7 : 13);

					DrawFigure(buf, fig[nextfig]);

					newfig.position.Y = 1;
					newfig.position.X = 37;

					for (size_t i = 0, rn = RandomNumbers.NextNumber() % 4; i < rn; i++)
					{
						RotateFigure(newfig);
					}

					fig_exists = true;
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
					memcpy((string)oldfig, (string)newfig, sizeof(FIGURE));

					changed = true;
				}

				PeekConsoleInput(GetStdHandle(STD_INPUT_HANDLE), input, 1, wr);

				inputCode = 0;

				if (wr != 0)
				{
					if (input.EventType == KEY_EVENT)
					{
						if (input.Event.KeyEvent.bKeyDown)
						{
							if (input.Event.KeyEvent.wVirtualKeyCode == VK_ESCAPE)
							{
								break;
							}
							else
							{
								inputCode = input.Event.KeyEvent.wVirtualKeyCode;
							}
						}
					}

					FlushConsoleInputBuffer(GetStdHandle(STD_INPUT_HANDLE));
				}

				if (inputCode == VK_UP)
				{
					if (!GetMatrixRotHeight(matrix, newfig))
					{
						ushort rot = GetRightRotationCollision(matrix, newfig);
						if (newfig.position.X - rot >= 26)
						{
							newfig.position.X -= rot;

							if (GetRightRotationCollision(matrix, newfig) == 0)
							{
								ushort diff = newfig.position.X + newfig.size.y - 1;

								if (newfig.size.y > newfig.size.x && diff > 49)
								{
									newfig.position.X -= diff - 49;
								}

								if (GetRightRotationCollision(matrix, newfig) == 0)
								{
									RotateFigure(newfig);
									changed = true;
								}
								else
								{
									newfig.position.X += diff - 49;
								}
							}
							else
							{
								newfig.position.X += rot;
							}
						}
					}
				}
				else if (inputCode == VK_LEFT && newfig.position.X > 26 && CanMoveLeft(matrix, newfig))
				{
					changed = true;
					newfig.position.X--;
				}
				else if (inputCode == VK_RIGHT && newfig.position.X + newfig.size.x <= 49 && CanMoveRight(matrix, newfig))
				{
					changed = true;
					newfig.position.X++;
				}
				else if (inputCode == VK_DOWN)
				{
					changed = true;
					SPEED = 1;
					counter = 0;
				}
				else if (inputCode == 'm' || inputCode == 'M')
				{
					if (play_music)
					{
						uFMOD_Pause();
					}
					else
					{
						uFMOD_Resume();
					}

					play_music = !play_music;
				}
				else if (inputCode == 'f' || inputCode == 'F')
				{
					glbl_draw_points = !glbl_draw_points;
				}

				if (changed)
				{
					DrawFigure(buf, oldfig, true);
					DrawShadowFigure(buf, matrix, oldfig, true);
				}

//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
				memcpy((string)oldfig, (string)newfig, sizeof(FIGURE));

				changed = false;

				if (GetMatrixHeight(matrix, newfig))
				{
					DrawFigure(buf, newfig);
					if (counter >= SPEED)
					{
						fig_exists = false;
						if (!FillMatrix(matrix, newfig))
						{
							points = 0;
							defspeed = 21;
							difficulty = 0;
							gameover = true;
							PrintGameOver(buf);
						}

						newfig = null;
						SPEED = defspeed;
						counter = 0;

						combo = 1;

						if (!gameover)
						{
							while (CheckMatrix(buf, matrix))
							{
								points += 100 * (8 - defspeed / 3) * combo;
								PrintCount(buf, points);

								combo += 1;

								while (difficulty < 19 && points >= dif_table[difficulty])
								{
									difficulty++;
									defspeed--;
									PrintDifficulty(buf, difficulty, dif_table[difficulty]);
								}
							}
						}

						DebugMatrix(buf, matrix);
					}
				}
				else if (fig_exists && counter == SPEED)
				{
					DrawShadowFigure(buf, matrix, newfig);
					DrawFigure(buf, newfig);

					newfig.position.Y++;
					changed = true;
					counter = 0;
				}
				else
				{
					DrawShadowFigure(buf, matrix, newfig);
					DrawFigure(buf, newfig);
				}


				if (inputCode == 'p' || inputCode == 'P')
				{
					if (play_music)
					{
						uFMOD_Pause();
					}

					SetConsoleActiveScreenBuffer(buf2);

					uint bear = 0;

					coord.X = 24;
					coord.Y = 1;
					DrawText(buf2, coord, ":::::::                                                                      ,:::,...,:::                                                                   ,::..++++?..::.                     ,..::,.                                     ::..+++++++..::....::::::,........,::::,,:::                                    ::.+++++...:::::::::::::~~~~~~~:..,:...?++~.:                                   ::.+++..::::::::::::::~~~~~~~~~~~~,..+++++++:                                   :::...:::::::::::::~~~~~~~~~~~~~~~~~~..++++.:                                    :::::::::::::::::~~~~~~~~~~~~~~~~~~~~,.++.:                                       ::::::::::::::~~~~~~~~~~~~~~~~~~~~~~~..:                                       ::::::::::::::~~~~~~~~~~~~~~~~~~~~~~~~~,                                       :::::::::::::::~~~~~~~~~~~~~~~~~~~~~~~~~                                       ::::::::::::.....:~~~~~~~~~~~~~~~~~~~~~~~~                                     ::::::::::::.......:~~~~~~~~~~~~~~~,...:~~~                                     ,:::::::::::.......~~~~~~~~~~~~~~~......~~~~                                    :::::::::::::.....,~~~~~~~~~~~~~~........~~~                                   ::::::::::::::::~~~~~~~~~~~~~~~~~~~.......~~~                                   :::::::::::::::::~~~~~~~~~~~~~~~~~~:....~~~~~                                   :::::::::::::::::..?++++++,.++++=~~~~~~~~~~~~                                  ::::::::::::::::..?++++++......+++++..~~~~~~~~                                 :::::::::::::::::.++++++++......++++++.:~~~~~~~~                               ::::::::::::::::::.?++++++++,...+++++++.:~~~~~~~~                            ,.::::::::::::::::::::..++++?.....~++++++~.~~~~~~~~~                           ,.:,..:::::::::::::::::::..~.IIIIII?......,~~~~~~~~~~~                               ::::::::::::::::::::..~.IIIIIIII~..~~~~~~~~~~~~~~~                             ::::::::::::::::::::::.~..IIIIII~~.~~~~~~~~..~~~                               :,..::..::.:::::::::::,.~~..IIII.~..~~~~.~~,..~~.,                                    ..,.:.,.:::::::::,.~~~....~~.:~~~~,...:,                                   ,:::::..:::::::::::::~~..:~~~~~~.:~~~~~~..::::~~~                              :::::::::::::::::::~~~~~~~,.....:~~~~~~~~:::::~~~~~,", 6);


					coord.X = 37;
					coord.Y = 0;
					DrawText(buf2, coord, "�����");
					coord.X = 27;
					coord.Y = 29;
					DrawText(buf2, coord, "��� ����������� ������� P");

					GetWindowRect(console, r);
					MoveWindow(console, r.left, r.top, 1000, 1000, 1);

					while (true)
					{
						PeekConsoleInput(GetStdHandle(STD_INPUT_HANDLE), input, 1, wr);

						if (wr != 0)
						{
							if (input.EventType == KEY_EVENT)
							{
								if (input.Event.KeyEvent.bKeyDown)
								{
									if (input.Event.KeyEvent.wVirtualKeyCode == 'p' || input.Event.KeyEvent.wVirtualKeyCode == 'P')
									{
										FlushConsoleInputBuffer(GetStdHandle(STD_INPUT_HANDLE));
										break;
									}
									else if (input.Event.KeyEvent.wVirtualKeyCode == VK_ESCAPE)
									{
										break;
									}
								}
							}

							FlushConsoleInputBuffer(GetStdHandle(STD_INPUT_HANDLE));
						}

						bear++;

						if (bear == 15)
						{
							coord.X = 54;
							coord.Y = 13;
							DrawText(buf2, coord, "~~~~~", 8);
						}
						else if (bear == 16)
						{
							coord.X = 53;
							coord.Y = 14;
							DrawText(buf2, coord, "~~~~~~", 8);
						}
						else if (bear == 17)
						{
							coord.X = 52;
							coord.Y = 15;
							DrawText(buf2, coord, "~~~~~~~~", 8);
						}
						else if (bear == 18)
						{
							coord.X = 53;
							coord.Y = 16;
							DrawText(buf2, coord, "~~~~~~~", 8);
						}
						else if (bear == 19)
						{
							coord.X = 53;
							coord.Y = 17;
							DrawText(buf2, coord, "~~~~~", 8);
						}

						else if (bear == 21)
						{
							coord.X = 53;
							coord.Y = 17;
							DrawText(buf2, coord, ":....", 6);
						}
						else if (bear == 22)
						{
							coord.X = 53;
							coord.Y = 16;
							DrawText(buf2, coord, ".......", 6);
						}
						else if (bear == 23)
						{
							coord.X = 52;
							coord.Y = 15;
							DrawText(buf2, coord, "........", 6);
						}
						else if (bear == 24)
						{
							coord.X = 53;
							coord.Y = 14;
							DrawText(buf2, coord, "......", 6);
						}
						else if (bear == 25)
						{
							coord.X = 54;
							coord.Y = 13;
							DrawText(buf2, coord, ",...:", 6);
						}
						else if (bear == 65)
						{
							bear = 0;
						}

						Sleep(50);
					}

					SetConsoleActiveScreenBuffer(buf);

					if (play_music)
					{
						uFMOD_Resume();
					}
				}

				Sleep(20);
				counter++;
			}
		}

		oldfig = null;
		SetConsoleActiveScreenBuffer(hStdout);
		CloseHandle(buf2);
		CloseHandle(buf);
	}

	/* HWAVEOUT* uFMOD_PlaySong(
	      void *lpXM,
	      void *param,
	      int fdwSong
	   )
	   ---
	   Description:
	   ---
	      Loads the given XM song and starts playing it immediately,
	      unless XM_SUSPENDED is specified. It will stop any currently
	      playing song before loading the new one.
	   ---
	   Parameters:
	   ---
	     lpXM
	        Specifies the song to play. If this parameter is 0, any
	        currently playing song is stopped. In such a case, function
	        does not return a meaningful value. fdwSong parameter
	        determines whether this value is interpreted as a filename,
	        as a resource identifier or a pointer to an image of the song
	        in memory.
	     param
	        If XM_RESOURCE is specified, this parameter should be the
	        handle to the executable file that contains the resource to
	        be loaded. A 0 value refers to the executable module itself.
	        If XM_MEMORY is specified, this parameter should be the size
	        of the image of the song in memory.
	        If XM_FILE is specified, this parameter is ignored.
	     fdwSong
	        Flags for playing the song. The following values are defined:
	        XM_FILE      lpXM points to filename. param is ignored.
	        XM_MEMORY    lpXM points to an image of a song in memory.
	                     param is the image size. Once, uFMOD_PlaySong
	                     returns, it's safe to free/discard the memory
	                     buffer.
	        XM_RESOURCE  lpXM specifies the name of the resource.
	                     param identifies the module whose executable file
	                     contains the resource.
	                     The resource type must be RT_RCDATA.
	        XM_NOLOOP    An XM track plays repeatedly by default. Specify
	                     this flag to play it only once.
	        XM_SUSPENDED The XM track is loaded in a suspended state,
	                     and will not play until the uFMOD_Resume function
	                     is called. This is useful for preloading a song
	                     or testing an XM track for validity.
	  ---
	  Return Values:
	  ---
	     On success, returns a pointer to an open WINMM output device handle.
	     Returns 0 on failure. If you are familiar with WINMM, you'll know
	     what this handle might be useful for :)
	  ---
	  Remarks:
	  ---
	     If no valid song is specified and there is one currently being
	     played, uFMOD_PlaySong just stops playback.
	*/
//C++ TO C# CONVERTER NOTE: __stdcall is not available in C#:
//ORIGINAL LINE: HWAVEOUT* __stdcall uFMOD_PlaySong(object*, object*, int);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//HWAVEOUT uFMOD_PlaySong(object NamelessParameter1, object NamelessParameter2, int NamelessParameter3);
	//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
	//ORIGINAL LINE: #define uFMOD_StopSong() uFMOD_PlaySong(0, 0, 0)

	/* void uFMOD_Jump2Pattern(
	      unsigned int pat
	   )
	   ---
	   Description:
	   ---
	      Jumps to the specified pattern index.
	   ---
	   Parameters:
	   ---
	      pat
	         Next zero based pattern index.
	   ---
	   Remarks:
	   ---
	      uFMOD doesn't automatically perform Note Off effects before jumping
	      to the target pattern. In other words, the original pattern will
	      remain in the mixer until it fades out. You can use this feature to
	      your advantage. If you don't like it, just insert leading Note Off
	      commands in all patterns intended to be used as uFMOD_Jump2Pattern
	      targets.
	      if the pattern index lays outside of the bounds of the pattern order
	      table, calling this function jumps to pattern 0, effectively
	      rewinding playback.
	*/
//C++ TO C# CONVERTER NOTE: __stdcall is not available in C#:
//ORIGINAL LINE: void __stdcall uFMOD_Jump2Pattern(uint);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//void uFMOD_Jump2Pattern(uint NamelessParameter);
	//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
	//ORIGINAL LINE: #define uFMOD_Rewind() uFMOD_Jump2Pattern(0)

	/* void uFMOD_Pause(void)
	   ---
	   Description:
	   ---
	      Pauses the currently playing song, if any.
	   ---
	   Remarks:
	   ---
	      While paused you can still control the volume (uFMOD_SetVolume) and
	      the pattern order (uFMOD_Jump2Pattern). The RMS volume coefficients
	      (uFMOD_GetStats) will go down to 0 and the progress tracker
	      (uFMOD_GetTime) will "freeze" while the song is paused.
	      uFMOD_Pause doesn't perform the request immediately. Instead, it
	      signals to pause when playback reaches next chunk of data, which may
	      take up to about 40ms. This way, uFMOD_Pause performs asynchronously
	      and returns very fast. It is not cumulative. So, calling
	      uFMOD_Pause many times in a row has the same effect as calling it
	      once.
	      If you need synchronous pause/resuming, you can use WINMM
	      waveOutPause/waveOutRestart functions.
	*/
//C++ TO C# CONVERTER NOTE: __stdcall is not available in C#:
//ORIGINAL LINE: void __stdcall uFMOD_Pause();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//void uFMOD_Pause();

	/* void uFMOD_Resume(void)
	   ---
	   Description:
	   ---
	      Resumes the currently paused song, if any.
	   ---
	   Remarks:
	   ---
	      uFMOD_Resume doesn't perform the request immediately. Instead, it
	      signals to resume when an internal thread gets a time slice, which
	      may take some milliseconds to happen. Usually, calling Sleep(0)
	      immediately after uFMOD_Resume causes it to resume faster.
	      uFMOD_Resume is not cumulative. So, calling it many times in a row
	      has the same effect as calling it once.
	      If you need synchronous pause/resuming, you can use WINMM
	      waveOutPause/waveOutRestart functions.
	*/
//C++ TO C# CONVERTER NOTE: __stdcall is not available in C#:
//ORIGINAL LINE: void __stdcall uFMOD_Resume();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//void uFMOD_Resume();

	/* unsigned int uFMOD_GetStats(void)
	   ---
	   Description:
	   ---
	      Returns the current RMS volume coefficients in (L)eft and (R)ight
	      channels.
	         low-order word: RMS volume in R channel
	         hi-order word:  RMS volume in L channel
	      Range from 0 (silence) to $7FFF (maximum) on each channel.
	   ---
	   Remarks:
	   ---
	      This function is useful for updating a VU meter, like the one
	      included in the example application. It's recommended to rescale
	      the output to log10 (decibels or dB for short), because human ears
	      track volume changes in a dB scale. You may call uFMOD_GetStats()
	      as often as you like, but take in mind that uFMOD updates both
	      channel RMS volumes every 20-40ms, depending on the output sampling
	      rate. So, calling uFMOD_GetStats about 16 times a second whould be
	      quite enough to track volume changes very closely.
	*/
//C++ TO C# CONVERTER NOTE: __stdcall is not available in C#:
//ORIGINAL LINE: uint __stdcall uFMOD_GetStats();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//uint uFMOD_GetStats();

	/* unsigned int uFMOD_GetRowOrder(void)
	   ---
	   Description:
	   ---
	      Returns the currently playing row and order.
	         low-order word: row
	         hi-order word:  order
	   ---
	   Remarks:
	   ---
	      This function is useful for synchronization. uFMOD updates both
	      row and order values every 20-40ms, depending on the output sampling
	      rate. So, calling uFMOD_GetRowOrder about 16 times a second whould be
	      quite enough to track row and order progress very closely.
	*/
//C++ TO C# CONVERTER NOTE: __stdcall is not available in C#:
//ORIGINAL LINE: uint __stdcall uFMOD_GetRowOrder();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//uint uFMOD_GetRowOrder();

	/* unsigned int uFMOD_GetTime(void)
	   ---
	   Description:
	   ---
	      Returns the time in milliseconds since the song was started.
	   ---
	   Remarks:
	   ---
	      This function is useful for synchronizing purposes. In fact, it is
	      more precise than a regular timer in Win32. Multimedia applications
	      can use uFMOD_GetTime to synchronize GFX to sound, for example. An
	      XM player can use this function to update a progress meter.
	*/
//C++ TO C# CONVERTER NOTE: __stdcall is not available in C#:
//ORIGINAL LINE: uint __stdcall uFMOD_GetTime();
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//uint uFMOD_GetTime();

	/* unsigned char* uFMOD_GetTitle(void)
	   ---
	   Description:
	   ---
	      Returns the current song's title.
	   ---
	   Remarks:
	   ---
	      Not every song has a title, so be prepared to get an empty string.
	      The string format may be ANSI or Unicode debending on the UF_UFS
	      settings used while recompiling the library.
	*/
	#if UNICODE
//C++ TO C# CONVERTER NOTE: __stdcall is not available in C#:
//ORIGINAL LINE: ushort* __stdcall uFMOD_GetTitle();
//C++ TO C# CONVERTER WARNING: C# has no equivalent to methods returning pointers to value types:
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//	ushort uFMOD_GetTitle();
	#else
//C++ TO C# CONVERTER NOTE: __stdcall is not available in C#:
//ORIGINAL LINE: byte* __stdcall uFMOD_GetTitle();
//C++ TO C# CONVERTER WARNING: C# has no equivalent to methods returning pointers to value types:
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//	byte uFMOD_GetTitle();
	#endif

	/* void uFMOD_SetVolume(
	      unsigned int vol
	   )
	   ---
	   Description:
	   ---
	      Sets the global volume. The volume scale is linear.
	   ---
	   Parameters:
	   ---
	      vol
	         New volume. Range: from uFMOD_MIN_VOL (muting) to uFMOD_MAX_VOL
	         (maximum volume). Any value above uFMOD_MAX_VOL maps to maximum
	         volume.
	   ---
	   Remarks:
	   ---
	      uFMOD internally converts the given values to a logarithmic scale (dB).
	      Maximum volume is set by default. The volume value is preserved across
	      uFMOD_PlaySong calls. You can set the desired volume level before
	      actually starting to play a song.
	      You can use WINMM waveOutSetVolume function to control the L and R
	      channels volumes separately. It also has a wider range than
	      uFMOD_SetVolume, sometimes allowing to amplify the sound volume as well,
	      as opposed to uFMOD_SetVolume only being able to attenuate it. The bad
	      things about waveOutSetVolume is that it may produce clicks and it's
	      hardware dependent.
	*/
//C++ TO C# CONVERTER NOTE: __stdcall is not available in C#:
//ORIGINAL LINE: void __stdcall uFMOD_SetVolume(uint);
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//void uFMOD_SetVolume(uint NamelessParameter);

	#if BENCHMARK
		/* uFMOD_tsc holds a performance counter. It measures the number of
		   clock cycles consumed in the internal thread to produce ~21 ms of
		   sound @ 48KHz. The lower - the better. Set UF_MODE to BENCHMARK
		   and recompile the library to make uFMOD_tsc available.
		*/
		public static uint uFMOD_tsc;
	#endif

	#if __cplusplus
	#endif

	//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
	//ORIGINAL LINE: #define uFMOD_DEFAULT_VOL uFMOD_MAX_VOL

}