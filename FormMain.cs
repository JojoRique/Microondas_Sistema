using System;
using System.Windows.Forms;


public class FormMain : Form
{
    // Variáveis globais
    private List<PredefinedProgram> predefinedPrograms; // Lista de programas pré-definidos
    private ComboBox cmbPrograms; // ComboBox para seleção de programas

    private TextBox txtTime;
    private TextBox txtPower;
    private Button btnStart;
    private Label lblDisplay;
    private TextBox activeTextBox; // Controla qual campo está ativo para o teclado digital

    private System.Windows.Forms.Timer timer;

    private int remainingTime; // Tempo restante em segundos
    private bool isHeating;   // Indica se o aquecimento está em andamento

    private string heatingProgress; // String informativa do aquecimento
    private int power;              // Potência atual

    private bool isPaused; // Indica se o aquecimento está pausado

    private bool isManualProgram = true; // Identifica se o programa atual é manual ou pré-definido.

    public FormMain()
    {
        // Configuração do Timer
    timer = new System.Windows.Forms.Timer();
    timer.Interval = 1000; // 1 segundo
    timer.Tick += TimerTick; // Evento para reduzir o tempo  

        // Inicializar a lista de programas pré-definidos
 predefinedPrograms = new List<PredefinedProgram>
{
    new PredefinedProgram
    {
        Name = "Pipoca",
        Food = "Pipoca (de micro-ondas)",
        Time = 180,
        Power = 7,
        HeatingString = "-" // Caractere que simula os "estouros" de pipoca
    },
    new PredefinedProgram
    {
        Name = "Leite",
        Food = "Leite",
        Time = 300,
        Power = 5,
        HeatingString = "~" // Caractere que simboliza líquido esquentando
    },
    new PredefinedProgram
    {
        Name = "Carnes de boi",
        Food = "Carne em pedaço ou fatias",
        Time = 840,
        Power = 4,
        HeatingString = "$" // Caractere que remete ao "peso" e valor da carne
    },
    new PredefinedProgram
    {
        Name = "Frango",
        Food = "Frango (qualquer corte)",
        Time = 480,
        Power = 7,
        HeatingString = "*" // Caractere "estrela" para simbolizar algo especial como frango
    },
    new PredefinedProgram
    {
        Name = "Feijão",
        Food = "Feijão congelado",
        Time = 480,
        Power = 9,
        HeatingString = "#" // Caractere que remete à "textura" do feijão
    }
};


    // Tempo
    Label lblTime = new Label { Text = "Tempo (s):", Left = 20, Top = 20, Width = 100 };
    txtTime = new TextBox { Left = 120, Top = 20, Width = 100 };
    txtTime.Enter += (sender, e) => activeTextBox = txtTime;

    // Validação em tempo real para o campo de tempo
txtTime.TextChanged += (sender, e) =>
{
    if (int.TryParse(txtTime.Text, out int time))
    {
        if (time < 1 || time > 120)
        {
            txtTime.ForeColor = System.Drawing.Color.Red; // Destaca o erro
        }
        else
        {
            txtTime.ForeColor = System.Drawing.Color.Black; // Remove o destaque
        }
    }
    else
    {
        txtTime.ForeColor = System.Drawing.Color.Red; // Caso não seja um número válido
    }
};

    // Potência
    Label lblPower = new Label { Text = "Potência (1-10):", Left = 20, Top = 60, Width = 100 };
    txtPower = new TextBox { Left = 120, Top = 60, Width = 100 };
    txtPower.Enter += (sender, e) => activeTextBox = txtPower;
    

    // Botão iniciar
    btnStart = new Button { Text = "Iniciar", Left = 120, Top = 100, Width = 100 };
 btnStart.Click += (sender, e) =>
{
    // Validação de tempo
    if (!int.TryParse(txtTime.Text, out int time) || time < 1 || time > 120)
    {
        MessageBox.Show("O tempo deve estar entre 1 e 120 segundos. Por favor, insira um valor válido.",
                        "Erro de Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return; // Impede o início do aquecimento
    }

    // Validação de potência
    if (string.IsNullOrWhiteSpace(txtPower.Text))
    {
        // Define o valor padrão de potência como 10
        txtPower.Text = "10";
    }

    if (!int.TryParse(txtPower.Text, out int enteredPower) || enteredPower < 1 || enteredPower > 10)
    {
        MessageBox.Show("A potência deve estar entre 1 e 10. Por favor, insira um valor válido.",
                        "Erro de Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return; // Impede o início do aquecimento
    }

    // Se passou na validação, inicia o aquecimento
    StartHeating(sender, e);
};


    // Botão de início rápido
    Button btnQuickStart = new Button { Text = "Início Rápido", Left = 240, Top = 100, Width = 120 };
    btnQuickStart.Click += new EventHandler(QuickStart);

    // Botão de pausa/cancelamento
    Button btnPauseCancel = new Button { Text = "Pausar/Cancelar", Left = 380, Top = 100, Width = 120 };
    btnPauseCancel.Click += new EventHandler(PauseOrCancel);

    // Display
    lblDisplay = new Label { Text = "", Left = 20, Top = 150, Width = 300, Height = 30, BorderStyle = BorderStyle.FixedSingle };

    // Criar o ComboBox para selecionar programas
    cmbPrograms = new ComboBox { Left = 20, Top = 300, Width = 200 };
    cmbPrograms.DataSource = predefinedPrograms;
    cmbPrograms.DisplayMember = "Name";

    // Configurar evento para preencher tempo e potência automaticamente
// Adicionar botões dinâmicos para programas pré-definidos
// Adicionar botões dinâmicos para programas pré-definidos (organizados em 2 colunas)
int programButtonTop = 300; // Posição inicial vertical
int programButtonLeft = 250; // Posição inicial horizontal
int buttonCounter = 0;

foreach (var program in predefinedPrograms)
{
    Button programButton = new Button
    {
        Text = program.Name,
        Width = 140,
        Height = 30,
        Left = programButtonLeft,
        Top = programButtonTop
    };

    // Configurar evento para iniciar o programa ao clicar
programButton.Click += (sender, e) =>
{
    // Atualiza o ComboBox para refletir o programa selecionado
    cmbPrograms.SelectedItem = program;

    // Preenche os campos de tempo e potência
    txtTime.Text = program.Time.ToString();
    txtPower.Text = program.Power.ToString();

    // Desabilita os campos para evitar alterações manuais
    txtTime.Enabled = false;
    txtPower.Enabled = false;

    // Define como programa pré-definido
    isManualProgram = false;

    // Define o caractere de aquecimento do programa pré-definido
    currentHeatingCharacter = program.HeatingString;

    // Define os valores no sistema
    remainingTime = program.Time;
    power = program.Power;

    // Reseta string de progresso e inicia o programa
    heatingProgress = "";
    isHeating = true;

    lblDisplay.Text = $"Programa: {program.Name}\nAlimento: {program.Food}\nInstruções: {program.Instructions}\nAquecendo: {FormatTime(remainingTime)}";

    // Inicia o timer
    timer.Start();
};



    // Adiciona o botão à interface
    this.Controls.Add(programButton);

    // Alternar entre colunas
    buttonCounter++;
    if (buttonCounter % 2 == 0)
    {
        programButtonTop += 40; // Move para a próxima linha
        programButtonLeft = 250; // Volta para a primeira coluna
    }
    else
    {
        programButtonLeft += 160; // Move para a segunda coluna
    }
}

    // Teclado Digital
    Panel keypad = CreateKeypad();
    keypad.Left = 20;
    keypad.Top = 200;

    // Adicionando os controles
    this.Controls.Add(lblTime);
    this.Controls.Add(txtTime);
    this.Controls.Add(lblPower);
    this.Controls.Add(txtPower);
    this.Controls.Add(btnStart);
    this.Controls.Add(btnQuickStart); // Botão de início rápido
    this.Controls.Add(lblDisplay);
    this.Controls.Add(keypad);
    this.Controls.Add(btnPauseCancel);
    this.Controls.Add(cmbPrograms);
    // Define o campo ativo inicial como o campo de tempo
    activeTextBox = txtTime;
}

private void StartHeating(object? sender, EventArgs e)
{
    try
    {
        // Verifica se há um programa pré-definido ativo
        if (!isManualProgram && cmbPrograms.SelectedItem is PredefinedProgram)
        {
            MessageBox.Show("Não é permitido alterar o tempo de programas pré-definidos.",
                            "Erro de Operação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return; // Cancela a operação
        }

        // Lógica para entradas manuais
        if (isManualProgram)
        {
            // Para entradas manuais, aceita qualquer valor positivo para tempo
            if (!int.TryParse(txtTime.Text, out int manualTime) || manualTime < 1)
            {
                throw new ArgumentException("O tempo deve ser um número positivo. Por favor, insira um valor válido.");
            }

            // Define potência padrão caso esteja vazia
            if (string.IsNullOrWhiteSpace(txtPower.Text))
            {
                txtPower.Text = "10"; // Potência padrão
            }

            if (!int.TryParse(txtPower.Text, out int manualPower) || manualPower < 1 || manualPower > 10)
            {
                throw new ArgumentException("A potência deve estar entre 1 e 10. Por favor, insira um valor válido.");
            }

            // Atualiza as variáveis
            remainingTime = manualTime;
            power = manualPower;
            currentHeatingCharacter = "."; // Caractere padrão para aquecimento manual
        }
        else
        {
            // Lógica para programas pré-definidos (mantém o tempo limitado)
            if (!int.TryParse(txtTime.Text, out int predefinedTime) || predefinedTime < 1 || predefinedTime > 120)
            {
                throw new ArgumentException("O tempo para programas pré-definidos deve estar entre 1 e 120 segundos.");
            }

            // Atualiza as variáveis
            remainingTime = predefinedTime;
            power = int.Parse(txtPower.Text); // Potência já está preenchida pelo programa
        }

        isHeating = true;
        heatingProgress = "";
        lblDisplay.Text = "Aquecendo...";

        // Inicia o temporizador
        timer.Start();
    }
    catch (ArgumentException ex)
    {
        MessageBox.Show(ex.Message, "Erro de Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
    catch (Exception ex)
    {
        MessageBox.Show("Erro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}



    private string FormatTime(int seconds)
    {
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;
        return minutes > 0 ? $"{minutes}:{remainingSeconds:D2}" : $"{seconds} segundos";
    }

    private Panel CreateKeypad()
    {
        Panel panel = new Panel();
        panel.Width = 200;
        panel.Height = 300;

        int buttonWidth = 50;
        int buttonHeight = 50;
        int padding = 10;

        string[] buttons = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" }; // Remove "C"
        int row = 0, col = 0;

        foreach (string btnText in buttons)
        {
            Button button = new Button
            {
                Text = btnText,
                Width = buttonWidth,
                Height = buttonHeight,
                Left = col * (buttonWidth + padding),
                Top = row * (buttonHeight + padding)
            };
            button.Click += KeypadButtonClick;
            panel.Controls.Add(button);

            col++;
            if (col == 3) // Mudança de linha
            {
                col = 0;
                row++;
            }
        }

        return panel;
    }


    private void KeypadButtonClick(object sender, EventArgs e)
    {
        Button button = sender as Button;
        if (button.Text == "C")
        {
            activeTextBox.Text = ""; // Limpa o campo ativo
        }
        else
        {
            activeTextBox.Text += button.Text; // Adiciona o texto ao campo ativo
        }
    }

private void QuickStart(object? sender, EventArgs e)
{
    try
    {
        if (!isManualProgram)
        {
            MessageBox.Show("Não é permitido alterar o tempo de programas pré-definidos.",
                            "Erro de Operação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Se já está aquecendo manualmente, adiciona 30 segundos
        if (isHeating)
        {
            remainingTime += 30;
            lblDisplay.Text = $"Aquecendo: {FormatTime(remainingTime)}\n{heatingProgress}";
            return;
        }

        // Tempo e potência padrão para início rápido
        txtTime.Text = "30";
        txtPower.Text = "10";

        remainingTime = 30;
        power = 10;
        currentHeatingCharacter = "."; // Caractere padrão para aquecimento manual

        isHeating = true;
        heatingProgress = "";
        lblDisplay.Text = $"Aquecendo: {FormatTime(remainingTime)}\n{heatingProgress}";

        // Inicia o temporizador
        timer.Start();
    }
    catch (Exception ex)
    {
        MessageBox.Show("Erro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

private string currentHeatingCharacter = "."; // Valor padrão para entradas manuais

private void TimerTick(object? sender, EventArgs e)
{
    if (remainingTime > 0)
    {
        remainingTime--;

        // Adiciona caracteres com base na potência
        heatingProgress += new string(currentHeatingCharacter[0], power);

        // Atualiza o display
        lblDisplay.Text = $"Aquecendo: {FormatTime(remainingTime)}\n{heatingProgress}";
    }
    else
    {
        // Final do aquecimento
        timer.Stop();
        isHeating = false;

        heatingProgress += " Aquecimento concluído.";
        lblDisplay.Text = heatingProgress;

        // Reseta a string de progresso para a próxima execução
        heatingProgress = string.Empty;
        currentHeatingCharacter = "."; // Reseta para o caractere padrão
    }
}

private void PauseOrCancel(object? sender, EventArgs e)
{
    if (!isHeating && !isPaused)
    {
        // Caso o aquecimento não tenha iniciado ou tenha sido cancelado
        txtTime.Text = string.Empty;
        txtPower.Text = string.Empty;

        // Reabilita os campos para permitir nova entrada
        txtTime.Enabled = true;
        txtPower.Enabled = true;

        lblDisplay.Text = "Aquecimento cancelado.";
        return;
    }

    if (isHeating)
    {
        // Pausar o aquecimento
        timer.Stop();
        isPaused = true;
        isHeating = false;
        lblDisplay.Text = "Aquecimento pausado.";
    }
    else if (isPaused)
    {
        // Cancelar o aquecimento
        isPaused = false;
        remainingTime = 0;

        txtTime.Text = string.Empty;
        txtPower.Text = string.Empty;

        // Reabilita os campos para permitir nova entrada
        txtTime.Enabled = true;
        txtPower.Enabled = true;

        lblDisplay.Text = "Aquecimento cancelado.";
        heatingProgress = string.Empty;
    }
}
}