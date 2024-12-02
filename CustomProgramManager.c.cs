using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public static class CustomProgramManager
{
    private const string FileName = "customPrograms.json"; // Nome do arquivo JSON

    // Método para salvar os programas customizados
    public static void Save(List<CustomProgram> customPrograms)
    {
        string json = JsonConvert.SerializeObject(customPrograms, Formatting.Indented);
        File.WriteAllText(FileName, json);
    }

    // Método para carregar os programas customizados
    public static List<CustomProgram> Load()
    {
        if (!File.Exists(FileName))
            return new List<CustomProgram>();

        string json = File.ReadAllText(FileName);
        return JsonConvert.DeserializeObject<List<CustomProgram>>(json) ?? new List<CustomProgram>();
    }
}
