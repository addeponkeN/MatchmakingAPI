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
    
    public static T? Load<T>(string filePath) where T : new()
    {
        TextReader reader = null;
        try
        {
            reader = new StreamReader(filePath);
            var fileContents = reader.ReadToEnd();
            return JsonSerializer.Deserialize<T>(fileContents);
        }
        catch
        {
            return default;
        }
        finally
        {
            if(reader != null)
                reader.Close();
        }
    }
}