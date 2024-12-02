using System;

public class Microwave
{
    public string StartHeating(int timeInSeconds, int? powerLevel = null)
    {
        // Configura a potência padrão para 10
        int power = powerLevel ?? 10;

        // Validações
        if (timeInSeconds < 1 || timeInSeconds > 120)
        {
            throw new ArgumentException("O tempo deve estar entre 1 e 120 segundos. Por favor, insira um valor válido.");
        }

        if (power < 1 || power > 10)
        {
            throw new ArgumentException("A potência deve estar entre 1 e 10. Por favor, insira um valor válido.");
        }

        // Ajuste de tempo
        string formattedTime = FormatTime(timeInSeconds);

        // Retorno do status do aquecimento
        return $"Aquecendo por {formattedTime} na potência {power}.";
    }

    private string FormatTime(int seconds)
    {
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;

        if (minutes > 0)
        {
            return $"{minutes}:{remainingSeconds:D2}";
        }
        return $"{seconds} segundos";
    }
}
