public class CustomProgram
{
    public string Name { get; set; }          // Nome do programa (obrigatório)
    public string Food { get; set; }          // Nome do alimento (obrigatório)
    public int Time { get; set; }             // Tempo em segundos (obrigatório)
    public int Power { get; set; }            // Potência (1-10) (obrigatório)
    public string HeatingString { get; set; } // Caractere de aquecimento (obrigatório)
    public string? Instructions { get; set; } // Instruções (opcional)
}
