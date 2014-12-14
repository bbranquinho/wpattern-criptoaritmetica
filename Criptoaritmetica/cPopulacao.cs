using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Criptoaritmetica
{
    public class cPopulacao
    {
        public List<cIndividuo> populacao;
        public Int32 numeroGeracao;
        public Int32 individuoAdicionado;
        private List<Int32> sorteioIndices;
        private static Random rand = new Random(); // Precisa ser estático para ser somente 1 semente duranto a execução.

        #region Construtor
        public cPopulacao()
        {
            numeroGeracao = 0;
        }
        #endregion

        #region Criar População
        public void CriarPopulacao(String string1, String string2, String resultado, Int32 tamanhoPopulacao, Int32 tipoSelecao, Int32 tipoAptidao)
        {
            populacao = new List<cIndividuo>(tamanhoPopulacao * 2);

            for (Int32 i = 0; i < tamanhoPopulacao; i++)
            {
                populacao.Add(new cIndividuo(string1, string2, resultado));

                if (0 == i)
                {
                    populacao[0].CriaCadeia();
                }
                populacao[i].CriarIndividuo(tipoSelecao, tipoAptidao);
            }

            numeroGeracao = 1;
        }
        #endregion

        #region Critério de Parada
        /// <summary>
        /// Determina se algum indivíduo foi encontrado como solução do problema.
        /// Neste caso, o indivíduo com aptidão igual a zero representa a solução.
        /// </summary>
        /// <returns>Solução do problema, em caso de null, não foi encontarda solução.</returns>
        public cIndividuo CriterioParada()
        {
            foreach (cIndividuo individuo in populacao)
            {
                if (0 == individuo.aptidao)
                {
                    return individuo;
                }
            }

            return null;
        }
        #endregion

        #region Seleciona
        public void Seleciona(Int32 NumeroPopulacaoSelecionada, Int32 tipoSelecao)
        {
            if (0 == tipoSelecao)
            {
                Roleta(NumeroPopulacaoSelecionada);
            }
            else
            {
                Torneio(NumeroPopulacaoSelecionada);
            }
        }
        #endregion

        #region Torneio 3
        private void Torneio(Int32 NumeroPopulacaoSelecionada)
        {
            Int32 i, j, aptidaoTmp, randomico, indice;
            
            sorteioIndices = new List<Int32>(NumeroPopulacaoSelecionada);
            numeroGeracao++;

            NumeroPopulacaoSelecionada -= NumeroPopulacaoSelecionada % 2;

            for (i = 0; i < NumeroPopulacaoSelecionada; i++)
            {
                aptidaoTmp = Int32.MaxValue;
                indice     = 0;

                for (j = 0; j < 3; j++)
                {
                    randomico = rand.Next(populacao.Count);

                    if (aptidaoTmp > populacao[randomico].aptidao)
                    {
                        indice     = randomico;
                        aptidaoTmp = populacao[randomico].aptidao;
                    }
                }

                sorteioIndices.Add(indice);
            }
        }
        #endregion

        #region Roleta
        /// <summary>
        /// Determina quais são os indivíduos que devem ser selecionados.
        /// </summary>
        /// <param name="NumeroPopulacaoSelecionada">Define o número de indivíduos
        /// que devem ser selecionados para o Crossover e Mutação.</param>
        private void Roleta(Int32 NumeroPopulacaoSelecionada)
        {
            Int32 i, j;

            sorteioIndices = new List<Int32>(NumeroPopulacaoSelecionada);
            numeroGeracao++;

            // A quantidade máxima de indivíduos selecionados é toda população.
            if (NumeroPopulacaoSelecionada > populacao.Count)
            {
                NumeroPopulacaoSelecionada = populacao.Count;
            }

            NumeroPopulacaoSelecionada -= NumeroPopulacaoSelecionada % 2; // É preciso de pares de indivíduos para crossover.

            // Ordena a populacao pela aptidão (Decrescente).
            populacao.Sort(new cIndividuoComparer());

            // Acumulado "invertido".
            populacao[0].acumuladoAptidao = populacao[0].aptidao;
            for(i = 1; i < populacao.Count; i++)
            {
                populacao[i].acumuladoAptidao = populacao[i].aptidao + populacao[i - 1].acumuladoAptidao;
            }
            
            // Acumulado do Acumulado para seleção.
            for (i = 1; i < populacao.Count; i++)
            {
                populacao[i].acumuladoAptidao += populacao[i - 1].acumuladoAptidao;
            }

            for (i = 0; i < NumeroPopulacaoSelecionada; i++)
            {
                float probabilidade = (float)rand.Next(Int32.MaxValue) / (float)Int32.MaxValue;

                for (j = 0; j < populacao.Count; j++)
                {
                    float razao = ((float)populacao[j].acumuladoAptidao / (float)populacao[populacao.Count - 1].acumuladoAptidao);
                    if (probabilidade <= razao)
                    {
                        sorteioIndices.Add(j);
                        break;
                    }
                }
            }

            sorteioIndices.Sort();
        }
        #endregion

        #region Reprodução
        /// <summary>
        /// Realiza Crossover e Mutação nos indivíduos sorteados.
        /// </summary>
        /// <param name="probabilidadeMutacao">Probabilidade de ocorrer mutação nos indivíduos.</param>
        public void Reproducao(Int32 probabilidadeMutacao, Int32 tipoCrossover, Int32 tipoAptidao)
        {
            individuoAdicionado = 0;

            for (Int32 i = 0; i < sorteioIndices.Count; i += 2)
            {
                if (sorteioIndices[i] != sorteioIndices[i + 1])
                {
                    CrossoverMutacao(new cIndividuo(populacao[sorteioIndices[i]]), new cIndividuo(populacao[sorteioIndices[i + 1]]), probabilidadeMutacao, tipoCrossover, tipoAptidao);
                }
            }
        }
        #endregion

        #region Evolução
        /// <summary>
        /// Elimina os indivíduos menos aptos.
        /// </summary>
        public void Evolucao()
        {
            populacao.Sort(new cIndividuoComparer());
            populacao.RemoveRange(0, individuoAdicionado);
        }
        #endregion

        #region Crossover e Mutação
        /// <summary>
        /// Realiza Crossover entre os indivíduos "a" e "b".
        /// Possui uma probabilidade para realizar mutação em "a" e "b".
        /// </summary>
        /// <param name="a">Primeiro indivíduo.</param>
        /// <param name="b">Segundo indivíduo.</param>
        /// <param name="probabilidadeMutacao">Probabilidade de ocorrer mutação em "a" e "b".</param>
        private void CrossoverMutacao(cIndividuo a, cIndividuo b, Int32 probabilidadeMutacao, Int32 tipo, Int32 tipoAptidao)
        {
            bool contemA = false, contemB = false;

            if (0 == tipo)
            {
                CrossoverPMX(a, b);
            }
            else
            {
                CrossoverCiclico(a, b);
            }

            a = Mutacao(a, probabilidadeMutacao);
            b = Mutacao(b, probabilidadeMutacao);

            if (tipoAptidao == 0)
            {
                a.CalculaAptidaoModulo();
                b.CalculaAptidaoModulo();
            }
            else
            if (tipoAptidao == 1)
            {
                a.CalculaAptidaoDigitoSoma();
                b.CalculaAptidaoDigitoSoma();
            }
            else
            {
                a.CalculaAptidaoDigitoMultiplo();
                b.CalculaAptidaoDigitoMultiplo();
            }

            foreach (cIndividuo individuo in populacao)
            {
                if (individuo.Equals(a))
                {
                    contemA = true;
                }
                if (individuo.Equals(b))
                {
                    contemB = true;
                }
            }

            if (!contemA)
            {
                populacao.Add(a);
                individuoAdicionado++;
            }

            if (!contemB)
            {
                if (!a.Equals(b))
                {
                    populacao.Add(b);
                    individuoAdicionado++;
                }
            }

            /*
            for (Int32 x = 0; x < populacao.Count; x++)
            {
                for (Int32 y = 0; y < populacao.Count; y++)
                {
                    if (x != y && populacao[x].Equals(populacao[y]))
                    {
                        int ko = 0;
                    }
                }
            }//*/
        }
        #endregion

        #region Crossover PMX
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void CrossoverPMX(cIndividuo a, cIndividuo b)
        {
            Int32 ponto1, ponto2, tmp, i, j;
            List<Int32> lista1 = new List<Int32>(8);
            List<Int32> lista2 = new List<Int32>(8);

            // Determina os pontos de corte.
            // Os pontos máximos e mínimos podem ser usados como ponto de corte somente se não forem ao mesmo tempo.
            do
            {
                ponto1 = rand.Next(cIndividuo.quantidade);
                ponto2 = rand.Next(cIndividuo.quantidade);

                if (ponto1 > ponto2)
                {
                    tmp = ponto1;
                    ponto1 = ponto2;
                    ponto2 = tmp;
                }
            } while (ponto1 == 0 && ponto2 == cIndividuo.quantidade && cIndividuo.quantidade > 1);

            // Permuta os "genes" de acordo com os pontos de corte.
            for (i = ponto1; i <= ponto2; i++)
            {
                tmp            = a.individuo[i];
                a.individuo[i] = b.individuo[i];
                b.individuo[i] = tmp;
            }

            // Determina os "genes" que estão se repetindo no indivíduo.
            for (j = 0; j < cIndividuo.quantidade; j++)
            {
                if (j < ponto1 || j > ponto2)
                {
                    for (i = ponto1; i <= ponto2; i++)
                    {
                        if (a.individuo[i] == a.individuo[j])
                        {
                            lista1.Add(a.individuo[i]);
                        }
                        if (b.individuo[i] == b.individuo[j])
                        {
                            lista2.Add(b.individuo[i]);
                        }
                    }
                }
            }
            
            // Busca a lista de valores repetidos para completar as litas de valores para Crossover.
            while (lista1.Count != lista2.Count)
            {
                if (lista1.Count < lista2.Count)
                {
                    while (Contem(b.individuo, lista1, tmp = rand.Next(10))) ;

                    lista1.Add(tmp);
                }
                else
                {
                    while (Contem(a.individuo, lista2, tmp = rand.Next(10))) ;

                    lista2.Add(tmp);
                }
            }

            // Troca os valores das listas para os indivíduos.
            for (i = 0; i < lista1.Count; i++)
            {
                for (j = 0; j < a.individuo.Length; j++)
                {
                    if (j < ponto1 || j > ponto2)
                    {
                        if (lista1[i] == a.individuo[j])
                        {
                            a.individuo[j] = lista2[lista2.Count - i - 1];
                        }
                        if (lista2[i] == b.individuo[j])
                        {
                            b.individuo[j] = lista1[lista1.Count - i - 1];
                        }
                    }
                }
            }
        }

        private bool Contem(Int32[] individuo, List<Int32> lista, Int32 value)
        {
            for (Int32 i = 0; i < cIndividuo.quantidade; i++)
            {
                if (individuo[i] == value)
                {
                    return true;
                }
            }
            foreach (Int32 comparacao in lista)
            {
                if (comparacao == value)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Crossover Cíclico
        /// <summary>
        /// Observação: Em caso de existir valores iguais na mesma posição, nunca ocorre crossover nestes pontos.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void CrossoverCiclico(cIndividuo a, cIndividuo b)
        {
            List<Int32> ciclo = new List<Int32>(10);
            Int32 posicaoCrossover;

            posicaoCrossover = Sorteio(a, b);

            // Uma posição foi sorteada para ocorrer crossover.
            if (posicaoCrossover >= 0 && posicaoCrossover < cIndividuo.quantidade)
            {
                Int32 i = -1, j;

                ciclo.Add(a.individuo[posicaoCrossover]);
                ciclo.Add(b.individuo[posicaoCrossover]);

                while (++i < 10)
                {
                    if (a.individuo[i] == ciclo[ciclo.Count - 1])
                    {
                        if (b.individuo[i] == ciclo[0])
                        {
                            i = 10;
                        }
                        else
                        {
                            ciclo.Add(b.individuo[i]);
                            i = -1;
                        }
                    }
                }

                cIndividuo copiaA = new cIndividuo(a);
                cIndividuo copiaB = new cIndividuo(b);

                for (i = 0; i < ciclo.Count; i++)
                {
                    for (j = 0; j < 10; j++)
                    {
                        // Crossover em "a".
                        if (copiaA.individuo[j] == ciclo[i])
                        {
                            a.individuo[j] = ((i + 1) == ciclo.Count ? ciclo[0] : ciclo[i + 1]);
                        }

                        // Crossover em "b".
                        if (copiaB.individuo[j] == ciclo[i])
                        {
                            b.individuo[j] = ((i - 1) == -1 ? ciclo[ciclo.Count - 1] : ciclo[i - 1]);
                        }
                    }
                }
            }
        }

        private Int32 Sorteio(cIndividuo a, cIndividuo b)
        {
            Int32 random;
            List<Int32> valores = new List<Int32>(cIndividuo.quantidade);

            do
            {
                if (valores.Count >= cIndividuo.quantidade)
                {
                    return -1;
                }

                while (Contem(valores, random = rand.Next(cIndividuo.quantidade))) ;

                valores.Add(random);
            }
            while (a.individuo[random] == b.individuo[random] && valores.Count < 10);

            return ((valores.Count < 10) ? valores[valores.Count - 1] : - 1);
        }

        private bool Contem(List<Int32> valores, Int32 value)
        {
            foreach (Int32 comparacao in valores)
            {
                if (comparacao == value)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Mutação
        /// <summary>
        /// Realiza a mutação. Este processo troca um par de genes por indivíduo.
        /// Os genes que devem ser trocados são escolhidos aleatoriamente.
        /// Exemplo de um indivíduo: 4 1 0 2 5 8 7
        /// Exemplo de uma mutação:  4 5 0 2 1 8 7
        /// </summary>
        /// <param name="individuo">Determina qual é o indivíduo que deve fazer a mutação.</param>
        /// <param name="probabilidadeMutacao"></param>
        /// <returns></returns>
        private cIndividuo Mutacao(cIndividuo individuo, Int32 probabilidadeMutacao)
        {
            // Deve existir mais de 1 "gene" por indivíduo para ocorrer mutação.
            if (cIndividuo.quantidade > 1)
            {
                Int32 probabilidade = rand.Next(101);

                if (probabilidade < probabilidadeMutacao)
                {
                    Int32 ponto1, ponto2, tmp;

                    ponto1 = rand.Next(cIndividuo.quantidade);

                    while (ponto1 == (ponto2 = rand.Next(cIndividuo.quantidade))) ;

                    tmp                         = individuo.individuo[ponto1];
                    individuo.individuo[ponto1] = individuo.individuo[ponto2];
                    individuo.individuo[ponto2] = tmp;
                }
            }

            return new cIndividuo(individuo);
        }
        #endregion
    }
}
