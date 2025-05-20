using System.Diagnostics;
using System;

namespace CodeGeneration
{
    public class CodeGenerator
    {
        public void ExecutePlanting(Robot robot, int[][] colorMap, int width, int height, SeedingParameters parameters)
        {
            try
            {
                robot.CheckBoards();
                Debug.WriteLine("Начало процедуры хоминга");
                robot.Home();

                for (int y = 0; y < height; y++)
                {
                    bool leftToRight = (y % 2) == 0;

                    for (int x = 0; x < width; x++)
                    {
                        int actualX = leftToRight ? x : (width - 1 - x);
                        int containerId = colorMap[y][actualX];

                        if ((x > 0) || (y > 0))
                        {
                            if (x > 0)
                            {
                                int steps = (int)(parameters.StepX * (leftToRight ? 1 : -1));
                                Debug.WriteLine($"Перемещение по X на {steps} шагов");
                                robot.Move(leftToRight ? Direction.Right : Direction.Left, steps > 0 ? steps : -steps);
                            }

                            if ((y > 0) && (x == 0))
                            {
                                Debug.WriteLine($"Перемещение по Y на {parameters.StepY} шагов");
                                robot.Move(Direction.Up, (int)parameters.StepY);
                            }
                        }

                        Debug.WriteLine($"Посев из контейнера {containerId}");
                        robot.Seed(containerId);
                    }
                }

                Debug.WriteLine("Возврат в исходную позицию");
                robot.Home();
                robot.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка выполнения: {ex.Message}");
                EmergencyStop(robot);
                throw;
            }
        }

        private void EmergencyStop(Robot robot)
        {
            Debug.WriteLine("АВАРИЙНАЯ ОСТАНОВКА!");
            robot.XYPlane.MotorX.DisableDriver();
            robot.XYPlane.MotorY.DisableDriver();
            robot.Seeder.MotorZ.DisableDriver();
        }
    }
}

