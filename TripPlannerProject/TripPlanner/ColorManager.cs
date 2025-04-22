namespace TripPlanner;

public static class ColorManager
{    
    public static (byte, byte, byte) GenerateRandomColor()
    {
        Random random = new Random();
        
        byte red = (byte)random.Next(0, 256);
        byte green = (byte)random.Next(0, 256);
        byte blue = (byte)random.Next(0, 256);
        
        return (red, green, blue);
    }
}