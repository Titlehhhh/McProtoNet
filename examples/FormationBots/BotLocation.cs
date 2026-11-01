namespace FormationBots;

// record-КЛАСС: ссылка меняется атомарно, читатели из других потоков не видят рваных координат
public sealed record BotLocation(double X, double Y, double Z, float Yaw, float Pitch);
