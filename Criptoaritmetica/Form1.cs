using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Criptoaritmetica
{
    public partial class frmMain : Form
    {
        cPopulacao mundo;

        public frmMain()
        {
            InitializeComponent();
        }

        private void btnProcessa_Click(object sender, EventArgs e)
        {
            if ((txtString1.Text.Length > 0) &&
                (txtString2.Text.Length > 0) &&
                (txtResultado.Text.Length > 0) &&
                (txtTamanhoPopulacao1.Text.Length > 0) &&
                (txtTaxaCrossover1.Text.Length > 0) &&
                (txtNumeroGeracao1.Text.Length > 0) &&
                (txtTaxaMutacao1.Text.Length > 0))
            {
                ltbSolucao.Items.Clear();
                ltbSolucao.Items.Add("Processando..........");
                ltbSolucao.Update();

                Int32 numeroSelecionado = (Int32)(((float)Convert.ToInt32(txtTaxaCrossover1.Text) / 100.0f) * Convert.ToInt32(txtTamanhoPopulacao1.Text));
                Int32 tipoAptidao;

                if (rdbModulo.Checked)
                {
                    tipoAptidao = 0;
                }
                else
                if (rdbSomaDigito.Checked)
                {
                    tipoAptidao = 1;
                }
                else
                {
                    tipoAptidao = 2;
                }

                cIndividuo solucao = AlgoritmoGenetico(txtString1.Text,
                                                       txtString2.Text,
                                                       txtResultado.Text,
                                                       Convert.ToInt32(txtTamanhoPopulacao1.Text),
                                                       Convert.ToInt32(txtNumeroGeracao1.Text),
                                                       numeroSelecionado,
                                                       Convert.ToInt32(txtTaxaMutacao1.Text),
                                                       rdbRoleta.Checked ? 0 : 1,
                                                       rdbCrossoverPMX.Checked ? 0 : 1,
                                                       tipoAptidao);

                if (solucao == null)
                {
                    // Sem solução para tais critérios.
                    String str1 = "Melhor Solução: ";
                    //String str2 = "Melhor Solução: ";
                    String str2 = "";

                    for (int i = 0; i < cIndividuo.quantidade; i++)
                    {
                        str1 += cIndividuo.cadeia[i] + " ";
                        str2 += mundo.populacao[mundo.populacao.Count - 1].individuo[i].ToString() + " ";
                    }

                    ltbSolucao.Items.Clear();
                    
                    ltbSolucao.Items.Add(str1);
                    ltbSolucao.Items.Add(str2);
                    ltbSolucao.Items.Add("Aptidão: " + mundo.populacao[mundo.populacao.Count - 1].aptidao.ToString());

                    Console.WriteLine(str2 + " Aptidão: " + mundo.populacao[mundo.populacao.Count - 1].aptidao.ToString());
                }
                else
                {
                    // Solução encontrada.
                    String str1 = "Solução: ";
                    //String str2 = "Solução: ";
                    String str2 = "";

                    for (int i = 0; i < cIndividuo.quantidade; i++)
                    {
                        str1 += cIndividuo.cadeia[i] + " ";
                        str2 += solucao.individuo[i].ToString() + " ";
                    }

                    ltbSolucao.Items.Clear();
                    
                    ltbSolucao.Items.Add(str1);
                    ltbSolucao.Items.Add(str2);
                    ltbSolucao.Items.Add("Número de Gerações: " + mundo.numeroGeracao.ToString());
                    Console.WriteLine(str2 + " Número de Gerações: " + mundo.numeroGeracao.ToString());
                }
            }
            else
            {
                ltbSolucao.Items.Clear();
                ltbSolucao.Items.Add("Parâmetro(s) Incorreto(s).");
            }
        }

        private cIndividuo AlgoritmoGenetico(String string1, String string2, String resultado, Int32 tamanhoPopulacao, Int32 numeroGeracao, Int32 numeroPopulacaoSelecionada, Int32 probabilidadeMutacao, Int32 tipoSelecao, Int32 tipoCrossover, Int32 tipoAptidao)
        {
            cIndividuo solucao;

            mundo = new cPopulacao();

            // 1. Criando a população inicial.
            mundo.CriarPopulacao(string1, string2, resultado, tamanhoPopulacao, tipoSelecao, tipoAptidao);

            // 5. Verifica se encontrou alguma solução de acordo com o número de gerações.
            while (((solucao = mundo.CriterioParada()) == null) && (mundo.numeroGeracao < numeroGeracao))
            {
                // 2. Seleciona população.
                mundo.Seleciona(numeroPopulacaoSelecionada, tipoSelecao);

                // 3. Reprodução (Crossover e Mutação).
                mundo.Reproducao(probabilidadeMutacao, tipoSelecao, tipoAptidao);

                // 4. Evolução (Elimina os indivíduos menos aptos).
                mundo.Evolucao();
            }

            return solucao;
        }

        private void txtGeracao_TextChanged(object sender, EventArgs e)
        {
            foreach(Char caracter in txtNumeroGeracao1.Text)
            {
                if (!(caracter.Equals('0') ||
                      caracter.Equals('1') ||
                      caracter.Equals('2') ||
                      caracter.Equals('3') ||
                      caracter.Equals('4') ||
                      caracter.Equals('5') ||
                      caracter.Equals('6') ||
                      caracter.Equals('7') ||
                      caracter.Equals('8') ||
                      caracter.Equals('9')))
                {
                    txtNumeroGeracao1.Text = txtNumeroGeracao1.Text.Trim(caracter);
                }
            }
        }

        private void txtPopulacao_TextChanged(object sender, EventArgs e)
        {
            foreach (Char caracter in txtTamanhoPopulacao1.Text)
            {
                if (!(caracter.Equals('0') ||
                      caracter.Equals('1') ||
                      caracter.Equals('2') ||
                      caracter.Equals('3') ||
                      caracter.Equals('4') ||
                      caracter.Equals('5') ||
                      caracter.Equals('6') ||
                      caracter.Equals('7') ||
                      caracter.Equals('8') ||
                      caracter.Equals('9')))
                {
                    txtTamanhoPopulacao1.Text = txtTamanhoPopulacao1.Text.Trim(caracter);
                }
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            txtString1.Text   = "";
            txtString2.Text   = "";
            txtResultado.Text = "";
            txtNumeroGeracao1.Text   = "";
            txtNumeroGeracao2.Text = "";
            txtTamanhoPopulacao1.Text = "";
            txtTamanhoPopulacao2.Text = "";
            txtTaxaMutacao1.Text = "";
            txtTaxaMutacao2.Text = "";
            txtTaxaCrossover1.Text = "";
            txtTaxaCrossover2.Text = "";
            txtNumeroTeste.Text = "";
            rdbNumeroGeracao.Checked = true;
            rdbCrossoverPMX.Checked = true;
            rdbRoleta.Checked = true;
            ltbSolucao.Items.Clear();
            txtString1.Select();
        }

        private void rdbNumeroGeracao_CheckedChanged(object sender, EventArgs e)
        {
            txtNumeroGeracao2.Enabled = (rdbNumeroGeracao.Checked ? true : false);
        }

        private void rdbTamanhoPopulacao_CheckedChanged(object sender, EventArgs e)
        {
            txtTamanhoPopulacao2.Enabled = (rdbTamanhoPopulacao.Checked ? true : false);
        }

        private void rdbTaxaCrossover_CheckedChanged(object sender, EventArgs e)
        {
            txtTaxaCrossover2.Enabled = (rdbTaxaCrossover.Checked ? true : false);
        }

        private void rdbTaxaMutacao_CheckedChanged(object sender, EventArgs e)
        {
            txtTaxaMutacao2.Enabled = (rdbTaxaMutacao.Checked ? true : false);
        }

        private void txtNumeroGeracao2_TextChanged(object sender, EventArgs e)
        {
            foreach (Char caracter in txtNumeroGeracao2.Text)
            {
                if (!(caracter.Equals('0') ||
                      caracter.Equals('1') ||
                      caracter.Equals('2') ||
                      caracter.Equals('3') ||
                      caracter.Equals('4') ||
                      caracter.Equals('5') ||
                      caracter.Equals('6') ||
                      caracter.Equals('7') ||
                      caracter.Equals('8') ||
                      caracter.Equals('9')))
                {
                    txtNumeroGeracao2.Text = txtNumeroGeracao2.Text.Trim(caracter);
                }
            }
        }

        private void txtTamanhoPopulacao2_TextChanged(object sender, EventArgs e)
        {
            foreach (Char caracter in txtTamanhoPopulacao2.Text)
            {
                if (!(caracter.Equals('0') ||
                      caracter.Equals('1') ||
                      caracter.Equals('2') ||
                      caracter.Equals('3') ||
                      caracter.Equals('4') ||
                      caracter.Equals('5') ||
                      caracter.Equals('6') ||
                      caracter.Equals('7') ||
                      caracter.Equals('8') ||
                      caracter.Equals('9')))
                {
                    txtTamanhoPopulacao2.Text = txtTamanhoPopulacao2.Text.Trim(caracter);
                }
            }
        }

        private void txtTaxaCrossover2_TextChanged(object sender, EventArgs e)
        {
            foreach (Char caracter in txtTaxaCrossover2.Text)
            {
                if (!(caracter.Equals('0') ||
                      caracter.Equals('1') ||
                      caracter.Equals('2') ||
                      caracter.Equals('3') ||
                      caracter.Equals('4') ||
                      caracter.Equals('5') ||
                      caracter.Equals('6') ||
                      caracter.Equals('7') ||
                      caracter.Equals('8') ||
                      caracter.Equals('9')))
                {
                    txtTaxaCrossover2.Text = txtTaxaCrossover2.Text.Trim(caracter);
                }
            }
        }

        private void txtTaxaMutacao2_TextChanged(object sender, EventArgs e)
        {
            foreach (Char caracter in txtTaxaMutacao2.Text)
            {
                if (!(caracter.Equals('0') ||
                      caracter.Equals('1') ||
                      caracter.Equals('2') ||
                      caracter.Equals('3') ||
                      caracter.Equals('4') ||
                      caracter.Equals('5') ||
                      caracter.Equals('6') ||
                      caracter.Equals('7') ||
                      caracter.Equals('8') ||
                      caracter.Equals('9')))
                {
                    txtTaxaMutacao2.Text = txtTaxaMutacao2.Text.Trim(caracter);
                }
            }
        }

        private void txtNumeroTeste_TextChanged(object sender, EventArgs e)
        {
            foreach (Char caracter in txtNumeroTeste.Text)
            {
                if (!(caracter.Equals('0') ||
                      caracter.Equals('1') ||
                      caracter.Equals('2') ||
                      caracter.Equals('3') ||
                      caracter.Equals('4') ||
                      caracter.Equals('5') ||
                      caracter.Equals('6') ||
                      caracter.Equals('7') ||
                      caracter.Equals('8') ||
                      caracter.Equals('9')))
                {
                    txtNumeroTeste.Text = txtNumeroTeste.Text.Trim(caracter);
                }
            }
        }
        
        private void frmMain_Load_1(object sender, EventArgs e)
        {
            txtString1.Select();
        }

        private void btnBuscaProcessa_Click(object sender, EventArgs e)
        {
            /*trd = new Thread(new ThreadStart(this.threadTask));
            trd.IsBackground = true;
            trd.Start();*/
            threadTask();
        }

        private void threadTask()
        {
            txtBuscaSolucao.Clear();
            txtBuscaSolucao.Text = "Processando.......";
            txtBuscaSolucao.Update();
            txtBuscaSolucao.Clear();

            Int32 numeroGeracao;
            Int32 numeroGeracao1 = Convert.ToInt32(txtBuscaNumeroGeracao1.Text);
            Int32 numeroGeracao2 = Convert.ToInt32(txtBuscaNumeroGeracao2.Text);
            Int32 numeroGeracaoR = Convert.ToInt32(txtBuscaNumeroGeracaoRazao.Text);

            Int32 tamanhoPopulacao;
            Int32 tamanhoPopulacao1 = Convert.ToInt32(txtBuscaTamanhoPopulacao1.Text);
            Int32 tamanhoPopulacao2 = Convert.ToInt32(txtBuscaTamanhoPopulacao2.Text);
            Int32 tamanhoPopulacaoR = Convert.ToInt32(txtBuscaTamanhoPopulacaoRazao.Text);

            Int32 taxaSelecao;
            Int32 taxaSelecao1 = Convert.ToInt32(txtBuscaTaxaCrossover1.Text);
            Int32 taxaSelecao2 = Convert.ToInt32(txtBuscaTaxaCrossover2.Text);
            Int32 taxaSelecaoR = Convert.ToInt32(txtBuscaTaxaCrossoverRazao.Text);

            Int32 taxaMutacao;
            Int32 taxaMutacao1 = Convert.ToInt32(txtBuscaTaxaMutacao1.Text);
            Int32 taxaMutacao2 = Convert.ToInt32(txtBuscaTaxaMutacao2.Text);
            Int32 taxaMutacaoR = Convert.ToInt32(txtBuscaTaxaMutacaoRazao.Text);

            Int32 numeroTeste;
            Int32 numeroTeste1 = 1;
            Int32 numeroTeste2 = Convert.ToInt32(txtBuscaNumeroTeste.Text);

            String string1   = txtBuscaString1.Text.ToString();
            String string2   = txtBuscaString2.Text.ToString();
            String resultado = txtBuscaResultado.Text.ToString();

            Int32 numeroSolucao;
            Int32 numeroSelecao;
            cIndividuo solucao = null;

            // Melhores Parâmetros.
            Int32 melhorCrossover        = 0;
            Int32 melhorSelecao          = 0;
            Int32 melhorNumeroGeracao    = Int32.MaxValue;
            Int32 melhorTamanhoPopulacao = Int32.MaxValue;
            Int32 melhorTaxaSelecao      = Int32.MaxValue;
            Int32 melhorTaxaMutacao      = Int32.MaxValue;

            Int32 melhorNumeroSolucao = Int32.MinValue;
            float melhorMediaGeracao  = 0.0f;
            float somaMediaGeracao    = 0.0f;

            Int32 tipoAptidao;

            if (rdbBuscaModulo.Checked)
            {
                tipoAptidao = 0;
            }
            else
            if (rdbBuscaSomaDigito.Checked)
            {
                tipoAptidao = 1;
            }
            else
            {
                tipoAptidao = 2;
            }

            // Crossover: 1 - Crossover PMX
            //            0 - Crossover Cíclico
            for (Int32 crossover = 0; crossover < 2; crossover++)
            {
                // Seleção: 0 - Roleta
                //          1 - Torneio 3
                for (Int32 selecao = 0; selecao < 2; selecao++)
                {
                    Console.WriteLine("Processo de Crossover: " + (crossover == 0 ? "PMX" : "Cíclico"));
                    Console.WriteLine("Processo de Seleção: " + (selecao == 0 ? "Roleta" : "Torneio 3"));

                    // Número de Gerações.
                    for (numeroGeracao = numeroGeracao1; numeroGeracao <= numeroGeracao2; numeroGeracao+=numeroGeracaoR)
                    {
                        Console.WriteLine("Número da Geração: " + numeroGeracao.ToString());

                        // Tamanho da População.
                        for (tamanhoPopulacao = tamanhoPopulacao1; tamanhoPopulacao <= tamanhoPopulacao2; tamanhoPopulacao+=tamanhoPopulacaoR)
                        {
                            // Taxa de Seleção.
                            for (taxaSelecao = taxaSelecao1; taxaSelecao <= taxaSelecao2; taxaSelecao+=taxaSelecaoR)
                            {
                                numeroSelecao = (Int32)(((float)(taxaSelecao * tamanhoPopulacao)) / 100.0f);

                                // Taxa de Mutação.
                                for (taxaMutacao = taxaMutacao1; taxaMutacao <= taxaMutacao2; taxaMutacao+=taxaMutacaoR)
                                {
                                    numeroSolucao = 0;
                                    somaMediaGeracao = 0.0f;

                                    // Número de Testes.
                                    for (numeroTeste = numeroTeste1; numeroTeste <= numeroTeste2; numeroTeste++)
                                    {
                                        solucao = AlgoritmoGenetico(string1, string2, resultado, tamanhoPopulacao, numeroGeracao, numeroSelecao, taxaMutacao, selecao, crossover, tipoAptidao);

                                        if (solucao != null)
                                        {
                                            numeroSolucao++;
                                            somaMediaGeracao += (float)mundo.numeroGeracao;

                                            Console.Write("Teste " + numeroTeste.ToString() + ": ");
                                            for (Int32 P = 0; P < cIndividuo.quantidade; P++)
                                            {
                                                Console.Write(solucao.individuo[P].ToString() + " ");
                                            }
                                            Console.WriteLine("Número de Gerações: " + mundo.numeroGeracao.ToString());
                                        }
                                        else
                                        {
                                            Console.WriteLine("Teste " + numeroTeste.ToString() + ": SOLUÇÃO NÃO ÓTIMA. Melhor Aptidão: " + mundo.populacao[mundo.populacao.Count - 1].aptidao.ToString());
                                        }
                                    }

                                    somaMediaGeracao /= (float)numeroSolucao;

                                    // Melhores Parâmetros.
                                    if (melhorNumeroSolucao < numeroSolucao)
                                    {
                                        melhorNumeroSolucao    = numeroSolucao;
                                        melhorCrossover        = crossover;
                                        melhorSelecao          = selecao;
                                        melhorNumeroGeracao    = numeroGeracao;
                                        melhorTamanhoPopulacao = tamanhoPopulacao;
                                        melhorTaxaSelecao      = taxaSelecao;
                                        melhorTaxaMutacao      = taxaMutacao;
                                        melhorMediaGeracao     = somaMediaGeracao;
                                    }
                                    else
                                        if (melhorNumeroSolucao == numeroSolucao)
                                        {
                                            if (((Int32)melhorMediaGeracao) > ((Int32)somaMediaGeracao))
                                            {
                                                melhorCrossover        = crossover;
                                                melhorSelecao          = selecao;
                                                melhorNumeroGeracao    = numeroGeracao;
                                                melhorTamanhoPopulacao = tamanhoPopulacao;
                                                melhorTaxaSelecao      = taxaSelecao;
                                                melhorTaxaMutacao      = taxaMutacao;
                                                melhorMediaGeracao     = somaMediaGeracao;
                                            }
                                            else
                                                if (((Int32)melhorMediaGeracao) == ((Int32)somaMediaGeracao))
                                                {
                                                    if (melhorTamanhoPopulacao > tamanhoPopulacao)
                                                    {
                                                        melhorCrossover        = crossover;
                                                        melhorSelecao          = selecao;
                                                        melhorNumeroGeracao    = numeroGeracao;
                                                        melhorTamanhoPopulacao = tamanhoPopulacao;
                                                        melhorTaxaSelecao      = taxaSelecao;
                                                        melhorTaxaMutacao      = taxaMutacao;
                                                    }
                                                    else
                                                        if (melhorTamanhoPopulacao == tamanhoPopulacao)
                                                        {
                                                            if (melhorTaxaSelecao > selecao)
                                                            {
                                                                melhorCrossover     = crossover;
                                                                melhorSelecao       = selecao;
                                                                melhorNumeroGeracao = numeroGeracao;
                                                                melhorTaxaSelecao   = taxaSelecao;
                                                                melhorTaxaMutacao   = taxaMutacao;
                                                            }
                                                        }
                                                }
                                        }
                                }
                            }
                        }
                    }
                }
            }

            txtBuscaSolucao.Text += "PARÂMETROS DE BUSCA:\r\n\r\n";
            txtBuscaSolucao.Text += "String 1:  " + txtBuscaString1.Text.ToString() + "\r\n";
            txtBuscaSolucao.Text += "String 2:  " + txtBuscaString2.Text.ToString() + "\r\n";
            txtBuscaSolucao.Text += "Resultado: " + txtBuscaResultado.Text.ToString() + "\r\n\r\n";
            txtBuscaSolucao.Text += "Número de Gerações:   Início: " + txtBuscaNumeroGeracao1.Text.ToString() + "\r\n";
            txtBuscaSolucao.Text += "                      Fim:    " + txtBuscaNumeroGeracao2.Text.ToString() + "\r\n";
            txtBuscaSolucao.Text += "                      Razão:  " + txtBuscaNumeroGeracaoRazao.Text.ToString() + "\r\n\r\n";
            txtBuscaSolucao.Text += "Tamanho da População: Início: " + txtBuscaTamanhoPopulacao1.Text.ToString() + "\r\n";
            txtBuscaSolucao.Text += "                      Fim:    " + txtBuscaTamanhoPopulacao2.Text.ToString() + "\r\n";
            txtBuscaSolucao.Text += "                      Razão:  " + txtBuscaTamanhoPopulacaoRazao.Text.ToString() + "\r\n\r\n";
            txtBuscaSolucao.Text += "Taxa de Crossover:    Início: " + txtBuscaTaxaCrossover1.Text.ToString() + "\r\n";
            txtBuscaSolucao.Text += "                      Fim:    " + txtBuscaTaxaCrossover2.Text.ToString() + "\r\n";
            txtBuscaSolucao.Text += "                      Razão:  " + txtBuscaTaxaCrossoverRazao.Text.ToString() + "\r\n\r\n";
            txtBuscaSolucao.Text += "Taxa de Mutação:      Início: " + txtBuscaTaxaMutacao1.Text.ToString() + "\r\n";
            txtBuscaSolucao.Text += "                      Fim:    " + txtBuscaTaxaMutacao2.Text.ToString() + "\r\n";
            txtBuscaSolucao.Text += "                      Razão:  " + txtBuscaTaxaMutacaoRazao.Text.ToString() + "\r\n\r\n";
            txtBuscaSolucao.Text += "Número de Teste(s):  " + txtBuscaNumeroTeste.Text.ToString() + "\r\n\r\n\r\n";
            txtBuscaSolucao.Text += "MELHOR SOLUÇÃO:\r\n\r\n";
            txtBuscaSolucao.Text += "Maior Número de Soluções: " + melhorNumeroSolucao.ToString() + "\r\n";
            txtBuscaSolucao.Text += (melhorCrossover == 0 ? "Melhor Crossover: PMX\r\n" : "Melhor Crossover: Cíclico\r\n");
            txtBuscaSolucao.Text += (melhorSelecao == 0 ? "Melhor Seleção: Roleta\r\n" : "Melhor Seleção: Torneio 3\r\n");
            txtBuscaSolucao.Text += "Melhor Número de Gerações: " + melhorNumeroGeracao.ToString() + "\r\n";
            txtBuscaSolucao.Text += "Melhor Média de Gerações: " + melhorMediaGeracao.ToString() + "\r\n";
            txtBuscaSolucao.Text += "Melhor Tamanho da População: " + melhorTamanhoPopulacao.ToString() + "\r\n";
            txtBuscaSolucao.Text += "Melhor Taxa de Seleção: " + melhorTaxaSelecao.ToString() + "\r\n";
            txtBuscaSolucao.Text += "Melhor Taxa de Mutação: " + melhorTaxaMutacao.ToString() + "\r\n";
        }
    }
}