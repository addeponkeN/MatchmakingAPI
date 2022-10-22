using System.Text.Json;

namespace Rovio.Utility;

public static class JsonHelper
{
    public static bool Save<T>(string filePath, T objectToWrite) where T : new()
    {
        TextWriter writer = null;
        try
        {
            var contentsToWriteToFile = JsonSerializer.Serialize(objectToWrite);
            writer = new StreamWriter(filePath, false);
            writer.Write(contentsToWriteToFile);
        }
        catch
        {
            return false;
        }
        finally
        {
            if(writer != null)
                writer.Close();
        }

        return true;
    }
}