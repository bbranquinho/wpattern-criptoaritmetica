using System;
using System.Collections.Generic;
using System.Text;

namespace Criptoaritmetica
{
    public class cIndividuo
    {
        public static String cadeia;
        public static Int32 quantidade;
        public static String string1;
        public static String string2;
        public static String resultado;

        private static Random rand = new Random(); // Precisa ser estático para ser somente 1 semente duranto a execução.

        public Int32[] individuo = new Int32[10] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
        public Int32 aptidao;
        public Int64 acumuladoAptidao; 

        #region Construtores
        public cIndividuo(cIndividuo ind)
        {
            for(Int32 i = 0; i < ind.individuo.Length; i++)
            {
                individuo[i] = ind.individuo[i];
            }
            aptidao = ind.aptidao;
            acumuladoAptidao = ind.acumuladoAptidao;
        }

        public cIndividuo(String str1, String str2, String res)
        {
            string1   = str1;
            string2   = str2;
            resultado = res;
        }
        #endregion

        #region Equals
        public override bool Equals(System.Object obj)
        {
            if (((cIndividuo)obj).aptidao != aptidao)
            {
                return false;
            }

            for (Int32 i = 0; i < 10; i++)
            {
                if (individuo[i] != ((cIndividuo)obj).individuo[i])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region CriaCadeia
        /// <summary>
        /// Cria a estrutura do indivíduo. Para nosso problema a String
        /// do indivíduo não se altera, somente os valores numéricos
        /// se alteram.
        /// </summary>
        /// <returns>Retorna false caso existam mais de 10 caracteres diferentes. True caso contrário.</returns>
        public bool CriaCadeia()
        {
            quantidade = 0;
            cadeia = "";

            foreach (Char caracter in (string1 + string2 + resultado))
            {
                if (!cadeia.ToString().Contains(caracter.ToString()))
                {
                    cadeia += caracter.ToString();

                    if (++quantidade > 10)
                    {
                        cadeia = "";
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion

        #region Criar Indivíduo
        /// <summary>
        /// Gera valores aleatório para o indivíduo e calcula a aptidão.
        /// Tipo de Aptidão:
        ///     1 - Módulo
        ///     2 - Dígito Soma.
        ///     3 - Dígito Múltiplo.
        /// </summary>
        public void CriarIndividuo(Int32 tipoSelecao, Int32 tipoAptidao)
        {
            Int32 i, value;
            Int32 j = (tipoSelecao == 0 ? cIndividuo.quantidade : 10);

            for (i = 0; i < j; i++)
            {
                while (contem(value = rand.Next(10))) ;

                individuo[i] = value;
            }

            if (tipoAptidao == 0)
            {
                CalculaAptidaoModulo();
            }
            else
            if (tipoAptidao == 1)
            {
                CalculaAptidaoDigitoSoma();
            }
            else
            if (tipoAptidao == 2)
            {
                CalculaAptidaoDigitoMultiplo();
            }
        }
        #endregion

        #region Calcular Aptidão Módulo da Diferença
        public void CalculaAptidaoModulo()
        {
            String s1, s2, s3;
            s1 = s2 = s3 = "";

            foreach (Char caracter in string1)
            {
                s1 += individuo[cadeia.IndexOf(caracter)].ToString();
            }
            foreach (Char caracter in string2)
            {
                s2 += individuo[cadeia.IndexOf(caracter)].ToString();
            }
            foreach (Char caracter in resultado)
            {
                s3 += individuo[cadeia.IndexOf(caracter)].ToString();
            }

            acumuladoAptidao = 0;

            aptidao = Math.Abs(Convert.ToInt32(s3) - Convert.ToInt32(s2) - Convert.ToInt32(s1));
        }
        #endregion

        #region Calcular Aptidão por Dígito (Soma)
        public void CalculaAptidaoDigitoSoma()
        {
            Int32 maxDigitos, i, dig1, dig2, dig3, sobe;

            aptidao = sobe = 0;

            // Determina o maior tamanho das strings.
            if ((maxDigitos = (string1.Length > string2.Length ? string1.Length : string2.Length)) < resultado.Length)
            {
                maxDigitos = resultado.Length;
            }

            for (i = 1; i <= maxDigitos; i++)
            {
                dig1 = dig2 = dig3 = 0;

                if (i <= string1.Length)
                {
                    dig1 = individuo[cadeia.IndexOf(string1[string1.Length - i])];
                }
                if (i <= string2.Length)
                {
                    dig2 = individuo[cadeia.IndexOf(string2[string2.Length - i])];
                }
                if (i <= resultado.Length)
                {
                    dig3 = individuo[cadeia.IndexOf(resultado[resultado.Length - i])];
                }

                aptidao += Math.Abs((dig1 + dig2 + sobe) % 10 - dig3);
                sobe = (Int32)((dig1 + dig2 + sobe) / 10);
            }
        }
        #endregion

        #region Calcular Aptidão por Dígito (Mútiplo)

        private int Nmax(int p1, int q1, int p2, int q2, int[] A, int[] B)
        {
            int i, j;

            if (p1 == q1 && p2 == q2) // p e q índices de início e fim dos vetores.
            {
                if (A[p1] > B[p2])
                {
                    return A[p1];
                }
                else
                {
                    return B[p2];
                }
            }
            else
            {
                i = (int)(q1 - p1 - 1) / 2 + p1; // Particiona A.
                j = (int)(q2 - p2 - 1) / 2 + p2; // Particiona B.
            }

            return 0;
        }

        public void CalculaAptidaoDigitoMultiplo()
        {
            Int32 maxDigitos, i, dig1, dig2, dig3, sobe, dif, aptidaoReal;

            // Determina o maior tamanho das strings.
            if ((maxDigitos = (string1.Length > string2.Length ? string1.Length : string2.Length)) < resultado.Length)
            {
                maxDigitos = resultado.Length;
            }

            aptidao = 1;
            aptidaoReal = sobe = 0;

            for (i = 1; i <= maxDigitos; i++)
            {
                dig1 = dig2 = dig3 = 0;

                if (i <= string1.Length)
                {
                    dig1 = individuo[cadeia.IndexOf(string1[string1.Length - i])];
                }
                if (i <= string2.Length)
                {
                    dig2 = individuo[cadeia.IndexOf(string2[string2.Length - i])];
                }
                if (i <= resultado.Length)
                {
                    dig3 = individuo[cadeia.IndexOf(resultado[resultado.Length - i])];
                }

                if ((dif = Math.Abs((dig1 + dig2 + sobe) % 10 - dig3)) > 0)
                {
                    aptidao *= dif;
                }
                aptidaoReal += dif;
                sobe = (Int32)((dig1 + dig2 + sobe) / 10);
            }

            if (aptidaoReal == 0)
            {
                aptidao = 0;
            }
        }
        #endregion

        #region Contem
        private bool contem(Int32 value)
        {
            foreach (Int32 comparacao in individuo)
            {
                if (comparacao == value)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
